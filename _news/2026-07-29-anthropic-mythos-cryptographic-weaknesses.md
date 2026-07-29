---
layout: post
title: "Anthropic's Mythos Finds Mathematical Weaknesses in Cryptographic Algorithms"
date: 2026-07-29
tags: news
categories: news
---

Anthropic's Frontier Red Team published a report showing that its AI model **Mythos** discovered a previously unknown symmetry in the mathematical lattice underpinning **HAWK**, a post-quantum digital signature candidate in NIST's standardization process—reducing the effective key length by half and dropping the estimated attack cost from 2⁶⁴ to 2³⁸ operations in 60 hours. Mythos also found a novel **Möbius bridge** attack against a reduced-round variant of AES-128, accelerating the best known attack by up to 800x, and produced improvements against simplified versions of LEA, Serpent-128, Salsa20, Poseidon, and SHA-1. Each attack cost roughly $100,000 in API compute, and Anthropic has released demonstration code, the AES reasoning chain, and a joint **CryptanalysisBench** benchmark with ETH Zurich and other universities.

[Anthropic report](https://www.anthropic.com/research/discovering-cryptographic-weaknesses) · [Demo code](http://github.com/anthropics/cryptography-research-demo) · [CryptanalysisBench](https://github.com/ethz-spylab/cryptanalysis-benchmark)
