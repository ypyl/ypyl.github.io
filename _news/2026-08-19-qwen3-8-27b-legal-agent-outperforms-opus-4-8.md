---
layout: post
title: "Custom Qwen3.8-27B Legal Agent Outperforms Claude Opus 4.8 at a Fraction of the Cost"
date: 2026-08-19
tags: news
categories: news
---

AI-memory startup **Engram** and legaltech company **Harvey** tested what happens when an agent pre-studies a law firm's workspace instead of re-reading documents on every request. They built a synthetic firm (**Calderwood & Harkness**) with 100M tokens across 9,286 files and 266 client cases, then had a **Qwen3.8-27B** model study it over thousands of runs — generating its own training data, writing notes, and compressing the workspace into 1M tokens of structured notes plus LoRA weight updates.

Key results:
- **Intelligence per token**: the custom Qwen scored **190.8** per 100K tokens vs **129.3** for Opus 4.8 (104.2 with high reasoning).
- **Accuracy**: 30% fully correct answers vs 25% for Opus 4.8, at **$0.13 per request vs $1.32** — roughly a 10x cost difference on OpenRouter (August 2026).
- **Weight knowledge**: correctness on workspace questions without any tools jumped from **4.7% to 72.6%** after study; the trained model reaches the right case in **3.7 turns** vs 5.9.

Engram notes that keeping memory fresh without retraining as documents change daily remains an open problem — and its main research direction.

**Related:** [Qwen Releases Qwen3.8-Max with 2.4 Trillion Parameters for Autonomous Coding](/news/2026/08/01/qwen-releases-qwen3-8-max/), [Qwen-AgentWorld Surpasses Claude Opus and GPT-5.4 on Agentic Benchmark](/news/2026/06/24/qwen-agentworld-beats-claude-opus-gpt-5-4/)

[Legal agents with memory](https://engram.com/blog/legal-agents-with-memory)
