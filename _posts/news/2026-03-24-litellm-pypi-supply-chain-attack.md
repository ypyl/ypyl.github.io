---
layout: post
title: "Malicious Code Found in LiteLLM PyPI Release 1.82.8"
date: 2026-03-24
tags: news
categories: news
---

The LiteLLM package on PyPI (version 1.82.8) was discovered to contain malicious code that could lead to severe data leaks, including SSH keys, cloud credentials (AWS/GCP/Azure), Kubernetes configs, API keys, command histories, SSL keys, CI/CD secrets, and database passwords. With nearly 97 million downloads per month, this supply chain attack poses a significant risk to many users.

The compromised package was identified accidentally when a developer experienced a memory leak failure during installation, which might have otherwise gone unnoticed for a long time. The malicious code could also reach users indirectly through other packages, such as `dspy`.

This incident highlights the growing dangers of supply chain attacks in software development, urging a reassessment of the reliance on extensive third-party libraries.

For more details, see the original GitHub issue: https://github.com/BerriAI/litellm/issues/24512 and the analysis by FutureSearch: https://futuresearch.ai/blog/litellm-pypi-supply-chain-attack/.