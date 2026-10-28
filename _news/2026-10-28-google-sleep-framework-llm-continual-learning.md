---
layout: post
title: "Google Proposes a 'Sleep' Stage So LLMs Keep What They Learn"
date: 2026-10-28
categories: news
---

Google Research has introduced **SLEEP**, a framework that adds a separate consolidation stage to continual learning so a model can absorb new material without losing older skills. The problem it targets is catastrophic forgetting: new information overwrites trained behaviour, and knowledge held in the context window never becomes permanent memory. SLEEP alternates two phases. In **wake**, the model works on incoming data and parks what it learns in short-term memory; in **sleep**, new input stops, the model activates additional MoE memory blocks, and that knowledge is written into their weights, which limits interference with what is already consolidated before the temporary memory is released. A further **dreaming** stage samples random combinations of experts to generate synthetic training examples, and only the most useful ones are used for further training. Reported results include recall over sequences up to **10M tokens** on BABILong by binding the information into parameters, almost no regression on previously learned languages in sequential translation training, and **79.2%** on AIME 2024 and **69%** on AIME 2025 in mathematics. The "sleep" here is a controlled fine-tuning loop run under rules the researchers set: the architecture and code stay fixed.

**Related:** [NVIDIA Introduces Human-Like Memory for LLMs with Test-Time Learning](/news/2026/03/11-nvidia-llm-memory-test-time-learning/), [MemT-1.0-Nano Keeps Long-Term Memory in Model State Instead of Context](/news/2026/10/21-memt-nano-long-term-memory-model/)

[The SLEEP framework](https://research.google/blog/the-sleep-framework-a-new-way-for-llms-to-retain-knowledge-in-continual-learning/)
