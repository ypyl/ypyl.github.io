---
layout: post
title: "What Survives a .NET Port: 771 Lines of a Composition Paradigm"
date: 2026-11-18
categories: programming
tags: [dotnet, csharp, architecture, plugin, testing]
---

Composition theory is comfortable with code that is known at compile time: imports, interfaces, function calls. The interesting case is the opposite one, a component that arrives after deployment, runs for a while, and leaves. Two things have to hold for that to be safe. The system must be able to reverse what the component did, and it must react when whatever the component depended on changes.

The paper I spent a day on, "A Programming Paradigm for Spatiotemporal Composability", calls those **temporal** and **spatial** composability, and proves the two carry from a single component to a whole interleaved system.

I do not learn design from proofs, so I rebuilt it. `cordis-sharp` is that rebuild: a small C# core, a demo, and tests, in the spirit of the paper's TypeScript implementation, Cordis. The goal was never a framework. It was to find the point where the idea stops being theory and starts being code, and then to find where the translation breaks.

The result is **771 lines across 14 files**, with 28 tests passing in under a second. Undoing what a component did, resolving what it depends on, and tearing both down in the right order are ordinary engineering, and all three worked on the first serious attempt. Unloading code is the part that does not fit, because the CLR resists it at every step. And one thing turned out better here than in the paper: its guarantees rest on an obligation nobody enforces, and in .NET a test can enforce it.

This is a work note, not a tutorial. What follows is what the port cost, what I would reuse, where the guarantee stops, and the one experiment I would run before trusting any of it in production.

## The core is smaller than the paper

Reading the reference implementation first was the right order of work. At 1,820 lines of TypeScript core it carries Koishi and its 4,000-odd plugins, and once that was in my head the port became a handful of decisions rather than a translation exercise:

- **Interfaces are keys.** A component declares `IReadOnlySet<Type>`, so a missing declaration is a compile error and key collision is an assembly-identity question rather than a string comparison.
- **`ctx.Get<T>()` is explicit.** The reference resolves `ctx[name]` through a `Proxy` get trap, the only device in the design with no clean C# equivalent.
- **Undo runs sequentially, last in first out.** The shipped package starts its disposers in reverse and then awaits them concurrently, which is faster and much harder to reason about.
- **A missing dependency parks the component.** It does not throw, and it is not retried on a timer.

None of those are clever. That is the finding: the difficulty lives in the metatheory, not in the code.

## Teardown is where the money is

The primitive underneath everything is one function: run a change, and hand back the inverse that fits the state you just created. The runtime folds those inverses into a single accumulator and runs it in reverse. It never checks that the inverse is right. Correctness is the author's, and the type system does not help.

`EffectScope` is that primitive without any component machinery, and it is usable on its own anywhere teardown is currently a hand-written `Dispose` chain. Nested scopes are where it starts paying:

```csharp
public EffectScope CreateChild()
{
    ThrowIfRecovered();
    var child = new EffectScope();
    // The child's existence is one inverse of this scope, ordered at creation.
    _undo.AddLast(() => child.RecoverAsync());
    return child;
}
```

Those four lines are the whole child-manager pattern. A parent that adds children through its own context records each child's existence as a tracked change of its own activation. Unloading the parent therefore retires the children first, one level at a time, with no unload code anywhere in the parent. The demo prints `w2`, then `w1`, then the parent, from a parent class that contains no teardown logic at all. I have written that manager by hand three times, and each version eventually leaked a child.

Two smaller rules from the same design are worth stealing even without the rest:

**Teardown has two phases.** Dependents stop first, while the provider's resource is still open, and only then does the provider's own accumulator run. A consumer closing a connection pool needs the pool to still exist while it closes. That class of bug disappears structurally rather than through care, and a test asserts it by reading a dependency during the dependent's own teardown.

