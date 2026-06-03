---
layout: post
title: "OpenSpec vs GitHub Spec-Kit — A Hands-On Comparison for Spec-Driven Development"
date: 2026-06-03
categories: programming
tags: spec-driven-development, openspec, spec-kit, ai-coding, developer-tools
---

**Spec-driven development** means writing a spec before code and using AI to generate the implementation from it. Two open-source tools compete in this space: **OpenSpec** by Fission-AI and **GitHub Spec-Kit** by GitHub. Here's how they compare — based on hands-on testing, documentation, and community reports.

## Quick Summary

| | **OpenSpec** (Fission-AI) | **GitHub Spec-Kit** (GitHub) |
|---|---|---|
| **Repo** | `Fission-AI/OpenSpec` | `github/spec-kit` |
| **Stars** | Growing | **108k** |
| **Language** | Node.js / TypeScript | Python |
| **Install** | `npm install -g @fission-ai/openspec` | `uv tool install specify-cli --from git+...` |
| **Philosophy** | Fluid, iterative, brownfield-first | Structured, constitutional, greenfield-optimized |
| **Spec model** | **Delta specs** (only what changes) | **Full specs** (complete feature descriptions) |
| **Workflow** | 3 steps: Propose → Apply → Archive | 4 steps: Constitution → Specify → Plan → Tasks → Implement |
| **Artifacts** | ~4 files per change | 7+ files per feature |
| **Parallel work** | Built-in (isolated change folders) | Per-feature branches |
| **Tool support** | 25+ AI coding agents | 20+ coding agents |
| **License** | MIT | MIT |

## Philosophy & Approach

**OpenSpec** is built around four principles: *fluid not rigid* — no phase gates, work on any artifact at any time; *iterative not waterfall* — learn as you build, refine as you go; *easy not complex* — lightweight setup, minimal ceremony; and *brownfield-first* — designed for existing codebases, not just greenfield projects.

**Spec-Kit** follows a structured, constitutional approach. A project-wide **constitution** establishes non-negotiable principles before any spec is written, and every feature inherits those rules. The workflow is sequential: constitution → specify → plan → tasks → implement.

> **Key difference**: OpenSpec embraces fluidity — you can edit any artifact at any time. Spec-Kit enforces a sequential pipeline with defined phase gates.

## Workflow & Artifacts

**OpenSpec** uses slash commands (`/opsx:*`):

```bash
/opsx:propose add-dark-mode   # creates proposal.md + specs/ + design.md + tasks.md
/opsx:apply                   # implements tasks
/opsx:archive                 # merges deltas into source-of-truth specs/
```

File structure:

```
openspec/
├── specs/                    # source of truth (current system behavior)
├── changes/
│   └── add-dark-mode/
│       ├── proposal.md       # why we're doing this
│       ├── specs/            # delta specs (only what's changing)
│       ├── design.md         # technical approach
│       └── tasks.md          # implementation checklist
└── archive/                  # completed changes
```

**Spec-Kit** uses slash commands (`/speckit.*`):

```bash
/speckit.constitution         # project-wide immutable principles
/speckit.specify              # feature spec with user stories & acceptance criteria
/speckit.plan                 # technical plan + research + data models + contracts
/speckit.tasks                # executable task breakdown
/speckit.implement            # code generation
```

File structure:

```
.specify/
├── memory/
│   └── constitution.md
├── scripts/                  # Bash / PowerShell helper scripts
└── templates/                # Markdown templates

specs/[branch-name]/
├── spec.md
├── plan.md
├── tasks.md
├── research.md
├── data-model.md
├── contracts/
└── quickstart.md
```

> **Key difference**: OpenSpec produces ~4 files per change (compact, delta-based). Spec-Kit produces 7+ files per feature (verbose, full specification). OpenSpec's delta approach means you only describe what's *changing*, not the entire system. In hands-on testing, Spec-Kit's artifact volume made review feel like auditing AI-generated prose rather than designing software.

## Speed & Developer Experience

Ran Isenberg, a senior engineer at Palo Alto Networks, evaluated all three major SDD tools on the same real feature in February 2026. Here's how OpenSpec and Spec-Kit scored across 13 dimensions:

| Dimension | OpenSpec | Spec-Kit |
|-----------|:--------:|:--------:|
| Specification quality | 4/5 | 2/5 |
| Developer experience | 4/5 | 3/5 |
| Human review checkpoints | 5/5 | 3/5 |
| Workflow visibility | 4/5 | 2/5 |
| Iterative refinement | 3/5 | 2/5 |
| Mid-feature course correction | 4/5 | 2/5 |
| AI tool compatibility | 5/5 | 4/5 |
| Parallel development support | 5/5 | 5/5 |
| Installation & upgrade | 4/5 | 2/5 |
| **Overall score** | **4.00** | **2.77** |

