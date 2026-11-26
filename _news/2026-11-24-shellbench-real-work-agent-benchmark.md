---
layout: post
title: "ShellBench: the Best Agent Completes 62.5% of Everyday Work Tasks"
date: 2026-11-24
categories: news
---

**ShellBench** tests 100 everyday tasks drawn from patterns in 10 million anonymised agent interactions. The benchmark comes from the OpenClaw Foundation with Hugging Face, Red Hat, UC Berkeley and Parsewave: each task starts with a short request (median 43 words) and a messy workspace (median 188 files), and 90 of the 100 are graded against 721 rubric criteria, 381 of them safety checks. Twelve models across five harnesses produced **53,100 graded runs**.

Even the strongest model finished about six tasks in ten. **GPT-6 Astra** led at **62.5%** at an estimated **$1.10** per run, Claude Fable 5.2 reached 61.9% ($0.83) and GPT-6.1 Sol 61.4% at **$0.19**, so Sol sits within 1.1 points of the top at roughly a sixth of the cost. Reliability lags capability: GPT-6 Astra completed 75.1% of tasks at least once in three attempts and 48.8% on all three.

Safety failures hide inside ordinary work. **15.7%** of all runs broke at least one safety check, and 2,589 of the 26,151 runs that met every correctness criterion (9.9%) still failed one. Model choice moved scores far more than harness choice: the median gap between the best and worst model inside a harness was 45.7 points, against 6.2 points for switching harness. The four open-source harnesses averaged 45.0% to 45.2% against Claude Code's 42.87%.

**Related:** [FrontierML Gives Five Frontier Models 12 Hours of Real ML Research and None Reaches Its Reference](/news/2026/11/20/frontierml-benchmark-five-models/), [Nous Research Publishes the Hermes Index Agent Benchmark](/news/2026/10/08/nous-research-hermes-index-agent-benchmark/)

[Introducing ShellBench](https://openclaw.ai/blog/shellbench) · [ShellBench paper](https://arxiv.org/abs/2611.32946) · [Source channel](https://t.me/vistehno)
