---
layout: default
title: LLM Project Checklist
permalink: /llm-project-checklist/
---

# LLM Project Checklist

An ordered checklist for a new project, or for reviewing an existing LLM deployment. It is the
companion artifact to [Sequence Beats Model Choice]({% post_url 2026-09-29-running-an-ai-project-checklist %}),
which explains the reasoning behind the two orderings the checklist depends on: controls are
designed in Phase 4 and built in Phase 6, and the evaluation suite in Phase 5 gates the build.
Vendor names appear as examples and traps, not as requirements.

**How to use**

- Work top to bottom. Phases follow the lifecycle in five stages: Discovery, Planning, Implementation,
  Release, Operate.
- `[If ...]` items apply only under that condition. Skip the rest.
- `[Per use case]` marks a phase, section, or item that repeats for each use case in scope.
- `[DOC]` marks an artifact worth producing. The full list is at the end.
- Nothing is "done" at the gate until the artifact exists and the check passes.
- **Scale to risk.** The document set and control rigour should match the consequence of a wrong
  call. A low-risk internal tool does not need all 36 artifacts, but it does need the alignment
  boundary and controls (Phase 4), the eval suite (Phase 5), and the outcome document (Phase 10).
- **Eval plan starts early, finalizes late.** State success criteria in Phase 1, but complete the
  suite in Phase 5, after safety design, so it includes the control behaviours. Do not write
  production code before the Phase 5 gate.
- **Controls are designed before the path is built.** Phase 4 decides what guards the request path;
  Phase 6 builds it. That is the opposite of bolting guardrails on afterward.

## Contents

