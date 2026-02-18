---
layout: post
title: "LLM Accuracy Significantly Improves by Repeating Prompt Twice"
date: 2026-02-18
tags: news
categories: news
---

A recent study has revealed that simply repeating the same prompt twice can dramatically boost the accuracy of large language models (LLMs). In one test involving searching for an element in a long list, the accuracy of a model increased from 21% to 97% without any fine-tuning, additional computation, or complex prompt engineering.

This improvement occurs because LLMs process text from left to right with causal attention, and repeating the input provides tokens a "second chance" to see the full context, enhancing attention connections. The effect was confirmed across 7 benchmarks and 7 models including GPT-4o, Claude, Gemini, and DeepSeek, especially in tasks involving search, retrieval, and long-context handling.

The study suggests that improving model output quality is increasingly achieved through managing context input rather than scaling model size, highlighting the importance of architectures and system-level practices that compensate for attention limitations.

Source: https://arxiv.org/pdf/2512.14982
