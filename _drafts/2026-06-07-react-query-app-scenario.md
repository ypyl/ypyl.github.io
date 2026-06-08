---
layout: post
title: "React Query App Scenario — End-to-End Data Fetching Walkthrough"
date: 2026-06-07
tags: react, react-query, tanstack-query, msal, authentication, pagination, typescript, frontend
categories: programming
---

A real app scenario showing how each backend request fits into React Query end to end: MSAL auth → feature flags → cursor-paginated item list with filters, sorting, and search → item detail → file downloads → feedback mutations. Questions will be extracted from this into the main Q&A collection.

---

## App roadmap

```
Login (MSAL) → Feature flags → Item list (cursor pagination + filters + sort + search)
                                      │
                                      └── Click item → Detail page (GET item, GET files)
                                                          │
                                                          ├── Download file
                                                          ├── Submit feedback (POST)
                                                          └── Edit feedback (PUT)
```

---

## 1. Setup: QueryClient with MSAL token injection

Wrap the app in both `MsalProvider` and `QueryClientProvider`. The key piece: every `queryFn` needs the access token. Use a custom hook so you don't repeat the token logic per query:

```tsx
// hooks/useAuthenticatedQuery.ts
import { useQuery, useInfiniteQuery } from '@tanstack/react-query'
import { useMsal } from '@azure/msal-react'

function useAccessToken() {
  const { instance, accounts } = useMsal()

  return async () => {
    const result = await instance.acquireTokenSilent({
      scopes: ['api://your-app/access_as_user'],
      account: accounts[0],
    })
    return result.accessToken
  }
}

// Thin wrappers that inject the token before the real queryFn
export function useAuthQuery({ queryKey, queryFn, ...options }) {
  const getToken = useAccessToken()

  return useQuery({
    queryKey,
    queryFn: async (ctx) => {
      const token = await getToken()
      return queryFn({ ...ctx, token })
    },
    ...options,
  })
}

export function useAuthInfiniteQuery({ queryKey, queryFn, ...options }) {
  const getToken = useAccessToken()

  return useInfiniteQuery({
    queryKey,
    queryFn: async (ctx) => {
      const token = await getToken()
      return queryFn({ ...ctx, token })
    },
    ...options,
  })
}
```

`queryFn` receives `token` in the context — call your API with `Authorization: Bearer ${token}`.

---

## 2. Feature flags — loaded once, cached for the session

```tsx
// features/flags/queries.ts
export const flagKeys = {
  all: ['flags'] as const,
}

export function useFeatureFlags() {
  const { accounts } = useMsal()

  return useAuthQuery({
    queryKey: flagKeys.all,
    queryFn: ({ token }) =>
      fetch('/api/features', {
        headers: { Authorization: `Bearer ${token}` },
      }).then((r) => r.json()),
    staleTime: Infinity,      // ← don't refetch during the session
    enabled: !!accounts[0],   // ← wait for login
  })
}

// Usage in a layout component
function AppLayout() {
  const { data: flags, isLoading } = useFeatureFlags()

  if (isLoading) return <FullPageSpinner />

  return (
    <>
      {flags?.showDashboard && <DashboardNav />}
      {flags?.showReports  && <ReportsNav />}
      <Outlet />
    </>
  )
}
```

---

## 3. Item list — cursor pagination + filters + sorting + search

All parameters go into the query key so they trigger automatic refetching. Use `useInfiniteQuery` for the cursor-based backend.

```tsx
// features/items/queries.ts
export const itemKeys = {
  all:     ['items'] as const,
  lists:   () => [...itemKeys.all, 'list'] as const,
  list:    (params: ItemListParams) => [...itemKeys.lists(), params] as const,
  details: () => [...itemKeys.all, 'detail'] as const,
  detail:  (id: string) => [...itemKeys.details(), id] as const,
}

interface ItemListParams {
  filters: { status?: string; category?: string }
  sorting: { field: string; direction: 'asc' | 'desc' }
  search: string
}

function useItemList(params: ItemListParams) {
  const debouncedParams = useDebounce(params, 300)

  return useAuthInfiniteQuery({
    queryKey: itemKeys.list(debouncedParams),
    queryFn: async ({ pageParam, token }) => {
      const url = new URL('/api/items', window.location.origin)
      url.searchParams.set('cursor', String(pageParam ?? 0))
      if (debouncedParams.filters.status)   url.searchParams.set('status', debouncedParams.filters.status)
      if (debouncedParams.filters.category) url.searchParams.set('category', debouncedParams.filters.category)
      if (debouncedParams.search)           url.searchParams.set('q', debouncedParams.search)
      url.searchParams.set('sort', debouncedParams.sorting.field)
      url.searchParams.set('order', debouncedParams.sorting.direction)

      const res = await fetch(url, {
        headers: { Authorization: `Bearer ${token}` },
      })
      return res.json()
      // Response shape: { data: Item[], nextCursor: string | null }
    },
    initialPageParam: 0,
    getNextPageParam: (lastPage) => lastPage.nextCursor,
  })
}
```

