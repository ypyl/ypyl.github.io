---
layout: post
title: "The Deconstruction Gate: First Principles for Working with AI"
date: 2026-09-05
categories: ai
tags: [ai, llm, first-principles, problem-solving, prompting]
---

Ask an AI to solve a problem and it answers with the most likely solution, which by definition is the one everyone already uses. **First principles thinking** is the countermeasure: break the problem into its most basic, undeniable truths and build from there, instead of iterating on what came before. It used to be a habit for people who wanted to avoid mediocrity. In the AI era it is the difference between delegating and outsourcing your judgment.

The rebuild used to be the hard part: generating, testing, and iterating candidate solutions took most of the effort. Now LLMs do that at near-zero marginal cost. The scarce skill moved entirely into the deconstruction. Hand a code agent a vague request and it will happily ship the conventional architecture at full speed, high quality, and complete confidence.

**The catch:** you cannot trust the first output because it is optimized to be plausible, so you get the analogy whether you asked for it or not. The fix is a gate you run before any delegation: strip the problem to its truths, encode them as acceptance tests, and only then let the AI generate and test against that fixed foundation. Two distinctions make the gate work with an LLM. First, label each assumption **LAW** (must hold), **ARTIFACT** (historical convention, contestable), or **PREFERENCE** (optional), because the AI cannot tell these apart for your domain. Second, separate HARD truths (domain or physics constraints) from SOFT ones (conventions you can drop), or the agent will treat them as equal weight.

The gate is a paste-in prompt. Run it before the first implementation message:

```text
# DECONSTRUCTION GATE · run before you solve

Role: You are a deconstruction partner, not a solver yet.
Do not propose a solution, method, or plan. Work with me
through five passes. Keep answers tight.

## PASS 1 · RESTATE THE PROBLEM
In one or two sentences, restate what I want to solve and what
that phrasing presumes.

## PASS 2 · SURFACE AND QUESTION ASSUMPTIONS
List every assumption the conventional solution would carry
(why it is done this way, what "best practice" implies here).
For each: why is it done this way? Is it a law of nature or a
hard requirement, or a historical artifact / preference / convention?
Mark each: LAW (must hold) · ARTIFACT (contestable) · PREFERENCE (optional).
Run Five Whys on the top three assumptions; use Socratic questioning
(evidence? alternatives? consequences?) where doubt remains.

## PASS 3 · DECONSTRUCT TO TRUTHS
What must remain true for the outcome to work, in the fewest
indivisible statements? This is the raw-material cost of the
problem: strip the market price of the usual solution and keep
only what the problem itself demands.
Separate HARD truths (domain/physics/requirement) from SOFT ones
(conventions we can drop).

## PASS 4 · DEFINE ACCEPTANCE AS TESTS
Turn every truth into a checkable criterion: "we are right when
X holds / when Y is measurably better / when Z can no longer
happen." These tests grade the solution. Flag anything that still
needs verification before we lock the brief.

## PASS 5 · PROPOSE DIVERGENT DIRECTIONS
Propose 2-3 radically different directions, each traced back to
the truths. Do not default to the conventional approach. For each,
state openly where the conventional solution would fail or what it
forces us to assume. Argue against the obvious choice if the truths
do not demand it.

Then stop. Present the gate report: truths, tests, directions,
open questions. Wait for my decision before you proceed.
```

The gate report is the artifact you hand over. It is a brief, a filter, and a contract in one: the truths say what must hold, the tests say how you check, and the divergent directions say the AI argued against the default instead of serving it.

