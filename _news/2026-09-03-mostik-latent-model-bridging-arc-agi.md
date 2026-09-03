---
layout: post
title: "Mostik Tops ARC-AGI with Latent Model Bridging"
date: 2026-09-03
tags: news
categories: news
---

Russian mathematicians behind **Mostik** have reached first place on the ARC-AGI benchmark using a new technique for connecting large and small language models. Instead of the standard approach where one model writes an intermediate text response that another reads, Mostik transfers **hidden internal states** through a small, trainable bridge. The original models remain frozen.

In the first experiment, a 753B-parameter **GLM-5.2** model passed its latent representation to a **4B-parameter Qwen-3.5** model via this bridge. The large model handled only the prompt processing (prefill), while the small model generated the final answer. The 4B model closed roughly **50% of the gap** to the 753B model's performance, boosted its own accuracy by about **25%**, and achieved up to **2x improvement** on harder task subsets. The combined pipeline required **2.5x less compute** than a mid-sized model delivering comparable quality, and latent hand-off outperformed plain text transfer by up to **10 percentage points**.

Mostik sees potential applications in multi-agent systems, knowledge distillation, model specialization, and research into internal representations.

[Mostik](https://mostik.ai/read-more), [X post](https://x.com/Machinelearrn/status/2095484584691441747)

**Related:** [Zhipu AI Releases GLM-5.2 with 1M Token Context Window](/news/2026/06/16/zhipu-ai-releases-glm-5-2/)
