---
layout: post
title: "React Query Q&A: Patterns for Lists, Filters, Pagination, and More"
date: 2026-06-04
tags: react, react-query, tanstack-query, pagination, filters, typescript, frontend
categories: programming
---

A growing collection of practical React Query patterns, structured as questions and answers. Each entry addresses one concrete use case with runnable code.

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

## Q5: Navigating between pages flashes a spinner. How to make it smooth?

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

## Q6: The filter input loses focus on every keystroke. Why?

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

## Q7: After a mutation (create/update/delete), the list is stale. How to fix?

**Invalidate the list query** in the mutation's `onSuccess`. Use structured query keys so you can target the right scope:

```tsx
// Query key factory (see Q8)
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

## Q8: How should I structure query keys for a feature with lists and details?

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

## Q9: I want to fetch only when a required dependency is ready (e.g., a category dropdown value).

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

*More Q&As to be added as cases come up.*
