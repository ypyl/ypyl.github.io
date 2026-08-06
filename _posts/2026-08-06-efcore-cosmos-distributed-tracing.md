---
layout: post
title: "EF Core + Cosmos DB: Why You See No Distributed Tracing Spans"
date: 2026-08-06
categories: programming
tags: [dotnet, ef-core, cosmos-db, open-telemetry, distributed-tracing]
---

EF Core's Cosmos provider never sets `DisableDistributedTracing`. The stable `Microsoft.Azure.Cosmos` SDK defaults the flag to `true`, so no Cosmos spans reach OpenTelemetry no matter how it is configured. EF Core exposes no way to change the property; two open issues track a fix ([dotnet/efcore#37615](https://github.com/dotnet/efcore/issues/37615), [dotnet/efcore#35482](https://github.com/dotnet/efcore/issues/35482)). Until the fix ships, use a preview SDK package, or emit your own spans from EF Core's `DiagnosticSource`.

## The provider never sets the flag

EF Core's Cosmos provider never sets `DisableDistributedTracing`. It has no reference to the property in any version, and no way to configure it: `CosmosDbContextOptionsBuilder` offers no hook to pass a `CosmosClientOptions`. The provider's only tracing is its own `DiagnosticSource` logging.

## Why you see no spans: the SDK default

`Microsoft.Azure.Cosmos` ships the flag in `CosmosClientTelemetryOptions`:

```csharp
public bool DisableDistributedTracing { get; set; } =
#if PREVIEW
    false;
#else
    true;
#endif
```

