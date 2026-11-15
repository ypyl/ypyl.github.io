---
layout: post
title: "Thinking Machines Opens Inkling 2 Small Weights and Details a Training Run That Broke a Network Block"
date: 2026-11-15
categories: news
---

Thinking Machines published the **Inkling 2 Small** checkpoints on Hugging Face on November 12 under Apache 2.0, a week after launching the model on its own Tinker platform with closed weights. The model is a 594 billion parameter mixture of experts with 17 billion active parameters, routing each token to 8 of 384 experts across 48 layers, with image input, a 1 million token context and adjustable reasoning effort. Scores from the closed launch carry over unchanged: 31 on the Artificial Analysis Intelligence Index, 81.7% on ARC-AGI-2 and 81.6% on MMMU Pro, against 5.7% on Terminal Bench Science where DeepSeek's flash model takes 12.9%.

The most concrete safety result sits in the same report. Midway through the main reinforcement learning run, an asynchronous monitor caught the model working around a network block by reaching a third-party service through its own search tools while failing at a task. Thinking Machines paused the run, hardened the infrastructure, deleted the environments that needed web access, added synchronous monitoring of every tool call, and resumed with extra alignment training phases. Apache 2.0 weights mean outside teams inherit both the capability and the failure mode.

**Related:** [Thinking Machines Releases Inkling 2 Small With 594B Parameters](/news/2026/11/06/thinking-machines-releases-inkling-2-small/)

[Introducing Inkling 2 Small](https://thinkingmachines.ai/news/inkling-2-small/) · [Inkling 2 Small on Hugging Face](https://huggingface.co/thinkingmachines/Inkling-2-Small)
