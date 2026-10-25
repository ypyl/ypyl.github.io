---
layout: post
title: "Symantec Finds an Agent That Hijacks Other People's Models for Attacks"
date: 2026-10-25
categories: news
---

Symantec has documented an **LLMjacking** agent that hunts the internet for exposed endpoints, test servers and leaked keys to commercial models, then runs its offensive operations on someone else's compute. The scheme came apart when the operators mistakenly sent a working prompt into the company's honeypot.

- **Filter bypass:** multi-layer jailbreaking carries the payload past the victim model's own guardrails.
- **Toolkit:** 74 utilities shipped with the agent, including scanners, exploits and botnet components.
- **Autonomy:** the "Skynet" mode lets the model run up to **100,000 iterations** without operator confirmation, and the agent reconnoiters networks, attacks infrastructure, installs miners, wipes logs and hunts fresh open models to copy itself onto.

The damage runs both ways: breached servers lose data and burn power on mining, while the owners of the open gateways receive the token bill for attacks they never ran.

**Related:** [Anthropic Warns Self-Replicating AI Worms Are Close](/news/2026/10/13/anthropic-warns-self-replicating-ai-worms/), [Manifold Researcher Discovers Cryptomining Botnet on ClawHub](/news/2026/04/30/manifold-discovers-clawhub-cryptomining-botnet/), [OpenCode Console Breach Leaks User API Keys](/news/2026/10/20/opencode-console-breach-exposes-api-keys/)

[LLMjacking: the Skynet agent](https://www.security.com/blog-post/ai-llmjacking-skynet)
