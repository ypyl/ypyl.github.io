---
layout: post
title: "Review the Decision, Not the Code"
date: 2026-11-09
categories: ai
tags: [ai-agents, spec-driven-development, code-review, ai-coding, testing]
---

An agent now writes more code in an afternoon than I can read in a week. Every review process I have worked under assumes the opposite: that a person reads the diff and decides. Once that assumption fails, the process does not need better reviewers. It needs to decide, before the code exists, which decisions are mine.

What follows is the routing rule I use, the document the gate depends on, and the record it leaves behind. The rule decides where a human looks. The document decides whether looking is worth anything. The record is what remains when nobody looked at all.

## The constraint moved

My artifact chain was the formal one, the version written in process documents rather than the way any particular team works. A PRD says what to build, an RFC opens the debate, an **ADR** records the choice and the trade-offs it accepts, a design doc maps the system. None of it is set in stone: teams drop the documents they do not need and rename the ones they keep. Every document in that chain exists to serve people who write and read code, so the chain scales with the volume of code. That was the right axis when code was the expensive thing.

Here is what running it unchanged costs. A cosmetic change in one of my projects produced a proposal, a design document of more than fifty lines, a task list and a delta spec. Over a hundred lines of process, wrapped around roughly thirty lines of code that a browser could verify in one look.

The artifact lines are not the cost. The attention spent reading and approving them is.

### Ceremony spent on a decision nobody kept

The design document is the part worth reading. It has a section on icon size, with a decision, a rationale, and an alternative considered and rejected.

That decision was reversed the same day. Two more changes to the same component followed within two days, and neither of them carried a proposal.

So the one decision I gave real attention to did not survive the afternoon, and the changes that did survive never met a gate. The pipeline was not too heavy. It was pointed at the wrong thing.

### Three questions

If attention is the thing in short supply, the process needs a test that spends it deliberately instead of a document that records it afterwards. A change needs a human when at least one of these is true:

1. **It cannot be undone easily.** Live data migrations, published API changes, deleted user content, anything an incident makes expensive to reverse.
2. **No machine can prove it correct.** If there is no test, no CI check, no visual diff that fails when the change is wrong, a human is the only instrument left.
3. **Something outside the change depends on it.** A contract, a schema, a permission model, a shared module.

When all three are false, let it land. The cosmetic change above passes all three: revert is one commit, the result is visible in a browser, and nothing outside that component depends on it. A data model change fails at least one; usually it fails all three.

The third question asks about consumers nobody has looked at, so it needs a concrete form: name the document that holds the contract, an OpenAPI file, a migration, a permissions table, a threat model. If you cannot name one, either nothing depends on the change or the contract is undocumented, and the second case is its own L2 decision.

The output of the test is not "review" or "do not review". It is which artifact a human reads. A change that cannot answer the three questions is not a candidate for a lighter process; it is not scoped enough to start.

## Four levels

