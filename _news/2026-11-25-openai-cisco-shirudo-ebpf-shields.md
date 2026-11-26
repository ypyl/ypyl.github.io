---
layout: post
title: "OpenAI and Cisco Launch Shirudo, an eBPF Shield for Systems Still Waiting for a Patch"
date: 2026-11-25
categories: news
---

OpenAI and Cisco announced **Shirudo** on November 23, an open source Linux runtime that blocks the code path behind a vulnerability while a permanent patch is still being rolled out. It is built on eBPF: a shield is a small kernel program, it can be tested against real traffic before it is enforced, and it loads without restarting the service. Shields are cryptographically signed, checked by the Linux eBPF verifier, and their load and removal events are recorded against a TPM.

In the partners' controlled test, a Codex-generated shield **blocked a Java4Shell exploit attempt against a running Elasticsearch service** with no restart. Agents can propose and validate shields, and security teams decide what is deployed.

**Related:** [Google's Agents Automate Exploit Discovery and Patching](/news/2026/11/18/google-agents-automate-exploit-discovery/)

[OpenAI and Cisco introduce Shirudo](https://openai.com/index/introducing-shirudo/) · [Channel](https://t.me/ai_machinelearning_big_data)
