---
layout: post
title: "What Matters From .NET Conf 2026: Day 1"
date: 2026-11-11
categories: programming
tags: [dotnet, csharp, ai-agents, blazor, dotnet-conf]
---

**.NET 11** shipped on November 10, 2026, as a **Standard Term Support release with two years of support**, and the one line that costs people money sits in the same paragraph: .NET 8 and .NET 9 reached end of support the same day. The rest of day 1 was a platform release with three things worth acting on, a language feature that changes how I model outcomes, an agent stack that finally has an end-to-end shape, and tooling work that removes seconds from every command I type. Everything below is labelled by how safe it is to adopt today.

## C# 15: unions and closed hierarchies

**Union types** are the headline. A value can be one of a declared set of case types, with conversions in and exhaustiveness checking in `switch`:

```csharp
public sealed record Product(int Id, string Name);
public sealed record ProductNotFound(int Id);

public union ProductLookup(Product, ProductNotFound);

ProductLookup result = new Product(42, "Mechanical keyboard");

string message = result switch
{
    Product product => $"Found {product.Name}",
    ProductNotFound missing => $"No product with ID {missing.Id}",
};
```

The return type now names the outcomes a caller must handle. No `object`, no surrogate base class, no documentation comment promising which cases exist. **Closed hierarchies** cover the other half of the problem: a `closed class` limits direct derivation to its declaring assembly, which gives the compiler enough information to check patterns over the whole hierarchy. Use unions to combine types you already have, closed hierarchies where you own the inheritance relationship.

Smaller additions ship in the same release without a preview flag: collection expression arguments (`[with(StringComparer.OrdinalIgnoreCase), "Alice", "ALICE"]` deduplicates to two entries), extension indexers, and labeled `break`/`continue`. The memory safety work stays in preview.

The part that makes unions a platform feature rather than a language feature: **ASP.NET Core serializes them anywhere it uses System.Text.Json**, so minimal API and MVC bodies work, and generated OpenAPI documents describe the alternatives with `anyOf`. The boundaries are JSON bodies only. Route, query, header and form binding get nothing, and when two cases share the same JSON shape you have to configure a classifier before deserialization can tell them apart.

## The agent stack now has an end-to-end shape

Four pieces, and this year they can be described as one path instead of four products: **Microsoft.Extensions.AI** (`IChatClient`, `IEmbeddingGenerator`) for model access, **MCP** for tool integration, **Microsoft Agent Framework** for agents and workflows, and **AG-UI** for the UI layer.

Three concrete additions from day 1:

- `dotnet new mcpserver` ships **in the SDK**, on the official MCP C# SDK v2.0, with a working sample tool. Exposing an existing service as a tool no longer starts with installing a template package.
- **Blazor AI components** (`Microsoft.AspNetCore.Components.AI`): streaming chat, tool rendering, approval flows, shared UI state. Basic chat works with any `IChatClient`. The package is **experimental and stays prerelease for the whole of .NET 11**.
- The **AG-UI .NET integration** carries streamed responses, tool events, approval requests and shared-state updates between an ASP.NET Core agent endpoint and the Blazor UI. Also prerelease.

The approval-flow and shared-state pair is what was missing for human-in-the-loop agents in a real workflow, and it is also the part most likely to change shape before .NET 12. Treat it as a direction to prototype against, not a foundation to build a product on.

The developer-facing agent tooling got equal weight:

- **.NET Skills** (`dotnet/skills`) feeds task-specific .NET guidance to coding agents, and there is a related push to deliver skills **inside NuGet packages** so a library can ship current documentation to a model instead of hoping the training data remembered.
- **C# Dev Kit 11** was rebuilt from five back-end processes into one Native AOT server with persistent caches. The team's figure for large workspaces is over a minute to load down to a few seconds.
- **.NET MAUI DevFlow** (experimental) exposes a running app's visual tree, screenshots and interaction tools so an agent can inspect and test the UI instead of reasoning about source.
- The **.NET Upgrade canvas** shows a GitHub Copilot modernization session as a reviewable surface: assessment findings, dependency compatibility, task progress.

## Two runtime bets, both in preview

**Runtime Async** moves async suspension and resumption out of Roslyn-generated state machines and into RyuJIT. The throughput argument is secondary for me. The useful result is a live call stack in a profiler that shows the async methods themselves rather than the machinery executing them. It is opt-in, on `net11.0`, without enabling all preview features:

```xml
<PropertyGroup>
  <Features>runtime-async=on</Features>
</PropertyGroup>
```

The runtime libraries are already built with it. Your code opting in is a separate decision, and one to validate against your profilers before it touches production.

The second bet is the **memory safety model**, stricter compile-time enforcement and better auditability of `unsafe`, also preview. Everything shipped and safe today is smaller: more JIT devirtualization and bounds-check removal, faster Native AOT interface dispatch, FP16 hardware support for `Half`, and in-process crash report logging that now works on Linux and macOS as well as mobile, capturing a managed stack trace before the process dies.

