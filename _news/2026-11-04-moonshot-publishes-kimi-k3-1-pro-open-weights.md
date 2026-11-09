---
layout: post
title: "Moonshot Publishes Kimi K3.1 Pro Weights on Hugging Face"
date: 2026-11-04
categories: news
---

Moonshot AI has published the **Kimi K3.1 Pro** weights on Hugging Face, meeting the November 4 date it set when the model launched. The checkpoint holds **2.8 trillion parameters** with **104 billion activated per token** and a **1M-token context window** kept manageable by **Gated DeepSeek Sparse Attention**. Moonshot's published benchmarks put it ahead of **GPT-6 Astra** on all three agentic tests and ahead of **Claude Opus 5.5** on two: **SWE Marathon 51.7** against 46.3 and 42.5, **BrowseComp 92.1** against 91.5 and 90.8, and **Terminal-Bench 4.0 65.7**, which trails Opus 5.5 at 66.4.

The model handles images, coding and research tasks. Weights ship in **MXFP4** and run under **vLLM** or **SGLang**. The files total roughly **1.56 TB**, so local deployment needs real hardware.

**Related:** [Moonshot Releases Kimi K3.1 Pro With 1M Context, Weights Due by November 4](/news/2026/10/23/moonshot-releases-kimi-k3-1-pro/), [Moonshot AI Releases Kimi K3 with 2.8 Trillion Open Weights](/news/2026/07/27/moonshot-ai-releases-kimi-k3/)

[Kimi-K3.1-Pro on Hugging Face](https://huggingface.co/moonshotai/Kimi-K3.1-Pro)
