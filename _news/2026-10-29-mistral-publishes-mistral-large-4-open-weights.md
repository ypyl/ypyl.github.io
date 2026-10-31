---
layout: post
title: "Mistral Publishes Mistral Large 4 Open Weights Under a Modified MIT License"
date: 2026-10-29
---

Mistral has released the weights of **Mistral Large 4**, the 1T-parameter multimodal flagship it trained from scratch on European infrastructure and previewed on October 6. After the preview, the model was fine-tuned with reinforcement learning for programming and cybersecurity tasks. The weights arrive as `Mistral-Large-4-1T-A52B` under a **modified MIT license**, and the model is reachable without local setup through the Mistral API, OpenRouter, Mistral's Vibe Code environment and NVIDIA NIM containers.

On the **Artificial Analysis Coding Agent Index** the model scores 48% and spends 47% fewer tokens per task than GLM-5.3. On the **Artificial Analysis Cyber** benchmark it scores 51% at finding, reproducing and patching vulnerabilities, and Mistral says it audits code without security filters raising false positives. Three built-in reasoning modes, Low, Medium and High, trade cost against depth, with High as the default.

Official API pricing is $0.70 per million input tokens ($0.07 when cached) and $2.10 per million output tokens.

**Related:** [Mistral Unveils Large 4, a 1T-Parameter Open-Weight Model Trained in Europe](/news/2026/10/06/mistral-large-4-le-chonk-one-trillion-parameters/), [Aleph Alpha Open-Sources Kolibri 1, a Bilingual German-English MoE Model](/news/2026/10/06/aleph-alpha-releases-kolibri-1-open-weights/)

[Mistral Large 4 open weights](https://mistral.ai/news/mistral-large-4-oss/) · [Weights on Hugging Face](https://huggingface.co/mistralai/Mistral-Large-4-1T-A52B)
