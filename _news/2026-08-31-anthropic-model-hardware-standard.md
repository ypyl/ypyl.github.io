---
layout: post
title: "Anthropic Unveils Model Hardware Standard for AI-Agent Control of Physical Devices"
date: 2026-08-31
---

Anthropic has opened a research preview of the **Model Hardware Standard (MHS)**—a shared specification that lets AI agents safely operate physical devices such as microscopes, liquid handlers, and robotic arms, cutting hardware integration time from weeks or months to hours or minutes. Developed with HHMI Janelia Research Campus, MHS uses a standardized driver that translates between a computer's OS and any device with a programmable interface via simple read/write primitives, while making devices discoverable in a common format.

With MHS, agents can run multiple instruments in parallel, chain commands, track outcomes, adjust experiment parameters in real time, and in some cases recover from hardware failures unaided. In tests, **Claude** recalibrated a laser by changing its position, observing results through a camera, and iterating—then distilled the discovered sequence into a reusable script. The standard is model-agnostic and works through the **Model Context Protocol (MCP)**; early access partners include AWS, Doosan Robotics, Universal Robots, QIAGEN, and Tecan, with Hugging Face planning LeRobot support and Raspberry Pi working on integration.

**Related:** [QuEra's Claude AI Agent Automatically Locks and Tunes Quantum Computer Lasers](/news/2026/08/30/quera-claude-ai-agent-quantum-lasers/)

[Anthropic: Previewing the Model Hardware Standard](https://anthropic.com/news/model-hardware-standard-research-preview)