**Reactivation keys on provider identity, not value.** Each binding gets a `uid` that is never reused. Swap in a provider with an equal value and every dependent still deactivates and reactivates, because what changed is who is behind the key, not what the key holds. This also means a component reads the snapshot it committed to, so it never observes a binding halfway through being replaced.

## Parking is a state, not an error

A component whose declared keys are not all bound sits at `Inactive` and waits. Adding it returns normally. There is no null to guard, no retry loop, no ordering constraint on registration: the gap between "component added" and "its dependency exists" is a legal state of the system.

Three outcomes get confused in most plugin systems, and this design separates them:

| Situation | Outcome |
|---|---|
| A declared key has no provider | `Inactive`, waiting |
| The component reads a key it never declared | `Failed`, and it is a bug |
| The activation throws | `Failed`, error recorded on the fiber, the call that triggered it still succeeds |

The last row matters more than it looks. A failing plugin records its exception and the host keeps running. Since the classifier only runs when the registry changes, the failed fiber does not retry against an unchanged environment, so a broken plugin cannot spin.

The catch: parking changes how people debug. "Why is nothing happening" now has a legitimate answer that is not an exception in the log, so `StateOf` and a registry view are not optional extras. They are the debugging interface.

## The inverse is unchecked, so I added the check

Every guarantee in the paper rests on an obligation it never enforces: the inverse you hand over must actually reverse your change. The metatheory assumes it. The runtime cannot see it. This is the one place where .NET is plainly better off, because a test can hold the state before and after and compare:

```csharp
var harness = new VerificationHarness(host);
harness.Watch("cache instances alive", () => MemoryCache.AliveInstances);
var before = harness.Capture();

await host.DisposeAsync();

harness.Verify(before);   // throws once, listing every difference
```

The registry is always watched, because the host owns it. Everything else you name yourself, and that is the useful part: a probe you forgot to write is state the harness will not defend, so the tracking boundary becomes visible instead of implied. A sloppy inverse fails a test instead of leaking quietly until production restarts.

I would take this piece into a codebase that has none of the rest. It is a handful of lines around a snapshot and an equality check, and it catches exactly the hand-written cleanup nobody reviews.

## The guarantee stops at the process edge

