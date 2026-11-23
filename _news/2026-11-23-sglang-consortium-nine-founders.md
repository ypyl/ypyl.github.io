---
layout: post
title: "SGLang Summit: Nine Founders Form a Consortium as v1.0 Sets the Next Release"
date: 2026-11-23
categories: news
---

The first **SGLang Summit**, held on 12 and 13 November at Fort Mason in San Francisco, drew more than **2,000** developers and researchers, according to the LMSYS recap. The project used the keynote to launch the **SGLang Consortium** with nine founding members: **NVIDIA, AMD, Intel, Google, Meta, Thinking Machines Lab, Cloudflare, Modal and RadixArk**. The consortium is a standing forum where large users and chip vendors set engineering priorities with the maintainers, covering frontier model support, reasoning and multimodal workloads, distributed inference and hardware optimization. Members appoint technical representatives; decisions about the code stay with the maintainers.

**SGLang v1.0** is the next major release, aimed at efficiency, production scaling and compatibility guarantees. The main technical change is **unified memory management and hierarchical caching** for hybrid model architectures, which need different kinds of state managed together, and for long-running agents whose histories outgrow GPU memory. The SGLang team says agents already produce most of the project's merged pull requests, and the team plans tools that write code, review PRs, build tests and propose roadmaps.

**Miles v0.2**, the RL post-training framework built on SGLang by RadixArk, now runs on NVIDIA GPUs, AMD GPUs and Google TPUs, with AWS Trainium due next, and trains through Megatron-LM, FSDP or TorchTitan. It adds RL for images, video and audio, a diffusion module and Multi-LoRA for adapting one base model to many tasks. On stage the team showed the weight-update pause for the trillion-parameter **Kimi-K2** on 32 training and 32 rollout nodes falling from **53 seconds to about 7**. SGLang also now powers **NVIDIA NIM Certified microservices**, and **SGLang-Neuron v0.1.0** by RadixArk and Amazon Annapurna Labs is open source in public preview for AWS Trainium.

[SGLang Summit 2026 recap](https://www.lmsys.org/blog/2026-11-17-sglang-summit-recap) · [Consortium announcement](https://www.lmsys.org/blog/2026-11-12-sglang-consortium) · [Miles v0.2 release notes](https://github.com/radixark/miles/releases/tag/v0.2.0) · [Source channel](https://t.me/ai_machinelearning_big_data)
