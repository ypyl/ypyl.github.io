---
layout: post
title: "DeepMind's AlphaProof Nexus Solves 9 Open Erdős Problems Using Formal Verification"
date: 2026-05-25
tags: [news]
categories: news
---

Google DeepMind has unveiled **AlphaProof Nexus**, a system that autonomously solved **9 open Erdős problems** — some outstanding for decades — at a cost of just a few hundred dollars per solution. The system also proved **44 open OEIS conjectures**, resolved a 15-year-old question in algebraic geometry, and discovered a previously undescribed algorithmic parameter in optimization theory.

The architecture pairs an LLM that generates proof ideas and fragments with the **Lean proof assistant**, which mechanically verifies every logical step — incorrect proofs simply fail to compile, eliminating the need for human peer review. A basic agent alternating LLM generation with compiler feedback replicated all 9 Erdős solutions; more sophisticated variants with evolutionary search and reinforcement learning only outperformed on the hardest cases. An unexpected side effect: the agent also identified **inaccuracies in existing formalizations** of known mathematical results, acting as a diagnostic tool for problem statements themselves. The same approach applies to code verification, protocol analysis, compiler correctness, and cryptography — wherever formal verification can filter out hallucinated reasoning.

[AlphaProof Nexus: Scaling Formal Theorem Proving](https://arxiv.org/html/2605.22763v1)
