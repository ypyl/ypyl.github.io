---
layout: post
title: "Microsoft Execution Containers Reach General Availability"
date: 2026-10-08
tags: news
categories: news
---

Microsoft has shipped **Microsoft Execution Containers (MXC)** to general availability, a policy-driven containment layer that runs untrusted code and agent workloads inside an OS-enforced boundary. Developers declare the files and network destinations a workload needs in a JSON policy, and MXC picks a backend to enforce it, with the policy kept outside the agent's control so generated code cannot grant itself more access.

- **Four backends**: process containers (Windows 11, macOS, Linux via AppContainer, Seatbelt or Bubblewrap), a Windows-only session container with its own desktop and clipboard, WSL containers, and an experimental hardware-isolated MicroVM.
- **Three modes**: enforcement, learning (denials blocked and written to a JSON activity report), and permissive (denials recorded but allowed) so teams can draft a least-privilege policy before enforcing it.
- **Ecosystem**: GitHub Copilot, OpenAI Codex, Replit, LM Studio and Unsloth AI support MXC today; Claude Code, Perplexity, Manus, Box and others are listed as upcoming. NVIDIA integrated OpenShell for file, network and credential controls.
- **Governance**: Microsoft Entra will separate agent activity from user activity in Agent 365, and Intune policy to manage MXC containers on Windows 11 is coming soon.

**Related:** [WSL Containers Reaches General Availability](/news/2026/09/30/wsl-containers-generally-available/), [Microsoft Open-Sources run-assert-eval for AI Agent Risk Discovery](/news/2026/09/25/microsoft-releases-run-assert-eval-agent-risk-discovery/)

[Microsoft Execution Containers: Policy-driven containment for AI agents](https://blogs.windows.com/windowsdeveloper/2026/10/07/microsoft-execution-containers-policy-driven-containment-for-ai-agents/)
