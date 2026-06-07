## Context

The blog is a Jekyll static site on GitHub Pages using the Minima theme with the "Kami" design system (parchment colors, serif typography, dark mode via `data-theme` attribute and localStorage). No local build tooling — GitHub Pages auto-builds on push. Existing Jekyll collections: `tools` and `learning` (both `output: false`). Posts are long-form Markdown stored in `_posts/`.

The blog currently has no post layout override — it uses Minima's default `post.html`. Navigation is rendered by `_includes/header.html`, which shows site title + page links + dark mode toggle. Interactive features (search, scroll-to-top, theme toggle) are implemented in vanilla JS.

## Goals / Non-Goals

**Goals:**
- Allow authors to create companion slide decks for any blog post, stored in `_presentations/`
- Render slides in the browser using Reveal.js (loaded from CDN), accessible at `/present/<slug>/`
- Support speaker notes, keyboard navigation, presenter mode (press S), code highlighting, and a light/dark theme toggle
- Show a `[🖥️ Present]` link in the site nav when viewing a post that has companion slides
- Zero local tooling required — fully compatible with GitHub Pages auto-build
- Slide styling consistent with the blog's Kami design system

**Non-Goals:**
- Auto-generating slides from article content (slides are authored independently)
- Slidev or other CLI-based presenters (Reveal.js only, in-browser)
- PDF export, recording, or drawing on slides
- Slide transitions other than Reveal.js defaults
- Modifying existing posts or their reading experience

## Decisions

### D1: Jekyll collection for slide decks

New collection `presentations` with `output: true` and `permalink: /present/:name/`. Each file is a standalone Markdown document with minimal frontmatter:

```yaml
---
layout: presentation
---
```

**Why a collection over a data file or standalone pages:** Collections provide automatic URL generation, access via `site.presentations` in Liquid, and a clean directory structure. Output pages are needed for GitHub Pages (no server-side routing).

**Alternatives considered:**
- Data files (`_data/`): No URL generation, can't render pages.
- Standalone pages in root: Clutters the repo, no grouping, no Liquid iteration.

### D2: Reveal.js from CDN

Reveal.js 5.x loaded from `cdn.jsdelivr.net` with these plugins:
- `RevealMarkdown` — native markdown support within slides
- `RevealHighlight` — code syntax highlighting
- `RevealNotes` — speaker notes / presenter mode

**Why Reveal.js over other libraries:**
- Battle-tested (27k stars), actively maintained
- Presenter mode with two windows (press S)
- Markdown-native slide authoring
- Works entirely client-side, no build step
- ~50KB gzipped from CDN, only loaded on presentation pages

**Why CDN over bundling:** No build tooling in the project. CDN keeps the repo lightweight. The presentation page is a separate URL — the extra load time is acceptable.

### D3: Slide boundaries via `<hr />` (Jekyll's `---`)

Authors write slides separated by `---` (standard Markdown horizontal rule). Jekyll renders this to `<hr />`. The `presentation.html` layout splits `{{ content }}` on `<hr />` using JavaScript, wrapping each segment in `<section>` tags that Reveal.js treats as slides.

```markdown
# Slide 1 title
- Bullet point

---

# Slide 2 title
- Another point
```

**Why split client-side in the layout vs. Jekyll pre-processing:** Jekyll's Liquid has no string split function. The `content` variable is pre-rendered HTML. Client-side splitting with vanilla JS is straightforward and adds ~10 lines.

**Alternative considered:** Using Jekyll `split` filter (doesn't exist for content). Using `{% for %}` with post excerpts (too rigid).

### D4: Speaker notes convention

Authors write speaker notes as blockquotes prefixed with `**Notes:**`:

```markdown
> **Notes:** Mention the v2 migration path here
```

The `presentation.html` layout, after splitting into `<section>` elements, converts any `<blockquote>` starting with `<strong>Notes:</strong>` into `<aside class="notes">` (Reveal.js speaker notes format). These are hidden from the audience but visible in presenter mode.

**Why a prefix convention over raw HTML:**
- More readable and writable than `<aside class="notes">` raw HTML
- Fits naturally in Markdown authoring flow
- Blockquotes are a natural container for "meta" content

### D5: Theme support — light and dark

The presentation page reads the blog's theme preference from `localStorage` (key: `theme`) or `prefers-color-scheme` media query, then applies a `data-theme` attribute to the Reveal.js container. Custom CSS uses the blog's Kami design tokens for slide styling:

- **Light mode:** Parchment background (`#f5f4ed`), near-black text, ink-blue (`#1B365D`) accents
- **Dark mode:** Deep dark background (`#141413`), light text, muted blue accents

A theme toggle button (sun/moon icon, matching the blog's existing toggle) is placed in the presentation controls bar.

**Why Kami tokens in Reveal.js:** Consistency with the blog. Presenter and audience see the same design language whether reading or watching slides.

### D6: Post-to-presentation association via slug matching

The `[🖥️ Present]` button appears in the site nav when viewing a post that has a companion presentation. Detection logic in `_includes/header.html`:

```liquid
{% if page.layout == 'post' %}
  {% for pres in site.presentations %}
    {% if pres.slug == page.slug %}
      <!-- render Present link -->
    {% endif %}
  {% endfor %}
{% endif %}
```

Presentation file naming: match the post slug EXACTLY (the post filename without the date prefix and `.md` extension). Example:

| Post file | Slug | Presentation file |
|-----------|------|-------------------|
| `_posts/2026-05-25-React-SPA-v2.md` | `React-SPA-v2` | `_presentations/React-SPA-v2.md` |

**Why slug matching over explicit `post_slug` frontmatter:** One less field to maintain. The filename convention is the contract. If an author wants different naming, they can still use the presentation independently — it just won't auto-link from the post.

### D7: Post layout override

Create `_layouts/post.html` that extends `default` and replicates Minima's post structure (title, date, content with `.post-content` class). This is necessary because:
1. We need to add the `[Present]` link logic to the nav, which checks `page.layout`
2. Minima's post layout is in the theme gem, not in our repo
3. Our `_layouts/post.html` will override the theme's, so we must replicate the post display

The layout includes the standard Minima post markup: `<article class="post">`, `<header class="post-header">`, `<div class="post-content">`.

## Risks / Trade-offs

**[R1] Reveal.js CDN dependency:** If `cdn.jsdelivr.net` is down, presentations won't load.
→ **Mitigation:** Low risk — jsDelivr has 99.9%+ uptime. Presentation is a presentation feature, not critical site functionality. Could add a `<noscript>` fallback message.

**[R2] Slide content may not render well on small screens:** Reveal.js scales slides to fit the viewport, but dense slides (large tables, wide diagrams) could be hard to read on small displays.
→ **Mitigation:** Authoring guidance — keep slides simple. Reveal.js scales proportionally; the presenter controls the viewport.

**[R3] Post layout override could diverge from Minima updates:** If the Minima theme updates its post.html, our override won't pick up changes.
→ **Mitigation:** Minima is stable and rarely changes. The post structure is simple (title, date, content). We pin the Minima version in `_config.yml`.

**[R4] Slug matching is case-sensitive and fragile:** If post slug doesn't match presentation filename exactly, the link won't appear.
→ **Mitigation:** Document the convention clearly. The presentation still works at its URL — authors just won't see the convenience button.

## Open Questions

- None remaining — all design decisions resolved.
