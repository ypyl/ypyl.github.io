# AGENTS.md

## Repository Overview

Jekyll blog published to GitHub Pages at `https://ypyl.github.io/`. No build tooling needed locally — GitHub Pages auto-builds on push.

## Content Structure

- `_posts/` — Blog posts. Filename format: `YYYY-MM-DD-title.md`
- `_posts/news/` — Short news entries (categories: news). Displayed as cards on the main page. Reference the source.
- `_tools/` — Tool reference cards (Jekyll collection, `output: false`, not rendered as pages). Browse at `/tools/`
- `_drafts/` — Unpublished drafts

## Frontmatter Conventions

**Posts** (`_posts/`):
```yaml
---
layout: post
title: Post Title
date: YYYY-MM-DD
tags: tag1, tag2
categories: category-name
---
```

**Tools** (`_tools/`):
```yaml
---
name: Tool Name
link: https://github.com/...
category: Category
tags: [tag1, tag2, YYYY-MM-DD]
description: One-line description.
---
```

## Adding Content

- **New post**: Create `YYYY-MM-DD-descriptive-title.md` in `_posts/`
- **New news**: Create `YYYY-MM-DD-title.md` in `_posts/news/` with `categories: news`. Keep short and reference the source.
- **New tool**: Create `*.md` in `_tools/` with the tool frontmatter
- **Draft**: Place in `_drafts/` (not rendered)

## Layouts

Site uses the `minima` theme. Custom layouts in `_layouts/`.

## GitHub Pages

Site builds automatically on push to `main`. No local Jekyll build required.