{::nomarkdown}
<figure>
<svg xmlns="http://www.w3.org/2000/svg" viewBox="0 0 940 480" role="img" style="width:100%;height:auto" font-family="-apple-system,BlinkMacSystemFont,'Segoe UI',Roboto,Helvetica,Arial,sans-serif">
  <title>What the human reads and what the machine proves</title>
  <desc>Four levels of gate. At L0 nobody reads anything and CI proves the change correct. At L1 the human reads the scenarios and the tests prove each one exists. At L2 the human reads the ADR and the machine proves compatibility, not whether breaking it is acceptable. At L3 the human reads the evidence and the machine proves the rehearsal, the rollback and the reconciliation. No machine proves the judgment, so human attention rises as the claim narrows.</desc>
  <defs>
    <pattern id="dots" width="22" height="22" patternUnits="userSpaceOnUse">
      <circle cx="1" cy="1" r="0.9" fill="#E3E2DC"/>
    </pattern>
  </defs>
  <rect width="940" height="480" fill="#f5f4ed"/>
  <rect width="940" height="480" fill="url(#dots)" opacity="0.55"/>

  <text x="160" y="42" fill="#1B365D" font-size="14" font-weight="600" font-family="'JetBrains Mono','SF Mono','Fira Code',Consolas,Monaco,monospace" letter-spacing="3">FIGURE  1</text>
  <text x="288" y="42" fill="#504e49" font-size="14" font-family="'JetBrains Mono','SF Mono','Fira Code',Consolas,Monaco,monospace" letter-spacing="3">WHAT YOU READ, WHAT IS PROVEN</text>
  <line x1="160" y1="54" x2="760" y2="54" stroke="#1B365D" stroke-width="0.8"/>

  <path d="M 112 430 V 112" fill="none" stroke="#504e49" stroke-width="1.5"/>
  <path d="M 106 118 L 112 110 L 118 118" fill="none" stroke="#504e49" stroke-width="1.5" stroke-linecap="round" stroke-linejoin="round"/>
  <text x="112" y="96" text-anchor="middle" fill="#6b6a64" font-size="11" font-family="'JetBrains Mono','SF Mono','Fira Code',Consolas,Monaco,monospace" letter-spacing="2">ESCALATE</text>
  <text x="112" y="452" text-anchor="middle" fill="#6b6a64" font-size="11" font-family="'JetBrains Mono','SF Mono','Fira Code',Consolas,Monaco,monospace" letter-spacing="2">START</text>

  <rect x="160" y="90" width="600" height="78" rx="6" fill="#faf9f5" stroke="#141413" stroke-width="1.5"/>
  <text x="182" y="124" fill="#6b6a64" font-size="13" font-weight="600" font-family="'JetBrains Mono','SF Mono','Fira Code',Consolas,Monaco,monospace">L3</text>
  <text x="228" y="124" font-size="18" font-weight="600" fill="#141413">Costly to undo, or someone else's data</text>
  <text x="228" y="148" font-size="14" fill="#504e49">rehearsal, rollback, reconciliation</text>
  <text x="744" y="124" text-anchor="end" fill="#6b6a64" font-size="13" font-family="'JetBrains Mono','SF Mono','Fira Code',Consolas,Monaco,monospace">reads: the evidence</text>
  <text x="744" y="148" text-anchor="end" fill="#6b6a64" font-size="13" font-family="'JetBrains Mono','SF Mono','Fira Code',Consolas,Monaco,monospace">proves: rehearsal</text>

  <rect x="160" y="182" width="600" height="78" rx="6" fill="#faf9f5" stroke="#141413" stroke-width="1.5"/>
  <text x="182" y="216" fill="#6b6a64" font-size="13" font-weight="600" font-family="'JetBrains Mono','SF Mono','Fira Code',Consolas,Monaco,monospace">L2</text>
  <text x="228" y="216" font-size="18" font-weight="600" fill="#141413">Contract, stack or capability change</text>
  <text x="228" y="240" font-size="14" fill="#504e49">an ADR: context, decision, consequences</text>
  <text x="744" y="216" text-anchor="end" fill="#6b6a64" font-size="13" font-family="'JetBrains Mono','SF Mono','Fira Code',Consolas,Monaco,monospace">reads: the ADR</text>
  <text x="744" y="240" text-anchor="end" fill="#6b6a64" font-size="13" font-family="'JetBrains Mono','SF Mono','Fira Code',Consolas,Monaco,monospace">proves: compatibility</text>

  <rect x="160" y="274" width="600" height="78" rx="6" fill="#faf9f5" stroke="#141413" stroke-width="1.5"/>
  <text x="182" y="308" fill="#6b6a64" font-size="13" font-weight="600" font-family="'JetBrains Mono','SF Mono','Fira Code',Consolas,Monaco,monospace">L1</text>
  <text x="228" y="308" font-size="18" font-weight="600" fill="#141413">Behavior inside a capability</text>
  <text x="228" y="332" font-size="14" fill="#504e49">a delta spec, one WHEN/THEN scenario per behavior</text>
  <text x="744" y="308" text-anchor="end" fill="#6b6a64" font-size="13" font-family="'JetBrains Mono','SF Mono','Fira Code',Consolas,Monaco,monospace">reads: scenarios</text>
  <text x="744" y="332" text-anchor="end" fill="#6b6a64" font-size="13" font-family="'JetBrains Mono','SF Mono','Fira Code',Consolas,Monaco,monospace">proves: tests</text>

  <rect x="160" y="366" width="600" height="78" rx="6" fill="#faf9f5" stroke="#141413" stroke-width="1.5"/>
  <text x="182" y="400" fill="#6b6a64" font-size="13" font-weight="600" font-family="'JetBrains Mono','SF Mono','Fira Code',Consolas,Monaco,monospace">L0</text>
  <text x="228" y="400" font-size="18" font-weight="600" fill="#141413">Revertible, provable, unshared</text>
  <text x="228" y="424" font-size="14" fill="#504e49">CSS, config wiring, tests, dependency bumps</text>
  <text x="744" y="400" text-anchor="end" fill="#6b6a64" font-size="13" font-family="'JetBrains Mono','SF Mono','Fira Code',Consolas,Monaco,monospace">reads: nothing</text>
  <text x="744" y="424" text-anchor="end" fill="#6b6a64" font-size="13" font-family="'JetBrains Mono','SF Mono','Fira Code',Consolas,Monaco,monospace">proves: CI</text>
