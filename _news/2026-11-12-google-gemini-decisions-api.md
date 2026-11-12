---
layout: post
title: "Google Releases the Gemini Decisions API for Choices and Routing"
date: 2026-11-12
categories: news
---

Google has released the **Gemini Decisions API**, an endpoint for choice, routing and classification tasks where the answer is picked from a fixed set: which tool an agent should call, whether a request passes a policy check, which model should handle it, or how usable the retrieved context is. The underlying **Gemini Decision 1** model scores every option in a single pass and returns probabilities across three question shapes: yes or no with the probability that the condition holds, a choice of 2 to 255 options with a probability for each, and an open-ended score on a continuous scale. One request accepts up to **256,000 tokens** of text, images, audio, video or PDF plus up to **1,000 independent questions**, and the content is billed once however many questions are attached.

Google says the model leads on its own agent-routing, reasoning, ranking and multimodal test sets with the lowest calibration error, at almost **five times less** than **Gemini 3.5 Flash-Lite** through the Interactions API. Pricing is **$0.10 per million input tokens** with output tokens free.

It is available now through the **Gemini API** and **Google AI Studio**, with a free tier for prototyping and Google Cloud support promised later.

**Related:** [OpenAI Opens the Decisions API for Agent Micro-Decisions](/news/2026/10/25/openai-opens-decisions-api/), [Microsoft Releases Decision-1, a Model That Only Makes Decisions](/news/2026/10/09/microsoft-releases-decision-1-model/), [OpenRouter Picks TypeSafe's Jev to Route Model Selection](/news/2026/09/29/openrouter-picks-jev-for-model-routing/)

[Gemini Decisions API](https://blog.google/innovation-and-ai/technology/developers-tools/gemini-decisions-api/)
