---
layout: post
title: "Code Mode: the Better Way to Use MCP"
date: 2025-09-26
tags: news
categories: news
---

Cloudflare has introduced **Code Mode**, a novel approach to using the Model Context Protocol (MCP) that significantly improves AI agent performance. Instead of having large language models (LLMs) call MCP tools directly, Code Mode converts these tools into a TypeScript API, enabling the LLMs to write and execute code that interacts with the API. This method enhances the handling of complex and multiple tools, reduces unnecessary token usage, and leverages LLMs' extensive training on real-world TypeScript code.

Code Mode runs the generated code securely in lightweight, fast V8 isolates on Cloudflare Workers, ensuring sandboxed execution without internet access except through authorized MCP APIs. This approach also improves security by hiding API keys and providing a uniform interface for AI agents to access external tools.

Cloudflare's Agents SDK supports Code Mode, fetching MCP server schemas and converting them into TypeScript definitions automatically. Developers can try Code Mode locally or sign up for the production beta to experience this efficient and secure way to empower AI agents.

[Read more on Cloudflare's blog](https://blog.cloudflare.com/code-mode/)
