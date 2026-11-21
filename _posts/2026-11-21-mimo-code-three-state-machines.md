---
layout: post
title: "MiMo Code: Three State Machines Behind a Long Task"
date: 2026-11-21
categories: ai
tags: [ai-agents, context-management, memory, orchestration, diagrams]
---

Xiaomi open-sourced MiMo Code under MIT on 2026-06-10: a terminal coding agent built on OpenCode, aimed at tasks that run for hundreds of steps. In its own double-blind test, 1,213 paired runs across 576 developers and 474 repositories, the win rate against Claude Code sat near 50% below 200 steps and rose above 65% past it. The article explains that gap with three time scales: computation inside a turn, memory across turns, evolution across sessions.

Drawn out, each time scale is a small state machine, and in each one the runtime takes a decision away from the model doing the work: the judge takes the plan choice, the verifier takes the stopping call, the writer takes the memory, and two maintenance agents take the pruning.

## Computation: one turn, four exits

Max Mode samples five plans in parallel at temperature 1 and never executes them; the same model judges the plans and one gets to run. That buys 10-20% on SWE-Bench Pro for four to five times the tokens. The Goal verifier is the cheaper lever and the one that decides how a turn ends: a separate call with the same conversation and the real tool outputs, no part in the work, so it has no interest in agreeing that the task is finished.

