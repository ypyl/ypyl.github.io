---
layout: post
title: "Nvidia Releases Nemotron 3.5 Transcribe for 40 Languages"
date: 2026-10-29
---

Nvidia has released **Nemotron 3.5 Transcribe**, an open speech-to-text model covering 40 languages, translating 31 of them and separating speakers in 13. Russian gets the full task set it was trained on: transcription, translation, speaker diarization and code-switched audio where languages alternate within one recording. It accepts files up to an hour long, returns word-level timestamps and follows instructions in the prompt such as transcribing a recording and labelling each speaker; question answering about audio content works in English only, and live streaming is not supported. Under the hood sit a speech encoder and a hybrid language model of Mamba and transformer layers with mixture-of-experts, at 34B total parameters and about 4.3B active per token, published under the **OpenMDW-1.1** license.

Benchmarks put it first among open models on the Hugging Face transcription leaderboard, with **5.33% word error rate** on English, 4.62% on the multilingual split and 10.98% on long recordings. Artificial Analysis measures 2.8% error, matching a competitor that needs roughly six times more active parameters; on one RTX PRO 6000 the model processed 100 hours of audio in under three minutes.

Alongside the weights, Nvidia published **Granary v2**, a dataset of 463,000 hours of speech across 48 languages assembled from 16 open corpora. The dataset card and processing pipeline are open, but the audio must be downloaded separately under each source corpus's license.

**Related:** [Nvidia Announces Nemotron 3 Ultra](/news/2026/06/02/nvidia-nemotron-3-ultra/), [Microsoft Ships Its First Streaming Transcription Model](/news/2026/10/03/microsoft-mai-transcribe-2-streaming-mai-voice-2-1/), [OpenAI Releases GPT Transcribe and GPT Live Transcribe](/news/2026/07/30/openai-releases-gpt-transcribe-and-gpt-live-transcribe/)

[Model announcement](https://huggingface.co/blog/nvidia/nemotron-transcribe-granary) · [Weights](https://huggingface.co/nvidia/NVIDIA-Nemotron-3.5-Transcribe-30B-A4B) · [Technical report](https://research.nvidia.com/labs/nemotron/files/NVIDIA-Nemotron-3.5-Transcribe-Technical-Report.pdf) · [Granary v2](https://huggingface.co/datasets/nvidia/Granary-v2) · [Demo](https://huggingface.co/spaces/nvidia/nemotron-transcribe)
