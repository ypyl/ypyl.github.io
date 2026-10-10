---
layout: post
title: "Microsoft Research: CABRA Shows Coding Agents Fail at Code Logic, Not Big Edits"
date: 2026-10-10
tags: news
categories: news
---

Microsoft Research has built **CABRA**, a task generator that raises one kind of complexity at a time, to isolate where coding agents actually break down. Across **6,840 tasks** tested with eight LLMs and six agents, models without tools degraded steadily as task size grew, while agents stayed nearly error-free, largely because work like finding the right function can be offloaded to grep.

The bottleneck appears when search is no longer enough: agents asked to extract the shared logic of two classes while preserving their behavioral differences lost accuracy as complexity rose, even though the size of the required diff stayed small. The researchers report that the agents confused logic and leaned too heavily on naming.

The team recommends that agent evaluations add tasks covering behavior comparison and preserving differences during refactoring, since **diff size alone is a poor signal of how much code had to be understood**.

**Related:** [LLMs Lose ~25% of Document Content During Long Editing Sessions](/news/2026/05/12/llms-lose-document-content-during-long-edits/), [Microsoft Open-Sources run-assert-eval for AI Agent Risk Discovery](/news/2026/09/25/microsoft-releases-run-assert-eval-agent-risk-discovery/), [Nous Research Launches Hermes Index for Agent Cost and Performance](/news/2026/10/08/nous-research-hermes-index-agent-benchmark/)

[CABRA: Benchmarking coding agents on code logic (arXiv)](https://arxiv.org/abs/2610.10610) · [microsoft/CABRA on GitHub](https://github.com/microsoft/CABRA)
