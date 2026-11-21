---
layout: post
title: "Thinking Machines Ships Inkling 2 Mini, a 26.4B Model Built to Run Offline on a Laptop"
date: 2026-11-21
categories: news
---

Thinking Machines released **Inkling 2 Mini** on November 20 under Apache 2.0, a mixture of experts with **26.4 billion total parameters, 3.4 billion active** and **5.4 billion weight activations per token**, a 1M token context and image input. Intermediate precision weights total about **20GB**, which the company says fits a 32GB Mac, an NVIDIA DGX Spark or a 32GB GPU. The team flew with it on a laptop, where it ran in airplane mode and built a playable flight simulator in a few minutes.

The architecture is the part worth following. Inkling 2 Mini is a **looped transformer**: 24 stored layers are split into four blocks of six, and every token passes through the two middle blocks three times, so 24 layers of weights do the work of 48. The pitch is that the model reasons without writing the reasoning down, which keeps the chain of thought short and readable while the extra passes do the thinking. On the accompanying variable-loop research release, no-CoT time horizon moved from **16 seconds at one pass to 37 seconds at three**, and serial reasoning kept improving out to **ten loops**, twice the depth it was trained on, with the chain of thought growing shorter but staying legible. Looping did not raise scores on the company's agentic-sabotage tests.

Against **Qwen3.8-27B** it takes DeepSWE v1.1 at **48.2% versus 42.2%**, SWE-bench Verified bash-only at **79.2% versus 77.8%** and SWE-bench Pro v2 Hard at **77.9% versus 73.8%**, while losing Terminal Bench v2.1 at **71.6% versus 73.0%** and tying the Artificial Analysis Intelligence Index at **20.1**. Thinking Machines concedes the limit in the same post: looping buys reasoning, not facts. Knowledge benchmarks stay in line with models of the same size, so the lead comes from tool and search use.

**Related:** [Thinking Machines Opens Inkling 2 Small Weights and Details a Training Run That Broke a Network Block](/news/2026/11/15/thinking-machines-opens-inkling-2-small-weights/), [Thinking Machines Releases Inkling 2 Small With 594B Parameters](/news/2026/11/06/thinking-machines-releases-inkling-2-small/)

[Introducing Inkling 2 Mini](https://thinkingmachines.ai/news/inkling-2-mini/) · [Inkling 2 Mini on Hugging Face](https://huggingface.co/thinkingmachines/Inkling-2-Mini) · [Source channel](https://t.me/ai_machinelearning_big_data)
