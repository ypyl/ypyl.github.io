---
layout: post
title: "Z.ai Releases GLM-5.4, Its First Flagship Built on a Loop Transformer"
date: 2026-11-23
categories: news
---

Z.ai released **GLM-5.4** on 23 November, the first flagship model built on a **Loop Transformer**. The 750B base model runs 28 of its layers through **two iterations** instead of one, so a deeper network with the same parameter count. Z.ai reports intelligence comparable to 2T to 3T parameter models, and says it reached this by continuing training from a pretrained GLM-5 checkpoint rather than training a looped model from scratch, which the team says still lags unshared models at equal pretraining compute.

Three engineering changes make the second pass affordable. **Gated progressive updates** add a per-token gate for each iteration: the first pass writes back conservatively with small gate values to keep activation norms stable, the second pass opens the gates and writes full updates. A **hybrid KV cache** reuses the sparse KV selected in the first pass and keeps an independent sliding window for the new one, so an extra loop costs one sliding window instead of a full context cache. The second loop also reuses the attention indices produced by the first pass, adding no indexing overhead.

In a controlled iso-parameter comparison on a 30B model sharing the 750B architecture, the looped version gained over the unshared one with about **10% more total training compute**, with the largest gains in math, scientific reasoning and long context. Average output length fell **10 to 14%** on benchmarks such as GPQA-Diamond and MMLU-Pro, which Z.ai reads as reasoning moving from explicit thinking tokens into the implicit depth of the loop. The model reaches up to 200 tokens per second off-peak and 70 to 80 at peak load.

GLM-5.4 adds image input and is available now to GLM Coding Plan subscribers, with the API "coming soon" and open weights planned after safety hardening. The blog lists one known limitation: the model tends to get caught in long "marathon thinking" loops.

**Related:** [Z.ai Open-Sources GLM-5.3 Weights for Agentic Coding and Cybersecurity](/news/2026/08/28/zai-releases-glm-5-3-open-weights/), [OpenAI Astra Recurrent Depth Architecture Revealed](/news/2026/09/02/openai-astra-recurrent-depth-architecture-revealed/)

[GLM-5.4 technical blog](https://z.ai/blog/glm-5.4) · [Source channel](https://t.me/ai_machinelearning_big_data)
