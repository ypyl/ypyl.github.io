---
layout: post
title: "Hugging Face Releases Multi-Harness RL for Training Models Inside Coding Agents"
date: 2026-10-07
tags: [news]
categories: news
---

Hugging Face has released **Multi-harness RL**, a tool that trains language models with reinforcement learning directly inside coding agents such as **Claude Code, Codex, and OpenCode**, with 10 environments supported out of the box. It works through a proxy layer that intercepts requests in OpenAI, Anthropic, or Gemini format, routes them to vLLM, logs token IDs and logprobs, and forwards the resulting trajectories to the **TRL** library, turning the agent's own interface into an RL environment without modifying the agent's codebase. The proxy source is published under **OpenEnv**, alongside TRL training scripts and seven pre-trained models.

[Multi-Harness RL on Hugging Face](https://huggingface.co/spaces/FineEnvs/multi-harness-rl#what-is-a-harness) · [Clement Delangue on X](https://x.com/ClementDelangue/status/2107120717980471638)

**Related:** [Stanford + MIT: Meta-Harness Optimization Boosts LLM Performance Up to 6x](/news/2026/09/13/stanford-mit-meta-harness-optimization-boosts-llm-performance/), [Harness Engineering: Leveraging Codex in an Agent-First World](/news/2026/02/11/harness-engineering-leveraging-codex/), [Hugging Face Launches ML Intern Agent for Automated ML Experiments](/news/2026/09/10/hugging-face-launches-ml-intern-agent/)
