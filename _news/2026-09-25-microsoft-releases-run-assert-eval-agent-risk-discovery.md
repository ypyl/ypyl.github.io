---
layout: post
title: "Microsoft Open-Sources run-assert-eval for AI Agent Risk Discovery"
date: 2026-09-25
tags: news
categories: news
---

Microsoft has released **run-assert-eval**, an open-source skill for VS Code that automates the full lifecycle of AI agent risk testing: find risks, measure them, deploy controls, and verify the fix.

The tool chains four components from a single prompt: **Clarity** scans for potential failure modes; **ASSERT** converts them into eval tests and measures violations; **Agent Control Specification (ACS)** generates runtime policies that block dangerous actions; and then the agent is re-tested on the same benchmarks to confirm the guardrails actually work.

In Microsoft's example, a support agent disclosed a different customer's private data in 30% of applicable tests. After adding ACS runtime controls, that rate dropped to 5.9%.

Both ASSERT and Agent Control Specification are available as open source.

**Related:** [OpenAI Discloses Six Cases of Model Misalignment](/news/2026/09/17/openai-discloses-six-cases-of-model-misalignment/), [Anthropic Measures AI Agent Autonomy in Real-World Use](/news/2026/02/19/anthropic-measures-ai-agent-autonomy/)

[Microsoft Announcement](https://commandline.microsoft.com/run-assert-eval-responsible-ai-agent-risk-discovery-at-runtime/)
