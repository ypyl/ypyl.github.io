---
layout: post
title: "OpenSpec Local Viewer — A Single-File, Local-First Viewer for OpenSpec Artifacts"
date: 2026-08-17
tags: [openspec, local-first, html5, developer-tools]
categories: programming
---

A small side project: [OpenSpec Local Viewer](https://github.com/ypyl/openspec-viewer) — a single `index.html` (v1.2.1) that browses OpenSpec artifacts entirely on your machine. Live at [ypyl.github.io/openspec-viewer](https://ypyl.github.io/openspec-viewer/), and it works just as well downloaded and opened from `file://`.

## The three whys

**Local first.** The viewer reads your `openspec/` folder with the browser's File System Access API and polls every 30 seconds — added, modified, or deleted artifacts appear without reloading. Nothing is uploaded anywhere; the folder never leaves your machine. The only thing persisted is your last-chosen folder (via IndexedDB), so the picker reopens where you left off.

**HTML5 for spec changes.** No server, no build step. One file of HTML + CSS + JS, using modern browser APIs directly: File System Access for the folder handle, IndexedDB for persistence. Changes to a spec are visible in the file on disk, and after a 30-second poll the viewer hot-refreshes them in place — spec review happens in the same loop you edit in, not after a publish step.

**Simple.** Open a change and its artifacts render as tabs — Proposal, Spec(s), Design, Tasks, Metadata. Dark and light themes, collapsible sidebar sections, and a filter box. OpenSpec's structure is simple by design (delta specs, `propose → apply → archive`), and the viewer tries to match it: browse, read, done.

## Notes

- Browsers without File System Access API fall back to a one-shot folder read — degrades gracefully, loses live updates.
- Live monitoring is Chrome/Edge only; the fallback covers the rest.
A note more than a tool: the whole app is small enough to read top to bottom in one sitting, which is the point. Keep the tooling for a spec-change workflow as close to the artifacts as the workflow itself.