# Observability

> **Parent**: [Observability, Safety & Governance](./00-observability-safety-governance-overview.md)
> **Layer**: Foundation — Safety and Governance build on this

Observability for AI agents is traditional application performance monitoring (APM) plus new signal types, a new industry standard (OTel GenAI conventions), and tooling that understands LLM calls, tool invocations, and agent reasoning. Without it, an agent that fails silently, cost-explodes, or loops indefinitely is indistinguishable from one that works correctly.

---

## Capability Taxonomy

Observability decomposes into distinct capabilities:

| Capability | Answers | For AI agents |
|------------|---------|---------------|
| **Metrics** | What changed? | Token counts, latency distributions, error rates, cost per interaction |
| **Logs** | What happened? | Prompt/completion pairs, tool inputs/outputs, guardrail decisions |
| **Traces** | Where did it happen? | LLM call → tool invocation → re-planning → response, with causal links |
| **Monitoring** | Are known failures happening? | Alerts on guardrail spikes, tool failures, cost anomalies |
| **Observability** | Can we explore unknown failures? | Ask novel questions without predefined dashboards |
| **Traceability** | What path did this request take? | Correlation IDs tying a user message to every downstream call |
| **Diagnosability** | How fast can we find root cause? | The faster you trace symptom to source, the better the obs design |

> **Auditability** — reconstructing *who* did *what* and *when* — sits in the **governance** layer. It consumes traceability data but adds identity, policy context, immutability, and retention.

---

## The Standard: OpenTelemetry GenAI Conventions

The industry is converging on **OpenTelemetry GenAI Semantic Conventions (v1.37+)**. Instrument with these conventions, and telemetry is portable across Datadog, Langfuse, Phoenix, SigNoz, Grafana — any OTLP-speaking backend.

| Convention | What it covers | Status |
|-----------|---------------|--------|
| **Model spans** (`gen_ai.*`) | LLM calls — model name, token counts, temperature, provider | Widely adopted |
| **Agent application** (`gen_ai.agent.*`) | Agent workflows — planning, tool calls, task execution | Draft |
| **Agent framework** | Common convention across frameworks (LangGraph, CrewAI, AutoGen) | In progress |

### Adoption

Major providers already ship OTel GenAI:
- **Datadog LLM Observability** — native OTel GenAI spans since Dec 2025
- **AWS Bedrock AgentCore** — OTel-compatible traces via ADOT
- **Microsoft Foundry** — `microsoft-opentelemetry` distro with GenAI conventions; co-developed multi-agent conventions with Cisco Outshift
- **Traceloop** (OpenLLMetry) — donating instrumentation to OTel
- **VS Code Copilot, OpenAI Codex, Claude Code** — all emit OTel GenAI traces natively

### Design Guideline

**Instrument with OTel GenAI conventions as the data vocabulary.** This ensures your telemetry is portable across tools and cloud providers. Layer vendor-specific extensions on top of the OTel base, not instead of it.

---

## Signals

### 1. Traces

A trace for an AI agent is a tree where the agent decides the shape at runtime:

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

**Spans are dynamic** — tracing infrastructure must handle variable-depth, variable-width traces.

#### Span Types (OTel GenAI Spec)

| Span type | `gen_ai.operation.name` | When to use |
|-----------|------------------------|-------------|
| **Invoke agent** (local) | `invoke_agent` | In-process agent invocation |
| **Invoke agent** (remote) | `invoke_agent` | Remote agent services (OpenAI Assistants, Bedrock Agents) |
| **Invoke workflow** | `invoke_workflow` | Multi-agent orchestration |
| **Plan** | `plan` | Agent planning/task decomposition (only when reliably distinguishable from generic reasoning) |

#### Key Span Attributes

**LLM spans** (`chat`, `text_completion`, `embeddings`, `generate_content`, `retrieval`):

