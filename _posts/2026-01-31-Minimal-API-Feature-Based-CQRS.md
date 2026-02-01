---
layout: post
title: "Architecture Instructions – ASP.NET Core Minimal API (Feature-Based CQRS)"
date: 2026-01-31
tags: dotnet minimal api asp net core feature-based cqrs architecture
categories: programming backend spa
---

# Architecture Instructions – ASP.NET Core Minimal API (Feature-Based CQRS)

## 1. Architectural Style

* Use **feature-based architecture**.
* Use **CQRS**:
  * **Commands** → state-changing
  * **Queries** → read-only
* **One HTTP request → exactly one command or one query**.
* ASP.NET Core **Minimal API**.
* **All HTTP endpoints are declared in `Program.cs`**.
* SPA `index.html` may be served by the same application.

---

## 2. Project Structure
> One-line rule: Group by feature, not by technical layer; keep shared code feature-agnostic.

```
/src
 ├── Program.cs
 │
 ├── Features
 │    ├── <FeatureName>
 │    │    ├── <UseCaseName>
 │    │    │    ├── Command.cs | Query.cs
 │    │    │    ├── Handler.cs
 │    │    │    └── Validator.cs (optional)
 │    │    └── Contracts.cs
 │
 ├── Workflows/
 │    ├── <WorkflowName>/
 │    │    ├── Workflow.cs
 │    │    ├── State.cs
 │    │    └── Policies.cs
 │
 ├── Infrastructure
 │    ├── Cosmos/
 │    ├── BlobStorage/
 │    ├── Telemetry/
 │    ├── Security/
 │    └── Resilience/
 │
 └── Shared
      ├── Abstractions/
      ├── Results/
      ├── Errors/
      ├── Behaviors/
      ├── Domain/
      ├── Validators/
      ├── Mappers/
```

---

## 3. Endpoint Rules (Program.cs)
> One-line rule: Endpoints dispatch to handlers; no business logic in routing layer.

**Endpoints are transport-only. They are not features.**

### Mandatory Security Rules

* **ALL endpoints require authentication**
* **NO anonymous endpoints are allowed**
* Authorization is **role-based**
* Missing or insufficient roles must result in **401 / 403**

### Endpoint Responsibilities

Endpoints must:

* Bind HTTP → command/query
* Enforce authentication & authorization
* Call `IDispatcher.Send(...)`
* Map `Result` / `Result<T>` → HTTP response
* Trigger workflows only via a single "Start" command (e.g., `StartOnboarding`) — do not orchestrate multiple commands from an endpoint

Endpoints must NOT:

* Contain business logic
* Call repositories or infrastructure
* Call handlers directly
* Compose multiple commands or queries
* Orchestrate workflows or publish multiple commands (start workflows only via a single trigger command)

---

## 4. Commands & Queries
> One-line rule: Commands change state and return void; queries return data and never mutate.

### Commands

* Represent intent to change state.
* Handler may return:
  * `Task` → treated as success
  * `Task<Result>` → explicit success/failure

### Queries

* Represent read-only operations.
* Handler **must return** `Task<Result<T>>`.

---

## 5. Aggregation Handlers
> One-line rule: Aggregate queries only; never combine commands or add side effects.

### Constraints

* **Queries only** - never aggregate commands
* **Read-only** - no state changes or side effects
* **Parallel execution** - dependencies must be independent
* **Fail fast** - any failure fails entire request
* Handler **must return** `Task<Result<T>>`

### Usage

* Combine multiple reads into single response
* Minimize network round-trips
* Independent data sources only

### Restrictions

* No commands
* No complex business logic
* No high fan-out scenarios
---

## 6. Dispatcher
> One-line rule: Single entry point for external commands; workflows bypass for internal steps.

* Dispatcher is the **only entry point** to application logic for external/public commands.
* Dispatcher:
  * Resolves handler via DI
  * Executes pipeline behaviors (validation, authorization, tracing, logging, metrics)
  * Wraps `Task` handlers into `Result.Success()`
