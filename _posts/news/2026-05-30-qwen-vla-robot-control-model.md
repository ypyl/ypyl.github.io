---
layout: post
title: "Qwen Announces VLA Model That Controls Robots Across Different Hardware"
date: 2026-05-30
tags: [news]
categories: news
---

Alibaba's Qwen team has unveiled **Qwen-VLA**, a vision-language-action model capable of controlling robots of varying designs without per-platform fine-tuning. Built on the **Qwen3.5-4B** vision-language backbone with a 1.15B-parameter action decoder, the model unifies three task types — manipulation, navigation, and trajectory prediction — and switches between different robot bodies by simply changing the text instruction.

Qwen-VLA matches or surpasses specialized systems trained for individual tasks, scoring **97.9% on LIBERO**, **87.2% on RoboTwin-Hard**, and **83.6% / 76.9%** success rates on the ALOHA dual-arm platform in familiar and unfamiliar settings respectively. A technical report and GitHub repository are now available, though the model weights themselves have not yet been released.

[Qwen-VLA Blog](https://qwen.ai/blog?id=qwenvla) · [arXiv Paper](https://arxiv.org/pdf/2605.30280) · [GitHub](https://github.com/QwenLM/Qwen-VLA)
