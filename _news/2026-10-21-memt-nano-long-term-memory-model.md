---
layout: post
title: "MemT-1.0-Nano Keeps Long-Term Memory in Model State Instead of Context"
date: 2026-10-21
tags: [news]
categories: news
---

**MemT-1.0-Nano** is an open model that writes what it learns from a conversation into an internal memory state and reuses it in later replies, so the full transcript never has to be reloaded into the context window. That state can also be saved and restored in a new session.

The model is built on **Qwen3.6-35B-A3B**, with **36B parameters** of which **3B are active**. Its developers report that with a **128K-token** history the memory state occupies roughly **66 times less space** than a full KV cache.

The trade-off is stated plainly: on tasks that require pulling exact details out of a long history, the best text-memory systems still lead. The target use case is assistants and agents that carry past experience forward without letting context grow with every turn.

**Related:** [NVIDIA Introduces Human-Like Memory for LLMs with Test-Time Learning](/news/2026/03/11-nvidia-llm-memory-test-time-learning/)

[MemT-1.0-Nano on Hugging Face](https://huggingface.co/MemTensor/MemT-1.0-Nano)
