---
layout: post
title: "OpenAI Publishes Detailed API Guide for GPT-6 Astra"
date: 2026-09-05
tags: [openai, gpt-6, astra, api, agents]
categories: news
---

OpenAI has released a comprehensive API guide for **GPT-6 Astra**, walking developers through the model's new capabilities and key migration differences. Astra supports **async tool calling** (the model continues reasoning while an external tool runs), **mid-turn steering** (instructions can be changed mid-task without restarting), and **dynamic reasoning** (effort level can be raised or lowered mid-dialogue while preserving prompt cache). The guide also covers computer use, structured outputs, multi-agent orchestration, programmatic tool calling, and compaction.

Migration brings several breaking changes: `reasoning_effort: none` is no longer supported (start with `low`), tool calling requires the Responses API, and `temperature`, `top_p`, and `top_logprobs` are unsupported. OpenAI notes that Astra often uses significantly fewer output tokens per task, so effective cost may be lower despite a higher per-token price. For autonomous agents, the company recommends explicitly instructing Astra to complete tasks end-to-end, as the model tends to ask clarifying questions more frequently.

[GPT-6 Astra API Guide](https://developers.openai.com/api/docs/guides/latest-model)

**Related:** [OpenAI Launches GPT-6 Astra with Strong Benchmark Results](/news/2026/09/03/openai-launches-gpt-6-astra/), [The Information Reveals Astra's Recurrent Depth Architecture](/news/2026/09/02/openai-astra-recurrent-depth-architecture-revealed/)
