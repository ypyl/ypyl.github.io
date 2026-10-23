---
layout: post
title: "Tencent Open-Sources Hy-Nomas, an Interactive Video World Model That Runs on One RTX 4090"
date: 2026-10-23
tags: [news]
categories: news
---

Tencent has released **Hy-Nomas**, an open-source world model that builds an interactive video world one segment at a time, with text and camera trajectory as the controls. On each segment you can change the scene, style, action, or viewpoint, and the model carries its world memory forward between segments. It outputs 720x1280 at 24 fps.

- **Memory and control**: the world state persists across segments, so edits do not reset continuity.
- **Robotics**: a lightweight LoRA adaptation targets narrow domains such as robot training, rather than a full retrain.
- **Hardware**: a single RTX 4090 runs it with INT8 weights, an INT8 KV cache, and CPU offload. The standard configuration assumes eight GPUs.
- **Access**: the repository ships training, inference, and demo code under **Apache-2.0**.

No benchmarks for the model itself have been published yet; Tencent says a technical report will follow.

**Related:** [Odyssey Releases Odyssey-3, a Real-Time Foundation World Model](/news/2026/10/09/odyssey-3-world-model-release/), [Overworld Releases Open-Source Real-Time Game World Generator](/news/2026/01/24/overworld-releases-opensource-realtime-game-world-generator/)

[GitHub](https://github.com/Tencent-Hunyuan/Hy-Nomas) · [Demo](https://hy-nomas.hy.tencent.com/demo) · [Weights on Hugging Face](https://huggingface.co/tencent/Hy-Nomas) · Source: [@ai_machinelearning_big_data](https://t.me/ai_machinelearning_big_data)
