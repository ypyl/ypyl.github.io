---
layout: post
title: "eval-cli: Cross-platform AI evaluation CLI wrapping Microsoft.Extensions.AI.Evaluation"
date: 2026-06-04
tags: dotnet, ai, evaluation, microsoft-extensions-ai, python, cross-platform, cli, native-aot
categories: programming
---

Following up on the [previous post about evaluating AI responses in .NET]({% post_url 2026-05-29-dotnet-ai-evaluation-quality-reporting %}), I built a CLI tool that wraps `Microsoft.Extensions.AI.Evaluation` into a single binary usable from any language. Repository: [github.com/ypyl/ai-eval-cli](https://github.com/ypyl/ai-eval-cli).

## Why

Microsoft's AI evaluation story is split across two separate products: `Microsoft.Extensions.AI.Evaluation` (NuGet, local library, .NET only) and `azure-ai-evaluation` (PyPI, cloud service, Python only). There is no .NET SDK for the Foundry cloud evaluation service, and the REST API doesn't expose row-level results programmatically. R has no SDK at all.

A single CLI binary that any language can call via subprocess or stdin pipe solves this without per-ecosystem packages.

## Why a console app, not a service

A REST API was the other option. It would be language-agnostic by definition and offer centralized management. The trade-offs that pushed toward a CLI:

- **No operational burden.** A service means owning auth, availability, rate limiting, scaling, cost attribution. A CLI binary has none of this — teams run it in their own CI pipelines.
- **No CI dependency.** If a central evaluation service goes down, other teams' builds can't run. A CLI fails only if the team's own machine fails.
- **Latency.** Each LLM-based evaluator call takes 1–5 seconds. Over HTTP, this compounds. A local process avoids network round-trips.
- **Long-running evaluations are fine in a CLI.** 30 scenarios × 5 evaluators with parallel execution takes 5–8 minutes. CI runners handle that naturally.

The engine library (`AiEvalCli.Engine`) is separated from the CLI entry point. When centralization becomes necessary, the same `EvalEngine.RunAsync()` method can power an ASP.NET REST API. The CLI is the first delivery vehicle, not the final architecture.

## Judge model constraint

The quality evaluators (`RelevanceEvaluator`, `CoherenceEvaluator`, etc.) are LLM-as-judge: they send the conversation and response to another model for scoring. Microsoft's evaluator prompts were tuned against GPT-4o and GPT-4.1.

From the [official docs](https://devblogs.microsoft.com/dotnet/exploring-agent-quality-and-nlp-evaluators/):

> *"The evaluation prompts... have been tuned and tested against OpenAI models such as GPT-4o and GPT-4.1. It is possible to use other models... however, the performance of those models against the evaluation prompts may vary and may be especially poor for smaller / local models."*

I confirmed this in testing. The evaluators call the configured model to score each scenario's response. With DeepSeek v4 flash as the judge, numeric scores came through but reasoning text was empty — the model didn't format judge responses the same way as GPT-4o. A `--judge-model` flag to specify a separate scoring model (GPT-4o-mini, ideally) while keeping the endpoint flexible is the next planned feature.

## Multi-provider support

The tool supports Azure OpenAI (with `DefaultAzureCredential` or API key) and any OpenAI-compatible endpoint:

```bash
# Azure OpenAI
eval-cli --endpoint "https://my.openai.azure.com" --model "gpt-4o-mini" --input scenarios.json

# OpenAI-compatible (DeepSeek, OpenCode, etc.)
eval-cli --provider openai \
  --endpoint "https://opencode.ai/zen/go/v1" \
  --model "deepseek-v4-flash" \
  --api-key "sk-..." --input scenarios.json
```

Scenarios are provided as a JSON array of `{name, userQuery, response, systemPrompt?, context?, referenceAnswer?}` objects, or piped via stdin. The `response` field contains the pre-existing LLM output to evaluate — the tool does not generate responses.

## AOT plans

The project is configured for self-contained single-file deployment. Native AOT is the next step — it needs validation against `Azure.AI.OpenAI` and `Azure.Identity` dependencies, which have historically used reflection patterns incompatible with AOT static analysis.

## Next: REST API service

The shared engine library was designed for this. Adding an ASP.NET Web API project that calls `EvalEngine.RunAsync()` with a job queue for async batch submission is the next phase. This gives teams the option of fire-and-forget evaluation without blocking CI, while the CLI remains available for local use and fast iteration.

## Links

- [eval-cli on GitHub](https://github.com/ypyl/ai-eval-cli)
- [Microsoft.Extensions.AI.Evaluation docs](https://learn.microsoft.com/en-us/dotnet/ai/conceptual/evaluation-libraries)
