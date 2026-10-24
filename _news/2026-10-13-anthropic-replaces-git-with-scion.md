---
layout: post
title: "Anthropic Replaces Git With Its Own Version Control System, Scion"
date: 2026-10-13
categories: news
---

Anthropic has moved its monorepo off Git onto **Scion**, a version control system it built in five months because Claude agents were writing more code than the existing tooling could handle. The company merged about **2,500 changes a day in May**, a number that passed **30,000 by September**; Scion now serves 25 million checkouts a day against close to 100 GB of main-branch history, using **200 times less CPU and memory** for version storage infrastructure. Checkouts went from minutes to seconds, and CI CPU load dropped 25%. A team of seven engineers built Scion and the code review system **Antique** with heavy use of Claude, and they say the model now writes better code than any of them, moving their work toward architecture, constraints, and decisions.

**Related:** [Entire Announces New Developer Platform Backed by $60M Seed Round](/news/2026/02/11/entire-announces-new-developer-platform/), [Anthropic Launches Claude Code Review](/news/2026/03/10/anthropic-launches-claude-code-review/)

[Redesigning source control at Anthropic](https://www.anthropic.com/engineering/redesigning-source-control)
