---
layout: post
title: "Two Paths to Spec-Driven Development: SPDD vs. OpenSpec"
date: 2026-05-16
tags: sdd, ai, spec-driven-development, spdd, openspec, software-engineering
categories: ai
---

Specification-Driven Development (SDD) is having a moment. As AI coding assistants become ubiquitous, the old question — *"how do we make sure the AI builds the right thing?"* — has become urgent. Two approaches have emerged as leading answers: **SPDD** (Structured Prompt-Driven Development), described in depth by Zhang Wei and published on [Martin Fowler's site](https://martinfowler.com/articles/structured-prompt-driven/), and **OpenSpec**, an open-source toolkit by [Fission-AI](https://github.com/Fission-AI/OpenSpec) with nearly 50,000 GitHub stars.

Both claim to solve the same problem: aligning AI-generated code with human intent. But they approach it from fundamentally different angles. Here's how they compare — and when you'd reach for one over the other.

---

## At a Glance

| Dimension | SPDD | OpenSpec |
|---|---|---|
| **Origin** | Zhang Wei / ThoughtWorks, published on martinfowler.com | Fission-AI, open-source (48.5k ★) |
| **Nature** | Methodology + lightweight CLI (`openspdd`) | Full toolkit + CLI (`openspec`) + slash commands |
| **Core artifact** | REASONS Canvas — a single structured prompt file | Change folder: `proposal.md`, `specs/`, `design.md`, `tasks.md` |
| **Spec format** | Natural language in a 7-part template | Markdown with Given/When/Then scenarios; ADDED/MODIFIED/REMOVED deltas |
| **Sync model** | Two-way: prompt ↔ code in both directions | One-way delta merge on archive |
| **Target** | Senior engineers, high-compliance domains | All developers, broad AI tool ecosystem |

---

## Core Philosophy

**SPDD** is built on a single insight: *AI needs bounded, well-defined problems to produce reliable output.* Its answer is the **REASONS Canvas** — a seven-part structured prompt that captures everything from business rules to method signatures before a single line of code is written. The prompt is the primary asset; code is a translation of intent. The methodology's mantra is "design before you generate."

**OpenSpec** takes a more pragmatic line: *specs should be the source of truth, and changes should be described as deltas.* It doesn't demand deep architectural reasoning up front. Instead, it provides a folder structure, command-driven workflow, and merge system that lets specs grow organically as features are built and archived.

The difference is subtle but real: SPDD asks you to *think hard first*, then generate. OpenSpec asks you to *capture intent incrementally* and let the process keep things organized.

---

## The Spec Artifact

### SPDD's REASONS Canvas

A single comprehensive document with seven dimensions:

- **R**equirements — with Definition of Done
- **E**xpectation — what success looks like
- **A**pproach — architectural strategy, domain model
- **S**tructure — components, interfaces, data flow
- **O**perations — concrete implementation steps, down to method signatures
- **N**orms — coding standards and conventions
- **S**afeguards — constraints, what NOT to do

This is exhaustive. It leaves the AI very little room for creative interpretation — which is exactly the point.

### OpenSpec's Change Folder

Four separate artifacts with clear dependencies:

- **`proposal.md`** — the "why" and "what"
- **`specs/<domain>/spec.md`** — requirements with Given/When/Then scenarios
- **`design.md`** — technical approach
- **`tasks.md`** — checkbox checklist

The key innovation is **delta specs**: you never rewrite an entire spec. You describe only what's changing — ADDED, MODIFIED, or REMOVED requirements. On archive, the deltas merge into the main specs, building a comprehensive specification incrementally.

---

## Workflow

**SPDD** follows a disciplined six-step loop:

1. **Story** — shape the raw idea into a user story
2. **Clarify** — human reviews and refines business meaning
3. **Analysis** — AI analyzes domain, risks, strategic direction
4. **Structured Prompt** — AI generates the REASONS Canvas, human reviews
5. **Code + Review** — generate code, run API tests, review, adjust
6. **Unit Tests** — generated last, after implementation is stable

Review is distributed across checkpoints. By the time you're looking at code, requirements, domain model, and design have already been signed off.

**OpenSpec** is more flexible and tool-driven:

```
Core path:      /opsx:propose → /opsx:apply → /opsx:sync → /opsx:archive
Expanded path:  /opsx:new → /opsx:ff|continue → /opsx:apply → /opsx:verify → /opsx:archive
```

Plus an `/opsx:explore` command for thinking through ideas before committing to a change. It supports parallel changes, context switching, and bulk archiving with automatic conflict resolution — features clearly designed for real-world team workflows.

---

## The Sync Problem

This is where the approaches diverge most sharply.

**SPDD** treats the structured prompt as a *living design document* that must stay synchronized with code. It provides three commands for different sync directions:

- `/spdd-generate` — prompt → code (initial generation)
- `/spdd-prompt-update` — requirements → prompt → code (when business rules change)
- `/spdd-sync` — code → prompt (after refactoring, sync changes back)

Logic corrections always update the prompt first. Refactoring always updates the code first, then syncs. This two-way discipline means neither side silently diverges.

**OpenSpec** syncs deltas once — on archive. The main `specs/` directory is the source of truth for *current system behavior*, but it's not a design document that mirrors internal architecture. When code drifts from spec during implementation, you update the artifacts. When you're done, archive merges everything and the change folder moves to history.

The difference reflects their philosophies: SPDD wants the prompt to be a *design twin* of the code. OpenSpec wants specs to be a *behavioral contract* that evolves with the system.

---

## Brownfield and Legacy Code

**OpenSpec** wins here. Its delta system — ADDED, MODIFIED, REMOVED — is purpose-built for incrementally growing specs over an existing codebase. The `/opsx:onboard` command scans your codebase and suggests improvement opportunities, then walks you through a complete change cycle. Spec coverage grows organically, one change at a time.

**SPDD** doesn't ignore brownfield — the analysis phase uses existing code as context — but its workflow is fundamentally designed for new work with clear requirements. There's no equivalent of OpenSpec's delta merge mechanism.

---

## Governance and Review

**SPDD** distributes human review across six checkpoints. The argument is that this keeps cognitive load manageable: you're never reviewing everything at once. There's also `/spdd-code-review`, an automated alignment check between the Canvas and the code. But human judgment remains load-bearing — only a human can tell whether the Canvas itself still matches the real business intent.

**OpenSpec** provides `/opsx:verify`, which checks three dimensions:
- **Completeness** — all tasks done, all requirements implemented
- **Correctness** — implementation matches spec intent
- **Coherence** — design decisions reflected in code

It's more automated but less philosophically layered. Verification is one step before archive, not distributed across the workflow.

---

## Team Scale and Tool Ecosystem

**OpenSpec** is clearly built for teams. It supports parallel changes in flight, context switching between work streams, bulk archive with automatic conflict resolution, and works across Claude Code, Cursor, Windsurf, GitHub Copilot, Kimi, and Trae. Each tool has its own command syntax, and OpenSpec handles the mapping.

**SPDD** is more of a methodology with a reference CLI implementation. Its fitness table rates team collaboration 4/5 stars, but the tooling for parallel work and conflict resolution isn't as developed. It's model-agnostic but benefits most from strong reasoning models (Claude Opus, GPT Codex, Gemini Pro) for Canvas generation.

---

## Learning Curve

**SPDD** honestly acknowledges it currently requires "senior expertise up front." The three core skills — Abstraction First, Alignment, Iterative Review — are taught explicitly, but translating business rules into clean abstractions and design constraints is not a junior-level activity. The team is working on lowering this barrier through organizational asset systems and decision memory.

**OpenSpec** is more immediately accessible. `/opsx:onboard` walks you through a complete workflow using your actual codebase. The artifact templates give structure without requiring deep architectural reasoning. But spec quality still depends on the human's ability to write good requirements — the tool provides structure, not judgment.

---

## When to Use Which

| Scenario | Better Fit |
|---|---|
| New system with clear, complex business rules | **SPDD** — the REASONS Canvas captures intent depth that lighter artifacts don't |
| Adding features incrementally to existing code | **OpenSpec** — delta specs are purpose-built for this |
| High-compliance domain (finance, healthcare) | **SPDD** — Safeguards + multi-checkpoint review provide stronger governance |
| Fast-moving team, multiple AI tools in use | **OpenSpec** — cross-tool support, parallel changes, quick archive |
| Complex domain modeling with rich abstractions | **SPDD** — Abstraction First and Approach/Structure sections excel here |
| Bug fixes and small improvements | **OpenSpec** — `/opsx:ff` for fast execution |
| Mixed-seniority team | **OpenSpec** — lower barrier to entry |
| Senior team wanting deep intent alignment | **SPDD** — the methodology's entire thesis |

---

## They're Complementary

Despite their differences, these two approaches aren't really competitors. They solve different problems.

**SPDD** is fundamentally about **intent fidelity** — ensuring the code faithfully realizes the human's intent. It's a cognitive framework for reasoning alongside AI, where the structured prompt is the north star. The question it asks is: *"Did we build the right thing, and does the code faithfully express our intent?"*

**OpenSpec** is fundamentally about **workflow structure** — giving AI coding assistants a predictable, traceable process. It's about artifact management, change tracking, and making specs the source of truth. The question it asks is: *"Do we have a clear spec, and can we trace every change back to it?"*

You could imagine using SPDD's REASONS Canvas *within* OpenSpec's change folder structure — or using OpenSpec's delta/archive system to manage the evolution of SPDD prompts over time. The real choice isn't between them. It's about whether your team needs deep cognitive rigor, process automation, or — ideally — some of both.

---

*Both SPDD and OpenSpec are evolving rapidly. SPDD's roadmap includes automated verification at the asset layer, decision memory for reuse, and progressively higher automation ratios. OpenSpec continues to expand cross-tool support and customization options. The SDD space is young, and these are likely the first of many approaches.*
