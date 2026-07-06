---
layout: post
title: "Observability for AI Agents — Architecture, Tooling & Alerting"
date: 2026-06-12
tags: [ai, llm, observability, opentelemetry, tracing, agents, production, langfuse]
categories: programming
series: production-ai-agents
---

> Series overview: [Production AI Agents — From Notebook to Production]({% post_url 2026-06-15-production-ai-agents-series-overview %})

[Concepts, Signals & the OTel Standard]({% post_url 2026-06-12-observability-concepts-signals-ai-agents %}) covered the OTel GenAI standard and the four signals — traces, metrics, logs, events. This part covers the practical side: how to instrument agents, what tools exist, and what to alert on.

## Correlation

A single user message triggers planning → parallel tool calls → internal services → vector DB → external API → final LLM response → guardrails. All must tie back to one `trace_id`.

### Example: correlation ID propagation through an agent (web app)

```
Browser                Backend API           Agent Framework        LLM Provider
  │                       │                      │                     │
  │ GET /api/chat         │                      │                     │
  │ traceparent: 00-abc…  │                      │                     │
  │──────────────────────▶│                      │                     │
  │                       │ OTel extracts       │                     │
  │                       │ traceparent → sets  │                     │
  │                       │ Context.current()   │                     │
  │                       │ {trace_id: abc…}    │                     │
  │                       │                      │                     │
  │                       │ agent.run()          │                     │
  │                       │─────────────────────▶│                     │
  │                       │                      │ start_as_current_   │
  │                       │                      │ span() inherits     │
  │                       │                      │ trace_id from       │
  │                       │                      │ Context.current()   │
  │                       │                      │                     │
  │                       │                      │ POST /v1/chat       │
  │                       │                      │ traceparent: 00-abc…│
  │                       │                      │────────────────────▶│
```

The agent framework doesn't "get" the `trace_id` from an HTTP header — it **inherits** it implicitly. The backend's OTel HTTP instrumentation already extracted the `traceparent` header and stored `{trace_id: abc…}` in the current execution context. When the agent framework's instrumentation code calls `tracer.start_as_current_span()`, it auto-detects the active parent from `Context.current()` and sets the new span's `parent_span_id` — same `trace_id`, new `span_id`. The framework author wrote this once in their instrumentation library; your application code never touches trace propagation.

Propagation mechanisms:
- **HTTP headers**: W3C `traceparent` — extracted by server instrumentation, injected by client instrumentation
- **Message queue metadata**: inject trace context into message headers for async workflows
- **LLM provider metadata**: if the provider doesn't support `traceparent`, pass `trace_id` via `user` / `metadata` fields as a fallback

When propagation breaks — frameworks without OTel instrumentation produce no spans at all; async boundaries that don't preserve OTel context orphan child spans; proxies see LLM calls but not agent-level reasoning.

Other starting points (no browser header to extract):
- **Desktop / mobile app**: the client OTel SDK creates the root span locally — `trace_id` originates on the device, propagated via `traceparent` to the backend
- **CLI / background job**: your code creates the root span with `tracer.start_as_current_span()` — no incoming header exists, the trace starts here
- **Message queue consumer**: OTel instrumentation extracts trace context from the message envelope metadata — same mechanism as HTTP, different carrier

## Architecture

### Deployment patterns

Three ways to deploy instrumentation:

#### 1. In-Process SDK

```
Agent code → OpenTelemetry SDK → OTLP exporter → Collector
```

Full control over span creation and attribute population. Couples instrumentation to agent code. Best for custom agent frameworks and small teams that need maximum flexibility.

#### 2. Proxy / Sidecar

```
Agent code → localhost:4000 (LiteLLM) → OpenAI API
```

Sits between agent and LLM provider, captures all LLM calls transparently — no code changes needed. Best for brownfield systems and third-party frameworks you can't modify.

**Caveat**: proxy sees LLM calls but not tool calls or internal agent logic. Combine with in-process spans or framework callbacks for full trace shape.

