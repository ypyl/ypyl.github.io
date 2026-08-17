---
layout: post
title: "Dive into Claude Code: Notes on the Design Space of AI Agent Systems"
date: 2026-08-16
tags: [claude-code, ai-agents, architecture, llm]
categories: ai
---

Notes from reading [*Dive into Claude Code: The Design Space of Today's and Future AI Agent Systems*](https://arxiv.org/abs/2604.14228) (VILA Lab, MBZUAI), grounded by inspecting the actual Claude Code v2.1.119 binary locally (the paper analyzed v2.1.88). Written as general lessons for building or understanding agent harnesses — the deterministic machinery around the model loop.

The central thesis of the paper: **the model is a thin decision layer inside a thick deterministic harness.** Roughly 1.6% of the codebase is AI decision logic; the other 98.4% is the operational harness — permissioning, tool routing, context management, recovery, persistence. The harness creates conditions under which the model can decide well, rather than constraining its choices.

---

## 1. Harness vs scaffolding: where to invest

Claude Code's bet: invest in **operational infrastructure**, not **decision scaffolding**.

| Approach | Examples | Center of gravity |
|---|---|---|
| Minimal harness, free model judgment | Claude Code | context mgmt, safety layers, recovery |
| Explicit planning graphs | LangGraph, Devin | routing model outputs through typed state graphs |
| Container isolation as safety | SWE-Agent, OpenHands | Docker boundary instead of layered policy |
| Version control as safety | Aider | git rollback as the primary net |

Recent benchmark evidence supports the harness bet: holding the model fixed and changing only the harness shifts long-horizon scores by up to 18 points (WildClawBench). As frontier models converge, the harness becomes the differentiator.

**Takeaway:** when building an agent, spend effort on context management, safety layering, and recovery — not on wrapping the model in more planning frameworks.

---

## 2. The agent loop

### 2.1 The core while-loop

The whole system is one reactive loop (ReAct pattern): assemble context → call model → execute tools → results feed back → repeat. The loop is an async generator so it can stream events to the UI while keeping a single synchronous control flow.

```python
while not stopped:
    context = assemble(system_prompt, tool_schemas, history, hook_additions)
    response = model(context, tools)          # flat tool pool
    if response.is_text_only(): break
    for tool_call in response.tool_use_blocks:
        if permitted(tool_call):              # permission gate
            result = execute(tool_call)
            history.append(tool_call, result)
```

### 2.2 One State object, whole-object swap at iteration boundaries

The loop keeps **all mutable state in a single `State` object**: messages, tool context, compaction tracking, recovery counters, turn count. Each iteration starts by destructuring the current snapshot; each exit point replaces the whole object rather than mutating fields:

```js
D = {
  messages, toolUseContext, autoCompactTracking,
  maxOutputTokensRecoveryCount, hasAttemptedReactiveCompact,
  maxOutputTokensOverride, pendingToolUseSummary,
  stopHookActive, turnCount,
  transition: { reason: "next_turn" }   // ← why we're continuing
};
continue;
```

Why this pattern matters:
- **Iteration-boundary atomicity.** Every exit path must rebuild the complete next state — you can't accidentally leave a stale field.
- **Snapshot semantics for async.** Tool results, hooks, and subagents run async; closures capture the destructured snapshot, so a late-arriving callback can't corrupt the next iteration (MVCC / copy-on-write thinking).
- **Budgets live in the state.** Anti-infinite-loop counters travel with the messages: retry counts, once-per-turn flags, overrides. A loop that can't retry itself infinitely is a loop with budgets in its state.
- **`transition.reason` is free telemetry.** Every loop exit stamps *why* — the entire control flow of a turn is reconstructable from a log of reasons.

**Verified in the binary (v2.1.119):** six continue sites, each a whole-object rebuild — `next_turn` (normal), `max_output_tokens_escalate`, `max_output_tokens_recovery` (bounded to 3 attempts), `malformed_tool_use_retry` (guarded against repeating itself), `reactive_compact_retry` (once per turn), `stop_hook_blocking` (Stop hook vetoed completion). The paper says seven (v2.1.88) — implementation drifted; treat the paper as a snapshot.

### 2.3 Continue sites = recovery infrastructure

The key rule observed at every continue site: **never re-loop identically — always modify the context first.** Each retry changes what the model sees:

| reason | context change before retry | guard |
|---|---|---|
| `next_turn` | append tool results | max_turns check; budgets reset |
| `reactive_compact_retry` | compacted messages | once per turn |
| `max_output_tokens_escalate` | higher output cap in state | feature-flag gated |
| `max_output_tokens_recovery` | inject "resume mid-thought, break work into pieces" meta-message | ≤ 3 attempts |
| `malformed_tool_use_retry` | inject "retry your tool call" meta-message | guard: previous reason ≠ this |
| `stop_hook_blocking` | inject hook blockingErrors | max_turns check |

Budgets are **scoped per failure mode**: token-cap failures count up, malformed-tool-use resets, reactive-compact is a boolean. Wrong scoping lets one failure mode's budget leak into another's.

**Takeaway:** a retry is only useful if it injects feedback the model can act on. An identically-repeated model call is a token furnace.

---

## 3. Context is the binding resource

The context window (200K–1M tokens) is the binding constraint. Five context-reduction strategies run **before every model call**, cheap-to-expensive:

```
budget reduction → snip → microcompact → context collapse → auto-compact
(always on)    (flag)   (flag, cache-aware)  (flag)      (model-generated summary)
```

Graduated lazy-degradation: apply the least disruptive compression first, escalate only when insufficient. Trade-off: five interacting layers, several flag-gated, are hard for users to predict — but no single strategy covers all pressure types.

### 3.1 Budget reduction (layer 1)

Per-message size limits on tool results. Oversized outputs are replaced with **content references**; the original is persisted for reconstruction on resume.

- Some tools declare `maxResultSizeChars` as non-finite → **exempt**, full output always kept (structurally small / always-needed results).
- Runs before microcompact because microcompact operates purely by `tool_use_id` and never inspects content — clean composition: each layer does its own job without understanding the others.

**Takeaway:** if a tool result is huge but already consumed, the model needs to know *the tool ran and returned something* — a reference with the persistence story satisfies that without burning the window.

### 3.2 Microcompact (layer 3): the scalpel

Fine-grained: removes small safe pieces (old tool results whose info is consumed) instead of dropping chunks or full summarization.

- **Time-based path (always on):** keep the last N tool results (`keepRecent`), replace older ones with references. Old content is persisted, so it's reconstructable.
- **Cache-aware path (feature flag):** the subtle part — see below.

### 3.3 Prompt-cache economics (why `cache_deleted_input_tokens` exists)

The API caches the **contiguous prefix** of the conversation; matching prefix = cheap `cache_read`. The cache is not infinite — it's a bounded, LRU-managed store with a TTL (5 min / 1h). `cache_deleted_input_tokens` is the API's **damage report**: tokens erased because of capacity eviction, prefix invalidation, or expiry.

Key rule: **appending never breaks the cache (old prefix stays a prefix); mutating the middle always does.**

```
turn 1: [A B C]            → cache_creation: 90K
turn 2: [A B C D]  append  → cache_read: 90K (cheap!), creation: 15K, deleted: 0
turn 3: [A B' C D] mutate  → read: ~10K, creation: 95K, deleted: ~100K
```

### 3.4 The deferral principle: mutate history only when it's free

Microcompact's cache-aware path **queues** its trims (`pendingCacheEdits`) instead of applying them before the request:

1. Send the request **as-is** → cache survives → cheap.
2. Response reports real cache numbers (`cacheReadTokens`, `cache_deleted_input_tokens`, plus a stored `prevCacheReadTokens` from a per-conversation DB).
3. **Compare actual vs actual** (reads this call vs last call, not the deletion counter alone — TTL expiry is silent):
   - cache healthy (≥ 95% of previous reads) → **drop the queued trims** (`pendingChanges = null`)
   - cache degraded → **apply the trims now** — erasing is happening anyway, so trimming is free.

The signals: the 95% read-comparison is the trigger, not `cache_deleted` (which can be 0 while the cache silently died via TTL).

```js
let w = prevCacheReadTokens - cacheReadTokens;
if (cacheReadTokens >= prevCacheReadTokens * 0.95 || w < smallThreshold) {
  pendingChanges = null;   // cache healthy → keep it, drop the trim
  return;
}
// else: apply the queued trims
```

**The general pattern (reusable):** when you have an expensive derived/cached state, batch cheap-looking mutations into the moments when that state is already being invalidated — and never be the cause of invalidation yourself. If it costs more to invalidate than the reclaim saves, decline.

**When the trim ever applies:** the moments the cache breaks "anyway" — TTL expiry during long tool runs (common), tool/schema changes, capacity eviction, or the harder shapers (auto-compact) firing. If it never breaks, microcompact simply declines and pressure is handled by the layers that pay the cost.

### 3.5 Context collapse (layer 4): read-only projection

The only **non-destructive** shaper. It does not touch the stored history at all:

- A **collapse store** (side-table) holds summaries keyed to ranges of messages.
- Before each model call, `applyCollapsesIfNeeded()` swaps in a **projected view**: recent messages + "collapsed" summaries for old ranges.
- The model sees the compact version; the transcript keeps the full truth; resume/fork reconstruct everything; collapses persist across turns because they're store metadata, not messages.

```
REPL array (truth):      [msg1..9] [msg10..200: huge] [msg201..500]
collapse store:          {10..200 → "read & edited auth.ts, fixed login flow"}
model sees:              [msg1..9] [summary line]     [msg201..500]
```

Compare: budget reduction/snip/microcompact/auto-compact all *commit* history changes (with varying persistence for reconstruction); context collapse commits nothing. Perfect auditability, no transcript pollution, free reversibility. Downside: invisible to the user — you can't easily see what was hidden.

**Takeaway:** "printing a table of contents" is sometimes better than "editing the book."

---

## 4. Safety: deny-first, layered

Default posture: **deny-first with human escalation** — deny rules always override allow rules, even a more specific allow; unrecognized actions escalate to the human instead of running silently.

Seven independent layers; any single one can block; each uses a different technique:

1. **Tool pre-filtering** — blanket-denied tools removed from the model's view before any call (model can't even attempt them).
2. **Deny-first rule evaluation** — declarative rules, content-level matching (`Bash(prefix:npm)`).
3. **Permission modes** — 7 modes from `plan` (approve plans first) through `default`, `acceptEdits`, `auto` (ML classifier), `dontAsk`, to `bypassPermissions`; plus internal `bubble` (subagent → parent escalation).
4. **Auto-mode classifier** — an ML model evaluates tool safety, can deny what rules would allow.
5. **Shell sandboxing** — filesystem/network isolation independent of authorization.
6. **No permission restore on resume** — sessions are isolated trust domains; re-grant, don't inherit.
7. **Hook interception** — PreToolUse can deny/rewrite; PermissionRequest can resolve async alongside the dialog.