* Endpoints **never** invoke handlers directly.
* **Important:** Dispatcher **SHOULD ONLY** be used for commands that cross a system boundary: HTTP-triggered commands, event-triggered commands, and workflow-start commands. **Pipeline behaviors apply to those commands.**
* **Workflows MUST NOT** use the dispatcher to execute internal workflow steps. Internal workflow steps should be executed via a workflow executor (see `IWorkflowExecutor`) or run directly as `IWorkflowStep<TState>` implementations so that pipeline behaviors are not re-applied.

---

## 7. Pipeline Behaviors
> One-line rule: Cross-cutting concerns live in behaviors; handlers stay focused on business logic.

Behaviors are **cross-cutting and generic**.

Typical behaviors:

* Validation
* Logging
* Tracing (OpenTelemetry)
* Metrics

Rules:

* Behaviors **never contain business logic**
* Behaviors **never return or create `Result`**
* Behaviors may **throw exceptions**
* Behaviors **do not translate errors to HTTP**
* **Scope rule:** Pipeline behaviors **apply only** when a command is sent through the `IDispatcher` (external/public commands and workflow-start commands). Pipeline behaviors **MUST NOT** apply to internal workflow step execution, compensation steps, or retry executions inside a workflow.

---

## 8. Security
> One-line rule: Authenticate by default, authorize explicitly; secure endpoints from the start.

**All endpoints require authentication. No exceptions.**

### Authentication
JWT Bearer tokens required for all requests.

### Authorization
Role-based access control:
- Admin role for administrative operations
- User role for standard operations

### Rate Limiting
Use Microsoft.AspNetCore.RateLimiting to configure rate limiting policies and attach them to endpoints.

### CORS
Restricted to production domains only.

### Input Validation
- SQL injection prevention via parameterized queries
- Path traversal validation for file operations
- Command injection sanitization for external processes
- Business rule validation in pipeline

### Security Pipeline Order
1. CORS (middleware)
2. Authentication (middleware)
3. Authorization (middleware)
4. Rate Limiting (middleware)
5. Validation (behavior)
6. Handler Execution

---

## 9. Validation
> One-line rule: Validate early via pipeline behaviors; handlers receive pre-validated requests.

* Validators implement `IValidator<TRequest>`
* Validators:
  * Are stateless
  * Do not throw
  * Return structured validation errors
* `ValidationBehavior`:
  * Executes all validators
  * Throws `ValidationException` on failure
  * Does not return `Result`

---

## 10. Error Handling & Problem Details
> One-line rule: Convert all failures to Problem Details; maintain consistent error contracts.

* **ALL errors must be converted to RFC 7807 Problem Details**
* No exception, validation failure, or business error may leak raw details to the client

### Error Sources Covered

* Validation failures
* Authorization failures
* Domain/business errors
* Infrastructure failures
* Unhandled exceptions

### Enforcement Rules

* A **global exception-handling middleware** is mandatory
* Middleware responsibilities:
  * Catch **all exceptions**
  * Convert them to **ProblemDetails**
  * Set correct HTTP status codes
  * Emit standardized error payloads
  * Log errors internally (with correlation IDs)

### Handlers

* Handlers:
  * May return `Result.Failure(...)`
  * May throw **domain-specific exceptions**
* Handlers **never**:
  * Create `ProblemDetails`
  * Set HTTP status codes
  * Depend on ASP.NET types

---

## 11. Migration and Versioning
> One-line rule: Version APIs explicitly; maintain backward compatibility within major versions.

### API Versioning

**URI Versioning** for explicit version control.

**Versioning Rules:**
* Major version (v1, v2) for breaking changes
* Minor changes within same major version (backward compatible)
* Minimum 6-month deprecation period
* Version headers for monitoring

### Database Migrations

**Schema Changes:**
* All changes through migration system
* Additive changes: new columns, tables, indexes
* Non-breaking: default values, nullable columns
* Breaking changes require new API version
* Data migrations separate from schema migrations

