---
layout: post
title: "Architecture v2 — ASP.NET Core Feature-Based"
date: 2026-07-10
tags: [dotnet, minimal-api, feature-based, cqrs, architecture]
categories: programming
---

## 1. Layers

| Layer | Purpose | Depends on |
|-------|---------|------------|
| Endpoints / Controllers | HTTP binding, auth, execute | Features, Shared |
| Features | All business logic | Features (barrel only), Shared |
| Shared | Abstractions, infrastructure | Shared only |

Foundation features depend only on Shared. Domain features depend on Shared + foundation features. Domain → domain forbidden.

---

## 2. Project Structure

```
src/
├── Program.cs
│
├── Features/
│   └── <Name>/
│       ├── CreateX.cs        # command record + handler
│       ├── UpdateX.cs        # command record + handler
│       ├── GetX.cs           # query record + handler
│       ├── Contracts.cs      # DTOs, enums
│       ├── Endpoints.cs      # MapXEndpoints(WebApplication) — preferred
│       └── Controller.cs     # fallback when Minimal API is not viable
│
├── Shared/
│   ├── Abstractions/         # ICommand, IQuery, IValidator<T>
│   ├── Persistence/          # DbContext, migrations
│   ├── Telemetry/            # OpenTelemetry wiring
│   ├── Resilience/           # Polly pipelines, standard handlers
│   └── GlobalUsings.cs
```

---

## 3. Feature Types

| Type | Purpose | Depends on | Example |
|------|---------|------------|---------|
| Foundation | Data models, core CRUD | Shared only | Users, Orders, Products |
| Domain | Business processes | Shared + foundation | Checkout, Onboarding, Dashboard |

Extract to foundation when 2+ domain features need the same logic.

---

## 4. Commands & Queries

| | Returns | Mutates | Failure |
|--|---------|---------|---------|
| Command | `Task` | ✅ | Throws domain exception |
| Query | `Task<T?>` | ❌ | Returns `null` for not-found; throws for unexpected errors |

One HTTP request = one command or one query. Aggregation queries allowed (multiple reads, single response). Never aggregate commands.

Command record and handler in one file. Query record and handler in one file.

Commands throw on failure. Queries return nullable — `null` means "not found." All exceptions flow to the global error middleware. One error model. One conversion point.

---

## 5. Endpoints & Controllers

Transport-only. Bind HTTP → command/query → execute handler directly.

Prefer Minimal API (`Endpoints.cs`). Fall back to a controller when Minimal API is not viable (e.g., complex model binding, MVC filters, or legacy constraints). Both follow the same rules.

**Must:** bind HTTP, enforce auth/authz, call handler directly, map outcome → HTTP response.

**Must NOT:** business logic, call repositories, compose multiple commands.

Mapping rules at the endpoint:
- Command throws → middleware converts to ProblemDetails
- Query returns `null` → 404
- Query throws → middleware converts to ProblemDetails
- Everything else → 200 with body

Feature endpoints via extension method groups: `app.MapOrderEndpoints()`. Controllers via `[ApiController]` + constructor-injected handlers.

---

## 6. Cross-Cutting Concerns

Handlers own their cross-cutting concerns explicitly. No hidden pipeline. Every handler must:

1. **Validate** — call FluentValidation validator, throw `ValidationException` on failure
2. **Trace** — create an OpenTelemetry span named `{Feature}.{Operation}` with meaningful attributes
3. **Log** — log request start, success/failure, and duration
4. **Meter** — increment success/failure counters and record duration histogram

Consistency is enforced by the spec, not by runtime infrastructure. Agents regenerate all handlers when cross-cutting rules change.

Order: Validate → Trace → Log/Meter (start) → Business logic → Log/Meter (end).

---

## 7. Validation

FluentValidation. `AbstractValidator<TRequest>`. Stateless. Return `ValidationResult`, never throw.

Each handler calls its validator explicitly as the first step. After validation passes, business logic assumes valid input — no re-validation scattered throughout the handler body.

---

## 8. Error Handling

**One error model. One translation point.**

### Domain Exceptions

Base class `DomainException` for all expected business failures. Carries an `ErrorCode` string for API consumers. Named subtypes for common cases.

```
DomainException (base, maps to 422)
├── ValidationException       → 400
├── UnauthorizedException     → 403
├── NotFoundException         → 404
├── ConflictException         → 409
└── CustomDomainException     → 422 (generic)
```

