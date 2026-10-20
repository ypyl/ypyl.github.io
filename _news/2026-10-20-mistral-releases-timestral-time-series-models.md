---
layout: post
title: "Mistral Releases Timestral, a CPU-First Time Series Forecasting Family"
date: 2026-10-20
tags: [news]
categories: news
---

Mistral has introduced **Timestral**, which it calls the first family of models built specifically for probabilistic multivariate time series analysis. The line has four configurations from **5M to 209M parameters**, and the largest one tops the main time series forecasting benchmarks, ahead of the bigger **Chronos-2** and **TimesFM-3**.

The architecture is tuned for CPUs rather than GPUs: the 209M model produces a one-day forecast in **140 ms on eight threads**, and the 5M model does it in **10 ms**. Context covers up to **32,768 timesteps**, and the output is a probability distribution across **21 quantiles** rather than a single point estimate.

Weights and code are published under a **modified MIT license**, with enterprise deployment and fine-tuning on private data offered through Mistral Forge.

**Related:** [Google Open-Sources TimesFM-3 Time Series Forecasting Model](/news/2026/09/01/google-open-sources-timesfm-3/)

[Timestral](https://mistral.ai/news/timestral/) · [Weights on Hugging Face](https://huggingface.co/collections/mistralai/timestral) · [Code on GitHub](https://github.com/mistralai/timestral)