### Backward Compatibility

**API Compatibility:**
* Maintain existing contracts within major versions
* Optional fields for extensions
* Breaking changes trigger new major version

**Database Compatibility:**
* Expand-Contract pattern for schema changes
* View-based compatibility for legacy versions
* Feature flags for gradual transitions

### Feature Rollout

**Progressive Deployment:**
* Feature toggles for runtime control
* Rollback capability
* Usage monitoring

---

## 12. Feature Flags
> One-line rule: New features start disabled; enable gradually with rollback capability.

**All new features must use feature flags for safe rollout.**

### Constraints

* New features start disabled
* Enable gradually (dev → staging → prod)
* Never use flags for core business logic
* Feature flags must not affect system architecture

---

## 13. Result Model (Monad)
> One-line rule: Wrap all handler outcomes in Result<T>; never throw exceptions for business failures.

* Handlers return:
  * `Result`
  * `Result<T>`
* `Result` represents:
  * Success
  * Failure with error codes and messages
* `Result` is translated to **ProblemDetails or success response** at the HTTP boundary only (`Program.cs` / middleware).

---

## 14. Resilience
> One-line rule: Centralize retry policies in infrastructure; handlers remain resilience-agnostic.

The application **must be resilient by default**. Transient failures are expected, not exceptional.

### Core Principles

* Assume **network, storage, and external services will fail**
* Failures must be:

  * Retried when safe
  * Timed out deterministically
  * Isolated (no cascading failures)
* Resilience is **infrastructure-level**, not business logic

### Required Practices

* Use **Microsoft.Extensions.Resilience** (Polly v8 stack)
* Configure **standard resilience pipelines** centrally
* Prefer **`AddStandardResilienceHandler()`** as the baseline

### Where Resilience Lives

* All resilience configuration is placed in:

  * `Infrastructure/Resilience`
  * Wired in `Program.cs`
* Handlers and features:

  * **Never configure retries**
  * **Never configure timeouts**
  * **Never reference Polly directly**

### What Must Be Protected

Resilience handlers must be applied to:

* HTTP clients (`HttpClient`)
* Storage clients (Cosmos DB, Blob Storage)
* External service adapters
* Message brokers

### Rules

* No retry logic in handlers
* No `try/catch` for transient failures in business code
* No silent swallowing of failures
* All exhausted retries must surface as errors and flow into **Problem Details**

Long-running workflows must be durable and rely on infrastructure-level resilience. In-memory workflows are forbidden beyond trivial cases.

---

## 15. Workflows & Execution Paths

> **One-line rule:** Commands that cross a system boundary go through pipeline behaviors; workflow-internal steps do not.

Summary

* Keep workflows explicit and durable; avoid implicit middleware.

Kinds of commands

* **External / Public** (e.g., `CreateUser`, `StartUserOnboarding`)
  * Enter via HTTP/messaging, cross trust boundaries
  * Require validation, authorization, telemetry, auditing, idempotency
  * **Pipeline behaviors APPLY**

* **Internal Workflow Steps** (e.g., `AssignDefaultRole`, `ProvisionUserStorage`)
  * Executed inside workflows; already validated/authorized
  * Owned by the workflow (tracing, idempotency, retries)
  * **Pipeline behaviors MUST NOT APPLY**

Execution paths (explicit)

* `IDispatcher.Send<TCommand>(...)` — behaviors ON
* `IWorkflowExecutor.Execute<TStep>(...)` — behaviors OFF

Workflow steps

* Implement `IWorkflowStep<TState>` with `ExecuteAsync` and `CompensateAsync`
* No `Result`, validation, or auth on step methods; failures bubble to the workflow engine
* Workflow logic handles retries/compensation and publishes durable status (Started/InProgress/Succeeded/Failed/Compensated)
* **Step failures are captured by the workflow engine and exposed via status endpoints, which convert them to Problem Details per Section 10**

Result contract

