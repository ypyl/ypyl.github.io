---
layout: post
title: "GitHub's Rubicon Git Storage Handles 7,200 Pushes a Second for Agent Fleets"
date: 2026-11-10
categories: news
---

GitHub has published the design of **Rubicon**, the Git storage architecture it announced at GitHub Universe, after internal tests carried **5,000 concurrent agents** on one repository at **7,200 pushes per second** while serving reads with steady latency. The busiest repositories on GitHub today take at most **dozens of writes per second**, usually in short bursts.

The gain comes from separating stored code from branch pointers. A push uploads immutable packs to **Azure Blob Storage**, and a replicated log service called **Ref-CAS** holds the references and the pack manifest. Each reference update is a compare-and-swap that locks only the references it touches, so agents pushing to their own branches commit in parallel and only simultaneous updates to the same reference serialize. A replica that is already current answers a read with a single version comparison, and Ref-CAS transacts **tens of thousands of operations per second** because it stores pointers, never object data. Compaction and garbage collection run on separate nodes against durable storage instead of competing with the servers that serve traffic.

Rubicon is in a limited private preview, with a broader rollout planned for early next year.

**Related:** [GitHub Rewrites Copilot Runtime in Rust Using Copilot Itself](/news/2026/09/17/github-copilot-runtime-rewritten-in-rust/)

[GitHub Blog](https://github.blog/engineering/architecture-optimization/inside-githubs-new-git-storage-architecture/) · [GitHub on X](https://x.com/github/status/2118830274431873379)