{::nomarkdown}
<figure>
<svg xmlns="http://www.w3.org/2000/svg" viewBox="0 0 940 420" role="img" style="width:100%;height:auto" font-family="-apple-system,BlinkMacSystemFont,'Segoe UI',Roboto,Helvetica,Arial,sans-serif">
  <title>What the accumulator can undo and what crosses the boundary</title>
  <desc>Inside one process, every recorded change carries an inverse and the accumulator runs them in reverse order: a cache instance, a child component, a provided binding. Three things cross a boundary the runtime cannot reverse: an HTTP request already sent, a callback registered on a long-lived static, a type loaded in an assembly load context. Reactivation of dependents keys on provider identity, so a swapped provider with an equal value still re-registers everyone.</desc>
  <defs>
    <pattern id="dots" width="22" height="22" patternUnits="userSpaceOnUse">
      <circle cx="1" cy="1" r="0.9" fill="#E3E2DC"/>
    </pattern>
  </defs>
  <rect width="940" height="420" fill="#f5f4ed"/>
  <rect width="940" height="420" fill="url(#dots)" opacity="0.55"/>

  <text x="80" y="42" fill="#1B365D" font-size="14" font-weight="600" font-family="'JetBrains Mono','SF Mono','Fira Code',Consolas,Monaco,monospace" letter-spacing="3">FIGURE  1</text>
  <text x="208" y="42" fill="#504e49" font-size="14" font-family="'JetBrains Mono','SF Mono','Fira Code',Consolas,Monaco,monospace" letter-spacing="3">WHAT THE ACCUMULATOR CAN UNDO</text>
  <line x1="80" y1="54" x2="760" y2="54" stroke="#1B365D" stroke-width="0.8"/>

  <rect x="80" y="88" width="480" height="270" fill="#faf9f5" stroke="#1B365D" stroke-width="1"/>
  <text x="100" y="114" fill="#1B365D" font-size="11" font-family="'JetBrains Mono','SF Mono','Fira Code',Consolas,Monaco,monospace" letter-spacing="2">ONE PROCESS, ONE ACCUMULATOR</text>

  <rect x="210" y="140" width="260" height="34" fill="#f5f4ed" stroke="#141413" stroke-width="1" rx="2"/>
  <text x="224" y="161" fill="#141413" font-size="13">cache instance created</text>

  <rect x="210" y="200" width="260" height="34" fill="#f5f4ed" stroke="#141413" stroke-width="1" rx="2"/>
  <text x="224" y="221" fill="#141413" font-size="13">child component added</text>

  <rect x="210" y="260" width="260" height="34" fill="#f5f4ed" stroke="#141413" stroke-width="1" rx="2"/>
  <text x="224" y="281" fill="#141413" font-size="13">binding provided</text>

  <line x1="185" y1="277" x2="185" y2="157" stroke="#1B365D" stroke-width="1"/>
  <line x1="185" y1="157" x2="210" y2="157" stroke="#1B365D" stroke-width="1"/>
  <line x1="185" y1="217" x2="210" y2="217" stroke="#1B365D" stroke-width="1"/>
  <line x1="185" y1="277" x2="210" y2="277" stroke="#1B365D" stroke-width="1"/>
  <path d="M 197 150 L 210 157 L 197 164" fill="none" stroke="#1B365D" stroke-width="1"/>

  <text x="100" y="330" fill="#504e49" font-size="10" font-family="'JetBrains Mono','SF Mono','Fira Code',Consolas,Monaco,monospace" letter-spacing="2">INVERSES RUN IN REVERSE ORDER</text>

  <line x1="600" y1="88" x2="600" y2="358" stroke="#6b6a64" stroke-width="1" stroke-dasharray="6 6"/>
  <text x="600" y="384" fill="#6b6a64" font-size="10" text-anchor="middle" font-family="'JetBrains Mono','SF Mono','Fira Code',Consolas,Monaco,monospace" letter-spacing="2">THE BOUNDARY, NO INVERSE</text>

  <line x1="470" y1="157" x2="650" y2="157" stroke="#504e49" stroke-width="1"/>
  <path d="M 641 150 L 650 157 L 641 164" fill="none" stroke="#504e49" stroke-width="1"/>
  <line x1="470" y1="217" x2="650" y2="217" stroke="#504e49" stroke-width="1"/>
  <path d="M 641 210 L 650 217 L 641 224" fill="none" stroke="#504e49" stroke-width="1"/>
  <line x1="470" y1="277" x2="650" y2="277" stroke="#504e49" stroke-width="1"/>
  <path d="M 641 270 L 650 277 L 641 284" fill="none" stroke="#504e49" stroke-width="1"/>

  <rect x="650" y="140" width="230" height="34" fill="#f5f4ed" stroke="#6b6a64" stroke-width="1" stroke-dasharray="5 5" rx="2"/>
  <text x="664" y="161" fill="#6b6a64" font-size="13">HTTP request sent</text>

  <rect x="650" y="200" width="230" height="34" fill="#f5f4ed" stroke="#6b6a64" stroke-width="1" stroke-dasharray="5 5" rx="2"/>
  <text x="664" y="221" fill="#6b6a64" font-size="13">timer on a static</text>

  <rect x="650" y="260" width="230" height="34" fill="#f5f4ed" stroke="#6b6a64" stroke-width="1" stroke-dasharray="5 5" rx="2"/>
  <text x="664" y="281" fill="#6b6a64" font-size="13">type in an ALC</text>
</svg>
</figure>
{:/nomarkdown}