* Endpoints that start workflows return `202 Accepted` and a status-check URI (e.g., `GET /workflows/{id}/status`)
* Provide a query endpoint that returns `Result<WorkflowStatus>` or ProblemDetails

What still applies (explicitly)

* Resilience (infrastructure-level), tracing (child spans), idempotency, and error mapping — all workflow-owned and explicit.

---

## 16. Dependency Injection
> One-line rule: Scope components by request lifecycle; maintain proper dependency lifetimes.

### Lifetime Requirements

| Component | Lifetime | Constraint |
|-----------|----------|------------|
| **Handlers** | `Scoped` | Must share request context |
| **Validators** | `Scoped` | Must access scoped dependencies |
| **Dispatcher** | `Scoped` | Must coordinate request-scoped handlers |
| **Workflows** | `Scoped` | Must maintain state per request |
| **Infrastructure** | `Singleton` | Must be stateless |
| **Repositories** | `Scoped` | Must share database context |

---

## 17. Database with Specification Pattern
> One-line rule: Encapsulate query logic in composable specifications; keep EF Core queries direct and testable.

**Summary**

Use specifications to build reusable, composable query logic without repository abstractions.

**Entity structure**

- BaseEntity with Id, CreatedAt, IsDeleted
- Soft delete enforced via global query filters
- Entities inherit common audit properties

**Specification contracts**

- `ISpecification<T>` exposes `Expression<Func<T, bool>>` Criteria
- Abstract `Specification<T>` provides And/Or composition methods
- `AndSpecification`/`OrSpecification` handle expression tree merging
- `ParameterReplacer` ensures consistent lambda parameters

**DbContext integration**

- Global query filters for soft delete (HasQueryFilter)
- Extension method `ApplySpecification<T>` applies criteria to DbSet
- Direct EF Core queries with specification filtering

**Specification implementation**

- One class per business rule (`UserByIdSpecification`, `UserByEmailSpecification`)
- Constructor accepts filter parameters
- Criteria property returns lambda expression
- Composable via And/Or methods

**Handler usage**

- Start with base specification or empty criteria
- Chain specifications using And/Or based on request parameters
- Apply to DbContext via `ApplySpecification` extension
- Project to DTOs in the same query

---

## 18. Shared Code Rules
> One-line rule: Shared code stays feature-agnostic; handlers orchestrate shared services.

Shared code **never lives inside Features**.

### `Shared` Contains

* CQRS abstractions (`ICommand`, `IQuery`, Dispatcher contracts)
* Result and error models
* Pipeline behaviors
* Domain services (pure business logic)
* Shared validators
* Mappers (entity → DTO)
* Utilities and constants

### `Infrastructure` Contains

* Cosmos DB repositories
* Blob storage adapters
* External API clients
* Telemetry configuration
* Security helpers (auth, CORS, policies)
* **Resilience pipelines**

Rules:

* Shared code must be **feature-agnostic**
* Shared code must not depend on `Features/*`
* Handlers orchestrate shared services

---

## 19. Observability
> One-line rule: Telemetry lives in pipeline behaviors; handlers remain instrumentation-free.

* Tracing, logging, metrics are implemented via **pipeline behaviors**
* Handlers **never**:

  * Create spans
  * Log cross-cutting concerns
* Telemetry wiring lives in `Infrastructure/Telemetry`

---

## 20. Deployment & Configuration

> One-line rule: Fail fast on startup; validate everything before accepting traffic.

**Summary**

Validate configuration and dependencies at startup; use health checks for runtime monitoring.

**Configuration Structure**

* Base settings in `appsettings.json` (connection strings, rate limits)
* Environment overrides in `appsettings.{Environment}.json`
* Secrets sourced from Key Vault only, never in config files
* Required keys: `ConnectionStrings:CosmosDb`, `ConnectionStrings:BlobStorage`

**Health Checks**

* Implement `IHealthCheck` for each external dependency
* Check actual connectivity (e.g., Cosmos `ReadAccountAsync`)
* Return Healthy/Unhealthy with descriptive messages
* Expose via /health endpoint

