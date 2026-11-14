---
layout: post
title: "Google Ships Gemini 4 Hydrogen as the Cheap Execution Model for Agents"
date: 2026-11-14
categories: news
---

Google released **Gemini 4 Hydrogen** on November 12, positioned below **Carbon** and **Argon** in the Gemini 4 family: those two keep the long-horizon reasoning work, Hydrogen takes the loop that writes code, runs tests and verifies changes. The company reports it finishes tasks at its high default thinking level 2.5 times faster than **Gemini 3.8 Flash** while more than tripling **Terminal-Bench 4.1** to 57% from 18.2%, and cuts fabricated claims by more than five times.

On the **Artificial Analysis Intelligence Index** it scores 51 at the default high thinking level against 50 for Argon, reaching 54 at the `xhigh` setting. Pricing is flat across the whole 1 million token context window: $1.50 per million input tokens, $7.50 per million output.

The more interesting part for anyone wiring model routing is **Orchestrator** in Google Antigravity, which switches between Carbon and Hydrogen inside a single session instead of picking one model for the whole task. Google measured near-Carbon task success on **Terminal-Bench 4.0** with 24% less quota consumed and 19% faster completion. For parallel agent work the split is explicit: Carbon coordinates, Hydrogen runs as worker and verifier agents in isolated workspaces.

Hydrogen is available in all tiers of Google AI Studio, the Gemini API, Gemini for business, the Gemini app, Google Search, and to paid AI Pro and Ultra subscribers in Antigravity.

**Related:** [Google Releases Gemini 4 Carbon](/news/2026/10/28/google-releases-gemini-4-carbon/), [Google Releases Gemini 4 Argon](/news/2026/10/13/google-releases-gemini-4-argon/), [Google Releases the Gemini Decisions API for Choices and Routing](/news/2026/11/12/google-gemini-decisions-api/)

[Introducing Gemini 4 Hydrogen](https://blog.google/innovation-and-ai/models-and-research/gemini-models/introducing-gemini-4-hydrogen/) · [Source channel](https://t.me/ai_machinelearning_big_data)
