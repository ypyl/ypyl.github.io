---
layout: post
title: "Alibaba Ships Qwen4-Max with 2.4 Trillion Parameters and 62% Lower Cost per Task"
date: 2026-11-16
categories: news
---

Alibaba has released **Qwen4-Max**, the flagship of a redesigned Qwen4 family: a sparse mixture-of-experts model with **2.4 trillion backbone parameters, 71 billion activated per token, and another 328 billion N-gram embedding parameters**, on a 1 million token context window with native vision. The architecture work sits in the attention stack and around it: three Gated DeltaNet layers compress history into fixed-size states ahead of each Qwen Sparse Attention layer, four parallel residual branches gate their own reads and writes for training stability, and a context-based N-gram lookup table adds capacity at the second layer.

The efficiency numbers are the release's real claim. On **Work360**, Alibaba's own benchmark over 26 top-level domains and 721 subfields, Qwen4-Max scores **52.69** against 47.74 for Qwen3.8-Max, while spending 24% (Xhigh) and 35% (High) fewer tokens per task. On **Terminal-Bench 4.0** the token reduction is 48% and 49%. Combined with lower API prices, average cost per Work360 task falls from **$4.43 to $1.67**, a 62% cut. The base model shows the same direction: 5% to 13% lower bits per byte across knowledge, code, QA and STEM while activating 25% fewer parameters.

Access opened on 11 November through Alibaba's Token Plan, and the model also runs in QwenWork, Qoder and QwenStudio. Alibaba says smaller Qwen4 models will follow as open weights, built on the same lineage.

**Related:** [Qwen Releases Qwen3.8-Max with 2.4 Trillion Parameters](/news/2026/08/01/qwen-releases-qwen3-8-max/), [Qwen3.8-27B Legal Agent Outperforms Opus 4.8](/news/2026/08/19/qwen3-8-27b-legal-agent-outperforms-opus-4-8/)

[Qwen4-Max: Your New Teammate Has Arrived](https://qwen.ai/blog?id=qwen4-max) · [Alibaba opens Qwen4-Max preview](https://technode.com/2026/11/11/alibaba-opens-qwen4-max-preview-with-a-million-token-context-window/) · [Source channel](https://t.me/ai_machinelearning_big_data)
