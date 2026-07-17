---
layout: post
title: "PrismML Releases Bonsai 27B — Extreme Compression for Local AI"
date: 2026-07-16
tags: news
categories: news
---

PrismML has released **Bonsai 27B**, a heavily compressed 27B-parameter model designed to run where models of its class were previously impractical: laptops, local agents, and even phones. Built on **Qwen3.6 27B**, it comes in two flavors — **Ternary Bonsai 27B** (5.9 GB, ~1.71 effective bits/weight) for desktops and laptops, and **1-bit Bonsai 27B** (3.9 GB, ~1.125 effective bits/weight) small enough to fit the memory budget of an iPhone 17 Pro.

Despite the aggressive compression, the model retains vision, tool calling, agentic loops, structured outputs, and 256k+ context, with support for speculative decoding, MLX (Apple), CUDA, and llama.cpp. Benchmarks show **~95% of full-precision quality** for the ternary version and **~90% for the 1-bit version** across reasoning, math, coding, instruction following, tool calling, and vision tasks. Released under **Apache 2.0** with models on Hugging Face, a WebGPU demo, API via Together, and a whitepaper.

[PrismML Blog — Bonsai 27B](https://prismml.com/news/bonsai-27b) · [The Information](https://theinformation.com/articles/khosla-backed-startup-claims-breakthrough-largest-ever-ai-model-iphone) · [CNBC](https://cnbc.com/2026/07/14/apple-prismml-ai-compression-iphone.html)
