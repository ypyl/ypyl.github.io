---
layout: post
title: "Anthropic Returns to Single Neurons for Safety Tools"
date: 2026-11-26
categories: news
---

Jasmine Shone and Lisa Dunlap, working in the Anthropic Fellows program, published [two posts](https://alignment.anthropic.com/2026/neuron-basis-applications/) on November 23 that argue individual neurons are more useful than the field assumed. Part 1 answers the three standing objections: neurons contribute two unrelated concepts at once in about **1% or fewer** of sampled cases, a neuron's output keeps roughly the same meaning across layers, and facts are spread out but stored interpretably.

Part 2 turns the neuron output vectors into untrained detectors and steering tools. Zero-label neuron detectors match or beat probes trained on **40 labels on 7 of 16** evaluation sets, and on 4 sets they match probes trained on **400 labels**. Steering with selected neurons produced the target behaviour in **21%** of responses against 2% without, about the same rate as trained steering vectors.

The auditing result is the practical one. Across **174 audits** of models with a hidden behaviour, an auditor reading only the answers was right 40% of the time, with steering 61%, and with neuron readouts **75%**. On WMDP-cyber, across **23 unlearned checkpoints**, the correct answer kept the highest probability for most questions the original model got right, and restoring a few dozen of roughly **460,000 neurons** restored the ranking. The authors conclude most unlearning methods they tested hide hazardous knowledge rather than remove it.

[Returning to the Neuron Basis, Part 1](https://alignment.anthropic.com/2026/neuron-basis-theory/) · [Part 2](https://alignment.anthropic.com/2026/neuron-basis-applications/) · [Channel](https://t.me/ai_machinelearning_big_data)
