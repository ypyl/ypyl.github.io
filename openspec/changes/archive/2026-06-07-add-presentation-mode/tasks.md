## 1. Configuration & scaffolding

- [x] 1.1 Add `presentations` collection to `_config.yml` with `output: true` and `permalink: /present/:name/`
- [x] 1.2 Create `_presentations/` directory
- [x] 1.3 Create `_layouts/post.html` stub extending `default` layout
- [x] 1.4 Create `_layouts/presentation.html` stub with Reveal.js skeleton

## 2. Post layout

- [x] 2.1 Implement `_layouts/post.html` with Minima-compatible post structure: `<article>`, `.post-header` (title + date), `.post-content`
- [x] 2.2 Verify existing posts render correctly with the new layout (title, date, code blocks, tables, dark mode, responsive)

## 3. Presentation layout — slide rendering

- [x] 3.1 Load Reveal.js 5.x from CDN with RevealMarkdown, RevealHighlight, and RevealNotes plugins
- [x] 3.2 Implement JS slide splitting: parse rendered content, split on `<hr />`, wrap each segment in `<section>` within `<div class="slides">`
- [x] 3.3 Implement speaker notes conversion: detect `<blockquote>` starting with `<strong>Notes:</strong>`, replace with `<aside class="notes">` containing the note text
- [x] 3.4 Initialize Reveal.js with options: controls, progress, slide number, history, center, transition

## 4. Header nav — Present link

- [x] 4.1 Add presentation detection logic in `_includes/header.html`: iterate `site.presentations`, match `pres.slug == page.slug` when `page.layout == 'post'`
- [x] 4.2 Render `[🖥️ Present]` link pointing to `/present/<slug>/` when match found
- [x] 4.3 Style the Present link to match existing nav item appearance (same font, color, hover behavior)

## 5. Kami styling for Reveal.js slides

- [x] 5.1 Create `assets/css/presentation.css` with Kami design tokens for slide styling (background, text, headings, links, code blocks) in light mode
- [x] 5.2 Add dark mode overrides using `[data-theme="dark"]` selector, matching blog's dark tokens
- [x] 5.3 Implement theme detection on load: read `localStorage('theme')` and `prefers-color-scheme`, apply `data-theme` attribute to `.reveal` container
- [x] 5.4 Add theme toggle button (sun/moon icon) to presentation page, sync with localStorage
- [x] 5.5 Style Reveal.js progress bar and navigation controls to match Kami aesthetic
- [x] 5.6 Include new CSS in `_includes/head.html` for presentation pages

## 6. Verification

- [x] 6.1 Create sample presentation file `_presentations/docker-compose.md` with 4+ slides, speaker notes, code blocks, and bullet lists
- [x] 6.2 Create or use an existing blog post with matching slug to verify `[🖥️ Present]` link appears
- [x] 6.3 Verify full presentation flow: article → click Present → slides render → keyboard navigation works → presenter mode opens with S key
- [x] 6.4 Verify theme toggle: switch light/dark, confirm preference persists across page reload and matches blog theme
- [x] 6.5 Verify no Present link appears on posts without companion slides
- [x] 6.6 Verify no regressions: home page, tools page, learning page, news cards all render unchanged
- [x] 6.7 Test on mobile viewport (375px) — slides should scale down and remain readable
