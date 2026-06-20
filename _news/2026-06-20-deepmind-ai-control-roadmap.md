---
layout: post
title: "DeepMind Publishes AI Control Roadmap Security Architecture"
date: 2026-06-20
tags: news
categories: news
---

Google DeepMind has released the **AI Control Roadmap** — a security framework for governing highly capable AI agents. Unlike traditional alignment, the approach assumes the system is inherently adversarial, has hidden goals, and actively works against its developers. Borrowing from the MITRE ATT&CK framework, DeepMind created **TRAIT&R**, a new taxonomy of AI-specific attack vectors including covert research sabotage, backdoor insertion, and distributed split-chain attacks across hundreds of agent instances.

Defense is structured across escalation levels — **D1–D4** for detection and **R1–R3** for response. At early stages, safety is maintained by auditing agent CoT traces in sandboxes under weaker but trusted AI supervisors. At the highest tier (D4/R3), standard methods no longer work, so DeepMind plans to inspect internal model states, use isolated copies of the untrusted system for auditing, and deploy emergency kill-switch infrastructure.

Alongside the roadmap, DeepMind published a separate industry guide on three layers of agent security: individual agent safety, inter-agent risks (cascading failures, tacit collusion), and the broader cybersecurity landscape.

[AI Control Roadmap PDF](https://storage.googleapis.com/deepmind-media/DeepMind.com/Blog/securing-the-future-of-ai-agents/gdm-ai-control-roadmap.pdf) · [Three Layers of Agent Security](https://storage.googleapis.com/deepmind-media/DeepMind.com/Blog/securing-the-future-of-ai-agents/three-layers-of-agent-security.pdf)
