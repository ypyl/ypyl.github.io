---
layout: post
title: "WSL Containers Reaches General Availability"
date: 2026-09-30
tags: news
categories: news
---

Microsoft has shipped **WSL containers** to general availability, letting developers build, run and deploy Linux containers directly on Windows. Install with `wsl --update`, and you get the **`wslc.exe`** CLI (with a `container.exe` alias for familiar commands) plus an API for driving Linux containers programmatically from native Windows apps, which Microsoft pitches for running local AI workloads or cloud-based containerized apps on a laptop.

What arrived with GA:

- **Lifecycle and networking commands**: `wslc container restart`, `wslc container cp`, `wslc system info`, `wslc network connect` and `disconnect`, `wslc events`, container health checks, `--stop-timeout`, `--mount`, and a configurable storage path for the default session.
- **Enterprise controls**: the Microsoft Defender for Endpoint plugin for WSL now surfaces container process, file and network activity joined to the Windows host, while Intune can enable or disable WSL containers and restrict image pulls to an approved registry allow list.
- **Ecosystem support**: `wslc` is now a driver for VS Code dev containers, Aspire treats it as a first-class container runtime, and community projects such as Lazywslc and WSL Container Desktop manage it from a TUI and a WinUI 3 app.
- **Roadmap**: `wslc compose` is the top feature request and the next focus, aiming for `wsl compose up` against existing `compose.yaml` files unchanged. Microsoft also reports up to 2x faster access to Windows files from Linux environments and a new `consomme` network mode.

**Related:** [Microsoft Unveils GitHub Copilot Desktop App, Frontier Tuning, and AI-Native Developer Tools](/news/2026/06/03/github-copilot-desktop-app-frontier-tuning-developer-tools/)

[WSL containers is now generally available](https://blogs.windows.com/windowsdeveloper/2026/09/29/wsl-containers-now-generally-available/)