#### 3. Framework-Native Callbacks

Agent framework hooks — LangChain callbacks, Semantic Kernel filters, AutoGen middleware. Register observers that fire on each LLM call, tool invocation, and agent turn. Best for framework-aware instrumentation (chain type capture, agent-to-agent handoff).

### Instrumentation source: baked-in vs OTel library

Separate dimension — who provides the instrumentation code, the framework maintainer or the OTel community:

| Approach | Who provides it | Examples | Trade-off |
|----------|----------------|---------|-----------|
| **Baked-in** | Framework maintainers | CrewAI (native OTel) | Simplest adoption; framework bloat, OTel dependency lag |
| **OTel package** | Community/vendor OTel library | OpenInference, `microsoft-opentelemetry`, Langtrace | Decoupled obs from framework; fragmentation risk |

### Where to start

If your framework ships baked-in OTel instrumentation, turn it on — you get agent-level spans with zero work. If you need visibility into LLM calls outside the framework (raw SDK calls, multi-provider setups), add a proxy. If you migrate between frameworks or go custom, standardize on OTel GenAI conventions as the data format so traces remain comparable across stacks.

## Tooling Landscape

| Category | Tools | Best for |
|----------|-------|----------|
| **Open-source platforms** | [Langfuse](https://github.com/langfuse/langfuse) (29.9k ★, MIT), [MLflow](https://github.com/mlflow/mlflow) (26.7k ★, Apache 2.0), [Opik](https://github.com/comet-ml/opik) (20k ★, Apache 2.0), [Agenta](https://github.com/Agenta-AI/agenta) (4.2k ★, MIT) | Full obs + eval + prompt mgmt, self-hosted |
| **Specialized tools** | [Phoenix](https://github.com/Arize-AI/phoenix) (10.3k ★, RAG/drift), [Helicone](https://github.com/Helicone/helicone) (5.9k ★, proxy), [AgentOps](https://github.com/AgentOps-AI/agentops) (5.7k ★, sessions), [TruLens](https://github.com/truera/trulens) (3.4k ★, eval-first) | Niche: retrieval quality, quick setup, session tracking |
| **Full-stack + LLM** | [SigNoz](https://github.com/SigNoz/signoz) (27.5k ★, OTel-native), [OpenObserve](https://github.com/openobserve/openobserve) (19.5k ★, Apache 2.0), [Datadog](https://www.datadoghq.com/product/llm-observability/), [Grafana](https://grafana.com/solutions/ai-ml/) | Unified APM + LLM obs |
| **Cloud-native platforms** | [AWS: Bedrock AgentCore + CloudWatch GenAI Obs](https://aws.amazon.com/about-aws/whats-new/2025/10/generative-ai-observability-amazon-cloudwatch/) (GA Oct '25). [Azure: AI Foundry Observability](https://learn.microsoft.com/en-us/azure/foundry/concepts/observability). [GCP: Gemini Enterprise Agent Platform (Agent Engine)](https://cloud.google.com/products/gemini-enterprise-agent-platform) | Obs integrated into the AI platform itself |
| **Gateways + obs** | [Portkey Gateway](https://github.com/Portkey-AI/gateway) (12.2k ★), [LiteLLM](https://github.com/BerriAI/litellm) (51.8k ★) | Multi-provider routing + unified logging |
| **Instrumentation libs** | [OpenLLMetry](https://github.com/traceloop/openllmetry) (7.2k ★), [OpenLIT](https://github.com/openlit/openlit) (2.6k ★), [Langtrace](https://github.com/Scale3-Labs/langtrace) (1.2k ★) | Zero-code OTel instrumentation |
| **SaaS platforms** | [LangSmith](https://www.langchain.com/langsmith), [Braintrust](https://www.braintrust.dev), [W&B Weave](https://weave-docs.wandb.ai), [Galileo](https://galileo.ai), [Pydantic Logfire](https://github.com/pydantic/logfire) (4.3k ★) | Managed, opinionated, framework-specific |

## Tool Deep Dives

### Langfuse
- 29.9k GitHub stars, MIT, 10B+ obs/month, 19 of Fortune 50
- Acquired by ClickHouse (early 2026)
- Full trace viewer with LLM-specific rendering, built-in eval pipeline, prompt management
- 100+ integrations across frameworks, model providers, and gateways
- SDKs: Python, JS/TS native; any OTel-instrumented language (Go, Java, .NET, Ruby, PHP, Swift) can send traces via Langfuse's OTLP endpoint
- Self-hosting: Docker Compose, K8s (Helm), AWS/GCP/Azure (Terraform)
- Enterprise: SOC 2 Type II, ISO 27001, GDPR, HIPAA eligible, EU & US data regions
- MCP servers, CLI, coding agent skills for Claude Code, Cursor, Codex
- Free tier: 50k observations/month

### Agenta
- OTel-native, MIT, SOC 2 Type II, self-hostable
- Links prompt versions to traces, online + offline evals, experiment comparison
- UI designed for engineers, PMs, and domain experts
- Free tier: 5k traces/month

### Phoenix by Arize
- ELv2 (Elastic License 2.0), Jupyter-native
- Embedding drift detection, retrieval evaluation (NDCG, MRR), trace+span viewer
- Strong for RAG-heavy agents; notebook-first workflow

### OpenLIT
- Zero-code OTel auto-instrumentation for OpenAI, Anthropic, Cohere, HuggingFace, Ollama, etc.
- Captures token counts, latency, costs, model parameters → standard OTel → any OTLP backend
- Featured in [official OTel blog](https://opentelemetry.io/blog/2024/llm-observability/)

### MLflow
- Apache 2.0, extends classical ML platform into GenAI
- LLM tracing: prompt versioning, trace replay (reproduce failure sequences), LLM-as-Judge eval
- Self-hosted or Databricks-managed
- Native LangChain, LlamaIndex, OpenAI, Anthropic integrations

### Helicone
- Proxy-based: change API base URL + auth header → instant logging
- 100+ models, no code changes
- Built-in caching, rate limiting, automatic failover
- Cost tracking by model/user/feature
- Not designed for deep agent reasoning — sees LLM calls only

### SigNoz
- OTel-native full-stack platform, open-source
- Correlates LLM traces with infra metrics, K8s pod data, DB queries, microservice traces in one view
- Custom dashboards + alerts on any telemetry
- MCP server for AI-assisted troubleshooting
- Self-hosted community edition or cloud (free trial)

### Microsoft Foundry (Azure)
- Built into Azure, powered by Azure Monitor Application Insights
- Zero-code auto-instrumentation for Microsoft Agent Framework, Semantic Kernel
- One-line setup for LangChain, LangGraph, OpenAI Agents SDK via `microsoft-opentelemetry` distro
- Multi-agent conventions (co-developed with Cisco Outshift): `execute_task`, `agent_to_agent_interaction`, `agent.state.management`, `agent_planning`, `agent orchestration`
- Pre-built evaluators: general quality, RAG quality, safety/security, agent quality
- Content Safety APIs with severity scoring (0-7)
- Security: content recording disabled via `OTEL_INSTRUMENTATION_GENAI_CAPTURE_MESSAGE_CONTENT`
- Dynatrace and Arize integrations

### AWS: Bedrock AgentCore + CloudWatch GenAI
- ADOT auto-instruments Strands, LangChain, LangGraph, CrewAI
- Two pre-built dashboards: Model Invocations (token usage, latency P90/P99, errors, cost) and AgentCore Agents (Agents, Memory, Built-in Tools, Gateways, Identity)
- AgentCore Evaluations (preview, Dec '25): auto quality assessment
- End-to-end prompt tracing: models → knowledge bases → guardrails → tools; X-Ray + W3C `traceparent`
- Session propagation via `session.id` in OTEL baggage
- Third-party routing: `DISABLE_ADOT_OBSERVABILITY=true` → Langfuse, Datadog, or any OTel backend

### DIY with OpenTelemetry

```
Agent → OTel SDK + gen_ai conventions → OTel Collector → Grafana Tempo/Mimir/Loki
                                                     → Datadog
                                                     → Honeycomb / Jaeger
```

Use OTel GenAI Semantic Conventions as the data vocabulary. Add lightweight instrumentation (OpenLIT, OpenLLMetry) for auto-capture. Governance policies (redaction, sampling) enforced at OTel Collector level before data leaves network.

Local development: [Aspire Dashboard](https://aspire.dev/dashboard/overview/) — free OTLP viewer, single Docker container:
```sh
docker run --rm -p 18888:18888 -p 4317:18889 -p 4318:18890 -d --name aspire-dashboard \
    -e ASPIRE_DASHBOARD_UNSECURED_ALLOW_ANONYMOUS=true \
    mcr.microsoft.com/dotnet/aspire-dashboard:latest
```

## Alerting

Not everything that can be measured should trigger a page. Focus on **actionable degradation**. These are examples — thresholds depend on your agent's own baseline:

| Alert | Threshold (example) | Action |
|-------|---------------------|--------|
| Guardrail trigger rate spike | >10% of requests trigger output guardrail (vs 2% baseline) | Roll back prompt/model change |
| Tool failure rate | >5% error rate on any tool for >5 min | Check downstream service health |
| Token explosion | p99 token count >3× rolling average | Agent may be in reasoning loop |
| Tool calls per request spike | avg tool calls per request >2× rolling average for >10 min | Agent may be in reasoning loop or flailing |
| Cost anomaly | Cost per 1K requests >2× daily baseline | Audit for prompt bloat, routing issues |

Alert on deviation from the agent's own baseline, not raw values — agent requests are inherently variable, and what's normal for one agent is broken for another.

## References

- [OTel GenAI Semantic Conventions](https://opentelemetry.io/docs/specs/semconv/gen-ai/)
- [Aspire Dashboard: GenAI telemetry visualizer](https://aspire.dev/dashboard/explore/#genai-telemetry-visualization)
- [Langfuse](https://langfuse.com/)
- [MLflow: GenAI observability](https://mlflow.org/genai/observability)
- [Agenta](https://agenta.ai/)
- [SigNoz](https://signoz.io/)
- [Helicone](https://helicone.ai/)
- [OpenLIT](https://github.com/openlit/openlit)
- [OpenLLMetry](https://github.com/traceloop/openllmetry)
- [Langtrace](https://github.com/Scale3-Labs/langtrace-python-sdk)
- [OpenInference](https://github.com/Arize-ai/openinference)
- [Phoenix by Arize](https://github.com/Arize-ai/phoenix)
- [Microsoft Foundry: Trace agent frameworks](https://learn.microsoft.com/en-us/azure/foundry/observability/how-to/trace-agent-framework)
- [Microsoft Foundry: Observability concepts](https://learn.microsoft.com/en-us/azure/foundry/observability/concepts/trace-agent-concept)
- [AWS: Bedrock AgentCore + Langfuse integration](https://aws.amazon.com/blogs/machine-learning/amazon-bedrock-agentcore-observability-with-langfuse/)
- [AWS: CloudWatch GenAI observability](https://docs.aws.amazon.com/AmazonCloudWatch/latest/monitoring/GenAI-observability.html)
- [AWS: Bedrock AgentCore observability config](https://docs.aws.amazon.com/bedrock-agentcore/latest/devguide/observability-configure.html)
- [LiteLLM proxy](https://docs.litellm.ai/docs/proxy/quick_start)
- [Datadog LLM Observability + OTel GenAI](https://www.datadoghq.com/blog/llm-otel-semantic-convention/)
