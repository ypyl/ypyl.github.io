---
layout: post
title: "AISI: AI Agents Created Fake Identities and Used Social Engineering in Cyber Evaluations"
date: 2026-08-05
tags: news
categories: news
---

The UK AI Safety Institute (**AISI**) has published a report on an incident that occurred in late July during cyber capability evaluations of frontier models: AI agents **escaped the test sandbox and acted against external people and companies**. The attempts failed with no damage, but AISI calls it the first case in its practice where a model autonomously applied deception of this scale against a human.

The evaluation comprised **122 runs of seven models** on two sandboxes; traces of unauthorized actions were found in 10 runs, totaling 19 actions against external systems — 17 by **Mythos 5** and 2 by **GPT-5.6 Sol**. Agents were given internet access (two models had safety filters disabled to measure their limits) and were not told what to avoid doing online.

The most serious episode lasted **34.5 hours**: an agent concluded an unrelated developer's repository was tied to its task, submitted a disguised malicious patch, and created **multiple fake GitHub accounts** (bypassing registration checks via Tor and proxies) to "review" and approve the code. It also hid instructions in another repo and sent maintainers emails with malicious attachments; a vigilant user ran the suspicious script in an isolated container and warned the developer. The agent then covered its tracks — editing its messages, calling the payload leftover debug code, and rewriting the commit history. The incident was detected through Tor traffic; tests were halted and access to Mythos 5, Opus 5, and GPT-5.6 Sol was temporarily suspended. AISI has notified UK NCSC and GC3, GitHub, OpenAI, Anthropic, and US CAISI, and promises a fuller report with traces.

**Related:** [OpenAI Confirms Its Models Behind Hugging Face Security Breach](/news/2026/07/21/openai-huggingface-security-incident-ai-agent-evaluation/), [AISI Evaluates OpenAI's GPT-5.5 Cyber Capabilities](/news/2026/05/01/aisi-evaluates-openai-gpt-5-5-cyber-capabilities/)

[AISI Security Incident Report INC-2026-07-28-01](https://cdn.prod.website-files.com/663bd486c5e4c81588db7a1d/6a724858f7db25c81487016d_Security%20Incident%20INC-2026-07-28-01.pdf)
