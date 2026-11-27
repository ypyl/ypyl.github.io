---
layout: post
title: "A 76,000-Line Lean Proof of the Riemann Hypothesis"
date: 2026-11-27
categories: news
---

A repository published on 2026-11-25 carries a Lean 4 proof of the Riemann Hypothesis and of the Generalized Riemann Hypothesis for Dirichlet L-functions, in two editions: a readable one of about 300,000 lines and a compact one of about 76,000 lines of code, or 113,000 with comments. It builds on Mathlib, covers every modulus and every character, primitive or not, and its five end theorems depend only on `propext`, `Classical.choice` and `Quot.sound`.

The text is a rewriting of Levent Alpöge's Lean formalisation of his own proof, done with colleagues at Anthropic; the repository states that the Lean and the accompanying documents were written by Claude. Reproducing the result needs Lean v4.35.0 with Mathlib pinned to commit `56a00585`, one to two hours on 12 cores and 24 GB of memory, plus the Comparator tool run for a second kernel check.

What has not happened is stated as plainly as what has. The proof passed machine checks only, no mathematician has read it through, and no paper comes with it. The repository had 1,006 stars on 2026-11-27.

[grh-lean](https://github.com/lsernau/grh-lean) · [Alpöge's remark](https://alpo.ge/remark.pdf) · [Channel](https://t.me/ai_machinelearning_big_data)
