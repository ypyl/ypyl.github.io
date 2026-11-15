---
layout: post
title: "Anthropic Lets Companies Self-Host Claude Managed Agents"
date: 2026-11-15
categories: news
---

Self-hosted **Claude Managed Agents** entered private beta, moving the whole agent control plane into the customer's cloud or own data centres: the harness, sandboxes, credential vaults and session history, plus the server, database and workers the agents execute on. Until now customers could self-host only the sandbox, while planning, model calls and session logs stayed with Anthropic.

Anthropic keeps the model itself hosted, so prompts, code and tool output still reach its API, or the customer's own route through Amazon Bedrock, Google's Agent Platform, Microsoft Foundry or a private gateway. The security controls are the ones the hosted service already used: credentials stay out of the sandbox behind a proxy, each environment gets an allowlist of reachable hosts, and every session exports over OpenTelemetry. Access is by approved application only.

**Related:** [Anthropic Launches Claude Managed Agents: Cloud Infrastructure for Autonomous AI Agents](/news/2026/04/09/anthropic-launches-claude-managed-agents/)

[Operate Claude Managed Agents inside your security perimeter](https://claude.com/resources/articles/self-hosted-claude-managed-agents)
