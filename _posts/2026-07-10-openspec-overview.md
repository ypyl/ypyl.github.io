---
layout: post
title: "OpenSpec Overview — Spec-Driven Development with AI"
date: 2026-07-10
tags: [openspec, sdd, spec-driven-development, ai-coding, developer-tools]
categories: programming
---

OpenSpec is a lightweight agreement layer between you and your AI coding assistant — **"plan mode, but organized."** It's lighter than alternatives like git-spec or BMAD, fluid by design (no phase gates), and built brownfield-first for existing codebases.

→ **[Open presentation](/present/openspec-overview/)**

## In Short

| | Heavier approaches | OpenSpec |
|---|---|---|
| **Setup** | Methodology + templates | `npm install` + `openspec init` |
| **Specs** | Full system spec first | Delta specs — only what's changing |
| **Workflow** | Separate tools, phase gates | Slash commands in your AI chat |
| **Existing code** | Document everything upfront | One change at a time, specs grow |

**The loop:** `propose` → `apply` → `archive`. Describe what you want, AI drafts the plan, you review, AI builds, you archive. All in your existing chat — no new UI to learn.

## Philosophy

```
fluid not rigid         — no phase gates
iterative not waterfall — refine as you go
easy not complex        — minimal ceremony
brownfield-first        — existing code, not just greenfield
```

Artifacts are **enablers, not gates** — edit any of them anytime. Discover the design was wrong mid-implementation? Fix `design.md` and keep going.
