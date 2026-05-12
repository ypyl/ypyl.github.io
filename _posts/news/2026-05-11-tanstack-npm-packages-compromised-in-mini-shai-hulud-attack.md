---
layout: post
title: "TanStack npm Packages Compromised in Ongoing Mini Shai-Hulud Supply-Chain Attack"
date: 2026-05-11
tags: news
categories: news
---

Socket's Threat Research team detected **84 compromised TanStack npm package artifacts** modified with credential-stealing malware as part of the ongoing **Mini Shai-Hulud** supply-chain campaign. Affected packages like `@tanstack/react-router` (12M+ weekly downloads) included a heavily obfuscated `router_init.js` payload that harvests CI/CD credentials (GitHub Actions, AWS, Vault, Kubernetes) and propagates itself by exploiting npm OIDC trusted-publisher bindings to autonomously publish malicious versions. TanStack has deprecated affected versions and merged hardening changes, attributing the breach to a chained GitHub Actions attack involving the `pull_request_target` "Pwn Request" pattern and OIDC token extraction from runner memory.

[Socket Research — TanStack npm Packages Compromised](https://socket.dev/blog/tanstack-npm-packages-compromised-mini-shai-hulud-supply-chain-attack)
