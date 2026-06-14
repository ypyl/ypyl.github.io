---
layout: presentation
---

# Architecture v2 — React SPA

Feature-Sliced Design with hard boundaries

- Layers: Pages → Features → Entities → Shared
- Strict unidirectional imports
- Queries separate from Mutations

> **Notes:** Mention this is the second iteration. v1 was Feb 2026.

---

# Project Structure

```
src/
├── app/         routing, layouts
├── shared/      reusable code
├── entities/    data models
├── pages/       route entry points
└── features/    user actions
```

Each layer only imports from below.

> **Notes:** Walk through each directory top to bottom.

---

# Import Hierarchy

| Layer ↓ / Import → | Pages | Features | Entities | Shared |
|---------------------|-------|----------|----------|--------|
| Pages               |  —    |    ✅    |    ❌    |   ✅   |
| Features            |  ❌   |    ⚠️    |    ✅    |   ✅   |
| Entities            |  ❌   |    ❌    |    —    |   ✅   |
| Shared              |  ❌   |    ❌    |    ❌    |   ✅   |

- Pages never import entities directly
- Features own entity relationships

> **Notes:** ⚠️ means hooks/types only — no UI components or api.ts imports between features

---

# API Client — Single Gateway

```
// shared/api/client.ts
// All HTTP goes through here. Never raw fetch().
```

Responsibilities:
- Auth token injection
- Error normalization → `ApiError`
- Dependency telemetry
- Correlation ID propagation

**One client, one error model.**

> **Notes:** This is the most important file in the architecture. If this is clean, everything works.

---

# Component Rules

3 core principles:

1. **Presentation-only** — no API calls, no mutations
2. **Props-driven** — receive data, don't fetch
3. **Error Boundaries** — one component crash ≠ page crash

```
// Components display states, nothing else
<OrderList
  orders={data}
  loading={isLoading}
  error={error}
/>
```

> **Notes:** Components never import api.ts. Hooks handle all data and side effects.

---

# Testing Strategy

| Layer | Test Type | Mock |
|-------|-----------|------|
| Entities | Unit | Nothing |
| Features | Integration | HTTP (MSW) |
| Pages | Component / E2E | Feature hooks |
| Shared | Unit | Nothing |

Test at the right layer. Fast feedback loops.

> **Notes:** Entities at 100% coverage. Features integration tests catch the most bugs.
