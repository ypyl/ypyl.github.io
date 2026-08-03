---
layout: post
title: "MiniMax Releases H3: 33B-Parameter Video Generation Model You Can Run Locally"
date: 2026-08-01
---

MiniMax has released **H3**, a 33B-parameter video generation model that produces video with **synchronized stereo audio** and supports multiple input modalities: text, images, video, and audio references. The model handles text-to-video, first/last frame control, motion transfer, scene editing, and clips up to 15 seconds at ~768p resolution. Under the hood, H3 uses a dense Omni-Transformer paired with **Qwen3-VL-32B** as its encoder.

ComfyUI has already shipped optimized builds with INT8, FP8, and NVFP4 quantization, bringing minimum download size down to roughly **42.5 GB** — runnable on consumer GPUs with RAM/SSD offloading. However, calling H3 "open source" requires a caveat: only the base weights are published. The **H3-Context-IR** system for complex multimodal queries remains server-side, 2K regen is API-only, and sparse attention is promised "later." A significant license restriction blocks public use of the weights in the **EU, US, UK, and South Korea** without separate permission from MiniMax.

**Related:** [MiniMax M3 Open-Weight Release](/news/2026/06/01/minimax-m3-open-weight-release/), [MiniMax M3 Pro Details](/news/2026/07/09/minimax-m3-pro-2-7-trillion-open-model-details/)

[Model on Hugging Face](https://huggingface.co/MiniMaxAI/MiniMax-H3) · [ComfyUI Build](https://huggingface.co/Comfy-Org/MiniMax-H3)
