---
layout: post
title: "GitHub Copilot Local Sandboxing Reaches General Availability"
date: 2026-10-09
tags: news
categories: news
---

GitHub has shipped **local sandboxing** for **GitHub Copilot** to general availability in Copilot CLI, the Copilot app, and VS Code sessions using Agent Host. Commands and tools started by the agent run inside an execution boundary that limits access to the host filesystem, network, credentials, and environment, based on policies set by the developer or their organization.

- **Engine**: local sandboxing runs on **Microsoft eXecution Container (MXC)**, which maps one sandbox policy onto native operating-system controls across Windows, macOS, and Linux, such as bubblewrap on Linux, Seatbelt on macOS, and process containers on Windows.
- **What is restricted**: readable and writable files and directories, internet and local network access, Git and GitHub CLI credentials, plus local MCP and language servers where supported. Tool execution is sandboxed regardless of which model Copilot uses.
- **Governance and cost**: enterprise-managed settings can require sandboxing and lock policies so developers cannot weaken them. Local sandboxing is included with Copilot at no additional cost and runs on the developer's own machine.
- **Local models**: Copilot CLI version `1.0.94-0` adds `/model` discovery from a running Ollama instance, where discovered models must be reviewed and confirmed before use. Developers can also pick **MAI Code 1.1 Flash** through the Windows ML provider, or connect an OpenAI-compatible local endpoint. Selecting a local model does not enable offline mode, which stays an explicit choice via `COPILOT_OFFLINE=true`.

**Related:** [Microsoft Execution Containers Reach General Availability](/news/2026/10/08/microsoft-execution-containers-generally-available/), [Microsoft Puts Hybrid Intelligence at the Center of Windows and Surface](/news/2026/10/09/microsoft-windows-surface-hybrid-intelligence-keynote/), [Google Launches Agent Sandbox on GKE and Open-Sources Agent Substrate](/news/2026/06/24/google-agent-sandbox-gke-agent-substrate/)

[Local sandboxing for GitHub Copilot now generally available](https://github.blog/changelog/2026-10-07-local-sandboxing-for-github-copilot-now-generally-available/) · [Bringing local models and sandboxed tools to Windows and GitHub Copilot](https://commandline.microsoft.com/local-models-sandboxed-tools-github-windows/)
