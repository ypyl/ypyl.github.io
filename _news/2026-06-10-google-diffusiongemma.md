---
layout: post
title: "Google Unveils DiffusionGemma — A 26B Diffusion Language Model That Refines Token-by-Token"
date: 2026-06-10
tags: news
categories: news
---

Google has released **DiffusionGemma**, a 26-billion-parameter diffusion language model with 4B active parameters built on the Gemma 4 architecture. Unlike traditional autoregressive models that predict one token at a time, DiffusionGemma generates 256 tokens in parallel and iteratively refines them over multiple passes — similar to how image generators gradually denoise a picture. On a single H100 in FP8, the model achieves over 1,000 tokens per second (compared to 303 for Gemma 4 with multi-token prediction), with around 700 tokens per second promised on an RTX 5090. The model is a preview of the diffusion-based reasoning paradigm and is already supported by vLLM, Unsloth, and other inference frameworks.

[DiffusionGemma-26B-A4B-it on Hugging Face](https://huggingface.co/google/diffusiongemma-26B-A4B-it) · [A Visual Guide to DiffusionGemma](https://newsletter.maartengrootendorst.com/p/a-visual-guide-to-diffusiongemma) · [Live Code Generation Demo](https://huggingface.co/spaces/huggingface-projects/diffusiongemma-codegen)
