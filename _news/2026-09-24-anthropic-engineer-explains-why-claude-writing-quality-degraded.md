---
layout: post
title: "Anthropic Engineer Explains Why Claude's Writing Quality Degraded"
date: 2026-09-24
tags: news
categories: news
---

Anthropic's Jackson Kernion has explained why Claude's prose became noticeably worse after Opus 4.6. The cause: training shifted toward math, code, and technical explanations aimed at other language models. LLMs have larger working memory than humans and pick up on fine-grained details more precisely, so the writing style optimized for them reads to people as dense, impenetrable slabs of text. The culprit sits in the RL reward signals: some rewards incentivize text that is clear to the model, others text that is clear to humans. The more math and code in training, the harder it becomes to reward the simpler explanations a person can actually read. Kernion says Opus 5.5 has finally found a better balance, though he notes the problem is difficult and work continues.

[X: Jackson Kernion](https://x.com/JacksonKernion/status/2102437423670325581)

**Related:** [Anthropic Reduces Claude's Flattery in Relationship Advice](/news/2026/05/03/anthropic-reduces-claude-flattery-in-relationship-advice/), [Anthropic Releases Claude Opus 5.5](/news/2026/09/22/anthropic-releases-claude-opus-5-5/)
