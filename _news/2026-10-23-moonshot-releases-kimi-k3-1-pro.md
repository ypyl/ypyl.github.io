---
layout: post
title: "Moonshot Releases Kimi K3.1 Pro With 1M Context, Weights Due by November 4"
date: 2026-10-23
tags: [news]
categories: news
---

Moonshot AI has released **Kimi K3.1 Pro**, a 3T-class open-weight model with a **1M-token context window**, built by applying substantially more post-training compute to Kimi K3. A second variant, **K3.1 Pro Highspeed**, runs 2 to 3 times faster. Both are available now on Kimi.ai, in the Kimi app, in Kimi Code and through the API, with full weights promised by **November 4, 2026**.

The results Moonshot reports put it near the top of the open-weight field, and close to Claude Opus 5.5 on agentic coding:

- **Terminal-Bench 4.0**: **65.7%**, ahead of GPT-6 Astra at 58.2% and behind Opus 5.5 at 66.4%.
- **BenchCAD** (3D reconstruction via CAD code, with tools): **92.7%**, against 96.2% for Opus 5.5 and 95.9% for GPT-6 Astra.
- **Financial Research**: 11.6 points above K3. Business simulations reached roughly **4.2 times** K3's score.
- Hallucination rate on an internal hard stress test fell from **23.4% to 8.3%**.

New capabilities include stronger full-project builds in **Unity, Unreal Engine and Godot**, 3D and CAD code generation, and a 3D Make tool on kimi.ai. Moonshot also removed cyber-related data from both mid-training and the RL environment mix to limit misuse; on its internal vulnerability benchmark that cut the solve rate from 90.5% to 81.0% at pass@3, even as the model still improved on its predecessor.

The caveat is the source of the numbers: every benchmark above is Moonshot's own, run on its own harnesses in several cases. Independent reruns will decide how much of the parity with Opus 5.5 survives.

**Related:** [Moonshot AI Releases Kimi K3 with 2.8 Trillion Open Weights](/news/2026/07/27/moonshot-ai-releases-kimi-k3/)

[Kimi K3.1 Pro: Advancing Open Intelligence](https://www.kimi.ai/blog/kimi-k3-1-pro)
