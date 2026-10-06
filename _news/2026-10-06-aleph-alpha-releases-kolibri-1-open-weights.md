---
layout: post
title: "Aleph Alpha Open-Sources Kolibri 1, a Bilingual German-English MoE Model"
date: 2026-10-06
---

Aleph Alpha has released **Kolibri 1**, a bilingual German-English model published under the Apache 2.0 license with full weights on [Hugging Face](https://huggingface.co/Aleph-Alpha/Kolibri-1). It is a mixture-of-experts transformer with 78B total and 3.46B active parameters, a native context window of 262,144 tokens that can be extended to 1,048,576, and a custom **UniBPE** tokenizer tuned to German word structure and compound nouns; German accounted for 21.3% of the 20T pre-training tokens. Aleph Alpha says the model was built with the EU AI Act, the General-Purpose AI Code of Practice and the GDPR in mind, and trained with its in-house **Merlin-Arthur** protocol so it abstains when the supplied context does not support an answer. On the company's own benchmark table, Kolibri leads smaller open-weight peers on math and long context but trails the much smaller Qwen3.6-35B-A3B on closed-book knowledge (AA-Omniscience Index of -32.8 against -15.3) and on function calling (BFCL v4 overall 61.4 against 67.2).

**Related:** [Mistral Releases Medium 3.5 and Remote Agents in Vibe Environment](/news/2026/04/30/mistral-releases-medium-35-and-remote-agents/), [Thinking Machines Lab Releases Inkling, Their First Open-Weights Multimodal Model](/news/2026/07/15/thinking-machines-inkling-open-weights-model/)

[Kolibri has landed: a sovereign open-weight model](https://aleph-alpha.com/en/blog/kolibri-has-landed-a-sovereign-open-weight-model/)
