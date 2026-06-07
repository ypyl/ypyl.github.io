## Why

Tags were recently added to post pages and the home page post list, but they need refinement. On the post page, tags render on a separate line below the date — they belong inline after the date for a tighter, more editorial feel. On the home page, tags use the same size as post page tags — they should be visually lighter to avoid competing with post titles. The Kami design system defines three tag tiers; this change applies the appropriate tier to each context.

## What Changes

- **Post page**: Move tag badges inside the date line (`<p class="post-meta">`) with a `·` separator, using `display: inline-flex` to keep them on the same row
- **Home page**: Apply smaller, lighter tag styling via `.post-list .tag` override — reduced font size (0.625rem), tighter padding (2px 6px), and lighter background (`--tag-bg-light` = `#EEF2F7`)
- Tags remain visual-only (non-clickable), matching existing behavior

## Capabilities

### New Capabilities

- `tag-display-refinements`: Refine tag presentation to use context-appropriate Kami tiers: inline after date on post pages (default tier), smaller with lighter background on home page post list (lightest tier).

### Modified Capabilities

<!-- No existing specs are modified. This refines post-tag-display behavior but that spec hasn't been archived/synced yet. -->

## Impact

- `assets/css/style.css`: add `.post-list .tag` and `.post-meta .tags` overrides
- `_layouts/post.html`: move tag loop inside `<p class="post-meta">` with `·` separator
- No JavaScript, no new dependencies
