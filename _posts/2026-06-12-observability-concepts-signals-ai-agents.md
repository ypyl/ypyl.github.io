---
layout: post
title: "Observability for AI Agents — Concepts, Signals & the OTel Standard"
date: 2026-06-12
tags: [ai, llm, observability, opentelemetry, tracing, agents, production, langfuse]
categories: programming
series: production-ai-agents
series_index: 1
---

> Series overview: [Production AI Agents — From Notebook to Production]({% post_url 2026-06-15-production-ai-agents-series-overview %})

Observability for AI agents is traditional application performance monitoring (APM) plus new signal types, a new industry standard (OTel GenAI conventions), and tooling that understands LLM calls, tool invocations, and agent reasoning. Without it, an agent that fails silently, cost-explodes, or loops indefinitely is indistinguishable from one that works correctly.

> **Try it now**: VS Code Copilot, OpenAI Codex, and Claude Code all emit OpenTelemetry traces using GenAI semantic conventions. Enable `github.copilot.chat.otel.enabled`, point at a local OTLP endpoint (e.g., [Aspire Dashboard](https://aspire.dev/dashboard/overview/)), and see `invoke_agent` → `chat` → `execute_tool` span trees from daily development work.

## Map

```
Observability for AI Agents
│
├── CAPABILITIES — taxonomy: metrics, logs, traces, monitoring, traceability, diagnosability
│
├── STANDARD: OTel GenAI Semantic Conventions (v1.37+)
│   ├── Model spans (gen_ai.*) — LLM calls, widely adopted
│   ├── Agent spans (gen_ai.agent.*) — workflows, drafted
│   └── Agent framework conventions — in progress
│
├── SIGNALS — Traces, Metrics, Logs, Events
│   ├── Traces — LLM calls, tool invocations, plan phases
│   ├── Metrics — token usage, latency, cost, guardrail rate
│   ├── Logs — structured records with trace_id linkage
│   └── Events — discrete agent actions (API calls, handoffs)
│
├── CORRELATION — trace_id propagated through every downstream call
│
├── ARCHITECTURE
│   ├── In-process SDK — direct OTel instrumentation
│   ├── Proxy/Sidecar — transparent LLM call capture
│   ├── Framework callbacks — LangChain, Semantic Kernel hooks
│   └── Instrumentation source — baked-in (CrewAI) vs OTel library (OpenInference)
│
├── TOOLING
│   ├── Open-source platforms — Langfuse, MLflow, Agenta
│   ├── Specialized — Phoenix (RAG), Helicone (proxy)
│   ├── Full-stack — SigNoz, Datadog, Grafana
│   ├── Cloud-native — AWS Bedrock + CloudWatch, Azure Foundry
│   └── DIY — OTel SDK → Collector → existing stack
│
└── ALERTING — actionable thresholds for guardrails, cost, latency
```

## Capability Taxonomy

Observability decomposes into distinct capabilities, each answering a different question:

| Capability | Answers | For AI agents |
|------------|---------|---------------|
| **Metrics** | What changed? | Token counts, latency distributions, error rates, cost per interaction |
| **Logs** | What happened? | Prompt/completion pairs, tool inputs/outputs, guardrail decisions |
| **Traces** | Where did it happen? | LLM call → tool invocation → re-planning → response, with causal links |
| **Monitoring** | Are known failures happening? | Alerts on guardrail spikes, tool failures, cost anomalies |
| **Observability** | Can we explore unknown failures? | Ask novel questions without predefined dashboards |
| **Traceability** | What path did this request take? | Correlation IDs tying a user message to every downstream LLM and tool call |
| **Diagnosability** | How fast can we find root cause? | The faster you trace symptom to source, the better the obs design |

> **Auditability** — reconstructing *who* did *what* and *when* — sits in the **governance** layer. It consumes traceability data but adds identity, policy context, immutability, and retention.

## The Standard: OTel GenAI Conventions

The industry is converging on **OpenTelemetry GenAI Semantic Conventions** (v1.37+) via OTel's [GenAI SIG](https://github.com/open-telemetry/community/blob/main/projects/gen-ai.md). If you instrument with these conventions, telemetry is portable across Datadog, Langfuse, Phoenix, SigNoz, Grafana — any OTLP-speaking backend.

| Convention | What it covers | Status |
|-----------|---------------|--------|
| **Model spans** (`gen_ai.*`) | LLM calls — model name, token counts, temperature, provider | Development (widely adopted) |
| **Agent application** (`gen_ai.agent.*`) | Agent workflows — planning, tool calls, task execution | Draft (Google AI Agent whitepaper) |
| **Agent framework** | Common convention across frameworks (LangGraph, CrewAI, AutoGen, IBM Bee, PydanticAI) | In progress ([#1530](https://github.com/open-telemetry/semantic-conventions/issues/1530)) |

> **Application vs Framework**: The GenAI SIG distinguishes an *agent application* (individual AI entity performing tasks autonomously) from an *agent framework* (infrastructure for building agents). The application convention is drafted; the framework convention is next priority. Source: [OTel blog on evolving agent standards](https://opentelemetry.io/blog/2025/ai-agent-observability/).

### Adoption

Major providers already ship OTel GenAI:
- **Datadog LLM Observability** — native OTel GenAI spans (v1.37+) since Dec 2025
- **AWS Bedrock AgentCore** — OTel-compatible traces via ADOT
- **Microsoft Foundry** — `microsoft-opentelemetry` distro with GenAI conventions; co-developed multi-agent conventions with Cisco Outshift
- **Traceloop** (OpenLLMetry) — [donating instrumentation](https://github.com/open-telemetry/community/issues/2571) to OTel

OpenInference (Arize) adds `llm.span_kind` (LLM, TOOL, CHAIN, AGENT, RETRIEVER) on top of OTel. Industry direction is OTel-native `gen_ai.*` as the base, vendor extensions layered on top.

## Signals

### Traces

A trace for an AI agent is a tree where the agent decides the shape at runtime — how many LLM calls, which tools, how many re-planning iterations. The OTel spec defines the span types:

```
User Request (span)
├── Guardrail: input validation (span)
├── Plan: task decomposition (plan span)
│   └── LLM Call: generates plan (chat span)
│       ├── gen_ai.usage.input_tokens: 1240
│       ├── gen_ai.usage.output_tokens: 87
│       ├── gen_ai.request.model: "gpt-4o-mini"
│       └── latency_ms: 1432
├── Tool Call: search_docs("refund policy") (execute_tool span)
│   ├── input: {query: "refund policy", top_k: 5}
│   ├── output: {results: 3, total_ms: 89}
│   └── latency_ms: 91
├── LLM Call: final response (chat span)
│   ├── gen_ai.usage.input_tokens: 2890
│   ├── gen_ai.usage.output_tokens: 412
│   ├── gen_ai.request.model: "gpt-4o"
│   └── latency_ms: 3210
├── Guardrail: output validation (span)
└── Response to user
```

**Spans are dynamic** — tracing infrastructure must handle variable-depth, variable-width traces. The agent decides at runtime.

#### Span Types

The [`gen-ai-agent-spans.md`](https://github.com/open-telemetry/semantic-conventions-genai/blob/main/docs/gen-ai/gen-ai-agent-spans.md) spec defines four span types:

| Span type (Weaver registry key) | `gen_ai.operation.name` | Span kind | When to use |
|--------------------------------|------------------------|-----------|-------------|
| **Invoke agent (local)** — `gen_ai.invoke_agent.internal` | `invoke_agent` | `INTERNAL` | In-process agent invocation (LangChain, CrewAI). Span name: `invoke_agent {agent.name}` |
| **Invoke agent (remote)** — `gen_ai.invoke_agent.client` | `invoke_agent` | `CLIENT` | Remote agent services (OpenAI Assistants, Bedrock Agents). Span name: `invoke_agent {agent.name}` |
| **Invoke workflow** — `gen_ai.invoke_workflow.internal` | `invoke_workflow` | `INTERNAL` | Multi-agent orchestration (e.g., CrewAI crew). Span name: `invoke_workflow {workflow.name}` |
| **Plan** — `gen_ai.plan.internal` | `plan` | `INTERNAL` | Agent planning/task decomposition. LLM call that produces the plan is a child span (`chat`); tool spans are siblings under `invoke_agent`. Span name: `plan {agent.name}` |

Frameworks that can distinguish workflow from agent (CrewAI) SHOULD report `invoke_workflow`. Frameworks that can't (Google ADK) SHOULD NOT report `invoke_workflow` — they report `invoke_agent` for all agent types.

> **Plan vs standard inference**: `plan` spans SHOULD be reported only when instrumentation can reliably determine the operation is planning/decomposition, and SHOULD NOT be reported when it can't distinguish planning from generic reasoning. If you're intercepting raw HTTP calls, emit `chat` spans, not `plan`.

#### Span Attributes

**LLM spans** — `gen_ai.operation.name` is set to one of: `chat`, `text_completion`, `embeddings`, `generate_content`, `retrieval`.

Attributes:

| Attribute | Type | Purpose | Opt-In? |
|-----------|------|---------|---------|
| `gen_ai.request.model` | string | Model identifier (`gpt-4o-2024-08-06`) | No |
| `gen_ai.usage.input_tokens` / `gen_ai.usage.output_tokens` | int | Token counts | No |
| `gen_ai.usage.cache_read.input_tokens` / `gen_ai.usage.cache_creation.input_tokens` | int | Cached-prompt cost attribution | No |
| `gen_ai.request.temperature` / `gen_ai.request.top_p` / `gen_ai.request.max_tokens` | double/int | Request parameters | No |
| `gen_ai.provider.name` | string | Cloud/provider (`openai`, `aws.bedrock`, `anthropic`) | No |
| `gen_ai.request.seed` / `gen_ai.request.stop_sequences` / `gen_ai.request.frequency_penalty` | int/string[]/double | Advanced parameters | No |
| `gen_ai.request.stream` | boolean | Whether the request was streaming | No |
| `gen_ai.response.finish_reasons` | string[] | Stop reasons (`stop`, `length`, `tool_calls`) | No |
| `gen_ai.system.instructions` | any | System prompt | Yes |
| `gen_ai.input.messages` / `gen_ai.output.messages` | any | Full prompt/completion | Yes |
| `gen_ai.tool.definitions` | any | Tool schemas passed to model | Yes |

**Agent spans** (`invoke_agent`, `invoke_workflow`, `plan`):

| Attribute | Type | Purpose |
|-----------|------|---------|
| `gen_ai.agent.name` | string | Human-readable name (`Math Tutor`) |
| `gen_ai.agent.description` | string | Free-form description |
| `gen_ai.agent.version` | string | Semver or date (`1.0.0`, `2025-05-01`) |
| `gen_ai.conversation.id` | string | Session/thread identifier |
| `gen_ai.workflow.name` | string | Multi-agent workflow name |
| `gen_ai.data_source.id` | string | RAG/grounding data source |
| `gen_ai.output.type` | string | Output type: `text`, `json`, `image`, `speech` |

Additional `gen_ai.operation.name` values for memory and persistence — not backed by dedicated span type definitions yet, but part of the same enumeration: `create_memory_store`, `delete_memory_store`, `create_memory`, `upsert_memory`, `update_memory`, `delete_memory`, `search_memory` ([spec](https://github.com/open-telemetry/semantic-conventions-genai/blob/main/docs/gen-ai/gen-ai-agent-spans.md)).

### Metrics

OTel GenAI defines two official metrics ([blog](https://opentelemetry.io/blog/2026/genai-observability/)):
- **`gen_ai.client.operation.duration`** — histogram of LLM call latencies (filterable by `gen_ai.request.model`)
- **`gen_ai.client.token.usage`** — histogram of token consumption (filterable by `gen_ai.token.type`: `input` / `output`)

These two official metrics feed dashboards for latency and token distributions. Beyond them, track these operational signals — most require span-level queries, not just metric aggregations:

| Metric | What it measures | How to calculate | Why it matters |
|--------|-----------------|-------------------|----------------|
| **Tokens per request** | Prompt + completion tokens per agent turn | Sum `gen_ai.usage.input_tokens + output_tokens` across all LLM spans in a trace | Cost attribution, anomaly detection |
| **LLM calls per user request** | Distribution of LLM rounds | Count spans with `gen_ai.operation.name` in (`chat`, `text_completion`, `generate_content`) grouped by `trace_id` | Spot agents in infinite planning loops |
| **Tool success rate** | Per-tool error rate, latency distribution | Filter spans where `gen_ai.operation.name=execute_tool`; error rate = count(status=ERROR) / total per tool | Flaky/slow tools |
| **Cache hit rate** | Prompt/embedding/cache hit rate | `gen_ai.usage.cache_read.input_tokens` / (`gen_ai.usage.cache_read.input_tokens + gen_ai.usage.input_tokens`) | ROI of caching infrastructure |
| **Time to first token (TTFT)** | Request → first response token | `gen_ai.response.time_to_first_chunk` — span attribute, set for streaming requests | Perceived responsiveness |
| **Rate limit hits** | 429 / rate-limit count, queue depth | Provider-specific: check HTTP 429 status on LLM spans or provider error response fields. Not yet covered by `gen_ai.response.finish_reasons`. Group by provider/model | Capacity planning, fallback models |
| **Guardrail trigger rate** | Input/output guardrail fire rate | Count spans tagged with guardrail trigger attributes (vendor-specific convention) vs total requests | Content safety, prompt injection trends |
| **Cost per successful interaction** | Total cost per completed request | Sum (input_tokens × input_price + output_tokens × output_price + cache_tokens × cache_price) per `trace_id`, divided by count of traces with success status | Example: GPT-4o at 5K in + 2K out ≈ $0.03. Complex agent with re-planning (20K in + 5K out) ≈ $0.10 (GPT-4o) to $0.26 (o3-pro). Pricing from [models.dev](https://models.dev/api.json) |

### Logs

**From the framework**: when instrumented with OTel, agent frameworks emit spans (traces) following GenAI semantic conventions — each LLM call, tool invocation, and planning step produces a span with `gen_ai.operation.name`, token counts, model name, and other attributes. Log records are a separate OTel signal — frameworks may or may not emit them depending on their instrumentation depth.

**From your code**: integrate OTel logging to produce structured log records. When a span is active, the OTel SDK automatically attaches `trace_id` and `span_id` to every log record — no manual correlation code. Add application-specific fields as structured attributes (agent turn number, decision context, eval results).

**Capturing prompt/completion content**: frameworks can populate `gen_ai.input.messages` and `gen_ai.output.messages` span attributes with full prompt and response text — but this is **opt-in** (disabled by default, since prompts can contain sensitive data). Turn it on via instrumentation config (e.g., `github.copilot.chat.otel.captureContent=true`) rather than writing your own capture code. When enabled, the raw text is large — strategies to control storage cost:

| Strategy | How it works | Best for |
|----------|-------------|----------|
| **Sampling** | Log full prompt/completion for 5% of requests; for the other 95%, log only token counts and model name | Production at scale — gives you enough samples for eval without the storage cost |
| **Separate store** | Write full prompts to cheap blob storage (S3, Azure Blob). Store only a blob pointer + metadata in your log aggregator | When you need 100% capture for compliance or audit trails |
| **Eval pipeline** | Stream sampled traces to an evaluation pipeline that scores them (relevance, groundedness, safety) and discards raw text. Store only the scores | Continuous quality monitoring — you watch scores, not raw text |

### Events (The Fourth Signal)

Discrete, semantically meaningful agent actions — structured records, not raw log lines. [IBM's observability framework](https://www.ibm.com/think/insights/ai-agent-observability) calls this the MELT model, where Events is the fourth signal alongside Metrics, Logs, and Traces:

| Event type | Example | Why it matters |
|-----------|---------|----------------|
| **API call** | Agent calls search API | Track tool usage and cost |
| **LLM call** | Agent sends prompt to GPT-4o | Quality analysis |
| **Failed tool call** | DB query returns connection error | Alert, root cause |
| **Human handoff** | Agent escalates refund dispute | Autonomy rate, capability gaps |
| **Guardrail trigger** | Output filter blocks response | Safety system effectiveness |

OTel [gen-ai-events.md](https://github.com/open-telemetry/semantic-conventions-genai/blob/main/docs/gen-ai/gen-ai-events.md) defines structured input/output message schemas (JSON). Events are less granular than spans but more structured than free-text logs — useful for audit trails and compliance.

> The companion article [Architecture, Tooling & Alerting]({% post_url 2026-06-12-observability-architecture-tooling-ai-agents %}) covers correlation, evaluation, instrumentation patterns, the tooling landscape, and alerting thresholds.

## References

- [OTel GenAI Semantic Conventions (v1.37+)](https://opentelemetry.io/docs/specs/semconv/gen-ai/)
- [OTel GenAI Events (gen-ai-events.md)](https://github.com/open-telemetry/semantic-conventions-genai/blob/main/docs/gen-ai/gen-ai-events.md)
- [OTel semantic-conventions-genai repo](https://github.com/open-telemetry/semantic-conventions-genai)
- [GenAI Agent Spans (gen-ai-agent-spans.md)](https://github.com/open-telemetry/semantic-conventions-genai/blob/main/docs/gen-ai/gen-ai-agent-spans.md)
- [OTel Blog: AI Agent Observability — Evolving Standards (2025)](https://opentelemetry.io/blog/2025/ai-agent-observability/)
- [OTel Blog: GenAI Observability with OpenTelemetry (2026)](https://opentelemetry.io/blog/2026/genai-observability/)
- [IBM: Why observability is essential for AI agents](https://www.ibm.com/think/insights/ai-agent-observability)
- [OTel Issue #1530: Agent Framework Semantic Convention](https://github.com/open-telemetry/semantic-conventions/issues/1530)
- [OpenInference](https://github.com/Arize-ai/openinference)
- [models.dev — model pricing API](https://models.dev/api.json)
