---
layout: post
title: "NVIDIA Introduces Human-Like Memory for LLMs with Test-Time Learning"
date: 2026-03-11
tags: news
categories: news
---

NVIDIA has unveiled a groundbreaking approach to LLM memory called **TTT-E2E (Test-Time Training End-to-End)** that enables models to learn and adapt during the response generation itself. Instead of treating context as static text, their method uses the context as training data, allowing the model to compress experience into its weights on the fly. This results in a form of memory where the LLM becomes smarter within a single session.

The approach significantly boosts efficiency, offering up to 2.7x speed improvement on 128K tokens and 35x on 2 million tokens with the H100 GPU, compared to traditional full-attention mechanisms. This innovation shifts retrieval-augmented generation (RAG) closer to human-like learning by internalizing experience rather than repeatedly referencing external context.

Source: [NVIDIA Developer Blog](https://developer.nvidia.com/blog/reimagining-llm-memory-using-context-as-training-data-unlocks-models-that-learn-at-test-time/)
