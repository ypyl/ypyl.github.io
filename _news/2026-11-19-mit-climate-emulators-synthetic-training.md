---
layout: post
title: "MIT Trains Climate Emulators on Scenarios That Will Never Happen"
date: 2026-11-19
categories: news
---

MIT researchers published a training method in **PNAS** this week, announced on November 12, that cuts the data a climate emulator needs from six to eight full Earth system model runs down to one, and makes the emulator hold up on futures it never saw during training.

Standard practice trains an emulator on realistic emission trajectories, and each of those requires a full physics simulation that can take months on a supercomputer cluster. The MIT algorithm instead generates synthetic scenarios with sharp jumps in CO2 and methane, measures the emulator's error against the full model, and updates the training data until that error drops. The scenarios end up looking nothing like a plausible future, which the authors argue is the point: the training set only has to be informative.

An emulator trained this way reproduced a scenario in which regional policy cuts one pollutant while the others keep rising, a case the traditional emulators in the comparison failed. Emulators run in seconds against months for the full model, so the gain is more futures tested per run.

[MIT News](https://news.mit.edu/2026/new-research-could-lead-faster-trustworthy-ai-models-climate-prediction-1112) · [PNAS paper](https://www.pnas.org/doi/10.1073/pnas.2620731123) · [Source channel](https://t.me/machinelearning_interview)
