---
layout: post
title: "Anthropic Releases Claude Haiku 5.5, Its Fastest and Cheapest Small Model"
date: 2026-10-07
tags: [news]
categories: news
---

Anthropic has released **Claude Haiku 5.5**, completing the Claude 5.5 line as the fastest and cheapest tier alongside Opus 5.5 and Sonnet 5.5. The model targets high-volume, cost-sensitive work such as summaries, compaction, database queries, and classification, and Anthropic says it now costs around **75% less to run** than Haiku 4.5. It is the first Haiku-class model with an adjustable effort setting, and it scores **1620 on GDPval-AA v2.1** against 735 for Haiku 4.5, **72.4% on OSWorld 2.1** against 15.7%, and **39.2% on Terminal-Bench 4.0** against 0%, though Sonnet 5.5 remains the better choice for complex agentic coding at 70.6%.

API pricing per million tokens for prompts up to 100k is $0.10 input, $0.50 output, $0.01 cache reads, and $0.125 cache writes, with higher rates above that threshold. Haiku 5.5 is available on AWS, Google Cloud, Azure, and the Claude Platform under the model ID `claude-haiku-5-5`. Anthropic also halved Sonnet 5.5 cache read pricing to $0.10 per million tokens, which it says cuts the cost of most agentic Sonnet 5.5 work by around 20%, and introduced monthly API credits for Max and Team subscribers ($100 for Max 5x, $200 for Max 20x, up to $500 pooled for Team).

[Introducing Claude Haiku 5.5](https://www.anthropic.com/claude-haiku-5-5)

**Related:** [Anthropic Releases Claude Sonnet 5.5 with Focus on Coding and Agents](/news/2026/09/28/anthropic-releases-claude-sonnet-5-5/), [Anthropic Releases Claude Opus 5.5](/news/2026/09/22/anthropic-releases-claude-opus-5-5/)
