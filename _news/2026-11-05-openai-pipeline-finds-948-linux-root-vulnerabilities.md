---
layout: post
title: "OpenAI Pipeline Finds 948 Linux Kernel Vulnerabilities for Local Root"
date: 2026-11-05
categories: news
---

OpenAI has published results from an autonomous vulnerability research pipeline that, in a mid-October run on the current Linux kernel, produced **2,308 candidate findings**, reproduced **1,724** of them in isolated virtual machines, and built working exploits and separate patches for **948 distinct local privilege escalation vulnerabilities**, each of which lets an unprivileged local user reach root. The full run cost **$1,213,430 in API credits**, or about **$1,279 per LPE**, well below the historical $20,000 to $70,000 Pwn2Own and TyphoonPWN bounties for the same class of exploit. The pipeline used an unreleased cyber model at five stages: scout agents picking audit targets, analyst agents scanning them, triage agents reproducing findings under KASAN, exploit agents developing PoCs with SMEP, SMAP and KASLR enabled, and patch generation, with human researchers reviewing before anything was reported.

The newer model found **more than 5x as many new LPEs** as a July run with GPT-5.6 Sol, which produced 166 for $513,690, and the agents used at least **10 exploitation techniques** that were new to third-party kernel exploit experts. OpenAI says it has sent hundreds of reports and patches to the Linux kernel maintainers and is working through the rest with Patch the Planet partners, but the 948 vulnerabilities are not all fixed.

**Related:** [LLMs Drive 3.5x Surge in Critical CVE Discoveries](/news/2026/07/06/llms-fuel-surge-in-cve-discoveries/), [Anthropic Opus 4.6 Discovers Over 500 Zero-Day Vulnerabilities in Open Source](/news/2026/02/07/anthropic-opus-46-discovers-500-zero-day-vulnerabilities/), [Linus Torvalds Supports AI in Kernel Development](/news/2026/07/17/linus-torvalds-supports-ai-in-kernel-development/)

[A thousand fewer paths to root](https://openai.com/index/a-thousand-fewer-paths-to-root/)
