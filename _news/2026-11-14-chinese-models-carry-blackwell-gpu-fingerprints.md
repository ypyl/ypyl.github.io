---
layout: post
title: "Six Chinese Frontier Models Carry the Fingerprint of Export-Controlled Blackwell GPUs"
date: 2026-11-14
categories: news
---

Will DePue of Unemployment Labs published a forensic report on November 12 that reads hardware provenance out of published model weights. Recovering the initial values PyTorch laid down at the start of training and then searching for the GPU configuration that reproduces them bit for bit, he found Blackwell-class traces in **six models from four Chinese labs**, and Hopper traces in their earlier releases.

The identifications split by chip core count:

- **DeepSeek-V4-Flash** and **MiniMax-M3** match B200 or B300, which have 148 streaming multiprocessors.
- **DeepSeek-V4-Pro**, **Kimi K3**, **Kimi K3.1-Pro** and **Ant Group's Ling-3.1-flash** match GB200 or GB300, which have 152.
- Earlier models from the same labs, up to DeepSeek-V3, Kimi K2.5, MiniMax-M2.7 and Ling-3.0-flash, read as Hopper, consistent with the H800s the labs disclosed.

The check is reproducible: on a real NVIDIA B300, 445 of 469 selected rows from DeepSeek-V4-Flash came back identical bit for bit, and the same rows had no exact match on an H100. Blackwell hardware requires a US export license for China and no such license has been reported.

The report is explicit about its limits. The fingerprint identifies how the weights were initialized, which supports but does not prove that the same hardware ran the training. It says nothing about where the chips sat, how access was obtained, or whether export controls were broken. Specially configured AMD hardware is the only alternative the author could not rule out in general, and he excludes it for DeepSeek-V4-Flash.

**Related:** [DeepSeek Exposes Its Whole Chat App Through the API](/news/2026/11/13/deepseek-exposes-app-features-through-api/), [Nvidia to Acquire Hugging Face](/news/2026/08/28/nvidia-to-acquire-hugging-face/)

[Checkpoint forensics: Blackwell fingerprints in Chinese models](https://unemploymentlabs.com/checkpoint-forensics) · [Source channel](https://t.me/ai_machinelearning_big_data)