What the figure is really saying is that the design buys reversibility, not disposability, and only inside the process. A request that has already left cannot be recalled. A timer callback registered on a long-lived static object keeps the thing it captured alive, which matters twice over in .NET: once for correctness, and once because it will pin a collectible assembly context and quietly prevent unloading.

Nothing is tracked implicitly, and that is a design decision rather than a gap. `DateTime.UtcNow`, `HttpClient` defaults, configuration statics, file writes, cache entries behind an external service: all of it acts as identity, and the author's undo is the only thing that reverses it. The practical consequence is that a .NET port tracks **less** than a reader of the paper would assume, and the harness's probe list is the honest inventory of what it tracks.

Two limits worth stating plainly, because they are easy to discover the hard way. First, **cycles become permanent inactivity**: two components that need each other's keys never activate, and decomposing them into an integration component grows quadratically. Second, **one provider per key**, with a broker as the only path to multiplicity, and a broker is where a local function call turns back into networking.

## Code unloading is the one piece that fights the CLR

Node hot replacement has a trick that makes the paper's reload path look easy: delete the entry from `require.cache`, and the next import re-evaluates the file while the process, its state and its other modules stay alive. .NET has no module cache to evict. An assembly stays bound to its load context, and the only eviction is unloading a whole collectible `AssemblyLoadContext`.

That path is real, and it is hostile in four ways:

- **Unload is best effort and silent.** `Unload()` returns immediately; the context dies at some later GC that finds no references into it. Failure looks exactly like success, so "unloaded" has to be an observed fact: keep a `WeakReference`, force collections, assert it is dead.
- **Almost anything pins it.** Static fields, thread statics, cached delegates, timer callbacks, unresolved continuations, event handlers on long-lived objects. A fiber's committed view holds references into the context, which here means references into the unloadable assembly.
- **Type identity splits.** Two collectible contexts loading the same assembly produce distinct `Type` objects, so a registry keyed on `Type` cannot see across them. The standard fix is a contract assembly in the default context, which gives back the nominal-typing advantage in exchange for the isolation.
- **The DLL file is locked while loaded.** Replacing code means loading a shadow copy of the binaries. Boring plumbing, and it still has to be built.

`dotnet watch` does not close the gap. It patches method bodies through metadata updates: no new types, no re-run of static constructors, and any cache you own must be cleared by hand from a `MetadataUpdateHandler`.

So the verdict after building M1 to M5 is the one I would act on: **unload effects, not code**. Event handlers, timers, subscriptions, hosted services, cache entries, connection pools: that is where the pain is, and that part is ordinary engineering. If genuine code eviction is ever needed, isolate it in a process and choose the coarse mechanism deliberately at the boundary.

## The test I would run before adopting any of it

The adoption question is economic and the paper never states it:

```
removal frequency  x  state-rebuild cost  >  cost of maintaining the discipline
```

Components that leave monthly on a system that restarts in ten seconds do not need any of this. A restart is correct there, and track-everything is pure tax. The mechanism pays where components arrive and leave while the process keeps running: an **agent harness** that installs and removes tools mid-session and cannot afford a process restart per change, a long-lived **plugin host** like Koishi, per-tenant modules where one tenant's change must not become everyone's outage.

The parts I would take without the rest are the four that survived scrutiny: the change-plus-inverse primitive, the reactive registry keyed on provider identity, two-phase teardown, and the verification harness. The parts I would skip are the calculus, the realm machinery and the loader, none of which a real use case has asked for yet.

What is still unproven is the code side. One time-boxed session is enough to settle it: load a plugin into a collectible `AssemblyLoadContext`, unload it, and watch the weak reference die. If that works, the remaining cost is proxy types and contract assemblies. If it does not, the answer is a restart at the process boundary, and the rest of the design still stands on its own.

[arXiv 2608.25512: A Programming Paradigm for Spatiotemporal Composability](https://arxiv.org/abs/2608.25512) · [Cordis, the TypeScript reference implementation](https://github.com/cordiverse/cordis)
