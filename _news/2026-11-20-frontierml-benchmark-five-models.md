---
layout: post
title: "FrontierML Gives Five Frontier Models 12 Hours of Real ML Research and None Reaches Its Reference"
date: 2026-11-20
categories: news
---

Synthetic Sciences published **FrontierML** on November 18: **25 open-ended ML research problems**, each worked for **12 hours** inside a virtual lab called Lab0. There the agent rents up to four H100s at **$3.78 per GPU-hour** from a shared cluster, pays for them from a wallet of $50 to $125, reads a frozen arXiv snapshot of about 2.9 million papers and gets **no score to optimize** along the way. Ten of the 25 tasks stay private so nothing can train toward them.

Five frontier models ran every task. **Claude Fable 5.2** leads with a mean normalized score of **0.523** and the best run on **13 of the 25** tasks, ahead of Claude Opus 5.5 at **0.399**, GPT-6 Astra at **0.371**, Gemini 4 Carbon at **0.353** and Grok 4.7 at **0.115** on the public split only. No run reaches its task's strong-result reference, and on the median task the best of five runs closes **62%** of the gap.

The report puts the failure in judgment, not engineering. The agents could usually build what they intended, but only **six runs** replicated a measurement before making a decision that rested on a comparison, and most handed in a version they had never measured. From Lab0's logs the authors built four composite measures of research taste, metacognition, novelty, efficiency and control, and **no model reaches two thirds of the target on any of them**. Opus 5.5 leads on judgment and Fable 5.2 on invention.

The benchmark is stricter than earlier ones in one specific way: the final grade is out of reach during the run, so an agent that wants to know whether it improved has to build its own proxy and decide how far to trust it.

**Related:** [ApprenticeBench Tests AI Ability to Learn Like a New Employee](/news/2026/09/13/apprenticebench-tests-ai-ability-to-learn-like-new-employee/)

[FrontierML: Evaluating AI Agents at Autonomous Empirical ML R&D](https://syntheticsciences.ai/research/frontierml) · [The 15 public tasks](https://github.com/synthetic-sciences/FrontierML) · [Source channel](https://t.me/vistehno)
