# Governance

> **Parent**: [Observability, Safety & Governance](./00-observability-safety-governance-overview.md)
> **Layer**: Organizational — builds on Safety and Observability

Safety stops the agent from doing something harmful *right now*. Governance ensures you can prove — to regulators, auditors, and your own leadership — that the system operates within defined boundaries *over time*. Safety is operational. Governance is organizational.

---

## Safety vs. Governance — The Distinction

| | Safety | Governance |
|---|--------|------------|
| **Timeframe** | Real-time (milliseconds to seconds) | Historical (days to years) |
| **Actor** | Automated guardrails | Humans (compliance officers, auditors, engineering leads) |
| **Question** | "Should the agent do this *right now*?" | "Can we prove the agent has *always* operated correctly?" |
| **Failure mode** | Blocked action, filtered response | Audit finding, regulatory penalty, reputational damage |
| **Tool** | Guardrails, content classifiers, tool gates | Audit logs, policy engines, model registries, compliance reports |

The two are complementary. Safety guardrails generate the events that governance consumes. An output content filter that fires is both a safety action (blocking harmful content) and a governance event (logged for compliance review).

> **Reference implementation**: Microsoft's open-source [Agent Governance Toolkit](https://github.com/microsoft/agent-governance-toolkit) (AGT, Python/TypeScript/.NET/Go/Rust) provides a deterministic policy engine, cryptographic agent identity (SPIFFE/DID/mTLS), execution sandboxing with privilege rings, Merkle-chained audit logs, and 992 conformance tests. It serves as a concrete example of the patterns described here.

---

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

Each layer builds on the one below. You can't have an audit trail without knowing who accessed what. You can't have policy enforcement without an audit trail to validate against. You can't have compliance reports without a model registry.

### 1. Access Control & Permissions

Agent access control is more nuanced than traditional RBAC — permissions apply at multiple levels:

| Level | Controls | Example |
|-------|----------|---------|
| **Agent access** | Who can invoke this agent | Only "Premium Support" tier can use the refund agent |
| **Tool access** | Which tools the agent is allowed to call for this user | Agent can `lookup_order` for everyone but `approve_refund` only for authenticated users |
| **Data access** | What data the agent can see | Agent running for user A can only see user A's orders |
| **Decision authority** | Which decisions auto-execute vs. require approval | Refunds <$50 auto-approve; >$50 queue for human review |

#### Agent Identity — "Which Agent Did This?"

In multi-agent systems where multiple agents share an API key, you need to answer "which *agent* took this action?"

| Approach | Mechanism | Strength |
|----------|-----------|----------|
| **API key per agent** | Provision separate key for each agent instance | Simple; keys can leak |
| **SPIFFE/X.509** | Short-lived X.509 certificates issued to each agent workload | Industry standard (Kubernetes, Istio) |
| **DID (Decentralized Identifier)** | Self-sovereign identity with Ed25519 keys; agent signs every inter-agent message | No central CA; verifiable without network calls |
| **mTLS** | Mutual TLS between agents; both sides present certificates | Transport-layer; pairs with SPIFFE |

#### Implementing Least-Privilege Tools

The agent doesn't receive the full tool catalog — it gets a subset based on the current user's permissions. This limits damage from prompt injection: even if the attacker tricks the agent into calling `approve_refund`, the tool simply doesn't exist in that session's context.

#### Policy-as-Code for Tool Authorization

