---
layout: post
title: "OpenAI's Decisions API Returns a Choice, Not a Paragraph"
date: 2026-09-30
tags: news
categories: news
---

OpenAI's **Decisions API**, announced at DevDay 2026, is a limited-preview endpoint built on a version of **GPT-6 Luna** that takes context plus a developer-defined set of allowed answers and returns a selection rather than generated text. Developers pass context as text or images, define a bounded question, and get back a value their code can branch on for content classification, request routing, or an agent's next step. OpenAI says it makes decisions **ten times faster** than calling GPT-6 Luna through the regular API, and coverage of the launch reports broader availability expected within days.

A [community guide published today on Hugging Face](https://huggingface.co/blog/sora-2/what-is-openai-decisions-api-a-practical-guide) works through where the pattern fits and where it breaks:

- **Bounded questions only.** "Which queue owns this ticket?" with four allowed answers is a decision; "write the best response" is not. Semantic contract first, unlike JSON mode, which formats text after generation.
- **A decision is a signal, not authorization.** Model judgment intersected with deterministic policy produces the executable path; permissions and business policy stay in code, and a confidence score is not a calibrated probability.
- **Tool-call gates and model routing** are the other uses it highlights: asking whether an action needs human confirmation, or classifying difficulty and risk before choosing a cheap model, an expensive one, retrieval, or human review.
- **Preview caveats.** Endpoint naming, authentication scope, supported inputs, limits, score semantics, region availability, and pricing may still change, so verify the official schema and keep the integration behind a small adapter rather than shipping against a third-party paraphrase.

**Related:** [TypeSafe AI Introduces System One Models and Jev](/news/2026/09/16/typesafe-ai-introduces-system-one-models-and-jev/), [OpenRouter Picks TypeSafe's Jev to Route Model Selection](/news/2026/09/29/openrouter-picks-jev-for-model-routing/), [OpenAI Releases GPT-6 Sol and GPT-6 Luna](/news/2026/09/22/openai-releases-gpt-6-sol-and-luna/)

[What Is OpenAI Decisions API? A Practical Guide](https://huggingface.co/blog/sora-2/what-is-openai-decisions-api-a-practical-guide) · [The Decoder: OpenAI expands Codex and its API at DevDay](https://the-decoder.com/openai-expands-codex-and-its-api-at-devday-with-security-scans-a-decisions-api-and-ultrafast/)
