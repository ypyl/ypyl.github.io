---
layout: post
title: "Axios Library Compromised with Remote Access Trojan"
date: 2026-03-31
tags: news
categories: news
---

On March 30, attackers published two malicious versions (1.14.1 and 0.30.4) of the widely used JavaScript library Axios on npm, embedding a hidden cross-platform Remote Access Trojan (RAT) installer. The compromised package included a dependency on a malicious plain-crypto-js@4.2.1, which executed the RAT dropper on Windows, macOS, and Linux immediately after installation.

The attack bypassed normal CI/CD pipelines by using a stolen npm token and compromised the account of a lead maintainer. The malicious versions were available on npm for about three hours before removal. Users who installed these versions are urged to check for the presence of plain-crypto-js in node_modules and consider their secrets compromised.

Source: [StepSecurity](https://www.stepsecurity.io/blog/axios-compromised-on-npm-malicious-versions-drop-remote-access-trojan)
