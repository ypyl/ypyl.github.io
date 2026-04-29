---
layout: post
title: "Open Retro Model Trained on Pre-1930 Data"
date: 2026-04-29
tags: news
categories: news
---

Alek Radford, creator of GPT-1, GPT-2, and CLIP, together with the University of Toronto, has trained a 13B parameter model exclusively on texts published before December 31, 1930. The training dataset includes 260 billion tokens from English books, newspapers, patents, and court archives.

The model, evaluated by Sonnet 4.6, contains no code in its training data, protecting it from benchmark leakage, but it can still write simple Python scripts on HumanEval. Perplexity increases on post-1930 texts, peaking on materials from the 1950s and 60s.

The team plans to scale the dataset to 1 trillion tokens by summer and release a retro model comparable in capability to ChatGPT.

Source: [talkie-lm.com](https://talkie-lm.com/introducing-talkie)