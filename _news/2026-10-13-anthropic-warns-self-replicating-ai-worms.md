---
layout: post
title: "Anthropic Warns Self-Replicating AI Worms Are Close"
date: 2026-10-13
tags: news
categories: news
---

Anthropic's **Frontier Red Team** warns that autonomous AI worms able to spread across internet-of-things devices without any data center are becoming feasible. In one experiment the team ran distributed inference on **Gemma4-26B-A4B** across 15 budget Android TV boxes, a cluster that produced 0.39 tokens per second, and showed that open models can already pick packages and build execution environments on heterogeneous hardware without root access. In an isolated environment **GLM-5.2** completed a full replication cycle: scanning the network, compromising a target host, deploying a runtime, and launching a copy of itself. Anthropic estimates that distributed botnets built on models of up to 30 billion parameters will be technically possible by the end of this year.

**Related:** [Anthropic's Mythos Finds Mathematical Weaknesses in Cryptographic Algorithms](/news/2026/07/29/anthropic-mythos-cryptographic-weaknesses/), [Anthropic Finds Chinese GLM-5.3 Nears Closed Models on Cybersecurity Tasks](/news/2026/10/01/anthropic-tests-glm-5-3-cybersecurity-capabilities/), [Manifold Researcher Discovers Cryptomining Botnet on ClawHub](/news/2026/04/30/manifold-discovers-clawhub-cryptomining-botnet/)

[Self-replicating AI worms](https://www.anthropic.com/research/self-replicating-ai)
