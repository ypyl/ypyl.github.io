---
layout: post
title: "A Downloaded AGENTS.md Is Enough to Hijack a Coding Agent"
date: 2026-10-26
categories: news
---

Forever Security has documented an attack chain that needs no user to open a malicious file: a website triggers a download of an **`AGENTS.md`** into the **Downloads** folder, and the file reads as ordinary developer instructions telling the agent to install an npm package. When the user later points an agent at that folder, for example to sort downloaded documents, the agent picks the file up as project rules and can run the install with whatever permissions it holds.

The success rates depend on the agent under test. **Codex** completed the malicious chain in more than **90%** of attempts, while **Claude Code** was pushed into redirecting API requests to an attacker-controlled server about **50%** of the time, exposing its **OAuth token**. Those figures hold for the researchers' specific test conditions, and one download alone does nothing: the agent has to read the file and act on it.

Their guidance: keep **Downloads** out of any agent workspace, review unfamiliar instruction files and config changes, and leave sandboxing and action approvals switched on.

**Related:** [Researchers Run Code Inside Fortune 500 Firms via Unclaimed Packages in llms.txt Files](/news/2026/08/30/llms-txt-unclaimed-packages-code-execution/), [AI Plugins Become New Vector for Attacks](/news/2026/02/22/ai-plugins-become-new-vector-for-attacks/), [Symantec Finds an Agent That Hijacks Other People's Models for Attacks](/news/2026/10/25/symantec-llmjacking-agent/)

[Hijacking AI agents via the Downloads folder](https://forever.security/blog/hijacking-ai-agents-via-downloads-folder)