Use a policy engine for fine-grained authorization: e.g., "premium agents can approve refunds only when order amount < $500 and within 30 days of creation." Policy engines like [Cedar](https://www.cedarpolicy.com/) (AWS Verified Permissions) or [Open Policy Agent](https://www.openpolicyagent.org/) (OPA) let you express business rules as version-controlled, testable code.

#### Design Guideline

**Implement least-privilege tool access from day one. Per-agent API keys are a sufficient starting point for identity. Graduate to SPIFFE or DID when you have more than a handful of agents or when agents delegate tasks to each other.**

### 2. Immutable Audit Trail

Every agent interaction must produce a structured audit record answering: **who did what, when, using which model, with what data, and what was the outcome?**

#### What to Record

Each audit record should capture:
- **Event ID** and **Trace ID** (links to observability traces)
- **Agent identity**: name, version, cryptographic identity
- **Model configuration**: provider, model ID, version date, temperature, prompt template hash
- **User context**: ID, role, session
- **Decision**: action, amount, confidence, rationale
- **Tool calls**: input/output hashes (full payloads in separate blob storage)
- **Retrieved documents**: source, hash
- **Guardrail results**: which guardrails fired, scores, actions taken
- **Outcome**: success, failure, escalation

#### Storage Requirements

| Requirement | Implementation |
|-------------|---------------|
| **Append-only** | Write-once storage — S3 Object Lock, Azure Immutable Blob, append-only ledger DBs. No updates, no deletes. |
| **Tamper-evident** | Cryptographically chain records — e.g., Merkle audit chains. Each entry includes hash of previous entry. Regulator can validate entire history without trusting your database. |
| **Retention** | Define based on regulatory requirements. GDPR: proportionality. Financial services: often 7 years. Define and enforce. |
| **Queryable** | Index on `agent_id`, `decision.action`, `decision.amount`, `timestamp`. Auditors need ad-hoc queries. |

#### Architectural Options

| Approach | Pros | Cons |
|----------|------|------|
| **Relational DB with append-only tables** | Familiar, queryable, ACID | Scale limits, retention costs |
| **Event sourcing** (Kafka + blob storage) | Immutable by design, replayable | Operational complexity |
| **Cloud audit services** (AWS CloudTrail, Azure Monitor) | Managed, compliant | Cost at high volume, not AI-aware natively |
| **Dedicated AI audit platforms** (Langfuse, Braintrust) | AI-native schemas, built-in eval | Vendor lock-in |

#### Design Guideline

**Start with append-only PostgreSQL or event sourcing to blob storage. Graduate to dedicated platforms when volume or compliance demands.** Instrument audit records from day one — retrofitting after a year is expensive and may leave gaps.

### 3. AI Registry & Model Lineage

When a regulator asks "which model made this decision?", answer precisely — not just "GPT-4o" but "GPT-4o `gpt-4o-2024-08-06` with temperature 0.3 and prompt template v2.4.1."

#### What to Track

Every agent deployment records:
- **Model**: Provider + model ID + version date
- **Prompt template**: Hash (integrity verification) + version number
- **Configuration**: Temperature, top_p, max_tokens, stop sequences, tool definitions
- **Deployment metadata**: Who deployed, when, to which environment, with which approval
- **Evaluation scores**: Safety, accuracy, groundedness at deployment time

A **model registry** catalogs every AI artifact in production. Tools: MLflow Model Registry, Weights & Biases Model Registry, or a simple versioned database table. Every audit record references a specific agent version; the registry provides full context.

#### Design Guideline

**Version your prompts like you version your code.** A prompt template hash in every audit record lets you answer "what was the system prompt when this decision was made?" without ambiguity.

### 4. Policy-as-Code Enforcement

Safety guardrails enforce *technical* boundaries. Governance policies enforce *business* boundaries.

| Policy Type | Example | Enforcement Point |
|-------------|---------|-------------------|
| **Monetary limits** | Refunds capped at $500 without manager approval | Tool gate |
| **Business rules** | Never offer free shipping on orders under $25 | Output guardrail |
| **Regulatory constraints** | GDPR: do not process data from users who requested deletion | Input gate + data access layer |
| **Brand guidelines** | Never compare our product to competitors by name | Output guardrail |
| **Rate limits** | Max 50 agent interactions per user per day | API gateway |

#### Implementation

Business policies belong in a **policy engine**, not in the agent's prompt. Prompt-based enforcement is fragile — the agent can be tricked into ignoring it. A policy engine evaluates rules deterministically:
- Each active policy is checked against the agent's decision and context before execution
- Any denial is logged and blocks the action
- Policies are version-controlled (policy changes are auditable like code changes)
- Policies are tested against known scenarios

#### Design Guideline

**Encode business rules in a policy engine (OPA, Cedar, AGT), not in system prompts.** Prompt-enforced rules are a governance theater — they can be bypassed, and there's no deterministic record of whether they were followed.

### 5. Compliance & Regulatory Landscape

#### OWASP Top 10 for Agentic Applications (2026)

Before looking at laws, map your threat landscape. The OWASP Agentic Security Initiative defines 10 risk categories (ASI01–ASI10): goal hijacking, tool misuse, identity abuse, supply chain compromise, code execution, context poisoning, insecure inter-agent communication, cascading failures, human trust exploitation, and rogue agents. Map each to a control in your governance stack.

#### EU AI Act

| Risk Tier | Examples | Requirements |
|-----------|----------|--------------|
| **Unacceptable** | Social scoring, real-time biometric surveillance | Prohibited |
| **High risk** | Critical infrastructure, employment, essential services | Conformity assessment, human oversight, transparency, accuracy, robustness, cybersecurity |
| **Limited risk** | Chatbots, emotion recognition | Transparency: users must know they're interacting with AI |
| **Minimal risk** | AI-enabled video games, spam filters | No specific obligations |

Customer-facing agents (support, sales, financial advice) likely fall into **limited** or **high** risk depending on domain. **Fines**: up to €35M or 7% of global turnover (Article 99, highest tier).

> Enforcement timeline: The "AI Omnibus" political agreement (May 2026) pushed high-risk enforcement to December 2027 / August 2028. Standards for high-risk systems remain in draft.

#### US Landscape

No comprehensive federal AI law yet. Key developments:
- **Executive Order 14110** (2023): Safety test sharing, NIST standards development
- **State-level**: Colorado AI Act (CAIA, 2024); similar bills emerging
- **Industry-specific**: FDA (medical AI), SEC (financial AI), FTC (consumer protection)

#### What to Prepare Regardless of Jurisdiction

| Artifact | Purpose |
|----------|---------|
| **Model card** | Model capabilities, limitations, training data, evaluation results |
| **System card** | Full agent system — tools, guardrails, integration points |
| **Impact assessment** | Intended use, foreseeable misuse, affected stakeholders, risk mitigations |
| **Incident response plan** | What happens when the agent causes harm — notification, remediation |
| **Transparency notice** | Users know they're interacting with AI, its capabilities/limitations, how to escalate |

#### Design Guideline

**Prepare model cards, system cards, and impact assessments before deploying to production.** They're required by emerging regulation and serve as the evidence base for any audit.

---

## Execution Sandboxing & SRE

Governance isn't just about *deciding* what an agent can do — it's about *containing* what happens when it does it.

### Privilege Rings

Apply OS protection rings to agent execution:

| Ring | Access Level | Example |
|------|-------------|---------|
| **Ring 0** | Full system access | Agent deployment pipeline, admin override |
| **Ring 1** | Write access to own domain | Customer support agent modifying tickets |
| **Ring 2** | Read access across domains | Analytics agent querying across databases |
| **Ring 3** | Strictly sandboxed | Untrusted third-party agent, user-uploaded agent |

Each ring maps to a container/runtime profile with resource limits, network policies, and filesystem isolation. Docker/gVisor profiles per ring level.

### SRE for Agents

Agents are software systems and need the same reliability engineering:

| Practice | Implementation |
|----------|---------------|
| **Kill switch** | Global circuit breaker halting all agent decisions on anomaly detection. Hard stop, not graceful shutdown. |
| **SLOs & error budgets** | Define acceptable error rates (e.g., "<0.1% of refund decisions overturned"). When error budget burns down, freeze agent until root cause identified. |
| **Chaos testing** | Deliberately inject failures (tool unavailability, slow LLM responses, malformed inputs). Run continuously. |
| **Circuit breakers** | If downstream tool returns errors above threshold, stop calling it. Prevent cascading failure. |

---

## Organizational Governance

Without clear ownership, governance falls through the cracks between engineering, legal, and compliance.

### Recommended Structure

| Role | Responsibility |
|------|---------------|
| **AI Governance Lead** (or committee) | Cross-functional ownership of AI risk. Reports to CTO or Chief Risk Officer. |
| **Engineers** | Implement audit trails, policy enforcement, guardrails. Own the technical governance stack. |
| **Legal / Compliance** | Interpret regulatory requirements, review high-risk use cases, define retention policies. |
| **Product Managers** | Define business policies → policy-as-code rules. Own intended use and limitations. |
| **Data Protection Officer (DPO)** | GDPR/CCPA compliance, data retention, PII handling within agent workflows. |
| **Security Team** | Red-teaming, vulnerability assessment, prompt injection testing. |

### Governance as a Gate

Integrate governance into the development lifecycle, not after the fact:

```
Idea → Risk Assessment → Design → [Governance Review Gate] → Build → [Safety Testing Gate] → Deploy → Monitor
```

The governance review gate checks: model card? Impact assessment? Defined audit trail? Policy-as-code rules? Without these, the agent doesn't ship.

---

## Tools Landscape

Governance is a stack, not a single tool.

### Policy Engines

| Tool | Best For |
|------|----------|
| **[Microsoft Agent Governance Toolkit](https://github.com/microsoft/agent-governance-toolkit)** | Full-stack agent governance kernel — deterministic policy engine, SPIFFE/DID identity, privilege rings, SRE circuit breakers, Merkle audit. Python/TS/.NET/Go/Rust. MIT. |
| **[Open Policy Agent](https://www.openpolicyagent.org/)** (OPA) | Teams with existing OPA/K8s infrastructure. Policies in Rego. CNCF graduated, 11.9k ★. |
| **[AWS Cedar](https://www.cedarpolicy.com/)** | AWS-native teams. Simpler than Rego; designed for application authorization. |

### AI Gateways

| Tool | Best For |
|------|----------|
| **[LiteLLM](https://github.com/BerriAI/litellm)** | Most popular open-source AI proxy. Unified API across 100+ LLMs. Cost tracking, guardrails, load balancing. 51k+ ★. |
| **[Bifrost](https://github.com/maximhq/bifrost)** (Maxim AI) | Infrastructure-level enforcement. Virtual keys, budgets, rate limits, MCP tool filtering. <100μs overhead at 5k RPS. 6k+ ★. |
| **[Portkey Gateway](https://github.com/Portkey-AI/gateway)** | Integrated guardrails + prompt management. 1,600+ LLMs, 50+ guardrails. 12k+ ★. |
| **[Azure API Management AI Gateway](https://learn.microsoft.com/en-us/azure/api-management/genai-gateway-capabilities)** | Managed gateway on Azure. Token quotas, self-service developer onboarding, Foundry integration. |
| **[AWS Bedrock](https://aws.amazon.com/bedrock/)** | IAM-integrated model access governance. No separate gateway. |
| **[GCP Agent Gateway](https://docs.cloud.google.com/gemini-enterprise-agent-platform/govern/gateways/agent-gateway-overview)** | Programmable data plane for agent-to-agent and agent-to-tool traffic. ISV ecosystem for security controls. |

### Agent Identity & Access

| Tool | Best For |
|------|----------|
| **[Frontegg Agent IAM](https://frontegg.com/product/agent-iam)** | SaaS platforms governing agent actions on behalf of end tenants. RBAC, ReBAC, step-up auth. |
| **[SPIFFE/SPIRE](https://spiffe.io/)** | Workload identity standard (X.509). Existing SPIFFE infrastructure in Kubernetes/Istio. |

### Compliance & Risk Management

| Tool | Best For |
|------|----------|
| **[Credo AI](https://www.credo.ai/)** | Pre-built policy packs for EU AI Act, NIST AI RMF, ISO 42001, SOC 2. Automated evidence generation. |
| **[IBM Watsonx.governance](https://www.ibm.com/watsonx/governance)** | IBM ecosystem. Risk scoring, bias/fairness monitoring, model lifecycle tracking. |
| **[OneTrust AI Governance](https://www.onetrust.com/)** | Heavily regulated industries. Data lineage, automated impact assessments, vendor AI oversight. |

### How the Pieces Fit Together

```
Agent Code
    │
    ▼
┌─────────────────────────────┐
│  Guardrails (Safety layer)  │  ← Content safety: NeMo Guardrails, LLM Guard
├─────────────────────────────┤
│  Policy Engine (AGT / OPA)  │  ← Action governance: allow/deny tool calls, enforce budgets
├─────────────────────────────┤
│  Gateway (Bifrost / LiteLLM)│  ← Infrastructure enforcement: keys, rate limits, tool filtering
├─────────────────────────────┤
│  Identity (SPIFFE / IAM)    │  ← Who is this agent? What can it do on whose behalf?
├─────────────────────────────┤
│  Audit (Langfuse / AGT)     │  ← Prove what happened
├─────────────────────────────┤
│  Compliance (Credo / OneTrust)│ ← Map to regulations, generate evidence
└─────────────────────────────┘
```

#### Design Guideline

**Start with the layer that addresses your most immediate risk. Get it working in production, then add the next.** Don't try to deploy all of this at once. The tools you pick depend on your cloud stack: Azure → Foundry + APIM AI Gateway + Credo. AWS → Bedrock + Cedar + CloudTrail. Multi-cloud → OPA + LiteLLM + Langfuse.

---

## Summary — Design Guidelines

1. **Access control is the foundation.** Implement least-privilege tools scoped per user session from day one. If you can't control who invokes your agent and with what permissions, nothing above it matters.
2. **Audit trail must be immutable and queryable.** Append-only storage, tamper-evident (Merkle chains), indexed for ad-hoc queries. Instrument from day one — retrofitting leaves gaps.
3. **Version your models, prompts, and configurations.** A model registry is not optional for production. Every audit record references a specific version with full context.
4. **Encode business policies in a policy engine, not in prompts.** Prompt-enforced rules are governance theater — bypassable and unauditable.
5. **Prepare compliance artifacts before deployment.** Model cards, system cards, impact assessments. They're your evidence base for any audit.
6. **Sandbox agent execution.** Privilege rings, resource limits, container isolation per risk tier. Contain the blast radius.
7. **Governance is an organizational function.** Assign clear ownership (AI Governance Lead or committee). Integrate governance gates into the development lifecycle — not after the fact.

---

## Key References

- [EU AI Act](https://artificialintelligenceact.eu/)
- [NIST AI Risk Management Framework](https://www.nist.gov/itl/ai-risk-management-framework)
- [Microsoft Agent Governance Toolkit](https://github.com/microsoft/agent-governance-toolkit)
- [OWASP Top 10 for Agentic Applications (2026)](https://genai.owasp.org/resource/owasp-top-10-for-agentic-applications-for-2026/)
- [Open Policy Agent](https://www.openpolicyagent.org/)
- [AWS Cedar Policy Language](https://www.cedarpolicy.com/)
- [LiteLLM](https://github.com/BerriAI/litellm)
- [Bifrost by Maxim AI](https://github.com/maximhq/bifrost)
- [Portkey Gateway](https://github.com/Portkey-AI/gateway)
- [Azure API Management AI Gateway](https://learn.microsoft.com/en-us/azure/api-management/genai-gateway-capabilities)
- [GCP Agent Gateway](https://docs.cloud.google.com/gemini-enterprise-agent-platform/govern/gateways/agent-gateway-overview)
- [Credo AI](https://www.credo.ai/)
- [Frontegg Agent IAM](https://frontegg.com/product/agent-iam)
- [SPIFFE/SPIRE](https://spiffe.io/)
- [MLflow Model Registry](https://mlflow.org/docs/latest/model-registry.html)
- [Google Model Card Toolkit](https://github.com/google/model-card-toolkit)
- [Microsoft Responsible AI Standard v2](https://www.microsoft.com/en-us/ai/responsible-ai)
- [Jones Walker — Building Governance That Actually Works](https://www.joneswalker.com/en/insights/blogs/ai-law-blog/ai-governance-series-part-3-building-governance-that-actually-works.html)
- [Cyber Security Tribe — What Separates Real AI Governance From Policy Theater](https://www.cybersecuritytribe.com/articles/what-separates-real-ai-governance-from-policy-theater)
- [Institute for Law & AI — Automated Compliance](https://law-ai.org/automated-compliance-and-the-regulation-of-ai/)
