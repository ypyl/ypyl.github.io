## Why

Post tags are currently invisible — they aren't rendered on the main page, post pages, or news cards. Before tags can be displayed, the underlying data must be normalized: ~300 posts use inconsistent tag formats (comma-separated, space-separated, YAML arrays, single strings), and Jekyll's official spec only supports YAML lists and space-separated strings. Comma-separated tags are actively broken in Jekyll, producing a single tag containing literal commas. Normalizing to a single canonical format unblocks tag display and future tag-based features (filtering, tag pages, RSS category tags).

## What Changes

- Normalize all post tag frontmatter to YAML array format: `tags: [tag1, tag2]`
- Fix ~30 comma-separated posts that are currently broken in Jekyll (produce one Frankenstein tag)
- Optionally review ~80 space-separated posts that Jekyll splits on spaces (may split intended multi-word tags like "machine learning")
- Update AGENTS.md to mandate YAML array format for post tags going forward

## Capabilities

### New Capabilities

- `post-tag-normalization`: Standardize all `_posts/*.md` and `_posts/news/*.md` tag frontmatter to use YAML array format (`tags: [tag1, tag2]`), ensuring Jekyll correctly produces iterable tag arrays for Liquid templates.

### Modified Capabilities

<!-- No existing specs are modified. This is purely a data cleanup change. -->

## Impact

- Affects ~310 files in `_posts/` and `_posts/news/` (frontmatter-only changes)
- No layout, CSS, or template changes (display work is a separate follow-up)
- AGENTS.md tag guidelines need updating to mandate YAML array format
