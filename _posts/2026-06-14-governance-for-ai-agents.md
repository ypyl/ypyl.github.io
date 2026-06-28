---
layout: post
title: "Governance for AI Agents — Policy, Audit, and Compliance"
date: 2026-06-14
tags: [ai, llm, governance, compliance, audit, policy, agents, production, eu-ai-act]
categories: programming
series: production-ai-agents
series_index: 3
---

> Series overview: [Production AI Agents — From Notebook to Production]({% post_url 2026-06-15-production-ai-agents-series-overview %})

Safety stops the agent from doing something harmful *right now*. Governance ensures you can prove — to regulators, auditors, and your own leadership — that the system operates within defined boundaries *over time*.

Safety is operational. Governance is organizational.

> **Reference implementation**: Microsoft's open-source [Agent Governance Toolkit](https://github.com/microsoft/agent-governance-toolkit) (AGT, Python/TypeScript/.NET/Go/Rust) is the most comprehensive implementation of the governance stack described in this post. It provides a deterministic policy engine, cryptographic agent identity (SPIFFE/DID/mTLS), execution sandboxing with privilege rings, Merkle-chained audit logs, and formal RFC 2119 specifications backed by 992 conformance tests. Where relevant, I'll reference how AGT approaches each layer — not as endorsement, but as a concrete example of these patterns in production code.
>
> **Content safety (input/output filtering) is covered in [Part 2: Safety for AI Agents]({% post_url 2026-06-13-safety-for-ai-agents %})** — guardrails, prompt injection defenses, jailbreak protection, and content moderation. This article focuses on the governance layer: policy enforcement, audit, identity, compliance, and organizational controls that sit *above* real-time safety. For the full guardrails tooling landscape (NeMo Guardrails, Guardrails AI, LLM Guard, Lakera Guard, Garak), see that article.

## Map

```
Governance for AI Agents
│
├── SAFETY VS GOVERNANCE — distinction, comparison table
│
├── THE GOVERNANCE STACK — five-layer diagram + layer walkthroughs
│   ├── Access Control — who can do what
│   ├── Audit Trail — record every decision
│   ├── Model Lineage — track what runs where
│   ├── Policy Enforcement — enforce business rules
│   └── Compliance & Regulatory Landscape — prove to auditors
│
├── EXECUTION SANDBOXING & SRE
│   ├── Privilege Rings — Ring 0 to Ring 3 isolation per agent risk tier
│   └── SRE — kill switch, SLOs, chaos testing, circuit breakers
│
├── ORGANIZATIONAL GOVERNANCE
│   ├── Roles & Responsibilities — cross-functional ownership
│   └── Governance as a Gate — integrated into development lifecycle
│
├── TOOLS LANDSCAPE
│   ├── Policy Engines — AGT, OPA, Cedar
│   ├── AI Gateways — LiteLLM, Bifrost, Portkey, Azure APIM, AWS Bedrock, GCP Agent Gateway
│   ├── Compliance & Risk — Credo AI, IBM Watsonx, OneTrust
│   ├── Agent Identity — Frontegg, SPIFFE
│   └── How the Pieces Fit Together — stack assembly diagram
│
└── REFERENCES
```

## Safety vs. Governance — The Distinction

| | Safety | Governance |
|---|--------|------------|
| **Timeframe** | Real-time (milliseconds to seconds) | Historical (days to years) |
| **Actor** | Automated guardrails | Humans (compliance officers, auditors, engineering leads) |
| **Question** | "Should the agent do this *right now*?" | "Can we prove the agent has *always* operated correctly?" |
| **Failure mode** | Blocked action, filtered response | Audit finding, regulatory penalty, reputational damage |
| **Tool** | Guardrails, content classifiers, tool gates | Audit logs, policy engines, model registries, compliance reports |

The two are complementary. Safety guardrails generate the events that governance consumes. An output content filter that fires is both a safety action (blocking harmful content) and a governance event (logged for compliance review).

## The Governance Stack

