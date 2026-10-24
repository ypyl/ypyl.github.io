---
layout: post
title: "OpenAI Models Jump Sharply on COBOL, Fortran and Visual Basic Coding"
date: 2026-10-20
categories: news
---

OpenAI reports that its newest models write code in rare and outdated programming languages far better than earlier generations, on languages that still run banking systems, ATMs and critical infrastructure. On OpenAI's benchmark, **GPT-6 Astra** passes **87.4%** of tests against **26.8%** for GPT-5.6 Sol, with the gains concentrated in the least common languages:

- **COBOL**: 10.9% to 72.1%
- **Fortran**: 21.4% to 83.5%
- **Visual Basic**: 27.5% to 89.4%

**GPT-6.1 Sol** reaches **97.9%** accuracy when porting programs from COBOL to modern languages. To measure this, OpenAI built **MultiLangBench**, in which a model must implement a SQLite equivalent in one of **20 programming languages**; the benchmark is open-sourced.

The commercial case is that large volumes of financial and government code were written decades ago, maintenance is expensive, and the engineers who can read it are retiring. The models are approaching the point of not only understanding such code but moving it to Python, Java or Rust while preserving the original logic.

**Related:** [OpenAI Releases GPT-6 Sol and GPT-6 Luna](/news/2026/09/22/openai-releases-gpt-6-sol-and-luna/)

[Low-resource programming languages](https://openai.com/index/low-resource-programming-languages/)
