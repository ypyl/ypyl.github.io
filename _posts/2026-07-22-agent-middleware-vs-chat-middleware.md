---
layout: post
title: "Agent Middleware vs. Chat Middleware in Microsoft Agent Framework"
date: 2026-07-22
tags: [agent-framework, middleware, python, dotnet, go, ai-agents]
categories: ai
---

Microsoft Agent Framework offers three flavours of middleware, but two in particular — **agent middleware** and **chat middleware** — sit at different layers of the pipeline and serve fundamentally different purposes. Getting them right is the difference between a clean, portable guard that works across every agent type, and a fine-grained hook that breaks the moment you switch backends.

Let's start from the big picture and work down through the layers.

---

## The Pipeline: Where Everything Lives

Every agent invocation flows through a layered pipeline. Understanding the nesting is the key to understanding the middleware:

```
┌─────────────────────────────────────────────────┐
│  Agent Middleware (outer layer)                 │
│  ┌───────────────────────────────────────────┐  │
│  │  History Provider: loads past messages    │  │
│  │  Context Providers: inject RAG, memory,   │  │
│  │    instructions, tools                    │  │
│  │  ┌─────────────────────────────────────┐  │  │
│  │  │  Function Invocation Loop           │  │  │
│  │  │  ┌───────────────────────────────┐  │  │  │
│  │  │  │  Chat Middleware              │  │  │  │
│  │  │  │  ┌─────────────────────────┐  │  │  │  │
│  │  │  │  │  RawChatClient → LLM    │  │  │  │  │
│  │  │  │  └─────────────────────────┘  │  │  │  │
│  │  │  └───────────────────────────────┘  │  │  │
│  │  │  Function Middleware (per tool call)│  │  │
│  │  └─────────────────────────────────────┘  │  │
│  └───────────────────────────────────────────┘  │
└─────────────────────────────────────────────────┘
```

- **Agent middleware** wraps the entire run: history loading, context injection, the whole chat client interaction, everything. It's the outermost layer.
- **Chat middleware** wraps *each individual call to the LLM*. It sits deep inside the function invocation loop, meaning it fires for every turn — the initial request, and every follow-up that sends tool results back to the model.
- **Function middleware** (not the focus here, but good to know) wraps individual tool/function calls within the loop.

---

## Agent Middleware: The Outer Guard

### What it wraps

The entire agent lifecycle. When you call `agent.run()`, agent middleware is the first thing entered and the last thing exited. It sees messages before history is loaded, before context providers inject their bits, before the chat client is ever touched — and it sees the final response after everything has unwound.

### What's in the context

The Python `AgentContext` gives you:

| Field | What it holds |
|-------|---------------|
| `messages` | The full list of chat messages at this point in the pipeline |
| `session` | The current `AgentSession`, if any |
| `options` | Agent run options for this invocation |
| `stream` | Boolean — is this a streaming response? |
| `metadata` | A free-form dict for passing data between middleware |
| `result` | The agent's response — you can *mutate or replace* this |
| `function_invocation_kwargs` | Runtime values forwarded to tools |
| `client_kwargs` | Runtime values forwarded to the chat client |

### What it can do

The superpower of agent middleware is that it can **terminate the entire run early**. Don't call `call_next()` and set `context.result` — done. The LLM is never invoked, no tools run, nothing.

This is the right layer for:
- **Security guards**: block requests containing secrets, PII, or forbidden topics before anything expensive happens.
- **Input sanitisation**: strip or rewrite user messages regardless of backend.
- **Global auditing/logging**: measure end-to-end latency, log every interaction.
- **Response overrides**: replace or transform the final output (works for both streaming and non-streaming).

### The critical advantage: portability

Agent middleware works with **any** agent type — `ChatClientAgent`, `A2AAgent`, `GitHubCopilotAgent`, `CopilotStudioAgent`. It doesn't care who or what the backend is. If your guard logic should apply universally, this is the layer.

---

## Chat Middleware: The Inner Hook

### What it wraps

A single call to the AI model. Not the whole run — just one request/response cycle to the LLM. Critically, it sits **inside the function invocation loop**:

```
User asks "What's the weather in Seattle?"
  → Chat middleware fires: model receives query, responds with tool call
  → Function middleware fires: get_weather("Seattle") executes
  → Chat middleware fires AGAIN: model receives tool result, responds with final answer
```

