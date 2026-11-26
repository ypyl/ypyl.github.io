---
layout: post
title: "One GPT-6.2 Agent Proves Optimal Solutions on Open Optimization Problems"
date: 2026-11-26
categories: news
---

Dmitry Rybin, writing on his [AutoKernel blog](https://rybindmitry.github.io/blogs/gpt-6-2-astra-is-superhuman-in-mathematical-optimization.html) on November 23, reports that a single GPT-6.2 Astra agent found and proved optimal solutions to open problems in four benchmark libraries. He picked 10 unsolved MILP problems from MIPLIB where **nine years of commercial solvers** had produced neither an optimal solution nor a proof, 10 open problems from QPLIB, open lower bounds in MINLPLIB, and 10 SDPLIB problems he says Astra fully solved. Average runtime was about **30 minutes per problem**, with most landing between 30 and 60.

The method matters as much as the score. The prompt asked the model to design custom specialised algorithms rather than call a solver, it was allowed to search the internet, and its answers mixed hand-built heuristics, tree search, proofs and open source LP solvers for subproblems. Rybin states two limits himself: in a small number of cases the optimal solution was already in the literature outside the benchmark sites, and when he framed a 4.6M parameter CNN as a bound optimisation problem the result generalised badly, **70% against an 85% baseline**.

The claim worth carrying forward is the comparison he draws at the end. Anthropic reported similar MIPLIB results two weeks earlier using **300 agents** of Fable 5.2, and Rybin got these from one agent at small time limits. This is one researcher's blog post, not a peer reviewed paper.

[GPT 6.2 Astra is superhuman in mathematical optimization](https://rybindmitry.github.io/blogs/gpt-6-2-astra-is-superhuman-in-mathematical-optimization.html) · [Channel](https://t.me/vistehno)
