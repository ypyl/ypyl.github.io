---
layout: post
title: "Revolut Traces Its 20 October Outage to a Missing Quorum Voter"
date: 2026-11-15
categories: news
---

Revolut published a technical post-mortem on 13 November for the outage that hit it on 20 October, when a power failure took out part of Google Cloud's London Zone B around 06:00 UTC and customers could not make payments, withdraw cash or open the app. Card authorisations and in-store payments returned at 12:40 UTC, app access, updated balances and transfers from 15:45 UTC, and the last services by 17:15 UTC.

Automatic failover never fired, for a reason smaller than the power cut. Each group of five coordination servers needs three to agree which database replica is primary, and some groups were already down to four after routine server replacements failed to restore all five members without anyone being alerted. Losing two servers in Zone B left two, so the failover manager could not promote a new primary at all. Recovery then slowed further because the primary dashboard for database metrics was deployed in Zone B, forcing engineers to read distributed database topologies from logs and command-line tools, and because replication lag left some PostgreSQL clusters read-only until replica state was verified by hand.

Revolut says it restored full failover by 13 November and has since extended its zonal failover tests to the conditions seen in this incident. In flight: monitoring of coordination server membership, voter health and remaining failure tolerance, end-to-end checks that alerts are actually generated and delivered, and a placement rule that stops the first server in each group from being stacked into the same zone.

[20 October outage: technical overview](https://www.revolut.com/blog/post/20octoutagetechnicaloverview/)
