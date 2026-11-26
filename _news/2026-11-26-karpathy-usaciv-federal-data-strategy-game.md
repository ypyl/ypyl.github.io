---
layout: post
title: "Karpathy's Claude-Built US Map Turns Federal Data into a Strategy Game"
date: 2026-11-26
categories: news
---

Andrej Karpathy asked Claude Opus 5.5 to populate a Civilization-style game map with real data from the United States. **Seven hours and 45,000 lines of code** later, [usaciv](https://karpathy.ai/usaciv/) is live: **31 public sources**, including Census, BEA, BLS, EIA, NOAA, USGS, DoD, NPS, FAA, FEMA and CDC, reconciled onto a single **25 km hex grid** covering elevation, land cover, GDP, jobs, power plants, mines, ports, airports, bases, parks, universities, freight flows and disasters.

The translation into game terms is the part no standard ETL pipeline does. GDP becomes gold yield, a city's districts are its real industry mix (San Jose gets a Campus, Detroit is "Motor city"), and citizens are the real workforce: 1 in 4 San Jose workers are engineers, coders or scientists against 1 in 14 nationally. Wonders follow published rules, national parks by visitor numbers and universities by R&amp;D spending. The build ships **19 lenses**, a City View for **524 metros**, nine advisors, and an Explorer that turns any column for any of **3,100+ counties** into a table and scatter plot linked to the map.

Karpathy ran the job in the Claude Code desktop app with Opus 5.5 on medium and "ultracode" enabled, and names the gap himself: **Claude skipped a few states and territories**. He calls the result fun to browse rather than finished, floats expanding it into an actual game, and notes he has half his weekly Claude quota left for someone else to take over.

[Andrej Karpathy on X](https://x.com/karpathy/status/2125779541952246039) · [usaciv](https://karpathy.ai/usaciv/) · [Channel](https://t.me/ai_machinelearning_big_data)
