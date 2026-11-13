---
layout: post
title: "DeepSeek Exposes Its Whole Chat App Through the API"
date: 2026-11-13
categories: news
---

DeepSeek has moved the features that until now ran only inside its own web and mobile app into the API: web search, deep thinking search, file input, URL reading, content safety review, conversation titles, speech to text, text to speech and OCR. A single request now carries the retrieval stack, the document parsing and the voice layer instead of a bare chat completion.

The work happens on DeepSeek's servers. With the `deepseek_web_search` tool enabled, the model decides when to search, runs the query itself and answers with citations, and the client only renders the returned blocks and passes them back unchanged on the next turn. `deepseek_read_url` reads links found in the user message the same way. Turning a feature on is a request field rather than an integration: `safety_check` with a level, `title` with a boolean, thinking off for plain search and on for multi-step deep search.

The tools are reachable over the three protocols DeepSeek already serves, so an OpenAI or Anthropic SDK client gets them without a new SDK:

- Messages API at `https://api.deepseek.com/anthropic/v1/messages`
- Responses API at `https://api.deepseek.com/responses`
- Chat Completions at `https://api.deepseek.com/chat/completions`

All of them use the `deepseek-flash` model. OCR is deliberately separate from the chat model: it returns text page by page from a PDF, Word or PowerPoint file or from an image, and the file is neither stored in the Files API nor passed to a chat model.

DeepSeek also published the reference client it built with these calls, a complete chat interface that runs at `cdn.deepseek.com/chat-demo` with source on GitHub. It demonstrates search, document and image upload, safety review, speech output, voice input, Markdown and Mermaid rendering, and it imports a chat history exported from DeepSeek Chat. The API key, conversations and uploaded images stay in the browser. For anyone shipping this to the public, the docs attach a compliance condition: enable the safety check and meet the registration and AI labeling obligations.

**Related:** [DeepSeek Releases V4.1 Flash](/news/2026/09/10/deepseek-releases-v4-1-flash/), [DeepSeek Launches V4-Flash API with Agentic Upgrades](/news/2026/07/30/deepseek-launches-v4-flash-api-with-agentic-upgrades/), [DeepSeek Releases DeepSeek-OCR 2](/news/2026/01/27/deepseek-releases-deepseek-ocr-2/)

[Build a Chatbot guide](https://api-docs.deepseek.com/guides/build_a_chatbot) · [OCR guide](https://api-docs.deepseek.com/guides/ocr) · [Demo client source](https://github.com/deepseek-ai/chat-demo) · [Source report](https://t.me/ai_machinelearning_big_data)
