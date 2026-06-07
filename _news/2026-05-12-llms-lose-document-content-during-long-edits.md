---
layout: post
title: "LLMs Lose ~25% of Document Content During Long Editing Sessions"
date: 2026-05-12
---

Microsoft Research has published a preprint showing that LLMs silently corrupt documents during extended editing sessions. Testing 19 models (GPT-5.4, Claude 4.6, Gemini 3.1 Pro, and others) on the new DELEGATE-52 benchmark revealed that after 20 sequential edits, top models lose ~25% of content on average, with the full sample averaging ~50% loss. The best performer, Gemini 3.1 Pro, was deemed ready for delegation (≥98% content retention) in only 11 of 52 domains — Python code was the sole area where most models (17/19) preserved content nearly flawlessly, while recipes, fiction, sheet music, and financial reports saw the worst degradation. The study found losses are driven by rare but catastrophic failures rather than gradual decay (accounting for ~80% of all damage), and that naive agent tooling (search, code execution, direct file editing) actually adds ~6% more losses on average.

[arXiv preprint](https://arxiv.org/pdf/2604.15597) · [DELEGATE-52 dataset](https://huggingface.co/datasets/microsoft/delegate52) · [GitHub](https://github.com/microsoft/DELEGATE52)
