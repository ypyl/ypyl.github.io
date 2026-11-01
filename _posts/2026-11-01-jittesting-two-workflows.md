---
layout: post
title: "JiTTesting Runs Two Workflows: The Catch Is What You Throw Away"
date: 2026-11-01
categories: programming
tags: [ai-agents, testing, llm, ci-cd, code-review]
---

The problem with tests as they stand: a behaviour bug is found either by a reviewer reading a diff, or by a red suite days after the change landed. Both arrive late. **JiTTesting** moves the red earlier by writing a throwaway test for the diff while it is still a merge candidate, running it against the parent commit and against the changed code, and alerting the author only when those two results disagree.

The catch is that a disagreement proves almost nothing on its own. A probe that passes on the parent and fails on the diff has shown that the code changed behaviour, and most of the time changing behaviour is the entire point of the diff. A renamed field, a new default, a tightened validation rule, a corrected error message: each one goes red for the same reason a bug does. The probe was also written by a model that read the same diff, so it can encode a wrong expectation of the new behaviour, pass on the parent, and look identical to a real catch. The remaining two outcomes carry even less: a probe that fails on both trees is testing the environment, and one that passes on both noticed nothing. Most generated probes land in one of those three cells, so the raw catch count says very little about how many bugs are in the diff.

That is why the design is a funnel rather than a detector. Meta runs two workflows, **dodgy diff** (aim at every change) and **intent-aware** (infer the goal, then aim at the ways it could fail), pools their catches, and drops nearly everything through two filters before an engineer sees one. The filters are what make the numbers work: 60% of the catches that survive to an engineer were confirmed true positives in H1 2026, and the human is asked one question, was this behaviour change intended. The funnel has its own failure mode, though. Intent-aware can infer the goal wrongly, build mutants the diff already passes, kill every probe it generated, and report a clean run with no signal that anything went wrong. The cards below carry that picture end to end: the two workflows, the comparison, the filter funnel and the operating rules. Card E is the one to read first if you only read one, because it shows what happens to a generated test.

{::nomarkdown}
<style>
.jit-cards { display: grid; grid-template-columns: 1fr; gap: var(--space-md); margin: var(--space-xl) 0; }
.jit-card { background: var(--surface); border: 1px solid var(--border-color); border-radius: 8px; overflow: hidden; }
.jit-card-head { display: flex; align-items: baseline; gap: var(--space-sm); padding: 10px var(--space-md); background: var(--tag-bg-light); border-bottom: 1px solid var(--border-color); }
.jit-id { font-family: var(--mono); font-size: 12px; letter-spacing: 2px; color: var(--brand); }
.jit-card-head h3 { margin: 0; padding: 0; font-family: var(--serif); font-size: 1.0625rem; font-weight: 500; color: var(--text); }
.jit-card-body { padding: var(--space-md); font-size: 0.9375rem; }
.jit-card-body > * + * { margin-top: 12px; }
.jit-card-body > :last-child { margin-bottom: 0; }
.jit-card-body p, .jit-card-body ul, .jit-card-body ol { margin-bottom: 0; }
.jit-card-body ul, .jit-card-body ol { padding-left: var(--space-lg); }
.jit-card-body figure { margin: 0; }
.jit-card-body table { background: var(--bg); font-size: 0.875rem; margin-bottom: 0; }
.jit-caption { margin-top: var(--space-sm); font-size: 0.875rem; color: var(--olive); }
.jit-filters { display: flex; flex-wrap: wrap; gap: var(--space-sm); }
.jit-filter { flex: 1 1 220px; padding: 10px 12px; border: 1px solid var(--border-color); border-radius: 6px; background: var(--bg); }
.jit-filter b { display: block; font-family: var(--mono); font-size: 12px; letter-spacing: 1px; color: var(--brand); margin-bottom: 4px; }
.jit-filter span { font-size: 0.875rem; color: var(--text-secondary); }
.jit-ok { color: var(--brand); font-weight: 500; }
.jit-no { color: var(--stone); }
.jit-warn { color: var(--olive); font-weight: 500; }
</style>

<div class="jit-cards">

