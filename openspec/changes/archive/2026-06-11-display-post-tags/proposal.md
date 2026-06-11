## Why

Post tags were normalized to YAML array format but remain invisible — they aren't rendered anywhere on the site. The tools page already displays tags as visual badges, proving the pattern works. With the data now clean and consistent, surfacing tags on post pages and the home page helps readers understand a post's topic at a glance and adds visual texture to the site.

## What Changes

- Move `.tag` / `.tags` CSS from `tools.css` to `style.css` to make tag styles globally available
- Add tag badges to the post page header (below date, before content)
- Add tag badges to home page post list items (below post title)
- Skip rendering tags when a post has no tags (no empty badge containers)
- Tags are visual-only (non-clickable), matching the tools page behavior

## Capabilities

### New Capabilities

- `post-tag-display`: Render tag badges on blog post pages and home page post list items using the existing Kami tag style (uppercase sans-serif badges), globally available via `style.css`.

### Modified Capabilities

<!-- No existing specs are modified. -->

## Impact

- `assets/css/style.css`: add `.tags` and `.tag` styles (moved from `tools.css`)
- `_layouts/post.html`: add tag loop in post header
- `_layouts/home.html`: add tag loop in post list items
- No new dependencies, no API changes, no JavaScript
