---
layout: post
title: "Anthropic Rewrote the Messages API in Rust With Its Own Agents"
date: 2026-11-26
categories: news
---

Anthropic has [published](https://www.anthropic.com/research/migrating-api-to-rust) how it moved the Messages API, the service behind Claude Code and Cowork, from Python to Rust. API traffic grew roughly **1,800 times** between early 2024 and fall 2026, and the Python service paid an object, memory and task-switching overhead on every request. Claude wrote a first version of the core request path in about a week.

The code was the easy half. Correctness took three loops: an inner loop where agents reproduced a failure and fixed it in a sandbox in under a second, an outer loop where engineers reviewed the fix and the build went back onto shadowed production traffic, and a memory loop that turned each correction into a rule, test or linter. The old Python tests mocked billing and rate limiting, so passing them never proved the new service behaved the same.

The shipped API uses about **70% less CPU per request** and cuts median time to first token by about **20%**, with the service never taken offline. By May 2026, Claude was writing more than 80% of the code Anthropic merged, which is why the team judged the migration worth attempting at all.

[Migrating the Messages API to Rust](https://www.anthropic.com/research/migrating-api-to-rust) · [Channel](https://t.me/machinelearning_interview)
