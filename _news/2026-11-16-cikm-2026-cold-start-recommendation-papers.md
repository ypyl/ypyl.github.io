---
layout: post
title: "Three CIKM Papers Attack Cold-Start Ranking, and One Questions How the Field Picks Winners"
date: 2026-11-16
categories: news
---

Three papers published at **ACM CIKM 2026** on 6 November share a problem: recommending items that have no interaction history. Each takes a different route, and the third one is about the benchmark rather than the model.

**[Give Me MoE: Dynamic Expert Routing for Inductive Graph-based Recommendation](https://dl.acm.org/doi/10.1145/3799682.3839967)** (Mardanova, Krasilnikov, Lashinin, Ananyeva, Makarov) replaces a single graph model with a mixture of graph experts, routing each item by its own features so a new item is scored by whichever experts fit it. Reported gain: 34.5% better ranking than PinSAGE on MovieLens new items.

**[E²L³AE: End-to-End Text Encoder Tuning for LLM-Enhanced Linear Autoencoders](https://dl.acm.org/doi/10.1145/3799682.3839945)** (Kurdun, Liakhnovich, Pechatov, Lashinin) stops training the text encoder and the recommender separately. Because the text encoder sees which items a user actually interacted with, it learns which parts of a product description matter. On office supplies it reports 20.3% higher recall and 17.2% better ranking than L3AE.

**[Split Decisions: How Evaluation Protocols Change Recommender Model Selection](https://dl.acm.org/doi/10.1145/3799682.3839942)** (Pechatov) is the one worth copying into your own pipeline. Random train/test splits let a model learn from events that happened after the test window, which no production system can do. Across six datasets and 18 comparisons, models selected without regard to chronology did worse on future interactions in 17 of them: ranking fell 49% on film data and 81% on short video. The model you pick depends on how you split, not only on the model.

Caveat on the numbers: ACM's pages block automated reads, so the figures above come from the authors' conference summary as the channels relayed it, not from the PDFs. Crossref confirms all three papers, their authors and the 6 November publication date.

[Give Me MoE](https://dl.acm.org/doi/10.1145/3799682.3839967) · [E²L³AE](https://dl.acm.org/doi/10.1145/3799682.3839945) · [Split Decisions](https://dl.acm.org/doi/10.1145/3799682.3839942) · [Source channel](https://t.me/ai_machinelearning_big_data)
