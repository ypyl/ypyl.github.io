---
layout: post
title: "DeepSeek Upgrades V4-Flash with 0731 Checkpoint: Major Gains from Re-Post-Training"
date: 2026-08-03
tags: [deepseek, llm, ai, open-source, coding, agentic-ai]
categories: news
---

DeepSeek has published **DeepSeek-V4-Flash-0731** on Hugging Face and moved the official V4-Flash API into public beta. The architecture and 284B-parameter size remain unchanged — the improvements come entirely from re-post-training. The checkpoint ships with the **DSpark speculative decoding module** attached, bringing the total to 304B parameters. The API now natively supports the Responses API format and is adapted for Codex.

On DeepSeek-reported benchmarks, the 0731 version dramatically outperforms both the V4-Flash preview and V4-Pro on agentic and coding tasks: **82.7% on TerminalBench 2.1** (up from 61.8), **54.4% on DeepSWE** (up from 7.3), and **76.7% on Cybergym** (up from 38.7). Self-hosting requires roughly 110 GB combined RAM+VRAM at 3-bit quantization or a 4×GB300 node at full precision. The model is **MIT-licensed** and ungated, with API pricing at **$0.14/1M input tokens** (cache miss) and **$0.28/1M output tokens** — roughly a third of V4-Pro output pricing.

**Related:** [DeepSeek Launches V4-Flash API with Agentic Upgrades](/news/2026/07/30/deepseek-launches-v4-flash-api-with-agentic-upgrades/)

[Hugging Face](https://huggingface.co/deepseek-ai/DeepSeek-V4-Flash-0731) · [API Pricing](https://api-docs.deepseek.com/quick_start/pricing)
