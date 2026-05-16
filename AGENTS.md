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
tags: [tag1, tag2]
description: One-line description.
---
```

### Valid Categories

| Category | Use for |
|----------|---------|
| AI Agent Framework | Frameworks/SDKs for building AI agents |
| AI Model | Pre-trained models, model inference, serving |
| AI Tool | AI-powered applications and utilities |
| API | API directories and guidelines |
| Automation | Automation and browser-automation tools |
| Communication | Chat, messaging, notifications |
| Data & Visualization | Charts, graphs, data analytics |
| Database | Database tools, clients, management |
| Design | Design tools, icon sets, resources |
| Developer Tool | Dev utilities, CLI tools, testing |
| Document | PDF, spreadsheets, document processing |
| Infrastructure | Monitoring, networking, home automation, distributed systems |
| Learning Resource | Tutorials, books, curated lists, courses |
| Machine Learning | ML frameworks, training, data tools |
| Mapping | Geospatial and map tools |
| Media | Audio, video, streaming |
| Productivity | Productivity and note-taking tools |
| Programming | Languages, libraries, frameworks |
| Project Management | PM and planning tools |
| Search | Search engines and tools |
| Security | Security tools, privacy, governance |
| UI Component | UI component libraries and frameworks |

### Tag Guidelines

- Use lowercase, hyphenated tags (e.g., `machine-learning`, `web-scraping`)
- Do not include dates — they belong in the filename/frontmatter `date` field
- Avoid vague qualifiers like `fast`, `lightweight`, `scalable`, `free`
- Prefer canonical forms: `llm` over `large-language-models`, `rag` over `retrieval-augmented-generation`, `nlp` over `natural-language-processing`
- Keep tags to 3–8 meaningful keywords
- Common tags include: `open-source`, `ai`, `python`, `typescript`, `react`, `javascript`, `rust`, `go`, `dotnet`

## Adding Content

- **New post**: Create `YYYY-MM-DD-descriptive-title.md` in `_posts/`
- **New news**: Create `YYYY-MM-DD-title.md` in `_posts/news/` with `categories: news`. Keep short and reference the source.
- **New tool**: Create `*.md` in `_tools/` with the tool frontmatter. Choose from valid categories above and follow tag guidelines.
- **Draft**: Place in `_drafts/` (not rendered)

## Layouts

Site uses the `minima` theme. Custom layouts in `_layouts/`.

## GitHub Pages

Site builds automatically on push to `main`. No local Jekyll build required.
