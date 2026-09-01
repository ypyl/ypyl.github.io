---
layout: post
title: "Google Open-Sources TimesFM-3 Time Series Forecasting Model"
date: 2026-09-01
tags: news
categories: news
---

**Google** has released the weights of **TimesFM-3**, a **330M-parameter** foundation model for time series forecasting that analyzes multiple interrelated metrics together with external variables. Internally it combines patch masking with a two-dimensional attention grid, alternately attending to data over time and to connections between series. The model produces its entire forecast horizon in a single pass, so errors do not accumulate step by step and inference is faster. Trained on a trillion data points, it requires no fine-tuning for specific tasks; Google says it leads the **Gift-Eval**, **FEV-Bench**, and **Time** benchmarks. Weights are available on **Hugging Face**, with BigQuery support to follow in the coming weeks.

[TimesFM-3: A Zero-Shot Foundation Model for Multivariate Forecasting](https://research.google/blog/timesfm-3-a-zero-shot-foundation-model-for-multivariate-forecasting/)
