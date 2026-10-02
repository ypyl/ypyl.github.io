---
layout: post
title: "Anthropic Finds Chinese GLM-5.3 Nears Closed Models on Cybersecurity Tasks"
date: 2026-10-01
tags: news
categories: news
---

Anthropic has tested Z.ai's open-weight **GLM-5.3** on cybersecurity tasks, and the results came close to those of closed frontier models. On **ExploitBench**, the model assembled working exploit chains in 50 of 410 attempts; given an isolated environment, it spent a day finding several new browser vulnerabilities and combined them into an attack that could read files from the test machine. The smaller **GLM-5.3-Flash** chained two known Chrome vulnerabilities into a working exploit for roughly 8 hours of model time, 20 minutes of human work, and $20.40 in costs.

Anthropic also probed the model's defensive guardrails. After modifying the weights, the refusal rate dropped from over **90% to single-digit percentages** with almost no loss of capability, an operation Anthropic estimates at about **$4,400** — or around **$1,200** for an experienced team.

**Related:** [Z.ai Open-Sources GLM-5.3 Weights for Agentic Coding and Cybersecurity](/news/2026/08/28/zai-releases-glm-5-3-open-weights/), [Z.ai Releases GLM-5.3-Flash with Native Multimodal Support](/news/2026/08/26/zai-releases-glm-5-3-flash/), [Anthropic Opus 4.6 Discovers Over 500 Zero-Day Vulnerabilities in Open Source](/news/2026/02/07/anthropic-opus-46-discovers-500-zero-day-vulnerabilities/)
