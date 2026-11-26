---
layout: post
title: "Tencent Open-Sources Hy-MT3 Translation Models for 102 Languages"
date: 2026-11-24
categories: news
---

Tencent released **Hy-MT3**, the third generation of its Hunyuan translation family, in three sizes: **1.8B**, **7B** and **30B-A3B** with 3B active parameters. Language coverage goes from 38 to **102** languages. Against Hy-MT2 on real-world scenario data, Tencent reports **43%** fewer mistranslations, **74%** fewer omissions or additions and **40%** fewer instruction-following errors.

The 30B-A3B model takes the best overall score among all evaluated models in non-thinking mode, ahead of Qwen3.8-Max, Kimi-K3.1-Pro and DeepSeek-V4.1-Flash, at under 1 TFLOP per sample, which is less than a thirtieth of what some of the models it beats consume. The 1.8B model beats commercial translation APIs (iFlytek Translator, Microsoft Translator, Qwen-MT-Flash) on general translation.

Weights are on Hugging Face under **Apache 2.0**, including FP8 builds and a GGUF build of the 1.8B model for offline translation through llama.cpp. The release also includes **TransRubric**, a rubric-based evaluation suite that beats other schemes on the WMT26 metrics shared task test set.

**Related:** [Tencent Opens Hy4 Weights Under a Custom License](/news/2026/11/23/tencent-opens-hy4-weights-under-custom-license/)

[Hy-MT3 paper](https://arxiv.org/abs/2611.27673) · [Hugging Face collection](https://huggingface.co/collections/tencent/hy-mt3) · [GitHub](https://github.com/Tencent-Hunyuan/Hy-MT3) · [Source channel](https://t.me/ai_machinelearning_big_data)
