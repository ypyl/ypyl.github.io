---
layout: post
title: "General Intuition, Kyutai, and Epic Games Release MIRA — a Generative Simulator for Rocket League"
date: 2026-07-08
---

**General Intuition**, in collaboration with **Kyutai** and **Epic Games**, has released **MIRA** — a generative simulator that runs Rocket League 2v2 matches in real time entirely in latent space, without any physics engine, renderer, or explicit 3D representation at inference. Powered by a 5-billion-parameter diffusion transformer and a 600-million-parameter video codec built on top of a frozen DINOv3-L, the model takes only frame history and key presses from all four players as input, achieving a gFID of 10.7 versus 81–105 for pixel-based approaches and an ARR of 0.91 versus 0.49–0.61. The latent-space design proved stable enough that no drift-countermeasures were needed — rollouts run for hours without collapse, and the ARR score matches human evaluations with a Pearson correlation of 0.84. The authors have open-sourced the training and inference code along with the Rocket Science dataset of 1,000 hours of 720p replays with action streams and physical states.

[MIRA paper](https://mira-wm.com/paper) · [Code on GitHub](https://github.com/mira-wm/mira) · [Rocket Science dataset](https://huggingface.co/datasets/kyutai/rocket-science) · [Playable demo](https://mira-wm.com/play/?via=game-00)
