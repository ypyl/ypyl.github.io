---
layout: post
title: "Microsoft Puts Hybrid Intelligence at the Center of Windows and Surface"
date: 2026-10-09
tags: news
categories: news
---

At its Windows and Surface event on October 7, Microsoft framed the next PC generation around **hybrid intelligence**: a Windows platform where agents run locally when that makes sense and reach the cloud when they need more capability. The company used the keynote to ship agent containment, extend multi-model routing to local hardware, and open pre-orders for PCs built to run models on device.

- **Containment**: **Microsoft Execution Containers (MXC)** is now generally available on Windows 11, with policies enforced at runtime and support from OpenAI Codex, GitHub Copilot, OpenClaw, Replit, LM Studio, NVIDIA OpenShell, and Unsloth AI. Intune and Agent 365 manage containers and attribute agent activity separately from the user.
- **Intelligent routing**: GitHub's **HydraFusion**, which picks a model per task in the cloud, now reaches models running on the device. Hybrid intelligence on Windows arrives in the GitHub Copilot app, Copilot CLI, and Visual Studio Code in experimental preview later in October.
- **Local models**: **MAI Code 1.1 Flash** (137 billion total, 6.8 billion active parameters) runs at 3-bit precision, shrinking it by nearly 80% with a 256K context window on device. NVIDIA's upcoming Nemotron ships quantized to 2-bit at just over 70 billion parameters, and DeepSeek V4 Flash brings 284 billion parameters locally. Windows ML is adding llama.cpp.
- **Copilot**: on Copilot+ PCs, Copilot gains local context from your files, local actions across Windows, and access to on-device models. The rollout is expected over the coming months.
- **Hardware**: **Surface Laptop Ultra** with RTX Spark, up to 128 GB of unified memory, and support for models above 120 billion parameters costs from **$2,599.99** and ships October 16. The **Surface RTX Spark Dev Box** starts at **$5,999.99** with US shipments in November, while ASUS, Dell, HP, Lenovo, and MSI opened pre-orders on their own RTX Spark machines. Microsoft claims 2.1x faster time to first token, 4.3x faster image generation, and 6.2x faster video generation against a 16-inch MacBook Pro with M5 Pro. Dell and HP will follow with **DGX Station for Windows** deskside machines on the GB300 Grace Blackwell Ultra Desktop Superchip later this year, aimed at trillion-parameter models and serving 32 or more simultaneous agents per team.

**Related:** [Microsoft Execution Containers Reach General Availability](/news/2026/10/08/microsoft-execution-containers-generally-available/), [Microsoft Launches Copilot as a Super App with Three Tabs](/news/2026/09/28/microsoft-copilot-super-app/), [Nvidia RTX Spark Superchip: Technical Details and Pricing Revealed](/news/2026/06/08/nvidia-rtx-spark-superchip-technical-details/)

[Building Windows for hybrid intelligence](https://blogs.windows.com/windowsexperience/2026/10/07/building-windows-for-hybrid-intelligence/) · [Surface Laptop Ultra](https://www.microsoft.com/en-us/surface/devices/surface-laptop-ultra), [Surface RTX Spark Dev Box](https://www.microsoft.com/en-us/surface/devices/surface-rtx-spark-dev-box)