<section class="jit-card" id="jit-a">
<header class="jit-card-head"><span class="jit-id">A</span><h3>Two workflows, one kind of test</h3></header>
<div class="jit-card-body">
<p>The LLM writes the same artifact in both cases: a test that passes on the parent commit and fails on the changed code.</p>
<p>What changes is the target. The dodgy diff already has a bug to aim at, because it treats the diff as the bug. Intent-aware has to invent the bug from an inferred goal.</p>
<p>So the answer to "two types of tests" is no. One test type, two ways to aim it.</p>
</div>
</section>

<section class="jit-card" id="jit-b">
<header class="jit-card-head"><span class="jit-id">B</span><h3>Dodgy diff, anomaly-driven</h3></header>
<div class="jit-card-body">
<ul>
<li>No intent pass and no risk model, so it is the cheaper workflow.</li>
<li>It aims at every behaviour change the diff makes, intended or not.</li>
<li>It cannot tell a deliberate behaviour change from a defect, so it produces many catches that a human would reject.</li>
<li>Its job is recall. It fills the pool of weak catches, on the bet that a real bug sits in there.</li>
</ul>
</div>
</section>

<section class="jit-card" id="jit-c">
<header class="jit-card-head"><span class="jit-id">C</span><h3>Intent-aware, hypothesis-driven</h3></header>
<div class="jit-card-body">
<ul>
<li>The LLM reads the diff, its title and its summary, then states what the change was trying to do.</li>
<li>It lists the ways that goal could be implemented wrongly. Each risk becomes one synthetic mutant.</li>
<li>Tests must kill those mutants, which means pass on the parent and fail on the mutant.</li>
<li>Behaviour that contradicts the author's own stated goal is strong evidence of a bug. That is the point of the extra pass.</li>
</ul>
</div>
</section>

<section class="jit-card" id="jit-d">
<header class="jit-card-head"><span class="jit-id">D</span><h3>Mental model and where they differ</h3></header>
<div class="jit-card-body">
<ul>
<li>Dodgy diff is anomaly-driven testing: differential, mechanical, high recall, no understanding. The closest classical relative is a regression or differential test generator, or a fuzzer aimed at the diff.</li>
<li>Intent-aware is hypothesis-driven testing: infer the goal, enumerate the ways it fails, assert against those.</li>
</ul>
<table>
<thead>
<tr><th>Dimension</th><th>Dodgy diff</th><th>Intent-aware</th></tr>
</thead>
<tbody>
<tr><td>Assumption</td><td>The diff is faulty</td><td>The diff has a goal, met badly</td></tr>
<tr><td>Target bug</td><td>The diff itself</td><td>One mutant per inferred risk</td></tr>
<tr><td>Extra input</td><td>None</td><td>Title, summary, inferred intent</td></tr>
<tr><td>Recall</td><td>Higher, it sees every change</td><td>Lower, it sees only inferred risks</td></tr>
<tr><td>Precision before filtering</td><td>Lower, most behaviour changes are intended</td><td>Higher, each catch maps to a named risk</td></tr>
<tr><td>Cost</td><td>Lower</td><td>One extra LLM pass</td></tr>
</tbody>
</table>
</div>
</section>

<section class="jit-card" id="jit-e">
<header class="jit-card-head"><span class="jit-id">E</span><h3>What happens to the tests</h3></header>
<div class="jit-card-body">
<p>"Diff" means the changed code, the version that would merge. "Parent" is the code before the change. A useful probe passes on the parent and fails on the diff.</p>

