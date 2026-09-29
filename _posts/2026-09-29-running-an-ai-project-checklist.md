---
layout: post
title: "Sequence Beats Model Choice: A Checklist for Running an AI Project"
date: 2026-09-29
categories: ai
tags: [ai, llm, architecture, project-delivery, evaluation, guardrails, governance]
---

A **gated checklist** for running an AI project end to end: 11 phases in five lifecycle stages, each phase ending in a **gate** with an explicit pass condition. This checklist has 333 items, 56 carrying an `[If ...]` condition, and 36 artifacts. The phases and the artifact list are the least interesting part. What matters is the sequence, because sequencing mistakes are free to fix on day one and a rewrite on day forty.

AI projects rarely fail at the model. They fail at the order things get built. Two orderings carry most of the value: **controls are designed before the request path is built**, and **the eval suite gates the build**. Everything else in the checklist is detail hanging off those two, plus a scaling rule: match the artifact set to the consequence of a wrong call, and to how many people are actually involved.

{::nomarkdown}
<figure>
  <svg xmlns="http://www.w3.org/2000/svg" viewBox="24 0 916 620" role="img" style="width:100%;height:auto" font-family="-apple-system,BlinkMacSystemFont,'Segoe UI',Roboto,Helvetica,Arial,sans-serif">
    <title>AI project lifecycle stages and the two ordering decisions</title>
    <desc>Five lifecycle stages run top to bottom: Discovery, Planning, Implementation, Release, Operate. At the Planning to Implementation boundary, two focal annotations mark the ordering rules: controls are designed in Phase 4 and built in Phase 6, and no production code passes the Phase 5 eval gate. A dashed line carries the outcome metric from Operate back to the baseline captured in Discovery.</desc>
    <defs>
      <pattern id="dots" width="22" height="22" patternUnits="userSpaceOnUse">
        <circle cx="1" cy="1" r="0.9" fill="#E3E2DC"/>
      </pattern>
    </defs>
    <rect width="964" height="620" fill="#f5f4ed"/>
    <rect width="964" height="620" fill="url(#dots)" opacity="0.55"/>

    <text x="80" y="40" fill="#1B365D" font-size="14" font-weight="600" font-family="'JetBrains Mono','SF Mono','Fira Code',Consolas,Monaco,monospace" letter-spacing="3">FIGURE  1</text>
    <text x="200" y="40" fill="#504e49" font-size="14" font-family="'JetBrains Mono','SF Mono','Fira Code',Consolas,Monaco,monospace" letter-spacing="3">LIFECYCLE STAGES AND THE TWO ORDERINGS</text>
    <line x1="80" y1="52" x2="884" y2="52" stroke="#1B365D" stroke-width="0.8"/>

    <!-- Stage nodes -->
    <rect x="80" y="100" width="160" height="64" rx="6" fill="#faf9f5" stroke="#141413" stroke-width="1.5"/>
    <text x="160" y="130" text-anchor="middle" font-size="18" font-weight="600" fill="#141413">Discovery</text>
    <text x="160" y="150" text-anchor="middle" font-size="14" fill="#6b6a64">who, why, metric</text>

    <rect x="80" y="196" width="160" height="64" rx="6" fill="#faf9f5" stroke="#141413" stroke-width="1.5"/>
    <text x="160" y="226" text-anchor="middle" font-size="18" font-weight="600" fill="#141413">Planning</text>
    <text x="160" y="246" text-anchor="middle" font-size="14" fill="#6b6a64">cost, design, evals</text>

    <rect x="80" y="292" width="160" height="64" rx="6" fill="#faf9f5" stroke="#141413" stroke-width="1.5"/>
    <text x="160" y="322" text-anchor="middle" font-size="18" font-weight="600" fill="#141413">Implementation</text>
    <text x="160" y="342" text-anchor="middle" font-size="14" fill="#6b6a64">wire and document</text>

    <rect x="80" y="388" width="160" height="64" rx="6" fill="#faf9f5" stroke="#141413" stroke-width="1.5"/>
    <text x="160" y="418" text-anchor="middle" font-size="18" font-weight="600" fill="#141413">Release</text>
    <text x="160" y="438" text-anchor="middle" font-size="14" fill="#6b6a64">verify and roll back</text>

    <rect x="80" y="484" width="160" height="64" rx="6" fill="#faf9f5" stroke="#141413" stroke-width="1.5"/>
    <text x="160" y="514" text-anchor="middle" font-size="18" font-weight="600" fill="#141413">Operate</text>
    <text x="160" y="534" text-anchor="middle" font-size="14" fill="#6b6a64">watch and measure</text>

    <!-- Main path -->
    <line x1="160" y1="164" x2="160" y2="192" stroke="#1B365D" stroke-width="1"/>
    <path d="M154 188 L160 196 L166 188" fill="none" stroke="#1B365D" stroke-width="1.5" stroke-linecap="round"/>
    <line x1="160" y1="260" x2="160" y2="288" stroke="#1B365D" stroke-width="1"/>
    <path d="M154 284 L160 292 L166 284" fill="none" stroke="#1B365D" stroke-width="1.5" stroke-linecap="round"/>
    <line x1="160" y1="356" x2="160" y2="384" stroke="#1B365D" stroke-width="1"/>
    <path d="M154 380 L160 388 L166 380" fill="none" stroke="#1B365D" stroke-width="1.5" stroke-linecap="round"/>
    <line x1="160" y1="452" x2="160" y2="480" stroke="#1B365D" stroke-width="1"/>
    <path d="M154 476 L160 484 L166 476" fill="none" stroke="#1B365D" stroke-width="1.5" stroke-linecap="round"/>

    <!-- Focal annotations, aligned with the row each one qualifies -->
    <rect x="560" y="196" width="160" height="64" rx="6" fill="#EEF2F7" stroke="#1B365D" stroke-width="1.5"/>
    <text x="640" y="226" text-anchor="middle" font-size="18" font-weight="600" fill="#141413">Controls first</text>
    <text x="640" y="246" text-anchor="middle" font-size="14" fill="#6b6a64">design 4, build 6</text>

    <rect x="560" y="292" width="160" height="64" rx="6" fill="#EEF2F7" stroke="#1B365D" stroke-width="1.5"/>
    <text x="640" y="322" text-anchor="middle" font-size="18" font-weight="600" fill="#141413">Eval gates build</text>
    <text x="640" y="342" text-anchor="middle" font-size="14" fill="#6b6a64">no code past 5</text>

    <line x1="240" y1="228" x2="560" y2="228" stroke="#504e49" stroke-width="1"/>
    <line x1="240" y1="324" x2="560" y2="324" stroke="#504e49" stroke-width="1"/>

    <!-- Outcome loop -->
    <path d="M240 516 L904 516 L904 132 L240 132" fill="none" stroke="#504e49" stroke-width="1" stroke-dasharray="6 4"/>
    <path d="M246 126 L240 132 L246 138" fill="none" stroke="#504e49" stroke-width="1.5" stroke-linecap="round"/>
    <rect x="740" y="100" width="140" height="20" fill="#f5f4ed"/>
    <text x="746" y="116" font-size="14" font-family="'JetBrains Mono','SF Mono','Fira Code',Consolas,Monaco,monospace" fill="#6b6a64">before metric reused</text>

    <text x="80" y="588" font-size="14" font-family="'JetBrains Mono','SF Mono','Fira Code',Consolas,Monaco,monospace" fill="#6b6a64">11 PHASES · 11 GATES · 36 ARTIFACTS</text>
  </svg>
