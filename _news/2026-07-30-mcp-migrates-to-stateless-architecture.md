---
layout: post
title: "MCP Migrates to Stateless Architecture"
date: 2026-07-30
tags: news
categories: news
---

The Model Context Protocol (MCP) has moved to a **stateless architecture**, removing persistent sessions and sticky routing — a change that allows MCP servers to be deployed behind standard cloud load balancers and in Kubernetes clusters without losing agent context on pod restarts. The update adds mandatory `issuer` parameter validation to protect against OAuth mix-up attacks, and introduces Enterprise Managed Authorization built in partnership with Okta. MCP Apps and MCP Tasks have been promoted to official extension status, and maintainers have committed to a 12-month deprecation policy for all future changes to support enterprise infrastructure stability.

**Related:** [MCP Apps Support in VS Code](/news/2026/01/26/mcp-apps-support-in-vs-code/)

[AAIF: Migrate sessions to stateless requests with MCP](https://aaif.io/blog/migrate-sessions-to-stateless-requests-with-mcp-2026-07-28)
