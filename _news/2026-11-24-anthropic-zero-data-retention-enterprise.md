---
layout: post
title: "Anthropic Opens Zero Data Retention to All Enterprise Customers"
date: 2026-11-24
categories: news
---

Anthropic moved **Enterprise Frontier Safeguards (EFS)** into public beta, which makes strong misuse detection compatible with **zero data retention**. Conversation logs stay in the customer's own Amazon S3 or Google Cloud Storage bucket under the customer's keys, and neither Anthropic nor the cloud provider keeps an additional copy.

Safety monitoring still runs. It reads request sequences to catch things a single request hides, such as an agent splitting one attack across many parallel calls, and reports incidents as a risk category plus metadata. Anthropic staff do not see conversation content, and nothing is used for model or classifier training.

More than 100 customers tested EFS, including a quarter of the Fortune 100 and named participants Bridgewater, Salesforce and Stripe. It is available on the Claude Platform, Claude Platform on AWS, Amazon Bedrock and Claude on Google Cloud, with Microsoft Foundry support coming later.

**Related:** [Anthropic Opens Self-Hosted Claude Managed Agents](/news/2026/11/15/anthropic-self-hosted-managed-agents/), [OpenRouter Off-Boards Three Providers and Strips ZDR Status From 125 Endpoints](/news/2026/11/23/openrouter-offboards-three-providers-over-zdr/)

[Zero data retention with Enterprise Frontier Safeguards](https://claude.com/resources/articles/zero-data-retention-with-enterprise-frontier-safeguards) · [Source channel](https://t.me/machinelearning_interview)