**Debouncing** handles the search input without refetching on every keystroke. Filters, sorting, and search all live in the same `params` object — when any of them change, the query key changes and a fresh infinite query starts from `cursor=0`.

```tsx
// features/items/ItemListPage.tsx
function ItemListPage() {
  const [filters, setFilters] = useState({ status: '', category: '' })
  const [sorting, setSorting] = useState({ field: 'createdAt', direction: 'desc' as const })
  const [search, setSearch] = useState('')

  const params = { filters, sorting, search }

  const {
    data,
    fetchNextPage,
    hasNextPage,
    isFetchingNextPage,
    status,
  } = useItemList(params)

  // Infinite scroll sentinel
  const loadMoreRef = useRef<HTMLDivElement>(null)
  useEffect(() => {
    const el = loadMoreRef.current
    if (!el) return
    const obs = new IntersectionObserver(
      ([e]) => { if (e.isIntersecting && hasNextPage && !isFetchingNextPage) fetchNextPage() },
      { rootMargin: '200px' }
    )
    obs.observe(el)
    return () => obs.disconnect()
  }, [hasNextPage, isFetchingNextPage, fetchNextPage])

  if (status === 'pending') return <Spinner />

  return (
    <>
      <SearchBar value={search} onChange={setSearch} />
      <FilterBar filters={filters} onChange={setFilters} />
      <table>
        <thead>
          <SortableHeader field="name"       sorting={sorting} onSort={setSorting} />
          <SortableHeader field="createdAt"  sorting={sorting} onSort={setSorting} />
        </thead>
        <tbody>
          {data.pages.map((page) =>
            page.data.map((item) => (
              <tr key={item.id}>
                <td><Link to={`/items/${item.id}`}>{item.name}</Link></td>
                <td>{item.createdAt}</td>
              </tr>
            ))
          )}
        </tbody>
      </table>
      <div ref={loadMoreRef}>
        {isFetchingNextPage && <Spinner />}
      </div>
    </>
  )
}

// Reusable sortable column header
function SortableHeader({ field, sorting, onSort, children }) {
  const isActive = sorting.field === field
  return (
    <th onClick={() => onSort({ field, direction: isActive && sorting.direction === 'asc' ? 'desc' : 'asc' })}>
      {children} {isActive ? (sorting.direction === 'asc' ? '↑' : '↓') : ''}
    </th>
  )
}
```

> **Why cursor params + filters in the query key?** Changing a filter resets the key entirely, so `useInfiniteQuery` discards old pages and starts fresh from `cursor=0`. No manual page reset needed — the query key change handles it.

---

## 4. Item detail — fetched by ID, separate endpoint

```tsx
// features/items/queries.ts (continued)
function useItemDetail(id: string) {
  return useAuthQuery({
    queryKey: itemKeys.detail(id),
    queryFn: ({ token }) =>
      fetch(`/api/items/${id}`, {
        headers: { Authorization: `Bearer ${token}` },
      }).then((r) => r.json()),
    enabled: !!id,
  })
}

// features/items/ItemDetailPage.tsx
function ItemDetailPage() {
  const { id } = useParams<{ id: string }>()
  const { data: item, status } = useItemDetail(id!)

  if (status === 'pending') return <Spinner />
  if (status === 'error')  return <Error />

  return (
    <>
      <h1>{item.name}</h1>
      <ItemFiles itemId={item.id} />
      <ItemFeedback itemId={item.id} />
    </>
  )
}
```

---

## 5. File downloads — not React Query's job

