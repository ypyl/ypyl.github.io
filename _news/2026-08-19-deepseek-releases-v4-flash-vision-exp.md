---
layout: post
title: "DeepSeek Releases Experimental V4-Flash-Vision Model"
date: 2026-08-19
tags: news
categories: news
---

DeepSeek has released **V4-Flash-Vision-Exp**, an experimental vision model built on V4-Flash that can parse diagrams, recognize text, and call tools. By DeepSeek's own measurements it performs at the level of **Claude Opus 4.8**.

Images can be passed as Base64, via public URL, or through the new **Files API** (upload once, reference by ID). Formats are detected by actual file content rather than extension, one image costs at most **384 tokens**, and prompts accept up to **600 files**. Images are normalized to 800x800 pixels, with a `detail` parameter to force 512x512; the max side length is 8192 pixels (dropping to 4096 with 15+ images).

**Related:** [DeepSeek Launches V4-Flash API with Agentic Upgrades](/news/2026/07/30/deepseek-launches-v4-flash-api-with-agentic-upgrades/), [DeepSeek Upgrades V4-Flash (0731)](/news/2026/08/03/deepseek-upgrades-v4-flash-0731/)

[DeepSeek vision guide](https://api-docs.deepseek.com/guides/vision/) · [Files API](https://api-docs.deepseek.com/guides/files_api/)