{::nomarkdown}
<figure>
<svg xmlns="http://www.w3.org/2000/svg" viewBox="0 0 960 600" role="img" style="width:100%;height:auto" font-family="-apple-system,BlinkMacSystemFont,'Segoe UI',Roboto,Helvetica,Arial,sans-serif">
  <title>Turn states and the four exits</title>
  <desc>The turn runs from assembled input through five sampled plans and a judge to execution. Each exit attempt goes to an independent verifier that either returns a gap, passes the task, marks it impossible, or the loop cap exits automatically.</desc>
  <defs>
    <pattern id="dots1" width="22" height="22" patternUnits="userSpaceOnUse">
      <circle cx="1" cy="1" r="0.9" fill="#E3E2DC"/>
    </pattern>
  </defs>
  <rect width="960" height="600" fill="#f5f4ed"/>
  <rect width="960" height="600" fill="url(#dots1)" opacity="0.55"/>

  <text x="80" y="40" fill="#1B365D" font-size="14" font-weight="600" font-family="'JetBrains Mono','SF Mono','Fira Code',Consolas,Monaco,monospace" letter-spacing="3">FIGURE  1</text>
  <text x="200" y="40" fill="#504e49" font-size="14" font-family="'JetBrains Mono','SF Mono','Fira Code',Consolas,Monaco,monospace" letter-spacing="3">TURN STATES, FOUR EXITS</text>
  <line x1="80" y1="52" x2="880" y2="52" stroke="#1B365D" stroke-width="0.8"/>

  <rect x="80" y="200" width="144" height="64" rx="6" fill="#faf9f5" stroke="#141413" stroke-width="1.5"/>
  <text x="152" y="230" text-anchor="middle" font-size="18" font-weight="600" fill="#141413">Turn Input</text>
  <text x="152" y="250" text-anchor="middle" font-size="14" fill="#6b6a64">window + files</text>

  <rect x="272" y="200" width="144" height="64" rx="6" fill="#EEF2F7" stroke="#1B365D" stroke-width="1.5"/>
  <text x="344" y="230" text-anchor="middle" font-size="18" font-weight="600" fill="#141413">5 Candidates</text>
  <text x="344" y="250" text-anchor="middle" font-size="14" fill="#6b6a64">plan only, T=1</text>

  <rect x="464" y="200" width="144" height="64" rx="6" fill="#faf9f5" stroke="#141413" stroke-width="1.5"/>
  <text x="536" y="230" text-anchor="middle" font-size="18" font-weight="600" fill="#141413">Judge</text>
  <text x="536" y="250" text-anchor="middle" font-size="14" fill="#6b6a64">picks one plan</text>

  <rect x="656" y="200" width="144" height="64" rx="6" fill="#faf9f5" stroke="#141413" stroke-width="1.5"/>
  <text x="728" y="230" text-anchor="middle" font-size="18" font-weight="600" fill="#141413">Execute</text>
  <text x="728" y="250" text-anchor="middle" font-size="14" fill="#6b6a64">tool calls run</text>

  <path d="M 224 232 H 272" fill="none" stroke="#1B365D" stroke-width="1.5"/>
  <path d="M 266 228 L 272 232 L 266 236" fill="none" stroke="#1B365D" stroke-width="1.5" stroke-linecap="round" stroke-linejoin="round"/>
  <path d="M 416 232 H 464" fill="none" stroke="#1B365D" stroke-width="1.5"/>
  <path d="M 458 228 L 464 232 L 458 236" fill="none" stroke="#1B365D" stroke-width="1.5" stroke-linecap="round" stroke-linejoin="round"/>
  <path d="M 608 232 H 656" fill="none" stroke="#1B365D" stroke-width="1.5"/>
  <path d="M 650 228 L 656 232 L 650 236" fill="none" stroke="#1B365D" stroke-width="1.5" stroke-linecap="round" stroke-linejoin="round"/>

  <path d="M 728 200 V 120 H 152 V 200" fill="none" stroke="#1B365D" stroke-width="1.5"/>
  <path d="M 148 194 L 152 200 L 156 194" fill="none" stroke="#1B365D" stroke-width="1.5" stroke-linecap="round" stroke-linejoin="round"/>
  <rect x="392" y="106" width="96" height="24" fill="#f5f4ed"/>
  <text x="440" y="124" text-anchor="middle" font-size="14" fill="#1B365D">next turn</text>

  <rect x="656" y="368" width="160" height="64" rx="6" fill="#EEF2F7" stroke="#1B365D" stroke-width="1.5"/>
  <text x="736" y="398" text-anchor="middle" font-size="18" font-weight="600" fill="#141413">Goal Check</text>
  <text x="736" y="418" text-anchor="middle" font-size="14" fill="#6b6a64">same context, no work</text>

  <path d="M 728 264 V 368" fill="none" stroke="#504e49" stroke-width="1.5"/>
  <path d="M 724 362 L 728 368 L 732 362" fill="none" stroke="#504e49" stroke-width="1.5" stroke-linecap="round" stroke-linejoin="round"/>
  <text x="740" y="322" font-size="14" fill="#504e49">exit attempt</text>

  <rect x="80" y="520" width="144" height="64" rx="6" fill="#faf9f5" stroke="#141413" stroke-width="1.5"/>
  <text x="152" y="550" text-anchor="middle" font-size="18" font-weight="600" fill="#141413">Continue</text>
  <text x="152" y="570" text-anchor="middle" font-size="14" fill="#6b6a64">gap fed back</text>

  <rect x="272" y="520" width="144" height="64" rx="6" fill="#faf9f5" stroke="#141413" stroke-width="1.5"/>
  <text x="344" y="550" text-anchor="middle" font-size="18" font-weight="600" fill="#141413">Done</text>
  <text x="344" y="570" text-anchor="middle" font-size="14" fill="#6b6a64">condition met</text>

  <rect x="464" y="520" width="144" height="64" rx="6" fill="#faf9f5" stroke="#141413" stroke-width="1.5"/>
  <text x="536" y="550" text-anchor="middle" font-size="18" font-weight="600" fill="#141413">Impossible</text>
  <text x="536" y="570" text-anchor="middle" font-size="14" fill="#6b6a64">marked, not looped</text>

  <rect x="656" y="520" width="144" height="64" rx="6" fill="#faf9f5" stroke="#141413" stroke-width="1.5"/>
  <text x="728" y="550" text-anchor="middle" font-size="18" font-weight="600" fill="#141413">Cap Exit</text>
  <text x="728" y="570" text-anchor="middle" font-size="14" fill="#6b6a64">loop under 0.5%</text>

  <path d="M 736 432 V 480 H 152" fill="none" stroke="#504e49" stroke-width="1.5"/>
  <path d="M 152 480 V 520" fill="none" stroke="#504e49" stroke-width="1.5"/>
  <path d="M 148 514 L 152 520 L 156 514" fill="none" stroke="#504e49" stroke-width="1.5" stroke-linecap="round" stroke-linejoin="round"/>
  <path d="M 344 480 V 520" fill="none" stroke="#504e49" stroke-width="1.5"/>
  <path d="M 340 514 L 344 520 L 348 514" fill="none" stroke="#504e49" stroke-width="1.5" stroke-linecap="round" stroke-linejoin="round"/>
  <path d="M 536 480 V 520" fill="none" stroke="#504e49" stroke-width="1.5"/>
  <path d="M 532 514 L 536 520 L 540 514" fill="none" stroke="#504e49" stroke-width="1.5" stroke-linecap="round" stroke-linejoin="round"/>
  <path d="M 728 480 V 520" fill="none" stroke="#504e49" stroke-width="1.5"/>
  <path d="M 724 514 L 728 520 L 732 514" fill="none" stroke="#504e49" stroke-width="1.5" stroke-linecap="round" stroke-linejoin="round"/>

  <path d="M 80 552 H 48 V 232 H 80" fill="none" stroke="#1B365D" stroke-width="1.5"/>
  <path d="M 74 228 L 80 232 L 74 236" fill="none" stroke="#1B365D" stroke-width="1.5" stroke-linecap="round" stroke-linejoin="round"/>