React Query stores serializable JSON. File downloads (binary blobs) are better handled outside the cache:

```tsx
// features/items/ItemFiles.tsx
function ItemFiles({ itemId }: { itemId: string }) {
  const getToken = useAccessToken()   // from the MSAL hook defined earlier

  const downloadFile = async (fileId: string, fileName: string) => {
    const token = await getToken()
    const res = await fetch(`/api/items/${itemId}/files/${fileId}`, {
      headers: { Authorization: `Bearer ${token}` },
    })
    const blob = await res.blob()

    // Trigger browser download
    const url = URL.createObjectURL(blob)
    const a = document.createElement('a')
    a.href = url
    a.download = fileName
    a.click()
    URL.revokeObjectURL(url)
  }

  // File metadata (names, sizes) lives in the item detail response or a separate query
  return (
    <ul>
      {item.files?.map((file) => (
        <li key={file.id}>
          <button onClick={() => downloadFile(file.id, file.name)}>
            📥 {file.name} ({(file.size / 1024).toFixed(0)} KB)
          </button>
        </li>
      ))}
    </ul>
  )
}
```

> If you need to cache file metadata independently (e.g., file list changes), add a `useQuery` for `['items', itemId, 'files']` and only use the direct `fetch` for the actual download.

---

## 6. Feedback — create and edit mutations

```tsx
// features/items/ItemFeedback.tsx
function ItemFeedback({ itemId }: { itemId: string }) {
  const queryClient = useQueryClient()
  const getToken = useAccessToken()
  const [draft, setDraft] = useState('')

  // Submit new feedback
  const createFeedback = useMutation({
    mutationFn: async (text: string) => {
      const token = await getToken()
      return fetch(`/api/items/${itemId}/feedback`, {
        method: 'POST',
        headers: {
          Authorization: `Bearer ${token}`,
          'Content-Type': 'application/json',
        },
        body: JSON.stringify({ text }),
      }).then((r) => r.json())
    },
    onSuccess: () => {
      queryClient.invalidateQueries({ queryKey: itemKeys.detail(itemId) })  // refetch detail (includes feedback)
      setDraft('')
    },
  })

  // Edit existing feedback
  const updateFeedback = useMutation({
    mutationFn: async ({ feedbackId, text }: { feedbackId: string; text: string }) => {
      const token = await getToken()
      return fetch(`/api/items/${itemId}/feedback/${feedbackId}`, {
        method: 'PUT',
        headers: {
          Authorization: `Bearer ${token}`,
          'Content-Type': 'application/json',
        },
        body: JSON.stringify({ text }),
      }).then((r) => r.json())
    },
    onSuccess: () => {
      queryClient.invalidateQueries({ queryKey: itemKeys.detail(itemId) })
    },
  })

  return (
    <div>
      <h3>Feedback</h3>
      <textarea value={draft} onChange={(e) => setDraft(e.target.value)} />
      <button
        onClick={() => createFeedback.mutate(draft)}
        disabled={!draft.trim() || createFeedback.isPending}
      >
        Submit
      </button>

      <ul>
        {item.feedback?.map((fb) => (
          <li key={fb.id}>
            {fb.text}
            <button onClick={() => {
              const text = prompt('Edit:', fb.text)
              if (text) updateFeedback.mutate({ feedbackId: fb.id, text })
            }}>
              Edit
            </button>
          </li>
        ))}
      </ul>
    </div>
  )
}
```

---

## Design decisions recap

| Concern | Decision | Why |
|---|---|---|
| Auth token in queries | Custom `useAuthQuery` wrapper, token fetched inside `queryFn` | Token is available in the closure at query time; no stale token pre-fetched |
| Feature flags | `staleTime: Infinity`, `enabled: !!account` | Flags don't change during a session; wait for login |
| Item list | `useInfiniteQuery` with all params in the query key | Cursor pagination is what `useInfiniteQuery` is for; key change resets to page 1 automatically |
| Search input | Debounced before the query key | Prevents refetch-on-keystroke; raw value stays in the `<input>` |
| File downloads | Direct `fetch` + `URL.createObjectURL`, not React Query | React Query is for serializable JSON, not binary blobs |
| Feedback mutations | `useMutation` + `invalidateQueries` on the detail key | Keeps the detail page fresh without manual cache surgery |
