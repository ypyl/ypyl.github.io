---
layout: post
title: "ApplicationInsights-JS 3.4 Adds OpenTelemetry Spans — Watch the Parent Context"
date: 2026-05-18
categories: programming
tags: [application-insights, opentelemetry, javascript, observability]
---

**ApplicationInsights-JS v3.4.0** now supports **OpenTelemetry spans** natively, allowing you to create and manage custom spans inside your JavaScript code via `appInsights.startSpan()` and the `otelApi` tracer.

The catch: manually created spans don't automatically chain to existing spans like page views. Calling `appInsights.trackPageView()` creates a page view record, but a subsequent `appInsights.startSpan("my-span")` won't be its child — the span will appear orphaned with no parent.

To fix this, retrieve the current trace context via `appInsights.getTraceCtx()` and pass it as the parent when starting the span:

```typescript
function startSpanWithPageViewParent(name: string) {
  const pageViewCtx = appInsights.getTraceCtx();
  const span = pageViewCtx
    ? appInsights.startSpan(name, undefined, {
        ...pageViewCtx,
        spanId:
          pageViewCtx.spanId != null && pageViewCtx.spanId !== ""
            ? pageViewCtx.spanId
            : pageViewCtx.traceId,
      })
    : appInsights.startSpan(name);

  if (!span) return;
  const scope = appInsights.setActiveSpan(span);
  return { span, scope };
}
```

The `spanId` fallback to `traceId` is necessary when `spanId` is empty — the page view span ID may not always be populated. Without this explicit parent context, your custom spans won't show up in the end-to-end transaction view under the correct page view.

[ApplicationInsights-JS repo](https://github.com/microsoft/ApplicationInsights-JS) · [OTel Examples](https://microsoft.github.io/ApplicationInsights-JS/OTel/examples) · [OTel API Reference](https://microsoft.github.io/ApplicationInsights-JS/OTel/otelApi)
