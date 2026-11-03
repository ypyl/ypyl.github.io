---
layout: post
title: "Mistral Publishes Technical Report for Mistral Large 4"
date: 2026-11-03
---

Mistral has published a technical report for **Mistral Large 4**, the 1T-parameter flagship whose weights shipped on October 29. The model activates 52 billion parameters per token using a mixture of experts with **128 experts**, four of which are selected per token plus one shared expert. The report details Multi-head Latent Attention, Sparse Attention and **KV-cache reuse across layers**, the changes that cut memory and compute on long-context inference for a **1M-token** window. The model handles images, more than 160 languages, tool calling and agentic workflows, and its weights are on Hugging Face in **FP8 and NVFP4**.

On the Artificial Analysis Coding Agent Index it scores 48%, about level with GLM-5.3 while using 47% fewer tokens according to Mistral. The model is not runnable on a consumer GPU, so the report is aimed at people working on LLM architecture and optimization rather than at local deployment.

**Related:** [Mistral Publishes Mistral Large 4 Open Weights Under a Modified MIT License](/news/2026/10/29/mistral-publishes-mistral-large-4-open-weights/), [Mistral Unveils Large 4, a 1T-Parameter Open-Weight Model Trained in Europe](/news/2026/10/06/mistral-large-4-le-chonk-one-trillion-parameters/)

[Technical report (PDF)](https://huggingface.co/mistralai/Mistral-Large-4-1T-A52B/blob/main/assets/technical_report.pdf)
