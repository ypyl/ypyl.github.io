---
layout: post
title: "Study Finds Claude Code Uses 4.7x More Overhead Tokens Than OpenCode for the Same Task"
date: 2026-07-21
tags: news
categories: news
---

A study by Systima comparing Claude Code 2.1.207 and OpenCode 1.17.18 on identical tasks through the same Claude Sonnet 4.5 model found that Anthropic's agent uses **4.7 times more overhead tokens** per request — with the bulk of the difference coming from tool descriptions (24K tokens vs 4.8K). Even a minimal "OK" command triggered 27.3K characters of system instructions, 100K characters of tool schemas, and 8K of additional context from Claude Code, versus 9.3K and 20.9K respectively for OpenCode. On a file-summarisation task, Claude Code consumed ~199K input tokens across 6 requests, while OpenCode used ~41K across 4 — both completing the task correctly. The gap narrows when tools are disabled but widens significantly with sub-agents: a task using 121K tokens jumped to 513K with two sub-agents.

[Source: Systima comparison study]
