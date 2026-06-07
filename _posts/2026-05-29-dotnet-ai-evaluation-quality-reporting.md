---
layout: post
title: "Evaluating AI Responses in .NET with Microsoft.Extensions.AI.Evaluation"
date: 2026-05-29
tags: [dotnet, ai, evaluation, microsoft-extensions-ai, testing, mstest]
categories: programming
---

You're building an LLM-powered feature in .NET. You've got your prompt, your RAG pipeline, your `IChatClient`. It works — but is it *good*? Does the model stay coherent across question types? Are responses grounded in your documents or hallucinating? How do you even answer these questions systematically?

**Microsoft.Extensions.AI.Evaluation** is Microsoft's answer. It's a suite of NuGet packages that let you embed AI quality evaluations directly into your test project — same test frameworks, same CI pipeline, same `dotnet test` workflow. This post walks through the two packages at the heart of it: **Quality** (the evaluators) and **Reporting** (caching, result storage, and report generation).

---

## The Packages You Need

The evaluation libraries are layered. You don't need all of them — pick what fits your use case:

```bash
# Core + Quality evaluators + Reporting (caching + storage + reports)
dotnet add package Microsoft.Extensions.AI.Evaluation
dotnet add package Microsoft.Extensions.AI.Evaluation.Quality
dotnet add package Microsoft.Extensions.AI.Evaluation.Reporting

# For your AI backend (Azure OpenAI shown here)
dotnet add package Azure.AI.OpenAI
dotnet add package Azure.Identity
dotnet add package Microsoft.Extensions.AI.OpenAI

# For configuration / secrets
dotnet add package Microsoft.Extensions.Configuration.UserSecrets
```

| Package | What it gives you |
|---|---|
| `Microsoft.Extensions.AI.Evaluation` | Core types: `IEvaluator`, `EvaluationResult`, `NumericMetric`, `EvaluationMetricInterpretation` |
| **`Microsoft.Extensions.AI.Evaluation.Quality`** | **LLM-based evaluators: Relevance, Coherence, Fluency, Groundedness, and more** |
| **`Microsoft.Extensions.AI.Evaluation.Reporting`** | **`ScenarioRun`, response caching, result persistence, report generation APIs** |
| `Microsoft.Extensions.AI.Evaluation.NLP` | Traditional NLP metrics (BLEU, GLEU, F1) — no LLM needed |
| `Microsoft.Extensions.AI.Evaluation.Safety` | Content safety evaluators backed by Azure AI Foundry |
| `Microsoft.Extensions.AI.Evaluation.Reporting.Azure` | Azure Storage backend for cached responses and results |
| `Microsoft.Extensions.AI.Evaluation.Console` | `aieval` CLI tool for generating reports from the command line |

---

## Step 1: Set Up the Reporting Configuration

The `ReportingConfiguration` is the central hub. It tells the system:

- **Which evaluators** to run against every response
- **Which LLM endpoint** the evaluators should use (quality evaluators ask an LLM to judge responses)
- **Where to store** results and cached responses
- **What execution name** to group results under (so you can compare runs over time)

```csharp
using Azure.AI.OpenAI;
using Azure.Identity;
using Microsoft.Extensions.AI;
using Microsoft.Extensions.AI.Evaluation;
using Microsoft.Extensions.AI.Evaluation.Quality;
using Microsoft.Extensions.AI.Evaluation.Reporting;
using Microsoft.Extensions.AI.Evaluation.Reporting.Storage;

private static ChatConfiguration GetChatConfiguration()
{
    IConfigurationRoot config = new ConfigurationBuilder()
        .AddUserSecrets<MyTests>().Build();

    string endpoint = config["AZURE_OPENAI_ENDPOINT"];
    string tenantId = config["AZURE_TENANT_ID"];

    AzureOpenAIClient azureClient = new(
        new Uri(endpoint),
        new DefaultAzureCredential(
            new DefaultAzureCredentialOptions { TenantId = tenantId }));
    IChatClient client = azureClient
        .GetChatClient(deploymentName: "gpt-4o-mini")
        .AsIChatClient();

    return new ChatConfiguration(client);
}
```

Now wire it all together:

```csharp
private static string ExecutionName => $"{DateTime.Now:yyyyMMddTHHmmss}";

private static readonly ReportingConfiguration s_reportingConfig =
    DiskBasedReportingConfiguration.Create(
        storageRootPath: @"C:\TestReports",
        evaluators: [
            new RelevanceEvaluator(),
            new CoherenceEvaluator(),
            new GroundednessEvaluator(),
            new WordCountEvaluator()        // custom evaluator — see below
        ],
        chatConfiguration: GetChatConfiguration(),
        enableResponseCaching: true,
        executionName: ExecutionName);
```

