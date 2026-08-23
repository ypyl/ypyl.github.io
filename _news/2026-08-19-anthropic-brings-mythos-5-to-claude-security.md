---
layout: post
title: "Anthropic Brings Mythos 5 to Claude Security and Launches $35M Defender Fund"
date: 2026-08-19
tags: news
categories: news
---

Anthropic has switched **Claude Security** code scanning to run on its most powerful model, **Mythos 5**, now in public beta for Claude Enterprise customers. An admin enables it in the console; users pick a repository and Claude reports each finding with a CWE category, confidence score, severity rating, and suggested fix.

- **No direct model access**: Mythos 5 runs in the background and only findings are exposed — users can't ask the model for exploits through this interface.
- **Human approval required**: patching happens in Claude Code with models the organization already has access to, and every patch must be reviewed and approved by a person.
- **No extra cost**: scanning counts as regular token usage under the existing plan; the same embedded-model approach will apply to partner products.

Separately, Anthropic launched the **0xDAF Defender Advantage** fund — **$35 million in Claude credits** for people who fix vulnerabilities in open-source code.

**Related:** [Anthropic Launches Claude Code Security](/news/2026/02/21/anthropic-launches-claude-code-security/), [Anthropic's Mythos Finds Mathematical Weaknesses in Cryptographic Algorithms](/news/2026/07/29/anthropic-mythos-cryptographic-weaknesses/)

[Bringing Claude Mythos 5 to more defenders](https://claude.com/blog/bringing-claude-mythos-5-to-more-defenders)