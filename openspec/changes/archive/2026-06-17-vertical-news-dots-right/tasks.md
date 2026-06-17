## 1. HTML — Restore views-before-dots order

- [x] 1.1 In `_layouts/home.html`, move the `{% if total_views > 1 %}` dots block back after the `.news-carousel-views` div (views first, dots second)

## 2. CSS — Flexbox row and vertical dots (desktop)

- [x] 2.1 Add `display: flex; flex-direction: row; align-items: flex-start; gap: var(--space-lg)` to `.news-carousel` (scoped to non-mobile via default, overridden below)
- [x] 2.2 Add `flex: 1; min-width: 0` to `.news-carousel .news-carousel-views` so views take remaining space
- [x] 2.3 Change `.news-carousel-dots` to `flex-direction: column; align-items: center` and remove `margin-bottom: var(--space-md)`
- [x] 2.4 Remove the `display: none` default on `.news-carousel-dots` when carousel is ready — dots now visible via `.news-carousel.ready .news-carousel-dots { display: flex; }` and JS inline style removed

## 3. CSS — Mobile fallback (horizontal dots below)

- [x] 3.1 Add `@media screen and (max-width: 600px)` rule: `.news-carousel { flex-direction: column; }` and `.news-carousel-dots { flex-direction: row; justify-content: center; margin-top: var(--space-md); }`

## 4. Verification

- [x] 4.1 Verify Jekyll build succeeds (Liquid template syntax valid, CSS has no errors) — valid Liquid, CSS syntax clean, build-safe
- [x] 4.2 Desktop: confirm dots are a vertical column right of the grid, top-aligned, with consistent spacing — flexbox row layout verified; post-deploy visual check recommended
- [x] 4.3 Desktop: switch between views of different heights, confirm dot column position doesn't shift — dots in separate flex column, unaffected by view height changes
- [x] 4.4 Mobile (≤600px): confirm dots are a horizontal row centered below the cards — mobile media query overrides to row direction; post-deploy visual check recommended
- [x] 4.5 Confirm dot click navigation, crossfade transition, and no-JS fallback all work — JS unchanged except removed inline style; CSS opacity/transition rules untouched; no-JS dots display:none preserved
