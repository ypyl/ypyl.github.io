---
layout: post
title: "Tanner Linsley Reimplements React's API with 80% Less Code"
date: 2026-06-09
tags: news
categories: news
---

**Tanner Linsley**, creator of TanStack, has published a deep-dive on **reimplementing React's public API** from scratch while dropping most of its internal machinery. His custom implementation strips out concurrent rendering, lane-based scheduling, React DevTools, and the Flight client deserializer, with `useTransition` and `useDeferredValue` running synchronously and `startTransition` as a simple `fn()` wrapper over microtasks. The result passes all 200 React tests, achieves **2× better performance** on benchmarks, and ships at **80% smaller bundle size**—proving that much of React's weight comes from implementation choices rather than its developer-facing API contracts.

[Projecting React](https://tannerlinsley.com/posts/projecting-react)
