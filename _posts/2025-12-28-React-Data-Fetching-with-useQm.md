---
layout: post
title: React Data Fetching with useQm
date: 2025-12-28
tags: react typescript frontend web-development data-fetching
categories: programming frontend
---

# React Data Fetching with useQm

In the modern React ecosystem, managing server state can often feel like a choice between two extremes: rolling your own `useEffect` logic (and all the boilerplate that comes with it) or reaching for a heavyweight library like TanStack Query.

**useQm** is something in between. **useQuery** and **useMutation** provide decoupled loading states, error handling, and type safety without the overhead of a full caching engine.

## What is useQm exactly?

`useQm` is a lightweight, zero-dependency (except React) in-place alternative to libraries like React Query. It is designed not to be installed as a package, but to be copied directly into your project. This "copy-pasteable" philosophy gives a full control over the fetching logic while providing a structured way to handle queries and mutations.

### Key Features

- **Decoupled States**: Easily access `data`, `loading`, and `problemDetails` (error details).
- **Problem Details Support**: Support for the `application/problem+json` standard.
- **Auto-abort**: Automatically cancels pending requests on unmount or new triggers.
- **Type Safe**: First-class TypeScript support out of the box.

### What is not?

- **useQm** is not a caching engine. It does not cache data.
- **useQm** is not a full-featured data fetching library. It does not provide features like pagination or infinite scrolling.
- **useQm** is not a debouncing, auto-fetching or auto-retry library.

## Getting Started: useQuery

The `useQuery` hook is for fetching data. It handles the manual `useEffect` dance and provides a clean interface for UI.

```tsx
import { useQuery } from './hooks/useQm';

interface User {
  id: string;
  name: string;
}

function UserProfile() {
  const { data: users, loading, problemDetails, query: refetch } = useQuery<User[]>('/api/users');

  if (loading) return <div>Loading users...</div>;
  if (problemDetails) return <div>Error: {problemDetails.title}</div>;

  return (
    <div>
      <h1>Users</h1>
      <ul>
        {users?.map(user => (
          <li key={user.id}>{user.name}</li>
        ))}
      </ul>
      <button onClick={() => refetch()}>Refresh List</button>
    </div>
  );
}
```

By default, `useQuery` fetches data as soon as the component mounts, but this behavior (and many other fetch options) can be configured via the optional second argument.

## Handling Actions: useMutation

When it's time to modify data—whether it's a POST, PUT, or DELETE - `useMutation` is for that. Unlike `useQuery`, mutations are triggered manually.

```tsx
import { useMutation } from './hooks/useQm';

function AddUserButton() {
  const { mutate, loading } = useMutation<User>('/api/users');

  const handleAddUser = async () => {
    const newUser = await mutate(undefined, {
      method: 'POST',
      body: JSON.stringify({ name: 'Jane Doe' }),
    });

    if (newUser) {
      alert(`User ${newUser.name} added successfully!`);
    }
  };

  return (
    <button onClick={handleAddUser} disabled={loading}>
      {loading ? 'Adding...' : 'Add User'}
    </button>
  );
}
```

## Global Configuration with QmProvider

While `useQm` works standalone, main app can be wrapped in a `QmProvider` to handle global concerns like authentication headers or centralized error tracking.

```tsx
import { QmProvider } from './hooks/useQm';

function App() {
  return (
    <QmProvider
      getAuthHeader={async () => `Bearer ${await getToken()}`}
      trackError={(err, details) => myLoggingService.log(err, details)}
    >
      <Main />
    </QmProvider>
  );
}
```

## Why useQm?

The primary strength of `useQm` is its simplicity. It doesn't try to solve every problem (like caching or debouncing).

[Example](https://github.com/ypyl/useQm/blob/master/src/App.tsx)

[Source](https://github.com/ypyl/useQm/tree/master)
