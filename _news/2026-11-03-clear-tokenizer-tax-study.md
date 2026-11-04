---
layout: post
title: "CLEAR Finds Amharic Costs Six Times More Than English in GPT-5 Tokens"
date: 2026-11-03
categories: news
---

The **Coalition for Language Equity in AI Research (CLEAR)** has published the first study in its language equity series, measuring what it calls the **tokenizer tax**: providers bill per token regardless of language, but tokenizers are trained mostly on English and about a dozen high-resource languages, so text in most other languages fragments into smaller pieces and costs more for the same meaning. One phrase takes **13 tokens in English, 26 in Malayalam and 78 in Amharic** under the GPT-5 tokenizer, because each Ethiopic character splits into two one-byte tokens. Amharic input therefore runs about **six times** the price of the English version, at nearly the same character count.

The measurements used the **FLORES-200** phrase set across **203 language and script pairs**. The headline metric is the ratio between the most and least expensive language, with a **Gini coefficient** borrowed from economics alongside it, and CLEAR grades the results:

- **A** up to 3x, **B** up to 6x, **C** up to 9x, **D** up to 12x, **F** above that.
- Across the 20 languages best represented online, models are close together, spreading **1.5 to 3.1 times**.
- Over all 200 languages, **NLLB-200** was the only model graded **A**. **Gemini** and **Gemma** got **B**. The Chinese stack (**Qwen 3**, **DeepSeek V4.1 Flash**, **Kimi K3**) scored **D** with **Fable 5**, **Opus 4.8** and **Llama 4 Scout**. **GPT-5** took an **F**.

Script matters more than speaker count: Latin-script languages stay cheap even when rare, since Asturian never appears in Common Crawl yet benefits from Latin letters and Romance relatives, while Burmese, Tibetan, Tifinagh, Khmer, Lao and Ol Chiki fare worst. Large languages pay the tax too, including **Urdu** at 288 million speakers, **Bengali** at 266 million and **Punjabi** at 200 million. The authors add two consequences from the same extra tokens: **slower responses**, and **less usable context**, because a rare language fills a nominal 128,000-token window with only part of the text English would fit.

CLEAR asks vendors to publish both metrics on model cards, reserve vocabulary share for more languages in the next tokenizer generation and, since a tokenizer cannot change before a new base model, normalise pricing by characters in the meantime. The test code is [open source](https://github.com/clear-coalition-ai/token-cost-equity), and a follow-up on equity of answer quality is promised.

**Related:** [Anthropic Research Reveals How Claude's Behavior Shifts Across Languages](/news/2026/07/13/anthropic-research-claude-behavior-across-languages/)

[The Tokenizer Tax](https://clear-languages.org/blog/the-tokenizer-tax)
