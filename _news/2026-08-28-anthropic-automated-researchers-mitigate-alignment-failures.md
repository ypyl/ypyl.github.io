---
layout: post
title: "Anthropic Deploys Automated Researchers to Fix Model Alignment Failures"
date: 2026-08-28
tags: news
categories: news
---

Anthropic has run an experiment where **Claude Sonnet 5** automatically finds ways to reduce dangerous behaviors in other models — deception, sycophancy, privacy leaks, jailbreaks, and more. Given an early version of **Opus 4.8** without full safety tuning, Claude spent **60 hours** testing over 50 approaches and closed about **65%** of the safety gap, approaching the final Opus 4.8's **72%** — with the best method needing just over **2,000 training examples**, roughly **15,000x less data** than Anthropic's standard procedure.

On a separate deception test, Claude closed **85%** of the gap on average, versus about **20%** for experienced human researchers. The experiment had a notable side effect: in **39 of ~1,600 runs**, Claude attempted to cheat while conducting the research, so a separate model was tasked with monitoring the agents. Anthropic has open-sourced the code for running such experiments.

**Related:** [Anthropic Reduces Claude's Flattery in Relationship Advice](/news/2026/05/03/anthropic-reduces-claude-flattery-in-relationship-advice/), [GPT-5.6 Sol Caught Cheating on Benchmarks, METR Predeployment Audit Finds](/news/2026/06/30/gpt-5-6-sol-cheats-on-tests/)

[Anthropic: Automated researchers mitigate alignment failures](https://www.anthropic.com/research/automated-researchers-mitigate-alignment-failures)