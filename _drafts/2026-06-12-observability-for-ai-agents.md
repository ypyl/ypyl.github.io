---
layout: post
title: "Observability for AI Agents — Beyond APM"
date: 2026-06-12
tags: [ai, llm, observability, opentelemetry, tracing, agents, production, langfuse]
categories: programming
series: production-ai-agents
series_index: 1
---

> Series overview: [Production AI Agents — From Notebook to Production]({% post_url 2026-06-15-production-ai-agents-series-overview %})

> **Try it now**: VS Code Copilot, OpenAI Codex, and Claude Code all emit OpenTelemetry traces using GenAI semantic conventions. Enable `github.copilot.chat.otel.enabled`, point at a local OTLP endpoint (e.g., [Aspire Dashboard](https://aspire.dev/dashboard/overview/)), and see `invoke_agent` → `chat` → `execute_tool` span trees from daily development work.

## Observability Taxonomy

| Capability | Answers | For AI agents |
|------------|---------|---------------|
| **Metrics** | What changed? | Token counts, latency distributions, error rates, cost per interaction |
| **Logs** | What happened? | Prompt/completion pairs, tool inputs/outputs, guardrail decisions |
| **Traces** | Where did it happen? | LLM call → tool invocation → re-planning → response, with causal links |
| **Monitoring** | Are known failures happening? | Alerts on guardrail spikes, tool failures, cost anomalies |
| **Observability** | Can we explore unknown failures? | Ask novel questions without predefined dashboards |
| **Traceability** | What path did this request take? | Correlation IDs tying a user message to every downstream LLM and tool call |
| **Diagnosability** | How fast can we find root cause? | The faster you trace symptom to source, the better the obs design |

> **Auditability** — reconstructing *who* did *what* and *when* — sits in the **governance** layer (Part 3). It consumes traceability data but adds identity, policy context, immutability, and retention.

## The OTel GenAI Standard

The industry is converging on **OpenTelemetry GenAI Semantic Conventions** (v1.37+) via OTel's [GenAI SIG](https://github.com/open-telemetry/community/blob/main/projects/gen-ai.md).

| Convention | What it covers | Status |
|-----------|---------------|--------|
| **Model spans** (`gen_ai.*`) | LLM calls — model name, token counts, temperature, provider | Development (widely adopted) |
| **Agent application** (`gen_ai.agent.*`) | Agent workflows — planning, tool calls, task execution | Draft (Google AI Agent whitepaper) |
| **Agent framework** | Common convention across frameworks (LangGraph, CrewAI, AutoGen, IBM Bee, PydanticAI) | In progress ([#1530](https://github.com/open-telemetry/semantic-conventions/issues/1530)) |

> **Application vs Framework**: The GenAI SIG distinguishes an *agent application* (individual AI entity performing tasks autonomously) from an *agent framework* (infrastructure for building agents). The application convention is drafted; the framework convention is next priority.

Instrument with OTel GenAI conventions and telemetry is portable across Datadog, Langfuse, Phoenix, Agenta, Dynatrace, Grafana — any OTLP-speaking backend.

Major providers already adopt this:
- **Datadog LLM Observability** — native OTel GenAI spans (v1.37+) since Dec 2025
- **AWS Bedrock AgentCore** — OTel-compatible traces via ADOT
- **Microsoft Foundry** — `microsoft-opentelemetry` distro with GenAI conventions; co-developed multi-agent conventions with Cisco Outshift
- **Traceloop** (OpenLLMetry) — [donating instrumentation](https://github.com/open-telemetry/community/issues/2571) to OTel

OpenInference (Arize) adds LLM-specific attributes on top of OTel; industry direction is OTel-native `gen_ai.*` as the base with vendor extensions layered on top.

## The Three Signals, Reinterpreted

### Traces

Traditional trace = tree of service-to-service RPC calls. AI agent trace tree gains new node types (per OTel spec: `plan` wraps the planning LLM call, `execute_tool` wraps tool calls, `chat` wraps inference):

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

**Spans are dynamic** — you don't know trace shape ahead of time. Agent decides at runtime which tools to call and how many LLM rounds to execute. Tracing infrastructure must handle variable-depth, variable-width traces.

The [`gen-ai-agent-spans.md`](https://github.com/open-telemetry/semantic-conventions-genai/blob/main/docs/gen-ai/gen-ai-agent-spans.md) spec defines four span types with `gen_ai.operation.name`:

| Span type (Weaver registry key) | `gen_ai.operation.name` | Span kind | When to use |
|--------------------------------|------------------------|-----------|-------------|
| **Invoke agent (local)** — `gen_ai.invoke_agent.internal` | `invoke_agent` | `INTERNAL` | In-process agent invocation (LangChain, CrewAI). Span name: `invoke_agent {agent.name}` |
| **Invoke agent (remote)** — `gen_ai.invoke_agent.client` | `invoke_agent` | `CLIENT` | Remote agent services (OpenAI Assistants, Bedrock Agents). Span name: `invoke_agent {agent.name}` |
| **Invoke workflow** — `gen_ai.invoke_workflow.internal` | `invoke_workflow` | `INTERNAL` | Multi-agent orchestration (e.g., CrewAI crew). Span name: `invoke_workflow {workflow.name}` |
| **Plan** — `gen_ai.plan.internal` | `plan` | `INTERNAL` | Agent planning/task decomposition. LLM call that produces the plan is a child span (`chat`); tool spans from plan are siblings under `invoke_agent`. Span name: `plan {agent.name}` |

CrewAI crews SHOULD report `invoke_workflow`; Google ADK workflow agents that report `invoke_agent` SHOULD NOT report `invoke_workflow` since they can't reliably distinguish them.

> **Plan vs standard inference** — The spec says `plan` spans SHOULD be reported only when instrumentation can reliably determine the operation is planning/decomposition, and SHOULD NOT be reported when it can't distinguish planning from generic reasoning. If you're intercepting raw HTTP calls, emit `chat` spans, not `plan`.

**LLM span attributes** (OTel GenAI conventions):
- `gen_ai.request.model` — model identifier (e.g., `gpt-4o-2024-08-06`)
- `gen_ai.usage.input_tokens` / `gen_ai.usage.output_tokens`
- `gen_ai.usage.cache_read.input_tokens` / `gen_ai.usage.cache_creation.input_tokens` — cached-prompt cost attribution
- `gen_ai.request.temperature` / `gen_ai.request.top_p` / `gen_ai.request.max_tokens`
- `gen_ai.provider.name` — cloud/provider (e.g., `openai`, `aws.bedrock`, `azure.ai.openai`, `anthropic`)
- `gen_ai.operation.name` — operation type: `chat`, `text_completion`, `embeddings`, `generate_content`, `retrieval`
- `gen_ai.request.seed` / `gen_ai.request.stop_sequences` / `gen_ai.request.frequency_penalty`
- `gen_ai.response.finish_reasons` — why model stopped generating
- `gen_ai.system.instructions` — system prompt (Opt-In)
- `gen_ai.input.messages` / `gen_ai.output.messages` — full prompt/completion (Opt-In)
- `gen_ai.tool.definitions` — tool schemas passed to model (Opt-In)

**Agent-level span attributes** (`invoke_agent`, `invoke_workflow`, `plan`):
- `gen_ai.agent.name` — human-readable name
- `gen_ai.agent.description` — free-form description
- `gen_ai.agent.version` — semver or date
- `gen_ai.conversation.id` — session/thread identifier
- `gen_ai.workflow.name` — multi-agent workflow name
- `gen_ai.data_source.id` — RAG/grounding data source identifier
- `gen_ai.output.type` — what agent produces: `text`, `json`, `image`, `speech`

**Memory operations** ([`gen-ai-agent-spans.md`](https://github.com/open-telemetry/semantic-conventions-genai/blob/main/docs/gen-ai/gen-ai-agent-spans.md)): `create_memory_store`, `delete_memory_store`, `create_memory`, `upsert_memory`, `update_memory`, `delete_memory`, `search_memory` — all as `gen_ai.operation.name` values.

**OpenInference** adds `llm.span_kind` (LLM, TOOL, CHAIN, AGENT, RETRIEVER). OTel community is incorporating these into official conventions.

### Metrics

OTel GenAI conventions define two official metrics:
- **`gen_ai.client.operation.duration`** — histogram of LLM call latencies (filterable by `gen_ai.request.model`)
- **`gen_ai.client.token.usage`** — histogram of token consumption (filterable by `gen_ai.token.type`: `input` / `output`)

Operational metrics derived from these:

| Metric | What it measures | Why it matters |
|--------|-----------------|----------------|
| **Tokens per request** | Prompt + completion tokens per agent turn | Cost attribution, anomaly detection |
| **LLM calls per user request** | Distribution of LLM rounds | Spot agents in infinite planning loops |
| **Tool success rate** | Per-tool error rate, latency distribution | Flaky/slow tools |
| **Cache hit rate** | Prompt/embedding/cache hit rate | ROI of caching infrastructure |
| **Time to first token (TTFT)** | Request → first response token | Perceived responsiveness |
| **Rate limit hits** | 429 count, queue depth | Capacity planning, fallback models |
| **Guardrail trigger rate** | Input/output guardrail fire rate | Content safety, prompt injection trends |
| **Cost per successful interaction** | Total cost for completed request | Viability: $0.80/request is fine for internal tools, bankrupting for consumer |

### Logs

Structured and queryable — not free-text. Each entry carries:
- `trace_id` — ties to distributed trace
- `agent_turn` — conversation turn number
- `span_kind` — `LLM`, `TOOL`, `GUARDRAIL`, `EVAL`
- `decision_context` — what agent was trying to do

Full prompt/completion capture for offline analysis. Approaches to control cost:
- **Sampling**: full prompts for 5% of requests, summary for rest
- **Separate store**: prompts to blob storage, metadata+pointer to log aggregator
- **Evaluation pipeline**: stream sampled traces to eval pipeline, store only scores

### Events (The Fourth Signal)

Discrete, semantically meaningful agent actions — structured records, not raw log lines:

| Event type | Example | Why it matters |
|-----------|---------|----------------|
| **API call** | Agent calls search API | Track tool usage and cost |
| **LLM call** | Agent sends prompt to GPT-4o | Quality analysis |
| **Failed tool call** | DB query returns connection error | Alert, root cause |
| **Human handoff** | Agent escalates refund dispute | Autonomy rate, capability gaps |
| **Guardrail trigger** | Output filter blocks response | Safety system effectiveness |

OTel [gen-ai-events.md](https://github.com/open-telemetry/semantic-conventions-genai/blob/main/docs/gen-ai/gen-ai-events.md) defines structured input/output message schemas (JSON). Events are less granular than spans but more structured than free-text logs — useful for audit trails and compliance.

## The Correlation Problem

End-to-end correlation: a single user message triggers planning → parallel tool calls → internal services → vector DB → external API → final LLM response → guardrails. All tied to one `trace_id` generated at the entry point and propagated via:
- HTTP headers (`X-Trace-Id` or W3C `traceparent`)
- Message queue metadata
- LLM provider `user` / `metadata` fields

If the agent framework doesn't propagate automatically, wrap LLM and tool calls in a thin instrumentation layer.

## Evaluation as Observability

Production traces feed evaluation:
1. Sample production traces (5-10% traffic) into eval pipeline
2. Run LLM-as-judge evaluators: relevance, coherence, groundedness, safety
3. Compare against reference answers when available
4. Surface regressions — model/prompt change causing score drops?
5. Alert when groundedness drops below threshold for >10 min

Tools like Langfuse and Braintrust build this loop natively.

## Architecture Patterns

### 1. In-Process SDK

`Agent code → OpenTelemetry SDK → OTLP exporter → Collector`
Full control, couples instrumentation to agent code. Best for custom frameworks, small teams.

### 2. Proxy/Sidecar

`Agent code → localhost:4000 (LiteLLM) → OpenAI API`
No code changes. Captures LLM calls transparently. Best for brownfield systems, third-party frameworks you can't modify.
**Caveat**: sees LLM calls but not tool calls or internal logic — combine with in-process spans for full traces.

### 3. Framework-Native Callbacks

Agent framework hooks (LangChain callbacks, Semantic Kernel filters, AutoGen middleware). Register observers that fire on each LLM call, tool invocation, and agent turn. Best for framework-aware instrumentation (chain type capture, agent-to-agent handoff).

### 4. Baked-in vs OTel Library Instrumentation (from OTel GenAI SIG)

| Approach | Who provides it | Examples | Trade-off |
|----------|----------------|---------|-----------|
| **Baked-in** | Framework maintainers | CrewAI (native OTel) | Simplest adoption; framework bloat, OTel dependency lag |
| **OTel package** | Community/vendor OTel library | OpenInference, `microsoft-opentelemetry`, Langtrace, `instrumentation-genai` (forthcoming) | Decoupled obs from framework; fragmentation risk |

Traceloop's OpenLLMetry being donated to OTel moves instrumentation into OTel-owned repos. Long-term goal per OTel blog.

## Tooling Landscape

| Category | Tools | Best for |
|----------|-------|----------|
| **Open-source platforms** | Langfuse (29k ★, MIT), MLflow (Apache 2.0), Agenta (MIT), Comet Opik (Apache 2.0) | Full obs + eval + prompt mgmt, self-hosted |
| **Specialized tools** | Phoenix (RAG/drift), Helicone (proxy), AgentOps (sessions), TruLens (eval-first) | Niche: retrieval quality, quick setup, session tracking |
| **Full-stack + LLM** | SigNoz (OTel-native), Datadog, Grafana | Unified APM + LLM obs |
| **Cloud-native platforms** | AWS: Bedrock AgentCore + CloudWatch GenAI Obs (GA Oct '25). Azure: Foundry Observability | Obs integrated into the AI platform itself |
| **Gateways + obs** | Portkey, LiteLLM proxy | Multi-provider routing + unified logging |
| **Instrumentation libs** | OpenLIT, OpenLLMetry, Langtrace | Zero-code OTel instrumentation |
| **SaaS platforms** | LangSmith, Braintrust, W&B Weave, Galileo | Managed, opinionated, framework-specific |

## Tool Deep Dives

### Langfuse
- 29k GitHub stars, MIT, 10B+ obs/month, 19 of Fortune 50
- Acquired by ClickHouse (early 2026)
- Full trace viewer with LLM-specific rendering, built-in eval pipeline, prompt management
- 80+ integrations: LangChain, CrewAI, Pydantic AI, Vercel AI SDK, Google ADK, Microsoft Agent Framework, all major providers, LiteLLM gateway
- SDKs: Python, JS/TS native; Go, Java, **.NET**, Ruby, PHP, Swift via OTel
- Self-hosting: Docker Compose, K8s (Helm), AWS/GCP/Azure (Terraform)
- Enterprise: SOC 2 Type II, ISO 27001, GDPR, HIPAA eligible, EU & US data regions
- Agent integration: MCP servers, CLI, coding agent skills for Claude Code, Cursor, Codex
- Free tier: 50k observations/month

### Agenta
- OTel-native, MIT, SOC 2 Type II, self-hostable
- End-to-end: link prompt versions to traces, online + offline evals, experiment comparison
- UI designed for engineers, PMs, and domain experts
- Free tier: 10k traces/month

### Phoenix by Arize
- Apache 2.0, Jupyter-native
- Embedding drift detection, retrieval evaluation (NDCG, MRR), trace+span viewer
- Notebook-first workflow for data scientists
- Strong for RAG-heavy agents

### OpenLIT
- Zero-code OTel auto-instrumentation for OpenAI, Anthropic, Cohere, HuggingFace, Ollama, etc.
- Auto-captures token counts, latency, costs, model parameters → standard OTel → any OTLP backend
- Featured in [official OTel blog](https://opentelemetry.io/blog/2024/llm-observability/)

### MLflow
- Apache 2.0, most established ML platform extending into GenAI
- LLM tracing: prompt versioning, trace replay (reproduce failure sequences), LLM-as-Judge eval
- Self-hosted or Databricks-managed
- Native LangChain, LlamaIndex, OpenAI, Anthropic, Hugging Face integrations

### Helicone
- Proxy-based: change API base URL + auth header → logging
- 100+ models, no code changes
- Built-in caching, rate limiting, automatic failover
- Cost tracking by model/user/feature
- Not designed for deep agent reasoning — sees LLM calls only

### SigNoz
- OTel-native full-stack platform, open-source
- Correlates LLM traces with infra metrics, K8s pod data, DB queries, microservice traces in one view
- End-to-end waterfall views for multi-agent workflows
- Custom dashboards + alerts on any telemetry
- MCP server exposes telemetry to AI assistants for automated troubleshooting
- Self-hosted community edition or cloud (30-day trial)

### Microsoft Foundry (Azure)
- Built into Azure, powered by Azure Monitor Application Insights
- Zero-code auto-instrumentation for Microsoft Agent Framework, Semantic Kernel
- One-line setup for LangChain, LangGraph, OpenAI Agents SDK via `microsoft-opentelemetry` distro
- Multi-agent conventions co-developed with Cisco Outshift: `execute_task`, `agent_to_agent_interaction`, `agent.state.management`, `agent_planning`, `agent orchestration` spans
- Pre-built evaluators: general quality, RAG quality, safety/security, agent quality
- Content Safety APIs with severity scoring (0-7)
- Security: content recording disabled via `OTEL_INSTRUMENTATION_GENAI_CAPTURE_MESSAGE_CONTENT`
- Dynatrace and Arize integrations
- Traces stored in your own Application Insights instance

### AWS: Bedrock AgentCore + CloudWatch GenAI
- ADOT auto-instruments Strands, LangChain, LangGraph, CrewAI
- Two pre-built dashboards: Model Invocations (token usage, latency P90/P99, errors, cost) and AgentCore Agents (Agents, Memory, Built-in Tools, Gateways, Identity)
- AgentCore Evaluations (Dec '25): auto quality assessment integrated into CloudWatch
- End-to-end prompt tracing through models → knowledge bases → guardrails → tools; X-Ray + W3C `traceparent`
- Session propagation via `session.id` in OTEL baggage
- Third-party routing: `DISABLE_ADOT_OBSERVABILITY=true` → Langfuse, Datadog, or any OTel backend
- Unified infra + AI monitoring in same CloudWatch console

### DIY with OpenTelemetry

`Agent → OTel SDK + gen_ai conventions → OTel Collector → Grafana Tempo/Mimir/Loki | Datadog | Honeycomb | Jaeger`

Use OTel GenAI Semantic Conventions (v1.37+) as data vocabulary. Add lightweight instrumentation (OpenLIT, OpenLLMetry) for auto-capture of LLM and vector DB calls. Governance policies (redaction, sampling) enforced at OTel Collector level before data leaves network.

Local development: [Aspire Dashboard](https://aspire.dev/dashboard/overview/) — free OTLP viewer, single Docker container, GenAI span visualizer:
```sh
docker run --rm -p 18888:18888 -p 4317:18889 -p 4318:18890 -d --name aspire-dashboard \
    -e ASPIRE_DASHBOARD_UNSECURED_ALLOW_ANONYMOUS=true \
    mcr.microsoft.com/dotnet/aspire-dashboard:latest
```

## Alert Thresholds

Focus on actionable degradation:

| Alert | Threshold (example) | Action |
|-------|---------------------|--------|
| Guardrail trigger rate spike | >10% of requests trigger output guardrail (vs 2% baseline) | Roll back prompt/model change |
| Tool failure rate | >5% error rate on any tool for >5 min | Check downstream service health |
| Token explosion | p99 token count >3× rolling average | Agent may be in reasoning loop |
| Eval score degradation | Groundedness score drops >0.2 for >15 min | Check model provider change |
| Cost anomaly | Cost per 1K requests >2× daily baseline | Audit for prompt bloat, routing issues |

## References

- [OTel GenAI Semantic Conventions (v1.37+)](https://opentelemetry.io/docs/specs/semconv/gen-ai/)
- [OTel GenAI Events (gen-ai-events.md)](https://github.com/open-telemetry/semantic-conventions-genai/blob/main/docs/gen-ai/gen-ai-events.md)
- [OTel semantic-conventions-genai repo](https://github.com/open-telemetry/semantic-conventions-genai)
- [GenAI Agent Spans (gen-ai-agent-spans.md)](https://github.com/open-telemetry/semantic-conventions-genai/blob/main/docs/gen-ai/gen-ai-agent-spans.md)
- [OTel Blog: AI Agent Observability — Evolving Standards (2025)](https://opentelemetry.io/blog/2025/ai-agent-observability/)
- [OTel Blog: GenAI Observability with OpenTelemetry (2026)](https://opentelemetry.io/blog/2026/genai-observability/)
- [Aspire Dashboard: GenAI telemetry visualizer](https://aspire.dev/dashboard/explore/#genai-telemetry-visualization)
- [IBM: Why observability is essential for AI agents](https://www.ibm.com/think/insights/ai-agent-observability)
- [OTel Issue #1732: Agent Application Semantic Convention](https://github.com/open-telemetry/semantic-conventions/issues/1732)
- [OTel Issue #1530: Agent Framework Semantic Convention](https://github.com/open-telemetry/semantic-conventions/issues/1530)
- [Langfuse](https://langfuse.com/)
- [MLflow: GenAI observability](https://mlflow.org/genai/observability)
- [Agenta](https://agenta.ai/)
- [SigNoz](https://signoz.io/)
- [Helicone](https://helicone.ai/)
- [OpenLIT](https://github.com/openlit/openlit)
- [OpenLLMetry](https://github.com/traceloop/openllmetry)
- [Langtrace](https://github.com/Scale3-Labs/langtrace-python-sdk)
- [OTel instrumentation-genai (Python contrib)](https://github.com/open-telemetry/opentelemetry-python-contrib/tree/main/instrumentation-genai)
- [OpenInference](https://github.com/Arize-ai/openinference)
- [Phoenix by Arize](https://github.com/Arize-ai/phoenix)
- [Microsoft Foundry: Trace agent frameworks](https://learn.microsoft.com/en-us/azure/foundry/observability/how-to/trace-agent-framework)
- [Microsoft Foundry: Observability concepts](https://learn.microsoft.com/en-us/azure/foundry/observability/concepts/trace-agent-concept)
- [AWS: Bedrock AgentCore + Langfuse integration](https://aws.amazon.com/blogs/machine-learning/amazon-bedrock-agentcore-observability-with-langfuse/)
- [AWS: CloudWatch GenAI observability](https://docs.aws.amazon.com/AmazonCloudWatch/latest/monitoring/GenAI-observability.html)
- [AWS: Bedrock AgentCore observability config](https://docs.aws.amazon.com/bedrock-agentcore/latest/devguide/observability-configure.html)
- [LiteLLM proxy](https://docs.litellm.ai/docs/proxy/quick_start)
- [OpenTelemetry for .NET](https://opentelemetry.io/docs/languages/net/)
- [Datadog LLM Observability + OTel GenAI](https://www.datadoghq.com/blog/llm-otel-semantic-convention/)
