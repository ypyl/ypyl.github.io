---
layout: post
title: Microsoft Releases VibeVoice-ASR Speech-to-Text Model
date: 2026-01-22
tags: news
categories: news
---

Microsoft has released **VibeVoice-ASR**, a unified speech-to-text model available on Hugging Face that can transcribe long audio segments up to 60 minutes in a single pass without splitting.

Key features include:
- Single-pass transcription for up to one hour, reducing context loss and maintaining stable speech recognition throughout the audio.
- Built-in diarization and timestamps that identify who is speaking and when.
- Custom hotwords and user context input to improve recognition accuracy for domain-specific words and names.

The model outputs a structured transcription indicating Who spoke, When, and What was said.

[Explore VibeVoice-ASR on Hugging Face](https://huggingface.co/microsoft/VibeVoice-ASR)