Behavioral motivation: users approve ~93% of permission prompts — approval fatigue makes interactive confirmation unreliable as the *sole* safety mechanism, so safety must hold independent of vigilance.

**Known failure mode** (paper §12.3): the layers share performance constraints; e.g., commands with >50 subcommands fall back to one generic prompt because per-subcommand parsing froze the UI. Defense-in-depth degrades when layers share failure modes. Also: pre-trust initialization — hooks/MCP/settings run before the trust dialog (CVE-2025-59536, CVSS 8.7): extensibility runs before safety is engaged.

---

## 5. Extensibility: four mechanisms, ordered by context cost

Why four mechanisms instead of one? Because different extensions impose different costs on the bounded context window:

| Mechanism | Unique capability | Context cost | Insertion point |
|---|---|---|---|
| **Hooks** | lifecycle interception, event-driven | zero by default | execute(): pre/post tool |
| **Skills** | domain instructions + meta-tool invocation | low (descriptions only) | assemble(): context injection |
| **Plugins** | multi-component packaging + distribution | medium | all three points |
| **MCP servers** | external service integration (8+ transports) | high (tool schemas) | model(): tool pool |

Hooks: 27 event types (PreToolUse, PostToolUse, SessionStart/End, Stop, SubagentStart/Stop, PermissionRequest/Denied, PreCompact/PostCompact, TeammateIdle, ...). Hook outputs support five capabilities: permission decisions, **context injection**, input modification, MCP result transformation, retry control.

