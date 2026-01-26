---
layout: post
title: "Anthropic Bans User for Automating Claude Instructions"
date: 2026-01-26
tags: news
categories: news
---

Portuguese developer Hugo Daniel lost access to Anthropic's Claude API after attempting to automate prompt generation between two Claude agents. Hugo set up a loop where one Claude instance updated a context file with instructions, while the other executed tasks based on those instructions. When errors occurred, Hugo fed the mistakes back to the first Claude to revise instructions.

However, this led to the first Claude generating system-like directives in all caps, which triggered Anthropic's prompt injection protection heuristics. The system flagged this as a possible jailbreak attempt, resulting in Hugo's API ban. Support offered no explanation, only a silent refund.

The case highlights the opaque nature of AI moderation systems and warns developers about the risks of meta-prompting and scaffolded auto-generation of prompts.

Source: https://hugodaniel.com/posts/claude-code-banned-me/ (via @ai_machinelearning_big_data)
