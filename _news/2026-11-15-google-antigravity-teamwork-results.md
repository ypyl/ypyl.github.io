---
layout: post
title: "Google's Antigravity Teamwork Agents Prove a 2013 Conjecture and Port a Video Decoder to Rust"
date: 2026-11-15
categories: news
---

Google Research published results from **Antigravity's /teamwork** on November 13: a harness that splits a task across parallel agents, hands each one only the files and tools it needs, and puts separate agents in the roles of falsifier, critic and auditor so results get checked from a clean context. Running with Gemini 4 Argon, Carbon and Hydrogen, it formalized a full proof of the 2013 **Courtade-Kumar conjecture** in information theory in about 50 million lines of Lean 4, with the analytical work split between the models and researchers from CUHK, USC and CMU. A Lean-specific variant also produced a 1.8 million line formalization of the **Weil conjectures** across 28,800 subagent invocations.

The engineering results carry the same pattern. Thirty specialized agents read nuclear magnetic resonance data and returned 95% top-1 and 100% top-3 exact molecular structures on complex benchmark molecules, nearly double the single-agent score. A rewrite pattern ported Google's AV1 decoder **libgav1** from C++ to about 170,000 lines of Rust with bit-exact parity on the conformance suite, then made it 20% to 30% faster than the original C++ and hand-written assembly on x86-64 and ARM once given profiling tools. A two-week campaign added about 800,000 lines of safe Rust to the Zircon microkernel, passing kernel unit tests and booting on arm64.

**Related:** [Google Releases Gemini 4 Hydrogen](/news/2026/11/14/google-releases-gemini-4-hydrogen/), [Google Releases Gemini 4 Argon](/news/2026/10/13/google-releases-gemini-4-argon/)

[Advancing science and engineering with Antigravity's /teamwork](https://research.google/blog/advancing-science-engineering-with-antigravitys-teamwork-feature/)
