---
layout: post
title: "Governance for AI Agents — Policy, Audit, and Compliance"
date: 2026-06-14
tags: [ai, llm, governance, compliance, audit, policy, agents, production, eu-ai-act]
categories: programming
series: production-ai-agents
series_index: 3
---

*Part 3 of a 4-part series on running AI agents in production. Also see: [Observability](/), [Safety](/), and the [series overview](/).*

---

Safety stops the agent from doing something harmful *right now*. Governance ensures you can prove — to regulators, auditors, and your own leadership — that the system operates within defined boundaries *over time*.

Safety is operational. Governance is organizational.

This post covers the governance layer for AI agents: audit trails, policy enforcement, model lineage, compliance frameworks, and the organizational structures that support them.

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

Each layer builds on the one below. You can't have policy enforcement without an audit trail to validate against. You can't have compliance reports without a model registry to know what was deployed when.

## 1. Audit Trail

An audit trail for AI agents must answer: **who did what, when, using which model, with what data, and what was the outcome?**

### What to Record

Every agent interaction should produce a structured audit record:

```json
{
  "event_id": "evt_9a7b3c",
  "timestamp": "2026-06-12T14:32:01.421Z",
  "trace_id": "trace_8f2a1d",
  "agent_id": "customer-support-agent",
  "agent_version": "2.4.1",
  "model": {
    "name": "gpt-4o-2024-08-06",
    "temperature": 0.3,
    "prompt_template_hash": "sha256:abc123..."
  },
  "user": {
    "id": "user_5542",
    "role": "customer",
    "session_id": "sess_7711"
  },
  "request": {
    "type": "text",
    "hash": "sha256:def456...",
    "token_count": 42
  },
  "reasoning": {
    "plan": "Check refund eligibility → look up order → calculate refund amount",
    "tool_calls": [
      {"tool": "lookup_order", "params": {"order_id": "ORD-8821"}, "result_hash": "sha256:ghi789..."},
      {"tool": "calculate_refund", "params": {"amount": 149.99}, "result_hash": "sha256:jkl012..."}
    ],
    "retrieved_docs": [
      {"source": "refund_policy_v3", "section": "30-day-returns"}
    ]
  },
  "decision": {
    "action": "approve_refund",
    "amount": 149.99,
    "confidence": 0.94,
    "rationale": "Order within 30-day window. Full refund per policy."
  },
  "guardrails": {
    "input_scan": "passed",
    "output_scan": "passed",
    "human_approval": null
  },
  "outcome": {
    "status": "approved",
    "user_feedback": null
  }
}
```

Notice what's hashed, not stored in plaintext: the full prompt and retrieved documents. Storing every prompt in your audit database is expensive and often unnecessary. Store hashes for integrity verification and pointers to blob storage for the full payloads.

### Storage Requirements

- **Append-only**: Records must be immutable. No updates, no deletes. Use write-once storage (S3 Object Lock, Azure Immutable Blob, append-only ledger DBs).
- **Retention**: Define retention periods based on your regulatory requirements. GDPR doesn't specify a fixed period but requires proportionality. Financial services often require 7 years. Define a policy and stick to it.
- **Queryable**: Auditors need to answer questions like "show me all refunds approved above $500 in Q3" without writing custom code. Index on `agent_id`, `decision.action`, `decision.amount`, `timestamp`.

### Architectural Options

| Approach | Pros | Cons |
|----------|------|------|
| **Relational DB with append-only tables** | Familiar, queryable, ACID | Scale limits, retention costs |
| **Event sourcing (Kafka + blob storage)** | Immutable by design, replayable | Operational complexity |
| **Cloud audit services** (AWS CloudTrail, Azure Monitor) | Managed, compliant out of the box | Cost at high volume, not AI-aware natively |
| **Dedicated AI audit platforms** (Langfuse, Braintrust) | AI-native schemas, built-in eval | Vendor lock-in, another tool to manage |

Most teams start with append-only PostgreSQL or event sourcing to blob storage, then graduate to dedicated platforms when volume or compliance requirements demand it.

## 2. Access Control

