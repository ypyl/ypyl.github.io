---
layout: post
title: "Tencent Opens 770B Hy4 Weights Under a License That Gates Large Model-as-a-Service Providers"
date: 2026-11-23
categories: news
---

Tencent published the finished **Hy4** weights on November 22: a mixture of experts with **770B total parameters and 49B active per token**, a 1M token context, and image and video input. The backbone is 78 layers, 77 of them MoE with 256 routed experts plus one shared expert and top-8 routing, behind Gated DeepSeek Sparse Attention with cross-layer index reuse. A 3.3B **DFly2** drafter shares the model's KV cache and sparse indices, so speculative decoding costs the same at any context length and adds no draft KV memory.

The license changed from the preview. Hy4-preview shipped under Apache 2.0; the final weights ship under the **Tencent Hy License 1.0**, which is Apache 2.0 plus a Section 10: a company offering Hy4 as an API or fine-tuning service needs written permission from Tencent once its gross revenue passes **$50M in any 12 months**, or one of its products passes **100M monthly active users**. Internal deployments and products where the model sits behind specific features are exempt. Weights, an FP8 build and the base checkpoint are on Hugging Face, with an 80-page system card covering cyber and bio risk.

Tencent benchmarked itself against GPT-6.1 Sol and Claude Opus 5.5 in its own harnesses. Hy4 takes **APEX-Agents v1.1 at 75.2** against 60.0 and 74.8, and **OSWorld 2.1 at 78.9** against 75.2 and 81.8, while **DeepSWE v1.1** comes out near level at **74.9** against 75.2 and 74.2. Long-horizon coding and science are where it falls away: **Terminal-Bench 4.0 at 44.4** against 55.9 and 66.4, **Terminal-Bench-Science at 19.8** against 57.0 and 58.7, and **MMMU-Pro at 78.6** against 84.9 and 88.2. The model card states the same gap, naming research-level mathematics, long-horizon coding and advanced visual question answering. On the finance and office workflows Tencent runs internally, it reports the hallucination rate dropping from 15.76% on the preview to 5.45%.

On Tencent Cloud the API lists at **$0.834 per million input tokens and $2.501 per million output**, at half price through December 31, 2026.

**Related:** [Tencent Releases Hy4-Preview, 770B Open-Weight MoE Model](/news/2026/08/28/tencent-releases-hy4-preview-open-weights/), [Tencent Open-Sources Hy-Nomas, an Interactive Video World Model That Runs on One RTX 4090](/news/2026/10/23/tencent-releases-hy-nomas-interactive-video-world-model/)

[Hy4 on Hugging Face](https://huggingface.co/tencent/Hy4) · [Tencent Hy License 1.0](https://huggingface.co/tencent/Hy4/blob/main/LICENSE) · [Channel](https://t.me/ai_machinelearning_big_data)
