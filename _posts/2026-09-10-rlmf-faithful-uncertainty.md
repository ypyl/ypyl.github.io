---
layout: post
title: "RLMF: Reward the Model for Knowing What It Doesn't Know"
date: 2026-09-10
categories: ai
tags: [ai, llm, calibration, metacognition, reinforcement-learning]
---

An LLM stating a fabricated fact at **confidence 0.9** is not lying in the usual sense. Its expressed confidence and its internal state are two different things, and no prompt closes that gap.

**RLMF** (Liu, Caciularu, Yona, Szpektor, Cohan, 2026) changes the training signal instead. During RL training, the model generates several candidate answers per query and scores them as usual for correctness and format. Then it is asked to **grade its own performance**. Answers where that self-assessment matched reality get a ranking boost; answers where the model was clueless about itself get pushed down. That is the whole trick, and it doubles as a way to pick which training examples are worth learning from.

The catch: "actual" performance is not ground truth. It is estimated by sampling the same question several times and checking whether the answers agree, and the metric used to rank results was proposed by the authors themselves. Both steps also add inference cost, and the payoff is honest hedging, not better answers.

Small open models trained this way reportedly reached a faithfulness score around 0.84, above GPT-5 and the Gemini 3 models given the same metacognitive prompt, with no loss on the tasks. This is a preprint, so treat the numbers as the authors' own.

The short version: calibration is a training problem, and the signal that moves it is the model's accuracy about its own performance.

[arXiv:2606.32032](https://arxiv.org/abs/2606.32032) · [Code (GitHub)](https://github.com/yale-nlp/RLMF)
