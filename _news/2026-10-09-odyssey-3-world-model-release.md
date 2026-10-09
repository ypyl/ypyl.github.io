---
layout: post
title: "Odyssey Releases Odyssey-3, a Real-Time Foundation World Model"
date: 2026-10-09
tags: news
categories: news
---

The startup **Odyssey**, founded by engineers who spent a decade building driverless cars, has released **Odyssey-3**, an autoregressive diffusion transformer that generates an interactive environment from a text prompt and predicts in real time how objects in it move and interact. A research preview is live on the company's site, with two versions: **Odyssey-3** at 832x480 and **Odyssey-3 Pro** at 1280x720.

- **Interaction**: the model continues video frame by frame using previous observations plus new inputs, so you can move through the world in first or third person, control the camera separately, and inject events mid-generation. Odyssey says a distilled few-step variant makes that responsive in real time.
- **Benchmarks**: Odyssey-3 Pro scored **66.1** on Physics-IQ Verified's video-to-video benchmark, which tests how plausible a model's continuation of real physical experiments is, and **54.7** on image-to-video. On WorldMark, Odyssey's own evaluation puts the model first in three of four splits (first-person stylized, third-person real, third-person stylized) and third in first-person real.
- **Physical AI**: Odyssey positions it as a simulator for training robots and other systems rather than a game engine. Action decoders and control policies are trained on top of the frozen backbone. Examples include manipulation tasks learned from tens of hours of robot demonstrations, humanoid policies built by Flexion, and a driving policy for real roads in India trained on 20 hours of data.
- **Access**: developers can request API access through Odyssey's developer portal. Pricing has not been announced.

**Related:** [General Intuition, Kyutai, and Epic Games Release MIRA, a Generative Simulator for Rocket League](/news/2026/07/08/general-intuition-kyutai-epic-games-release-mira-generative-simulator/), [Overworld Releases Open-Source Real-Time Game World Generator](/news/2026/01/24/overworld-releases-opensource-realtime-game-world-generator/)

[Meet Odyssey-3](https://odyssey.systems/meet-odyssey-3) · [Try the research preview](https://experience.odyssey.systems/)
