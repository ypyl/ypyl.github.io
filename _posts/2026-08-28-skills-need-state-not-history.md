---
layout: post
title: "Skills Need State, Not History"
date: 2026-08-28
categories: ai
tags: [llm-agents, agent-runtime, execution-state, procedural-skills, context-management]
---

An agent running a long procedure shouldn't be rereading its own transcript at every step.

**SKILL.state** (Badhe, Tiwari, Chung, 2026) proposes a simple shift: at each execution step, the model receives only three inputs — the **immutable skill specification**, the current **structured execution state**, and the latest **observation**. After producing a validated state update, the **reasoning trace is discarded**. Only the updated state carries forward.

{::nomarkdown}
<figure>
<svg xmlns="http://www.w3.org/2000/svg" viewBox="24 0 916 600" role="img" style="width:100%;height:auto" font-family="-apple-system,BlinkMacSystemFont,'Segoe UI',Roboto,Helvetica,Arial,sans-serif">
  <title>SKILL.state execution cycle</title>
  <desc>At each step the model reads only the skill spec, the execution state, and the latest observation. After a validated state update, the reasoning trace is discarded and only the updated state survives.</desc>
  <defs>
    <pattern id="dots" width="22" height="22" patternUnits="userSpaceOnUse">
      <circle cx="1" cy="1" r="0.9" fill="#E3E2DC"/>
    </pattern>
  </defs>
  <rect width="964" height="600" fill="#f5f4ed"/>
  <rect width="964" height="600" fill="url(#dots)" opacity="0.55"/>

  <text x="80" y="40" fill="#1B365D" font-size="14" font-weight="600" font-family="'JetBrains Mono','SF Mono','Fira Code',Consolas,Monaco,monospace" letter-spacing="3">FIGURE  1</text>
  <text x="200" y="40" fill="#504e49" font-size="14" font-family="'JetBrains Mono','SF Mono','Fira Code',Consolas,Monaco,monospace" letter-spacing="3">SKILL.STATE EXECUTION CYCLE</text>
  <line x1="80" y1="52" x2="884" y2="52" stroke="#1B365D" stroke-width="0.8"/>

  <!-- Input nodes -->
  <rect x="80" y="100" width="160" height="64" rx="6" fill="#faf9f5" stroke="#141413" stroke-width="1.5"/>
  <text x="160" y="130" text-anchor="middle" font-size="18" font-weight="600" fill="#141413">Skill Spec</text>
  <text x="160" y="150" text-anchor="middle" font-size="14" fill="#6b6a64">immutable</text>

  <rect x="80" y="200" width="160" height="64" rx="6" fill="#EEF2F7" stroke="#1B365D" stroke-width="1.5"/>
  <text x="160" y="230" text-anchor="middle" font-size="18" font-weight="600" fill="#141413">Execution State</text>
  <text x="160" y="250" text-anchor="middle" font-size="14" fill="#6b6a64">structured, persists</text>

  <rect x="80" y="300" width="160" height="64" rx="6" fill="#faf9f5" stroke="#141413" stroke-width="1.5"/>
  <text x="160" y="330" text-anchor="middle" font-size="18" font-weight="600" fill="#141413">Observation</text>
  <text x="160" y="350" text-anchor="middle" font-size="14" fill="#6b6a64">latest one only</text>

  <!-- LLM -->
  <rect x="400" y="96" width="160" height="272" rx="6" fill="#faf9f5" stroke="#141413" stroke-width="1.5"/>
  <text x="480" y="216" text-anchor="middle" font-size="18" font-weight="600" fill="#141413">LLM</text>
  <text x="480" y="240" text-anchor="middle" font-size="14" fill="#6b6a64">reads only these three</text>

  <!-- Output nodes -->
  <g opacity="0.65">
    <rect x="700" y="100" width="160" height="64" rx="6" fill="#faf9f5" stroke="#6b6a64" stroke-width="1.5" stroke-dasharray="6 4"/>
    <text x="780" y="130" text-anchor="middle" font-size="18" font-weight="600" fill="#6b6a64" text-decoration="line-through">Reasoning Trace</text>
    <text x="780" y="150" text-anchor="middle" font-size="14" fill="#6b6a64">discarded each step</text>
  </g>

  <rect x="700" y="200" width="160" height="64" rx="6" fill="#faf9f5" stroke="#141413" stroke-width="1.5"/>
  <text x="780" y="230" text-anchor="middle" font-size="18" font-weight="600" fill="#141413">State Patch</text>
  <text x="780" y="250" text-anchor="middle" font-size="14" fill="#6b6a64">validated by runtime</text>

  <rect x="700" y="300" width="160" height="64" rx="6" fill="#faf9f5" stroke="#141413" stroke-width="1.5"/>
  <text x="780" y="330" text-anchor="middle" font-size="18" font-weight="600" fill="#141413">Action</text>
  <text x="780" y="350" text-anchor="middle" font-size="14" fill="#6b6a64">one action</text>

  <rect x="700" y="448" width="160" height="64" rx="6" fill="#faf9f5" stroke="#141413" stroke-width="1.5"/>
  <text x="780" y="478" text-anchor="middle" font-size="18" font-weight="600" fill="#141413">Environment</text>
  <text x="780" y="498" text-anchor="middle" font-size="14" fill="#6b6a64">emits observations</text>

  <!-- Inputs to LLM -->
  <path d="M 240 132 H 400" fill="none" stroke="#504e49" stroke-width="1.5"/>
  <path d="M 394 128 L 400 132 L 394 136" fill="none" stroke="#504e49" stroke-width="1.5" stroke-linecap="round" stroke-linejoin="round"/>
  <path d="M 240 232 H 400" fill="none" stroke="#1B365D" stroke-width="1.5"/>
  <path d="M 394 228 L 400 232 L 394 236" fill="none" stroke="#1B365D" stroke-width="1.5" stroke-linecap="round" stroke-linejoin="round"/>
  <path d="M 240 332 H 400" fill="none" stroke="#504e49" stroke-width="1.5"/>
  <path d="M 394 328 L 400 332 L 394 336" fill="none" stroke="#504e49" stroke-width="1.5" stroke-linecap="round" stroke-linejoin="round"/>

  <!-- LLM to outputs -->
  <path d="M 560 132 H 700" fill="none" stroke="#6b6a64" stroke-width="1.5"/>
  <path d="M 694 128 L 700 132 L 694 136" fill="none" stroke="#6b6a64" stroke-width="1.5" stroke-linecap="round" stroke-linejoin="round"/>
  <path d="M 560 232 H 700" fill="none" stroke="#1B365D" stroke-width="1.5"/>
  <path d="M 694 228 L 700 232 L 694 236" fill="none" stroke="#1B365D" stroke-width="1.5" stroke-linecap="round" stroke-linejoin="round"/>
  <path d="M 560 332 H 700" fill="none" stroke="#504e49" stroke-width="1.5"/>
  <path d="M 694 328 L 700 332 L 694 336" fill="none" stroke="#504e49" stroke-width="1.5" stroke-linecap="round" stroke-linejoin="round"/>

  <!-- Action to environment -->
  <path d="M 780 364 V 442" fill="none" stroke="#504e49" stroke-width="1.5"/>
  <path d="M 776 442 L 780 448 L 784 442" fill="none" stroke="#504e49" stroke-width="1.5" stroke-linecap="round" stroke-linejoin="round"/>

  <!-- Loop 1: validated state update back to Execution State (right channel, enters from the left edge to avoid crossing Observation) -->
  <path d="M 860 232 H 900 V 400 H 64 V 232 H 80" fill="none" stroke="#1B365D" stroke-width="1.5"/>
  <path d="M 74 228 L 80 232 L 74 236" fill="none" stroke="#1B365D" stroke-width="1.5" stroke-linecap="round" stroke-linejoin="round"/>
  <text x="480" y="424" text-anchor="middle" font-size="14" fill="#1B365D">validated update, state survives</text>

  <!-- Loop 2: next observation back to Observation -->
  <path d="M 780 512 V 552 H 160 V 372" fill="none" stroke="#504e49" stroke-width="1.5"/>
  <path d="M 154 372 L 160 364 L 166 372" fill="none" stroke="#504e49" stroke-width="1.5" stroke-linecap="round" stroke-linejoin="round"/>
  <text x="452" y="572" text-anchor="middle" font-size="14" fill="#504e49">next observation</text>
</svg>
</figure>
{:/nomarkdown}

**How to read it**: The three boxes on the left are the whole prompt at every step: the skill file, a small JSON working memory, and the latest event. The right column is what the model gives back: reasoning (dashed, thrown away once the patch validates), a state patch (ink blue, the path that persists, looping back into execution state), and one action (olive) that runs against the environment. Only the arrows that come back are the ones that last.

The result: a bounded prompt footprint and token cost that grows linearly with execution length, not quadratically. No stale observations, no obsolete reasoning, no reconstructing world state from accumulated text.

The constraint: this only works when the structured state is a **sufficient statistic** — everything the future needs must be projectable into it the moment it becomes known. Static schemas, no deferred relevance, output not being the trajectory itself.

When those conditions hold, discard the reasoning and keep the state.

[SKILL.state: Scalable Long-Horizon Agent Skills (arXiv 2608.26263v2)](https://arxiv.org/abs/2608.26263)