</figure>
{:/nomarkdown}

**How to read it**: the left column is the order you actually do the work, and the two ink-blue boxes are the only decisions in the figure that are expensive to reverse. Both of them govern the same transition, Planning into Implementation, which is the point: by the time you reach Implementation, the shape of the guarded path and the definition of "passing" are already fixed. The dashed line is the third thing people skip, the outcome metric returning to the baseline you captured in the first stage. Skip it and you finish the project with no way to say what it changed.

## The spine

Five stages, 11 phases, 11 gates. Nothing is done because work happened; it is done when the artifact exists and the check passes.

| Stage | Phases | Gate |
|---|---|---|
| Discovery | 0 Intake, 1 Discovery | Requirements trace to the business case; assumptions are labeled; the before metric exists |
| Planning | 2 Feasibility and sizing, 3 Architecture, 4 Safety and controls, 5 Evaluation | Cost and latency modeled at production volume; every control has an owner, a failure direction, and an evidence artifact; the eval suite exists and passes |
| Implementation | 6 Integration and reliability, 7 Documentation and handoff | Identity, authorization, data handling, observability, and reliability are each owned and evidenced; an architect who was not in the room can make a safe change |
| Release | 8 Pre-launch readiness, 9 Launch | The governance table, SLA wiring, owners, and verification checklist exist before launch; controls are live and fail closed |
| Operate | 10 Operate | Monitoring has triggers and owners; the outcome document is complete or has a named owner and a completion milestone |