{::nomarkdown}
<figure>
<svg xmlns="http://www.w3.org/2000/svg" viewBox="0 0 864 456" role="img" style="width:100%;height:auto" font-family="-apple-system,BlinkMacSystemFont,'Segoe UI',Roboto,Helvetica,Arial,sans-serif">
  <title>Deconstruct before you delegate</title>
  <desc>You run the gate: restate the problem, surface its assumptions, and with the AI partner draft the truths and the brief. The brief crosses to the AI lane, which generates candidates and tests them against the truths, iterating until they hold. You make the final call.</desc>
  <defs>
    <pattern id="dots" width="22" height="22" patternUnits="userSpaceOnUse">
      <circle cx="1" cy="1" r="0.9" fill="#E3E2DC"/>
    </pattern>
  </defs>
  <rect width="864" height="456" fill="#f5f4ed"/>
  <rect width="864" height="456" fill="url(#dots)" opacity="0.55"/>

  <text x="40" y="40" fill="#1B365D" font-size="14" font-weight="600" font-family="'JetBrains Mono','SF Mono','Fira Code',Consolas,Monaco,monospace" letter-spacing="3">FIGURE  1</text>
  <text x="152" y="40" fill="#504e49" font-size="14" font-family="'JetBrains Mono','SF Mono','Fira Code',Consolas,Monaco,monospace" letter-spacing="3">DECONSTRUCT BEFORE YOU DELEGATE</text>
  <line x1="40" y1="52" x2="824" y2="52" stroke="#1B365D" stroke-width="0.8"/>

  <text x="40" y="104" fill="#6b6a64" font-size="14" font-family="'JetBrains Mono','SF Mono','Fira Code',Consolas,Monaco,monospace" letter-spacing="3">YOU · THE GATE</text>
  <text x="520" y="104" fill="#6b6a64" font-size="14" font-family="'JetBrains Mono','SF Mono','Fira Code',Consolas,Monaco,monospace" letter-spacing="3">AI · GENERATE &amp; TEST</text>

  <rect x="40" y="120" width="160" height="64" rx="6" fill="#faf9f5" stroke="#141413" stroke-width="1.5"/>
  <text x="120" y="150" text-anchor="middle" font-size="18" font-weight="600" fill="#141413">PROBLEM</text>
  <text x="120" y="170" text-anchor="middle" font-size="14" fill="#6b6a64">the ask as given</text>

  <rect x="40" y="232" width="160" height="64" rx="6" fill="#EEF2F7" stroke="#1B365D" stroke-width="1.5"/>
  <text x="120" y="262" text-anchor="middle" font-size="18" font-weight="600" fill="#141413">TRUTHS</text>
  <text x="120" y="282" text-anchor="middle" font-size="14" fill="#6b6a64">AI drafts · you judge</text>

  <rect x="40" y="344" width="160" height="64" rx="6" fill="#faf9f5" stroke="#141413" stroke-width="1.5"/>
  <text x="120" y="374" text-anchor="middle" font-size="18" font-weight="600" fill="#141413">BRIEF</text>
  <text x="120" y="394" text-anchor="middle" font-size="14" fill="#6b6a64">gate report · you lock</text>

  <rect x="520" y="120" width="160" height="64" rx="6" fill="#faf9f5" stroke="#141413" stroke-width="1.5"/>
  <text x="600" y="150" text-anchor="middle" font-size="18" font-weight="600" fill="#141413">DECISION</text>
  <text x="600" y="170" text-anchor="middle" font-size="14" fill="#6b6a64">the call is yours</text>

  <rect x="520" y="232" width="160" height="64" rx="6" fill="#faf9f5" stroke="#141413" stroke-width="1.5"/>
  <text x="600" y="262" text-anchor="middle" font-size="18" font-weight="600" fill="#141413">TEST</text>
  <text x="600" y="282" text-anchor="middle" font-size="14" fill="#6b6a64">run against truths</text>

  <rect x="520" y="344" width="160" height="64" rx="6" fill="#faf9f5" stroke="#141413" stroke-width="1.5"/>
  <text x="600" y="374" text-anchor="middle" font-size="18" font-weight="600" fill="#141413">CANDIDATES</text>
  <text x="600" y="394" text-anchor="middle" font-size="14" fill="#6b6a64">generate N directions</text>

  <path d="M 120 184 V 232" fill="none" stroke="#504e49" stroke-width="1.5"/>
  <path d="M 116 226 L 120 232 L 124 226" fill="none" stroke="#504e49" stroke-width="1.5" stroke-linecap="round" stroke-linejoin="round"/>
  <path d="M 120 296 V 344" fill="none" stroke="#504e49" stroke-width="1.5"/>
  <path d="M 116 338 L 120 344 L 124 338" fill="none" stroke="#504e49" stroke-width="1.5" stroke-linecap="round" stroke-linejoin="round"/>

  <path d="M 200 376 H 520" fill="none" stroke="#504e49" stroke-width="1.5"/>
  <path d="M 514 372 L 520 376 L 514 380" fill="none" stroke="#504e49" stroke-width="1.5" stroke-linecap="round" stroke-linejoin="round"/>

  <path d="M 600 344 V 296" fill="none" stroke="#504e49" stroke-width="1.5"/>
  <path d="M 596 302 L 600 296 L 604 302" fill="none" stroke="#504e49" stroke-width="1.5" stroke-linecap="round" stroke-linejoin="round"/>
  <path d="M 600 232 V 184" fill="none" stroke="#504e49" stroke-width="1.5"/>
  <path d="M 596 190 L 600 184 L 604 190" fill="none" stroke="#504e49" stroke-width="1.5" stroke-linecap="round" stroke-linejoin="round"/>

  <path d="M 200 264 H 520" fill="none" stroke="#1B365D" stroke-width="1.5"/>
  <path d="M 514 260 L 520 264 L 514 268" fill="none" stroke="#1B365D" stroke-width="1.5" stroke-linecap="round" stroke-linejoin="round"/>
  <text x="360" y="248" text-anchor="middle" font-size="14" fill="#1B365D">truths are the filter</text>

  <path d="M 680 264 H 824 V 376 H 680" fill="none" stroke="#1B365D" stroke-width="1.5"/>
  <path d="M 674 372 L 680 376 L 674 380" fill="none" stroke="#1B365D" stroke-width="1.5" stroke-linecap="round" stroke-linejoin="round"/>
  <text x="752" y="320" text-anchor="middle" font-size="14" fill="#1B365D">iterate candidates</text>
