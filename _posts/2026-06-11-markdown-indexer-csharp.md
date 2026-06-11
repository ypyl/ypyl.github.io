---
layout: post
title: "MarkdownIndexer: vendorable C# markdown-to-tree parser inspired by PageIndex"
date: 2026-06-11
tags: [csharp, dotnet, markdown, pageindex, rag, llm, indexing, tree-search]
categories: programming
---

A quick note about a tool I built this week: **[MarkdownIndexer](https://github.com/ypyl/MarkdownIndexer)** — a C# markdown indexing library that you vendor by copying two `.cs` files into your project. Zero NuGet dependencies (in the vendored files), pure BCL.

## The idea

[PageIndex](https://github.com/VectifyAI/PageIndex) popularized **vectorless RAG** — instead of chunking documents and searching via embeddings, you build a hierarchical tree (like a table of contents with summaries) and let an LLM *reason* its way to the right section. It achieved 98.7% on FinanceBench vs ~30-50% for vector RAG on complex documents.

PageIndex is Python. I ported the markdown path to C#.

## How it works

```
.md file → extract headers → slice text per section → build tree → assign IDs → JSON
                                       │
                                  [optional: thin small subtrees]
                                  [optional: LLM summarizes each node]
```

Output is a structured tree where every node has a title, `node_id`, and either `summary` (leaf) or `prefix_summary` (branch signpost). The downstream LLM reads the compact tree (no text — just summaries), selects relevant `node_id`s, fetches only those sections, and generates the answer.

## What's interesting

The two vendored files accept all external concerns as `Func<>` delegates — token counting, LLM calls — so the caller uses whatever libraries they already have. The files themselves are pure algorithm.

If you work with .NET and deal with long structured documents (docs, wikis, specs), take a look: [github.com/ypyl/MarkdownIndexer](https://github.com/ypyl/MarkdownIndexer).
