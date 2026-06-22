---
layout: post
title: "Approximate Span Matching in .NET — OCR Passage Retrieval with Smith-Waterman Alignment"
date: 2026-06-22
categories: programming
tags: [dotnet, ocr, text-search, approximate-matching]
---

**ApproximateSpanMatching** is a .NET 10 library for locating the most similar contiguous passage in an OCR-extracted document given a query string. It tokenizes markdown at the word level, builds an inverted index, then runs a seed-and-cluster pipeline with **Smith-Waterman local alignment** and affine gap penalties to score candidate spans. The document is indexed once and reused across many queries — zero external dependencies.

```csharp
using ApproximateSpanMatching;
using ApproximateSpanMatching.Matching;
using ApproximateSpanMatching.Models;

// Index once
var doc = IndexedDocument.FromMarkdown(
    "The **quick** brown fox jumps over the lazy dog.");

// Query many times
var matcher = new SpanMatcher();
var results = matcher.Search(doc, "quick brown fox", topN: 3);

foreach (var match in results)
{
    Console.WriteLine($"Score: {match.NormalizedScore:F2}, " +
                      $"Coverage: {match.Coverage:F2}");
    Console.WriteLine($"  Text: {match.OriginalText}");
}
```

The catch: **default gap penalties are aggressive**. Smith-Waterman with `gapOpenPenalty=-2.0` and `gapExtendPenalty=-1.0` means a single missing word costs −3 — more than a match's +1 reward. The algorithm therefore often prefers a shorter gap-free sub-alignment over covering the full query. Searching for `"quick brown fox jumps over lazy dog"` against text containing `"the"` between `"over"` and `"lazy"` yields a `Coverage` of 5/7 rather than 7/7, because spanning the single-word gap is penalized more than it's rewarded. Fix this by tuning penalties to match your data:

```csharp
// Milder penalties: gaps are tolerable, full-query alignment wins
var matcher = new SpanMatcher(
    new SmithWatermanAlignment(
        gapOpenPenalty: -0.5,
        gapExtendPenalty: -0.1));
```

Another gotcha: **case sensitivity must agree between document and query**. `IndexedDocument.FromMarkdown` defaults to case-insensitive (lowercasing all tokens). If you build a document with `caseSensitive: true`, then every query against it is also tokenized case-sensitively — but a case-insensitive document lowercases the query tokens. Mixing modes silently produces no matches:

```csharp
// Document is case-insensitive (default)
var doc = IndexedDocument.FromMarkdown("The Quick Brown Fox");

// Query is tokenized case-insensitively → ["quick", "brown"] → matches
var r1 = matcher.Search(doc, "Quick Brown");  // ✅ found

// If the document were case-sensitive, "quick" and "Quick" would NOT match
var csDoc = IndexedDocument.FromMarkdown("The Quick Brown Fox",
    caseSensitive: true);
var r2 = matcher.Search(csDoc, "quick brown");  // ❌ empty
```

The **overlap deduplication** uses a non-standard metric: `|A ∩ B| / min(|A|, |B|) > 0.5`, not Jaccard. This means a short high-scoring span sitting inside a long low-scoring one is kept — they overlap by at most 50% of the smaller span. With Jaccard the same overlap would be much lower and the long span would survive. If you're switching from another dedup strategy, expect different elimination behavior.

One more nuance: **tokenization is markdown-aware but not markdown-specialized**. Backticks, brackets, and slashes are all delimiters, so `https://example.com/path` tokenizes to `["https", "example", "com", "path"]`. Hyphens and dashes (U+002D, U+2010–U+2015) are word characters, making `"state-of-the-art"` a single token — it won't match the query `"state of the art"`. The soft hyphen (U+00AD) is a delimiter, so a document with soft hyphens splits the way you'd expect while real hyphens stay fused. Plan your queries accordingly, or swap in a custom `ITokenizer` at the pipeline boundary.

[ApproximateSpanMatching on GitHub](https://github.com/ypyl/ApproximateSpanMatching) · [Smith-Waterman algorithm](https://en.wikipedia.org/wiki/Smith%E2%80%93Waterman_algorithm) · [OpenSpec design docs](https://github.com/ypyl/ApproximateSpanMatching/tree/main/openspec)
