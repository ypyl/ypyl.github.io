---
layout: post
title: "Liquid AI Releases LFM2.5-2.6B: On-Device Agentic Model for Phones"
date: 2026-08-04
tags: news
categories: news
---

Liquid AI has released **LFM2.5-2.6B**, a compact 2.6B-parameter agentic model designed to run entirely on-device — planning, tool calling, and multi-step tasks work on a phone without any cloud API. Trained on ~34 trillion tokens with a 128K-token vocabulary and 128K context, the model's post-training used expert models to strengthen math, code, tool use, and long context, then transferred those skills into one model via on-policy distillation, with a final stage inside real agent environments including Hermes Agent and OpenClaw.

According to Liquid AI's benchmarks, the model reaches up to **220 tokens/s on Apple M5 Max**, ~30 tokens/s on a smartphone, and nearly 15,000 output tokens/s on a single H100 under high concurrency, with CPU memory usage below 2.5 GB. It outperforms larger Gemma and Qwen models on several instruction-following and tool-use benchmarks, though larger models still lead in complex programming. Open-weight base and fine-tuned versions are available with support for llama.cpp, MLX, vLLM, SGLang, and ONNX.

[Liquid AI blog](https://www.liquid.ai/blog/lfm2-5-2-6b)
