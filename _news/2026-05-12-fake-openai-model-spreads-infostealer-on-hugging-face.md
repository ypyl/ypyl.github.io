---
layout: post
title: "Fake OpenAI Model Spreads Infostealer on Hugging Face"
date: 2026-05-12
---

A malicious repository on Hugging Face, **Open-OSS/privacy-filter**, masqueraded as an OpenAI privacy tool and climbed to the top of the platform's charts, racking up 244,000 downloads in just 18 hours. The repository distributed an info-stealer for Windows that escalated privileges via UAC bypass and added itself to Microsoft Defender exclusions. The stealer harvested passwords, cryptocurrency wallet data, Discord session tokens, and FileZilla configurations before wiping all traces from the system.

According to researchers at **HiddenLayer**, the attack infrastructure is linked to the Chinese hacker group **Silver Fox**. Hugging Face has since blocked access to the repository.

[The Hacker News](https://thehackernews.com/2026/05/fake-openai-privacy-filter-repo-hits-1.html)
