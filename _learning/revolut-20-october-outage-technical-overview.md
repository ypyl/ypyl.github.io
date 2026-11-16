---
name: Revolut 20 October outage technical overview
link: https://www.revolut.com/blog/post/20octoutagetechnicaloverview/
category: Guide
date: 2026-11-15
tags: [2026-11-15, distributed-systems, postgres, database, architecture, cloud, reliability]
description: Revolut's post-mortem of the 20 October outage, in which a Google Cloud zone lost power and failover still did not fire because quorum groups had been running a server short for weeks with no alert on the missing member, and the database-metrics dashboard itself sat in the zone that died. The clearest public account of how a chain of small silent gaps turns a single-zone failure into a 06:00 to 17:15 UTC outage.
completed: 2026-11-16
---
