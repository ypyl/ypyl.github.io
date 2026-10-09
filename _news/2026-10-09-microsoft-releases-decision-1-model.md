---
layout: post
title: "Microsoft Releases Decision-1, a Model That Only Makes Decisions"
date: 2026-10-09
tags: news
categories: news
---

Microsoft has released **Microsoft-Decision-1**, a model that takes a request plus a fixed set of options and returns a calibrated probability for each in a single pass: yes/no, list selection, ranking, or rubric-based scoring of an AI answer or agent action. It is built on **Qwen3.5-9B** after post-training, and Microsoft reports the best accuracy among both LLMs and other decision models across 36 closed benchmarks and nearly 150,000 questions, while being **4.5x faster** than the nearest competitor and **35x faster** than GPT-6 Sol. Decisions change in only **1.3%** of cases when options are paraphrased or shuffled.

Microsoft says output tokens are free and input costs **$0.042 per million tokens**, and the model is already in internal use: Xbox Research labeled 10,000 reviews at GPT-6 Sol quality but 14x faster and 200x cheaper, while Copilot evaluates agent responses 100x faster than GPT-5.6 Luna. Decision-1 is available now in **Microsoft Foundry**, with an OpenRouter release planned.

**Related:** [OpenAI's Decisions API Returns a Choice, Not a Paragraph](/news/2026/09/30/openai-decisions-api-practical-guide/), [TypeSafe AI Introduces System One Models and Jev](/news/2026/09/16/typesafe-ai-introduces-system-one-models-and-jev/), [Microsoft Releases Two New MAI Models for Image Generation and Speech](/news/2026/07/27/microsoft-releases-two-new-mai-models/)

[Microsoft-Decision-1 in Microsoft Foundry](https://commandline.microsoft.com/microsoft-decision-1-model-foundry/)
