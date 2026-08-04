---
layout: post
title: "Azure AI Foundry Workflows: Building a Multi-Agent Insurance Claims Demo"
date: 2026-08-04
tags: [azure, ai-foundry, multi-agent, workflows, llm, mcp, hitl]
categories: azure
---

In late April 2026 I started building a demo to show a non-technical audience what a real multi-agent business process looks like: an insurance claims pipeline where four AI agents take a claim from first report to final payout. A claimant describes an accident in plain English. The system extracts the facts, verifies the policy, runs fraud checks, routes the claim to a human underwriter when it is risky or expensive, calculates the settlement, and writes the decision letter. All of it orchestrated in the Azure AI Foundry portal with clicks, not code.

By mid-May the workflow ran end to end across three demo scenarios, and a couple of months later Microsoft announced that Foundry Workflows will be retired on December 1, 2026, with [Microsoft Agent Framework](https://learn.microsoft.com/en-us/azure/foundry/agents/concepts/workflow) as the recommended path. The orchestration ideas survive the platform. Some of the debugging habits do too.

---

## The Demo

The scenario is an insurance claims pipeline built entirely in the Foundry UI, with no custom orchestration code. The pitch to the business: speed for routine claims, automated fraud screening, and human review for the claims that need it.

```
Intake Agent
    ↓
Policy Agent  (verify coverage, fail fast)
    ↓
Fraud Agent  (cross-check signals)
    ↓
if / else
├─ fraudScore > 30  ──────→ Ask a question (Human Review) → Settlement Agent
├─ claimValue > $5,000  ──→ Ask a question (Human Review) → Settlement Agent
└─ else ───────────────────→ Settlement Agent (auto)
    ↓
Send message (Decision letter + payout)
```

Four prompt agents, each with a single job and its own tools:

| Agent | Job | Tools | Model |
|-------|-----|-------|-------|
| Intake Agent | Extract structured claim data from free text | None | gpt-4.1-mini |
| Policy Agent | Verify coverage, limits, deductibles | File Search (policy docs), MCP (Claims System) | gpt-4.1-mini |
| Fraud Agent | Cross-check claimant history, provider validity, incident corroboration | MCP (Claims System), OpenAPI (Providers), Web Search, File Search (fraud rules) | gpt-4o |
| Settlement Agent | Calculate payout, generate decision letter | Code Interpreter | gpt-4.1-mini |

The Claims Management System is an MCP server and the Provider Verification API an OpenAPI service. Both run from a single FastAPI + FastMCP container on Azure Container Apps, protected by a bearer token wired into Foundry as a Custom Keys connection. Wiring a real containerized backend into Foundry's tool catalog worked well.

The workflow is a Sequential Workflow with roughly fifteen nodes: Invoke agent, Send message, Set variable, Invoke agent, and so on. An if/else node holds two conditional branches, each containing an **Ask a question** node that pauses for the underwriter's approve or deny decision. All branches converge on the Settlement Agent. Auto-approval happens when `fraudScore <= 30` and `claimValue <= $5,000`.

### The key concept: data flows between nodes as text

Foundry agents receive text, not structured JSON. Each agent outputs JSON against a configured schema, but the next agent cannot read those fields directly. Between every agent node you have to insert a **Set variable** node that formats the previous agent's output into a text string:

```text
"CLAIM TO INVESTIGATE" & Char(10) &
"- Claim type: " & Local.IntakeResult.claimType & Char(10) &
"- Policy number: " & Local.IntakeResult.policyNumber & Char(10) &
"- Estimated amount: " & Text(Local.IntakeResult.estimatedAmount) & Char(10) &
"- Full description: " & Local.IntakeResult.extractedContext
```

Power Fx concatenation with `Char(10)` for line breaks. Every hop in the pipeline is a serialization step. This shapes the design: agent outputs become strings that become inputs, and a field that is not in the schema does not exist downstream.

---

## The Debugging Journey

The workflow took roughly five weeks to get from first node to a reliable end-to-end run. That is calendar time, not effort: this was a side project, built in spare hours during the week. The agent prompts were easy to write. The platform was in preview, and preview meant a steady stream of failures, most of them vague. I kept a detailed log of every issue, and patterns emerged.

### Part 1: The silent data loss (the one that cost a week)

The first serious blocker: the Policy Agent worked perfectly in isolation. It returned complete structured JSON when I tested it in the Playground. Inside the workflow, its output was never stored in the workflow variable. `Local.PolicyAgentResult` came back empty. Every downstream node that referenced it printed blank text: "Policy - -. Checking for fraud signals..."

The Intake Agent, which had no tools, worked fine. The Policy Agent used File Search and MCP. The obvious hypothesis was that tools were breaking the output path. I chased that theory through a week of variations: different tool combinations, different conversation handling, recreated workflows from scratch.

What eventually worked: simplify the JSON schema and see if it still fails. The suggestion came from Microsoft support, and it turned the week-long mystery into a systematic test.

| Schema size | Tools | Result |
|-------------|-------|--------|
| 3 top-level fields | MCP + File Search | variable captured |
| 8 fields | none | variable captured |
| 8 fields | MCP + File Search | variable captured |
| 10 fields | MCP + File Search | variable captured |
| 11 fields | MCP + File Search | variable NOT captured |

At 11 or more top-level fields in the output JSON schema, the workflow silently dropped the agent's output variable. No error, no warning. The node showed a green checkmark, the agent's log showed a complete response, and the variable was empty. My original Policy Agent schema had 16 top-level fields. That is why it failed in the workflow while working fine standalone.

The workaround was blunt but effective: shrink every agent schema to at most 10 top-level fields. The Policy Agent went from 16 fields to 6 by merging related fields into composite strings (`coverageDetails`, `fraudSignals`). The Intake Agent went to 8. The Settlement Agent sits at exactly 10. It is a constraint I would rather not have had, but it forced a useful discipline: small, flat schemas are easier to reason about.

Two weeks later I hit the second silent failure. The Fraud Agent returned `fraudScore: 45`; the workflow logged "SUSPICIOUS" and "manual_review" in its output. The routing condition `Local.FraudScore > 30` evaluated as false, sending a high-risk claim down the auto-approval path. The score in the variable was 0. The agent was right; the variable was not. I never pinned that one to a single root cause (mapping path, type coercion from string to number, and parsing fallback are all candidates). The lesson stuck: when routing depends on a number extracted from agent output, verify what actually landed in the variable before trusting the branch.

### Part 2: The workflow engine's rough edges

Before the claims demo, I had built an earlier workflow (a demo-catalog builder with industry-specific agents), and it introduced me to the platform's general instability. A representative sample from the issue log:

| Issue | Symptom | Notes |
|-------|---------|-------|
| Builder page load error | "Sorry, it looks like the page you're trying to access isn't available right now" | Workflow canvas inaccessible; client error ID, no recovery |
| Hidden UI control | A control rendered with `z-index: -2`, behind the canvas background | Found via browser dev tools; invisible and unclickable |
| 500 on "Ask a question" | Workflow failed with "Request failed with status code 500" on the third question node | Two separate workflow versions, identical failure |
| Preview unavailable | "Service temporarily unavailable. Please try again later." | Failed before any node ran |
| Infinite loop | Agent re-executed with identical output; "Go to node" created a backward loop | `Local.UserFeedback = true` branch never exited |
| Empty trace details | "Input + Output" panel blank for a completed agent invocation | Workflow succeeded, but nothing to inspect |

None of these were mine to fix, and none of them told me what to do. The recurring pattern: failures that gave me a status code or a generic message and no actionable detail. The only reliable debugging tool was the conversation and invocation IDs. Capturing those on every failure let me open support threads with something concrete.

The claims workflow added two more variations of the same problem:

- **Hanging agents.** Nodes that spun forever with a loading indicator, no output, no error, no timeout. Same agent, same input, worked in the Playground. The workflow invocation path and the Playground path were not equivalent.
- **"Error network error."** That string, shown in a red banner at a failing agent node. A network error somewhere between the workflow runtime and the agent. The agent worked in isolation; the workflow context did not.

Both taught me the same workflow-debugging habit: test every agent in isolation before wiring it into the workflow, and treat an agent that works standalone but fails in the workflow as a workflow-context problem, not an agent problem. That heuristic saved days.

### Part 3: Configuration and integration traps

These were preview-platform gotchas, but each left a lesson that transfers:

- **Agent with no model.** A published agent can lose its model assignment. The dropdown is empty and the agent is dead. Check the model on every agent you are about to demo. Twice.
- **`reasoning.effort` on a non-reasoning model.** The workflow failed with `Unsupported parameter: 'reasoning.effort' is not supported with this model` when the Intake Agent was pointing at gpt-4o. Some configuration had set a parameter the model could not take. Validate model-parameter compatibility explicitly.
- **Guardrails blocking legitimate claims.** A routine auto-collision claim was blocked by Foundry's safety controls: "This interaction was blocked by a safety and security control in this asset's Foundry guardrail." Financial amounts and policy numbers triggered a false positive. The guardrail profile needed domain tuning, not a default.
- **"No tool output found for remote function call."** The Settlement Agent's tool call returned nothing and the workflow died with `invalid_request_error`. The tool worked when tested directly; the invocation path through the agent did not return the result. Same class of failure as the hanging agents: tool behavior in isolation differs from tool behavior under orchestration.

The one that took three container revisions to find: the MCP endpoint returned HTTP 421 "Invalid Host header" from Azure Container Apps. The REST endpoints worked; the MCP endpoint did not. The root cause was the SDK default:

```python
# mcp.server.fastmcp.server.py, lines 178-183
if transport_security is None and host in ("127.0.0.1", "localhost", "::1"):
    transport_security = TransportSecuritySettings(
        enable_dns_rebinding_protection=True,
        allowed_hosts=["127.0.0.1:*", "localhost:*", "[::1]:*"],
    )
```

`FastMCP()` defaults to `host="127.0.0.1"`, which auto-enables DNS rebinding protection locked to localhost. The ACA ingress rewrites the Host header to the container's FQDN, which matches none of the allowed patterns, so the server returns 421. The fix was one line:

```python
mcp = FastMCP("ClaimsSystem", host="0.0.0.0")
```

When the host is not a localhost address, the protection defaults off and the FQDN passes. The lesson: "it worked locally" and "it works behind a reverse proxy" are different problems, and DNS rebinding protection is worth checking whenever a service runs behind any ingress.

### Part 4: Model behavior quirks

Two failures looked like platform bugs and turned out to be model behavior:

- **Repetitive output.** The Fraud Agent "completed" and returned `{"policy_number":"AUTO-2023-8842"}` three times in a row. No verdict, no score, no reasoning, just echoed input wrapped in JSON. The workflow showed success with useless output. A green checkmark on an agent node means it ran, not that it produced what you asked for. Validate output completeness somewhere, or the workflow will look healthy while doing nothing.
- **The newline loop.** The Settlement Agent generated a good decision letter, then continued with hundreds of `\n\n` sequences that filled the output panel with blank lines. The model hit a degenerate repetition pattern after its natural stopping point. Stop sequences, tighter max tokens, or post-processing would fix it, but in a visual workflow builder you cannot easily add a sanitizing step mid-pipeline.

---

## What I'd Do Differently

- **Write validation into the workflow.** In code I would assert on the agent output before routing on it. In the visual builder, a node that checked the score variable is a number would have caught the 45-vs-0 failure immediately. I did not build one because the builder did not make it obvious, and the cost of not having it was higher.
- **Bisect schemas earlier.** The 10-field limit took a week to find because I was testing the wrong hypothesis (tools, not schema). Simplify-and-retest is a general technique: when something fails silently, reduce it to the smallest failing case first, not the most plausible cause.
- **Keep a failure ledger.** The issue log with conversation IDs, screenshots, and exact node names was the single most useful artifact. It became a reproducible test matrix I could hand to support. Do this on every preview-platform project.
- **Assume the builder has no auto-save.** Save after every change. The workflow tool does not auto-save, and rebuilding a lost canvas state is painful.

---

## The Retirement, and What Transfers

On December 1, 2026, Foundry Workflows is being retired. Microsoft's guidance is to build new workflows with Microsoft Agent Framework and to follow the migration guide for existing ones. I am not going to argue with the decision. Five weeks of spare-time work against preview-grade issues is a fair picture of the platform. The visual path is not gone: Microsoft's migration guide points to Azure Logic Apps for low-code orchestration, with Foundry agents as callable steps in the canvas.

But the work does not die with the platform. Everything this demo taught me transfers:

- **Multi-agent pipelines are a product decision, not a code decision.** Intake, verify, assess, route, settle is an architecture you can draw on a whiteboard and implement in any framework. The demo showed the value of specialization: four small agents with focused tools beat one agent doing everything.
- **Human-in-the-loop is the feature.** Routing on risk and value, pausing for a human decision, and continuing the pipeline with that decision is the pattern that resonated most with a business audience. "The AI does the research and presents the evidence; you make the call" works on any platform.
- **Data flow is the design constraint.** Agents exchanging text instead of structured objects means every hop needs an explicit serialization step. That holds in Microsoft Agent Framework too, and the discipline of small, flat, explicit schemas carries straight over.
- **Silent failure is the enemy.** On preview platforms, a green checkmark proves execution, not correctness. Verify what is in the variables before you trust the branches.

If I were to rebuild this today, I would build the same pipeline in Microsoft Agent Framework, keep the same agents and mock backends, and encode the routing logic and HITL checkpoints in code instead of clicking nodes together. It would be more testable, more debuggable, and after this project I would trust it more.

The platform got retired, and the five weeks of issues above show why. The claims pipeline, the three demo scenarios, and the habit of verifying every variable before trusting a branch are staying with me.

## References

- [Azure AI Foundry workflows: concept overview](https://learn.microsoft.com/en-us/azure/foundry/agents/concepts/workflow)
- [Migration guide](https://learn.microsoft.com/en-us/azure/foundry/agents/concepts/workflow#migration-guide)
- [Option 1: Microsoft Agent Framework (recommended)](https://learn.microsoft.com/en-us/azure/foundry/agents/concepts/workflow#option-1-microsoft-agent-framework-recommended)
- [Option 2: Azure Logic Apps](https://learn.microsoft.com/en-us/azure/foundry/agents/concepts/workflow#option-2-azure-logic-apps)
- [Option 3: Connect agents directly with A2A](https://learn.microsoft.com/en-us/azure/foundry/agents/concepts/workflow#option-3-connect-agents-directly-with-a2a)