Agent access control is more nuanced than traditional RBAC because permissions apply at multiple levels:

| Level | Controls | Example |
|-------|----------|---------|
| **Agent access** | Who can invoke this agent | Only "Premium Support" tier can use the refund agent |
| **Tool access** | Which tools the agent is allowed to call for this user | Agent can `lookup_order` for everyone but `approve_refund` only for authenticated users |
| **Data access** | What data the agent can see | Agent running for user A can only see user A's orders |
| **Decision authority** | Which decisions auto-execute vs. require approval | Refunds <$50 auto-approve; >$50 queue for human review |

### Implementing Least-Privilege Tools

The agent's tool set should be scoped per user session:

```python
def get_agent_tools(user: User, session: Session) -> list[Tool]:
    tools = []

    if user.is_authenticated:
        tools.append(lookup_order)       # Always available for authenticated users
        tools.append(check_refund_policy)

    if user.tier == "premium":
        tools.append(approve_refund)    # Premium only

    if user.role == "admin":
        tools.append(override_decision) # Admin only
        tools.append(view_all_orders)

    return tools
```

The agent doesn't receive the full tool catalog — it gets a subset based on the current user's permissions. This limits the damage from prompt injection: even if the attacker tricks the agent into calling `approve_refund`, the tool simply doesn't exist in that session's context.

### Policy-as-Code for Tool Authorization

Beyond simple role checks, use a policy engine for fine-grained authorization:

```cedar
// Cedar policy: who can approve refunds
permit(
  principal in Role::"PremiumAgent",
  action == Action::"approve_refund",
  resource is Order
)
when {
  resource.amount <= 500 &&
  resource.created_at.daysSince() <= 30
};
```

