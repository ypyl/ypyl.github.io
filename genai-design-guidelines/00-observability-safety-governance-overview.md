# Observability, Safety & Governance

> **GenAI Design Guidelines** — Pillar: Production Readiness

## Purpose

This page defines the three interdependent layers that form the production foundation for AI agents: **Observability**, **Safety**, and **Governance**. These are not optional for systems that interact with real users, real data, or make real decisions. However, **they carry real costs and failure modes** — teams should apply them proportionally to risk and maturity, not as universal defaults from day one.

## The Three Pillars

These are **layers** that build on each other:

```
┌─────────────────────────────────────────────┐
│               GOVERNANCE                    │
│  Auditability · Policy · Compliance · Org   │
│  "Can we prove correctness over time?"      │
├─────────────────────────────────────────────┤
│                 SAFETY                      │
│  Guardrails · Tool Gates · Content Filters  │
│  "Can we stop harm in real time?"           │
├─────────────────────────────────────────────┤
│             OBSERVABILITY                   │
│  Traces · Metrics · Logs · Monitoring       │
│  Traceability · Diagnosability              │
│  "Do we know what's happening and why?"     │
└─────────────────────────────────────────────┘
```

### Layer 1: Observability (the foundation)

The technical capability to infer internal system state from external outputs. For AI agents, this means tracking every LLM call (model, tokens, latency, cost), every tool invocation, every reasoning step — so when an agent does something unexpected, you can trace exactly what happened and why.

Built from: **traces** (request lifecycle across services), **metrics** (aggregated numeric signals), **logs** (event-level records), and **events** (discrete agent actions). On top: **monitoring** (alerts for known failures), **traceability** (correlation IDs end-to-end), **diagnosability** (root cause in minutes, not days).

> **Detailed guidance**: [Observability](./01-observability.md)

### Layer 2: Safety (the operational guard)

Runtime control layer — automated guardrails that prevent the agent from causing harm *right now*. Spans three boundaries: **input** (prompt injection, jailbreaks), **tools** (dangerous tool calls, permission escalation), and **output** (toxic content, hallucination, PII leakage).

Safety depends on observability: guardrails emit events that traces capture. A safety incident that isn't observable is invisible.

> **Detailed guidance**: [Safety](./02-safety.md)

### Layer 3: Governance (the organizational layer)

Answers: **"Who did what, when, under which policy, and can we prove it to an auditor?"** It's about auditability, accountability, compliance, and policy enforcement — business rules encoded as version-controlled, testable policies.

Governance consumes observability data (audit records from production traces) and safety data (guardrail events as compliance evidence). But governance is fundamentally organizational — it involves humans (compliance officers, auditors, engineering leads) operating on longer timeframes.

> **Detailed guidance**: [Governance](./03-governance.md)

### A Note on Evaluation

Evaluation systematically measures the quality, safety, and correctness of AI outputs. It cuts across all three layers:
- Feeds **observability** — eval scores become production metrics
- Informs **safety** — safety evaluators detect harmful outputs before they reach users
- Provides evidence for **governance** — eval results become audit artifacts

Every major platform now treats evaluation alongside observability: Microsoft Foundry Agent Evaluators, Langfuse evaluation pipelines, AWS Bedrock AgentCore Evaluations. Evaluation makes the three pillars provable rather than anecdotal.

## Adoption Sequence

Building all three pillars simultaneously is rare. A typical sequence:

1. **Start with observability** (week 1-2). Without visibility, failures are invisible. Instrument LLM spans from day one — even a proxy capturing token counts and latencies is valuable.

2. **Layer on safety** (week 2-4). Output guardrails first (highest immediate value for lowest effort). Then tool gates: classify tools by risk level and insert human-in-the-loop checkpoints for destructive operations. Input guardrails follow as prompt injection patterns emerge.

3. **Formalize governance** (ongoing). The audit trail grows organically from observability data. Formalize it into append-only storage with defined retention. Add policy-as-code when business rules outgrow prompt-based enforcement. Prepare compliance artifacts before regulators ask.

> **⚠️ Governance retrofitted after a year of operation is far more expensive than instrumented from day one.** Structured audit records, model version tracking, and tool permission scoping should be present from the start — even at minimal fidelity.

## When NOT to Apply

These pillars carry real costs and failure modes. They should not be deployed as universal defaults. Ask:

### 1. Does the task need an AI agent at all?

If the input is structured and the outcome is binary (patching, provisioning, status checks), deterministic automation costs zero dollars and runs in milliseconds. Adding an LLM agent creates a cost chain: the agent itself, then observability to debug it, then safety to constrain it, then governance to audit it — all for a problem solved by 50 lines of code.

### 2. Is the system interacting with real users and real data, at scale?

Internal prototypes with controlled inputs do not need full production infrastructure. Start with minimal fidelity — trace every LLM call, track costs, capture errors. Defer eval suites, drift detection, red-teaming pipelines until the product earns them.

### 3. Is each pillar solving a measured problem, or deployed because "best practice" says so?

Infrastructure that doesn't address a specific, observed failure mode is overhead.

### Known Failure Modes

| Pillar | Failure Mode |
|--------|-------------|
| **Observability** | SaaS tooling + engineering time is the real cost; cloud ingestion is ~1% of inference. Over-engineered for early-stage products. |
| **Safety** | False positive cascade: five guardrails at 90% accuracy each → only 59% of legitimate requests pass. Drives shadow AI usage. No deterministic defense against prompt injection exists. |
| **Governance** | Policy theater — frameworks that are comprehensive on paper but ineffective in practice. 70% of orgs have AI policies; the question is whether they reduce risk. Premature regulation creates paperwork without improving outcomes. |

When these questions have clear answers — yes, the system warrants AI; yes, it touches real users; yes, specific failures have occurred — then observability, safety, and governance stop being overhead and start being the engineering foundation.

## Benefits

| Benefit | How |
|---------|-----|
| Find root cause in minutes, not days | Full tracing from user request to LLM call to tool invocation |
| Stop harmful outputs before they reach users | Guardrails at input, tool, and output boundaries |
| Pass any audit with structured, queryable evidence | Append-only audit trail, model lineage, policy-as-code |
| Ship faster | Observability enables safe, confident iteration |
| Build trust | With users, stakeholders, and regulators |

## Liability of Not Having It

| Risk | Consequence |
|------|------------|
| Invisible failures | Agents fail silently — no stack trace, no error log |
| Undetected harm | Prompt injection, toxic outputs, data leaks go unnoticed |
| Compliance fines | EU AI Act: up to €35M or 7% of global turnover (Article 99) |
| Audit failure | Cannot prove correctness when regulators ask |
| Retrofitting cost | Adding governance after 1 year is far more expensive |

## Key References

- [OWASP Top 10 for LLM Applications (2025)](https://genai.owasp.org/llm-top-10/)
- [OWASP Top 10 for Agentic Applications (2026)](https://genai.owasp.org/resource/owasp-top-10-for-agentic-applications-for-2026/)
- [EU AI Act — Regulatory Framework](https://digital-strategy.ec.europa.eu/en/policies/regulatory-framework-ai)
- [NIST AI Risk Management Framework 1.0](https://www.nist.gov/itl/ai-risk-management-framework)
- [OpenTelemetry GenAI Semantic Conventions](https://opentelemetry.io/docs/specs/semconv/gen-ai/)
