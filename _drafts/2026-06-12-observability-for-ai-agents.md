---
layout: post
title: "Observability for AI Agents — Beyond APM"
date: 2026-06-12
tags: [ai, llm, observability, opentelemetry, tracing, agents, production, langfuse]
categories: programming
series: production-ai-agents
series_index: 1
---

*Part 1 of a 4-part series on running AI agents in production. Also see: [Safety](/), [Governance](/), and the [series overview](/).*

---

Traditional APM (Application Performance Monitoring) was built for deterministic systems. You trace a request through services, measure latency at each hop, and alert on error rates. It works because the system does the same thing every time.

AI agents break this model. An agent might call an LLM 3 times or 30 times for the same user request. It might invoke tools you didn't know existed. It might succeed at the task but cost 10× more than expected because it took an inefficient reasoning path. None of this fits into a standard APM dashboard.

This post covers what observability means for AI agents — the signals you need, the patterns that work, and the tooling worth adopting.

## The Observability Taxonomy — A Quick Mental Model

Before diving into tools and patterns, worth being precise about what "observability" actually contains. The SRE world breaks it into several distinct capabilities, each answering a different question:

| Capability | Answers | For AI agents, that means… |
|------------|---------|---------------------------|
| **Metrics** | What changed? | Token counts, latency distributions, error rates, cost per interaction |
| **Logs** | What happened? | Full prompt/completion pairs, tool inputs/outputs, guardrail decisions |
| **Traces** | Where did it happen? | LLM call → tool invocation → re-planning → response, with causal links |
| **Monitoring** | Are known failures happening? | Alerts on guardrail spikes, tool failures, cost anomalies |
| **Observability** | Can we explore unknown failures? | Ask novel questions of production data without predefined dashboards |
| **Traceability** | What path did this request take? | Correlation IDs tying a user message to every downstream LLM and tool call |
| **Diagnosability** | How fast can we find root cause? | Emergent property — the faster you can trace from symptom to source, the better your observability design |

> **What about auditability?** Auditability — reconstructing *who* did *what* and *when* — sits in the **governance** layer (Part 3 of this series). It consumes traceability data from observability but adds identity, policy context, immutability, and retention. This post covers the technical foundation; governance covers what gets built on top.

The rest of this post walks through each of these capabilities in the context of AI agents, starting with the standard that makes them interoperable.

## The OTel GenAI Standard — One Convention to Rule Them All

Before diving into signals and tools, a critical development: the industry is converging on **OpenTelemetry GenAI Semantic Conventions** (v1.37+) as the standard vocabulary for LLM and agent observability. This is the work of OTel's [GenAI Special Interest Group (SIG)](https://github.com/open-telemetry/community/blob/main/projects/gen-ai.md), which defines standardized attribute names for traces, metrics, and events across all GenAI systems.

The key namespaces:

| Convention | What it covers | Status |
|-----------|---------------|--------|
| **Model spans** (`gen_ai.*`) | LLM calls — model name, token counts, temperature, provider | Development (widely adopted) |
| **Agent spans** (`gen_ai.agent.*`) | Agent workflows — planning steps, tool calls, task execution | Draft, based on Google's AI Agent whitepaper |
| **Framework conventions** | Vendor-specific extensions for LangGraph, CrewAI, AutoGen, etc. | In progress |

Why this matters: if you instrument with OTel GenAI conventions, your telemetry is portable across **Datadog, Langfuse, Phoenix, Agenta, Dynatrace, Grafana** — any backend that speaks OTLP. No vendor lock-in. 

