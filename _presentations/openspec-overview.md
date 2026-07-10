---
layout: presentation
---

# OpenSpec

**"Plan mode, but organized."** Lighter, fluid, brownfield-oriented. No ceremony.

| | Heavier approaches | OpenSpec |
|---|---|---|
| **Setup** | Methodology + templates | `npm install` + `openspec init` |
| **Specs** | Full system spec first | Delta specs — only what's changing |
| **Workflow** | Separate tools, phase gates | Slash commands in your AI chat |
| **Existing code** | Document everything upfront | One change at a time, specs grow |
| **Tooling** | Tied to specific IDEs/models | 30+ AI coding tools |

> **Notes:** OpenSpec is a spec-driven development framework — "plan mode, but organized." MIT-licensed, Node.js CLI, works with 30+ AI coding tools. Compared to approaches like git-spec or BMAD, OpenSpec skips the ceremony. Two terminal commands to install, then you live in your AI chat. Delta specs mean you write only what's changing — no upfront documentation.

---

# Philosophy

```
fluid not rigid         — no phase gates
iterative not waterfall — refine as you go
easy not complex        — minimal ceremony
brownfield-first        — existing code, not just greenfield
```

**Enablers, not gates.** Edit any artifact anytime.

> **Notes:** Traditional spec systems lock you into phases. OpenSpec refuses that. The order proposal → specs → design → tasks shows dependencies for the AI's context, not a rigid process. Discover during implementation that the design was wrong? Edit design.md and keep going.

---

# What's in the Repo

Two folders. One truth.

```
openspec/
├── specs/              ← how things work today
│   └── auth/
│       └── spec.md
│
├── changes/            ← what you're proposing
│   └── add-dark-mode/
│       ├── proposal.md      ← why & what
│       ├── design.md        ← how
│       ├── tasks.md         ← steps
│       └── specs/           ← delta (ADDED/MODIFIED/REMOVED)
│
└── config.yaml
```

> **Notes:** specs/ is what's true. changes/ is what you're proposing. Archiving moves a proposal into truth. Each change is a self-contained folder. Changes can exist in parallel without conflicting.

---

# The Core Loop

```text
propose ──► apply ──► archive
 (plan)     (build)   (record)
```

Slash commands in your AI chat — not the terminal.

> **Notes:** You type them in your AI chat: /opsx:propose, /opsx:apply, /opsx:archive. The CLI (openspec init, openspec update) runs in your terminal. This split is the most common point of confusion for new users.

---

# Step 1: Propose

**`/opsx:propose`** — one command, four artifacts.

```text
You: /opsx:propose add-dark-mode

AI:  Created openspec/changes/add-dark-mode/
     ✓ proposal.md
     ✓ specs/ (delta)
     ✓ design.md
     ✓ tasks.md
```

Review. Adjust. Then build.

> **Notes:** Propose generates all artifacts in one step. You review and adjust before saying "go." This is the agreement moment — human and AI align on the plan.

---

# The Artifacts

```text
proposal ──► specs ──► design ──► tasks
   why        what       how       steps
```

| Artifact | Question |
|----------|----------|
| **proposal.md** | Why? |
| **specs/** | What's changing? |
| **design.md** | How? |
| **tasks.md** | What steps? |

**Enablers, not gates.** Edit any artifact anytime.

> **Notes:** Artifacts build on each other — you can't write good tasks without specs. But nothing locks. Discover the design was wrong? Edit design.md and keep going. Scope shrinks? Update the proposal. Real work is messy and iterative.

---

# Delta Specs

Describe the **diff**, not the whole world.

```markdown
## ADDED Requirements
### Requirement: Two-Factor Authentication
The system MUST require a second factor during login.

## MODIFIED Requirements
### Requirement: Session Timeout
(Previously: 60 minutes → Now: 30 minutes)

## REMOVED Requirements
### Requirement: Remember Me
(Deprecated in favor of 2FA)
```

**On archive:** ADDED → appended, MODIFIED → replaced, REMOVED → deleted.

> **Notes:** This is what makes OpenSpec work for brownfield. You write a small delta describing only what's changing — ADDED, MODIFIED, REMOVED. On archive, main specs update automatically.

---

# Step 2: Apply

**`/opsx:apply`** — AI works through tasks, checking them off.

```text
You: /opsx:apply

AI:  ✓ 1.1 Created ThemeContext
     ✓ 1.2 Added CSS custom properties
     ✓ 1.3 Implemented localStorage persistence
     ✓ 2.1 Created ThemeToggle component
     All tasks complete!
```

Pause anytime. Resume later. Parallel changes supported.

> **Notes:** Apply reads tasks.md and implements sequentially. Hit an issue? Pause, update artifacts, resume. Task file tracks progress across context switches.

---

# Step 3: Archive

**`/opsx:archive`** — close the loop.

```text
You: /opsx:archive

AI:  ✓ Delta specs merged into specs/
     ✓ Change moved to archive/2026-07-10-add-dark-mode/
```

| Operation | Result |
|-----------|--------|
| ADDED | Appended to main spec |
| MODIFIED | Replaces existing |
| REMOVED | Deleted from main spec |

Change preserved with date stamp. Specs now describe reality.

> **Notes:** Archiving closes the loop. Delta specs merge into source of truth. Change folder preserved with date stamp for audit history.

---

# Brownfield by Design

```text
   Other tools                         OpenSpec
   ═══════════                         ════════════

   "Describe the entire               "Here's what's changing:"
    system"                                │
        │                          ┌───────┼───────┐
        ▼                          ▼       ▼       ▼
   ┌──────────┐               ADDED    MODIFIED  REMOVED
   │ 200+ line│
   │ full spec│               Three small blocks.
   │ upfront  │               No full rewrite.
   └──────────┘
```

> **Notes:** Other spec tools assume greenfield. OpenSpec assumes brownfield: you only describe what's different. ADDED, MODIFIED, REMOVED. That's it. This makes it practical for real-world teams.

---

# Specs as Source of Truth

Requirements + scenarios. Testable. No implementation detail.

```markdown
### Requirement: Theme Selection
The system SHALL allow theme switching.

#### Scenario: Manual toggle
- GIVEN a user on any page
- WHEN they click the theme toggle
- THEN the theme switches immediately
```

| Keyword | Strength |
|---------|----------|
| MUST/SHALL | Absolute |
| SHOULD | Recommended |
| MAY | Optional |

> **Notes:** RFC 2119 keywords communicate intent strength. Scenarios are concrete given/when/then examples. A spec is a behavior contract, not an implementation plan.
