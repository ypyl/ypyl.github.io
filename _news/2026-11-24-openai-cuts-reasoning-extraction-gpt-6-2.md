---
layout: post
title: "OpenAI Cuts Reasoning Extraction From 54% to 0.2% in GPT-6.2"
date: 2026-11-24
categories: news
---

OpenAI published how it protects the hidden **chain-of-thought** of GPT-6.2 Astra and Sol. In an internal adaptive black-box evaluation across six attacker models, the average success rate for recovering reasoning the model produced while answering an earlier request fell from **54.0%** against GPT-6 Astra to **0.2%** against GPT-6.2 Astra, and from 22.9% to 0.9% for the current request.

The defenses stack three layers. Adversarial training teaches the model to refuse extraction attempts, input and output classifiers watch both streaming and non-streaming responses plus **tool-call arguments**, and API controls tie protected reasoning to the conversation history that produced it, dropping the reasoning if an earlier message is rewritten. Encrypted reasoning can no longer move between users, workspaces, organisations or model families, which closes a pathway described in OpenAI's September disclosure.

OpenAI also says its weekly enforcement against distillation campaigns ranges from thousands to tens of thousands of accounts, and that it briefed Congress in February on DeepSeek attempting to distill US frontier models.

**Related:** [OpenAI Releases GPT-6.2 Astra and Sol](/news/2026/11/18/openai-releases-gpt-6-2-astra-and-sol/)

[Combatting adversarial distillation](https://openai.com/index/combatting-adversarial-distillation/) · [Source channel](https://t.me/ai_machinelearning_big_data)
