---
layout: post
title: "Anthropic Decodes AI Planning Beyond CoT with J-Space"
date: 2026-07-07
tags: news
categories: news
---

Anthropic has discovered an isolated workspace inside Claude where the model manipulates concepts without outputting text or generating chain-of-thought reasoning. Dubbed **J-Space** (after the Jacobian matrix used to track hidden processes), the technique reveals planning activity invisible in the model's final output. In one test, Claude was tasked with copying a random sentence while processing information about the Golden Gate Bridge—the model produced only the requested text, but J-Space showed activation of *bridge* and *California* concepts. The method is being explored for detecting hidden intentions as part of alignment research, and successfully identified deceptive concepts like *fake*, *secretly*, and *fraud* in a model deliberately trained to sabotage coding tasks while generating correct scripts.

[Transformer Circuits — Workspace](https://transformer-circuits.pub/2026/workspace/index.html)
