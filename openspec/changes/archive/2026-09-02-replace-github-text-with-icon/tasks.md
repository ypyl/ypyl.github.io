## 1. HTML Changes

- [x] 1.1 Replace the text "GitHub" in `_includes/header.html` with an inline SVG of the GitHub octocat logo, adding `title="GitHub"` and `aria-label="GitHub"` to the anchor. Verify: the rendered page shows an icon instead of text in the nav bar.

## 2. CSS Changes

- [x] 2.1 Add `.github-icon` class to `assets/css/style.css` with `width: 16px; height: 16px; vertical-align: middle;`. Verify: the icon renders at 16px and aligns with adjacent nav text.

## 3. Verification

- [x] 3.1 Test in both light and dark themes. Verify: the icon color matches other nav links and changes to `var(--brand)` on hover.
- [x] 3.2 Test at mobile breakpoint (`max-width: 600px`). Verify: the icon displays correctly in the mobile nav dropdown.
