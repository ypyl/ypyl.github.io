---
layout: post
title: "OpenAI Explains How Codex CLI Works"
date: 2026-01-28
tags: news
categories: news
---

OpenAI has published a detailed breakdown of how **Codex CLI** operates, a local agent that works with code on your machine. The system relies on an **agent loop** where a prompt is built with instructions, and the model either outputs text or requests tool execution, repeating until the task is complete.

Key points include the challenge of context size with a 32KB limit, the use of prompt caching to reduce inference costs, and a shift to a stateless model design for zero data retention. Additionally, a **compaction mechanism** compresses old messages to keep the context window manageable.

Codex CLI also supports local models via a flag for integration with Ollama or LM Studio. More details on CLI architecture and tool usage are expected in future articles.

[Read more](https://openai.com/index/unrolling-the-codex-agent-loop/)

Source: @ai_machinelearning_big_data
