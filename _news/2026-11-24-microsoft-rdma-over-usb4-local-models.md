---
layout: post
title: "Microsoft Runs One Local Model Across Three Mini-PCs Over USB4"
date: 2026-11-24
categories: news
---

Microsoft opened a developer preview of **RDMA over USB4** on Windows. Two or more USB4-capable PCs are connected with USB4 cables and split one model across their combined memory and GPU compute, through a new NetworkDirect provider and updated in-box USB4 drivers. There is no RDMA adapter to buy, no switch to configure and no third-party driver to install.

In Microsoft's lab, three mini-PCs with **AMD Ryzen AI Max+ 395**, Radeon 8060S graphics and 128 GB of unified memory each ran **Qwen3-235B-A22B** at about **18.5 tokens per second** with 4-bit quantization; AMD measured 21.98 tokens per second decode on its own copy of that setup. Tensor parallelism exchanges data many times per token, so latency sets the pace: RDMA over USB4 cut round-trip latency for small messages to about **28 to 50 microseconds**, roughly four times lower than Ethernet over the same link.

The provider and a llama.cpp-based testbed are open source. The preview is "coming soon" to Windows Insiders in the Experimental channel and is off by default.

**Related:** [llama.cpp and GGML Reach 1.0](/news/2026/11/13/llama-cpp-and-ggml-reach-1-0/)

[Introducing RDMA over USB4 on Windows](https://techcommunity.microsoft.com/blog/microsoftusbblog/introducing-rdma-over-usb4%C2%AE-on-windows-run-larger-local-ai-models-across-pcs/4574602) · [NetworkDirect provider](https://aka.ms/usb4-rdma-nd-provider) · [Source channel](https://t.me/ai_machinelearning_big_data)
