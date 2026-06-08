---
layout: post
title: "React Query Q&A: Patterns for Lists, Filters, Pagination, and More"
date: 2026-06-04
tags: react, react-query, tanstack-query, pagination, filters, typescript, frontend
categories: programming
---

A growing collection of practical React Query patterns, structured as questions and answers. Each entry addresses one concrete use case with runnable code.

---

## Query Keys & Refetching

The query key is the single most important concept in React Query — it controls when your query runs, how data is cached, and how you invalidate. These questions cover getting the key right.

---

## Q1: I have a paginated list with filters. How do I auto-refetch when filters change?

**Put the filters in the `queryKey`.** React Query treats the query key like `useEffect`'s dependency array — when any value in it changes, `queryFn` re-runs automatically.

```tsx
const [filters, setFilters] = useState({ search: '', status: 'all' });

const { data } = useQuery({
  queryKey: ['products', filters],
  queryFn: () => fetchProducts(filters),
});

// User changes filters → setFilters → queryKey changes → auto-refetch
<FilterBar filters={filters} onChange={setFilters} />
```

You don't call `refetch(newFilters)` — that's not what `refetch` is for. `refetch` re-runs the query with the *current* parameters (see Q3).

---

## Q2: The filter is a text input. How do I avoid refetching on every keystroke?

**Debounce the value before it hits the query key.** Keep the *raw* value in your `<input>` so it stays responsive, but use the *debounced* value in `queryKey`.

```tsx
function useDebounce<T>(value: T, delay: number): T {
  const [debounced, setDebounced] = useState(value);

  useEffect(() => {
    const timer = setTimeout(() => setDebounced(value), delay);
    return () => clearTimeout(timer);
  }, [value, delay]);

  return debounced;
}

// Usage
const [search, setSearch] = useState('');
const debouncedSearch = useDebounce(search, 300);

const { data } = useQuery({
  queryKey: ['items', debouncedSearch],   // ← debounced
  queryFn: () => fetchItems(debouncedSearch),
});

<Input value={search} onChange={e => setSearch(e.target.value)} />  {/* ← raw */}
```

300–500ms is a good default delay.

---

## Q3: I also need a "Refresh" button to manually refetch with the same filters.

**Use `refetch()`.** It re-runs `queryFn` with whatever is already in the `queryKey` — no parameters needed.

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

**All three go into the query key** — and reset pagination to page 1 when filters change.

```tsx
const [pagination, setPagination] = useState({ pageIndex: 0, pageSize: 20 });
const [filters, setFilters] = useState({ search: '', status: '' });
const [sorting, setSorting] = useState([{ id: 'name', desc: false }]);

const debouncedFilters = useDebounce(filters, 300);

// Reset to first page when filters change
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

**Cause**: The component rendering the input gets unmounted during loading (e.g., you return a full-page spinner when `isLoading` is true).

**Fix**: Keep the filter form outside conditional rendering. Use `isFetching` for subsequent fetches:

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

## Q6: I want to fetch only when a required dependency is ready (e.g., a category dropdown value).

**Use the `enabled` option.** The query won't run until `enabled` is `true`.

```tsx
const [selectedCategory, setSelectedCategory] = useState<string | null>(null);

const { data } = useQuery({
  queryKey: ['items', selectedCategory],
  queryFn: () => fetchItems(selectedCategory!),
  enabled: !!selectedCategory,  // ← won't fetch until a category is picked
});

<CategoryDropdown value={selectedCategory} onChange={setSelectedCategory} />
{data && <ItemList items={data} />}
```

This also handles the case where a dependency becomes `null` again — the query pauses.

---

## Pagination

Two patterns depending on the API: offset-based (page numbers) vs cursor-based (next token).

---

## Q7: Navigating between pages flashes a spinner. How to make it smooth?

**Use `placeholderData: keepPreviousData`.** React Query keeps showing the *previous* page's data while fetching the new page. The table stays visible; only a subtle indicator tells the user something is loading.

```tsx
useQuery({
  queryKey: ['todos', 'list', { page: pagination.pageIndex }],
  queryFn: () => fetchTodos({ page: pagination.pageIndex }),
  placeholderData: keepPreviousData,
});

// In your UI, use isFetching (not isLoading) for background updates:
{isFetching && <LoadingBar />}       // ← subtle, doesn't unmount the table
{!data && isFetching && <Spinner />}  // ← full spinner only on initial load
```

`isFetching` is `true` for *any* fetch (initial + background). `isLoading` is `true` only on the first fetch when there's no cached data.

---

## Q8: My API uses cursor / next-token pagination (not page numbers). How do I support that?

**Use `useInfiniteQuery`.** Offset-based pagination (page 1, 2, 3…) works with `useQuery` + `placeholderData` (see Q7). Cursor-based pagination (next token, next cursor) is what `useInfiniteQuery` was built for.

### What the API should return

The server response must include the cursor/token for the next page alongside the data:

```json
// GET /api/items?cursor=0
{ "data": [...], "nextCursor": "abc123" }

