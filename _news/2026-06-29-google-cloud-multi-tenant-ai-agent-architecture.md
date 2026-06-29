---
layout: post
title: "Google Cloud Publishes Reference Architecture for Multi-Tenant AI Agents"
date: 2026-06-29
tags: news
categories: news
---

Google Cloud has released a reference architecture for **multi-tenant AI agent systems** built on a hub-and-spoke model. A central hub handles routing, IAM, security, logging, and monitoring, while separate tenant projects serve individual business units—support, finance, sales, analytics—with isolated data, tools, and access rules.

Requests flow through Cloud Load Balancer, Cloud Armor, IAP, and Model Armor before reaching the target tenant, where the agent runs via Agent Runtime, ADK, MCP servers, and its own datastore (BigQuery or AlloyDB). A **Principal Access Boundary Policy** prevents agents in one tenant from accessing another tenant's data, while Model Armor screens for prompt injection, PII, and harmful content.

This provides a solid enterprise template: not one monolithic agent for the whole company, but a governed system with access boundaries, audit trails, and security controls.

[Google Cloud documentation](https://docs.cloud.google.com/architecture/multi-tenant-agentic-ai-system)