</svg>
</figure>
{:/nomarkdown}

Every exit but one costs a verifier call, and the exception is a counter in the runtime, not a judgement. The verifier is the only state in the graph that can refuse the agent's answer, which is why it holds the same context and none of the work. Two further compute levers do not change the graph: the constrained command syntax is design intent, not migrated, and Dynamic Workflow moves orchestration out of the prompt into a sandboxed script whose results land on disk after every `agent()` call.

## Memory: the window is a disposable unit

Checkpoints fire at roughly 20, 45 and 70% of the budget, not at 95%. Extraction needs room to read, hold an interpretation and write, and the middle of a long context is where attention is weakest, so the compression cannot be scheduled for the moment the window is nearly gone. The main agent does not maintain its own state: a separate writer does it, and the only thing the main agent may write is `notes.md`, which the writer drains at each checkpoint. Underneath the structured files sits the raw SQLite trace, the fallback when a detail was dropped rather than compressed.

{::nomarkdown}
<figure>
<svg xmlns="http://www.w3.org/2000/svg" viewBox="0 0 960 640" role="img" style="width:100%;height:auto" font-family="-apple-system,BlinkMacSystemFont,'Segoe UI',Roboto,Helvetica,Arial,sans-serif">
  <title>Window lifecycle and memory layers</title>
  <desc>A window moves from working to a checkpoint, written by a separate writer into session memory, then continues until a rebuild opens the next window seeded from the files. Session memory is promoted to project memory, then global memory, with the raw history below as the fallback.</desc>
  <defs>
    <pattern id="dots2" width="22" height="22" patternUnits="userSpaceOnUse">
      <circle cx="1" cy="1" r="0.9" fill="#E3E2DC"/>
    </pattern>
  </defs>
  <rect width="960" height="640" fill="#f5f4ed"/>
  <rect width="960" height="640" fill="url(#dots2)" opacity="0.55"/>

  <text x="80" y="40" fill="#1B365D" font-size="14" font-weight="600" font-family="'JetBrains Mono','SF Mono','Fira Code',Consolas,Monaco,monospace" letter-spacing="3">FIGURE  2</text>
  <text x="200" y="40" fill="#504e49" font-size="14" font-family="'JetBrains Mono','SF Mono','Fira Code',Consolas,Monaco,monospace" letter-spacing="3">WINDOW LIFECYCLE AND MEMORY LAYERS</text>
  <line x1="80" y1="52" x2="880" y2="52" stroke="#1B365D" stroke-width="0.8"/>

  <rect x="80" y="120" width="160" height="64" rx="6" fill="#faf9f5" stroke="#141413" stroke-width="1.5"/>
  <text x="160" y="150" text-anchor="middle" font-size="18" font-weight="600" fill="#141413">Working</text>
  <text x="160" y="170" text-anchor="middle" font-size="14" fill="#6b6a64">turns accumulate</text>

  <rect x="80" y="256" width="160" height="64" rx="6" fill="#EEF2F7" stroke="#1B365D" stroke-width="1.5"/>
  <text x="160" y="286" text-anchor="middle" font-size="18" font-weight="600" fill="#141413">Checkpoint</text>
  <text x="160" y="306" text-anchor="middle" font-size="14" fill="#6b6a64">at 20 / 45 / 70%</text>

  <rect x="80" y="392" width="160" height="64" rx="6" fill="#EEF2F7" stroke="#1B365D" stroke-width="1.5"/>
  <text x="160" y="422" text-anchor="middle" font-size="18" font-weight="600" fill="#141413">Rebuild</text>
  <text x="160" y="442" text-anchor="middle" font-size="14" fill="#6b6a64">cuts the window</text>

  <rect x="80" y="528" width="160" height="64" rx="6" fill="#faf9f5" stroke="#141413" stroke-width="1.5"/>
  <text x="160" y="558" text-anchor="middle" font-size="18" font-weight="600" fill="#141413">Next Window</text>
  <text x="160" y="578" text-anchor="middle" font-size="14" fill="#6b6a64">seeded from files</text>

  <path d="M 160 184 V 256" fill="none" stroke="#504e49" stroke-width="1.5"/>
  <path d="M 156 250 L 160 256 L 164 250" fill="none" stroke="#504e49" stroke-width="1.5" stroke-linecap="round" stroke-linejoin="round"/>
  <path d="M 160 320 V 392" fill="none" stroke="#504e49" stroke-width="1.5"/>
  <path d="M 156 386 L 160 392 L 164 386" fill="none" stroke="#504e49" stroke-width="1.5" stroke-linecap="round" stroke-linejoin="round"/>
  <path d="M 160 456 V 528" fill="none" stroke="#504e49" stroke-width="1.5"/>
  <path d="M 156 522 L 160 528 L 164 522" fill="none" stroke="#504e49" stroke-width="1.5" stroke-linecap="round" stroke-linejoin="round"/>

  <rect x="400" y="256" width="160" height="64" rx="6" fill="#faf9f5" stroke="#141413" stroke-width="1.5"/>
  <text x="480" y="286" text-anchor="middle" font-size="18" font-weight="600" fill="#141413">Writer</text>
  <text x="480" y="306" text-anchor="middle" font-size="14" fill="#6b6a64">separate subagent</text>

  <path d="M 240 288 H 400" fill="none" stroke="#1B365D" stroke-width="1.5"/>
  <path d="M 394 284 L 400 288 L 394 292" fill="none" stroke="#1B365D" stroke-width="1.5" stroke-linecap="round" stroke-linejoin="round"/>

  <rect x="680" y="256" width="160" height="64" rx="6" fill="#faf9f5" stroke="#141413" stroke-width="1.5"/>
  <text x="760" y="286" text-anchor="middle" font-size="18" font-weight="600" fill="#141413">Session</text>
  <text x="760" y="306" text-anchor="middle" font-size="14" fill="#6b6a64">checkpoint.md</text>

  <rect x="680" y="344" width="160" height="64" rx="6" fill="#faf9f5" stroke="#141413" stroke-width="1.5"/>
  <text x="760" y="374" text-anchor="middle" font-size="18" font-weight="600" fill="#141413">Project</text>
  <text x="760" y="394" text-anchor="middle" font-size="14" fill="#6b6a64">MEMORY.md</text>

  <rect x="680" y="432" width="160" height="64" rx="6" fill="#faf9f5" stroke="#141413" stroke-width="1.5"/>
  <text x="760" y="462" text-anchor="middle" font-size="18" font-weight="600" fill="#141413">Global</text>
  <text x="760" y="482" text-anchor="middle" font-size="14" fill="#6b6a64">user preferences</text>

  <rect x="680" y="520" width="160" height="64" rx="6" fill="#faf9f5" stroke="#141413" stroke-width="1.5"/>
  <text x="760" y="550" text-anchor="middle" font-size="18" font-weight="600" fill="#141413">History</text>
  <text x="760" y="570" text-anchor="middle" font-size="14" fill="#6b6a64">SQLite trace</text>

  <path d="M 560 288 H 680" fill="none" stroke="#1B365D" stroke-width="1.5"/>
  <path d="M 674 284 L 680 288 L 674 292" fill="none" stroke="#1B365D" stroke-width="1.5" stroke-linecap="round" stroke-linejoin="round"/>
  <path d="M 760 320 V 344" fill="none" stroke="#504e49" stroke-width="1.5"/>
  <path d="M 756 338 L 760 344 L 764 338" fill="none" stroke="#504e49" stroke-width="1.5" stroke-linecap="round" stroke-linejoin="round"/>
  <path d="M 760 408 V 432" fill="none" stroke="#504e49" stroke-width="1.5"/>
  <path d="M 756 426 L 760 432 L 764 426" fill="none" stroke="#504e49" stroke-width="1.5" stroke-linecap="round" stroke-linejoin="round"/>
  <path d="M 760 496 V 520" fill="none" stroke="#504e49" stroke-width="1.5"/>
  <path d="M 756 514 L 760 520 L 764 514" fill="none" stroke="#504e49" stroke-width="1.5" stroke-linecap="round" stroke-linejoin="round"/>

  <text x="80" y="620" font-size="14" fill="#6b6a64">a cycle: checkpointed turns ending in one rebuild, no upper bound on cycles</text>
