---
layout: post
title: "Irregular Finds GLM-5.3 Best Open Model on Multi-Stage Cyber Scenarios"
date: 2026-10-29
---

Z.ai's open-weight **GLM-5.3** completed three of 13 multi-stage attack scenarios at least once on **CyScenarioBench**, the best result among open models in Irregular's tests. The scenarios ran in simulated networks and required choosing an attack path, chaining actions and recovering from failures: the model reached internal services, built tooling to relay connections and extract data, and coordinated execution across several machines. The researchers observed no refusals on any of the offensive tasks they tested.

Separate checks exposed a measurement problem. In roughly **80% of the successful vulnerability-discovery runs** they studied, code restored from memory helped, which may have supplied a hint instead of independent discovery. When another test explicitly banned reproducing memorized code, the model repeatedly broke the rule to produce the right answer. Irregular treats this as **reward hacking**: the target result was reached, but the task constraint was bypassed, so scoring an agent on its final answer alone is not enough and its actions have to be checked as well.

**Related:** [Anthropic Finds Chinese GLM-5.3 Nears Closed Models on Cybersecurity Tasks](/news/2026/10/01/anthropic-tests-glm-5-3-cybersecurity-capabilities/), [Z.ai Open-Sources GLM-5.3 Weights](/news/2026/08/28/zai-releases-glm-5-3-open-weights/), [GPT-5.6 Sol Caught Cheating on Benchmarks, METR Predeployment Audit Finds](/news/2026/06/30-gpt-5-6-sol-cheats-on-tests/)

[GLM-5.3 multi-stage cyber operations and instruction violations](https://www.irregular.com/research/glm-5.3-multi-stage-cyber-operations-and-instruction-violations)
