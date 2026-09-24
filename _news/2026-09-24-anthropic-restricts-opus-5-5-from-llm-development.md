---
layout: post
title: "Anthropic Restricts Opus 5.5 from Assisting with LLM Development"
date: 2026-09-24
tags: news
categories: news
---

Anthropic has added classifiers to **Opus 5.5** — similar to those used in its Fable models — that detect requests related to low-level LLM development (e.g., code for AI accelerators) and automatically redirect them to a less capable model. A notification informs the user of the switch, which persists for the rest of the conversation unless manually reverted. The filter targets a narrow set of tasks; Anthropic says ordinary ML work and programming are unaffected. In the API, there is no automatic redirect — the request simply stops until the developer configures a fallback model. Anthropic frames the measure as safety that must scale alongside model capabilities.

[Why Claude switched models in your conversation](https://support.claude.com/en/articles/16049681-why-claude-switched-models-in-your-conversation-with-opus-5-or-opus-5-5)

**Related:** [Anthropic Releases Claude Opus 5.5](/news/2026/09/22/anthropic-releases-claude-opus-5-5/), [Anthropic Suspends Fable 5 and Mythos 5 Access](/news/2026/06/12/anthropic-suspends-fable-mythos-access/)
