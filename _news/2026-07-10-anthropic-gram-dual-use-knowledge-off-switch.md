---
layout: post
title: "Anthropic Proposes GRAM Method to Modularly Remove Dual-Use Knowledge from LLMs"
date: 2026-07-10
tags: news
categories: news
---

Anthropic, in collaboration with AE Studio, has published research on **GRAM** (a modular approach to managing dual-use knowledge in large language models). The method adds groups of neurons—one module per sensitive topic—to each layer of a neural network; training on topic-specific data updates only the corresponding module, which can then be removed or gated to eliminate the capability without retraining the entire model. In experiments across models from 50M to 5B parameters, removing a module suppressed the targeted ability nearly as completely as if the model had never been trained on that data, withstood jailbreak-style recovery attempts, and left general performance intact. GRAM has **not** been applied to any production Claude model, and Anthropic acknowledges that some useful knowledge may be too intertwined with dangerous information to cleanly separate.

[Research paper](https://www.anthropic.com/research/off-switch-dual-use) · [Code on GitHub](https://github.com/agencyenterprise/modular-pretraining) · [Datasets on Hugging Face](https://huggingface.co/datasets/AE-data/dual-use-papers)