**Startup Validation**

* Validate required configuration keys exist before `app.Run()`
* Test connectivity to all external dependencies during startup
* Throw exceptions for missing config or unreachable services
* Fail fast principle: better to crash than run degraded

**Deployment Requirements**

* Configuration validation at startup
* Health checks before accepting traffic
* Secrets from Key Vault only
* 30-second graceful shutdown timeout
* Zero-downtime deployments via health check integration

---

## 21. Monitoring

> One-line rule: Metrics collection happens only in pipeline behaviors; handlers remain telemetry-free.

**Summary**

Keep telemetry concerns separated from business logic; collect metrics at the pipeline level.

**Metrics Collection Strategy**

**Pipeline-Level Collection**
- Metrics behaviors intercept all requests/responses
- Automatic timing, counting, and error tracking
- Feature and operation tagging from request type
- Success determination from Result pattern

**Handler-Level Collection (FORBIDDEN)**
- Handlers must not emit telemetry directly
- Business logic stays focused on domain concerns
- Metrics concerns handled by infrastructure

**Standard Metrics**

**Request Counters**
- Total requests per feature/operation
- Error counts with failure categorization
- Success/failure ratios

**Performance Histograms**
- Request duration in milliseconds
- Percentile distributions (P50, P95, P99)
- Performance trend analysis

**Implementation Requirements**

**MetricsCollector Service**
- Singleton registration with IMeterFactory
- Standard meter name: "API.CQRS"
- Counter and histogram creation
- TagList with feature/operation dimensions

**Pipeline Behavior**
- Generic IPipelineBehavior implementation
- Stopwatch timing around next() delegate
- Feature extraction from namespace
- Operation name from request type
- Exception handling for error metrics

**OpenTelemetry Integration**
- AddMeter configuration for custom metrics
- AspNetCore instrumentation for HTTP metrics
- Application Insights export configuration

**What applies explicitly**

Naming convention (feature.operation), pipeline-only collection, automatic tagging, and export to monitoring systems.

---

## 22. Non-Negotiable Rules

* ❌ No anonymous endpoints
* ❌ No business logic in `Program.cs`
* ❌ No handlers in endpoints
* ❌ No multiple commands per HTTP request
* ❌ No multi-command handlers
* ❌ No workflows inside Features
* ❌ No infrastructure access from endpoints
* ❌ No raw exceptions or errors returned to clients
* ❌ No ad-hoc retry / timeout logic
* ❌ No `AllowAnyOrigin()` in production CORS
* ❌ No server-side HTML encoding (client responsibility)
* ❌ No endpoints without rate limiting
* ✅ One use case = one handler
* ✅ One request = one command or query
* ✅ Aggregation handlers are allowed **only for queries**
* ❌ Never aggregate commands
* ❌ Never implement workflows in handlers
* ✅ Aggregation is composition, not orchestration
* ✅ Handlers orchestrate shared services
* ✅ Behaviors handle cross-cutting concerns
* ✅ All errors → Problem Details
* ✅ Resilience is mandatory and centralized
* ✅ Client handles HTML encoding and XSS prevention
* ✅ All endpoints authenticated by default

---

## 23. Mental Model
> One-line rule: Request flows through layers with clear separation; each layer has single responsibility.

```
HTTP (Program.cs, Authenticated, Rate Limited, CORS)
   └─> ASP.NET Core Middleware (CORS → Auth → Authorization → Rate Limiting)
       └─> Dispatcher
            → Behaviors (Validation → Logging → Tracing)
              ├─> Handler
              |     → Shared Domain
              |     → Infrastructure (Resilient)
              |        → Failure / Result / Exception
              |           → Global Error Middleware
              |              → ProblemDetails
              └─> StartWorkflowCommand
                    → Workflow (durable)
                        → Step A (direct handler call — behaviors OFF)
                        → Step B (direct handler call — behaviors OFF)
                        → Compensation if needed (workflow-controlled)
                        → Persisted state / Events / Domain events
```
