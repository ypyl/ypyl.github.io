---
layout: post
title: "Xiaomi Releases Open-Source AI Agent MiMo Code"
date: 2026-06-13
tags: news
categories: news
---

Xiaomi has unveiled **MiMo Code**, an open-source terminal AI agent built on OpenCode and released under the MIT license. The agent is designed to tackle the problem of context loss in long-horizon tasks spanning hundreds of sequential steps. In a blind A/B test across 474 repositories, MiMo Code matched Claude Code paired with Sonnet on short tasks but outperformed it in 65% of cases on tasks exceeding 200 steps.

The core architecture relies on infinite logical sessions: a sub-agent periodically saves intermediate states to disk, and when the token limit is exhausted, the session restarts with all accumulated data loaded into a new window. Memory is organised into four levels—from short local notes to permanent project facts. MiMo Code is available via NPM or Bash scripts.

[MiMo Code announcement](https://mimo.xiaomi.com/zh/blog/mimo-code-long-horizon)
