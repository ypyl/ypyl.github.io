# AGENTS.md

## Repository Overview

Jekyll blog published to GitHub Pages at `https://ypyl.github.io/`. GitHub Pages builds on push to `master`.

Pages, layouts, and collections are wired by hand, so a new content type takes more than a folder:

- Root pages: `index.md` (home), `tools.md`, `learning.md`, `apps.md`. Each one picks a layout.
- `_config.yml` registers every collection (`output: false` for card collections) and lists the nav pages under `header_pages`. A collection is invisible until it is registered there and given a root page.

## Design

This site follows the **[Kami](https://github.com/tw93/Kami)** design system. See [Kami design reference](https://github.com/tw93/Kami/blob/main/references/design.md) for the full specification.

Key Kami constraints applied here:
- **Surfaces**: warm parchment (`#f5f4ed`, `--parchment`), never pure white.
- **Accent**: single ink-blue (`#1B365D`, `--brand`). No second chromatic color.
- **Typography**: serif hierarchy (Charter/Georgia), weight locked at 400/500, no italic.
- **Grays**: all warm-toned (yellow-brown undertone), no cool blue-grays.
- **Tags**: three tiers — default (`--tag-bg: #E4ECF5`), lightest (`--tag-bg-light: #EEF2F7`), and gradient. Solid hex only, never rgba.
- **Shadows**: whisper shadow (`0 4px 24px rgba(0,0,0,0.05)`) or ring (`0 0 0 1px var(--border)`), never hard drop shadows.

Design tokens are defined in `assets/css/style.css` (section 2, Design Tokens). Every custom stylesheet in `assets/css/` (`tools.css`, `learning.css`, `apps.css`, `presentation.css`) and all inline styles must use Kami tokens, never raw values.

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

- `_posts/` — Blog posts. Filename format `YYYY-MM-DD-title.md`. Layout: `post`.
- `_news/` — Short news entries (Jekyll collection, `output: true`). Filename format `YYYY-MM-DD-title.md`. Rendered as cards on the home page; reference the source.
- `_tools/` — Tool reference cards (Jekyll collection, `output: false`). Rendered by `tools.md` through `_layouts/tools.html`; browse at `/tools/`.
- `_learning/` — Learning resource cards (Jekyll collection, `output: false`). Rendered by `learning.md` through `_layouts/learning.html`; browse at `/learning/`.
- `_apps/` — Apps built by me (Jekyll collection, `output: false`). Rendered by `apps.md` through `_layouts/apps.html`; browse at `/apps/`.
- `_presentations/` — Reveal.js slide decks (Jekyll collection, `output: true`). Paired with a post by matching filename slug (without the date prefix). Layout: `presentation`; browse at `/present/<slug>/`.
- `_drafts/` — Unpublished drafts. Not rendered.

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
tags: [YYYY-MM-DD, tag1, tag2]   # first tag is the date the card was added
description: One-line description.
---
```

**Learning Resources** (`_learning/`):
```yaml
---
name: Resource Name
link: https://github.com/...
category: Category      # required: Book, Course, Curated List, Guide, Research Paper, Tutorial
tags: [YYYY-MM-DD, tag1, tag2]   # first tag is the date the record was added
description: One-line description.
---
```

`category` is the only place a learning resource records what it is. Never repeat it as a tag.

**Apps** (`_apps/`):
```yaml
---
name: App Name
link: https://live-app-url/
repo: https://github.com/...   # optional
order: 1                       # display order, ascending
tags: [YYYY-MM-DD, tag1, tag2] # first tag is the date the card was added
description: One or two sentences on what the app does.
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
| Simulation | Monte Carlo / scenario simulators and forecasting engines |
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

- Use lowercase, hyphenated tags (e.g., `machine-learning`, `web-scraping`). Spaces, underscores, capitals, and `.`/`#` prefixes are not allowed.
- **The first tag of a card record is the date it was added** (`YYYY-MM-DD`), e.g. `tags: [2026-06-09, rag, python, llm]`. It records when the entry entered the collection, so an old card stays distinguishable from a new one. When the frontmatter has a `date` field, the tag must equal it. All tools, learning resources, and apps carry one; the older tool and app cards were backfilled from their git add-date. Card includes hide this tag from the visible tag chips and from the search index (`_includes/tool-item.html`, `learning-item.html`, `app-item.html` keep only non-date tags).
- Tags otherwise describe the subject only. Never put metadata in them: no collection name (`news`), and for learning resources no resource type (`book`, `course`, `curated-list`, `guide`, `research-paper`, `tutorial`) because that is the job of `category`.
- `stale` is the only status tag. Use it on a tool whose upstream repo is archived, deleted, or has gone quiet for over a year.
- Avoid vague qualifiers like `fast`, `lightweight`, `scalable`, `free`, and empty labels like `useful`, `links`, `general`, `todo`.
- Keep tags to 3–8 meaningful keywords. More than 8 means the classification is too fine to filter on.

**One canonical spelling per concept.** These aliases were reconciled in October 2026 and must not reappear:

| Canonical | Rejected variants |
|-----------|-------------------|
| `ai-agents` | `agents`, `agent`, `agentic`, `agentic-ai`, `llm-agents`, `agent-framework`, `agent-runtime` |
| `csharp` | `c#` |
| `dotnet` | `.net`, `net`, `.NET` |
| `go` | `golang` |
| `real-time` | `realtime` |
| `nlp` | `natural-language-processing` |
| `llm` | `language-model` |
| `prompt-engineering` | `prompting`, `prompts`, `meta-prompting` |
| `spec-driven-development` | `sdd`, `spdd` |
| `developer-tool` | `dev-tool`, `developer-tools` |
| `ui-library` | `uikit`, `ui-component`, `uicomponents`, `components` |
| `orchestration` | `agent-orchestration` |
| `plugin` | `plugins`, `agent-plugin` |
| `diagrams` | `diagramming` |
| `aspnet` | `asp`, `asp-net-core` |
| `ef-core` | `ef` |
| `cosmos-db` | `cosmosDB` |
| `influxdb` | `indluxdb` |
| `opentelemetry` | `open-telemetry` |
| `react-query` | `tanstack-query` |
| `testing` | `test` |
| `mocking` | `mock` |
| `desktop-application` | `desktop` |
| `mathematics` | `math` |

- Common tags include: `open-source`, `ai`, `python`, `typescript`, `react`, `javascript`, `rust`, `go`, `dotnet`, `llm`, `ai-agents`

Before pushing a content change, check the frontmatter with:

```bash
# capitals or underscores inside tags (must be empty)
grep -rh '^tags:' _tools _learning _apps _posts | grep -E '[A-Z]|_'
# tags that are not a YAML array (must be empty)
grep -rn '^tags: [^[]' _tools _learning _apps _posts
# card records missing the add-date tag (must be empty)
for f in _tools/*.md _learning/*.md _apps/*.md; do grep -qE '^tags: \[[0-9]{4}-[0-9]{2}-[0-9]{2},' "$f" || echo "no date tag: $f"; done
```

## Adding Content

- **New post**: Create `YYYY-MM-DD-descriptive-title.md` in `_posts/`
- **New news**: Create `YYYY-MM-DD-title.md` in `_news/` with `layout: post`, `title`, and `date` frontmatter. No `categories` or `tags` needed — the collection itself identifies the content type. Keep short and reference the source.
- **New tool**: Create `*.md` in `_tools/` with the tool frontmatter. Choose from valid tool categories above and follow tag guidelines.
- **New learning resource**: Create `*.md` in `_learning/` with the learning resource frontmatter, including `category` from the valid categories above. Follow tag guidelines.
- **New app**: Create `*.md` in `_apps/` with the app frontmatter (`link` is the live app, `repo` the source). Follow tag guidelines.
- **New presentation**: Create `<post-slug>.md` in `_presentations/` with `layout: presentation`. The filename must match the companion post's slug (without date prefix). Use `---` separators for slide breaks. **Code blocks**: specify a language identifier on fenced code blocks (e.g., ```` ```bash ````, ```` ```typescript ````). Jekyll's Kramdown invokes Rouge to pre-highlight language-tagged blocks, but a JS bridge in the presentation layout strips the Rouge spans and passes the language hint to Reveal.js's highlight.js plugin, which applies the monokai theme.
- **Draft**: Place in `_drafts/` (not rendered)

## Layouts

The `minima` theme supplies the base; overrides live in `_layouts/`.

| Layout | Used by |
|--------|---------|
| `default` | Every page: head, header nav, footer, scroll-to-top. |
| `home` | `index.md`: news carousel and the post list. |
| `post` | `_posts/` and `_news/`. |
| `presentation` | `_presentations/`: Reveal.js deck. |
| `tools` | `tools.md`: categories plus search. |
| `learning` | `learning.md`: searchable resource cards. |
| `apps` | `apps.md`: app cards ordered by `order`. |

Card markup lives in one include per collection: `_includes/tool-item.html`, `learning-item.html`, `app-item.html`.

## Local Validation with wslc

Use the Windows Subsystem for Linux Container CLI (`wslc`) to check a change before pushing. Docker Desktop is not involved and its daemon does not need to be running. Run these from the repo root in Git Bash.

`MSYS_NO_PATHCONV=1` stops Git Bash from rewriting the container path, and `$(pwd -W)` gives the Windows path that `wslc` needs for the bind mount.

**Build check.** Fails on a Liquid or YAML error:

```bash
export MSYS_NO_PATHCONV=1
wslc run --rm \
  -v "$(pwd -W):/srv/jekyll" \
  -w /srv/jekyll \
  jekyll/jekyll:4 jekyll build --trace
```

**Preview server** at http://localhost:4000:

```bash
export MSYS_NO_PATHCONV=1
wslc run -d --name jekyll-preview \
  -p 4000:4000 \
  -v "$(pwd -W):/srv/jekyll" \
  -w /srv/jekyll \
  jekyll/jekyll:4 jekyll serve \
  --host 0.0.0.0 --port 4000 --force_polling --disable-disk-cache
```

```bash
wslc logs jekyll-preview      # build output
wslc rm -f jekyll-preview     # stop and remove
rm -rf _site .jekyll-cache    # remove build artifacts
```

- `--force_polling` is required. The bind mount does not deliver inotify events, so without it the server misses your edits.
- The first run pulls the `jekyll/jekyll:4` image.
- The build prints Sass `lighten()` deprecation warnings from the `minima` theme. They are not errors. Treat the build as failed only on a real error or a non-zero exit.
- `_site/` and `.jekyll-cache/` are gitignored.

## Publishing

Push to `master`. GitHub Pages rebuilds and the change is live in about a minute; confirm the URL returns 200 rather than trusting the push alone.

```bash
git push github master
```

`master` tracks the `github` remote. A `nas` remote also exists; GitHub Pages does not use it.
