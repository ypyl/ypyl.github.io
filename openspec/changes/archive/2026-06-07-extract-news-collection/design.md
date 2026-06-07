## Context

News entries currently live at `_posts/news/` and use `categories: news` to distinguish themselves from regular blog posts. The home page (`_layouts/home.html`) filters `site.posts` by category to separate news from posts. Every news file carries redundant `categories: news` and `tags: [news]` frontmatter. The `_presentations/` collection already establishes the pattern of using Jekyll collections for distinct content types.

## Goals / Non-Goals

**Goals:**
- Move 186 news files from `_posts/news/` to `_news/` as a Jekyll collection
- Strip redundant `categories: news` and `tags: [news]` from all news frontmatter
- Simplify `home.html` to use `site.news` directly
- Preserve existing URL structure with `/news/` prefix: `/news/:year/:month/:day/:title/`
- Keep `layout: post` for news entries (no layout changes)

**Non-Goals:**
- Changing news card design, CSS, or visual appearance
- Adding RSS feed for news
- Changing news file naming convention (date prefixes remain)
- Adding navigation links for news
- Modifying how regular blog posts work

## Decisions

### Decision 1: Date-aware permalink pattern

Use `permalink: /news/:year/:month/:day/:title/` for the collection. This preserves the date context in URLs (like current post URLs), uses the `date:` frontmatter field that all 186 files already have, and adds a `/news/` prefix for collection identification.

**Alternatives considered:**
- `/news/:title/` — cleaner but loses date context; harder to browse chronologically
- `/news/:name/` — uses filename (with date prefix) as slug, ugly URLs like `/news/2026-01-20-title/`

### Decision 2: Keep layout: post

News entries continue using `layout: post`. The post layout is content-agnostic (title, date, content) and works without modification. A separate news layout would only add complexity with no current benefit. Can be added later if news-specific features (source attribution, different header) are needed.

### Decision 3: Remove categories and tags from news frontmatter

Since the `_news/` collection itself identifies the content type, both `categories: news` and `tags: [news]` become redundant. Removing them eliminates ~372 lines of unnecessary frontmatter across 186 files.

**Alternatives considered:**
- Keep `tags: [news]` for Jekyll's `site.tags` aggregation — not needed since tags aren't displayed on posts yet and the "news" tag adds no information

### Decision 4: No file renaming

Keep existing filenames (`YYYY-MM-DD-title.md`) in `_news/`. Jekyll collections don't require date prefixes, but keeping them maintains consistency and makes the migration purely a move+trim operation.

## Risks / Trade-offs

- **[Risk] Existing URLs change** → Mitigation: The old URLs were `/YYYY/MM/DD/title.html` (Jekyll post default). New URLs are `/news/YYYY/MM/DD/title/`. Old links will 404. News are ephemeral and not externally linked, so impact is minimal. If needed, redirects could be added later via jekyll-redirect-from plugin or a `_redirects` file.
- **[Risk] Home page breaks if `site.news` is empty** → Mitigation: The Liquid template already has `{% if news_posts.size > 0 %}` guards; same pattern applies to `site.news`.
- **[Risk] Collection not recognized by Jekyll** → Mitigation: Follows the exact same pattern as `_presentations/` which already works; GitHub Pages supports collections natively.

## Migration Plan

1. Add `news` collection to `_config.yml`
2. Create `_news/` directory and move all files from `_posts/news/`
3. Strip `categories: news` and `tags: [news]` from each moved file
4. Update `home.html` to use `site.news`
5. Update AGENTS.md
6. Build and verify locally or push and verify GitHub Pages build