</svg>
</figure>
{:/nomarkdown}

Read it as two lanes with one bridge. You run the gate: PROBLEM to TRUTHS to BRIEF, drafted with the AI as your partner but judged and locked by you. The brief crosses to the AI lane, where the AI generates candidates against the brief and tests them against the truths, iterating until the truths hold. The two ink-blue strokes are the mechanism: the truths filter the tests, and the loop is where failure is useful, because a candidate that fails a test is cheap and the test itself is fixed. The decision node sits in the AI lane but is labeled "the call is yours", because only you hold the domain intent that distinguishes a LAW from an ARTIFACT.

**Another gotcha:** the gate protects the decision point, but not the build. During implementation, an LLM will silently bend a HARD truth to keep the momentum and please you, and it will do it smoothly. The fix is an explicit in-work rule appended to the brief:

```text
While working the chosen direction: if any step would violate a
HARD truth, or bend an acceptance test, stop and report the
conflict. The gate decides, not the execution.
```

That rule is what turns the brief from a document into a contract. A final point about scope: the same gate runs at three levels of delegation. **Reflection mode** runs PASS 1-4 only, no solution generation, for problems you will not hand over. **Options mode** adds PASS 5 and the test loop, when you want directions but not execution. **Execution mode** is the full gate plus the in-work rule, which is where code agents live. The prompt is identical; you only grant more privileges.

Start with one assumption. Take the next problem you were about to paste raw into an agent, run the gate first, and compare the brief you get with the reply you would have received. That comparison is the whole point: the analogy is free, the truth is the only thing you can own.

[First Principles Thinking: A Framework for Solving Problems (maray.ai)](https://www.maray.ai/posts/first-principles-thinking) · [First Principles: The Science of Discovering What You Don't Know (Farnam Street)](https://fs.blog/first-principles/)