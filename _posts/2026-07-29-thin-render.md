---
layout: post
title: "Thin-render: A ~700-line React Renderer with Per-path Re-renders"
date: 2026-07-29
tags: [react, typescript, open-source]
categories: programming
---

**thin-render** is a spec-driven React renderer that avoids cascade re-renders by subscribing leaf components to individual store paths via `useSyncExternalStore`, rather than re-rendering the entire tree on any state change. Edit one cell in a 1000-row table and only that cell's component updates. The core is ~700 lines — a deliberately minimal alternative to `@json-render/react` that strips away AI streaming, validation, directives, and multi-framework output to focus purely on efficient rendering with a path-based store and action system.

[thin-render on GitHub](https://github.com/ypyl/thin-render) · [Live demo](https://ypyl.github.io/thin-render/)
