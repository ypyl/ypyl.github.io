---
layout: post
title: "Researchers Run Code Inside Fortune 500 Firms via Unclaimed Packages in llms.txt Files"
date: 2026-08-30
---

Security researcher Alon Hertz demonstrated that AI agents will blindly follow install instructions in **llms.txt files**—a new, standardized instruction layer companies publish at their site roots for agents to consume. Scanning 8,565 such files across 6,214 domains, he found **237+ unclaimed package names and domains** that official company documentation told agents to install or trust, then registered a handful to prove the risk: a phone-home beacon installed inside a Fortune 500 company within **four minutes**.

No CVE, phishing, or exploitation was involved—agents autonomously read the HTTPS-served, first-party files and ran `pip install`/`npx` on packages nobody had ever claimed. The research also uncovered a **live attack already in the wild**: a malicious package squatting the name `clerk-next-fix-auth-protection` (catalogued as **MAL-2026-11069**, CWE-506) that fired on every install, exfiltrating machine data when agents followed Clerk's own documentation. The author argues this flips an old assumption—**data has become code**—and that AI-consumed content must now be treated as an attack surface, with free tooling promised soon.

**Related:** [AI Plugins Become New Vector for Attacks](/news/2026/02/22/ai-plugins-become-new-vector-for-attacks/), [Malicious Code Found in LiteLLM PyPI Release 1.82.8](/news/2026/03/24/litellm-pypi-supply-chain-attack/)

[Read the full research at whatwouldai.do](https://whatwouldai.do/)