<figure>
<svg viewBox="0 0 944 420" xmlns="http://www.w3.org/2000/svg" role="img" style="width:100%;height:auto" font-family="-apple-system,BlinkMacSystemFont,'Segoe UI',Roboto,Helvetica,Arial,sans-serif">
  <title>Two aims, one funnel</title>
  <desc>Dodgy diff and intent-aware probes converge into one useful probe, which passes on the parent and fails on the diff. Every other outcome is dropped. Two filters then rank the survivors, and the engineer confirms whether the behaviour change was intended.</desc>
  <defs>
    <pattern id="jitdots" width="22" height="22" patternUnits="userSpaceOnUse">
      <circle cx="1" cy="1" r="0.9" fill="#E3E2DC"/>
    </pattern>
  </defs>
  <rect width="944" height="420" fill="#f5f4ed"/>
  <rect width="944" height="420" fill="url(#jitdots)" opacity="0.55"/>

  <text x="80" y="40" fill="#1B365D" font-size="14" font-weight="600" font-family="'JetBrains Mono','SF Mono','Fira Code',Consolas,Monaco,monospace" letter-spacing="3">FIGURE  1</text>
  <text x="200" y="40" fill="#504e49" font-size="14" font-family="'JetBrains Mono','SF Mono','Fira Code',Consolas,Monaco,monospace" letter-spacing="3">TWO AIMS, ONE FUNNEL</text>
  <line x1="80" y1="52" x2="864" y2="52" stroke="#1B365D" stroke-width="0.8"/>

  <!-- Aims -->
  <rect x="80" y="96" width="160" height="64" rx="6" fill="#faf9f5" stroke="#141413" stroke-width="1.5"/>
  <text x="160" y="126" text-anchor="middle" font-size="18" font-weight="600" fill="#141413">Dodgy diff</text>
  <text x="160" y="146" text-anchor="middle" font-size="14" fill="#6b6a64">aims at the whole diff</text>

  <rect x="80" y="224" width="160" height="64" rx="6" fill="#faf9f5" stroke="#141413" stroke-width="1.5"/>
  <text x="160" y="254" text-anchor="middle" font-size="18" font-weight="600" fill="#141413">Intent-aware</text>
  <text x="160" y="274" text-anchor="middle" font-size="14" fill="#6b6a64">aims at inferred risks</text>

  <!-- Useful probe (focal) -->
  <rect x="288" y="160" width="160" height="64" rx="6" fill="#EEF2F7" stroke="#1B365D" stroke-width="1.5"/>
  <text x="368" y="190" text-anchor="middle" font-size="18" font-weight="600" fill="#141413">Useful probe</text>
  <text x="368" y="210" text-anchor="middle" font-size="14" fill="#1B365D">parent pass, diff fail</text>

  <!-- Filters -->
  <rect x="496" y="160" width="160" height="64" rx="6" fill="#faf9f5" stroke="#141413" stroke-width="1.5"/>
  <text x="576" y="190" text-anchor="middle" font-size="18" font-weight="600" fill="#141413">Two filters</text>
  <text x="576" y="210" text-anchor="middle" font-size="14" fill="#6b6a64">Judge, then RubFake</text>

  <!-- Engineer -->
  <rect x="704" y="160" width="160" height="64" rx="6" fill="#faf9f5" stroke="#141413" stroke-width="1.5"/>
  <text x="784" y="190" text-anchor="middle" font-size="18" font-weight="600" fill="#141413">Engineer</text>
  <text x="784" y="210" text-anchor="middle" font-size="14" fill="#6b6a64">confirms intent</text>

  <!-- Dropped -->
  <rect x="272" y="296" width="192" height="64" rx="6" fill="#faf9f5" stroke="#6b6a64" stroke-width="1.5" stroke-dasharray="6 4"/>
  <text x="368" y="326" text-anchor="middle" font-size="18" font-weight="600" fill="#6b6a64">Not a catch</text>
  <text x="368" y="346" text-anchor="middle" font-size="14" fill="#6b6a64">no diff, flaky, new behaviour</text>

  <!-- Arrows -->
  <path d="M 240 128 H 264 V 192 H 288" fill="none" stroke="#504e49" stroke-width="1.5"/>
  <path d="M 282 188 L 288 192 L 282 196" fill="none" stroke="#504e49" stroke-width="1.5" stroke-linecap="round" stroke-linejoin="round"/>
  <path d="M 240 256 H 264 V 192 H 288" fill="none" stroke="#504e49" stroke-width="1.5"/>
  <path d="M 282 188 L 288 192 L 282 196" fill="none" stroke="#504e49" stroke-width="1.5" stroke-linecap="round" stroke-linejoin="round"/>

  <path d="M 448 192 H 496" fill="none" stroke="#1B365D" stroke-width="1.5"/>
  <path d="M 490 188 L 496 192 L 490 196" fill="none" stroke="#1B365D" stroke-width="1.5" stroke-linecap="round" stroke-linejoin="round"/>

  <path d="M 656 192 H 704" fill="none" stroke="#504e49" stroke-width="1.5"/>
  <path d="M 698 188 L 704 192 L 698 196" fill="none" stroke="#504e49" stroke-width="1.5" stroke-linecap="round" stroke-linejoin="round"/>

  <path d="M 368 224 V 296" fill="none" stroke="#6b6a64" stroke-width="1.5"/>
  <path d="M 364 290 L 368 296 L 372 290" fill="none" stroke="#6b6a64" stroke-width="1.5" stroke-linecap="round" stroke-linejoin="round"/>
