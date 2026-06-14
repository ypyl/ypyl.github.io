---
layout: presentation
---

# What is eval-cli?

**Cross-platform AI evaluation CLI** — a single tool that any team can use to evaluate LLM responses, regardless of their language stack.

- Runs quality evaluators: **Relevance, Coherence, Fluency, Groundedness, Completeness, Equivalence**
- **Zero runtime dependencies** — self-contained native binary or dotnet tool
- Works with **Azure OpenAI** and any **OpenAI-compatible** endpoint (DeepSeek, OpenCode, etc.)
- No code required — pipe JSON in, get scores out
- Standardized evaluators and caching across teams

> **Notes:** Wraps Microsoft.Extensions.AI.Evaluation into a single binary. Any language can call it via subprocess or stdin pipe — no per-ecosystem packages.

---

# Single Scenario Evaluation

Evaluate individual LLM responses against quality metrics.

```
rm -rf eval-results

eval-cli \
  --endpoint "https://opencode.ai/zen/go/v1" \
  --model "deepseek-v4-pro" \
  --api-key "$API_KEY" \
  --input integration/scenarios.json \
  --evaluators relevance,coherence,fluency \
  --parallel 3
```

> **Notes:** Zero config beyond endpoint + model + input file.

---

# Single Scenario — Output

```
presentation-single — 5 scenarios, 5 groups

demo.knowledge.earth-moon (n=1)
  ✅ Relevance: 4.00
  ❌ Fluency: 3.00
  ✅ Coherence: 4.00

demo.reasoning.water-states (n=1)
  ✅ Relevance: 5.00
  ✅ Coherence: 4.00
  ❌ Fluency: 3.00
```

> **Notes:** Each scenario name is unique, forming one group. Fluency flags short or bare responses — the math answer "136" scores 0 because it has no grammar to judge.

---

# Multi-Run Aggregation

LLMs are non-deterministic. One run isn't enough. Run the same prompt multiple times and see the variance.

```
rm -rf eval-results

eval-cli \
  --endpoint "https://opencode.ai/zen/go/v1" \
  --model "deepseek-v4-pro" \
  --api-key "$API_KEY" \
  --input integration/multi-run-scenarios.json \
  --evaluators relevance,coherence \
  --parallel 3
```

> **Notes:** Same-name scenarios collapse into groups with n=3 runs each; means, std dev, and ranges are computed across those runs.

---

# Multi-Run — Output

```
presentation-multi — 5 scenarios, 2 groups

qa.water-boil (n=3)
  ✅ Coherence: 4.00 ± 0.00  [4.00–4.00]
  ✅ Relevance: 4.33 ± 0.58  [4.00–5.00]

qa.moon-distance (n=2)
  ✅ Relevance: 4.00 ± 0.00  [4.00–4.00]
  ✅ Coherence: 4.00 ± 0.00  [4.00–4.00]
```

> **Notes:** A model with 4.0 ± 0.2 is more reliable than one with 4.5 ± 1.5. Std dev is arguably more important than mean.

---

# What the Stats Tell You

| Pattern | Meaning |
|---------|---------|
| High mean, low std dev | Consistently good |
| High mean, high std dev | Usually good, occasionally bad — **flaky** |
| Low mean, low std dev | Consistently bad — prompt is a weakness |
| Low mean, high std dev | Inconsistent — sometimes ok, mostly not |

Std dev is arguably more important than mean. A model with 4.0 ± 0.2 is more *reliable* than one with 4.5 ± 1.5.

> **Notes:** This is the core insight for teams adopting LLM evaluation. Don't just look at averages — look at variance.

---

# Results Folder — What's on Disk

Every run persists results automatically. No flag needed.

```
eval-results/
  cache/                              ← response cache
  results/
    presentation-multi/               ← library-native format
      qa.moon-distance/
        1.json
        2.json
      qa.water-boil/
        1.json
        2.json
        3.json
  stats/
    presentation-multi/               ← aggregated stats
      qa.moon-distance/
        _stats.json     ← mean, std dev, min, max
      qa.water-boil/
        _stats.json
```

> **Notes:** `aieval report` reads results/ for individual runs; the stats/ layer captures the aggregate picture with mean, stdDev, min, max, and failedFraction per metric.

---

# Rich HTML Reports via aieval

`eval-cli` writes in the library's native format. Use the official `aieval` CLI to generate interactive HTML reports — no extra export step needed.

```
dotnet aieval report -p ./eval-results -o report.html --open
```

Interactive HTML with per-metric scores, ratings, and evaluator reasoning. Execution-level grouping — run multiple evaluations with different `--name` values for trend comparison.

> **Notes:** `aieval` reads the same `eval-results/` directory `eval-cli` writes to. Zero config — the two tools are designed to compose.

---

![aieval HTML report](/assets/images/presentations/aieval-report.png)

> **Notes:** Interactive HTML report with per-metric scores, ratings, and evaluator reasoning.

---

# Why a CLI, Not a Service

| CLI | REST API Service |
|-----|------------------|
| No operational burden | Auth, availability, rate limiting |
| No CI dependency | Central service = single point of failure |
| Local process = no network latency | HTTP round-trips compound |
| CI runners handle long runs naturally | Timeouts and connection management |

The engine library is separated from the CLI entry point. When centralization becomes necessary, the same `EvalEngine.RunAsync()` can power an ASP.NET REST API. The CLI is the first delivery vehicle, not the final architecture.

> **Notes:** Each LLM-based evaluator call takes 1–5 seconds. 30 scenarios × 5 evaluators with parallel execution takes 5–8 minutes. CI runners handle that naturally.


