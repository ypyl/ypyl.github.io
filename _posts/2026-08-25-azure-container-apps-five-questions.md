---
layout: post
title: "Azure Container Apps: Five Questions with Non-Obvious Answers"
date: 2026-08-25
tags: [azure, azure-container-apps, serverless, keda, dapr, envoy]
categories: azure
---

Azure Container Apps (ACA) is a serverless platform for containerized applications: no servers, no orchestration, no cluster to manage. The best one-line framing I found in the docs is "managed Kubernetes semantics without the Kubernetes API." You get scaling, revisions, ingress, and service discovery, but you never touch a cluster. Under the hood it is Kubernetes plus three CNCF open-source projects: KEDA for scaling, Envoy for traffic, Dapr for service-to-service calls.

While reading the Azure Container Apps documentation, I logged every question that came up: 37 in all. This post covers the five where the answer contradicted my assumption. Each of those five started from a wrong guess, and I only caught it by asking follow-ups.

---

## 1. What is a revision, really?

My first question: during a deployment, does the platform run two containers side by side and wait for the new one to be healthy before killing the old? Close, but the mental model underneath was wrong.

**A revision is a spec snapshot, not compute.** It is an immutable record of the app template (image, environment variables, resources, probes, scale rules). The running instances are replicas, created from a revision's spec. Scaling only creates or destroys replicas; it never changes the revision. This one distinction explains deploys, rollback, and blue/green:

- **Rollback is reactivating a stored spec, not redeploying.** The platform keeps 100 inactive revisions by default, so the previous spec is always there.
- **Two changes, two scopes.** A change to the template creates a new revision. A change to configuration (secrets, ingress, Dapr) applies to all revisions at once.
- **Deploys are health-gated.** The new revision provisions while the old one keeps traffic; traffic diverts only when the new revision is healthy, then the old one deprovisions. A failed update means traffic stays on the old revision.

The follow-up question was practical: how do I give a test group a preview URL? In multiple-revision mode you attach a label to a revision, hand the label URL only to testers (the main URL is untouched), and promote it by moving the label. That is blue/green without any extra service.

## 2. Is a job just cron?

I assumed ACA jobs were a cron feature, then opened a jobs quickstart and saw a "manual" trigger. It turns out cron is just one of three trigger types:

| Trigger | What drives it |
|---|---|
| Manual | On demand: CLI, portal, API |
| Schedule | Cron expression, 5 fields, **UTC only** (no timezone parameter) |
| Event | Any KEDA scaler: queue depth, Service Bus, Kafka, and more |

The deeper surprise is the scaling unit. Apps scale replicas, which stay alive and consume events continuously. Jobs scale executions: one execution runs to completion and stops. There is no idle capacity, so jobs are "scale to zero by construction" rather than a special case.

My follow-up: does a scheduled job need Event Grid or some queue? No. A cron job needs only an environment and a container image in any reachable registry. The docs' own example uses a public Docker Hub image directly:

```bash
az containerapp job create \
    --name nightly --environment <ENV> --resource-group <RG> \
    --trigger-type Schedule --image <IMAGE> \
    --cron-expression "0 0 * * *"
```

## 3. Why Functions on ACA instead of a plain container?

Both a container app and an Azure Functions app on ACA run on the same substrate: same compute, same KEDA scaling, same networking. So what does Functions add? The programming model, not the hosting. Triggers and bindings give you declarative event wiring: write a method with attributes and the platform connects the event source for you. In a plain container you implement that plumbing yourself.

The non-obvious part is how scaling works. At startup the Functions host inspects your triggers and ACA **auto-generates matching KEDA scale rules** from what it finds: HTTP, Queue, Timer, Event Grid, Service Bus, and more. Scale goes from 0 to 1,000 instances. The portal checkbox "Optimize for Azure Functions" is just the UI for the same preset as `kind=functionapp`: the kind plus target port 80 plus platform-managed scaling.

The practical consequence: existing Functions code runs unchanged. It is a packaging change, not a code change. You ship your function container and the platform wires the event-driven scaling.

## 4. Can infra-level resiliency replace Polly?

ACA has service-discovery resiliency policies (in preview): timeouts, retries, circuit breakers, connection pools.

**Restarts are the platform's job via health probes.** On shutdown the platform sends SIGTERM and then SIGKILL after 30 seconds, and containers restart regularly: treat ACA as stateless by design and store state elsewhere. Resiliency policies are a separate layer: they shape how callers and load balancing behave around failures. A circuit breaker ejects a sick replica from the load-balancing pool for a window, then restores it. That is traffic-level ejection, not a restart.

Two scope limits matter more than the mechanics:

- Policies apply to **inbound service-discovery calls only**. Outbound calls from your app, and Dapr service invocation, are explicitly exempt and stay your code's job.
- Retry correctness (idempotency) is always the app's responsibility.

My follow-up was a design opinion: keep retries in code (testable, portable, one owner, uniform for inbound and outbound), and make the circuit-breaker ejection the one infra policy, because only infra can stop Envoy from routing to a sick replica. Never retry at both layers: 3 retries at each of two layers is 9 attempts against a failing endpoint.

## 5. Is it free for a small load?

Not free, but a small workload often bills $0. Three mechanisms stack:

- **Scale-to-zero.** The default minimum is 0 replicas, so an idle app has no compute cost.
- **Monthly free grants per subscription** (charged only above them): the first 180,000 vCPU-seconds (~50 vCPU-hours), the first 360,000 GiB-seconds (~100 GiB-hours), and the first 2 million HTTP requests.
- **Only external HTTP requests count.** Internal environment traffic and health probes are not billed. Log Analytics ingestion bills separately.

Two gotchas keep this from being a free tier:

- **Scale-to-zero is conditional.** CPU and memory scale rules cannot scale to zero; only event-driven KEDA rules can. And an app with no ingress, no scale rule, and min replicas 0 scales to zero and has no way to start again.
- **An idle environment auto-deletes after 90 days.** A personal learning environment that costs $0 can quietly vanish one day.

---

## Sources

Facts verified against Microsoft Learn in May 2026; resiliency policies were in preview at the time.

- Azure Container Apps overview — [learn.microsoft.com/en-us/azure/container-apps/overview](https://learn.microsoft.com/en-us/azure/container-apps/overview)
- Revisions and revision modes — [learn.microsoft.com/en-us/azure/container-apps/revisions](https://learn.microsoft.com/en-us/azure/container-apps/revisions)
- Jobs: triggers and recipes — [learn.microsoft.com/en-us/azure/container-apps/jobs](https://learn.microsoft.com/en-us/azure/container-apps/jobs)
- Azure Functions on Azure Container Apps — [learn.microsoft.com/en-us/azure/container-apps/functions-overview](https://learn.microsoft.com/en-us/azure/container-apps/functions-overview)
- Service discovery resiliency — [learn.microsoft.com/en-us/azure/container-apps/service-discovery-resiliency](https://learn.microsoft.com/en-us/azure/container-apps/service-discovery-resiliency)
- Billing in the consumption plan — [learn.microsoft.com/en-us/azure/container-apps/billing](https://learn.microsoft.com/en-us/azure/container-apps/billing)