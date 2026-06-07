---
layout: post
title: ".NET AI Dependency Injection with Distributed Cache — Watch Your Memory Size"
date: 2026-05-19
categories: programming
tags: [dotnet, ai, microsoft-extensions-ai, azure-openai, caching]
---

The **dotnet/ai-samples** repository demonstrates how to wire up Azure OpenAI in a .NET application using **`Microsoft.Extensions.AI`** with dependency injection and response caching. The pattern is clean: register an `AzureOpenAIClient` with `DefaultAzureCredential`, wrap it as an `IChatClient` via `AsChatClient("gpt-4o-mini")`, and layer `UseDistributedCache()` on top — all inside a standard `Host.CreateApplicationBuilder()` pipeline:

```csharp
var builder = Host.CreateApplicationBuilder();

builder.Services.AddSingleton(
    new AzureOpenAIClient(
        new Uri(Environment.GetEnvironmentVariable("AZURE_OPENAI_ENDPOINT")),
        new DefaultAzureCredential()));

builder.Services.AddDistributedMemoryCache();

builder.Services.AddChatClient(services =>
        services.GetRequiredService<AzureOpenAIClient>().AsChatClient("gpt-4o-mini"))
    .UseDistributedCache();

var app = builder.Build();
var chatClient = app.Services.GetRequiredService<IChatClient>();
Console.WriteLine(await chatClient.GetResponseAsync("What is AI?"));
```

The catch: `AddDistributedMemoryCache()` uses an in-memory `MemoryCache` under the hood, which has **no size limit by default**. In a long-running application with many chat interactions, cached responses will grow indefinitely and can lead to excessive memory consumption.

To cap the cache, pass a `SizeLimit` option. This wasn't obvious at first glance — the official docs describe `SizeLimit` units as "arbitrary" and dependent on how entries are sized. But looking at the `MemoryDistributedCache` source, it explicitly sets each entry's size to the byte array length:

```csharp
// MemoryDistributedCache.cs (dotnet/runtime)
entry.Size = value.Length;
```

This means `SizeLimit` is effectively in **bytes** for this scenario — the total byte size of all stored values:

```csharp
builder.Services.AddDistributedMemoryCache(options =>
{
    options.SizeLimit = 20 * 1024 * 1024; // 20 MB = 20,971,520 bytes
});
```

20 MB in bytes: 20 × 1,024 × 1,024 = **20,971,520**. When the total byte size of all cached entries exceeds the limit, the oldest entries are evicted. This is especially useful for keeping repeated queries efficient without risking unbounded memory growth in production.

Another gotcha: **register the chat client as a singleton**. `AddChatClient` defaults to transient registration — each `IChatClient` injection creates a brand-new `DistributedCachingChatClient` with its own decorator chain, even though the underlying `AzureOpenAIClient` and `IDistributedCache` are singletons. That means every resolution allocates a fresh pipeline (inner client + cache wrapper), which adds unnecessary GC pressure. Worse, if you attach middleware like `UseOpenTelemetry()`, each transient instance gets its own telemetry state, producing duplicate or inconsistent traces. Force singleton lifetime:

```csharp
builder.Services.AddSingleton<IChatClient>(services =>
    new DistributedCachingChatClient(
        services.GetRequiredService<AzureOpenAIClient>().AsChatClient("gpt-4o-mini"),
        services.GetRequiredService<IDistributedCache>()));
```

[dotnet/ai-samples: DependencyInjection.cs](https://github.com/dotnet/ai-samples/blob/main/src/microsoft-extensions-ai/azure-openai/AzureOpenAIExamples/DependencyInjection.cs) · [MemoryCacheOptions.SizeLimit docs](https://learn.microsoft.com/en-us/dotnet/api/microsoft.extensions.caching.memory.memorycacheoptions.sizelimit)