That's two chat middleware invocations for one `agent.run()`. If the model calls three tools in sequence, chat middleware fires four times (one initial + three tool-result follow-ups).

### What's in the context

The Python `ChatContext` gives you:

| Field | What it holds |
|-------|---------------|
| `client` | The chat client being invoked |
| `messages` | Messages being sent to the AI service *right now* |
| `options` | Options for this specific chat request |
| `stream` | Boolean — is this a streaming call? |
| `metadata` | Free-form dict for cross-middleware data |
| `result` | The raw chat response — mutable |
| `function_invocation_kwargs` | Tool-only values forwarded at this layer |

### What it can do

Chat middleware sees the exact payload heading to the LLM — after history and context providers have done their work, after the tool-calling loop has assembled the messages for this turn. It can:

- **Inspect or modify the raw prompt** before it reaches the model.
- **Log individual model calls** with token counts, latency, and the actual messages sent.
- **Enrich messages with dynamic context** at the last moment (e.g., injecting a system reminder about the current time before every turn).
- **Transform or filter the raw response** before it enters the tool-calling loop or returns to the agent.

### The limitation: tied to IChatClient

Chat middleware only makes sense for agents backed by a local chat client. `A2AAgent` and `GitHubCopilotAgent` don't use one — their "model calls" go to remote services over protocols that don't expose a chat client hook. Trying to use chat middleware with them is a no-op at best, a misunderstanding at worst.

---

## Side-by-Side Comparison

| | Agent Middleware | Chat Middleware |
|---|---|---|
| **Scope** | Entire `agent.run()` lifecycle | Single LLM call |
| **Fires per run** | Once (plus once per streaming response) | Multiple times (once per model turn in the tool loop) |
| **Sees** | Input messages before history/context, final response after everything | Messages just before they hit the LLM, raw response |
| **Can terminate early** | Yes — don't call `call_next()`, set `result` | Yes — but only terminates that model call, not the whole run |
| **Agent type compatibility** | All types (ChatClientAgent, A2AAgent, GitHubCopilotAgent, etc.) | Only agents with a local chat client (ChatClientAgent, direct Agent with SupportsChatGetResponse) |
| **Registration** | `Agent(middleware=[...])` or per-run `agent.run(middleware=[...])` | Via chat client builder or context providers |
| **Best for** | Security, audit, global transforms | Prompt inspection, per-turn enrichment, raw response manipulation |

---

## Registration and Nesting

### Agent middleware: two tiers

Agent middleware can be registered at **agent level** (applies to every run, configured once) or at **run level** (applies to a single invocation, layered *inside* agent-level middleware). Execution order when both are present: `Agent[A1, A2] → Run[R1] → Agent core → R1 → A2 → A1`.

### Chat middleware: three paths

1. **Chat client builder** — registered on the chat client *before* it's handed to the agent. Applies to every model call.
2. **Context provider attachment** — context providers can attach chat middleware dynamically per invocation via `context.extend_middleware()`, giving you middleware that's decided at runtime and still sits at the right layer (inside the tool loop).

---

## When to Use Which

**Use agent middleware when you need:**
- A security guard that blocks before *any* work is done.
- End-to-end timing or telemetry across the full run.
- A guard that must work across multiple agent types (ChatClientAgent + A2AAgent + GitHubCopilotAgent).
- Global response transformation (e.g., stripping PII from output).

**Use chat middleware when you need:**
- To inspect or modify the exact prompt going to the LLM on each turn.
- Per-model-call logging with token counts and precise timing.
- To inject context that should be refreshed *every turn* (e.g., "the current time is...").
- To work at a finer granularity inside the tool-calling loop.

**You'll often use both.** A common pattern: agent middleware for authentication/authorization and global telemetry, chat middleware for prompt debugging and per-turn enrichment. They nest cleanly — the agent middleware wrapping the whole show, chat middleware firing inside the loop.

---

*Found this useful? Check out the official docs on [Agent Pipeline](https://learn.microsoft.com/en-us/agent-framework/agents/agent-pipeline) and [Middleware](https://learn.microsoft.com/en-us/agent-framework/agents/middleware/).*