### Middleware

Global exception middleware catches everything and converts to RFC 7807 Problem Details.

- `DomainException` → 4xx (client error, expected)
- Everything else → 500 (server error, unexpected)

Middleware extracts `ErrorCode` from `DomainException` into ProblemDetails `detail` field. No raw stack traces to clients.

Handlers throw domain exceptions freely. Endpoints never catch — let them bubble to the middleware.

Query handlers return `null` for not-found (no throw). Command handlers throw for failures.

---

## 9. Security

**Fallback policy** — authenticated by default. Opt out explicitly.

```csharp
options.FallbackPolicy = new AuthorizationPolicyBuilder()
    .RequireAuthenticatedUser()
    .Build();
```

`[AllowAnonymous]` only where justified and reviewed.

- JWT Bearer tokens
- Role-based authorization
- CORS restricted to production domains
- Rate limiting (built-in `AddRateLimiter` / `UseRateLimiter`)
- Security headers via `NetEscapades.AspNetCore.SecurityHeaders`

Middleware order: CORS → Auth → Authorization → Security Headers → Rate Limiting → Endpoint → Handler.

---

## 10. Resilience

Infrastructure-level. `Microsoft.Extensions.Resilience` (Polly v8). Standard resilience handlers on all `HttpClient`, storage, and external adapters.

Apply globally to all HTTP clients by default:

```csharp
builder.Services.ConfigureHttpClientDefaults(builder =>
    builder.AddStandardResilienceHandler());
```

No retry logic in handlers. No `try/catch` for transient failures in business code. Exhausted retries throw — caught by global error middleware → Problem Details.

---

## 11. Dependency Injection

| Component | Lifetime |
|-----------|----------|
| Handlers | Scoped |
| Validators | Scoped |
| Options classes | Singleton (direct, not `IOptions<T>`) |
| Infrastructure (Shared) | Singleton |

Infrastructure services must be stateless.

### Options Registration

Register the options class directly — consumers inject `MyOptions`, not `IOptions<MyOptions>`.

```csharp
builder.Services.Configure<MyOptions>(builder.Configuration.GetSection("MyOptions"));
builder.Services.AddSingleton(sp => sp.GetRequiredService<IOptions<MyOptions>>().Value);
```

---

## 12. Engineering Practices

Enforced at project level. No per-file overrides.

| Rule | Mechanism |
|------|-----------|
| Treat warnings as errors | `<TreatWarningsAsErrors>true</TreatWarningsAsErrors>` |
| Nullable enabled | `<Nullable>enable</Nullable>` — no `#nullable disable` |
| Implicit usings | `<ImplicitUsings>enable</ImplicitUsings>` |
| File-scoped namespaces | `namespace Foo.Bar;` only |
| Primary constructors | For DI dependencies |
| Happy path at bottom | Early returns for guard clauses; main logic last |
| Code analysis | `<AnalysisMode>Recommended</AnalysisMode>` — CA warnings are errors |
| `.editorconfig` | Repo root. No style debates. |
| Global usings | `Shared/GlobalUsings.cs` |

### Code Smells

Soft signals — not enforced, but triggers for refactoring.

- Methods > 20 lines
- Classes > 200 lines
- `#region` — extract a new class or method instead

---

## 13. Hard Rules

1. One HTTP request = one command or one query.
2. No business logic in Program.cs.
3. Endpoints call handlers directly. No dispatcher or mediator.
4. Commands throw on failure. Queries return `T?` — null means not found.
5. Every handler explicitly validates, traces, logs, and meters.
6. All errors → global middleware → Problem Details. One error model, one conversion point.
7. All endpoints authenticated. Fallback policy enforces this.
8. Resilience mandatory and centralized. No ad-hoc retries.
9. Foundation → Shared only. Domain → Shared + foundation. Domain → domain forbidden.
10. New code with warnings does not compile.

---

## 14. Mental Model

```
HTTP Request
  └─ Middleware (CORS → Auth → Authorization → Security Headers → Rate Limiting)
      └─ Endpoint (binds HTTP → command/query)
          └─ Handler
              ├─ Validate
              ├─ Trace (span: Feature.Operation)
              ├─ Log / Meter (start)
              ├─ Domain logic
              ├─ Shared services (resilient)
              ├─ Log / Meter (end)
              └─ Exception (if failure)
                  └─ Global Error Middleware
                      └─ ProblemDetails
```
