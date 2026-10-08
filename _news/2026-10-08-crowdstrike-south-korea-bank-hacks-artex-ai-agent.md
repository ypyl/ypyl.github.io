---
layout: post
title: "CrowdStrike Links South Korean Bank Hacks to AI Pentest Agent ARTEX"
date: 2026-10-08
tags: news
categories: news
---

CrowdStrike has attributed a campaign against South Korean financial firms, active from late September to early October 2026, to a threat actor that used **ARTEX**, an open-source agentic penetration testing tool developed in China, paired with large language models. The company assesses with moderate confidence that the actor is a Chinese speaker and financially motivated, and has not tied the activity to a named adversary.

- **Model stack**: ARTEX ran **DeepSeek v4.1-flash** as its primary LLM backend, with **GLM-5.3** (Zhipu AI) and **Grok 4.6** used for additional Claude Code sessions. No frontier model was required.
- **How it surfaced**: open directories on attacker-controlled servers exposed Claude Code session histories, ARTEX configuration files, and Claude memory files. In those sessions the operator asked Claude where Korean breach data is sold and for help finding Korean Telegram data sales groups.
- **Victims**: South Korean police opened a formal investigation on October 6 covering seven financial firms, including Shinhan, KB Kookmin, and Hana. Reuters counts at least nine banks targeted since late September; Shinhan reported roughly **25,000 customer records** exposed, KB Kookmin reported **119**.
- **Suspect**: one prompt asked Claude to draft a security researcher résumé with an age, university, and a home in Guangdong. CrowdStrike says the details likely belong to the operator but cannot definitively link them to the attacks. Anthropic, South Korean police, and China's foreign ministry did not respond to Reuters' requests for comment.

**Related:** [Anthropic Finds Chinese GLM-5.3 Nears Closed Models on Cybersecurity Tasks](/news/2026/10/01/anthropic-tests-glm-5-3-cybersecurity-capabilities/), [OpenClaw Hacks Gym Booking System](/news/2026/08/11/openclaw-hacks-gym-system-first-confirmed-autonomous-ai-attack/)

[Unknown Threat Actor Uses AI-Driven ARTEX to Target South Korean Finance](https://www.crowdstrike.com/en-us/blog/unknown-threat-actor-uses-artex-to-target-south-korean-finance/) · [TNW report](https://thenextweb.com/news/crowdstrike-south-korea-bank-hack-suspect-china)
