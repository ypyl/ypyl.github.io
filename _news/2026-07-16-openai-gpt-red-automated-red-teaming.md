---
layout: post
title: "OpenAI Builds GPT-Red — An AI That Finds Vulnerabilities in Its Own Models"
date: 2026-07-16
tags: news
categories: news
---

OpenAI has developed **GPT-Red**, an internal system that automates red-teaming of its own LLMs. The model simulates prompt injections and hidden attacks delivered through emails, files, and web pages, then routes them through an adversarial loop: an attacker model and a defender model compete continuously, trained with reinforcement learning and self-play.

In test scenarios, GPT-Red achieved an **84% vulnerability detection rate** compared to **13% for human specialists**. According to OpenAI, GPT-5.6 Sol's susceptibility to direct prompt injections dropped **6x** compared to models from four months ago — though roughly **3.8% of complex attacks** still get through, on par with Claude Opus 4.5. The tool remains internal; OpenAI plans to publish a paper detailing the architecture and training process.

[Unlocking Self-Improvement: GPT-Red — OpenAI](https://openai.com/index/unlocking-self-improvement-gpt-red/)