| Attribute | Purpose | Opt-In? |
|-----------|---------|---------|
| `gen_ai.request.model` | Model identifier | No |
| `gen_ai.usage.input_tokens` / `output_tokens` | Token counts | No |
| `gen_ai.usage.cache_read.input_tokens` | Cached-prompt cost attribution | No |
| `gen_ai.request.temperature` / `top_p` / `max_tokens` | Request parameters | No |
| `gen_ai.provider.name` | Cloud/provider | No |
| `gen_ai.response.finish_reasons` | Stop reasons | No |
| `gen_ai.system.instructions` | System prompt | **Yes** |
| `gen_ai.input.messages` / `output.messages` | Full prompt/completion | **Yes** |

**Agent spans** (`invoke_agent`, `invoke_workflow`, `plan`): `gen_ai.agent.name`, `gen_ai.agent.description`, `gen_ai.agent.version`, `gen_ai.conversation.id`, `gen_ai.workflow.name`, `gen_ai.output.type`.

#### Design Guideline

**Enable LLM span attributes by default. Make prompt/completion capture opt-in** — prompts can contain sensitive data. Control via instrumentation config (e.g., `captureContent=true`).

### 2. Metrics

OTel GenAI defines two official metrics:

| Metric | Type | Filterable by |
|--------|------|--------------|
| `gen_ai.client.operation.duration` | Histogram | `gen_ai.request.model` |
| `gen_ai.client.token.usage` | Histogram | `gen_ai.token.type` (`input` / `output`) |

**Additional operational signals** to track (via span-level queries):

| Metric | Why it matters |
|--------|---------------|
| **Tokens per request** | Cost attribution, anomaly detection |
| **LLM calls per user request** | Spot agents in infinite planning loops |
| **Tool success rate** (per tool) | Flaky/slow tools |
| **Cache hit rate** | ROI of caching infrastructure |
| **Time to first token (TTFT)** | Perceived responsiveness |
| **Rate limit hits** (429 count) | Capacity planning, fallback models |
| **Guardrail trigger rate** | Content safety, prompt injection trends |
| **Cost per successful interaction** | Business-level cost tracking |

#### Design Guideline

**Track the two official OTel metrics as a baseline, then add operational signals as your system matures.** Cost per successful interaction is the highest-level business signal — track it from day one.

### 3. Logs

Integrate OTel logging to produce structured log records. When a span is active, the OTel SDK automatically attaches `trace_id` and `span_id` to every log record — no manual correlation code.

**Controlling storage cost for prompt/completion content:**

| Strategy | Best for |
|----------|----------|
| **Sampling** | Log full prompts for 5% of requests; others: token counts + model name only |
| **Separate store** | Write full prompts to cheap blob storage (S3, Azure Blob). Store pointer + metadata in log aggregator |
| **Eval pipeline** | Stream sampled traces to evaluation pipeline, discard raw text, store only scores |

#### Design Guideline

**Use sampling by default. Enable full capture only when compliance or audit requirements demand it.** Separate blob storage is the most scalable pattern for 100% capture.

### 4. Events (The Fourth Signal)

Discrete, semantically meaningful agent actions — structured records bridging traces and logs:

| Event type | Why it matters |
|-----------|----------------|
| API call | Track tool usage and cost |
| LLM call | Quality analysis |
| Failed tool call | Alert, root cause |
| Human handoff | Autonomy rate, capability gaps |
| Guardrail trigger | Safety system effectiveness |

OTel GenAI Events spec defines structured input/output message schemas (JSON). Events are less granular than spans but more structured than free-text logs — useful for audit trails and compliance.

---

## Correlation

A single user message triggers planning → parallel tool calls → internal services → vector DB → external API → final LLM response → guardrails. All must tie back to one `trace_id`.

**How propagation works:**
- **HTTP**: W3C `traceparent` header — extracted by server instrumentation, injected by client instrumentation
- **Message queues**: Inject trace context into message headers for async workflows
- **LLM provider metadata**: If `traceparent` isn't supported, pass `trace_id` via `user`/`metadata` fields as fallback
- **Desktop/CLI**: Client OTel SDK creates the root span; no incoming header needed