</svg>
</figure>
{:/nomarkdown}

Most changes belong on the bottom rung, and that is the point of drawing it. The three rungs above exist so that the attention they take is spent on the decisions that survive the week.

Follow the right-hand labels up the figure and what the machine proves changes character rather than disappearing. At **L0** it proves the change is correct. At **L1** it proves only that each scenario has a test which fails when the behavior changes. At **L2** it proves compatibility, not whether breaking it is acceptable. At **L3** it proves the mechanics: the rehearsal ran, the rollback works, the counts reconcile. The judgment is never proved, which is why attention climbs as the claim narrows, and why the level is derived from the change rather than chosen.

Read it from the bottom, because that is where a change starts.

**L0** is the default, not a privilege. A change lands there when one commit undoes it, when CI or a browser can show that it is wrong, and when nothing outside it depends on the result. L0 skips the middle of the process entirely: no design document, no durable spec, no review. One piece stays, for a reason that has nothing to do with review.

**L1** is for behavior. You read the scenarios and accept that the product will do these things. The code goes unread, which is tolerable only because each scenario names a test that fails when the behavior changes. Skip L1 and a behavior change lands with nobody having stated it, and the durable spec drifts one step further from the code.

**L2** is for the contracts named above. You read the ADR: what forced the choice, what was rejected, what it costs. A compatibility check, if you run one, tells you whether the contract still accepts its callers. It cannot tell you whether breaking them is acceptable. Skip L2 and the contract changes anyway, and six months later the only record is a diff.

**L3** is for what cannot be undone easily, and it changes the regime rather than the author. The agent writes the code and has to show what it did. Four things: the decision with its blast radius, the change rehearsed against a production-shaped copy, a rollback that was executed rather than assumed, and a reconciliation after apply. You read that evidence, not the code, which is the only reason the rung works at all. Rehearsal means running the change against a copy of production data; reconciliation means checking after apply that row counts and checksums match what the change declared.

Most changes that look irreversible are not. Expand and contract: add the column, dual-write, backfill, reconcile, switch the reads, drop it in a later change. What makes that safe is not the number of steps but two properties of each one: the system stays valid after it, and its effect can be taken back. L3 is left with what has neither property: deleting user data, changing a cryptographic scheme, publishing an API version that cannot be retracted.

A step that is small but not undoable is not a decomposition, it is a schedule. Delete ten percent of the rows six times and no single change looks irreversible, each one passes CI, and the loss is total after the sixth. Which is why the level is set for the feature rather than the step: if the series is L3, every phase inherits it and the blast radius is declared once for the series.

### How it runs in practice

