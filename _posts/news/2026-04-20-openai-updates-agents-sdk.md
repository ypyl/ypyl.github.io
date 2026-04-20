---
layout: post
title: "OpenAI Updates Agents SDK with Enhanced Capabilities"
date: 2026-04-20
tags: news
categories: news
---

OpenAI has released a major update to its Agents SDK, enabling agents to read and write files, install dependencies, execute code, and interact with external tools beyond simple user dialogue. The new SDK includes customizable memory, sandbox-aware orchestration, and built-in file system tools, features previously seen in Codex.

The SDK supports seven sandbox providers out of the box and allows integration with custom infrastructure. A new Manifest abstraction standardizes agent workspace configuration, supporting local files and cloud storage solutions like AWS S3, Google Cloud Storage, Azure Blob Storage, and Cloudflare R2. 

Architecturally, the SDK separates agent logic from the execution environment, enhancing security by preventing credential exposure and enabling state snapshots for recovery. Currently, the SDK is available for Python, with a TypeScript version planned for the future.

[Read more on OpenAI's official announcement](https://openai.com/index/the-next-evolution-of-the-agents-sdk/).