The agent framework inherits the trace context implicitly — OTel HTTP instrumentation extracts `traceparent` and stores it in the current execution context. When the agent creates spans, they auto-inherit the parent context.

#### Design Guideline

**Ensure trace context propagation across every boundary: browser → API → agent framework → LLM provider.** Common breakage points: frameworks without OTel instrumentation (no spans), async boundaries that don't preserve OTel context (orphaned spans), proxies that see LLM calls but not agent reasoning.

---

## Architecture

### Deployment Patterns

#### 1. In-Process SDK

```
Agent code → OpenTelemetry SDK → OTLP exporter → Collector
```

Full control over span creation and attribute population. Best for custom agent frameworks and teams that need maximum flexibility.

#### 2. Proxy / Sidecar

```
Agent code → localhost:4000 (LiteLLM) → OpenAI API
```

Captures all LLM calls transparently — no code changes needed. Best for brownfield systems. **Caveat**: sees LLM calls but not tool calls or internal agent logic.

#### 3. Framework-Native Callbacks

Agent framework hooks — LangChain callbacks, Semantic Kernel filters, AutoGen middleware. Register observers that fire on each LLM call, tool invocation, and agent turn.

### Instrumentation Source

| Approach | Examples | Trade-off |
|----------|----------|-----------|
| **Baked-in** (framework maintainers) | CrewAI (native OTel) | Simplest adoption; OTel dependency lag |
| **OTel package** (community/vendor) | OpenInference, `microsoft-opentelemetry`, Langtrace | Decoupled from framework; fragmentation risk |

#### Design Guideline

**If your framework ships baked-in OTel instrumentation, turn it on — zero-work agent-level spans. If you need visibility outside the framework, add a proxy. Standardize on OTel GenAI conventions as the data format so traces remain comparable across stacks.**

---

## Tooling Landscape

