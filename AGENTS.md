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

## Diagrams

Diagrams follow the **[Kami diagrams reference](https://github.com/tw93/Kami/blob/main/references/diagrams.md)**: self-contained inline SVG, parchment + ink-blue + warm grays, no second design system.

- **Draw only when needed**: "Would a well-written paragraph teach the reader less than this diagram?" If no, don't draw.
- **Embedding**: inline the `<svg>` directly in a post inside a `<figure>`, wrapped in `{::nomarkdown}...{:/nomarkdown}` so Kramdown passes it through untouched. Include `role="img"`, `<title>`, and `<desc>`. Use an in-SVG "FIGURE N · TITLE" header instead of a `<figcaption>`. Set `style="width:100%;height:auto"` on the root.
- **Palette**: use the Kami diagram token map only — canvas `#f5f4ed` (with optional dotted-noise overlay), standard node fill `#faf9f5` + stroke `#141413`, focal fill `#EEF2F7` + stroke `#1B365D`, standard arrow `#504e49`, discarded/muted `#6b6a64` dashed. Solid hex only, never `rgba()`.
- **Complexity budget**: max 9 nodes per figure; 1–2 focal elements max (ink-blue); node widths 128/144/160, heights 32/64; names in sans, mono only for small tags/labels.
- **Line discipline**: orthogonal lines only (no curves, no diagonals); main path in brand, auxiliary lines in olive; open chevron arrowheads (two-stroke path, never `<marker>` or filled triangles); arrow endpoints land exactly on node edges.
- **Maturity encoding**: dashed `--stone` stroke + reduced opacity (≈0.55–0.65) for nodes that don't persist (future/concept-only), never focal color.
- **Anti-patterns to check before shipping**: no gradients, shadows, pure `#fff`, scripts, external images, em dashes, `rgba()`, text on top of lines without a masking rect, or text clipped at the viewBox top (`text` `y` is the baseline — pad the top by font-size × 1.2).

Reference example in this repo: the SKILL.state execution cycle figure in `_posts/2026-08-28-skills-need-state-not-history.md`.

## Content Quality

Content follows the **[Kami anti-patterns reference](https://github.com/tw93/Kami/blob/main/references/anti-patterns.md)**: one claim, one proof. Rewrite drafts that exhibit any of the following — they are the common AI-document failures, not style preferences.

**Substance**
- No adjective pile-up without numbers: "significant growth" → "revenue grew 34% YoY". No opening-paragraph filler ("In today's rapidly evolving landscape..."); start with the first real claim. Body must add information the heading does not carry. Pin vague time references to a date ("launched Q1 2026"). One claim, one proof, move on — never restate the same point in fresh synonyms.
- Match the source's precision: "around 10K" stays approximate; never add decimal precision without a citation; never invent comparison baselines ("3x faster than alternatives" needs a named benchmark); never mix comparison windows (YoY vs QoQ) without labeling each; text must always match any figure it accompanies.

**Sources**
- Verify version numbers and availability before citing ("uses the latest framework" → "uses Next.js 15, as of 2026-04"). Ranking/market claims need a cited source, or soften to "one of the established solutions". Hand-copied dynamic counts (GitHub stars, prices) are expiring facts — print a snapshot date next to them or omit.

**Tone**
- No corporate speak ("leverage", "unlock synergies", "end-to-end solution") and no AI-tone cliches ("essentially", "notably", "it is worth noting"). No em dashes in prose — use a colon or a full stop. Self-check with a grep for `leverag|unlock|synerg|notably|worth noting|——|–` before pushing.
- Captions (figures, images, diagrams) must give a judgment the visual alone cannot: what to conclude, not what the drawing shows. A caption that restates the diagram or title is wasted attention.

**Visuals**
- One accent color for emphasis (the single Kami brand); hierarchy comes from weight and size, not additional colors or decorative icons — no emoji or decorative icons as section markers.
- Every chart or figure must carry an insight the surrounding text does not already state; a decorative restatement of the prose gets deleted.
- Images: decide the slot ratio before generating; keep real screenshots real (never redraw them as fake UI); normalize mixed screenshot ratios within one group; never substitute abstract atmosphere for a missing screenshot — mark the material gap or omit the panel.

**Slides** (presentations)
- Titles must form a coherent argument read alone (the ghost-deck test). Maximum one evidence shape per slide (chart, code block, screenshot, or quote — not several together).

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
- **New presentation**: Create `<post-slug>.md` in `_presentations/` with `layout: presentation`. The filename must match the companion post's slug (without date prefix). Use `---` separators for slide breaks. **Code blocks**: specify a language identifier on fenced code blocks (e.g., ```` ```bash ````, ```` ```typescript ````). Jekyll's Kramdown invokes Rouge to pre-highlight language-tagged blocks, but a JS bridge in the presentation layout strips the Rouge spans and passes the language hint to Reveal.js's highlight.js plugin, which applies the monokai theme.
- **Draft**: Place in `_drafts/` (not rendered)

## Layouts

Site uses the `minima` theme. Custom layouts in `_layouts/`.

## GitHub Pages

Site builds automatically on push to `main`. No local Jekyll build required.
