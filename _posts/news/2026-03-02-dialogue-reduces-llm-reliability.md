---
layout: post
title: "Microsoft Research and Salesforce Reveal Dialogue Reduces LLM Reliability"
date: 2026-03-02
tags: news
categories: news
---

Microsoft Research and Salesforce have highlighted a rarely discussed issue: dialogue significantly lowers the reliability of large language models (LLMs). Testing 15 top models, including GPT-4.1, Gemini 2.5 Pro, and Claude 3.7 Sonnet, on over 200,000 simulated conversations showed that while single-turn queries maintain about 90% quality, multi-turn dialogues drop to roughly 65%.

The main problem is not the model's intelligence but reliability, with errors and failures increasing by 112%. Problems arise because models often respond before receiving full context, early mistakes persist throughout the conversation, and longer answers introduce new assumptions leading to more errors.

The study suggests providing the entire context and requirements in a single message for stability, as most benchmarks test single-turn scenarios under ideal conditions, while real dialogues reduce model reliability critically for AI agents and production use.

[Read more on arXiv](https://arxiv.org/abs/2505.06120)
