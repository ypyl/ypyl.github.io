---
layout: post
title: Notes about software architecture metrics
date: 2023-12-04
categories: programming
tags: architecture software
---

# Four Key Architecture Metrics

## Development throughput - Deployment Frequency

Deployment frequency refers to the rate at which individual changes successfully exit the development pipeline over a given period. These changes encompass various deployment units, such as code, configurations, or a blend of both. Examples include the introduction of new features or the resolution of software bugs.

## Development throughput - Lead Time for Changes

The lead time for changes represents the duration it takes for a developer's completed code or configuration alterations to traverse the development pipeline and emerge at the final stage.

## Stability of the overall service - Change Failure Rate

The change failure rate is the proportion of changes that exit the development pipeline and subsequently cause a failure in the operational service. A "failure" is anything blocking users from completing their tasks via the service.

## Stability of the overall service - Time to Restore Service

Time to restore service measures how long it takes to detect a service failure and deliver the necessary fix to restore it for users.
