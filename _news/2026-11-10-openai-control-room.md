---
layout: post
title: "OpenAI Builds Control Room to Halt Model Workloads Within 35 Seconds"
date: 2026-11-10
categories: news
---

OpenAI has built **Control Room**, a central system that gates every compute workload on its research infrastructure and can stop a model from producing output when a safety concern is raised. It covers training experiments, evaluations, agent runs and data processing. Each job gets a **job_id** tied to an approved **Safety Case**, and the inference engine asks Control Room for permission before every token generation, so revoking that permission makes the next request fail. The compute process itself can keep running; the model simply emits nothing further for it.

The stop is not instant: after a command, new output ends within **35 seconds**, because permissions are cached to absorb request volume. OpenAI says it plans to cut that latency to zero. Monitoring models that watch trained models for suspicious behaviour can trigger a stop on their own, and the system is **fail-closed**: if Control Room is unreachable, the inference engine rejects generation requests until it comes back. It is already live for all frontier-model training research, with the rest of the research workloads due in the coming days, and OpenAI runs automated checks for compute engines that skip or fail to implement the gate.

**Related:** [OpenAI Designates Astra as First Model with Critical Cyber Capabilities](/news/2026/09/01/openai-astra-first-critical-cyber-capability-model/), [OpenAI Slows Model Development Over Cyber Capabilities](/news/2026/08/19/openai-paces-model-development-cyber-capabilities/), [US Congress Bill Proposes Emergency Kill Switch for AI Systems](/news/2026/07/25/us-congress-ai-kill-switch-bill/)

[Control Room](https://openai.com/index/control-room/)
