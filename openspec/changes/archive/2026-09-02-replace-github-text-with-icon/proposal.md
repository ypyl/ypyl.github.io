## Why

The header navigation currently shows a text link "GitHub" that links to the user's profile. Text links are visually heavier than icons and break the pattern set by other UI elements in the site (theme toggle uses an SVG icon, not text). Replacing the text with the standard GitHub octocat SVG icon creates visual consistency and saves horizontal space in the navigation bar.

## What Changes

- Replace the text "GitHub" in the header navigation with an inline SVG of the GitHub octocat logo
- The SVG will use `fill="currentColor"` to inherit text color from CSS
- Add a `title` attribute for accessibility on hover
- Adjust CSS to accommodate the icon (size, spacing, hover state)

## Capabilities

### Modified Capabilities

- `header-github-link`: The requirement changes from displaying text "GitHub" to displaying an SVG icon that links to the GitHub profile

## Impact

- **Files affected**: `_includes/header.html`, `assets/css/style.css`
- **No breaking changes**: The link still goes to the same URL, still uses `page-link` class
- **Accessibility**: SVG will have `aria-hidden="true"`, anchor will have descriptive `title` and `aria-label`
- **Responsive**: Icon works at all breakpoints; mobile nav already handles `page-link` display