`hook_additions` (the pseudocode line): context that hooks push into the model's view during assembly — e.g., UserPromptSubmit injects context every turn, SessionStart once at start, PostToolUse/PostToolUseFailure add guidance. Stop hooks' `blockingErrors` are literally hook additions (we saw them injected at the `stop_hook_blocking` continue site). This is how external code shapes what the model sees *without touching the system prompt or polluting the transcript*.

**Takeaway:** a single extension API forces authors to pay high context costs for everything. Layering by context cost (free/cheap/medium/expensive) scales wide cheaply.

---

## 6. Subagents: recursion of the same harness

### 6.1 Same code, fresh instance

A subagent is **not** a simplified mini-loop and **not** the parent's loop continuing — it's a *second invocation of the identical loop machinery* with a fresh instance of all per-conversation state. Like forking a process: same binary, separate memory.

Verified in `runAgent` (the binary): it calls the same query function (`BS`) with its own messages, system prompt, tools, permission callback, querySource (`agent:...`), and maxTurns — then yields events upward and returns only the final summary.

| Shared (the code) | Isolated (the instance) |
|---|---|
| query loop implementation | messages / context window |
| tool factory + implementations | system prompt (from agent definition file) |
| permission engine, hooks, classifier | tool pool set (allow/deny lists) |
| compaction pipeline | permission context + overrides, own `canUseTool` |
| persistence code | maxTurns, session hooks registry (cleared), read-file cache (cleared), prompt-cache DB key, transcript file (sidechain) |

