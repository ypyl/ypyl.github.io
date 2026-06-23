---
layout: post
title: "OpenSpec Scoping: One Change or Many?"
date: 2026-06-23
tags: [openspec, sdd, ai-coding, spec-driven-development]
categories: ai
---

Using OpenSpec SDD with AI coding agents, the trickiest judgment call isn't writing specs — it's knowing when to split. One proposal or three? The maintainers don't give a hard rule, but after digging through docs, issues, and practical usage patterns, here's a decision framework that works.

## The core test: can one proposal stay coherent?

A change maps to one proposal when it has a **single intent** and a **single scope boundary** you can write down cleanly. OpenSpec's proposal template forces this with explicit "In scope / Out of scope" lists.

## Practical split signals

If **any 2+** of these apply, break it into separate changes:

| Signal | What it looks like |
|--------|-------------------|
| **Motivation needs "and also"** | Can't describe the whole thing in one sentence without conjunctions |
| **Independently shippable** | Each piece would be useful on its own if the others were never built |
| **Unrelated spec areas** | Delta specs touch `specs/auth/` AND `specs/billing/` AND `specs/ui/` with no shared logic |
| **Separate technical approaches** | `design.md` needs sub-sections for unrelated concerns ("backend approach" vs "frontend redesign" vs "migration") |
| **Task list balloons** | `tasks.md` pushing past ~15-20 tasks, or tasks cluster into groups with no dependencies between them |
| **Two commit-message titles** | "Add dark mode" + "Migrate settings layout" = two changes pretending to be one |

## Keep as one change if

- It's a single user-facing capability or architectural concern, even touching many files
- The "out of scope" list naturally excludes adjacent work
- `tasks.md` reads top-to-bottom as one coherent feature

## Where to put the check

One file: **`openspec/config.yaml`**. OpenSpec injects it into artifact-generation prompts via two fields:

- `context` — appears in **all** artifact instructions (ambient guidance)
- `rules` — appears only for the **matching artifact** (deterministic, enforced)

```yaml
# openspec/config.yaml
schema: spec-driven

context: |
  We bias toward small, narrowly-scoped changes over large bundled ones.

rules:
  proposal:
    - Before finalizing scope, check for split signals: does the motivation
      need "and also" to describe it; are the pieces independently shippable;
      do the spec deltas touch unrelated domains (e.g. specs/auth/ AND
      specs/billing/) with no shared logic; would design.md need separate
      sub-sections for unrelated technical approaches; is tasks.md trending
      past ~15 tasks or clustering into independent groups; could you write
      two different commit-message titles for this change. If 2+ signals
      apply, propose splitting into separate changes with distinct names.
    - Write "In scope" and "Out of scope" sections explicitly, even if short.
```

The `context` field sets the tone everywhere. The `rules.proposal` field fires deterministically every time `proposal.md` is generated — via `/opsx:propose`, `/opsx:new`+`/opsx:ff`, or any path that produces a proposal artifact.

### What happens when the check fires?

These are AI instructions, not enforced constraints — the agent interprets them. If 2+ signals apply, the agent should **pause and flag it** rather than silently generating one oversized proposal:

```text
Agent: This looks like two changes:
       - `add-dark-mode` (UI toggle + system detection + persistence)
       - `migrate-settings-layout` (refactor settings page structure)

       Want me to scope them separately, or is there a reason
       to keep them together?
```

You can always override and say "no, keep it as one" — you're in control. But the check prevents the default behavior of bundling unrelated work into one change without a deliberate decision.

### What about the explore phase?

`/opsx:explore` is unstructured conversation — no artifact is being generated, so `config.yaml` rules don't fire there. But that's fine: if the agent overscopes during exploration, the `rules.proposal` check catches it the moment a proposal is actually written. No need for a separate checkpoint in AGENTS.md (which would load into *every* interaction, wasting context tokens).

If you *really* want a guardrail during explore, put a short line in your project's own instructions file — but in practice, `config.yaml` alone is sufficient.

## The resulting flow

```
/opsx:explore
  → unstructured conversation (no formal check, but context: "bias small")
/opsx:propose
  → config.yaml rules.proposal fires → scope is validated as proposal.md is written
/opsx:apply
/opsx:archive
```

One deterministic checkpoint, no extra commands. If it passes, the change is sized right before any code gets written.

The structural hint from OpenSpec itself: `/opsx:bulk-archive` explicitly handles overlapping spec changes and merges chronologically — the tool is built assuming many small, possibly-overlapping changes, not few large ones. **Bias small.**
