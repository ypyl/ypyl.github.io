# AGENTS.md

## Repository Overview

Jekyll blog published to GitHub Pages at `https://ypyl.github.io/`. No build tooling needed locally — GitHub Pages auto-builds on push.

## Design

This site follows the **[Kami](https://github.com/tw93/Kami)** design system. See [Kami design reference](https://github.com/tw93/Kami/blob/main/references/design.md) for the full specification.

Key Kami constraints applied here:
- **Surfaces**: warm parchment (`#f5f4ed`, `--parchment`), never pure white.
- **Accent**: single ink-blue (`#1B365D`, `--brand`). No second chromatic color.
- **Typography**: serif hierarchy (Charter/Georgia), weight locked at 400/500, no italic.
- **Grays**: all warm-toned (yellow-brown undertone), no cool blue-grays.
- **Tags**: three tiers — default (`--tag-bg: #E4ECF5`), lightest (`--tag-bg-light: #EEF2F7`), and gradient. Solid hex only, never rgba.
- **Shadows**: whisper shadow (`0 4px 24px rgba(0,0,0,0.05)`) or ring (`0 0 0 1px var(--border)`), never hard drop shadows.

Design tokens are defined in `assets/css/style.css` (section 2, Design Tokens). All custom CSS in `tools.css` and inline styles must use Kami tokens.

## Content Structure

- `_posts/` — Blog posts. Filename format: `YYYY-MM-DD-title.md`
- `_news/` — Short news entries (Jekyll collection, `output: true`). Displayed as cards on the main page. Filename format: `YYYY-MM-DD-title.md`. Reference the source.
- `_tools/` — Tool reference cards (Jekyll collection, `output: false`, not rendered as pages). Browse at `/tools/`
- `_learning/` — Learning resource cards (Jekyll collection, `output: false`, not rendered as pages). Browse at `/learning/`
- `_presentations/` — Reveal.js slide decks (Jekyll collection, `output: true`). Paired with a blog post by matching filename slug. Uses `layout: presentation`. Browse at `/present/<slug>/`
- `_drafts/` — Unpublished drafts

## Frontmatter Conventions

**Posts** (`_posts/`):
```yaml
---
layout: post
title: Post Title
date: YYYY-MM-DD
tags: [tag1, tag2]
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

**Learning Resources** (`_learning/`):
```yaml
---
name: Resource Name
link: https://github.com/...
category: Category
tags: [tag1, tag2]
description: One-line description.
---
```

### Valid Tool Categories

New categories can be added when a tool doesn't fit any existing one — keep them broad enough to group related tools.

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
| Machine Learning | ML frameworks, training, data tools |
| Mapping | Geospatial and map tools |
| Media | Audio, video, streaming |
| Productivity | Productivity and note-taking tools |
| Programming | Languages, libraries, frameworks |
| Project Management | PM and planning tools |
| Search | Search engines and tools |
| Security | Security tools, privacy, governance |
| UI Component | UI component libraries and frameworks |

### Valid Learning Resource Categories

| Category | Use for |
|----------|---------|
| Book | Full-length textbooks and book companion repositories |
| Course | Structured learning with progressive sessions, workshops, hands-on platforms |
| Curated List | Aggregations of links, tools, or resources (awesome-* repos, curated collections) |
| Guide | Articles, blog posts, documentation guides on a specific topic |
| Research Paper | Academic research or deep technical analysis papers |
| Tutorial | Step-by-step how-to walkthroughs of a specific technique |

### Tag Guidelines

**Format**: All tags MUST use YAML array (flow sequence) format: `tags: [tag1, tag2]`. This applies to posts, tools, learning resources, and all other content types. Jekyll processes YAML arrays natively; comma-separated and other formats are not valid.

Create new tags freely when they describe a meaningful aspect of the content (e.g., a technology, domain, or capability).

- Use lowercase, hyphenated tags (e.g., `machine-learning`, `web-scraping`)
- Do not include dates — they belong in the filename/frontmatter `date` field
- Avoid vague qualifiers like `fast`, `lightweight`, `scalable`, `free`
- Prefer canonical forms: `llm` over `large-language-models`, `rag` over `retrieval-augmented-generation`, `nlp` over `natural-language-processing`
- Keep tags to 3–8 meaningful keywords
- Common tags include: `open-source`, `ai`, `python`, `typescript`, `react`, `javascript`, `rust`, `go`, `dotnet`

## Adding Content

- **New post**: Create `YYYY-MM-DD-descriptive-title.md` in `_posts/`
- **New news**: Create `YYYY-MM-DD-title.md` in `_news/` with `layout: post`, `title`, and `date` frontmatter. No `categories` or `tags` needed — the collection itself identifies the content type. Keep short and reference the source.
- **New tool**: Create `*.md` in `_tools/` with the tool frontmatter. Choose from valid tool categories above and follow tag guidelines.
- **New learning resource**: Create `*.md` in `_learning/` with the learning resource frontmatter. Choose from valid learning resource categories above and follow tag guidelines.
- **New presentation**: Create `<post-slug>.md` in `_presentations/` with `layout: presentation`. The filename must match the companion post's slug (without date prefix). Use `---` separators for slide breaks.
- **Draft**: Place in `_drafts/` (not rendered)

## Layouts

Site uses the `minima` theme. Custom layouts in `_layouts/`.

## GitHub Pages

Site builds automatically on push to `main`. No local Jekyll build required.