On raw speed and cost:

| Metric | OpenSpec | Spec-Kit |
|--------|----------|----------|
| Planning time | 3 hours | 4 hours |
| Implementation time | 1 day | 1 day |
| Planning cost | ~$25 | ~$30 |
| Implementation cost | ~$70 | ~$45 |

Isenberg noted: *"Spec-Kit generates execution-internal artifacts (research notes, status files) that land in your project root alongside source code, not meant for human review. No help command or status view: the tool doesn't tell you where you are or what to do next."* OpenSpec provides `openspec status` and `/opsx:continue` to display the artifact dependency graph and always suggest the next step.

## Iterative Refinement & Course Correction

**OpenSpec** handles mid-feature changes through fluidity: edit any artifact at any time, run `/opsx:apply`, and it picks up from where you left off without regenerating unchanged files. Course correction is frictionless.

**Spec-Kit** requires re-running affected commands from the phase where changes are needed. Each command regenerates its *full* document — a plan your team already reviewed gets replaced entirely, with no diff of just the changed sections. There's no built-in code review step between phases either. Iteration stops at planning; changing direction means restarting the chain.

## Brownfield vs Greenfield

**OpenSpec** is brownfield-first. Delta specs describe only what's changing in an existing codebase — no constitution needed upfront. The downside: the delta model assumes the AI understands the unchanged parts of your codebase. On projects with poor inline documentation, the agent may hallucinate surrounding context.

**Spec-Kit** is greenfield-optimized. The constitution-first approach works for new apps but adds overhead when modifying existing systems. Tutorials focus on "build an app from scratch" — introducing Spec-Kit into existing code takes more work. The research phase does investigate existing code, but Isenberg reported cases where the agent correctly documented preexisting classes, then regenerated duplicates from scratch during implementation.

## Installation & Upgrades

**OpenSpec**: `npm install -g @fission-ai/openspec@latest` then `openspec init`. User content (`openspec/specs/`, `openspec/changes/`) and tool runtime files live in separate directories. Upgrading the package and running `openspec update` regenerates agent instructions without touching your specs.

**Spec-Kit**: Requires Python + `uv` toolchain. `uv tool install specify-cli --from git+https://github.com/github/spec-kit.git@v0.1.6` then `specify init <project>`. Has a [documented upgrade hazard](https://github.com/github/spec-kit/blob/v0.1.6/docs/upgrade.md): *"⚠️ Important Warning: Upgrade will overwrite customizations."* Templates, scripts, and user modifications share the same file space — if you've customized the plan template with an organizational security review checklist, one upgrade deletes it.

## Community & Project Health

| Metric | OpenSpec | Spec-Kit |
|--------|----------|----------|
| GitHub stars | Growing fast | 108k |
| Commits (90 days) | 158 | 37 |
| Open issues / close rate | 201 / 24% | 533 / 37% |
| PR backlog (median age) | 37 / 30 days | 94 / 62 days |
| Bus factor | 1 | 2 |
| Community channel | Active Discord | None (GitHub issues only) |
| Backed by | Fission-AI (independent) | GitHub / Microsoft |

## When to Use Which

**Choose OpenSpec when:**
- Working on an **existing codebase** (brownfield)
- You want **minimal ceremony** and fast iteration
- You need **parallel work** across multiple features
- You value fluidity — edit specs out of order without restarting a pipeline
- You use Node.js and want npm-based tooling
- Your coding agent already has OpenSpec skills built in (pi, Cursor, Claude Code, etc.)

**Choose Spec-Kit when:**
- Starting a **greenfield project** from scratch
- Your organization needs a **constitutional foundation** — enforced architectural principles that survive across all features
- You want maximum **community support**, tutorials, and LinkedIn Learning courses
- You prefer structured, phase-gated workflows with explicit clarification checkpoints
- You're in the GitHub Copilot ecosystem natively
- You need deep customization via extensions and presets

## Bottom Line

OpenSpec is fluid and lightweight; Spec-Kit is structured and comprehensive. OpenSpec scored higher in hands-on testing for brownfield work and developer experience. Spec-Kit works better when starting fresh and wanting an immutable constitution. Regardless of which you pick: specs are starting points, not contracts. Review every generated artifact — the agent's confidence doesn't mean it'll follow its own plan.

[OpenSpec on GitHub](https://github.com/Fission-AI/OpenSpec) · [GitHub Spec-Kit repo](https://github.com/github/spec-kit) · [Ran Isenberg's hands-on comparison](https://ranthebuilder.cloud/blog/i-tested-three-spec-driven-ai-tools-here-s-my-honest-take/) · [Martin Fowler: Understanding SDD Tools](https://martinfowler.com/articles/exploring-gen-ai/sdd-3-tools.html)
