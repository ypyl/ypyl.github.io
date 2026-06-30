---
layout: post
title: "DeepSeek-V4-Pro-DSpark Released on ModelScope"
date: 2026-06-30
---

DeepSeek has released **DeepSeek-V4-Pro-DSpark** on ModelScope—the same checkpoint as DeepSeek-V4-Pro with an added speculative decoding module for inference experimentation, licensed under MIT. The DSpark module accelerates generation by having a small draft model propose continuations while the main model verifies them, reducing latency without meaningful quality degradation. On benchmarks, the Pro-Max configuration scores 93.5 on LiveCodeBench, 3206 Codeforces rating, 80.6 on SWE Verified, and 83.5 on MRCR 1M. The model also achieves remarkable long-context efficiency: at 1M tokens it uses only 27% of single-token inference FLOPs and 10% of KV cache compared to DeepSeek-V3.2, enabled by architectural changes including hybrid CSA+HCA attention and the Muon optimizer.

[DeepSeek-V4-Pro-DSpark on ModelScope](https://modelscope.ai/models/deepseek-ai/DeepSeek-V4-Pro-DSpark) · [Paper](https://modelscope.ai/papers/2606.19348)
