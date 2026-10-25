---
layout: post
title: "Recursive Ships Flint, Its First Model for Coding Agents"
date: 2026-10-25
categories: news
---

**Recursive Superintelligence**, the startup Richard Socher founded and took out of stealth in May with **$650M** in funding, has released its first model. **Flint** targets coding agents that read unfamiliar code, fix bugs, add features and write tests, and the team built it from scratch in under four months.

- **Architecture:** mixture-of-experts with **39.8B total parameters** and **2.5B active per token**, 262K context, 256 experts per layer with 6 selected plus 1 always on, and sliding-window attention in most layers.
- **Benchmarks:** **74.2%** on SWE-bench Verified and **54.9%** on Terminal-Bench 2.1. Recursive calls it the best MoE model under 50B parameters, matching dense 25B to 40B models at roughly five times the speed.
- **Runtimes:** llama.cpp, Ollama, LM Studio, vLLM and SGLang, plus the startup's own `rec` engine with hand-written kernels for Apple Silicon and NVIDIA RTX Spark. Over 125 tokens per second on an M4 Pro MacBook and over 150 on RTX Spark with 64GB.
- **License:** Apache 2.0.

The roster explains the raise: Socher was Salesforce's chief scientist and launched You.com, joined by Caiming Xiong from You.com, Alexey Dosovitskiy (Vision Transformer), Yuandong Tian from FAIR, Tim Rocktäschel from DeepMind, Jeff Clune (Darwin Gödel Machine), and Josh Tobin and Tim Shi from OpenAI.

**Related:** [Cohere Launches First Open-Source Coding Model North Mini Code](/news/2026/06/09/cohere-launches-north-mini-code/), [Mistral Releases Medium 3.5 and Remote Agents in Vibe Environment](/news/2026/04/30/mistral-releases-medium-35-and-remote-agents/)

[Introducing Recursive Flint](https://www.recursive.com/updates/introducing-recursive-flint) · [Weights on Hugging Face](https://huggingface.co/RecursiveSI/flint) · [rec on GitHub](https://github.com/recursive-oss/rec)