</svg>
</figure>
<p class="jit-caption">Only the top branch earns an alert. The funnel, not the generator, is what turns a flood of behaviour differences into a 60% true-positive rate at the engineer's desk.</p>

<div class="jit-filters">
<div class="jit-filter"><b>LLM-as-Judge</b><span>An ensemble of several strong LLMs, each classifying a catch as a likely true or false positive. Reads meaning, repeats itself poorly.</span></div>
<div class="jit-filter"><b>RubFake</b><span>Rule-Based False-Positive Killing Environment: fixed rules with verifiable output. Narrow, but deterministic.</span></div>
</div>

<table>
<thead>
<tr><th>Parent</th><th>Diff</th><th>What it means</th><th>Kept</th></tr>
</thead>
<tbody>
<tr><td>pass</td><td>fail</td><td>The change broke behaviour the parent had</td><td><span class="jit-ok">weak catch</span></td></tr>
<tr><td>pass</td><td>pass</td><td>No behavioural difference found</td><td><span class="jit-no">dropped</span></td></tr>
<tr><td>fail</td><td>fail</td><td>The test is flaky or environment-dependent</td><td><span class="jit-no">dropped</span></td></tr>
<tr><td>fail</td><td>pass</td><td>The change adds behaviour the parent lacked</td><td><span class="jit-warn">not bug evidence alone</span></td></tr>
</tbody>
</table>

<ul>
<li>The red result is the evidence. It arrives on the merge candidate, before the change lands, so no engineer ever sees a red suite.</li>
<li>These probes never join the repository suite. Their job ends when the engineer confirms the bug or declares the change intended.</li>
<li>Of the catches that reach an engineer, 60% are confirmed true positives. Meta gives one figure for both workflows.</li>
</ul>
</div>
</section>

<section class="jit-card" id="jit-f">
<header class="jit-card-head"><span class="jit-id">F</span><h3>How to run it</h3></header>
<div class="jit-card-body">
<ol>
<li>Run at night on spare capacity, over the previous day's diffs.</li>
<li>Rank the diffs first with a risk score. Only risky diffs get tests.</li>
<li>Run both workflows on the same diff. Pool the weak catches.</li>
<li>Filter with LLM-as-Judge and RubFake. Reach out only above a threshold.</li>
<li>Watch the true-positive rate in aggregate. Tune thresholds, not single catches.</li>
</ol>
<p>This is human-on-the-loop. The engineer usually confirms one thing: was this behaviour change intended?</p>
</div>
</section>

<section class="jit-card" id="jit-g">
<header class="jit-card-head"><span class="jit-id">G</span><h3>What it depends on</h3></header>
<div class="jit-card-body">
<p>The points below are my reading, not claims from Meta.</p>
<ul>
<li>Weak intent inference gives mutants that the diff already passes. The run ends with no catch and no signal, and nobody knows the intent was wrong.</li>
<li>RubFake is deterministic and cheap to trust. The LLM judges drift, so their thresholds need watching.</li>
<li>Two workflows cost double the compute. Risk-based targeting, and spare overnight capacity, are what keep it affordable.</li>
</ul>
</div>
</section>

</div>
{:/nomarkdown}

[Catching Bugs Before They Land (Meta Engineering)](https://engineering.fb.com/2026/10/29/security/catching-bugs-before-they-land-jittestings-human-on-the-loop-approach-at-full-scale/) · [Engineering at Meta](https://engineering.fb.com/)
