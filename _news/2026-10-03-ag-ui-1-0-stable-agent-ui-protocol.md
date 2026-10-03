---
layout: post
title: "CopilotKit Ships AG-UI 1.0, a Stable Spec for Connecting Agents to Apps"
date: 2026-10-03
---

CopilotKit has released **AG-UI 1.0**, the first stable version of the Agent-User Interaction Protocol, an open standard that defines a shared event stream between agent backends and user-facing applications. The release pairs a full specification with a JSON Schema that fixes the exact fields every event carries, and the TypeScript, Python and .NET SDKs are now generated from that schema instead of being maintained by hand. AG-UI is adopted by Google, Microsoft, Amazon and Oracle and supported by frameworks including LangChain, Mastra and Anthropic's Claude Managed Agents. Version 1.0 adds subagent support, custom metadata, multimodal tool results, human-in-the-loop interrupts and token usage reporting, and it is backwards compatible, so 0.x agents work with 1.0 clients and the reverse.

**Related:** [OpenAI Launches WebMCP Challenge as Major Tech Companies Rally Behind Agent-Web Standard](/news/2026/08/30/openai-webmcp-challenge/), [Google Standardizes AI Agent Interaction with Websites via WebMCP](/news/2026/02/14/google-standardizes-ai-agent-web-interactions/)

[Introducing AG-UI 1.0](https://www.copilotkit.ai/blog/ag-ui-1.0)
