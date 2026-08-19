---
layout: post
title: "Xiaomi Open-Sources MiDashengLM-Gen, a Text-to-Audio Scene Generator"
date: 2026-08-19
tags: news
categories: news
---

Xiaomi's **MiLM Plus** team has open-sourced **MiDashengLM-Gen**, a text-to-audio generator that produces complete audio scenes — speech, music, sound effects, and ambient acoustics — from a single description. It combines a custom **MiDashengLM-0.6B** audio tokenizer (based on the DashengTokenizer architecture), a fine-tuned **Qwen3-1.7B** language model, and DiT-based flow matching.

The prompt format separates control factors into explicit tags (`<|caption|>`, `<|asr|>`, `<|speech|>`, `<|sfx|>`, `<|music|>`, `<|env|>`), so voice style doesn't bleed into background noise. Trained on the private 77K-hour **ACAVCaps** superset plus TTS corpora (Emilia, LibriTTS, LJSpeech, AISHELL-3, WenetSpeech4TTS), the model knows English (54%) and Chinese (24.5%) best; Russian speech quality lags (13.19% errors vs 2.42% English). Output is 16 kHz WAV, up to 20 seconds.

Results: speech intelligibility on English improved dramatically — Seed-TTS error dropped from **12.15% to 2.79%** — with strong emotion accuracy on CV3-Eval (joy 0.98, sadness 0.96, anger 0.92). On single-sound AudioCaps generation it still trails TangoFlux (FAD 5.01 vs 2.26). Released under **Apache 2.0**; voice cloning, low-resource languages, and time control are planned next.

**Related:** [Qwen Releases Open-Source Qwen3-TTS with Voice Design and Cloning](/news/2026/01/22/qwen-releases-open-source-qwen3-tts/), [Xiaomi Releases Open-Source AI Agent MiMo Code](/news/2026/06/13/xiaomi-open-source-ai-agent-mimo-code/)

[MiDashengLM-Gen demo](https://xingws.github.io/midashenglm-gen-demo/) · [Paper](https://arxiv.org/pdf/2608.11804) · [Weights](https://huggingface.co/mispeech/midashenglm-gen)
