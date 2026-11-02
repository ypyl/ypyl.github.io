---
layout: post
title: "GPT-6 Astra Edits 0.016% of a Small Model's Weights to Fix Its Maths"
date: 2026-11-02
categories: news
---

MIT CSAIL researchers gave **GPT-6 Astra** direct access to the weights of **Qwen3.5-0.8B** through Python and let it run up to 50 test passes, asking it to improve the model's maths without gradients, fine-tuning or backpropagation. Astra traced the small model's poor scores to a reasoning loop: it repeated the same fragments and spent its token budget without producing an answer. It localised the cause to the MLP blocks of layers 15 and 19, found a direction in weight space tied to the looping, and moved **131,072 of 800 million parameters**, about **0.016%** of the model.

The results on the unedited architecture:

- **MATH-500:** 7.9% to 48.1%
- **GSM8K:** 13.8% to 67.1%
- Responses that hit the token limit: 93% to 33.1%

The edit removed a mechanism blocking reasoning the model could already do, rather than teaching it new maths. On **Gemma 3 270M**, which did not show the same looping, the approach barely helped. The authors call the method **Weight-Space Programming** and present it as a case for mechanistic interpretability: a model locating a specific unwanted behaviour and correcting it in the weights themselves.

**Related:** [OpenAI Launches GPT-6 Astra with Strong Benchmark Results](/news/2026/09/03/openai-launches-gpt-6-astra/), [The Information Reveals Astra's Recurrent Depth Architecture](/news/2026/09/02/openai-astra-recurrent-depth-architecture-revealed/), [OpenAI's Astra Solves 10 Longstanding Math Problems](/news/2026/08/01/openai-astra-solves-10-longstanding-math-problems/)

[Weight-Space Programming](https://rikiyat.github.io/blog/weight-space-programming/)