| Category | Tools | Best for |
|----------|-------|----------|
| **Open-source platforms** | [Langfuse](https://github.com/langfuse/langfuse) (29.9k ★, MIT), [MLflow](https://github.com/mlflow/mlflow) (26.7k ★, Apache 2.0), [Opik](https://github.com/comet-ml/opik) (20k ★, Apache 2.0), [Agenta](https://github.com/Agenta-AI/agenta) (4.2k ★, MIT) | Full obs + eval + prompt mgmt, self-hosted |
| **Specialized** | [Phoenix](https://github.com/Arize-AI/phoenix) (RAG/drift), [Helicone](https://github.com/Helicone/helicone) (proxy), [AgentOps](https://github.com/AgentOps-AI/agentops) (sessions), [TruLens](https://github.com/truera/trulens) (eval-first) | Niche: retrieval quality, quick setup, session tracking |
| **Full-stack + LLM** | [SigNoz](https://github.com/SigNoz/signoz), [Datadog](https://www.datadoghq.com/product/llm-observability/), [Grafana](https://grafana.com/solutions/ai-ml/) | Unified APM + LLM obs |
| **Cloud-native** | AWS Bedrock AgentCore + CloudWatch, Azure AI Foundry, GCP Gemini Enterprise Agent Platform | Obs integrated into AI platform |
| **Gateways + obs** | [Portkey Gateway](https://github.com/Portkey-AI/gateway), [LiteLLM](https://github.com/BerriAI/litellm) | Multi-provider routing + unified logging |
| **Instrumentation libs** | [OpenLLMetry](https://github.com/traceloop/openllmetry), [OpenLIT](https://github.com/openlit/openlit), [Langtrace](https://github.com/Scale3-Labs/langtrace) | Zero-code OTel instrumentation |
| **SaaS platforms** | [LangSmith](https://www.langchain.com/langsmith), [Braintrust](https://www.braintrust.dev), [W&B Weave](https://weave-docs.wandb.ai), [Galileo](https://galileo.ai) | Managed, opinionated, framework-specific |

### Tool Deep Dives

**Langfuse** — 29.9k ★, MIT, acquired by ClickHouse (2026). Full trace viewer with LLM-specific rendering, built-in eval pipeline, prompt management. 100+ integrations. Self-hosting: Docker Compose, K8s, Terraform. Enterprise: SOC 2 Type II, ISO 27001, GDPR, HIPAA.

**Agenta** — OTel-native, MIT, SOC 2 Type II. Links prompt versions to traces, online + offline evals, experiment comparison. UI designed for engineers, PMs, and domain experts.

**Phoenix (Arize)** — Embedding drift detection, retrieval evaluation (NDCG, MRR), trace+span viewer. Strong for RAG-heavy agents.

**SigNoz** — OTel-native full-stack. Correlates LLM traces with infra metrics, K8s, DB queries, microservice traces in one view.

**Microsoft Foundry (Azure)** — Built into Azure. Zero-code auto-instrumentation for Microsoft Agent Framework, Semantic Kernel. One-line setup for LangChain, LangGraph, OpenAI Agents SDK. Multi-agent conventions (co-developed with Cisco Outshift). Pre-built evaluators. Content Safety APIs.

**AWS Bedrock AgentCore + CloudWatch** — ADOT auto-instruments Strands, LangChain, LangGraph, CrewAI. Pre-built dashboards: Model Invocations + AgentCore Agents. AgentCore Evaluations. End-to-end prompt tracing.

### DIY with OpenTelemetry

```
Agent → OTel SDK + gen_ai conventions → OTel Collector → Grafana Tempo/Mimir/Loki
                                                     → Datadog
                                                     → Honeycomb / Jaeger
```

For local development: [Aspire Dashboard](https://aspire.dev/dashboard/overview/) — free OTLP viewer, single Docker container.

#### Design Guideline

**Start with your cloud platform's built-in observability** (Foundry on Azure, CloudWatch on AWS). **Graduate to dedicated AI observability platform** (Langfuse, Agenta) when you need eval pipelines, prompt management, and experiment tracking. **Fall back to DIY OTel** when you need maximum control or multi-cloud portability.

---

## Alerting

Not everything that can be measured should trigger a page. Focus on **actionable degradation**. Thresholds depend on your agent's own baseline:

| Alert | Example threshold | Action |
|-------|-------------------|--------|
| Guardrail trigger rate spike | >10% of requests trigger output guardrail (vs 2% baseline) | Roll back prompt/model change |
| Tool failure rate | >5% error rate on any tool for >5 min | Check downstream service health |
| Token explosion | p99 token count >3× rolling average | Agent may be in reasoning loop |
| Tool calls per request spike | avg tool calls per request >2× rolling average for >10 min | Agent may be flailing |
| Cost anomaly | Cost per 1K requests >2× daily baseline | Audit for prompt bloat, routing issues |

#### Design Guideline

**Alert on deviation from the agent's own baseline, not raw values.** Agent requests are inherently variable — what's normal for one agent is broken for another. Start with the top three: tool failure rate, guardrail trigger rate, and cost anomaly.

---

## Key References

- [OTel GenAI Semantic Conventions (v1.37+)](https://opentelemetry.io/docs/specs/semconv/gen-ai/)
- [OTel GenAI Agent Spans](https://github.com/open-telemetry/semantic-conventions-genai/blob/main/docs/gen-ai/gen-ai-agent-spans.md)
- [OTel GenAI Events](https://github.com/open-telemetry/semantic-conventions-genai/blob/main/docs/gen-ai/gen-ai-events.md)
- [OTel Blog: AI Agent Observability (2025)](https://opentelemetry.io/blog/2025/ai-agent-observability/)
- [OTel Blog: GenAI Observability with OpenTelemetry (2026)](https://opentelemetry.io/blog/2026/genai-observability/)
- [IBM: Why observability is essential for AI agents](https://www.ibm.com/think/insights/ai-agent-observability)
- [Azure Monitor pricing](https://azure.microsoft.com/en-us/pricing/details/monitor/)
- [OpenInference](https://github.com/Arize-ai/openinference)
- [models.dev — model pricing API](https://models.dev/api.json)
