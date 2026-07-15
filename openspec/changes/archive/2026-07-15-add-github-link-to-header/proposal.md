## Why

Visitors have no quick way to reach the site's GitHub repository from the header navigation. Adding a "GitHub" link after the existing "Tools" link gives readers one-click access to the source code, issues, and project history.

## What Changes

- Add a "GitHub" link to the site header navigation, after the dynamically-generated page links, pointing to `https://github.com/{{ site.github_username }}`

## Capabilities

### New Capabilities

- `header-github-link`: Display a "GitHub" link in the site header navigation that points to the site's GitHub repository

### Modified Capabilities

<!-- No existing specs to modify -->

## Impact

- `_includes/header.html` — add one `<a>` element in the `.trigger` nav div after the page loop
- No new dependencies, config changes, or API usage
