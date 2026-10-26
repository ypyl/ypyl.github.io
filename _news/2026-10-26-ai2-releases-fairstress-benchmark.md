---
layout: post
title: "FairStress Shows Language Models Over-Correct on Race and Gender Cues"
date: 2026-10-26
categories: news
---

The Allen Institute for AI has released **FairStress**, a benchmark that tests whether a model changes its decision about a person because of race, gender, religion, age, disability or orientation in hiring, lending, medicine and education. Each prompt adds adversarial prefixes that make an authoritative or socially loaded argument (leadership's opinion, past discrimination, informal recommendations), then measures how readily the model drops the objectively stronger candidate under that pressure.

The set holds **300 scenarios** across **29 domains** with fictional candidates, **33 demographic group pairs**, four levels of identity disclosure and up to **78 persuasion phrasings**, for more than **13 million variants**. Across **15 models** from the Olmo, Llama, Qwen, Gemma, Claude and GPT families it found **anti-stereotype hypercorrection**: with no identity data models were unbiased, but once the group was stated they abandoned the correct choice **9.3 percentage points** more often when that switch favoured a minority candidate. Where no candidate was a clear favourite, minorities were chosen **60 to 65%** of the time, and models justified one candidate with criteria and facts while crediting another with abstract resilience and diversity.

The audit concludes that current alignment training teaches models to block discriminatory requests without teaching them to weigh objective skill under pressure. Only a **counterfactual chain of thought**, which forces a re-run of the reasoning with demographic markers swapped, narrowed the asymmetry without hurting accuracy. The dataset is on [Hugging Face](https://huggingface.co/datasets/allenai/fairstress) under **CC-BY-4.0**.

**Related:** [Anthropic Research Reveals How Claude's Behavior Shifts Across Languages](/news/2026/07/13/anthropic-research-claude-behavior-across-languages/), [White House Finalizes AI Model Evaluation Framework Behind Closed Doors](/news/2026/08/04/white-house-finalizes-ai-model-evaluation-framework/)

[FairStress: testing whether AI keeps its judgment when identity enters the room](https://allenai.org/blog/fairstress)
