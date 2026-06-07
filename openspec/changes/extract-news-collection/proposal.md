## Why

News entries currently live as a subdirectory of `_posts/` and are distinguished from blog posts solely by `categories: news`. This forces the home page to iterate all 318 posts twice with Liquid filters, adds redundant `categories: news` and `tags: [news]` frontmatter to every news file, and conflates two distinct content types in one Jekyll namespace. Extracting news into their own `_news/` collection gives them first-class entity status, simplifies templates, and follows the pattern already established by `_presentations/`.

## What Changes

- Create a `_news/` Jekyll collection with `output: true` and date-aware permalink pattern
- Move all 186 files from `_posts/news/` to `_news/`
- Strip redundant `categories: news` and `tags: [news]` from news frontmatter
- Simplify `home.html` to use `site.news` directly instead of filtering `site.posts` by category
- Update `_config.yml` to register the new collection
- Update AGENTS.md to document `_news/` as a separate content type

## Capabilities

### New Capabilities

- `news-collection`: A Jekyll collection at `_news/` that stores short news entries, accessed via `site.news` in Liquid, rendered at `/news/:year/:month/:day/:title/` permalinks, and separated from blog posts in both filesystem and template logic.

### Modified Capabilities

<!-- No existing specs are modified. -->

## Impact

- 186 files moved from `_posts/news/` to `_news/` (frontmatter-only changes: remove two lines each)
- `_config.yml`: new `news` collection entry
- `_layouts/home.html`: replace category-filtering loops with direct `site.news` access
- `AGENTS.md`: document `_news/` in content structure section
- No CSS, layout, or design changes — news cards render identically
- RSS feed unaffected (news were not intended for feed)
