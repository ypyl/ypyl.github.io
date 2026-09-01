---
layout: post
title: "DeepSeek Open-Sources V4-Flash-Vision-Exp Weights Under MIT"
date: 2026-09-01
tags: news
categories: news
---

Ten days after the API release, DeepSeek has published the weights of its **305B-parameter V4-Flash-Vision-Exp** model on Hugging Face under the **MIT license**. The experimental checkpoint stacks vision modules on the V4-Flash architecture and is fine-tuned for image understanding, producing an agent that can parse screenshots, read charts, and work with tools.

On multimodal benchmarks the model scores **36.5 on ApexBench** (vs 26.2 for V4-Flash-0731, which cannot take image input), 27.3 on Agents Last Exam, and 35.0 on ZeroBench, though it trails Claude Opus 4.8 on ApexBench and Chartography. In text tasks it improves on its predecessor in Toolathlon-Verified (75.9), NL2Repo (57.7), and DeepSWE (59.3, above Opus 4.8's 58.0), while slipping slightly on Cybergym and Terminal Bench 2.1.

The repo ships a tokenizer and minimal PyTorch inference with a visual encoder, DFlash attention, MoE, Hyper-Connections, and a DSpark forward pass. Images can be fed as OpenAI-style blocks or compact tag-based encoding; the model runs via vLLM, SGLang, Transformers, and Docker, and the community has already published quantized versions for local use.

**Related:** [DeepSeek Releases Experimental V4-Flash-Vision Model](/news/2026/08/23/deepseek-releases-v4-flash-vision-exp/), [DeepSeek Upgrades V4-Flash (0731)](/news/2026/08/03/deepseek-upgrades-v4-flash-0731/), [DeepSeek Launches V4-Flash API with Agentic Upgrades](/news/2026/07/30/deepseek-launches-v4-flash-api-with-agentic-upgrades/)

[Model on Hugging Face](https://huggingface.co/deepseek-ai/DeepSeek-V4-Flash-Vision-Exp) · [Quantized versions](https://huggingface.co/models?other=base_model:quantized:deepseek-ai/DeepSeek-V4-Flash-Vision-Exp)
