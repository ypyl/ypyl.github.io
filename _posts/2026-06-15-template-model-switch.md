---
layout: post
title: "Per-Prompt Model Switching for Pi Coding Agent"
date: 2026-06-15
tags: [pi, extension, prompt-engineering, open-source]
categories: ai
---

Different prompt templates need different models. `/brainstorm` wants deep reasoning; `/commit` just needs a small model that follows instructions. But pi keeps one model active at a time, so you end up manually switching with `/model` between invocations.

Add a `model:` field to the template's frontmatter and let the tooling handle it:

```markdown
---
description: Brainstorm ideas for a given topic
model: opencode-go/deepseek-v4-pro
---
Brainstorm ideas for: {{"{{topic}}"}}
```

Now `/brainstorm` always runs with a reasoning model. No manual switching, no forgetting to switch back.

The extension is ~150 lines — it hooks pi's `input`, `before_agent_start`, and `agent_end` events to read the template, swap the model for that single execution, then restore. Templates without `model:` are unaffected.

The repo: **[github.com/ypyl/pi-template-model-switch](https://github.com/ypyl/pi-template-model-switch)**. Install with `pi install git:github.com/ypyl/pi-template-model-switch`. MIT.

## Why declarative over manual

Pi already has great model switching — `/model`, `Ctrl+P` cycling. But those are manual. You have to remember which model you wanted for which prompt, and switch back after.

This flips it: the template author declares what the prompt needs, and the tooling enforces it at invocation time. Same principle as shebangs — the metadata drives the behavior, not your memory.

## The ecosystem

Three event hooks and a TypeScript file. That's all it took to add something that feels like a built-in feature. No forking, no upstream PRs — just `~/.pi/agent/extensions/` or a git repo anyone can install.