**Discovery**
- [Phase 0: Intake](#phase-0-intake)
- [Phase 1: Discovery](#phase-1-discovery)

**Planning**
- [Phase 2: Feasibility and sizing](#phase-2-feasibility-and-sizing)
- [Phase 3: Architecture and pattern design](#phase-3-architecture-and-pattern-design)
- [Phase 4: Safety, controls, and fairness](#phase-4-safety-controls-and-fairness)
- [Phase 5: Evaluation](#phase-5-evaluation)

**Implementation**
- [Phase 6: Integration, reliability, and enterprise readiness](#phase-6-integration-reliability-and-enterprise-readiness)
- [Phase 7: Documentation and handoff](#phase-7-documentation-and-handoff)

**Release**
- [Phase 8: Pre-launch readiness](#phase-8-pre-launch-readiness)
- [Phase 9: Launch](#phase-9-launch)

**Operate**
- [Phase 10: Operate](#phase-10-operate)

**Reference**
- [Review Mode](#review-mode-auditing-an-existing-project)
- [Document set](#document-set-summary)
- [Cross-cutting conditionals](#cross-cutting-conditionals-keep-visible-throughout)
- [Appendix: beyond the source material](#appendix-beyond-the-source-material)

---

## Phase 0: Intake

- [ ] Identify the requester, the sponsor, and the budget owner (may be one person or three).
- [ ] Name the user if known. If not, make "who is the user" the first discovery question.
- [ ] Record the business trigger (why now) and the value hypothesis.
- [ ] Name the candidate business metric that will judge the project (baseline confirmed in Phase 1).
- [ ] Name what "done" means to the sponsor, in business terms.
- [ ] Classify the engagement: net-new build, extension of an existing system, or audit.
- [ ] Confirm partner/account context: partner tier, procurement commitments, cloud agreements.
- [ ] Note the data classes involved: public, internal, confidential, regulated (PHI, PII, financial).
- [ ] List governing obligations early: HIPAA, GDPR, FedRAMP, privilege, data residency, internal policy.
- [ ] Sanity-check the premise: is a deterministic or existing-system solution cheaper, safer, or more auditable? Raise it now if so.
- [ ] `[If regulated]` Identify who in legal/compliance must sign off.
- [ ] `[If regulated]` Identify required entry point and delivery route constraints before any design.
- [ ] List the open questions discovery must answer.
- [ ] `[If audit]` Switch to Review Mode at the bottom of this file.
- [ ] `[DOC]` Project charter or one-page brief.

**Gate:** the charter records the requester, sponsor, budget owner, governing constraint, data classes,
and candidate metric; the engagement is classified; and the premise has been sanity-checked against a
non-LLM solution.

---

## Phase 1: Discovery

### Elicitation

- [ ] Run discovery as listen, translate, write down. Not a design session.
- [ ] Capture the business goal in the stakeholder's own words.
- [ ] Capture any solution the requester arrives with, separately from the problem; treat that sketch as a hypothesis, not the design.
- [ ] Name stakeholders: sponsor, budget owner, users, reviewers, system owners, data owners, legal, security.
- [ ] Identify affected groups and any decision with disparate-impact potential.

### The four question categories [Per use case]

- [ ] What must the system **do**? (capabilities as business outcomes)
- [ ] What must the system **not** do? (boundaries, prohibited actions, human-only cases)
- [ ] What must the system **cost**? (budget, latency target, volume forecast)
- [ ] What must the system **prove**? (evidence, audit, compliance obligations)

### Use-case inventory

- [ ] `[If the brief contains multiple candidate use cases]` Enumerate them, then with the sponsor select and sequence the one in scope. Scope one use case at a time unless two genuinely share an architecture.

### Capability list [Per use case]

- [ ] Break the goal into a **capability list**, naming each capability separately. "Process insurance claims" is a goal; extracting structured fields, looking up policy coverage, routing the claim by type and value, and drafting the adjuster notification are capabilities. This list is what Phase 3 assigns to owners.

### Translate preferences into constraints [Per use case]

- [ ] Flag every experience word: seamless, fast, simple, easy, intuitive.
- [ ] For each, ask what would break it, what the user must never notice, what must be true on failure.
- [ ] Convert each into a testable, bounded constraint.
- [ ] State volume, latency (p95), input size, and cost ceiling explicitly.
- [ ] Capture peak versus average volume and the long-tail input sizes.
- [ ] Characterize the input distribution: document and query types, rare cases, adversarial cases.
- [ ] Do not issue any verdict or timeline before these are gathered.

### Record requirements [Per use case]

- [ ] Source every row from the four question categories, the capability list, and the constraints translated from preferences above.
- [ ] One row per item: statement, implied constraint, required decision, assumption.
- [ ] Trace every requirement to something the stakeholder said.
- [ ] Label anything untraceable as an **assumption** and confirm it.
- [ ] Capture the **before metric** now. It cannot be reconstructed later.
- [ ] Define the metric measurement method and definition, so before and after use the same one.
- [ ] Define the use case scope boundary, listing what is explicitly out of scope.
- [ ] List the known failure modes: what would make an output unacceptable.
- [ ] Capture non-functional requirements: availability, retention, audit, and security posture.
- [ ] **Start the eval plan:** state success criteria (finalized in Phase 5 after safety design).

### Data and systems

- [ ] Inventory existing systems, data sources, and system-of-record owners.
- [ ] Map data sources and sinks; flag cross-border or third-party flows.
- [ ] Identify what is live state versus stable reference material.
- [ ] Record operational constraints: change windows, existing SLAs, integration limits.
- [ ] `[If PII/PHI]` Identify sensitive fields and the minimum necessary set.
- [ ] `[If conversational]` Capture conversation lengths, topic shifts, and follow-up patterns for the multi-turn eval set.

- [ ] `[DOC]` Discovery notes.
- [ ] `[DOC]` Translation table (statement, constraint, decision, assumption).
- [ ] `[DOC]` Assumption register (with owners and resolution criteria).
- [ ] `[DOC]` Baseline metric record.

**Gate:** requirements trace to the business case; assumptions are labeled; the before metric exists; and the Phase 0 open questions are answered or moved into the assumption register.

---

## Phase 2: Feasibility and sizing [Per use case]

### Technical feasibility (the four AI properties)

- [ ] Next-token prediction: does the task need probabilistic generation or precision on specific values?
- [ ] Knowledge: is the needed information rare, contested, recent, or domain-specific?
- [ ] Working memory: do inputs fit, or in aggregate exceed, the context window?
- [ ] Steerability: are instructions specific, concrete, and verifiable?
- [ ] For each, name the compensating control (code, tool call, retrieval, pipeline, verifier).
- [ ] `[If working memory is binding]` Distinguish single-document size from corpus scale.

### Sizing

- [ ] Estimate call volume from the business owner, not intuition.
- [ ] Build the token budget per request as a **distribution**, not an average.
- [ ] Model the model-tier cost (input and output priced separately).
- [ ] `[If long, stable prefix]` Model prompt caching, including write cost and TTL.
- [ ] `[If async SLA permits]` Model the Batch API and check BAA coverage if regulated.
- [ ] Project monthly cost against the ceiling.
- [ ] Include eval and judge call cost, and human-review labor cost, in the model.
- [ ] Model latency at **p95**, not median.
- [ ] Include retrieval and tool-call round trips in the latency budget; validate against the SLA.
- [ ] Run sensitivity analysis: volume doubles, distribution shifts to the tail.

### SLA

- [ ] Define the SLA now, not at launch: what is measured, what counts as a breach, what happens on breach.
- [ ] Trace each threshold to a real source: user-experience expectation, business criticality, or
      eval acceptance criteria.

### Verdict and value

- [ ] State the verdict: feasible as scoped, feasible with constraints, not feasible.
- [ ] `[If not feasible]` Stop before architecture; record which constraint is disqualifying.
- [ ] `[If feasible with constraints]` Document each constraint and the failure mode if violated.
- [ ] Name the **load-bearing boundary condition**.
- [ ] Map value: baseline, projected, run cost, payback period, sensitivity.
- [ ] Confirm the baseline is measured, not estimated, and comes from the owner's operational data.
- [ ] Confirm the projection does not assume full automation where the design requires human review.
- [ ] Map each value claim to a pillar (efficiency, transformation, productivity, cost, SLA).

- [ ] `[DOC]` Feasibility memo and verdict.
- [ ] `[DOC]` Cost and latency model.
- [ ] `[DOC]` ROI / business case with the load-bearing boundary condition.
- [ ] `[DOC]` SLA definition.

**Gate:** cost and latency modeled at production volume; the verdict, the load-bearing boundary condition, and the SLA are documented (the full boundary-condition list is finalized in Phase 3).

---

## Phase 3: Architecture and pattern design [Per use case]

### Decomposition

- [ ] Assign every step to the model, an existing system, or a human.
- [ ] For each step moved off the model, name the reason and the property behind it: a deterministic threshold (steerability and next-token precision), live state (knowledge), or stakes (confidence is not correctness, so a human verifies).
- [ ] Over-assigning steps to the model is the default failure. Check for it.
- [ ] Identify the delegation map: AI-appropriate, human-retained, collaborative.
- [ ] Revisit the boundary conditions now that owners are assigned, and add any ownership-driven constraints (for example, authoritative data must be retrieved, not generated).

### Pattern selection (five factors, first that rules out)

- [ ] Predictability: can the steps be enumerated in advance?
- [ ] Error cost: retry, audit, or lawsuit?
- [ ] Observability: can ops reconstruct what happened?
- [ ] Latency budget.
- [ ] Cost at volume.
- [ ] Choose: augmented LLM, workflow, or agent.
- [ ] `[If workflow]` Choose sub-pattern: chaining, routing, parallelization, evaluator-optimizer.
- [ ] `[If agent]` Justify: path cannot be enumerated AND failure is acceptable and recoverable.
- [ ] `[If agent]` Set constrained tool entry points, turn budgets, permissions, stopping criteria.
- [ ] Do not choose an agent because the task "feels open-ended" or to buy flexibility.

### Reference architecture (the wiring, not a re-pick of the pattern)

- [ ] Pick the reference architecture: agent, RAG, document pipeline, routing, coding agent.
- [ ] Name its known failure mode and the mitigation.
- [ ] `[If composing two patterns]` Justify because the parts break differently; double maintenance.
- [ ] `[If retrieval]` Chunk by corpus structure: fixed, semantic, hierarchical.
- [ ] `[If retrieval]` Index by query pattern: dense, sparse, hybrid; use RRF for hybrid.
- [ ] `[If retrieval]` Confirm the data is stable knowledge, not live state.

### Multi-agent (if orchestrator-workers)

- [ ] Define orchestrator and subagent roles: the orchestrator delegates and synthesizes, it does not do the work.
- [ ] Define how work decomposes, how results are structured for combination, and how conflicts resolve.
- [ ] Confirm the shape is fan-out unless the units genuinely depend on each other.
- [ ] Define recoverable (subagent) versus unrecoverable (orchestrator) failure boundaries.
- [ ] Add a coverage check so results returned equal units dispatched.
- [ ] Propagate a shared trace ID across orchestrator and subagents.
- [ ] Place human gates before irreversible or high-stakes subagent actions; sample the rest.
- [ ] Constrain each subagent's tool set to its task.

### Tools (if the system calls tools)

- [ ] Design the minimal tool set; remove tools the task does not require.
- [ ] Give each tool a narrow scope and typed arguments.
- [ ] Define tool error handling and what the model does when a tool fails.

### Model, context, prompts

- [ ] Make the model-tier decision **per step**, not per system.
- [ ] Start on a mid-tier model; move up or down only on an eval result.
- [ ] `[If swapping models]` Gate the swap with an eval and a pre-set rollback threshold.
- [ ] Choose the context strategy per need: monolithic, progressive, retrieval, compaction.
- [ ] `[If caching]` Order static content before dynamic and set cache breakpoints explicitly.
- [ ] Budget context: largest realistic conversation + retrieved + system + scratch + margin.
- [ ] Define behavior at the window ceiling: an oversized request is rejected before generation, while a generation that hits the ceiling returns truncated output.
- [ ] `[If extended reasoning]` Justify from a measured accuracy gap, not "can't hurt".
- [ ] Design the system prompt: role and scope, constraints, output contract.
- [ ] Enforce guardrails in the output contract, not as prose.
- [ ] Choose prompt technique by complexity: zero-shot, few-shot, chain-of-thought.
- [ ] Check prompt bias: leading phrasing, unbalanced examples, presumed answers.
- [ ] Decide packaging: prompt library, direct tools, or a versioned reusable asset.
- [ ] `[Per use case]` Carry the boundary conditions, the explicit out-of-scope list, and the open assumptions into the statement of work or shared scope document.

- [ ] `[DOC]` Solution architecture document.
- [ ] `[DOC]` Decision log (decision, rejected alternatives, trade-off resolved, owner, date).
- [ ] `[DOC]` Boundary conditions (the full list, finalized here after decomposition; the load-bearing one from Phase 2 is carried in the feasibility memo).
- [ ] `[DOC]` Statement of work / scope document.

**Gate:** every architecture choice names the constraint it resolves and the alternative it rejected.

---

## Phase 4: Safety, controls, and fairness

Design the guarded path here, before it is built. This is the alignment boundary plus every control
that will sit on the request path in Phase 6.

### Alignment boundary

- [ ] Separate what trained behavior covers from what the application layer must enforce.
- [ ] Enforce every deployment-specific rule in a layer you build.
- [ ] Do not assume the model enforces a rule it was never given.

### Guardrails

- [ ] Place **input screening** before the model call.
- [ ] Place **output screening** before the response reaches the user.
- [ ] Place **tool-call authorization** before any side-effecting action.
- [ ] Choose model-based or deterministic per point (authorization is deterministic).
- [ ] Set the failure direction. Safety controls **fail closed** where a wrong pass causes harm.
- [ ] Do not rely on one filter to cover three points. Screening detects content; authorization permits actions.
- [ ] `[If retrieval or tools]` Screen retrieved content and tool outputs before appending to context.
- [ ] `[If refusals handled]` Read the refusal category, route by class, reset context after a refusal.
- [ ] `[If reusable skill or prompt assets are used]` Audit the bundle, run least-privilege in a sandbox, trusted source only,
      record an approve/reject/remediate verdict.

### Risk assessment [Per use case]

- [ ] Identify risks in the five recurring categories: direct injection, indirect injection,
      token-budget exhaustion, tool and action abuse, data exposure.
- [ ] At each entry point (user input, retrieved content, tool outputs, model output, logs), ask what
      an adversary could do and which control blocks it.
- [ ] Record each risk as category, affected component, likelihood x impact, mitigation, owner, evidence.

### Fairness and transparency [Per use case]

- [ ] Instrument all four injection points: corpus, prompt framing, examples, routing.
- [ ] Build decision logging once; serve user, regulator, team, and control register from it.
- [ ] Check the log itself falls under the compliance register (minimization, retention, access).
- [ ] Verify aggregate dashboards have a per-subgroup breakdown.

### Human review [Per use case]

- [ ] Route by confidence, reversibility, and cost of a wrong answer. Not by volume.
- [ ] Place the gate: pre-action, post-action, or sampled.
- [ ] Give the reviewer the inputs, the model output, and the reason it was flagged.
- [ ] `[If agentic]` Avoid per-step approvals; move review to plan-level or exception checkpoints.
- [ ] Sample lower-stakes actions rather than gating each one.
- [ ] Pre-identify the stakes that warrant review.
- [ ] Confirm the confidence signal is calibrated before relying on it.

### Compliance

- [ ] `[If regulated]` Map each obligation to a technical control, an accountable owner, and an evidence artifact.
- [ ] `[If regulated]` Build the constraint-to-integration matrix at the route and configuration level.
- [ ] `[If regulated]` Confirm evidence artifacts will be inspectable (agreement, config screen, authorization record, log query).

- [ ] `[DOC]` Risk assessment (category, component, likelihood x impact, mitigation, owner, evidence;
      this scores the Phase 3 boundary conditions).
- [ ] `[DOC]` Guardrail design and failure directions.
- [ ] `[DOC]` Fairness instrumentation and decision-log design.
- [ ] `[DOC]` Review-routing rule.
- [ ] `[DOC]` `[If regulated]` Control register (obligation, control, owner, evidence artifact).

**Gate:** each control has an owner, a failure direction, and an evidence artifact.

---

## Phase 5: Evaluation [Per use case]

Complete the eval suite here, after the safety design, so it tests the control behaviors as well as
task quality. This phase gates the build.

### Eval suite

- [ ] Define the task in measurable terms with a test prompt.
- [ ] Build a golden dataset: representative, with edge cases and adversarial inputs.
- [ ] Include the safety and control behaviors from Phase 4 as dataset categories.
- [ ] Choose the grading method per behavior: code-based where deterministic, judge where
      interpretive, human for high-stakes or novel.
- [ ] Run automated checks for unambiguous behaviors.
- [ ] Use a judge model for interpretive behaviors, with constrained verdicts and a rubric.
- [ ] Calibrate the judge against human-labeled examples; use a different model than the one judged.
- [ ] Favor broad automatic coverage over a small manual set.
- [ ] Set thresholds from the business requirement, not from the first prototype.
- [ ] Make each named failure mode a dataset category.
- [ ] Interpret per-category and per-subgroup results, not only the overall mean; a rising mean can
      hide a degraded category.
- [ ] `[If conversational]` Build multi-turn evals with their own transcript dataset.
- [ ] Keep the golden dataset current with every system change.

### Gate every change

- [ ] Test set with known-good outputs.
- [ ] Grading function (model-graded or programmatic).
- [ ] Delta threshold **set before** the eval run.
- [ ] Apply the gate to model swaps, prompt revisions, context changes, retrieval changes, control changes.

- [ ] `[DOC]` Eval plan and golden dataset.
- [ ] `[DOC]` Model-change gate record.

**Gate:** the eval suite exists and passes; every change runs through it. No production code before
this gate.

---

## Phase 6: Integration, reliability, and enterprise readiness

Build the guarded path decided in Phase 4, wire it into the enterprise stack, and add the reliability
controls where the route and boundary are now known.

### Delivery-team environment (before any code) [If you use an AI coding assistant]

- [ ] Set the shared baseline: a shared instruction file (for example `CLAUDE.md`), agreed tools and MCP servers, permission posture.
- [ ] Choose the reusable-asset distribution mechanism: org-provisioned, packaged as a plugin, project-scoped, or exposed over the API.
- [ ] `[If the asset needs versioning, targeting, or rollback]` Use an organization-managed plugin
      and identify an owner.
- [ ] Set spend posture: model defaults, allowlists and restrictions, effort guidance, caps. Set it
      before the first bill.
- [ ] Integrate assistance into the existing workflow, not a side tool.
- [ ] Roll out through a champion per team, then batches.

### Entry point and route (compliance first)

- [ ] Let compliance rule routes and entry points in or out **before** cost or preference.
- [ ] Choose the entry point for the user and the work.
- [ ] Choose the build-time interface: API, SDK, MCP, or an agent framework.
- [ ] Use the API or SDK for a single request and response; use an agent framework for multi-turn action inside your own product.
- [ ] `[If embedding in a product]` Do not use an interactive coding assistant as the backend.
- [ ] `[If MCP]` Name the second client that reuses the tools, or do not use MCP.
- [ ] `[If MCP]` Secure the server: authentication, least-privilege scopes, no over-exposed tools.
- [ ] Choose the delivery route by procurement and residency: a direct provider API, or a cloud-hosted route such as Bedrock, Vertex, or Foundry.
- [ ] `[If residency]` Set the region explicitly and verify it at request time. Do not rely on global default endpoint resolution.
- [ ] `[If strict obligation]` Verify the specific obligation is covered by the configuration in use.
- [ ] `[If FedRAMP]` Use a route authorized at the required impact level rather than the general commercial endpoint.
- [ ] `[If EU residency]` Check which geographies the request-level inference-geography parameter actually accepts. Pin the region on a cloud route, and set storage residency as a separate control.
- [ ] `[If multi-entry-point]` Write the entry-point-responsibility map: which route owns which task, and why.
- [ ] Re-verify current model names, pricing, route availability, and cache or batch limits against platform documentation.
- [ ] Document model identifier and feature differences across the chosen routes.
- [ ] Confirm rate limits and quotas per route for expected volume.

### Identity, authorization, data

- [ ] Verify user identity **server-side**, before the model call.
- [ ] Inject role and authorized data from the server, never from the user message.
- [ ] Apply the existing authorization model to the model layer.
- [ ] Implement tool-call authorization deterministically: allowlist, identity, scope.
- [ ] Implement the Phase 4 screens at their placement points.
- [ ] Pass only the **minimum necessary** data into the context window.
- [ ] Redact or strip sensitive fields before the call, not in the prompt.
- [ ] Redact or exclude sensitive fields from **request and response logs**, not just the call.
- [ ] Define retention and deletion for logs, caches, and stored outputs.
- [ ] Treat the context window as data leaving your boundary, not a governance boundary.
- [ ] Do not conflate training-use exclusion with retention; they are separate claims.
- [ ] Store keys and secrets in a managed secret store, never in code, prompts, or logs.
- [ ] `[If multi-tenant]` Use separate API keys per tenant.
- [ ] `[If multi-agent]` Enforce the Phase 3 subagent tool scopes.

### Observability

- [ ] Log the request: model version, input tokens, prompt id.
- [ ] Log the response: output tokens, latency, stop reason.
- [ ] Log the context: user role, session id, caching hit or miss.
- [ ] Log the outcome: downstream acceptance and rejection signals.
- [ ] `[If multi-agent]` Propagate the Phase 3 trace ID across orchestrator and subagents.
- [ ] `[If regulated]` Verify which agentic actions are recorded across all surfaces.

### Reliability

- [ ] Validate the Phase 2 cost and latency model against the built system at production volume.
- [ ] Build retries with exponential backoff, close to the API call.
- [ ] Build a fallback chain in the orchestration layer, using the route choices above.
- [ ] `[If batch]` Implement asynchronous submission and result polling, and confirm regulated data is covered before routing it.
- [ ] Build a circuit breaker at the service boundary.
- [ ] Inventory downstream dependencies and their failure modes.
- [ ] Set budget and rate alerts (for example, cost above 150% of the 7-day average, or p95 crossing the SLA).
- [ ] Name the architecture-specific failure mode and its mitigation.
- [ ] `[If agent]` Eval the stopping behavior, not just output quality.
- [ ] `[If retrieval]` Monitor retrieval precision and recall as system metrics.
- [ ] `[If document pipeline]` Add a confidence score and route low confidence to human review.
- [ ] `[If orchestrator-workers]` Define recoverable vs unrecoverable boundaries; reconcile coverage.
- [ ] `[Per use case]` Configure the per-step model routing decided in Phase 3, and pin each model version; monitor deprecations and keep an update runbook.

- [ ] `[DOC]` Entry-point-responsibility map (if multi-entry-point).
- [ ] `[DOC]` Integration design document.
- [ ] `[DOC]` Data-flow record (where every copy lives, plus retention and deletion).
- [ ] `[DOC]` Constraint-to-integration matrix (if regulated).
- [ ] `[DOC]` Reliability design (retry, fallback, circuit breaker).
- [ ] `[DOC]` Failure-mode and mitigation table (architecture-specific; see Phase 4 for scoring).
- [ ] `[DOC]` Team setup and shared configuration.
- [ ] `[DOC]` Spend policy.

**Gate:** identity, authorization, data handling, observability, and reliability are each owned and
evidenced.

---

## Phase 7: Documentation and handoff

The document serves three readers: the **inheriting engineer**, the **auditor**, and the **returning
architect**. Serving one without the others leaves it incomplete.

- [ ] Record each decision with its date.
- [ ] Record the rejected alternatives and the trade-off each resolved.
- [ ] Label assumptions as assumptions, with owners and resolution criteria.
- [ ] Attach an owner to every decision and control.
- [ ] Attach an evidence artifact to every control.
- [ ] Apply the completeness test: can an architect who was not in the room make a safe change?
- [ ] Mark the load-bearing decisions explicitly.
- [ ] Carry the control register forward into the living document.
- [ ] Write the runbook of symptom-to-cause-to-action paths, each entry naming the symptom, the cause, and the first action.
- [ ] Store the documentation in version control alongside the code.
- [ ] Version the prompts and reusable assets alongside the code and docs.
- [ ] Define the escalation path.
- [ ] Confirm the statement of work produced in Phase 3 still reflects the final scope.
- [ ] `[If multi-entry-point]` Include the entry-point-responsibility map.

- [ ] `[DOC]` Architecture document with rationale (the living document; carries the decision log and control register forward).
- [ ] `[DOC]` Runbook.
- [ ] `[DOC]` Escalation path.

**Gate:** completeness test passes; no load-bearing decision is undocumented.

---

## Phase 8: Pre-launch readiness

Everything that must exist before launch: the team's readiness to run AI-assisted work, the evidence
that AI-assisted work is trustworthy, and the monitoring that decides what happens when a signal
moves. All of it ships with the launch, not after it.

### Team readiness at launch (delivery team) [If the delivery team uses AI-assisted development]

- [ ] Define the review discipline per workflow stage: writing, reviewing, debugging.
- [ ] Build the verification checklist: correctness, security, maintainability, human understanding.
- [ ] Automate every check that can be automated (tests, evals).
- [ ] Watch for lumpy adoption and stalling at basic chat.

### Monitoring readiness

- [ ] Build the governance table: signal, trigger, action, owner, regulated checkpoint.
- [ ] Include slow drifts as well as hard failures.
- [ ] `[If regulated]` Wire scheduled checkpoints to the calendar, independent of any metric.
- [ ] Wire the Phase 2 SLA thresholds to review triggers and owners.
- [ ] Build the translation layer from technical metrics to business metrics.
- [ ] Confirm the escalation path and owners are published and on call.
- [ ] Document the rollback plan for model, prompt, retrieval, and configuration changes.
- [ ] Provision access: SSO groups, roles, least privilege.
- [ ] Raise rate limits and quotas for expected launch volume.
- [ ] Dry-run the runbook and incident response.
- [ ] Confirm no open assumption blocks launch, or record it as an accepted risk.
- [ ] Revalidate the control register before launch.

- [ ] `[DOC]` Verification checklist.
- [ ] `[DOC]` Governance table.
- [ ] `[DOC]` Rollback plan.

**Gate:** the governance table, SLA wiring, owners, and verification checklist all exist before launch.

---

## Phase 9: Launch

- [ ] Confirm every gate through Phase 8 has passed.
- [ ] Confirm the controls are live and fail closed.
- [ ] Confirm logging, trace IDs, and caching behavior are live.
- [ ] Confirm the fallback and circuit-breaker paths work under load.
- [ ] Confirm the rollback path has been tested.
- [ ] `[If risk warrants]` Launch to a canary or limited subset before full traffic.
- [ ] Confirm the model version is pinned and recorded.
- [ ] Confirm monitoring owners and the escalation path are active.
- [ ] Record the launch date, model version, and configuration.

**Gate:** the system is live with controls, logging, and owners in place.

---

## Phase 10: Operate

Run the feedback loop, run experiments, and complete the outcome document.

### Monitor

- [ ] Operate the governance table: triage signals, act, review whether the rule needs to change.
- [ ] Diagnose drift by cause: prompt failure, hallucination, model mismatch; distinguish model drift, data drift, and model-update effects.
- [ ] Keep the decision log current.
- [ ] Compare output distributions periodically to catch slow drift that thresholds miss.
- [ ] `[If regulated]` Run scheduled audits on the calendar, regardless of metrics.
- [ ] Watch model deprecations and re-validate before a forced migration.
- [ ] Review cost against the budget model on a cadence.
- [ ] Revalidate the control register on a cadence.

### Experiment (if optimizing a live system)

- [ ] Write a specific, falsifiable hypothesis naming treatment, metric, and threshold.
- [ ] Define the primary metric before the run.
- [ ] Randomize treatment and control; keep assignment consistent per user or session.
- [ ] Compute sample size from effect size, baseline, and confidence.
- [ ] Control the input distribution across groups.
- [ ] `[If a single bad output is too risky, or traffic is low]` Use shadow testing.
- [ ] `[If shadow testing]` Accept the loss of downstream signal; score offline against a rubric.
- [ ] Check secondary metrics did not degrade.
- [ ] Check for treatment x input-type interaction effects.
- [ ] Do not declare a winner on significance alone; require materiality.

### Outcome [Per use case]

- [ ] `[If measurable now]` Complete the outcome document: six fields.
- [ ] `[If the after metric needs runtime]` Name the measurement owner, confirm logging is live, set
      the completion milestone. Do not claim the gate is satisfied.
- [ ] Judge the phase gate before moving to expansion or handoff.

- [ ] `[DOC]` Experiment design and result record.
- [ ] `[DOC]` Outcome document.

**Gate:** monitoring has triggers and owners; the outcome document is either complete or has a named
owner and completion milestone.

---

## Review Mode: auditing an existing project

Run the same phases as an audit. For each, ask "does this exist, is it current, and who owns it?"

- [ ] *(Phase 4)* Is the alignment boundary correct, or are deployment rules assumed to be model-enforced?
- [ ] *(Phase 4)* Are all three control points present, and do they fail closed?
- [ ] *(Phase 3)* Is retrieval being used for live state?
- [ ] *(Phase 4)* Is there one output filter pretending to cover three points?
- [ ] *(Phase 4)* Do fairness injection points have instrumentation?
- [ ] *(Phase 4)* Is review routed by stakes, or by volume?
- [ ] *(Phase 7)* Does every control have an owner and a live evidence artifact?
- [ ] *(Phase 5)* Is the eval dataset representative and current with the last change?
- [ ] *(Phase 2)* Is the cost model based on a distribution or an average?
- [ ] *(Phase 3)* Is the model tier decided per step, or defaulted?
- [ ] *(Phase 6)* Are model versions pinned, with a re-validation gate?
- [ ] *(Phase 8)* Do observability metrics have triggers, or just dashboards?
- [ ] *(Phase 7)* Can a successor make a safe change from the documentation alone?
- [ ] *(Phase 10)* Does the outcome document carry a before metric and an auditable control?
- [ ] *(Phase 8)* Is any shared asset distributed without versioning or rollback?
- [ ] *(Phase 6)* Are multi-tenant keys, trace IDs, and coverage checks in place where applicable?

---

## Document set (summary)

The risk picture appears three times by design: **boundary conditions (#12)** list the constraints,
the **risk assessment (#13)** scores likelihood and impact, and the **failure-mode table (#25)** maps
architecture-specific failures. Produce them in that order, or merge them into one register if the
project is small. Boundary conditions are finalized in Phase 3, after owner assignment; the load-bearing
condition identified in Phase 2 travels in the feasibility memo.

Each row is one file to produce, numbered to match. A scaffold per document is useful, and this page
lists them so you can see the shape before you write one.

| # | Document | Phase | Required when |
|---|----------|-------|---------------|
| 1 | Project charter / one-page brief | 0 | always |
| 2 | Discovery notes | 1 | always |
| 3 | Translation table | 1 | always |
| 4 | Assumption register | 1 | always |
| 5 | Baseline metric record | 1 | always |
| 6 | Feasibility memo | 2 | always |
| 7 | Cost and latency model | 2 | always |
| 8 | ROI / business case | 2 | always (needed for expansion) |
| 9 | SLA definition | 2 | always |
| 10 | Solution architecture document | 3 | always |
| 11 | Decision log | 3 | always |
| 12 | Boundary conditions | 3 | if feasible with constraints |
| 29 | Statement of work / scope document | 3 | always |
| 13 | Risk assessment | 4 | always |
| 14 | Guardrail design and failure directions | 4 | always |
| 15 | Fairness instrumentation and decision-log design | 4 | always |
| 16 | Review-routing rule | 4 | if human review exists |
| 17 | Control register | 4 | if regulated |
| 18 | Eval plan and golden dataset | 5 | always |
| 19 | Model-change gate record | 5 | always |
| 20 | Entry-point-responsibility map | 6 | if multi-entry-point |
| 21 | Integration design document | 6 | always |
| 22 | Data-flow record | 6 | if residency or PHI |
| 23 | Constraint-to-integration matrix | 6 | if regulated |
| 24 | Reliability design | 6 | always |
| 25 | Failure-mode and mitigation table | 6 | always |
| 30 | Team setup and shared configuration | 6 | if you use an AI coding assistant |
| 32 | Spend policy | 6 | if you use an AI coding assistant |
| 26 | Architecture document with rationale | 7 | always |
| 27 | Runbook | 7 | always |
| 28 | Escalation path | 7 | always |
| 31 | Verification checklist | 8 | always |
| 33 | Governance table | 8 | always |
| 34 | Rollback plan | 8 | always |
| 35 | Experiment design and result record | 10 | if optimizing |
| 36 | Outcome document | 10 | always (may be deferred to a milestone) |

---

## Cross-cutting conditionals (keep visible throughout)

| Condition | What changes |
|-----------|--------------|
| **Regulated** (HIPAA, GDPR, FedRAMP, privilege) | Compliance rules routes first; control register; scheduled checkpoints; BAA/DPA per configuration |
| **PHI or PII** | Minimum-necessary data; redaction before the call and before logging; log scope; retention controls |
| **Data residency** | Explicit region pinning at the integration layer; verify logs, caches, monitoring, retention |
| **Retrieval** | Screen retrieved content; monitor precision and recall; never use for live state |
| **Agentic** | Tool budgets, stopping criteria, gate placement, coverage reconciliation, no per-step approvals |
| **Multi-agent** | Shared trace ID; recoverable vs unrecoverable boundaries; coverage check at synthesis |
| **Multi-tenant** | Separate API keys; per-tenant attribution and isolation |
| **Multi-entry-point / multi-platform** | Entry-point-responsibility map; explicit region; model-id and feature-lag differences |
| **High-consequence action** | Deterministic authorization before the action; human gate before irreversible steps |
| **Conversational** | Multi-turn evals with a transcript golden dataset |

---

## Appendix: beyond the source material

A few items in this checklist are standard delivery practice rather than framework guidance. They are
included because the checklist runs real projects, but the source material does not require them.

**Operational delivery**

- Change windows and existing SLAs, peak versus average volume, and cross-border data flows (Phase 1)
- Non-functional requirements beyond latency and cost (Phase 1)
- Rate limits and quotas per route, downstream dependency inventory, and a managed secret store (Phase 6)
- Per-step model routing configuration (Phase 6)
- Rollback plan, access provisioning, quota raising, and a runbook dry run (Phase 8)
- Canary launch and rollback testing (Phase 9)
- Cost review cadence (Phase 10)

**Artifact hygiene**

- Version control for documentation, prompts, and reusable assets (Phase 7)
- Statement of work as a tracked document (Phase 3)
- Assumption closure before launch (Phase 8)

Everything else traces to the source material. The core is the safety controls (Phase 4), the
evaluation suite (Phase 5), and the per-use-case scoping chain in Phases 1 and 2.
