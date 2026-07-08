---
layout: post
title: "Google Releases Gemma 4 Technical Report"
date: 2026-07-08
tags: news
categories: news
---

Google has published the technical report for **Gemma 4**, its new open multimodal model family spanning 2.3B to 31B parameters with reasoning, vision, audio, and long-context capabilities. The smaller E2B and E4B variants (2.3B and 4B effective parameters) match or surpass Gemma 3 27B while using roughly 10× fewer parameters, and E4B scores 86.6 on the RULER 128k long-context benchmark versus Gemma 3 27B's 66.0. The 31B dense model tops the Arena Text leaderboard among open dense models, while its 26B MoE variant activates only 3.8B parameters and still achieves 1438 Elo. Notably, the 12B model skips separate vision and audio encoders entirely, feeding image patches and audio chunks directly into the LLM.

[Gemma 4 Technical Report](https://www.alphaxiv.org/abs/2607.02770)