Two design decisions worth calling out:

**`DiskBasedReportingConfiguration`** stores everything on local disk — cache and results live on the same machine that ran the tests. In ephemeral CI environments (fresh agent per run), the cache is lost between builds and every run pays full LLM cost. For persistent, team-wide caching, swap to **`AzureStorageReportingConfiguration`** backed by an Azure Storage container — all developers and CI agents share the same cache. Both implement the same `ReportingConfiguration` base.

**`ExecutionName`** groups all results from the same test run. The docs recommend a timestamp, build number, or assembly version — anything that uniquely identifies one evaluation pass. If you omit it, everything lands under `"Default"` and new runs overwrite old ones, losing the ability to track trends.

---

## Step 2: Write the Test That Runs the Evaluation

The evaluators run inside standard MSTest (or xUnit/NUnit) test methods. Each test represents one **scenario** — a specific prompt you want to evaluate. The pattern is:

1. Create a `ScenarioRun`
2. Get the LLM response (using the `IChatClient` from the scenario — this gives you caching)
3. Call `scenario.EvaluateAsync()` to run all configured evaluators
4. Optionally assert on the results

```csharp
public TestContext? TestContext { get; set; }

private string ScenarioName =>
    $"{TestContext!.FullyQualifiedTestClassName}.{TestContext.TestName}";

[TestMethod]
public async Task MoonDistanceResponse()
{
    await using ScenarioRun scenario =
        await s_reportingConfig.CreateScenarioRunAsync(ScenarioName);

    // Get the IChatClient from the scenario — this enables response caching
    IChatClient chatClient = scenario.ChatConfiguration!.ChatClient;

    IList<ChatMessage> messages =
    [
        new ChatMessage(ChatRole.System,
            "You're an AI assistant that answers astronomy questions. " +
            "Keep responses concise and under 100 words."),
        new ChatMessage(ChatRole.User,
            "How far is the Moon from the Earth at its closest and furthest points?")
    ];

    ChatResponse response = await chatClient.GetResponseAsync(messages);

    // Run all configured evaluators against the response
    EvaluationResult result = await scenario.EvaluateAsync(messages, response);

    // Validate — optional, see trade-offs below
    ValidateResult(result);
}
```

### Why `await using` on the ScenarioRun?

Disposing the `ScenarioRun` triggers persistence of evaluation results to the storage root. If you forget to dispose, results might not get saved — and won't show up in reports.

### The scenario name matters for report hierarchy

The report generator splits scenario names on `.` to create a hierarchical view. Using `FullyQualifiedTestClassName.TestName` (e.g., `TestAIWithReporting.MyTests.MoonDistanceResponse`) groups results by namespace → class → method in the HTML report — you get nested aggregation and drill-down for free.

---

## Step 3: Understand Response Caching

When `enableResponseCaching: true` and you use the `IChatClient` from the `ScenarioRun` (not your own), the library:

1. **First run**: calls the LLM, stores the response in the configured cache backend
2. **Subsequent runs** (same prompt + same model): serves from cache — zero LLM cost
3. **Cache expiry**: 14 days by default, then it re-fetches

**Where the cache lives matters.** With `DiskBasedReportingConfiguration`, the cache is a local directory — subsequent runs on *your machine* are nearly free, but a CI agent on a fresh ephemeral runner starts cold every build. With `AzureStorageReportingConfiguration`, the cache is an Azure Storage container shared across all developers and CI agents — the second person (or build) to run a test benefits from the first one's cached responses.

This matters because quality evaluators are themselves LLM calls — `RelevanceEvaluator` sends your conversation + response back to the model and asks it to score relevance. Without caching, each test run hits the LLM for both the primary response and all evaluation turns. With a shared cache, the team pays the LLM cost once per unique prompt+model combination, not once per person per run.

---

## Step 4: The Built-In Quality Evaluators

All quality evaluators live in `Microsoft.Extensions.AI.Evaluation.Quality`. They ask an LLM to judge a response and return a `NumericMetric` with a score (1–5, where 1 is poor and 5 is excellent) and a human-readable reason explaining the score. Scores are then mapped to the `EvaluationRating` enum: `Unknown` → `Inconclusive` → `Unacceptable` → `Poor` → `Average` → `Good` → `Exceptional`.

