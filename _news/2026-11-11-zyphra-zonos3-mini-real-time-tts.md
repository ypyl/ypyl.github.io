---
layout: post
title: "Zyphra Releases ZONOS3-Mini, a Voice-Cloning TTS Model That Runs in a Browser"
date: 2026-11-11
categories: news
---

**Zyphra** has released **ZONOS3-Mini**, a zero-shot voice-cloning text-to-speech model under **Apache 2.0** with 800 million total and roughly 100 million active parameters, a sparse mixture of experts running 64 experts with top-2 routing. It stays real-time on consumer hardware: under 570 ms to first audio in the browser on a Pixel 9, under 150 ms on an M3 MacBook Air, and a median 80 ms on one H100 serving eight concurrent requests, where peak throughput reaches 457 seconds of audio per second. Zyphra ranks it first among open models on its own **ZTTS2-Eval**, ahead of Fish S2 Pro and MOSS TTS v1.5, a comparison the company publishes rather than an independent one.

A voice is cloned from a reference clip of up to 60 seconds, or built from a single 192-dimension speaker embedding, with controls for accent, mood, speaking rate and delivery down to individual words. That control rests on a new **ZAC-1** codec at 15 Hz, which Zyphra says cuts word error rate 10x against DAC under the same data, and the model covers **43 languages**. Runtimes ship for browsers through WebGPU, for servers through SGLang and vLLM, and for anything else through ggml; training took 112 H100s and 2.5 days. Hosted inference on Zyphra Cloud costs **$10 per 1M UTF-8 bytes**, capped at one concurrent request and 100 generations a day.

**Related:** [Zyphra Releases ZAYA1-8B, a Compact MoE Model Trained on AMD](/news/2026/05/07/zyphra-releases-zaya1-8b-moe-model-trained-on-amd/), [MOSS-TTS-v1.5 Tops Hugging Face Trending for Text-to-Speech](/news/2026/06/03/moss-tts-v1-5-huggingface-trending/)

[ZONOS3-Mini release post](https://www.zyphra.com/our-work/zonos3-mini) · [Weights](https://huggingface.co/Zyphra/ZONOS3-Mini) · [GitHub](https://github.com/Zyphra/ZONOS3-Mini) · [Demo](https://cloud.zyphra.com/audio-playground?modelId=zyphra%2FZONOS3-Mini)