The mechanics in this section belong to [OpenSpec](https://github.com/Fission-AI/OpenSpec), a small command-line toolkit that keeps changes and specs in the repository next to the code. The word spec means two different files here, and the table below uses both. The **delta spec** sits inside the change folder and states only what this change alters. The **durable spec** sits in `openspec/specs/` and describes the system as it stands, so archiving a change merges the delta into it.

The level is decided once, at the start, and it selects the procedure rather than the paperwork. Every level runs the same four steps, `explore` then `propose`, `apply`, `archive`; what the level decides is which of those steps stops for a person.

| Level | How it starts | What the change folder holds | Where the human stops |
|---|---|---|---|
| **L0** | `propose`, no explore | proposal, tasks | nowhere |
| **L1** | `propose` | proposal, delta spec, tasks, `design.md` when the approach is not obvious | at the spec, before apply |
| **L2** | `explore`, then `propose` | ADR first, then proposal, delta spec, design, tasks | at the ADR, then at the spec, which must match it |
| **L3** | `explore` in options-only mode | everything L2 holds, plus the rehearsal output, the rollback result and the reconciliation counts | at the ADR, at the spec, then on the evidence |

The readings accumulate rather than replace each other: L1 reads the spec, L2 reads the ADR and then the spec, and L3 adds the evidence to both. The second stop at L2 is not a second opinion on the decision. It asks a narrower question, whether the delta spec says what the ADR decided, which is the check that catches an agent implementing something adjacent to the choice it was told to make.

Explore is the L2 entry rather than a general step: it is what produces the ADR before any delta exists. At L1 it is optional. At L0 it is a signal that the change was routed one rung too low.

No row above L3 stops after apply, and that is deliberate rather than careless. The result is still checked at every level, by the machine. The apply guidance you put in the config has the agent run lint, the build and the tests before it finishes, and the archive entry records what was verified. A person reads the result only where the result cannot be undone. Everywhere below that, the price of a wrong result is a revert, so the correct amount of human attention on the outcome is none until something suggests otherwise.

Every rung above L0 assumes the same thing: whatever it puts in front of a person has to be something that person can judge. A gate that hands a reviewer a document they cannot evaluate is not a gate, it is a signature. That failure deserves its own section, because it is the one I keep making, and it happens at L1.

## A gate is only as good as the document

### The spec is the review surface, so it has to be readable

If a human at L1 reads only the spec, then the spec is the product. This is where the process usually collapses, and it collapsed in mine.

A spec written from a change inherits the change's vocabulary. The agent drafting it has the diff in context, so the spec names the file it touched, the class it added, the value it picked. Run this in a codebase you have been spec-driving for a few months:

```bash
grep -rE 'px|class=|fill=|var\(' openspec/specs/
```

What comes back is not a specification, it is a diff with headings. A sentence like "the icon SHALL use `fill="currentColor"`" describes a rendering trick, not a behavior, and a product owner reading it has nothing to accept or reject.

The second failure follows from the same cause. One capability accumulates one spec per change that touched it, so after a year the folder is a change log wearing requirement headers.

A spec in that shape makes the L1 gate theater. The human signs, the accountability moves, and nothing was reviewed.

### Three rules keep the spec readable

**1. Organize by capability, not by change.** One file per durable capability, extended when behavior grows. In OpenSpec that is `context` and `rules` in `openspec/config.yaml`:

```yaml
schema: spec-driven

context: |
  Specifications are organized by business domain and capability:
    openspec/specs/<domain>/<capability>/spec.md
  Before creating a new specification, inspect the existing ones, find the
  domain that owns the behavior, and extend an existing capability when the
  behavior belongs to it. A new ticket, story, endpoint or change does not
  justify a new specification.

rules:
  specs:
    - Organize specifications as specs/<domain>/<capability>/spec.md.
    - Prefer extending an existing capability over creating a new one.
    - Keep specifications implementation-independent: no file paths, class
      names, CSS values or library choices.
```

The config adds no capability: nested paths already work, and the tool's own instruction says to skip a domain level when a project is flat. It changes the default, from one directory per change back to the existing capability.

It is not enforced either. `openspec validate` checks that deltas exist, not where they land, so the rule that bites is a CI check flagging any change that adds a directory under `specs/`. A new capability then takes a deliberate yes.

**2. Implementation belongs in the change, not in the spec.** The delta spec inside `changes/<change>/` may be technical; it is a scratchpad and it gets archived. The durable spec may not be. The schema's own test is the one to apply: if the implementation can change without changing externally visible behavior, it does not belong in the spec.

**3. Skip the spec, not the record.** Small technical work should not touch the durable spec. OpenSpec has a marker for it in `changes/<change>/.openspec.yaml`:

```yaml
schema: spec-driven
skip_specs: true
```

`openspec validate` enforces it in both directions: a change with no deltas and no marker fails, and so does a marker sitting next to files under `specs/`. The record still has to say something, though, so an exempt change ships as a proposal and a task list, and the proposal names the requirement the work leaves intact. An exemption leaves a record; an omission leaves nothing.

After those three rules, `openspec/specs/` holds the two things a non-technical owner can judge: what the artifact does, and the constraints it must keep. Privacy, accessibility, a performance budget, browser support. Functional and non-functional requirements in the same file, in a form that survives the implementation.

The same test applies to every other file you are tempted to commit. If a tool can rebuild it from the repository, it does not need to be stored. If no tool can, nothing fails when it stops being true.

## What code review used to do

### Proof moves from reading to executing

Since the code is unreviewed, the proof has to be executable. Each scenario in the delta spec has to be able to fail a test whose name a human can read:

```markdown
#### Scenario: Declined card payment
- **WHEN** the provider declines the card
- **THEN** the order SHALL stay unpaid
- **AND** the customer SHALL see the decline reason
```

The human approves that list. The agent writes the test that matches it. The test name appears in CI output, so a broken scenario is visible without opening the test body.

The catch: an agent that writes tests from its own implementation produces tests that assert whatever it built, and a spec approved in the morning can be quietly edited by the afternoon. "The spec does not change after approval" is a preference, not a constraint, so the fix is two commit boundaries rather than an instruction.

**Freeze the spec at a commit.** Commit the change's spec files before apply starts. From then on, "the spec changed after approval" is not a claim, it is a diff against that commit, and the review reads the spec at the commit rather than at HEAD. There is a second reason to do it: the delta spec is the only record of what was approved, and at archive time that text merges into the durable spec. A silent edit does not just skip the gate, it becomes the permanent record.

**Bind each scenario to a test by name.** A CI check that every `#### Scenario:` in an approved change has a test whose name carries the scenario slug does not prove the assertion is right. It proves the mapping exists, and it makes deleting a scenario fail a build.

Neither check fixes assertion quality: a scenario can be covered by a test that asserts nonsense. That residual is the real price of not reading the code, and it is smaller than the price of reading a diff you cannot evaluate.

### Every line can be traced to a change

Code review used to carry two things nobody counted: it verified the change, and it produced a spoken record of why. Proof replaces the first. This is the second.

A repository contains only what somebody committed: the code, the git history, and the artifacts chosen to go with it. An agent leaves no account of its reasoning in there. Whatever a session recorded stays on the machine that ran it, unversioned, and never reaches the person reading the code six months later.

The change folder's second job is this one, and it is why the record survives even when the spec does not. The proposal says what was intended and why, `design.md` says which approach was taken over which alternative, `tasks.md` names the files.

In practice: from the line, to the commit that touched it, to the change that commit came from. You get the intent the change was working to, what it was allowed to touch, and what was rejected. Not why that line special-cases zero, and you should not need that, because local reasoning belongs in the code where it can be read. In work no human reviewed, that distance is the difference between archaeology and a guess.

At L3 the record carries evidence as well as intent: the rehearsal output, the rollback result, the reconciliation counts. At that point the archive stops being a reconstruction of what the agent meant and becomes an account of what the system did.

Approval is a signature, and it lives in the pull request rather than in the repository. A merged PR records who accepted the change and when, which is why reviewed work should land through a human merge: the merge is the attestation, and work pushed straight to the trunk leaves none. That is enough for accountability, because the person who merges carries the consequence, and a signature does not have to prove diligence to work.

It only works while the thing being signed can be judged. A spec full of pixel values forces a reviewer to sign something they cannot evaluate, and then the signature passes on the blame without passing on the control. Which is why the readability rules above are load-bearing rather than tidy.

The mechanical version: make the level a required check on the pull request. L0 merges on green CI, L1 and above require a human approval, L3 requires the evidence in the diff. The routing rule then stops being a habit and becomes a gate.

## What breaks

**Emergent architecture.** A hundred L0 changes, each harmless, compose into a structure nobody chose. A generated map handles this case well. Container and module maps, dependency graphs, API surfaces and schemas can be rebuilt from the repository on demand, so they need not be stored and cannot go stale. Generation shows you the shape. It cannot tell you whether the shape was intended, and that part needs an ADR.

**Exemption creep.** A category becomes L0 because review was inconvenient, then something in it breaks and nobody saw it. The rule that helps: a revert or an incident escalates that category permanently, and the escalation is written into the config rules, not remembered.

**Spec drift.** Code correct, spec stale, and the spec now lies. I have no clean answer for this one. My current handling is to treat `specs/` as authoritative for behavior, which makes stale text a bug. I fix it in the next change that touches that capability, not in a cleanup pass nobody schedules.

## The takeaway

You are not reviewing code any more. You are reviewing three questions: can it be undone, can a machine prove it, and who else depends on it. Everything else is production, and production is what the agent is for. The one thing production still owes the future is a record of what it was trying to do.

[OpenSpec on GitHub](https://github.com/Fission-AI/OpenSpec) · [Documenting architecture decisions (Nygard, 2011)](https://cognitect.com/blog/2011/11/15/documenting-architecture-decisions) · [Structured Prompt-Driven Development (Fowler)](https://martinfowler.com/articles/structured-prompt-driven/) · [OpenSpec scoping: one change or many?]({% post_url 2026-06-23-openspec-scoping-check %})