```
┌──────────────────────────────────────┐
│          COMPLIANCE REPORTS           │  ← Prove to auditors
├──────────────────────────────────────┤
│       POLICY-AS-CODE ENGINE           │  ← Enforce business rules
├──────────────────────────────────────┤
│    AI REGISTRY & MODEL LINEAGE        │  ← Track what runs where
├──────────────────────────────────────┤
│       IMMUTABLE AUDIT TRAIL           │  ← Record every decision
├──────────────────────────────────────┤
│     ACCESS CONTROL & PERMISSIONS      │  ← Who can do what
└──────────────────────────────────────┘
```

Each layer builds on the one below. You can't have an audit trail without knowing who accessed what. You can't have policy enforcement without an audit trail to validate against. You can't have compliance reports without a model registry to know what was deployed when.

### 1. Access Control

Agent access control is more nuanced than traditional RBAC because permissions apply at multiple levels:

| Level | Controls | Example |
|-------|----------|---------|
| **Agent access** | Who can invoke this agent | Only "Premium Support" tier can use the refund agent |
| **Tool access** | Which tools the agent is allowed to call for this user | Agent can `lookup_order` for everyone but `approve_refund` only for authenticated users |
| **Data access** | What data the agent can see | Agent running for user A can only see user A's orders |
| **Decision authority** | Which decisions auto-execute vs. require approval | Refunds <$50 auto-approve; >$50 queue for human review |

#### Agent Identity — "Which Agent Did This?"

User-level RBAC answers "who invoked the agent." But in a multi-agent system where multiple agents share an API key, you also need to answer "which *agent* took this action?" Without cryptographic agent identity, an incident response starts with "an agent did it" — which is not actionable.

Agent identity solutions assign each agent a verifiable credential:

| Approach | Mechanism | Strength |
|----------|-----------|----------|
| **API key per agent** | Provision a separate key for each agent instance | Simple; keys can leak |
| **SPIFFE/X.509** | Short-lived X.509 certificates issued to each agent workload | Industry standard for workload identity (Kubernetes, Istio) |
| **DID (Decentralized Identifier)** | Self-sovereign identity with Ed25519 keys; agent signs every inter-agent message | No central CA; verifiable without network calls |
| **mTLS** | Mutual TLS between agents; both sides present certificates | Transport-layer; pairs with SPIFFE |

AGT's AgentMesh implements SPIFFE + DID + mTLS: every agent gets an Ed25519 keypair, every inter-agent message is signed, and trust scores (0–1000) are assigned based on identity verification and behavioral history. A newly discovered agent starts at 500 (Standard); repeated policy violations drive it toward 0 (Untrusted — read-only or blocked).

For most teams, per-agent API keys are a sufficient starting point. Graduate to SPIFFE or DID when you have more than a handful of agents or when agents delegate tasks to each other.

#### Implementing Least-Privilege Tools

The agent doesn't receive the full tool catalog — it gets a subset based on the current user's permissions (e.g., authenticated users get lookup and policy-check tools; premium users additionally get refund approval; admins get override and full-visibility tools). This limits the damage from prompt injection: even if the attacker tricks the agent into calling `approve_refund`, the tool simply doesn't exist in that session's context.

#### Policy-as-Code for Tool Authorization

Beyond simple role checks, use a policy engine for fine-grained authorization — for example, a rule allowing premium agents to approve refunds only when the order amount is under $500 and within 30 days of creation.