> **Heads up:** The evaluation prompts are tuned for and tested against **GPT-4o**. They'll work with other models, but performance can vary — especially with smaller or local models. The `ChatConfiguration.ChatClient` you supply is what the evaluators use, so pick the best model you can for the evaluation itself.

| Evaluator | Metric name | What it measures | Notes |
|---|---|---|---|
| `RelevanceEvaluator` | `Relevance` | How relevant the response is to the query | |
| `CompletenessEvaluator` | `Completeness` | How comprehensive and accurate the response is | |
| `CoherenceEvaluator` | `Coherence` | Logical flow and orderly presentation of ideas | |
| `FluencyEvaluator` | `Fluency` | Grammar, vocabulary range, readability | |
| `GroundednessEvaluator` | `Groundedness` | How well the response aligns with provided context | Requires `GroundednessEvaluatorContext` with grounding text |
| `EquivalenceEvaluator` | `Equivalence` | Similarity between generated text and ground truth | Requires a reference answer via `EquivalenceEvaluatorContext` |
| `RetrievalEvaluator` | `Retrieval` | Performance in retrieving information for additional context | |
| `RelevanceTruthAndCompletenessEvaluator` | `Relevance (RTC)`, `Truth (RTC)`, `Completeness (RTC)` | Multi-metric CoT prompt — returns three scores in one evaluator call | Marked experimental |
| `IntentResolutionEvaluator` | `Intent Resolution` | How well the AI identifies and resolves user intent | Agent-focused |
| `TaskAdherenceEvaluator` | `Task Adherence` | How well the AI sticks to its assigned task | Agent-focused |
| `ToolCallAccuracyEvaluator` | `Tool Call Accuracy` | How effectively the AI uses supplied tools | Agent-focused |

For groundedness evaluations, pass context the evaluator can compare against:

```csharp
await scenario.EvaluateAsync(
    messages,
    response,
    additionalContext: [
        new GroundednessEvaluatorContext(
            "The Moon's orbit is elliptical. At perigee (closest), " +
            "it is about 225,623 miles from Earth. At apogee " +
            "(farthest), about 252,088 miles.")
    ]);
```

---

## Step 5: Write a Custom Evaluator

Not all metrics need an LLM. Sometimes a simple word count, execution time, or regex check is more useful — and cheaper. Implement `IEvaluator`:

```csharp
using System.Text.RegularExpressions;
using Microsoft.Extensions.AI;
using Microsoft.Extensions.AI.Evaluation;

public class WordCountEvaluator : IEvaluator
{
    public const string WordCountMetricName = "Words";

    public IReadOnlyCollection<string> EvaluationMetricNames
        => [WordCountMetricName];

    private static int CountWords(string? input)
    {
        if (string.IsNullOrWhiteSpace(input)) return 0;
        return Regex.Matches(input, @"\b\w+\b").Count;
    }

    private static void Interpret(NumericMetric metric)
    {
        if (metric.Value is null)
        {
            metric.Interpretation = new EvaluationMetricInterpretation(
                EvaluationRating.Unknown, failed: true,
                reason: "Failed to calculate word count.");
        }
        else if (metric.Value > 5 && metric.Value <= 100)
        {
            metric.Interpretation = new EvaluationMetricInterpretation(
                EvaluationRating.Good,
                reason: "Response was between 6 and 100 words.");
        }
        else
        {
            metric.Interpretation = new EvaluationMetricInterpretation(
                EvaluationRating.Unacceptable, failed: true,
                reason: "Response was either too short or over 100 words.");
        }
    }

    public ValueTask<EvaluationResult> EvaluateAsync(
        IEnumerable<ChatMessage> messages,
        ChatResponse modelResponse,
        ChatConfiguration? chatConfiguration = null,
        IEnumerable<EvaluationContext>? additionalContext = null,
        CancellationToken cancellationToken = default)
    {
        // modelResponse.Text concatenates all messages in the response.
        // For a single-message response this is equivalent to
        // modelResponse.Messages[0].Text.
        int wordCount = CountWords(modelResponse.Text);
        var metric = new NumericMetric(
            WordCountMetricName, wordCount,
            $"Response contained {wordCount} words.");
        Interpret(metric);
        return new ValueTask<EvaluationResult>(new EvaluationResult(metric));
    }
}
```

Add it to your evaluator list like any built-in evaluator — it runs as part of the same pipeline:

```csharp
evaluators: [
    new RelevanceEvaluator(),
    new CoherenceEvaluator(),
    new WordCountEvaluator()
]
```

---

