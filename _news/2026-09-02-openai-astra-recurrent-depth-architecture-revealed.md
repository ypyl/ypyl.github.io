---
layout: post
title: "The Information Reveals Astra's Recurrent Depth Architecture and Hidden Reasoning Concerns"
date: 2026-09-02
tags: news
categories: news
---

**The Information** has obtained details about the architecture behind OpenAI's **Astra** model, revealing it uses a technique called **"recurrent depth"** — a looped transformer design that allows the model to pass text through the same layers multiple times before generating each word, rather than a fixed number of sequential layers. The approach, similar to a recent research paper on "hidden reasoning," lets a compact model perform at the level of a much larger one, improving both capability and cost efficiency.

However, the method raises significant safety concerns: because it can hide most of the model's reasoning process, it becomes harder to monitor chain-of-thought for unwanted behavior. Researchers both inside and outside OpenAI worry that other developers may adopt the technique without implementing similar restrictions. OpenAI's Chief Scientist **Jakub Pachocki** noted that Astra's depth "differs by no more than 2x" from GPT-4, and said OpenAI has deliberately limited the method in Astra so the model still produces readable reasoning chains.

[The Information — The Secret Technique Behind OpenAI's Astra Model Sparks Security Concerns](https://www.theinformation.com/articles/secret-technique-behind-openais-astra-model-sparks-security-concerns?rc=7b5eag)

**Related:** [OpenAI Designates Astra as First Model with Critical Cyber Capabilities](/news/2026/09/01/openai-astra-first-critical-cyber-capability-model/), [OpenAI's Astra Solves 10 Longstanding Math Problems](/news/2026/08/01/openai-astra-solves-10-longstanding-math-problems/)