Policy engines like [Cedar](https://www.cedarpolicy.com/) (used by AWS Verified Permissions) or [Open Policy Agent](https://www.openpolicyagent.org/) let you express business rules as version-controlled, testable code. The policy is evaluated at the tool gate before execution — it's the governance counterpart to the safety tool gate.

### 2. Audit Trail

An audit trail for AI agents must answer: **who did what, when, using which model, with what data, and what was the outcome?**

#### What to Record

Every agent interaction should produce a structured audit record capturing: event and trace IDs, agent identity and version, model configuration (name, temperature, prompt template hash), user context (ID, role, session), the decision made (action, amount, confidence, rationale), tool calls with input/output hashes, retrieved documents, guardrail results, and outcome status. Store full prompts and retrieved documents as hashes for integrity verification, with full payloads in separate blob storage — storing every prompt in your audit database is expensive and often unnecessary.

#### Storage Requirements

- **Append-only**: Records must be immutable. No updates, no deletes. Use write-once storage (S3 Object Lock, Azure Immutable Blob, append-only ledger DBs).
- **Tamper-evident**: Beyond append-only storage, cryptographically chain records so any tampering is detectable. AGT uses **Merkle audit chains** — each audit entry includes a hash of the previous entry, creating a verifiable chain. A regulator can validate the entire history without trusting your database.
- **Retention**: Define retention periods based on your regulatory requirements. GDPR doesn't specify a fixed period but requires proportionality. Financial services often require 7 years. Define a policy and stick to it.
- **Queryable**: Auditors need to answer questions like "show me all refunds approved above $500 in Q3" without writing custom code. Index on `agent_id`, `decision.action`, `decision.amount`, `timestamp`.

#### Architectural Options

| Approach | Pros | Cons |
|----------|------|------|
| **Relational DB with append-only tables** | Familiar, queryable, ACID | Scale limits, retention costs |
| **Event sourcing (Kafka + blob storage)** | Immutable by design, replayable | Operational complexity |
| **Cloud audit services** (AWS CloudTrail, Azure Monitor) | Managed, compliant out of the box | Cost at high volume, not AI-aware natively |
| **Dedicated AI audit platforms** (Langfuse, Braintrust) | AI-native schemas, built-in eval | Vendor lock-in, another tool to manage |

Most teams start with append-only PostgreSQL or event sourcing to blob storage, then graduate to dedicated platforms when volume or compliance requirements demand it.

### 3. Model Versioning & Lineage

When a regulator asks "which model made this decision?", you need to answer precisely — not just "GPT-4o" but "GPT-4o `gpt-4o-2024-08-06` with temperature 0.3 and prompt template v2.4.1."

#### What to Track

Every agent deployment should record:
- **Model**: Provider + model ID + version date (e.g., `openai/gpt-4o-2024-08-06`)
- **Prompt template**: Hash of the system prompt (for integrity verification) + version number
- **Configuration**: Temperature, top_p, max_tokens, stop sequences, tool definitions
- **Deployment metadata**: Who deployed, when, to which environment, with which approval

This data lives in a **model registry** — a catalog of every AI artifact in production.

#### Model Registry

A model registry for agents isn't just about model weights. It tracks each agent version with its associated model (provider + version date), prompt template (hash + version), tool set, evaluation scores, and deployment metadata (who deployed, when, to which environment).

Tools like MLflow Model Registry, Weights & Biases Model Registry, or a simple database table can serve this purpose. The key is that every audit record references a specific agent version, and the registry provides the full context for that version.

### 4. Policy Enforcement

Safety guardrails enforce *technical* boundaries (don't output PII, don't call dangerous tools). Governance policies enforce *business* boundaries (don't approve discounts above 20%, don't make promises about delivery times).

#### Policy Types

| Policy | Example | Enforcement Point |
|--------|---------|-------------------|
| **Monetary limits** | Refunds capped at $500 without manager approval | Tool gate |
| **Business rules** | Never offer free shipping on orders under $25 | Output guardrail |
| **Regulatory constraints** | GDPR: do not process data from users who have requested deletion | Input gate + data access layer |
| **Brand guidelines** | Never compare our product to competitors by name | Output guardrail |
| **Rate limits** | Max 50 agent interactions per user per day | API gateway |

#### Implementing Business Policies

Business policies belong in a policy engine, not in your agent's prompt. Prompt-based enforcement is fragile — the agent can be tricked into ignoring it. A policy engine evaluates rules deterministically — each active policy is checked against the agent's decision and context before execution, and any denial is logged and blocks the action.

Policies are version-controlled (policy changes are auditable just like code changes) and tested against known scenarios.

### 5. Compliance & Regulatory Landscape

The regulatory environment for AI is evolving fast.

#### OWASP Top 10 for Agentic Applications (2026)

Before looking at laws, look at the threat landscape. The [OWASP Agentic Security Initiative](https://genai.owasp.org/resource/owasp-top-10-for-agentic-applications-for-2026/) defines 10 risk categories (ASI01–ASI10) specific to autonomous agents: goal hijacking, tool misuse, identity abuse, supply chain compromise, code execution, context poisoning, insecure inter-agent communication, cascading failures, human trust exploitation, and rogue agents.

This is a practical starting point for threat modeling. Map each risk to a control in your governance stack. AGT ships with a [full mapping](https://github.com/microsoft/agent-governance-toolkit/blob/main/docs/compliance/owasp-agentic-top10-architecture.md) of every ASI risk to concrete mitigations (policy rules, static analysis rules, trust gates, circuit breakers) — useful as a reference even if you're not using the toolkit.

#### EU AI Act

The EU AI Act classifies AI systems into risk tiers:
- **Unacceptable risk**: Prohibited (social scoring, real-time biometric surveillance)
- **High risk**: Requires conformity assessment, human oversight, transparency, accuracy, robustness, cybersecurity
- **Limited risk**: Transparency obligations (users must know they're interacting with AI)
- **Minimal risk**: No specific obligations

Customer-facing agents (support, sales, financial advice) likely fall into **limited** or **high** risk depending on the domain. High-risk classification triggers significant documentation and oversight requirements.

#### US Landscape

No comprehensive federal AI law yet, but:
- **Executive Order 14110** (2023): Requires developers of powerful AI systems to share safety test results. Directs NIST to develop AI safety standards.
- **State-level laws**: Colorado's AI Act (CAIA, 2024) regulates high-risk AI systems; similar bills emerging in other states.
- **Industry-specific**: FDA for medical AI, SEC for financial AI, FTC for consumer protection.

#### What to Prepare Regardless of Jurisdiction

| Artifact | Purpose |
|----------|---------|
| **Model card** | Documents model capabilities, limitations, training data, evaluation results |
| **System card** | Documents the full agent system — not just the model but tools, guardrails, and integration points |
| **Impact assessment** | Documents the intended use, foreseeable misuse, affected stakeholders, and risk mitigations |
| **Incident response plan** | Defines what happens when the agent causes harm — who is notified, how fast, and what remediation looks like |
| **Transparency notice** | Tells users they're interacting with an AI agent, what it can/can't do, and how to escalate to a human |

## Execution Sandboxing & SRE

Governance isn't just about *deciding* what an agent can do — it's about *containing* what happens when it does it.

### Privilege Rings for Agent Execution

Traditional OSes use protection rings (Ring 0 kernel, Ring 3 user space). Apply the same model to agents:

| Ring | Access Level | Example |
|------|-------------|---------|
| **Ring 0** | Full system access | Agent deployment pipeline, admin override |
| **Ring 1** | Write access to own domain | Customer support agent modifying tickets |
| **Ring 2** | Read access across domains | Analytics agent querying across databases |
| **Ring 3** | Strictly sandboxed | Untrusted third-party agent, user-uploaded agent |

Each ring maps to a container/runtime profile with resource limits, network policies, and filesystem isolation. AGT's Agent Runtime implements this with Docker/gVisor profiles per ring level.

### SRE for Agents

Agents are software systems and need the same reliability engineering as any production service:

- **Kill switch**: A global circuit breaker that halts all agent decisions when anomalies are detected. Not a graceful shutdown — a hard stop. AGT provides this via Agent SRE's termination control.
- **SLOs & error budgets**: Define acceptable error rates for agent decisions (e.g., "<0.1% of refund decisions overturned"). When the error budget burns down, freeze the agent until root cause is identified.
- **Chaos testing**: Deliberately inject failures (tool unavailability, slow LLM responses, malformed inputs) to verify the agent degrades gracefully. Run this continuously, not as a one-off.
- **Circuit breakers**: If a downstream tool returns errors above a threshold, stop calling it. Don't let one failing tool cascade into agent-wide failure.

## Organizational Governance

Who owns AI governance? Without clear ownership, governance falls through the cracks between engineering, legal, and compliance.

### Recommended Structure

| Role | Responsibility |
|------|---------------|
| **AI Governance Lead** (or committee) | Cross-functional ownership of AI risk. Reports to CTO or Chief Risk Officer. |
| **Engineers** | Implement audit trails, policy enforcement, guardrails. Own the technical governance stack. |
| **Legal / Compliance** | Interpret regulatory requirements, review high-risk use cases, define retention policies. |
| **Product Managers** | Define business policies that become policy-as-code rules. Own the agent's intended use and limitations. |
| **Data Protection Officer (DPO)** | GDPR/CCPA compliance, data retention, PII handling within agent workflows. |
| **Security Team** | Red-teaming, vulnerability assessment, prompt injection testing. |

### Governance as a Gate

For customer-facing or regulated agents, integrate governance into the development lifecycle, not after the fact:

```
Idea → Risk Assessment → Design → [Governance Review Gate] → Build → [Safety Testing Gate] → Deploy → Monitor
```

The governance review gate checks: is there a model card? An impact assessment? Defined audit trail? Policy-as-code rules? Without these, the agent doesn't ship.

## Tools Landscape

Governance is a stack, not a single tool.

### Policy Engines

| Tool | Approach | Best For |
|------|----------|----------|
| **[Microsoft Agent Governance Toolkit](https://github.com/microsoft/agent-governance-toolkit)** | Full-stack agent governance kernel — deterministic policy engine, SPIFFE/DID identity, privilege rings, SRE circuit breakers, Merkle audit. Python/TS/.NET/Go/Rust. 992 conformance tests. | Teams running multi-agent systems who need a single governance stack across frameworks (LangChain, AutoGen, CrewAI, OpenAI Agents SDK). MIT licensed. |
| **[Open Policy Agent](https://www.openpolicyagent.org/)** (OPA) | General-purpose policy engine. Policies written in Rego — a declarative language. Decouples policy decisions from application code. | Teams that already use OPA for infrastructure/K8s and want to extend the same policy language to AI agents. CNCF graduated, 11.9k ★. |
| **[AWS Cedar](https://www.cedarpolicy.com/)** | Policy language and engine designed for application authorization. Used by AWS Verified Permissions. | Teams on AWS who want managed policy evaluation. Simpler than Rego; designed specifically for authZ (not general-purpose). |

### AI Gateways

| Tool | Approach | Best For |
|------|----------|----------|
| **[LiteLLM](https://github.com/BerriAI/litellm)** | Open-source Python proxy and SDK. Unified OpenAI-compatible API across 100+ LLM providers. Built-in cost tracking, guardrails, load balancing, and logging. 51k+ ★. | The most popular open-source AI gateway. Teams that want a battle-tested proxy with broad provider support. Self-hosted. |
| **[Bifrost](https://github.com/maximhq/bifrost)** (Maxim AI) | High-performance open-source AI gateway. Virtual keys with per-key model access, budgets, rate limits, MCP tool filtering. <100μs overhead at 5k RPS. 6k+ ★. | Teams that need infrastructure-level enforcement at the API gateway layer — govern which models and tools each team/agent can access. Drop-in OpenAI-compatible API. |
| **[Portkey Gateway](https://github.com/Portkey-AI/gateway)** | AI gateway with integrated guardrails. Routes to 1,600+ LLMs with 50+ guardrails. Prompt management, evaluation frameworks, MCP gateway. 12k+ ★. | Teams that want guardrails and prompt management built into the gateway layer rather than as separate tools. |
| **[Azure API Management AI Gateway](https://learn.microsoft.com/en-us/azure/api-management/genai-gateway-capabilities)** (Microsoft) | Managed AI gateway within Azure APIM. Authenticate, authorize, load-balance, and govern LLM endpoints, MCP servers, and A2A agent APIs. Unified model API for multi-provider governance. Integrates with Microsoft Foundry. | Teams on Azure who want a managed gateway with built-in compliance, token quotas, and self-service developer onboarding. |
| **[AWS Bedrock](https://aws.amazon.com/bedrock/)** (Amazon) | Managed AI service with native IAM integration, CloudWatch monitoring, and model customization. Governs model access through existing AWS IAM policies. | AWS-native teams who want governance through existing IAM without deploying a separate gateway. |
| **[GCP Agent Gateway](https://docs.cloud.google.com/gemini-enterprise-agent-platform/govern/gateways/agent-gateway-overview)** (Google) | Programmable data plane for user-to-agent, agent-to-agent, and agent-to-tool traffic. ISV ecosystem for injecting security controls (DLP, threat detection). Part of Gemini Enterprise Agent Platform. | GCP-native teams building multi-agent systems who want security controls injected at the network level. |

### Guardrails (Input/Output Safety)

Content guardrails — prompt injection detection, jailbreak protection, PII scanning, output moderation — are covered in detail in [Part 2: Safety for AI Agents]({% post_url 2026-06-13-safety-for-ai-agents %}). Key tools there include NVIDIA NeMo Guardrails, Guardrails AI, Lakera Guard, LLM Guard, and Llama Guard.

These operate at the *safety* layer (real-time, automated). The tools below operate at the *governance* layer (policy, audit, identity, compliance). The two are complementary — safety guardrails generate the events that governance consumes.

### Compliance & Risk Management

| Tool | Approach | Best For |
|------|----------|----------|
| **[Credo AI](https://www.credo.ai/)** | AI governance platform with pre-built policy packs for EU AI Act, NIST AI RMF, ISO 42001, SOC 2. Automated evidence generation, continuous governance loop. | Compliance teams in regulated industries that need audit-ready documentation and risk assessment workflows. |
| **[IBM Watsonx.governance](https://www.ibm.com/watsonx/governance)** | Integrated risk scoring, bias/fairness monitoring, model lifecycle tracking. Tightly coupled to IBM Cloud. | Organizations already on the IBM ecosystem. |
| **[OneTrust AI Governance](https://www.onetrust.com/)** | Extends OneTrust's GRC platform to AI: data lineage integration, automated impact assessments, vendor AI oversight. | Privacy/compliance teams in heavily regulated industries (finance, healthcare) that need AI governance integrated with existing GRC. |

### Agent Identity & Access

| Tool | Approach | Best For |
|------|----------|----------|
| **[Frontegg Agent IAM](https://frontegg.com/product/agent-iam)** | Extends traditional IAM (RBAC, ReBAC, feature flags) to AI agents as first-class identities. Step-up auth for high-risk actions, PII masking. | SaaS platforms that need to govern AI agent actions on behalf of end tenants. |
| **[SPIFFE/SPIRE](https://spiffe.io/)** | Workload identity standard (X.509 certificates). Already used in Kubernetes/Istio for service identity — same primitives work for agents. | Teams with existing SPIFFE infrastructure. AGT uses this under the hood. |

### Audit & Observability

These were covered in detail in [Part 1: Observability]({% post_url 2026-06-12-observability-concepts-signals-ai-agents %}). Key platforms to evaluate:

| Tool | Focus |
|------|-------|
| **[Langfuse](https://langfuse.com/)** | Open-source LLM observability — traces, evaluations, cost tracking. Can serve as audit store for agent decisions. |
| **[Braintrust](https://www.braintrust.dev/)** | AI evaluation platform with dataset management, scoring, and experiment tracking. AI-native audit schemas. |
| **[Arize Phoenix](https://github.com/Arize-AI/phoenix)** | Open-source observability for LLM apps — spans, traces, embeddings drift. |

### How the Pieces Fit Together

No single tool covers everything. A practical stack for a mid-size team running agents in production:

```
Agent Code
    │
    ▼
┌─────────────────────────────┐
│  Guardrails → See Part 2    │  ← Content safety: NeMo Guardrails, Lakera, LLM Guard
├─────────────────────────────┤
│  Policy Engine (AGT / OPA)  │  ← Action governance: allow/deny tool calls, enforce budgets
├─────────────────────────────┤
│  Gateway (Bifrost / AGT)    │  ← Infrastructure enforcement: keys, rate limits, tool filtering
├─────────────────────────────┤
│  Identity (SPIFFE / Agent IAM) │ ← Who is this agent? What can it do on whose behalf?
├─────────────────────────────┤
│  Audit (Langfuse / AGT)     │  ← Prove what happened
├─────────────────────────────┤
│  Compliance (Credo / OneTrust) │ ← Map to regulations, generate evidence
└─────────────────────────────┘
```

The tools you pick depend on your stack and maturity. Start with the layer that addresses your most immediate risk, get it working in production, then add the next. Don't try to deploy all of this at once.

Governance is the organizational layer that makes safety provable and auditable. The [Agent Governance Toolkit](https://github.com/microsoft/agent-governance-toolkit) (`pip install agent-governance-toolkit`) provides a production-grade open-source implementation of this entire stack — if you're building agent governance from scratch, start there rather than reinventing each layer.

Core stack layers:

1. **Access control** — least-privilege tools scoped per user session, cryptographic agent identity
2. **Audit trail** — immutable, queryable record of every agent decision
3. **Model lineage** — registry tracking every model, prompt, and configuration version
4. **Policy-as-code** — business rules enforced deterministically, not via prompting
5. **Compliance artifacts** — model cards, system cards, impact assessments prepared before deployment

Beyond the stack: **execution sandboxing** (privilege rings, resource limits, container isolation per agent risk tier) and **organizational governance** (clear ownership, governance gates in the development lifecycle).

Start with access control — it's the foundation everything else builds on. If you can't control who invokes your agent and with what permissions, nothing above it matters.

> The companion article [Safety for AI Agents]({% post_url 2026-06-13-safety-for-ai-agents %}) covers the real-time guardrail layer — prompt injection defenses, tool gates, content filtering, and red-teaming. Governance builds on safety: safety stops the agent from doing harm *now*; governance proves it has *always* operated correctly.

## References

- [EU AI Act](https://artificialintelligenceact.eu/)
- [AWS Cedar Policy Language](https://www.cedarpolicy.com/)
- [Open Policy Agent](https://www.openpolicyagent.org/) — General-purpose policy engine with Rego language (CNCF graduated)
- [NIST AI Risk Management Framework](https://www.nist.gov/itl/ai-risk-management-framework)
- [MLflow Model Registry](https://mlflow.org/docs/latest/model-registry.html)
- [Google Model Card Toolkit](https://github.com/google/model-card-toolkit)
- [Microsoft Responsible AI Standard v2](https://www.microsoft.com/en-us/ai/responsible-ai)
- [Microsoft Agent Governance Toolkit](https://github.com/microsoft/agent-governance-toolkit) — Policy engine, identity, sandboxing, SRE, and compliance for AI agents (Python/TS/.NET/Go/Rust)
- [LiteLLM](https://github.com/BerriAI/litellm) — Open-source AI gateway and Python SDK for 100+ LLMs with cost tracking, guardrails, and load balancing
- [Portkey Gateway](https://github.com/Portkey-AI/gateway) — AI gateway with integrated guardrails, prompt management, and 1,600+ LLM support
- [Bifrost by Maxim AI](https://github.com/maximhq/bifrost) — Open-source AI gateway with virtual keys, budgets, and MCP tool filtering
- [Azure API Management AI Gateway](https://learn.microsoft.com/en-us/azure/api-management/genai-gateway-capabilities) — Managed AI gateway for governing LLMs, MCP servers, and agent APIs on Azure
- [AWS Bedrock](https://aws.amazon.com/bedrock/) — Managed AI service with IAM-integrated model access governance
- [GCP Agent Gateway](https://docs.cloud.google.com/gemini-enterprise-agent-platform/govern/gateways/agent-gateway-overview) — Google Cloud's programmable data plane for agent traffic governance
- [Credo AI](https://www.credo.ai/) — AI governance platform with lifecycle compliance and automated evidence
- [Frontegg Agent IAM](https://frontegg.com/product/agent-iam) — Identity and access management for AI agents
- [OWASP Top 10 for Agentic Applications (2026)](https://genai.owasp.org/resource/owasp-top-10-for-agentic-applications-for-2026/) — Threat taxonomy for autonomous AI agents
