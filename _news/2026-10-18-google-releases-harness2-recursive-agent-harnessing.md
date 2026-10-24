---
layout: post
title: "Google Releases Harness² for Recursive Agent Harnessing"
date: 2026-10-18
categories: news
---

Google Research has open-sourced **Harness²**, a framework that improves an agent by rewriting the system around it rather than the model weights. A harness covers the system prompt, guardrails, memory, tool guidance, subagents, skills, scripts, and plugins, all of which Harness² exposes as editable components for a given task. Its task-level recursion runs a **Propose–Probe–Compose** step: the agent proposes edits, probes their effect on the same task, and composes a refined harness from behavioral contrasts between runs, with no ground-truth answer and no external judge. A second, harnessing-level loop keeps a persistent improvement harness with adopted harnesses and evidence-supported lessons, so the next round starts from what the previous runs proved, in either parallel or sequential mode. Across three professional benchmarks on GLM-5 and Gemini 3.5 Flash, one step lifted results by up to **11.5 points** on WorkBuddy-Bench, **4.5 points** on JobBench, and **11.8 percentage points** on LAB's partial pass rate, with larger recursion budgets adding further gains; executors supported are **OpenCode and Codex**.

**Related:** [Stanford + MIT: Meta-Harness Optimization Boosts LLM Performance Up to 6x](/news/2026/09/13/stanford-mit-meta-harness-optimization-boosts-llm-performance/), [Hugging Face Releases Multi-Harness RL for Training Models Inside Coding Agents](/news/2026/10/07/hugging-face-multi-harness-rl/), [Databricks Open-Sources Omnigent, a Meta-Harness for AI Agents](/news/2026/06/16/databricks-omnigent/)

[Harness² on GitHub](https://github.com/google-research/harness2) · [Paper (arXiv)](https://arxiv.org/pdf/2610.14224) · [Project site](https://harness2.github.io)