## Step 6: Validate Results (or Don't)

You *can* assert on individual evaluation results within your test:

```csharp
private static void ValidateResult(EvaluationResult result)
{
    NumericMetric relevance =
        result.Get<NumericMetric>(RelevanceEvaluator.RelevanceMetricName);
    Assert.IsFalse(relevance.Interpretation!.Failed);
    Assert.IsTrue(relevance.Interpretation.Rating
        is EvaluationRating.Good or EvaluationRating.Exceptional);

    NumericMetric wordCount =
        result.Get<NumericMetric>(WordCountEvaluator.WordCountMetricName);
    Assert.IsTrue(wordCount.Value > 5 && wordCount.Value <= 100);
}
```

But the docs offer a counterpoint: LLM responses are non-deterministic. Evaluation scores shift naturally as models and prompts evolve. Asserting on every run can **block CI builds unnecessarily** — a legitimate model change might drop relevance from `Exceptional` to `Good`, and that's not a bug, it's a signal.

The full `EvaluationRating` scale is: `Unknown` → `Inconclusive` → `Unacceptable` → `Poor` → `Average` → `Good` → `Exceptional`. Quality evaluators produce scores from 1 to 5 that map into this scale. When deciding thresholds for assertions, pick the rating levels that match your quality bar — don't just check for `Good` or `Exceptional` if `Average` is acceptable for your use case.

A pragmatic approach: let all tests pass regardless of scores, rely on the **report** to surface trends, and only add hard assertions when a specific metric crossing a threshold means *shipping is blocked*.

---

## Step 7: Generate Reports with the `aieval` CLI

After `dotnet test` has stored results to your `storageRootPath`:

```bash
# Install the console tool (do this once)
dotnet tool install Microsoft.Extensions.AI.Evaluation.Console --create-manifest-if-needed

# Generate an HTML report and open in browser
dotnet tool run aieval report --path C:\TestReports --output report.html --open
```

The HTML report renders a hierarchical view: namespace → class → test method → metrics. Each metric card shows the numeric score, interpretation rating, and the LLM's reasoning. You can drill into individual scenario runs and compare across execution names.

For CI/CD, generate the report as a pipeline artifact:

```yaml
- name: Run evaluation tests
  run: dotnet test

- name: Generate report
  run: dotnet tool run aieval report -p C:\TestReports -o report.html

- name: Publish artifact
  uses: actions/upload-artifact@v4
  with:
    name: ai-eval-report
    path: report.html
```

The `aieval` tool also supports JSON output (`--format json`) for custom analysis, and commands for cleaning up cached responses and stale result data — run `dotnet aieval --help` to see the full set.

---

## The Full Picture

Putting it all together, here's the flow:

1. **Configure** — `DiskBasedReportingConfiguration` with your evaluators, LLM endpoint, and storage path
2. **Test** — each test method is a scenario: create a `ScenarioRun`, call the LLM, call `EvaluateAsync`
3. **Cache** — first run pays the LLM cost, subsequent runs reuse cached responses (both primary and evaluation turns)
4. **Store** — results persist to disk on `ScenarioRun` disposal
5. **Report** — `aieval report` turns stored data into an HTML dashboard or JSON data set
6. **Trend** — over multiple runs (via execution names), you can see whether your system is improving or regressing

The libraries are designed to feel like a natural extension of the .NET test ecosystem — MSTest, xUnit, `dotnet test`, Test Explorer, CI pipelines — rather than a separate evaluation platform. If you're already testing your .NET code, you're already set up to evaluate your AI responses.

---

## Links

- [Official docs: Evaluation libraries overview](https://learn.microsoft.com/en-us/dotnet/ai/evaluation/libraries)
- [Tutorial: Evaluate with caching and reporting](https://learn.microsoft.com/en-us/dotnet/ai/evaluation/evaluate-with-reporting)
- [NuGet: Microsoft.Extensions.AI.Evaluation.Quality](https://www.nuget.org/packages/Microsoft.Extensions.AI.Evaluation.Quality)
- [NuGet: Microsoft.Extensions.AI.Evaluation.Reporting](https://www.nuget.org/packages/Microsoft.Extensions.AI.Evaluation.Reporting)
- [NuGet: Microsoft.Extensions.AI.Evaluation.Console](https://www.nuget.org/packages/Microsoft.Extensions.AI.Evaluation.Console)
- [API usage examples](https://github.com/dotnet/ai-samples/tree/main/src/microsoft-extensions-ai-evaluation)
- [Source code](https://github.com/dotnet/extensions)