Post-quantum work expanded: `CompositeMLKem` combines post-quantum and classical key encapsulation, and `Hpke` arrived. Both are experimental, behind `SYSLIB5006` and `SYSLIB5009`. Check `CompositeMLKem.IsAlgorithmSupported` rather than assuming the platform has it.

## The CLI is a native app, and the build server stays warm

The `dotnet` CLI host is **Native AOT by default**, so help and version queries skip managed startup entirely. The **MSBuild server is on by default**, keeping a warm worker between invocations; opt out with `DOTNET_CLI_USE_MSBUILD_SERVER=false`.

Multithreaded MSBuild arrives as `-mt`. The numbers from the preview 6 release notes are the reason to try it:

| Change | Measured effect |
|---|---|
| MSBuild server node moved to Server GC | 233-project solution, median build 90.6 s to 81.0 s |
| Out-of-proc pipe buffer 128 KB to 1 MB | transport send time 114.2 s to 9.0 s |
| Parallel project evaluation, per-path locks instead of a global lock | 200 projects on 16 threads, 1002 ms to 530 ms |

The catch: non-thread-safe custom tasks get moved to sidecar TaskHosts, so `-mt` is a measure-it-per-repo change, not a switch to flip everywhere. Also new: `dotnetup`, a rustup-style toolchain manager in public preview, installing and updating user-local SDKs on macOS and Linux.

## Libraries, web, desktop

The library release is a long list of things that previously needed a helper: **Zstandard** in `System.IO.Compression` alongside a span-based encode/decode surface for Deflate, ZLib and GZip; a **Process API** with one-shot run-and-capture, inherited handles and suspended starts; four in-memory stream adapters (`ReadOnlyMemoryStream`, `WritableMemoryStream`, `ReadOnlySequenceStream`, `StringStream`) so a segmented `ReadOnlySequence<byte>` can be read without flattening it first; `FullJoin`, `LeftJoin` and `RightJoin` in LINQ; JSON support for C# unions and JSON Lines from async sequences.

Numerics had its own session for good reason: **IEEE 754 `Decimal32`, `Decimal64` and `Decimal128`**, `BFloat16`, generic `Complex<T>`, FP16 intrinsics, and `INumberBase<TSelf>.TryParsePartial` for delimiter-aware parsing.

On the web side, three changes reach production code quickly: **asynchronous validation** with the new async DataAnnotations (a validator can await a database lookup instead of blocking a thread), **OpenAPI 3.2 as the default document version**, and **SignalR authentication refresh** without reconnecting, which Blazor Interactive Server components can observe. Blazor also gains `CacheView` for caching a rendered subtree during static SSR, and automatic circuit pause on inactive tabs, the latter requiring an extra package and explicit configuration so upgrades do not change circuit behaviour silently.

EF Core 11 requires .NET 11, removes joins in more reference-navigation and split-query cases, and translates `FullJoin`. **SQL Server vector indexes and approximate vector search are experimental.** In MAUI, **CoreCLR is now the default runtime** across platforms, XAML Incremental Hot Reload is preview, `dotnet run`/`test`/`watch` work on devices and emulators, and the `Microsoft.Maui.Controls.Compatibility` package no longer ships. WinForms got an **opt-in modern visual styles mode** at application or control level, which is the right shape: an upgrade is not an automatic visual redesign.

## What I am doing with it

1. **Model outcomes with unions at API and domain boundaries now.** The OpenAPI `anyOf` output is what makes it a contract tool rather than syntax sugar.
2. **Try `-mt` behind a benchmark on the largest solution I own.** The wins are large and the failure mode is a task that was never thread-safe.
3. **Prototype AG-UI plus Blazor AI components on an internal tool where approval is the actual requirement.** Expect breakage before .NET 12.
4. **Leave Runtime Async alone on latency-sensitive services** until the profiler story is proven, and start reading the memory safety preview so the `unsafe` audit is not a surprise.
5. **Treat the .NET 8 end of support as a live incident**, not a backlog item. .NET 10 is the LTS exit, .NET 11 the STS one.

The release exists in the usual two shapes: more than 7,000 contributors and more than 589,000 unique packages on NuGet across 1.1 trillion downloads, and a support window that is either two years or zero depending on what your services target today.

[Announcing .NET 11](https://devblogs.microsoft.com/dotnet/announcing-dotnet-11/) · [Day 1 livestream](https://www.youtube.com/watch?v=SIBdHRrk8os) · [.NET Conf 2026 agenda](https://www.dotnetconf.net/agenda) · [What's new in .NET 11](https://learn.microsoft.com/dotnet/core/whats-new/dotnet-11/overview) · [ASP.NET Core 11 release notes](https://learn.microsoft.com/aspnet/core/release-notes/aspnetcore-11) · [MSBuild preview 6 notes](https://github.com/dotnet/core/blob/main/release-notes/11.0/preview/preview6/msbuild.md)
