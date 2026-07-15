## Why

Readers sometimes want to copy the raw Markdown source of a post (e.g., for reference or remixing). Currently there's no direct path from a published post to its source file on GitHub — the reader has to manually navigate the repo. Adding a "View source" link in the post meta line gives one-click access to the raw Markdown on GitHub.

## What Changes

- Add a "View source" link to the post meta line in `_layouts/post.html`, linking to the post's Markdown file on GitHub (`blob` view)

## Capabilities

### New Capabilities

- `github-source-link`: Display a "View source" link in the post meta line that opens the post's Markdown source on GitHub

### Modified Capabilities

<!-- No existing specs to modify -->

## Impact

- `_layouts/post.html` — add one `<a>` element in the post header meta line
- No new dependencies, config changes, or API usage
