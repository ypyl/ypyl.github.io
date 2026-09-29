---
layout: post
title: "Synthetic Sciences Releases OpenScience with Autonomous Experiment Mode"
date: 2026-09-29
tags: news
categories: news
---

Startup **Synthetic Sciences**, from Y Combinator's Winter 2026 batch, has launched **OpenScience** on Product Hunt: an open desktop workspace where an AI agent runs research end to end, from literature review and hypothesis to code, experiment, analysis, and paper text. The headline addition is **Autoresearch**, a mode where the agent runs a series of experiments without human input and improves a target metric given in plain words, such as reducing validation loss, stopping after 20 runs or 8 hours, or halting when the metric plateaus. After four runs with no progress it switches to a different class of ideas, and every six runs it revisits its approach; the whole session is logged in files for the task, idea queue, results table, and conclusions.

The agent ships with **295 built-in skills** (model training, molecular biology, cheminformatics, LaTeX typesetting) and roughly **40 scientific databases**, including UniProt, PDB, ChEMBL, PubChem, arXiv, and Semantic Scholar. Experiments run locally, over SSH, in a Slurm or PBS cluster, or on Modal, with API keys from Anthropic, OpenAI, Google, or other providers, ChatGPT or Codex subscription login, local models, or the company's Ace service. The project is licensed under **Apache 2.0**.

**Benchmarks:**
- Terminal-Bench Science: **53 of 70 tasks (75.7%)**, above Snorkel's 68.1% for Codex with GPT-6 Astra and 63.3% for Claude Code with Opus 5.5.
- Terminal-Bench 4.0 science section: **10 of 14 tasks**, against 60% for Claude Code with Fable 5.1.
- BiomniBench-DA: **82.2 points**, against 81.04 for aipoch.

**Related:** [SakanaAI Launches Marlin AI Analyst Agent](/news/2026/06/16/sakanaai-marlin-analyst/), [Hugging Face Launches ML Intern Agent for Automated ML Experiments](/news/2026/09/10/hugging-face-launches-ml-intern-agent/)

[OpenScience on Product Hunt](https://www.producthunt.com/products/openscience) · [GitHub](https://github.com/synthetic-sciences/OpenScience) · [Documentation](https://www.openscience.sh/docs#/openscience/index) · [Benchmarks](https://www.openscience.sh/benchmark) · [Traces](https://github.com/synthetic-sciences/benchmarks-openscience)
