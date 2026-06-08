---
layout: post
title: "React Query Q&A: Patterns for Lists, Filters, Pagination, and More"
date: 2026-06-08
tags: [react, react-query, tanstack-query, pagination, typescript, frontend, javascript, state-management]
categories: programming
---

Practical React Query patterns as direct questions and answers. Each entry addresses one concrete use case with runnable code.

---

- [Q1: I have a paginated list with filters. How do I auto-refetch when filters change?](#q1)
- [Q2: The filter is a text input. How do I avoid refetching on every keystroke?](#q2)
- [Q3: I also need a "Refresh" button to manually refetch with the same filters.](#q3)
- [Q4: I have pagination + filters + sorting. How to combine them in one query?](#q4)
- [Q5: The filter input loses focus on every keystroke. Why?](#q5)
- [Q6: I want to fetch only when a required dependency is ready.](#q6)
- [Q7: Navigating between pages flashes a spinner. How to make it smooth?](#q7)
- [Q8: My API uses cursor / next-token pagination. How do I support that?](#q8)
- [Q9: How should I structure query keys for a feature with lists and details?](#q9)
- [Q10: After a mutation, the list is stale. How to fix?](#q10)
- [Q11: One query needs data from another query's result. How do I chain them?](#q11)
- [Q12: How do I subscribe to query/mutation lifecycle events (start, success, failure)?](#q12)

---

## Q1: I have a paginated list with filters. How do I auto-refetch when filters change?
{: #q1}

Put the filters in the `queryKey`. When any value in the key changes, `queryFn` re-runs automatically.

```tsx
const [filters, setFilters] = useState({ search: '', status: 'all' });

const { data } = useQuery({
  queryKey: ['products', filters],
  queryFn: () => fetchProducts(filters),
});

<FilterBar filters={filters} onChange={setFilters} />
```

---

## Q2: The filter is a text input. How do I avoid refetching on every keystroke?
{: #q2}

Debounce the value before it hits the query key. Keep the raw value in the input, use the debounced value in `queryKey`.

```tsx
function useDebounce<T>(value: T, delay: number): T {
  const [debounced, setDebounced] = useState(value);

  useEffect(() => {
    const timer = setTimeout(() => setDebounced(value), delay);
    return () => clearTimeout(timer);
  }, [value, delay]);

  return debounced;
}

const [search, setSearch] = useState('');
const debouncedSearch = useDebounce(search, 300);

const { data } = useQuery({
  queryKey: ['items', debouncedSearch],
  queryFn: () => fetchItems(debouncedSearch),
});

<Input value={search} onChange={e => setSearch(e.target.value)} />
```

---

## Q3: I also need a "Refresh" button to manually refetch with the same filters.
{: #q3}

Call `refetch()`. It re-runs `queryFn` with the current `queryKey`.

```tsx
const { data, isFetching, refetch } = useQuery({
  queryKey: ['items', { filters, pagination }],
  queryFn: () => fetchItems({ filters, pagination }),
});

<button onClick={() => refetch()} disabled={isFetching}>
  {isFetching ? 'Refreshing…' : '🔄 Refresh'}
</button>
```

| Trigger | Mechanism | When |
|---|---|---|
| Filters/pagination change | `queryKey` change → auto | User interacts with UI |
| Refresh button | `refetch()` → same key | User wants latest data |

---

## Q4: I have pagination + filters + sorting. How to combine them in one query?
{: #q4}

Put all three into the query key and reset pagination to page 1 when filters change.

```tsx
const [pagination, setPagination] = useState({ pageIndex: 0, pageSize: 20 });
const [filters, setFilters] = useState({ search: '', status: '' });
const [sorting, setSorting] = useState([{ id: 'name', desc: false }]);

const debouncedFilters = useDebounce(filters, 300);

useEffect(() => {
  setPagination(p => ({ ...p, pageIndex: 0 }));
}, [debouncedFilters]);

const { data, isFetching } = useQuery({
  queryKey: ['todos', 'list', { ...debouncedFilters, ...pagination, sorting }],
  queryFn: () => fetchTodos({ ...debouncedFilters, ...pagination, sorting }),
  placeholderData: keepPreviousData,
});
```

---

## Q5: The filter input loses focus on every keystroke. Why?
{: #q5}

The component rendering the input gets unmounted during loading (e.g., a full-page spinner on `isLoading`). Keep the filter form outside conditional rendering, and use `isFetching` for subsequent fetches.

```tsx
// ❌ Input unmounts on every refetch
if (isLoading) return <Spinner />;
return <><FilterForm /><DataTable /></>;

// ✅ FilterForm always stays mounted
return (
  <>
    <FilterForm filters={filters} onChange={setFilters} />
    {isFetching && <LoadingOverlay />}
    <DataTable data={data} />
  </>
);
```

---

## Q6: I want to fetch only when a required dependency is ready.
{: #q6}

Use `enabled`. The query won't run until it's `true`.

```tsx
const [selectedCategory, setSelectedCategory] = useState<string | null>(null);

const { data } = useQuery({
  queryKey: ['items', selectedCategory],
  queryFn: () => fetchItems(selectedCategory!),
  enabled: !!selectedCategory,
});

<CategoryDropdown value={selectedCategory} onChange={setSelectedCategory} />
{data && <ItemList items={data} />}
```

---

## Q7: Navigating between pages flashes a spinner. How to make it smooth?
{: #q7}

Use `placeholderData: keepPreviousData`. The previous page's data stays visible while the new page fetches.

```tsx
useQuery({
  queryKey: ['todos', 'list', { page: pagination.pageIndex }],
  queryFn: () => fetchTodos({ page: pagination.pageIndex }),
  placeholderData: keepPreviousData,
});

// use isFetching (not isLoading) for background updates:
{isFetching && <LoadingBar />}
{!data && isFetching && <Spinner />}  // full spinner only on initial load
```

---

## Q8: My API uses cursor / next-token pagination. How do I support that?
{: #q8}

Use `useInfiniteQuery`.

**API response shape** — the server must include the next cursor:

```json
// GET /api/items?cursor=0
{ "data": [...], "nextCursor": "abc123" }

// GET /api/items?cursor=abc123
{ "data": [...], "nextCursor": null }
```

**"Load More" button:**

```tsx
import { useInfiniteQuery } from '@tanstack/react-query'

function ItemList() {
  const fetchItems = async ({ pageParam }) => {
    const res = await fetch(`/api/items?cursor=${pageParam}`)
    return res.json()
  }

  const {
    data,
    fetchNextPage,
    hasNextPage,
    isFetchingNextPage,
    status,
  } = useInfiniteQuery({
    queryKey: ['items'],
    queryFn: fetchItems,
    initialPageParam: 0,
    getNextPageParam: (lastPage) => lastPage.nextCursor,
  })

  if (status === 'pending') return <p>Loading…</p>

  return (
    <>
      {data.pages.map((page) =>
        page.data.map((item) => <ItemCard key={item.id} item={item} />)
      )}

      <button
        onClick={() => fetchNextPage()}
        disabled={!hasNextPage || isFetchingNextPage}
      >
        {isFetchingNextPage
          ? 'Loading more…'
          : hasNextPage
            ? 'Load More'
            : 'Nothing more to load'}
      </button>
    </>
  )
}
```

**Infinite scroll (Intersection Observer):**

```tsx
function InfiniteItemList() {
  const {
    data,
    fetchNextPage,
    hasNextPage,
    isFetchingNextPage,
    status,
  } = useInfiniteQuery({
    queryKey: ['items'],
    queryFn: fetchItems,
    initialPageParam: 0,
    getNextPageParam: (lastPage) => lastPage.nextCursor,
  })

  const loadMoreRef = useRef<HTMLDivElement>(null)

  useEffect(() => {
    const el = loadMoreRef.current
    if (!el) return

    const observer = new IntersectionObserver(
      ([entry]) => {
        if (entry.isIntersecting && hasNextPage && !isFetchingNextPage) {
          fetchNextPage()
        }
      },
      { rootMargin: '200px' }
    )

    observer.observe(el)
    return () => observer.disconnect()
  }, [hasNextPage, isFetchingNextPage, fetchNextPage])

  if (status === 'pending') return <p>Loading…</p>

  return (
    <>
      {data.pages.map((page) =>
        page.data.map((item) => <ItemCard key={item.id} item={item} />)
      )}
      <div ref={loadMoreRef}>
        {isFetchingNextPage && <Spinner />}
      </div>
    </>
  )
}
```

**Offset-based API?** Treat the page number as the cursor:

```tsx
useInfiniteQuery({
  queryKey: ['items'],
  queryFn: ({ pageParam }) => fetch(`/api/items?page=${pageParam}&limit=20`),
  initialPageParam: 1,
  getNextPageParam: (lastPage, allPages) => {
    if (lastPage.data.length < 20) return undefined
    return allPages.length + 1
  },
})
```

| | `useQuery` + `placeholderData` | `useInfiniteQuery` |
|---|---|---|
| Best for | Offset pagination | Cursor-based, infinite scroll, "load more" |
| Data shape | Single response per key | Accumulated `data.pages[]` |
| Previous data visible | Yes (`placeholderData`) | Yes (all pages stay) |
| Navigation | Jump to any page | Append/prepend only |
| Refetch behavior | One request | Re-fetches all pages sequentially |

---

## Q9: How should I structure query keys for a feature with lists and details?
{: #q9}

Use a query key factory: generic → specific. Enables bulk invalidation at any level.

```tsx
const todoKeys = {
  all:     ['todos'] as const,
  lists:   () => [...todoKeys.all, 'list'] as const,
  list:    (filters: Filters) => [...todoKeys.lists(), { filters }] as const,
  details: () => [...todoKeys.all, 'detail'] as const,
  detail:  (id: number) => [...todoKeys.details(), id] as const,
};

// Invalidate all todo queries
queryClient.invalidateQueries({ queryKey: todoKeys.all });

// Invalidate only lists
queryClient.invalidateQueries({ queryKey: todoKeys.lists() });

// Invalidate one detail
queryClient.invalidateQueries({ queryKey: todoKeys.detail(42) });
```

---

## Q10: After a mutation, the list is stale. How to fix?
{: #q10}

Invalidate the list query in the mutation's `onSuccess`. Combine with `setQueryData` for instant detail updates.

```tsx
const todoKeys = {
  all:     ['todos'] as const,
  lists:   () => [...todoKeys.all, 'list'] as const,
  detail:  (id: number) => [...todoKeys.all, 'detail', id] as const,
};

useMutation({
  mutationFn: deleteTodo,
  onSuccess: () => {
    queryClient.invalidateQueries({ queryKey: todoKeys.lists() });
  },
});

useMutation({
  mutationFn: updateTodo,
  onSuccess: (updated) => {
    queryClient.setQueryData(todoKeys.detail(updated.id), updated);
    queryClient.invalidateQueries({ queryKey: todoKeys.lists() });
  },
});
```

---

## Q11: One query needs data from another query's result. How do I chain them?
{: #q11}

Gate the downstream query with `enabled` on the upstream result.

```tsx
// Step 1: get the user
const { data: user } = useQuery({
  queryKey: ['user', email],
  queryFn: () => getUserByEmail(email),
});

const userId = user?.id;

// Step 2: get the user's projects — only runs once userId exists
const { data: projects, status, fetchStatus } = useQuery({
  queryKey: ['projects', userId],
  queryFn: () => getProjectsByUser(userId!),
  enabled: !!userId,
});
```

The downstream query transitions through three states:

| user query | userId | projects `status` | projects `fetchStatus` |
|---|---|---|---|
| fetching | `undefined` | `pending` | `idle` |
| success | 42 | `pending` | `fetching` |
| success | 42 | `success` | `idle` |

Distinguish "waiting for dependency" from "fetching":

```tsx
if (status === 'pending' && fetchStatus === 'idle') return <p>Waiting…</p>;
if (status === 'pending' && fetchStatus === 'fetching') return <p>Loading…</p>;
```

**useQueries** when the upstream returns multiple values:

```tsx
const { data: userIds } = useQuery({
  queryKey: ['users'],
  queryFn: getUsersData,
  select: (users) => users.map((user) => user.id),
});

const usersMessages = useQueries({
  queries: userIds
    ? userIds.map((id) => ({
        queryKey: ['messages', id],
        queryFn: () => getMessagesByUser(id),
      }))
    : [],
});
```

`useQueries` returns an array of query results. While `userIds` is `undefined`, `queries` is `[]` — no downstream queries run.

**Performance note**: dependent queries create a request waterfall (serial, not parallel). Before chaining, consider adding a combined backend endpoint to avoid the extra round-trip.

---

## Q12: How do I subscribe to query/mutation lifecycle events (start, success, failure)?
{: #q12}

Use the global callbacks on `QueryCache` and `MutationCache`, passed to `QueryClient`. These fire **once per query/mutation** (not per observer), making them the right place for cross-cutting concerns like logging or tracing.

### Mutations — all three phases are built-in

```tsx
const queryClient = new QueryClient({
  mutationCache: new MutationCache({
    onMutate: (variables) => {
      // mutation started
    },
    onSuccess: (data, variables) => {
      // mutation succeeded
    },
    onError: (error, variables) => {
      // mutation failed
    },
    onSettled: (data, error, variables) => {
      // final outcome (success or error)
    },
  }),
});
```

### Queries — success and failure are built-in, "started" needs `subscribe()`

The `QueryCache` has `onSuccess`, `onError`, and `onSettled` — but no `onStart`. To capture the fetch start, watch for the `fetchStatus` transition via `subscribe()`:

```tsx
const queryCache = new QueryCache({
  onSuccess: (data, query) => {
    // query succeeded
  },
  onError: (error, query) => {
    // query failed after all retries
  },
  onSettled: (data, error, query) => {
    // final outcome (success or error)
  },
});

// "Started" — only via subscribe()
const seen = new Set<string>();

queryCache.subscribe((event) => {
  if (
    event.type === 'updated' &&
    event.query.state.fetchStatus === 'fetching' &&
    !seen.has(event.query.queryHash)
  ) {
    seen.add(event.query.queryHash);
    // query fetch started
  }
  if (
    event.query.state.fetchStatus === 'idle' &&
    (event.query.state.status === 'success' || event.query.state.status === 'error')
  ) {
    seen.delete(event.query.queryHash);
  }
});

const queryClient = new QueryClient({ queryCache });
```

`subscribe()` fires on every cache update (`added`, `removed`, `updated`, `observerAdded`, etc). Filter by `event.type === 'updated'` and `fetchStatus === 'fetching'` to detect start. The `seen` set prevents duplicate "started" events across retries — remove it if you want per-retry granularity.

### Per-query metadata via `meta`

Pass extra context from individual queries/mutations to the global callbacks:

```tsx
useQuery({
  queryKey: ['items'],
  queryFn: fetchItems,
  meta: { operationName: 'fetch-items' },
});

// In the global callback:
// query.meta.operationName === 'fetch-items'
```

| Event | Query | Mutation |
|---|---|---|
| Started | `queryCache.subscribe()` + `fetchStatus` check | `MutationCache.onMutate` |
| Succeeded | `QueryCache.onSuccess` | `MutationCache.onSuccess` |
| Failed | `QueryCache.onError` | `MutationCache.onError` |
| Settled | `QueryCache.onSettled` | `MutationCache.onSettled` |

**Why no `onStart` for queries?** Queries are reactive — they fire from many triggers (mount, focus, `refetch()`, invalidation). The library exposes only the final outcome (`onSuccess`/`onError`) and leaves `subscribe()` as the escape hatch for those who need the start event, rather than picking a definition of "started" that may not fit every use case.
