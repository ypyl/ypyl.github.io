---
layout: post
title: "GPT-6.2 Astra Closes a 23-Year-Old Probability Conjecture"
date: 2026-11-27
categories: news
---

Maximilian Janisch [reported on X](https://x.com/MaximilianJani/status/2126033809342959752) on 2026-11-26 that GPT-6.2 Astra produced a complete proof of a conjecture he and Thomas Lehéricy published in 2024, a special case of which Martin Raič posed in 2003. The statement bounds how far the distribution of a sum can sit from a normal approximation when each variable depends on a few neighbours, a Berry-Esseen problem on dependency graphs.

Janisch had given the problem to every model he could reach since finishing his PhD. GPT-6 Astra and Claude Fable 5.2 produced partial progress, and Astra was the first with a complete proof. The manuscript ships with a Lean 4.24.0 formalisation whose kernel replay checked 39,553 declarations above `Init` and found only the three standard axioms.

The bound is explicit, `min(1, 2,000,000 * q^(δ-1) * Σ E|Y_i/σ|^δ)`, so the result settles the conjecture without making the constant useful for small samples. The paper is submitted to arXiv and not yet public; Janisch expects the link early next week.

**Related:** [One GPT-6.2 Agent Proves Optimal Solutions on Open Optimization Problems](/news/2026/11/26/gpt-6-2-astra-open-optimization-problems/)

[Announcement](https://x.com/MaximilianJani/status/2126033809342959752) · [Proof and Lean formalisation](https://github.com/maximilian-janisch/berry-esseen-dependency-graphs) · [Channel](https://t.me/machinelearning_interview)
