---
layout: post
title: "Google Releases EmbeddingGemma 2 for On-Device Multimodal Search"
date: 2026-10-06
tags: [news]
categories: news
---

Google has released **EmbeddingGemma 2**, an embedding model that maps text, code, images, video, and audio into a single vector space, so a device can retrieve a photo or a video clip from a text description without contacting a server. The family totals **740M parameters**: 270M for text, 170M for images, and 300M for audio, and each module can be attached on its own. In an optimized run on a Pixel 11 Pro, the full model uses roughly **567 MB of active RAM**, while the text-only path stays near 191 MB. The weights ship under **Apache 2.0**, which permits commercial use, and the intended workload is local search over documents and media where data cannot leave the device.

[EmbeddingGemma](https://deepmind.google/models/gemma/embeddinggemma/)

**Related:** [Google Releases Gemma 4 12B](/news/2026/06/04/google-releases-gemma-4-12b/), [Google Unveils Coral Board: A RISC-V SBC for On-Device Gemma 3](/news/2026/05/29/google-coral-riscv-board-gemma/)
