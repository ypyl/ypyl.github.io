---
layout: post
title: "Stanford + MIT: Meta-Harness Optimization Boosts LLM Performance Up to 6x"
date: 2026-09-13
tags: news
categories: news
---

A new paper from Stanford and MIT, **Meta-Harness: End-to-End Optimization of Model Harnesses**, demonstrates that the same LLM can produce radically different results depending on the surrounding system code. The harness controls context storage, memory retrieval, data selection, tool invocation, error handling, and workflow design. On a single benchmark, performance varied by up to **6×** between harnesses using the same base model. The authors propose **Meta-Harness**, an external optimization loop that automatically rewrites harness code by reading execution traces and logs through a filesystem-like interface. Results include **+7.7 points** on online text classification with roughly 4× fewer context tokens, **+4.7 points** on retrieval-augmented math reasoning across 5 held-out models and 200 IMO-level problems, and outperformed hand-engineered baselines on TerminalBench-2 for agentic coding. The core finding: the question is no longer just which model is better, but how the entire system around it is built.

**Related:** [Databricks Open-Sources Omnigent, a Meta-Harness for AI Agents](/news/2026/06/16/databricks-omnigent/), [SkillsBench Research Shows Real Impact of Skills on LLM Agents](/news/2026/02/20/skillsbench-research-on-llm-agents-skills/)

[Meta-Harness: End-to-End Optimization of Model Harnesses (arXiv)](https://arxiv.org/abs/2603.28052)
