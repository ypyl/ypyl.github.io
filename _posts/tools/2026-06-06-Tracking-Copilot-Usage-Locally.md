---
layout: post
title: "Tracking GitHub Copilot usage and cost locally"
date: 2026-06-06
tags: copilot, opentelemetry, powershell, cost
categories: tools, ai
---

GitHub Copilot Business gives you a pool of AI credits, but as a regular user you can't see your own consumption — only org admins get the dashboard. Meanwhile, credit usage can spike unexpectedly, and you have no idea which sessions or models ate them up.

The simplest path is VS Code's SQLite span exporter. Two settings, zero infrastructure:

```json
"github.copilot.chat.otel.dbSpanExporter.enabled": true
```

Then **Ctrl+Shift+P → "Chat: Export Agent Traces DB"** gives you a `.db` file with every chat, tool call, model, and token count.

I wrote two PowerShell scripts to automate the setup and extract useful stats from that database. The core idea: parse the exported spans, filter out internal Copilot overhead (session naming, embeddings, context summarization — not billed to you), apply GitHub's published per-model pricing, and print a JSON report.

## Usage

```powershell
# One-time setup
.\setup-otel.ps1

# After using Copilot, export the DB, then:
.\copilot-stats.ps1 -DbPath agent-traces.db -Sessions   # per-session breakdown
.\copilot-stats.ps1 -DbPath agent-traces.db -Daily       # daily aggregation
.\copilot-stats.ps1 -DbPath agent-traces.db -Cost        # cost by model
.\copilot-stats.ps1 -DbPath agent-traces.db -Daily -Period "2026-06-06"
```

The cost estimates are computed from GitHub's published model pricing, not from any billing API. Close enough to spot discrepancies before the bill arrives.

Repo: [github.com/ypyl/vs-code-copilot-stats](https://github.com/ypyl/vs-code-copilot-stats)
