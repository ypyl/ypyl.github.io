## Why

Blog articles are dense prose optimized for reading — not for screen-sharing during a live demo. When presenting an article's topic to an audience, the author currently has to manually create a separate slide deck in an external tool, duplicating effort and diverging from the blog as the source of truth. Adding a companion presentation capability lets the author write slides in the same repo, launch them from the article, and present directly in the browser with zero local tooling — keeping the blog as the single launch point for both reading and presenting.

## What Changes

- New Jekyll collection `presentations` for slide decks stored in `_presentations/`
- New layout `presentation.html` that renders slides using Reveal.js (loaded from CDN)
- New layout `post.html` that detects companion slides and renders a `[🖥️ Present]` button in the site nav
- Speaker notes supported via a Markdown convention (`> **Notes:** ...`), converted to Reveal.js `<aside class="notes">` by the layout
- Slide files are authored independently from articles — clean separation of prose (for reading) and bullets (for presenting)
- Both light and dark theme support, matching the blog's Kami design system
- Presentation pages accessible at `/present/<slug>/` from GitHub Pages with no local build tooling
- No changes to existing articles — companion slides are opt-in per article

## Capabilities

### New Capabilities

- `presentations-collection`: Jekyll collection for slide decks with companion association to blog posts
- `presentation-page`: Page layout that renders Reveal.js slides from `_presentations/` content, with speaker notes, light/dark theme toggle, and keyboard navigation

### Modified Capabilities

<!-- None — existing capabilities unchanged -->

## Impact

- New directory: `_presentations/` (Jekyll collection, `output: true`)
- New layouts: `_layouts/presentation.html`, `_layouts/post.html`
- Modified `_config.yml`: add `presentations` collection with `output: true` and `permalink: /present/:name/`
- Modified `_includes/header.html`: conditionally render `[🖥️ Present]` link when companion slides exist
- Zero JavaScript dependencies installed locally — Reveal.js loaded from CDN at presentation time
- No changes to existing articles, tools, learning resources, or site styling
