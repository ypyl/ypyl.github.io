---
layout: post
title: "Gemini 4 Agent Teams Cut Quantum Circuit Costs 290x and Prove Each Result in Lean"
date: 2026-11-24
categories: news
---

Google DeepMind set independent teams of **Gemini 4** agents on well-known quantum algorithms. Each proposed circuit was verified in Lean before entering a shared notebook, and every four hours the agents met at a virtual conference to trade and compare ideas. The results are cost reductions on problems that cannot be benchmarked on today's hardware, so the proof is the measurement: every optimisation carries a formal correctness proof.

The largest cuts: **11.8x** cheaper simulation of an inertial confinement fusion circuit, with memory down from 5,650 to 4,338 error-corrected qubits; **8x** for a light-sensitive molecule; **2.5x** for the drug-metabolising enzyme Cytochrome P450, with memory down from 481 to 333 qubits; **2.57x** for one implementation of Grover's search; and **290x** for arcsin, a routine many quantum algorithms use to load numbers into quantum states.

DeepMind open-sourced the Lean library, the problem specifications and the verified solutions. The circuits still need larger, more reliable quantum computers before they run outside a proof assistant.

**Related:** [DeepMind's AlphaProof Nexus Publishes Formally Verified Proofs](/news/2026/05/25/deepmind-alphaproof-nexus-formal-proofs/), [Google Reports Antigravity Teamwork Agent Results](/news/2026/11/15/google-antigravity-teamwork-results/)

[Finding faster quantum algorithms with agents and formal verification](https://deepmind.google/blog/finding-faster-quantum-algorithms-with-agents-and-formal-verification/) · [Paper](https://arxiv.org/abs/2611.09822) · [Lean library](https://github.com/google-deepmind/formal-quantum-computation) · [Source channel](https://t.me/vistehno)
