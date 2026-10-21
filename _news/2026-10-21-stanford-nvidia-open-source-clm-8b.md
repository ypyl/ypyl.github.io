---
layout: post
title: "Stanford and NVIDIA Open-Source CLM-8B, an Open Rival to Jev"
date: 2026-10-21
tags: [news]
categories: news
---

Stanford and NVIDIA have released **CLM-8B**, the first open model in the Contrastive Language Models class and an open counterpart to **Jev**, which TypeSafe AI introduced on 15 September. Given a current state and a set of candidate actions, it returns a calibrated probability for each, and it never generates text: it can judge whether a claim is true, pick among options, or score on an ordered scale.

The architecture is **Qwen3-8B** with two small projection heads trained on top, one for state and one for actions. State and actions are encoded separately, the model compares their vectors, and the closeness becomes a probability. Vectors for unchanged actions can be precomputed and reused, which is where the speed on multi-step tasks comes from.

The release reports that, without fine-tuning, CLM-8B matches Jev on computer control, games and tool calling while answering up to **9x faster**, and up to **13x** with vector caching when choosing from about a thousand options. Fine-tuned as a verifier that picks the best of several coding-agent attempts, it lifts **DeepSWE** from 73.7% to **81.6%** and **Terminal-Bench 2.1** from 84% to **87.6%**, against 71.1% and 83.1% for Jev in the same role.

CLM-8B is Apache 2.0 licensed and runs on Linux with an NVIDIA GPU and Qwen3-8B served through vLLM.

**Related:** [TypeSafe AI Introduces System One Models and Jev](/news/2026/09/16-typesafe-ai-introduces-system-one-models-and-jev/), [Microsoft Releases Decision-1, a Model That Only Makes Decisions](/news/2026/10/09/microsoft-releases-decision-1-model/)

[Blog post](https://contrastive-lm.notion.site/) · [Models on Hugging Face](https://huggingface.co/Contrastive-LM) · [Code on GitHub](https://github.com/Contrastive-LM/CLM)