// GET /api/items?cursor=abc123
{ "data": [...], "nextCursor": null }   // null = no more pages
```

### "Load More" button pattern

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
    getNextPageParam: (lastPage) => lastPage.nextCursor,  // ← return null/undefined when done
  })

  if (status === 'pending') return <p>Loading…</p>

  return (
    <>
      {/* Flatten all pages into one list */}
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

### Infinite scroll pattern (Intersection Observer)

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

  // Sentinel ref — when this div enters the viewport, load more
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
      { rootMargin: '200px' }  // ← trigger 200px before the sentinel is visible
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
      {/* Sentinel at the bottom */}
      <div ref={loadMoreRef}>
        {isFetchingNextPage && <Spinner />}
      </div>
    </>
  )
}
```

### When the API uses offset params instead of a cursor

If your API uses `?page=1&limit=20` instead of a cursor, you can still use `useInfiniteQuery` — just treat the page number as the "cursor":

```tsx
useInfiniteQuery({
  queryKey: ['items'],
  queryFn: ({ pageParam }) => fetch(`/api/items?page=${pageParam}&limit=20`),
  initialPageParam: 1,
  getNextPageParam: (lastPage, allPages) => {
    if (lastPage.data.length < 20) return undefined  // no more
    return allPages.length + 1                        // next page number
  },
})
```

### How `useInfiniteQuery` refetches

When cached infinite query data goes stale, **each page is refetched sequentially starting from the first**. This prevents stale cursors from causing duplicates or skipped records. If the cache is fully removed, pagination restarts from `initialPageParam`.

### Comparison: `useQuery` vs `useInfiniteQuery` for pagination

| | `useQuery` + `placeholderData` | `useInfiniteQuery` |
|---|---|---|
| Best for | Offset pagination (page numbers, filters) | Cursor/token-based, infinite scroll, "load more" |
| Data shape | Single response per query key | Accumulated `data.pages[]` array |
| Previous data visible | Yes (via `placeholderData`) | Yes (all fetched pages stay) |
| Navigation | Jump to any page | Append/prepend only |
| Refetch behavior | One request with current params | Re-fetches all pages sequentially |

---

## Query Key Architecture & Cache Updates

Once you have multiple queries for the same feature, shared key structure and cache invalidation become critical.

---

## Q9: How should I structure query keys for a feature with lists and details?

**Generic → specific, using a query key factory.** This enables bulk invalidation at any level of granularity:

```tsx
const todoKeys = {
  all:     ['todos'] as const,
  lists:   () => [...todoKeys.all, 'list'] as const,
  list:    (filters: Filters) => [...todoKeys.lists(), { filters }] as const,
  details: () => [...todoKeys.all, 'detail'] as const,
  detail:  (id: number) => [...todoKeys.details(), id] as const,
};

// Invalidate everything todo-related
queryClient.invalidateQueries({ queryKey: todoKeys.all });

// Invalidate only lists
queryClient.invalidateQueries({ queryKey: todoKeys.lists() });

// Invalidate one specific detail
queryClient.invalidateQueries({ queryKey: todoKeys.detail(42) });
```

Co-locate the factory in the feature directory (e.g., `features/todos/queries.ts`), not in a global constants file.

---

## Q10: After a mutation (create/update/delete), the list is stale. How to fix?

**Invalidate the list query** in the mutation's `onSuccess`. Use structured query keys so you can target the right scope:

```tsx
// Query key factory (see Q9)
const todoKeys = {
  all:     ['todos'] as const,
  lists:   () => [...todoKeys.all, 'list'] as const,
  detail:  (id: number) => [...todoKeys.all, 'detail', id] as const,
};

// Mutations
useMutation({
  mutationFn: deleteTodo,
  onSuccess: () => {
    queryClient.invalidateQueries({ queryKey: todoKeys.lists() });
  },
});

useMutation({
  mutationFn: updateTodo,
  onSuccess: (updated) => {
    // Optimistic: update the detail cache directly
    queryClient.setQueryData(todoKeys.detail(updated.id), updated);
    // Invalidate lists so they refetch
    queryClient.invalidateQueries({ queryKey: todoKeys.lists() });
  },
});
```

---

## Putting It All Together

---

## Q11: I've learned the individual patterns. How do they all fit together in a real app?

A full walkthrough wiring up auth, feature flags, cursor-paginated lists with filters/sorting/search, detail pages, file downloads, and mutations — all in one app:

→ **[React Query App Scenario — End-to-End Data Fetching Walkthrough]({% post_url 2026-06-07-react-query-app-scenario %})**

It covers wiring up MSAL token injection, `useAuthQuery` wrappers, `useInfiniteQuery` for the item table, debounced search, sortable headers, detail page fetching, binary file downloads outside React Query, and feedback mutations with cache invalidation — all in runnable code.

---

*More Q&As to be added as cases come up.*