Policy engines like [Cedar](https://www.cedarpolicy.com/) (used by AWS Verified Permissions) or [Open Policy Agent](https://www.openpolicyagent.org/) let you express business rules as version-controlled, testable code. The policy is evaluated at the tool gate before execution — it's the governance counterpart to the safety tool gate.

## 3. Model Versioning & Lineage

When a regulator asks "which model made this decision?", you need to answer precisely — not just "GPT-4o" but "GPT-4o `gpt-4o-2024-08-06` with temperature 0.3 and prompt template v2.4.1."

### What to Track

Every agent deployment should record:
- **Model**: Provider + model ID + version date (e.g., `openai/gpt-4o-2024-08-06`)
- **Prompt template**: Hash of the system prompt (for integrity verification) + version number
- **Configuration**: Temperature, top_p, max_tokens, stop sequences, tool definitions
- **Deployment metadata**: Who deployed, when, to which environment, with which approval

This data lives in a **model registry** — a catalog of every AI artifact in production.

### Model Registry

A model registry for agents isn't just about model weights. It tracks:

```
Agent: customer-support-agent
├── v2.4.1 (current)
│   ├── Model: openai/gpt-4o-2024-08-06
│   ├── Prompt template: refund_prompt_v3 (hash: sha256:abc...)
│   ├── Tools: [lookup_order, check_refund_policy, approve_refund]
│   ├── Eval scores: groundedness=0.91, relevance=0.87, safety=0.98
│   ├── Deployed by: jane@company.com
│   └── Deployed at: 2026-06-10T09:00:00Z
├── v2.4.0 (superseded)
│   ├── Model: openai/gpt-4o-2024-05-13
│   └── ...
└── v2.3.0 (superseded)
```

Tools like MLflow Model Registry, Weights & Biases Model Registry, or a simple database table can serve this purpose. The key is that every audit record references a specific agent version, and the registry provides the full context for that version.

## 4. Policy Enforcement

Safety guardrails enforce *technical* boundaries (don't output PII, don't call dangerous tools). Governance policies enforce *business* boundaries (don't approve discounts above 20%, don't make promises about delivery times).

### Policy Types

| Policy | Example | Enforcement Point |
|--------|---------|-------------------|
| **Monetary limits** | Refunds capped at $500 without manager approval | Tool gate |
| **Business rules** | Never offer free shipping on orders under $25 | Output guardrail |
| **Regulatory constraints** | GDPR: do not process data from users who have requested deletion | Input gate + data access layer |
| **Brand guidelines** | Never compare our product to competitors by name | Output guardrail |
| **Rate limits** | Max 50 agent interactions per user per day | API gateway |

### Implementing Business Policies

Business policies belong in a policy engine, not in your agent's prompt. Prompt-based enforcement is fragile — the agent can be tricked into ignoring it. A policy engine evaluates rules deterministically:

```python
# Before the agent's decision takes effect
def enforce_policies(decision: AgentDecision, context: AgentContext) -> PolicyResult:
    for policy in active_policies:
        result = policy.evaluate(decision, context)
        if result == PolicyResult.DENY:
            log_violation(policy, decision, context)
            return PolicyResult.DENY

    return PolicyResult.ALLOW
```

Policies are version-controlled (policy changes are auditable just like code changes) and tested against known scenarios.

## 5. Compliance & Regulatory Landscape

The regulatory environment for AI is evolving fast. Key frameworks to be aware of:

### EU AI Act

The EU AI Act classifies AI systems into risk tiers:
- **Unacceptable risk**: Prohibited (social scoring, real-time biometric surveillance)
- **High risk**: Requires conformity assessment, human oversight, transparency, accuracy, robustness, cybersecurity
- **Limited risk**: Transparency obligations (users must know they're interacting with AI)
- **Minimal risk**: No specific obligations

Customer-facing agents (support, sales, financial advice) likely fall into **limited** or **high** risk depending on the domain. High-risk classification triggers significant documentation and oversight requirements.

### US Landscape

No comprehensive federal AI law yet, but:
- **Executive Order 14110** (2023): Requires developers of powerful AI systems to share safety test results. Directs NIST to develop AI safety standards.
- **State-level laws**: Colorado's AI Act (CAIA, 2024) regulates high-risk AI systems; similar bills emerging in other states.
- **Industry-specific**: FDA for medical AI, SEC for financial AI, FTC for consumer protection.

### What to Prepare Regardless of Jurisdiction

| Artifact | Purpose |
|----------|---------|
| **Model card** | Documents model capabilities, limitations, training data, evaluation results |
| **System card** | Documents the full agent system — not just the model but tools, guardrails, and integration points |
| **Impact assessment** | Documents the intended use, foreseeable misuse, affected stakeholders, and risk mitigations |
| **Incident response plan** | Defines what happens when the agent causes harm — who is notified, how fast, and what remediation looks like |
| **Transparency notice** | Tells users they're interacting with an AI agent, what it can/can't do, and how to escalate to a human |

## 6. Organizational Governance

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

Integrate governance into the development lifecycle, not after the fact:

```
Idea → Risk Assessment → Design → [Governance Review Gate] → Build → [Safety Testing Gate] → Deploy → Monitor
```

The governance review gate checks: is there a model card? An impact assessment? Defined audit trail? Policy-as-code rules? Without these, the agent doesn't ship.

## Summary

Governance is the organizational layer that makes safety provable and auditable:

1. **Audit trail** — immutable, queryable record of every agent decision
2. **Access control** — least-privilege tools scoped per user session
3. **Model lineage** — registry tracking every model, prompt, and configuration version
4. **Policy-as-code** — business rules enforced deterministically, not via prompting
5. **Compliance artifacts** — model cards, system cards, impact assessments prepared before deployment

Start with the audit trail — it's the foundation everything else builds on. If you can't prove what your agent did, you can't govern it.

## References

- [EU AI Act](https://artificialintelligenceact.eu/)
- [AWS Cedar Policy Language](https://www.cedarpolicy.com/)
- [Open Policy Agent](https://www.openpolicyagent.org/)
- [NIST AI Risk Management Framework](https://www.nist.gov/itl/ai-risk-management-framework)
- [MLflow Model Registry](https://mlflow.org/docs/latest/model-registry.html)
- [Google Model Card Toolkit](https://github.com/google/model-card-toolkit)
- [Microsoft Responsible AI Standard v2](https://www.microsoft.com/en-us/ai/responsible-ai)
