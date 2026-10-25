---
layout: post
title: "OpenAI Opens the Decisions API for Agent Micro-Decisions"
date: 2026-10-25
categories: news
---

OpenAI has opened its **Decisions API**, which narrows **GPT-6 Luna** to tasks with tightly bounded answers and returns a value an agent can branch on instead of generated text. The company says it cuts latency and cost on the small decisions that dominate agent loops and runs up to **ten times faster** than the Responses API.

- **Three answer formats:** probability that a condition holds, selection from a supplied list, or a score on a scale.
- **Batching:** multimodal context in, up to **400 questions per request**.
- **Pricing:** output tokens free, input at **$0.10 per million tokens**, or **$0.01 per million** with context caching.
- **Governance:** processing localized in the US and EU, plus Zero Data Retention for enterprise customers.

The endpoint was shown at DevDay 2026 as a limited preview, so this announcement settles the details developers had been waiting on.

**Related:** [OpenAI's Decisions API Returns a Choice, Not a Paragraph](/news/2026/09/30/openai-decisions-api-practical-guide/), [OpenAI Releases GPT-6 Sol and GPT-6 Luna](/news/2026/09/22/openai-releases-gpt-6-sol-and-luna/)

[OpenAI Decisions API](https://openai.com/index/decisions-api/)
