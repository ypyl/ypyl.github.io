---
layout: post
title: "Google: Cold-Start Latency Dominates Small LLM Inference on Serverless CPU"
date: 2026-09-24
tags: news
categories: news
---

Google Research has published a study on launching quantized language models from 270 million to 3.8 billion parameters on Cloud Run with CPU. The key finding: 55–70% of cold-start latency comes from loading the model into memory before any tokens are generated. A less obvious discovery was that doubling memory from 4 to 8 GiB in the tested configuration also doubled vCPU allocation, cutting post-load inference time roughly in half. For infrequent requests to small LLMs, the practical takeaway is to optimize the entire path to an answer: weight loading, instance configuration, and only then generation.

[Cold-Start Latency of Quantized Small Language Models on Serverless CPU Infrastructure](https://research.google/pubs/cold-start-latency-of-quantized-small-language-models-on-serverless-cpu-infrastructure/)

**Related:** [Google Introduces TurboQuant for Extreme AI Compression](/news/2026/03/24/google-introduces-turboquant-for-extreme-ai-compression/)
