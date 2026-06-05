---
layout: post
title: "From PRDs to ADRs to OpenSpec — How AI-Driven Development Reshapes Engineering Artifacts"
date: 2026-06-05
categories: programming
tags: architectural-decision-records, spec-driven-development, openspec, prd, ai-coding
---

In traditional software engineering, information flows through a well-understood chain of **artifacts**: a **BRD** (Business Requirements Document) defines the market opportunity, a **PRD** (Product Requirement Document) translates that into features, an **RFC** (Request for Comments) opens technical debate, an **ADR** (Architectural Decision Record) locks in specific choices, and a **TDD** (Technical Design Document) maps out the full system blueprint. Each document is written by humans, for humans.

The catch: when you introduce an AI coding agent into this pipeline, none of these documents survive the handoff intact. Feed an agent a 20-page PRD and it will hallucinate architecture — the language is too ambiguous, the context too broad, and there's no machine-readable boundary between what to change and what to leave alone.

## What an ADR Looks Like

An **Architectural Decision Record** is the most actionable artifact in the traditional chain. It's a lightweight, version-controlled Markdown file that documents a single technical choice — the context that forced it, the decision itself, and the trade-offs accepted. Here's the canonical template:

```markdown
# ADR 012: Implement Redis for Session Caching

## Status
Accepted

## Context
Our user base has scaled 40%. PostgreSQL is experiencing high CPU
utilization due to constant session read/write queries, degrading
checkout performance.

## Decision
We will migrate all user session management out of PostgreSQL and
into a dedicated Redis cluster.

## Consequences
- **Positive:** Reduces DB load by 30%; session read latency <2ms.
- **Negative:** New infrastructure to maintain; session data lost
  if Redis cluster fails entirely (acceptable risk for our SLA).
```

This is clean, version-controlled, and permanently answers "why did we choose Redis?" for every future engineer. But notice what's missing: there's no mention of *which files to create*, *which existing modules to modify*, or *what to delete*. Those are the exact directives an AI agent needs — and ADRs don't provide them.

## The OpenSpec Bridge: Delta Specs

**OpenSpec**, an open-source SDD toolkit by Fission-AI, fills this gap with a **delta spec** — a `proposal.md` file that translates an ADR's abstract decision into concrete, file-level directives using three explicit blocks: `ADDED`, `MODIFIED`, and `REMOVED`.

Here's how the same Redis ADR becomes an OpenSpec proposal that an AI agent can execute:

```markdown
# Proposal: Implement Redis Session Cache

## ADDED
- `src/infra/redis/client.ts` — Connection handler with retry logic,
  health-check ping, and graceful shutdown.
- `src/infra/redis/config.ts` — Redis host, port, TTL, and
  connection-pool settings sourced from environment variables.

## MODIFIED
- `src/auth/session.ts` — Update `getSession()` to read from Redis
  first, falling back to PostgreSQL. Update `setSession()` to write
  through to both stores during the migration window.

## REMOVED
- `src/infra/postgres/queries/sessions.ts` — Drop direct SQL
  session-lookup queries after the migration window closes.
```

The difference is structural, not cosmetic. An ADR says *"use Redis."* A delta spec says *"create these two files, modify that auth module, and eventually delete the old SQL queries."* The AI agent no longer has to interpret intent — it has executable boundaries.

## Where OpenSpec Artifacts Sit in the Pipeline

Another gotcha: engineers frequently miscategorize OpenSpec as a **TDD replacement**. It's not. A TDD is a macroscopic, human-authored blueprint of system topology, database schemas, and cross-team contracts. OpenSpec operates one layer below — it atomizes the TDD's decisions into micro-artifacts that an AI agent can consume sequentially.

Here's the artifact mapping from traditional planning documents to OpenSpec's execution files:

| Traditional Artifact | OpenSpec Equivalent | Role in AI Execution |
|---|---|---|
| **PRD** (what to build) | `proposal.md` | Isolated scope of the change — ADDED, MODIFIED, REMOVED |
| **TDD** (high-level design) | `design.md` | Focused micro-design for *this specific change*, not the whole system |
| **QA Test Plan** | `specs/<domain>/spec.md` | Behavioral constraints in Given/When/Then format for automated verification |
| **Jira Subtasks** | `tasks.md` | Machine-readable checklist processed step by step via `openspec apply` |

## The Execution Checklist

The `tasks.md` file is where planning meets code. Unlike a Jira ticket (which a human reads and interprets), an OpenSpec task list is structured for sequential agent execution. Each item is an atomic action the agent checks off as it works:

```markdown
## 1. Infrastructure Setup
- [x] Create `src/infra/redis/types.ts` with RedisConfig interface
- [x] Create `src/infra/redis/client.ts` with connection handler
- [x] Add `ioredis` dependency to `package.json`
- [ ] Wire Redis connection into app lifecycle (`src/app.ts`)

## 2. Session Migration
- [ ] Modify `src/auth/session.ts` — add Redis read path with fallback
- [ ] Modify `src/auth/session.ts` — add dual-write logic for migration window
- [ ] Add integration tests for Redis fallback behavior
- [ ] Add `REDIS_URL` to `.env.example` and deployment config

## 3. Cleanup (post-migration)
- [ ] Remove `src/infra/postgres/queries/sessions.ts`
- [ ] Mark PostgreSQL session table as deprecated in migration file
```

This is where the traditional artifact chain — BRD → PRD → ADR → TDD — finally meets executable code. OpenSpec doesn't replace those documents; it translates their decisions into a format that survives the handoff from human reasoning to AI execution. The ADR records *why* Redis was chosen. The OpenSpec delta spec and task list ensure the agent builds it correctly — no interpretation required.

[OpenSpec on GitHub](https://github.com/Fission-AI/OpenSpec) · [ADR pattern by Michael Nygard](https://cognitect.com/blog/2011/11/15/documenting-architecture-decisions) · [OpenSpec docs: proposal format](https://github.com/Fission-AI/OpenSpec/tree/main/docs)