Stable SDK builds disable distributed tracing by default. Preview builds enable it. Because EF Core never touches the property, the SDK default applies. With the stable SDK, no Cosmos spans are emitted regardless of OpenTelemetry configuration. With a preview SDK package, spans appear without any EF Core change. This matches [dotnet/efcore#37615](https://github.com/dotnet/efcore/issues/37615): the reporter could only get Cosmos traces with preview `Microsoft.Azure.Cosmos` packages.

## Open issues

| Issue | State | What it tracks |
|---|---|---|
| [dotnet/efcore#37615](https://github.com/dotnet/efcore/issues/37615) | open | Add `CosmosClientTelemetryOptions.DisableDistributedTracing` to EF Core. Created 2026-02-02. Maintainers (2026-02-24/25): covered by #35482/#33034; may ship before the full solution "depending on demand". |
| [dotnet/efcore#35482](https://github.com/dotnet/efcore/issues/35482) | open | Allow passing a `CosmosClient` (and `CosmosClientOptions`) directly to `UseCosmos()`. Created 2025-01-15. Maintainer comment (2026-03-29): internally approved; will use reference equality for EF's service provider caching. |
| [dotnet/efcore#33034](https://github.com/dotnet/efcore/issues/33034) | open | Ensure all Cosmos client options are exposed via EF Core. Created 2024-02-08. |
| [dotnet/efcore#29281](https://github.com/dotnet/efcore/issues/29281) | open | Investigate direct OpenTelemetry support for EF Core. Created 2022-10-06. EF Core currently emits only via `DiagnosticSource`. |
| [dotnet/efcore#23203](https://github.com/dotnet/efcore/issues/23203) | open | Expose Cosmos retry policy options. Same family: SDK options EF Core does not expose. |
| [Azure/azure-cosmos-dotnet-v3#4541](https://github.com/Azure/azure-cosmos-dotnet-v3/issues/4541) | open | Stable SDK's `DiagnosticSource` name is `"Azure.Cosmos"`, not `"Azure.Cosmos.Operation"` as documented. |
| [Azure/azure-cosmos-dotnet-v3#4553](https://github.com/Azure/azure-cosmos-dotnet-v3/issues/4553) | open | Distributed tracing / OpenTelemetry feature requests. |
| [Azure/azure-cosmos-dotnet-v3#4202](https://github.com/Azure/azure-cosmos-dotnet-v3/issues/4202) | closed | Same symptom: distributed tracing never emitted to outputs. |

## Workarounds until #35482 lands

1. **Bump `Microsoft.Azure.Cosmos` to a preview version.** The SDK default flips to `false`; tracing works with no code changes.
2. **Replace EF's internal client wrapper.** Implement your own `ISingletonCosmosClientWrapper` that builds the client with `DisableDistributedTracing = false`, and register it with `options.ReplaceService<ISingletonCosmosClientWrapper, MyWrapper>()`. This is an internal API and may break between EF Core versions.
3. **Aspire does not help.** Aspire's Cosmos integration registers its own `CosmosClient` in DI, but `UseCosmos` builds its own internal client. Aspire's client options do not flow through to EF Core.

## Custom spans: EF Core's DiagnosticSource

EF Core emits all database events through `System.Diagnostics.DiagnosticSource`. The listener name is `"Microsoft.EntityFrameworkCore"`. The `OpenTelemetry.Instrumentation.EntityFrameworkCore` contrib package uses the same mechanism, but it instruments only relational `DbCommand` events (`CommandExecuting`/`CommandExecuted`), so it produces nothing for Cosmos.

All Cosmos command events are in the category `Microsoft.EntityFrameworkCore.Database.Command`:

| Event name | Payload | Direction |
|---|---|---|
| `...Command.ExecutingSqlQuery` | `CosmosQueryEventData` | query start |
| `...Command.ExecutingReadItem` | `CosmosReadItemEventData` | read start |
| `...Command.ExecutedReadNext` | `CosmosQueryExecutedEventData` | query done |
| `...Command.ExecutedReadItem` | `CosmosItemCommandExecutedEventData` | read done |
| `...Command.ExecutedCreateItem` | `CosmosItemCommandExecutedEventData` | create done |
| `...Command.ExecutedReplaceItem` | `CosmosItemCommandExecutedEventData` | replace done |
| `...Command.ExecutedDeleteItem` | `CosmosItemCommandExecutedEventData` | delete done |
| `...Command.ExecutedTransactionalBatch` | `CosmosTransactionalBatchExecutedEventData` | batch done |

The executed payloads carry `Elapsed`, `RequestCharge`, `ContainerId`, `PartitionKeyValue`, and for queries only `QuerySql`. These are public API classes in `Microsoft.EntityFrameworkCore.Diagnostics`.

These do not work for Cosmos:

- `IDbCommandInterceptor` / `DbCommandInterceptor`: relational only, Cosmos has no `DbCommand`.
- `OpenTelemetry.Instrumentation.EntityFrameworkCore`: listens for `CommandExecuting`/`CommandExecuted`; Cosmos never fires those events.
- `ISaveChangesInterceptor`: works for Cosmos saves, but gives save-level hooks, not per-query spans.

### Observer implementation

```csharp
using System.Diagnostics;
using Microsoft.EntityFrameworkCore.Diagnostics;

public sealed class CosmosTracingObserver : IObserver<DiagnosticListener>, IObserver<KeyValuePair<string, object?>>, IDisposable
{
    private static readonly ActivitySource Source = new("MyApp.EFCore.Cosmos");

    private static readonly HashSet<string> QueryEvents = new(StringComparer.Ordinal)
    {
        "Microsoft.EntityFrameworkCore.Database.Command.ExecutedReadNext",
        "Microsoft.EntityFrameworkCore.Database.Command.ExecutedReadItem",
        "Microsoft.EntityFrameworkCore.Database.Command.ExecutedCreateItem",
        "Microsoft.EntityFrameworkCore.Database.Command.ExecutedReplaceItem",
        "Microsoft.EntityFrameworkCore.Database.Command.ExecutedDeleteItem",
        "Microsoft.EntityFrameworkCore.Database.Command.ExecutedTransactionalBatch",
    };

    private IDisposable? _listenerSub, _sourceSub;

    public void OnNext(DiagnosticListener listener)
    {
        if (listener.Name == "Microsoft.EntityFrameworkCore")
            _sourceSub = listener.Subscribe(this, IsEnabled);
    }

    private static bool IsEnabled(string eventName) => QueryEvents.Contains(eventName);

    public void OnNext(KeyValuePair<string, object?> pair)
    {
        var operation = pair.Key[(pair.Key.LastIndexOf('.') + 1)..];
        var activity = Source.StartActivity(ActivityKind.Client);
        if (activity is null) return; // no listener registered for this source, no-op

        activity.DisplayName = $"cosmos.{operation}";
        activity.AddTag("db.system", "cosmos");
        activity.AddTag("db.operation", operation);

        switch (pair.Value)
        {
            case CosmosQueryExecutedEventData e:
                activity.AddTag("db.cosmosdb.container", e.ContainerId);
                activity.AddTag("db.statement", e.QuerySql); // gated on e.LogSensitiveData
                activity.AddTag("db.cosmosdb.request_charge", e.RequestCharge);
                activity.SetEndTime(activity.StartTimeUtc + e.Elapsed);
                break;
            case CosmosItemCommandExecutedEventData e:
                activity.AddTag("db.cosmosdb.container", e.ContainerId);
                activity.AddTag("db.cosmosdb.id", e.ResourceId);
                activity.AddTag("db.cosmosdb.request_charge", e.RequestCharge);
                activity.SetEndTime(activity.StartTimeUtc + e.Elapsed);
                break;
            case CosmosTransactionalBatchExecutedEventData e:
                activity.AddTag("db.cosmosdb.container", e.ContainerId);
                activity.AddTag("db.cosmosdb.request_charge", e.RequestCharge);
                activity.SetEndTime(activity.StartTimeUtc + e.Elapsed);
                break;
            default:
                return; // not a Cosmos execution event
        }

        activity.Stop();
    }

    public void OnError(Exception error) { }
    public void OnCompleted() { }
    public void Dispose() { _sourceSub?.Dispose(); _listenerSub?.Dispose(); Source.Dispose(); }
}
```

Behavior notes:

- `Source.StartActivity()` returns `null` unless something registered a listener for `"MyApp.EFCore.Cosmos"` (for example `AddSource("MyApp.EFCore.Cosmos")` in OpenTelemetry setup). Without it, the observer is a no-op.
- The callback runs inside the ambient `Activity.Current` (for example the HTTP request span), so the Cosmos span becomes its child automatically.
- The end time is backdated from the payload's `Elapsed`, which the SDK measures and includes retries. No start/stop pairing is needed.
- To wrap the call instead, add `ExecutingSqlQuery` and `ExecutingReadItem` to the set, start the activity there, and call `Activity.Current?.Stop()` in the executed events. EF dispatches both events synchronously in the same async flow, so `Activity.Current` pairing is reliable.

### Registration in ASP.NET Core

Subscribe once per process, in `Program.cs`, before the first EF Core call:

```csharp
DiagnosticListener.AllListeners.Subscribe(new CosmosTracingObserver());
```

Do not subscribe in middleware (runs per request, duplicate subscriptions), in the `DbContext` constructor or `OnConfiguring` (runs per internal service provider), or in an `IHostedService` (too late if EF runs during host startup, for example seeding).

`DiagnosticListener.AllListeners` replays listeners that already exist, and EF Core creates its listener lazily on first context use, so subscribing in `Program.cs` misses nothing.

`WebApplicationFactory` tests boot multiple hosts in the same process, so `Program.cs` runs multiple times. Guard the subscription:

```csharp
public static class CosmosTracing
{
    private static int _subscribed;

    public static void EnsureSubscribed()
    {
        if (Interlocked.Exchange(ref _subscribed, 1) == 0)
            DiagnosticListener.AllListeners.Subscribe(new CosmosTracingObserver());
    }
}
```

### Filtering out noise

Without the `IsEnabled` predicate, every EF Core event reaches the observer. The noisy ones:

- `ShadowPropertyChanged` (category `ChangeTracking`): fires once per tracked property change during `SaveChanges`. Dozens of spans per save.
- `QueryCompilationStarting` (category `Query`): once per query compilation, then cached.
- `ContextInitialized` (category `Infrastructure`): once per `DbContext` instance.

Keeping the filter to the `Database.Command` executed events excludes all of them. The event set above is the complete list of Cosmos execution events.

## Sources

- EF Core Cosmos source: https://github.com/dotnet/dotnet/tree/main/src/efcore/src/EFCore.Cosmos
- `SingletonCosmosClientWrapper.cs`: `src/efcore/src/EFCore.Cosmos/Storage/Internal/SingletonCosmosClientWrapper.cs`
- `CosmosEventId.cs`: `src/efcore/src/EFCore.Cosmos/Diagnostics/CosmosEventId.cs`
- Cosmos SDK: `Microsoft.Azure.Cosmos/src/CosmosClientTelemetryOptions.cs`, https://github.com/Azure/azure-cosmos-dotnet-v3
- Issues: links in the table in the Open issues section.