Planning carries four of the eleven phases and the two orderings, which is why it is where projects are won or lost. Phase 4 designs the guarded path. Phase 6 builds it. Phase 5 decides whether the build is allowed to start at all.

## The catch: guardrails designed after the path exists

A control added to a request handler that already exists is not a guardrail, it is a rewrite of the handler. Design the three control points first, then place them:

```python
def handle(request):
    # 1. input screening, before the model call
    if not screen_input(request.text):
        return refuse(request)

    # identity from the server, never from the message
    user = verify_identity_server_side(request)
    result = call_model(prompt=build_prompt(user), messages=request.text)

    # 2. tool-call authorization, deterministic, before the side effect
    for call in result.tool_calls:
        if not is_authorized(user.role, call.name, call.args):
            return refuse(request)          # fail closed

    # 3. output screening, before the user sees anything
    if not screen_output(result.text):
        return refuse(request)
    return result.text
```

The most common version of this mistake is one output filter doing the job of three controls. Screening detects content; authorization permits actions. They are not interchangeable, and a filter that returns a boolean cannot tell you whether a specific user was allowed to call a specific tool with specific arguments. The other half is the failure direction: where a wrong pass causes harm, the control **fails closed**, and a timeout is a failure, not a pass.

## Another gotcha: an eval gate with a threshold chosen after the run

An **eval gate** is a test set with known-good outputs, a grading function, a delta threshold, and a rule that every change re-enters it. The threshold is the part people get wrong, because it is trivial to set it after seeing the numbers:

```yaml
# eval-gate.yaml, committed before the run
gate: prompt-v14
model: pinned-model-id
baseline: prompt-v13
metrics:
  field_extraction_f1: {min: 0.94, delta_min: -0.01}
  refusal_accuracy:    {min: 0.97, delta_min:  0.00}   # control behavior
categories: [happy_path, edge_case, adversarial, refusal, tool_error]
```

A threshold chosen after the run is a description, not a gate. Set the delta before the run, make every named failure mode its own category, and read per-category and per-subgroup results rather than the mean, because a rising mean can hide a category that collapsed. Then wire it so the build cannot proceed past a failure:

```bash
# applies to model swaps, prompt revisions, context changes, retrieval changes, control changes
python -m evals.run --gate eval-gate.yaml || exit 1
```

Two further details that pay for themselves: use a **different model as the judge** than the one being judged, calibrated against human-labeled examples, and include the control behaviors from Phase 4 in the dataset. An eval suite that only tests task quality will happily pass a build whose guardrails do not work.

## Another gotcha: averaging the cost and latency budget

Averages hide the two numbers that decide whether the project is viable. Real traffic has a tail, and the tail is where the SLA breaks and the bill comes from:

```python
shapes = [shape_for(r) for r in traffic_sample]
monthly = sum(s.input * price_in + s.output * price_out for s in shapes) / len(shapes) * volume
assert percentile(latency_of(shapes), 95) < sla_p95_ms

# sensitivity, not optimism: double the volume and shift the mix toward the tail
assert worst_case_doubled_tail(monthly) < budget_ceiling
```

Three costs belong in that model and usually are not: retrieval and tool-call round trips in the latency budget, eval and judge calls in the cost model, and human review labor wherever the design routes work to a person. A projection that assumes full automation while the design requires review is not a projection, it is a wish.

## Another gotcha: the model tier decided per system instead of per step

**Over-assigning work to the model is the default failure.** Every step goes to the model, an existing system, or a human, and each handoff needs a reason:

```yaml
steps:
  extract_fields:   {to: model, tier: mid, version: pinned, reason: eval-win}
  lookup_coverage:  {to: system, reason: live state, not retrieval}
  draft_notice:     {to: model, tier: mid, version: pinned, reason: baseline}
  high_value_claim: {to: human, reason: stakes, confidence is not correctness}
```

Two rules are worth keeping on the wall here. The tier is decided per step and changed only on an eval result, with the version pinned and a rollback threshold set in advance. And retrieval serves stable knowledge, not live state: policy coverage, account status, and inventory levels are tool calls against the system of record, because an index is a snapshot and a snapshot is stale the moment it is written.

## Another gotcha: monitoring that reports but does not decide

The release artifact is a **governance table** with five columns: signal, trigger, action, owner, and regulated checkpoint. A dashboard answers what is happening. The table answers who does what when it moves, which is the only version of monitoring that survives a launch. Cost above 150% of the seven-day average and p95 crossing the SLA are triggers, not charts. Slow drifts belong in the table alongside hard failures, because a category that quietly degrades never fires an alert. Where a compliance checkpoint exists, it goes on the calendar and runs whether or not a metric moved.

## Another gotcha: documentation written for one reader

Handoff documentation has three readers: the inheriting engineer, the auditor, and the returning architect. Serving one leaves it incomplete, and the completeness test is blunt: can someone who was not in the room make a safe change from the document alone? That means recording the rejected alternatives and the trade-off each decision resolved, not just the decision. The **runbook** is symptom, cause, first action, one entry per failure mode. Prompts and reusable assets are versioned alongside the code and the docs, because a prompt is a deployment artifact with the same change discipline as a migration.

## The last gotcha: the before metric is gone by then

The **outcome document** closes the loop with six fields: before metric, after metric, measurement method, owner, measurement window, and the control that produced the evidence.

```markdown
before:  4.2 min median handling time (n=1,180, queue system, measured 2026-06-02)
after:   pending
method:  median handling time, same query, same population
owner:   operations lead
window:  2026-10-01 to 2026-12-31
control: route selection logged, decision log retained 400 days
```

The catch is the first field. A before metric cannot be reconstructed later, so it is captured during discovery, together with the measurement method, so both halves of the comparison use the same definition. If the after metric needs runtime, name the owner, confirm logging is live, and set the completion milestone. Do not mark the gate passed.

## Scale to risk, and to headcount

The checklist is explicitly **scaled to risk**: a low-risk internal tool does not need all 36 artifacts, but it does need the alignment boundary and its controls, the eval suite, and the outcome document. Those three are the floor.

There is a second axis for a project run by one person, and it is where a checklist written for teams becomes unusable if you take it literally. Boundary conditions, the risk assessment, and the failure-mode table are three artifacts because three roles read them. One person merges them into one register, and the merge is fine as long as the entries keep their owner, their likelihood times impact, and their evidence. Team setup and spend policy stop being a rollout plan and become self-configuration. Escalation paths shrink to the one external contact who can unblock you.

Review mode is the same checklist run as an audit, and it condenses to five questions: is the alignment boundary correct, or are deployment-specific rules assumed to be model-enforced? Are all three control points present, and do they fail closed? Is retrieval being used for live state? Is review routed by stakes, or by volume? Can a successor make a safe change from the documentation alone?

Draw the gate before you build the step. The items are detail. The orderings are the content. The full list is at [LLM Project Checklist](/llm-project-checklist/): 333 items, 11 phases, 11 gates, 36 artifacts, with the conditional items marked so you can skip what does not apply.

One note on provenance. The checklist was assembled while preparing for the Claude Certified Architect professional exam. It reproduces none of the exam guide's objectives or weights. It is delivery practice, and the framework rules it borrows are cited below.

[Anthropic: Building Effective Agents](https://www.anthropic.com/engineering/building-effective-agents) · [Prompt caching](https://docs.claude.com/en/docs/build-with-claude/prompt-caching) · [Batch processing](https://docs.claude.com/en/docs/build-with-claude/batch-processing) · [Data residency](https://docs.claude.com/en/docs/build-with-claude/data-residency) · [Model Context Protocol](https://modelcontextprotocol.io/docs/getting-started/intro) · [Cormack et al., Reciprocal Rank Fusion (SIGIR 2009)](https://plg.uwaterloo.ca/~gvcormac/cormacksigir09-rrf.pdf)
