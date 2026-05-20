---
layout: post
title: "Prime Intellect Open-Sources General-Agent for Synthetic AI Training Data"
date: 2026-05-20
tags: news
categories: news
---

Prime Intellect has open-sourced **General-Agent** — a synthetic environment that generates training data for AI agents without human annotators. Instead of static datasets, it dynamically creates tasks with automatic semantic validation.

The system uses a competitive setup between two models: a **Synthesizer** constructs tasks with databases and verification functions, while a **Solver** attempts to complete them. Tasks evolve across five difficulty levels — simple scenarios gradually accumulate additional constraints, cross-references, and complex instructions. The platform retains tasks solved within a target probability threshold, and the hardest cases are used to seed the next generation round. Fine-tuning a 30-billion-parameter model on trajectories collected in General-Agent improved tool-calling accuracy on the BFCL benchmark from 18.9% to 52.3%.

[General-Agent: Self-Supervised Data Generation for Agentic Training](https://www.primeintellect.ai/blog/general-agent)
