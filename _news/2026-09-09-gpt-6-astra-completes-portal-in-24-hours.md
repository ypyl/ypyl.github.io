---
layout: post
title: "GPT-6 Astra Completes Portal in 24 Hours Autonomously"
date: 2026-09-09
tags: news
categories: news
---

Twitter user **cozyblaze** got **GPT-6 Astra** to play through the original Portal from start to finish, watching credits roll after 23 hours 42 minutes. The experiment started on September 4 at 5 PM and ended September 5 at 4:43 PM. Astra ran in maximum reasoning mode via MCP and a modified SourcePauseTool: the game would pause, the model would receive a screenshot, character coordinates, and camera angle, plan its next move, send commands, and unpause. This cycle repeated for nearly 24 hours.

Pure gameplay without reasoning pauses amounted to roughly 2 hours ([YouTube recording](https://www.youtube.com/watch?v=g5u2y0BwRJ0)). According to the [published log](https://github.com/cozyblaze/portal-agent/blob/main/evidence/summary.json), the run consumed 434.8 million tokens total: 433.2 million input and 1.6 million output, with 1.18 million used for reasoning. The model made 3,300 tool calls, 3,210 of which were in-game commands.

The key economics: 426 million of the 433 million input tokens hit cache, bringing the API cost to approximately $574 at published rates ($10/M input, $50/M output, $1/M cached input). Without caching, the same run would have cost around $4,400. In practice, cozyblaze spent nothing beyond a $200/month Codex subscription.

[cozyblaze on X](https://x.com/cozyblazex/status/2096383114851533097)

**Related:** [OpenAI Launches GPT-6 Astra with Strong Benchmark Results](/news/2026/09/03/openai-launches-gpt-6-astra/), [OpenAI Publishes Detailed API Guide for GPT-6 Astra](/news/2026/09/05/openai-publishes-detailed-api-guide-for-gpt-6-astra/), [OpenAI's Astra Solves 10 Longstanding Math Problems](/news/2026/08/01/openai-astra-solves-10-longstanding-math-problems/), [Cortical Labs Human Neurons Play Doom Faster Than GPT-4](/news/2026/03/05/cortical-labs-human-neurons-play-doom/)