</svg>
</figure>
{:/nomarkdown}

Everything else in this figure follows from one split: the writer never has to solve the task. The percentages, the eleven fields and the 65K rebuild budget are tuning around that split, not independent ideas. It also fixes the failure mode: the agent debugging a bad test cannot quietly downgrade the record of what it was asked to do.

## Evolution: two clocks, two outputs

Dream runs every 7 days over the sessions and the existing memory file: merge, deduplicate, check that referenced paths still exist, keep what is current. Distill runs every 30 days over the same history but looks for process instead of knowledge, and writes what it finds into skills, CLI commands, custom agents and SOP documents.

{::nomarkdown}
<figure>
<svg xmlns="http://www.w3.org/2000/svg" viewBox="0 0 960 520" role="img" style="width:100%;height:auto" font-family="-apple-system,BlinkMacSystemFont,'Segoe UI',Roboto,Helvetica,Arial,sans-serif">
  <title>Two maintenance clocks</title>
  <desc>Session history feeds both maintenance agents. Dream runs every seven days and produces a compacted memory file. Distill runs every thirty days and produces reusable artifacts.</desc>
  <defs>
    <pattern id="dots3" width="22" height="22" patternUnits="userSpaceOnUse">
      <circle cx="1" cy="1" r="0.9" fill="#E3E2DC"/>
    </pattern>
  </defs>
  <rect width="960" height="520" fill="#f5f4ed"/>
  <rect width="960" height="520" fill="url(#dots3)" opacity="0.55"/>

  <text x="80" y="40" fill="#1B365D" font-size="14" font-weight="600" font-family="'JetBrains Mono','SF Mono','Fira Code',Consolas,Monaco,monospace" letter-spacing="3">FIGURE  3</text>
  <text x="200" y="40" fill="#504e49" font-size="14" font-family="'JetBrains Mono','SF Mono','Fira Code',Consolas,Monaco,monospace" letter-spacing="3">TWO MAINTENANCE CLOCKS</text>
  <line x1="80" y1="52" x2="880" y2="52" stroke="#1B365D" stroke-width="0.8"/>

  <rect x="80" y="224" width="160" height="64" rx="6" fill="#faf9f5" stroke="#141413" stroke-width="1.5"/>
  <text x="160" y="254" text-anchor="middle" font-size="18" font-weight="600" fill="#141413">History</text>
  <text x="160" y="274" text-anchor="middle" font-size="14" fill="#6b6a64">raw sessions</text>

  <rect x="300" y="112" width="160" height="64" rx="6" fill="#EEF2F7" stroke="#1B365D" stroke-width="1.5"/>
  <text x="380" y="142" text-anchor="middle" font-size="18" font-weight="600" fill="#141413">Dream</text>
  <text x="380" y="162" text-anchor="middle" font-size="14" fill="#6b6a64">every 7 days</text>

  <rect x="520" y="112" width="160" height="64" rx="6" fill="#faf9f5" stroke="#141413" stroke-width="1.5"/>
  <text x="600" y="142" text-anchor="middle" font-size="18" font-weight="600" fill="#141413">Memory</text>
  <text x="600" y="162" text-anchor="middle" font-size="14" fill="#6b6a64">merged, path-checked</text>

  <rect x="300" y="368" width="160" height="64" rx="6" fill="#EEF2F7" stroke="#1B365D" stroke-width="1.5"/>
  <text x="380" y="398" text-anchor="middle" font-size="18" font-weight="600" fill="#141413">Distill</text>
  <text x="380" y="418" text-anchor="middle" font-size="14" fill="#6b6a64">every 30 days</text>

  <rect x="520" y="368" width="160" height="64" rx="6" fill="#faf9f5" stroke="#141413" stroke-width="1.5"/>
  <text x="600" y="398" text-anchor="middle" font-size="18" font-weight="600" fill="#141413">Artifacts</text>
  <text x="600" y="418" text-anchor="middle" font-size="14" fill="#6b6a64">skills, CLI, SOP</text>

  <path d="M 240 256 H 268 V 144 H 300" fill="none" stroke="#1B365D" stroke-width="1.5"/>
  <path d="M 294 140 L 300 144 L 294 148" fill="none" stroke="#1B365D" stroke-width="1.5" stroke-linecap="round" stroke-linejoin="round"/>
  <path d="M 268 256 V 400 H 300" fill="none" stroke="#1B365D" stroke-width="1.5"/>
  <path d="M 294 396 L 300 400 L 294 404" fill="none" stroke="#1B365D" stroke-width="1.5" stroke-linecap="round" stroke-linejoin="round"/>

  <path d="M 460 144 H 520" fill="none" stroke="#504e49" stroke-width="1.5"/>
  <path d="M 514 140 L 520 144 L 514 148" fill="none" stroke="#504e49" stroke-width="1.5" stroke-linecap="round" stroke-linejoin="round"/>
  <path d="M 460 400 H 520" fill="none" stroke="#504e49" stroke-width="1.5"/>
  <path d="M 514 396 L 520 400 L 514 404" fill="none" stroke="#504e49" stroke-width="1.5" stroke-linecap="round" stroke-linejoin="round"/>
</svg>
</figure>
{:/nomarkdown}

The two outputs are not interchangeable. Dream shrinks what the agent has to read at the start of a session, so it buys back context. Distill removes a step the agent would otherwise reason through, so it saves tokens on a repeated task. Only the second one compounds without needing a bigger window.

The price is five extra model roles outside the task: judge, verifier, writer, dream, distill. The article reports no ablation that isolates any of them, and its offline benchmarks measure one issue at a time, so nothing in the published numbers measures the mechanisms that only fire past 200 steps. Whether five roles stay separate, or collapse into one runtime-owned maintenance process, is the part still open.

[MiMo Code: Scaling Coding Agents to Long-Horizon Tasks](https://mimo.xiaomi.com/blog/mimo-code-long-horizon)