Major providers have already adopted this:
- **Datadog LLM Observability** natively ingests OTel GenAI spans (v1.37+) as of December 2025
- **AWS Bedrock AgentCore** emits OTel-compatible traces via AWS Distro for OpenTelemetry
- **Microsoft Foundry** uses a `microsoft-opentelemetry` distro with GenAI conventions and has co-developed multi-agent semantic conventions with Cisco Outshift
- **Traceloop** (maintainers of OpenLLMetry) is [donating their instrumentation](https://github.com/open-telemetry/community/issues/2571) to the OTel project

OpenInference (Arize's OTel extension) adds LLM-specific attributes on top of OTel — if you send OpenInference spans to a non-Arize backend, those extra attributes become custom attributes. The industry direction is toward the OTel-native `gen_ai.*` conventions as the base, with vendor extensions layered on top.

## The Three Signals, Reinterpreted

Classic observability rests on three pillars: **logs**, **metrics**, and **traces**. They still apply to AI agents, but the *content* of each signal changes fundamentally.

### Traces

In a traditional microservice, a trace is a tree of service-to-service RPC calls. For an AI agent, the trace tree gains new node types:

```
User Request (span)
├── Guardrail: input validation (span)
├── LLM Call #1: planning (span)
│   ├── gen_ai.usage.input_tokens: 1240
│   ├── gen_ai.usage.output_tokens: 87
│   ├── gen_ai.request.model: "gpt-4o-mini"
│   └── latency_ms: 1432
├── Tool Call: search_docs("refund policy") (span)
│   ├── input: {query: "refund policy", top_k: 5}
│   ├── output: {results: 3, total_ms: 89}
│   └── latency_ms: 91
├── LLM Call #2: final response (span)
│   ├── gen_ai.usage.input_tokens: 2890
│   ├── gen_ai.usage.output_tokens: 412
│   ├── gen_ai.request.model: "gpt-4o"
│   └── latency_ms: 3210
├── Guardrail: output validation (span)
└── Response to user
```

The key difference: **spans are dynamic**. You don't know the trace shape ahead of time — the agent decides at runtime which tools to call and how many LLM rounds to execute. Your tracing infrastructure must handle variable-depth, variable-width traces.

**What to capture on every LLM span** (OTel GenAI conventions):
- `gen_ai.request.model` — model identifier (e.g., `gpt-4o-2024-08-06`)
- `gen_ai.usage.input_tokens` / `gen_ai.usage.output_tokens`
- `gen_ai.request.temperature` / `gen_ai.request.top_p`
- `gen_ai.provider.name` — which cloud/provider served the model
- `gen_ai.operation.name` — operation type: `chat`, `text_completion`, `embeddings`, `tool_call`
- `gen_ai.request.prompt_template_hash` — hash of the template version (not the full prompt)

For agent-level operations:
- `gen_ai.agent.id` — which agent instance
- `gen_ai.agent.tool` — which tool was invoked
- `gen_ai.agent.goal` — the agent's objective for this task

**OpenInference** (Arize's OTel extension) adds LLM-aware span attributes on top of OTel's base `gen_ai.*` conventions — things like `llm.span_kind` (values: `LLM`, `TOOL`, `CHAIN`, `AGENT`, `RETRIEVER`). The OTel community is moving toward incorporating these concepts into the official semantic conventions, with agent spans now an active area of standardization.

### Metrics

Standard RED metrics (Rate, Errors, Duration) still matter, but you need a new layer on top:

| Metric | What it measures | Why it matters |
|--------|-----------------|----------------|
| **Tokens per request** | Prompt + completion tokens, per request and per agent turn | Cost attribution, anomaly detection (did a prompt suddenly blow up?) |
| **LLM calls per user request** | Distribution of how many LLM rounds an agent takes | Spot agents going into infinite planning loops |
| **Tool success rate** | Per-tool error rate, latency distribution | Which tools are flaky? Which are slow? |
| **Cache hit rate** | For prompt caching, embedding cache, exact-match response cache | ROI of caching infrastructure |
| **Time to first token (TTFT)** | Latency from request → first response token | Perceived responsiveness for streaming agents |
| **Rate limit hits** | Count of 429 responses, queue depth | Capacity planning — when do you need to scale or add fallback models? |
| **Guardrail trigger rate** | How often input/output guardrails fire | Is your content safety working? Is prompt injection increasing? |

A critical metric that's easy to miss: **cost per successful interaction**. An agent that succeeds but costs $0.80 per request is fine for an internal tool but bankrupting for a consumer chatbot. Track cost broken down by model, by tool, and by user tier.

### Logs

Logs for AI agents need to be *structured* and *queryable* — not just free-text print statements. Every log entry should carry:

- `trace_id` — ties back to the distributed trace
- `agent_turn` — which conversation turn this belongs to
- `span_kind` — `LLM`, `TOOL`, `GUARDRAIL`, `EVAL`
- `decision_context` — what the agent was trying to do (planning, retrieval, response generation)

The log should capture the *full prompt and completion* for offline analysis — but be smart about it. Dumping 10K tokens of chat history into every log line will blow up your logging budget. Common approaches:
- **Sampling**: Log full prompts for 5% of requests, summary for the rest
- **Separate store**: Prompts go to blob storage (S3, Azure Blob), only metadata and a blob pointer go to your log aggregator
- **Evaluation pipeline**: Stream sampled traces to an eval pipeline that scores them and stores only the scores

## The Correlation Problem

The hardest operational problem in agent observability is **end-to-end correlation**. A user sends one message. The agent:
1. Calls a planning LLM → spawns 3 tool calls in parallel
2. Each tool call might hit an internal service, a vector DB, and an external API
3. Results are aggregated and fed to a final LLM for response generation
4. The response goes through output guardrails before reaching the user

All of this needs to tie back to a single user interaction. The solution is a **correlation ID** (`trace_id`) generated at the entry point (API gateway or agent framework) and propagated through every downstream call via:
- HTTP headers (`X-Trace-Id` or W3C `traceparent`)
- Message queue metadata
- LLM provider metadata (some providers let you pass a `user` or `metadata` field)

If your agent framework doesn't propagate trace context automatically, wrap every LLM and tool call in a thin instrumentation layer that injects `trace_id` into the call and extracts it on the other side.

## Evaluation as Observability

Offline evaluation is not a separate activity — it's a feedback loop that closes the observability cycle. Production traces are gold for evaluation:

1. **Sample production traces** (5-10% of traffic) into an eval pipeline
2. **Run LLM-as-judge evaluators** on sampled responses: relevance, coherence, groundedness, safety
3. **Compare against reference answers** when available (from support ticket resolutions, human agent transcripts)
4. **Surface regressions** — did a model or prompt change cause scores to drop?
5. **Feed back into alerts** — if groundedness score drops below threshold for >10 minutes, page on-call

This pattern turns observability from passive ("I can see what happened") to active ("I know when quality degrades"). Tools like Langfuse and Braintrust build this loop natively.

## Architecture Patterns

There are three ways to instrument an AI agent:

### 1. In-Process SDK

Your agent code calls `tracer.start_span(...)` directly. Full control, but couples instrumentation to agent code.

```
Agent code ──► OpenTelemetry SDK ──► OTLP exporter ──► Collector
```

Best for: custom agent frameworks, small teams, maximum flexibility.

### 2. Proxy / Sidecar

A local proxy (e.g., LiteLLM proxy, Langfuse proxy) sits between your agent and the LLM provider. It captures all LLM calls transparently — no code changes needed.

```
Agent code ──► localhost:4000 (LiteLLM) ──► OpenAI API
                    │
                    └──► OpenTelemetry / Langfuse export
```

Best for: brownfield systems, third-party agent frameworks you can't modify, quick instrumentation without touching agent code.

**Caveat**: A proxy sees LLM calls but not tool calls or internal agent logic. You miss the full trace shape unless you combine proxy traces with in-process spans.

### 3. Framework-Native Callbacks

Most agent frameworks provide hooks — LangChain has callbacks, Semantic Kernel has filters, AutoGen has middleware. You register observers that fire on each LLM call, tool invocation, and agent turn.

```
Agent code ──► Framework Callback ──► Your instrumentation ──► OTLP
```

Best for: teams already using an agent framework, wanting framework-aware instrumentation (e.g., LangChain callback captures chain type, AutoGen middleware captures agent-to-agent handoff).

### Recommendation

Combine all three: use a proxy (like LiteLLM or an AI Gateway) for *every* LLM call (uniform capture across models and providers), framework callbacks for *agent-specific* events (tool calls, reasoning steps, guardrail hits), and the OTel GenAI conventions as the data format. Export everything to the same trace collector with a shared `trace_id`, and send to whichever backend fits your needs — Langfuse for an integrated platform, Datadog for unified infrastructure + AI observability, or Grafana for full DIY.

## The Tooling Landscape at a Glance

The LLM observability ecosystem has matured into a few clear categories:

| Category | Tools | Best for |
|----------|-------|----------|
| **Open-source platforms** | Langfuse (29k ★, MIT), MLflow (Apache 2.0), Agenta (MIT), Comet Opik (Apache 2.0) | Full observability + eval + prompt management, self-hosted |
| **Specialized tools** | Phoenix (RAG/drift), Helicone (proxy-based), AgentOps (agent sessions), TruLens (eval-first) | Specific niches — retrieval quality, quick setup, multi-agent session tracking |
| **Full-stack + LLM** | SigNoz (OTel-native, open-source), Datadog, Grafana | Unified APM + LLM observability in one tool |
| **Cloud-native platforms** | **AWS**: Bedrock AgentCore + CloudWatch GenAI Observability (GA Oct 2025). **Azure**: Microsoft Foundry Observability (built-in tracing + eval) | Observability integrated into the AI platform itself — no separate tool. Also unifies AI + traditional infrastructure monitoring in one console |
| **Gateways + obs** | Portkey, LiteLLM proxy | Multi-provider routing with unified logging |
| **Instrumentation libs** | OpenLIT, OpenLLMetry | Zero/low-code OTel instrumentation, send to any backend |
| **SaaS platforms** | LangSmith, Braintrust, W&B Weave, Galileo | Managed, opinionated, often framework-specific |

## Tooling Deep Dive

### Langfuse (open-source, MIT licensed)

The most widely adopted open-source option — **29k GitHub stars**, 10+ billion observations/month, used by 19 of Fortune 50. Acquired by ClickHouse in early 2026. Key facts:
- Full trace viewer with LLM-specific span rendering (token counts, costs, prompt/completion side-by-side)
- Built-in evaluation pipeline — attach scores to traces, run LLM-as-judge
- **Prompt management** — version, deploy, and roll back prompts independently from code
- **80+ integrations**: every major framework (LangChain, CrewAI, Pydantic AI, Vercel AI SDK, Google ADK, Microsoft Agent Framework), every major model provider, plus gateways (LiteLLM proxy)
- **Language support**: Python, JS/TS native SDKs, plus Go, Java, **.NET**, Ruby, PHP, Swift via OTel
- **Self-hosting**: Docker Compose, Kubernetes (Helm), AWS/GCP/Azure (Terraform)
- **Enterprise**: SOC 2 Type II, ISO 27001, GDPR, HIPAA eligible, EU & US data regions
- **Agent-friendly**: MCP servers, CLI, coding agent skills for Claude Code, Cursor, Codex
- Free tier: 50k observations/month

**When to choose**: You want an all-in-one platform (observability + evals + prompt management), you value open-source and data portability, and you're running at scale.

### Agenta (open-source, MIT licensed)

A fast-rising OTel-native LLMOps platform combining observability, prompt management, and evaluation:
- **OTel-native**: uses standard `gen_ai.*` semantic conventions, vendor-neutral
- **End-to-end workflow**: link prompt versions to traces, run online + offline evals, compare experiments
- **Cross-functional UI**: designed for engineers, PMs, and domain experts — not just developers
- SOC 2 Type II compliant, self-hostable
- Free tier: 10k traces/month

**When to choose**: You want tight integration between observability and prompt management, and need a UI that non-engineers can work with.

### Phoenix by Arize (open-source, Apache 2.0)

Jupyter-native observability with a focus on embedding analysis and retrieval quality:
- Trace + span viewer with OpenInference compatibility
- Embedding drift detection — are your vector embeddings changing over time?
- Retrieval evaluation — precision/recall for RAG pipelines, built-in NDCG and MRR metrics
- Notebook-first workflow (good for data scientists)

**When to choose**: Your agent is heavily RAG-based and you care about retrieval quality, or your team is notebook-oriented.

### OpenLIT (open-source OTel auto-instrumentation)

An OTel-native auto-instrumentation library that captures LLM and VectorDB calls without code changes:
- Drop-in instrumentation for OpenAI, Anthropic, Cohere, HuggingFace, Ollama, and more
- Auto-captures token counts, latency, costs, and model parameters
- Emits standard OTel data — send to any OTLP-compatible backend
- Featured in the [official OTel blog](https://opentelemetry.io/blog/2024/llm-observability/) as a reference implementation

**When to choose**: You want zero-code instrumentation and already have an OTel collector + backend (Grafana, Jaeger, Datadog).

### MLflow (open-source, Apache 2.0)

The most established ML platform now extends into GenAI observability. MLflow's LLM tracing captures the full agent lifecycle — prompt versioning, trace replay (reproduce exact failure sequences), and LLM-as-a-Judge evaluation — in a single Apache 2.0 platform:
- **End-to-end GenAI lifecycle**: tracing, evaluation, prompt registry, AI Gateway — not bolted-on features
- **Trace replay**: reproduce non-deterministic agent failures step-by-step (a feature most tools lack)
- **Prompt versioning + A/B testing**: track prompt changes alongside traces, compare variants under production traffic
- **Self-hosted or Databricks-managed**: fits both DIY and managed workflows
- Natively integrates with LangChain, LlamaIndex, OpenAI, Anthropic, Hugging Face

**When to choose**: You already use MLflow for ML experiments, or you need a single platform spanning classical ML and GenAI observability without maintaining two separate systems.

### Helicone (SaaS, proxy-based)

The canonical proxy-based observability tool. Helicone sits in front of your LLM provider — change your API base URL and add an auth header, and it starts logging every request:
- **One-minute setup**: no SDK, no code changes beyond the endpoint URL
- **100+ models** supported across providers, no provider lock-in
- Built-in **caching, rate limiting, and automatic failover**
- Cost tracking by model, user, and feature
- **Not designed for deep agent reasoning analysis** — it sees LLM calls but not tool calls or internal agent logic

**When to choose**: You need basic LLM observability (cost, latency, errors) deployed in under an hour. Pairs well with an agent-level tool for deeper tracing.

### SigNoz (open-source, OTel-native full-stack)

SigNoz is an OTel-native observability platform that unifies LLM monitoring with full application observability. Unlike LLM-only tools, it correlates LLM traces with infrastructure metrics, Kubernetes pod data, database queries, and microservice traces in a single view:
- **Correlated observability**: jump from an LLM trace to the related system logs, exceptions, and infra metrics in one click
- **End-to-end waterfall views** of multi-agent workflows: model calls, tool invocations, reasoning steps, failed loops
- **Custom dashboards + alerts** on any telemetry (token usage by model/user/feature, cost, latency, error rates)
- **MCP server** exposes telemetry data to AI assistants for automated troubleshooting
- **Self-hosted** (community edition) or **SigNoz Cloud** (30-day free trial, usage-based pricing)

**When to choose**: You want LLM observability in the same tool as your infrastructure and application monitoring — no separate AI observability silo. Teams already bought into OpenTelemetry will find SigNoz a natural fit.

### Microsoft Foundry (Azure-native, built-in)

If you're on Azure, Foundry is not a tool you add — it's the observability layer baked into the platform. Microsoft Foundry (formerly Azure AI Studio) provides end-to-end GenAI observability through its **Observability → Traces** portal, powered by Azure Monitor Application Insights under the hood:
- **Native framework tracing**: zero-code auto-instrumentation for Microsoft Agent Framework and Semantic Kernel. One-line setup for LangChain, LangGraph, and OpenAI Agents SDK via the [`microsoft-opentelemetry`](https://pypi.org/project/microsoft-opentelemetry/) distro
- **Multi-agent semantic conventions** (co-developed with Cisco Outshift): `execute_task`, `agent_to_agent_interaction`, `agent.state.management`, `agent_planning`, `agent orchestration` spans — standardized for multi-agent workflows
- **OpenInference-compatible**: supports `openinference-*` instrumentation packages, so you can trace any framework that has an OpenInference integration
- **Built-in evaluation**: pre-built evaluators covering general quality, RAG quality, safety/security, and agent quality — run evaluations directly on traces
- **Content Safety APIs**: enterprise-grade harm detection with severity scoring (0-7) and detailed flag reasoning
- **Security controls**: content recording can be disabled in production via `OTEL_INSTRUMENTATION_GENAI_CAPTURE_MESSAGE_CONTENT`; traces stored in your own Application Insights instance with your retention policies
- **Dynatrace and Arize integrations**: Dynatrace offers a Foundry extension for end-to-end tracing + audit. Arize AX integrates for evaluation-driven experimentation workflows on top of Foundry traces

**When to choose**: You're deploying AI agents on Azure and want observability that's integrated with your cloud infrastructure, IAM, and compliance posture — not a separate third-party tool. The trade-off is Azure lock-in; the upside is zero-infrastructure observability that Just Works with your Azure resources.

### AWS: Bedrock AgentCore + CloudWatch GenAI Observability

AWS's observability story is two-tiered: **AgentCore** (the agent platform) emits OTel telemetry via **AWS Distro for OpenTelemetry (ADOT)**, and **CloudWatch GenAI Observability** (GA since October 2025) provides the dashboards. It's all integrated into the AWS console you already use:
- **Auto-instrumentation**: ADOT auto-instruments Strands, LangChain, LangGraph, and CrewAI agents. For other frameworks, plug in OpenInference, OpenLLMetry, or OpenLIT
- **Two pre-built dashboards**: **Model Invocations** (token usage, latency P90/P99, error rates, cost by user/role) and **AgentCore Agents** (performance across Agents, Memory, Built-in Tools, Gateways, Identity components)
- **AgentCore Evaluations** (Dec 2025): automated quality assessment of AI agent outputs, integrated directly into CloudWatch
- **End-to-end prompt tracing**: traces flow through models → knowledge bases → guardrails → tools, with X-Ray and W3C `traceparent` support
- **Session-aware**: propagate `session.id` via OTEL baggage for multi-turn agent conversations
- **Third-party routing**: set `DISABLE_ADOT_OBSERVABILITY=true` to route traces to Langfuse (official AWS blog post confirms this integration), Datadog, or any OTel backend instead of CloudWatch
- **Unified infrastructure + AI monitoring**: same CloudWatch you use for EC2, Lambda, and RDS now covers your AI agents — correlate agent latency with database query performance in one view

**When to choose**: You're running production AI agents on AWS and want observability that's already wired into your cloud infrastructure, IAM, and billing — no extra tool to deploy, no separate vendor to manage. The trade-off: you're tied to CloudWatch as your visualization layer (though traces can be exported to third-party tools).

### DIY with OpenTelemetry

Send OTLP traces to your existing observability stack and build custom dashboards:

```
Agent → OTel SDK + gen_ai conventions → OTel Collector → Grafana Tempo/Mimir/Loki
                                                       → Datadog (native OTel GenAI)
                                                       → Honeycomb / Jaeger / etc.
```

Use the OTel GenAI Semantic Conventions (v1.37+) as your data vocabulary, plus a lightweight instrumentation library like OpenLIT or OpenLLMetry to auto-capture LLM and vector DB calls. You get:
- Unified observability (AI and non-AI services in the same tool)
- No vendor lock-in — switch backends anytime
- Leverage existing infrastructure and expertise
- Governance policies (redaction, sampling) enforced at the OTel Collector level — before data leaves your network

The trade-off: you build your own dashboards, alert rules, and eval integration. Langfuse and Datadog give you much of this out of the box; DIY means more upfront work but maximum flexibility.

## What to Alert On

Not everything that can be measured should trigger a page. Focus alerts on **actionable degradation**:

| Alert | Threshold (example) | Action |
|-------|---------------------|--------|
| Guardrail trigger rate spike | >10% of requests trigger output guardrail (vs baseline 2%) | Roll back recent prompt/model change, investigate |
| Tool failure rate | >5% error rate on any tool for >5 min | Check downstream service health, circuit-break |
| Token explosion | p99 token count exceeds 3× rolling average | Agent might be in a reasoning loop — investigate traces |
| Eval score degradation | Groundedness score drops >0.2 for >15 min | Check if model provider changed something, roll back |
| Cost anomaly | Cost per 1K requests exceeds 2× daily baseline | Audit for prompt bloat, model routing issues |

Resist the urge to alert on raw latency. Agent requests are inherently variable — a 30-second response might be normal if the agent ran 8 tool calls. Alert on *unexpected* latency, defined as deviation from the agent's own baseline.

## Summary

Observability for AI agents is APM plus:
- **OTel GenAI conventions** (`gen_ai.*`) — the industry-standard vocabulary for LLM and agent spans, now at v1.37+ and adopted by Datadog, AWS, Langfuse, and others
- **Agent-aware spans** — not just LLM calls but the full decision graph (planning → tool calls → re-planning → response)
- **Dynamic trace shapes** that the agent determines at runtime
- **Correlation IDs** propagated through LLM providers and tool calls
- **Evaluation pipelines** that close the feedback loop from production to quality measurement — traces become datasets, datasets drive experiments
- **Cost observability** — token-level cost attribution with cache hit tracking and budget alerts

Start with a proxy (quick win for LLM call visibility), add framework callbacks for agent-level context, adopt the OTel GenAI conventions as your data format, and route to whichever backend fits your stack. The standardization happening now in the OTel GenAI SIG means the tooling choice is secondary to the data format — instrument once, switch backends anytime.

## References

- [OTel GenAI Semantic Conventions (v1.37+)](https://opentelemetry.io/docs/specs/semconv/gen-ai/) — official standard for GenAI spans, metrics, and agent operations
- [OTel Blog: AI Agent Observability — Evolving Standards (2025)](https://opentelemetry.io/blog/2025/ai-agent-observability/) — the GenAI SIG's roadmap for agent conventions
- [Langfuse: open-source LLM engineering platform](https://langfuse.com/) — 29k stars, MIT, ClickHouse-backed
- [MLflow: GenAI observability + trace replay](https://mlflow.org/genai/observability) — Apache 2.0, full lifecycle platform
- [Agenta: OTel-native LLMOps platform](https://agenta.ai/) — MIT, SOC 2, integrated prompt management + eval + observability
- [SigNoz: OTel-native full-stack observability + LLM](https://signoz.io/) — open-source, correlates LLM traces with infra
- [Helicone: proxy-based LLM observability](https://helicone.ai/) — one-line setup, 100+ models
- [OpenLIT: OTel-native auto-instrumentation for LLMs & VectorDBs](https://github.com/openlit/openlit)
- [OpenLLMetry: OTel-based LLM instrumentation by Traceloop](https://github.com/traceloop/openllmetry)
- [OpenInference: Arize's OTel extension for LLM observability](https://github.com/Arize-ai/openinference)
- [Phoenix by Arize: AI observability & evaluation](https://github.com/Arize-ai/phoenix)
- [Microsoft Foundry: Configure tracing for AI agent frameworks](https://learn.microsoft.com/en-us/azure/foundry/observability/how-to/trace-agent-framework) — official docs for LangChain, LangGraph, Semantic Kernel, OpenAI Agents SDK
- [Microsoft Foundry: Observability concepts for GenAI](https://learn.microsoft.com/en-us/azure/foundry/observability/concepts/trace-agent-concept) — multi-agent semantic conventions, security & privacy
- [AWS: Bedrock AgentCore Observability with Langfuse](https://aws.amazon.com/blogs/machine-learning/amazon-bedrock-agentcore-observability-with-langfuse/) — official blog confirming the Langfuse + AgentCore integration pattern
- [AWS: CloudWatch Generative AI observability docs](https://docs.aws.amazon.com/AmazonCloudWatch/latest/monitoring/GenAI-observability.html) — pre-built dashboards, prompt tracing, AgentCore agents
- [AWS: Bedrock AgentCore — Observability configuration](https://docs.aws.amazon.com/bedrock-agentcore/latest/devguide/observability-configure.html) — ADOT setup, third-party routing, session propagation
- [LiteLLM proxy for LLM call instrumentation](https://docs.litellm.ai/docs/proxy/quick_start)
- [OpenTelemetry for .NET](https://opentelemetry.io/docs/languages/net/)
- [Datadog LLM Observability + OTel GenAI](https://www.datadoghq.com/blog/llm-otel-semantic-convention/)