### 6.2 Parent-child integration = a function call with streams

Channel table:

| Direction | Channel | Content |
|---|---|---|
| parent → child | delegation prompt → first user message | the task |
| parent → child | fork context (filtered) | parent's messages seeded in, only complete tool_use→tool_result chains survive |
| parent → child | config | agentType, isolation (worktree/remote/in-process), cwd, maxTurns, model, permissionMode, allowedTools |
| child → parent | stream events | progress, attachments, hook events, api errors — rendered live in parent UI |
| child → parent | summary-only return | child's final assistant text → AgentTool's `tool_result` → parent's messages |
| child → parent | agentId | "use SendMessage to: '<id>' to continue this agent" — the child is *continuable*, not fire-and-forget |
| durable | sidechain transcript | child's full history in its own .jsonl + .meta.json (audit, never in parent context) |
| ambient | permission bubble | child's prompts can escalate to the parent's terminal (bubble mode) |

### 6.3 Recursion: agents spawning agents

AgentTool is in the **base tool pool** (always included) and `runAgent` is re-entrant — so a subagent can spawn its own subagents unless its definition disallows the tool. Nesting is tracked by a **depth counter** (`queryDepth`, incremented per nested query; rides along in telemetry and the cache tracker). No hard depth cap found in v2.1.119 — bounded by economics (each level is a full agent with its own context, model calls, cache) and per-node maxTurns. In practice depth stays shallow (2–3); the paper contrasts Hermes (depth capped at 1, leaf children can't delegate) and OpenClaw (max 5, default 1).

**Takeaway:** choose what crosses the subagent boundary deliberately — config down, summary up, agentId for continuability, streams for observability. Wall off everything else.

---

## 7. Persistence: append-only, recovery, session trust

- Transcripts are **mostly append-only JSONL** per project/session: conversation events, compaction markers, file-history snapshots. Human-readable, version-controllable, audit-friendly — favors auditability over query power.
- Compaction writes boundary+summary events; it **never modifies or deletes prior lines** (read-time chain patching via boundary UUIDs) — the append-only log is the write-ahead log; in-memory state is replayed from it.
- **Resume/fork** replays the transcript — but **session-scoped permissions are not restored** (deliberate: sessions are isolated trust domains; re-grant rather than inherit). The time-based microcompact path persists cleared content for exactly this: reconstruction on resume.
- Separate channels: session transcripts (project-scoped), global prompt history (history.jsonl), subagent sidechains.

---

## 8. Agent-team coordination: filesystem as the broker

Multi-instance coordination uses **file locking, not a message broker**. Each teammate has an inbox JSON file; messages are pushed under a per-inbox lockfile; all at predictable filesystem paths.

Mechanism (verified: the `lockfile` npm package):
- Lock = **lock *directory***; `mkdir` is atomic → mutually exclusive acquisition (only one process wins).
- Send = acquire lock (mkdir) → read inbox → append `{from, content, read: false}` → write → rmdir lock.
- Receive = read inbox, filter `read !== true` (e.g., driven by the `TeammateIdle` hook). Mark read after processing.
- Crash recovery = stale-lock detection: if lock dir mtime exceeds a threshold, remove and retry.

```
<base>/auth-fix/inboxes/
  agent-coder.json        ← messages for coder, plain JSON array
  agent-verifier.json
  agent-coder.json.lock   ← lock dir, exists only during a write
```

Trades throughput (polling, lock contention, no push) for:
- **Zero-dependency deployment** — no broker, nothing to run, pure fs.
- **Full debuggability** — any agent's state is `cat`-able plain JSON.

General pattern: mkdir-lock JSON mailboxes are the right tool when correctness, auditability, and zero-ops outweigh broker-grade throughput (a handful of agents on one machine).

---

## 9. Patterns worth stealing (the checklist)

When building a harness, in order of leverage:

1. **One State object, whole-object swap at iteration boundaries.** Atomic snapshots; budgets (retry counts, once-per-turn flags) live in the state; `transition.reason` = free trace.
2. **Never retry identically.** Every loop continuation modifies context first (inject feedback, compact, escalate) — and every retry is counter-bounded.
3. **Graduated compaction, cheap-to-expensive, before every call.** Layers should compose without understanding each other (size-level vs id-level vs semantic-level concerns).
4. **Respect cache economics.** Append never breaks the cache; middle-mutation always does. Defer mutations until the response proves the cache is already dead — then apply them free. Never speculate about cost you can read from the response.
5. **Prefer read-only projections when auditability matters.** A view over history (context collapse) beats destructive compaction.
6. **Deny-first, layered safety.** Independent techniques per layer; remove forbidden options from the model's view pre-call; don't let safety rest on human vigilance (approval fatigue is real).
7. **Extensibility by context cost.** Zero-cost interceptors (hooks) up to high-cost tool surfaces (MCP) — not one unified API.
8. **Same harness, fresh instance for delegation.** Re-enter your own loop for subagents; cross the boundary with config down, summary up, agentId for continuation; write sidechain logs.
9. **Append-only durable state with replay.** Transcripts are the log; resume/fork replay it; session-scoped trust does not persist.
10. **Files and mkdir when you don't need a broker.** Correct, inspectable, zero-dependency coordination.

The meta-lesson across all of them: **the harness's job is to make model judgment cheap to exercise, safe to exercise, and auditable after the fact** — deterministic infrastructure around a thin, unconstrained decision layer.