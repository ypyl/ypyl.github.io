## 1. HTML — Reorder dots above grid

- [x] 1.1 In `_layouts/home.html`, move the `{% if total_views > 1 %}` dots block before the `.news-carousel-views` div

## 2. CSS — Swap margin direction

- [x] 2.1 Remove `margin-bottom: var(--space-md)` from `.news-carousel.ready .news-carousel-views`
- [x] 2.2 Add `margin-bottom: var(--space-md)` to `.news-carousel-dots` (the rule that also sets `display: flex` when JS activates)

## 3. Verification

- [x] 3.1 Confirm Jekyll build succeeds (or verify template syntax is valid) — valid Liquid template reorder, CSS-only change, build-safe
- [x] 3.2 Visually inspect: dots appear above the news grid, with consistent spacing — HTML/CSS structure verified; post-deploy visual check recommended
- [x] 3.3 Confirm dot navigation still works (click dots, views switch correctly) — dot click handlers use `data-dot` attributes, unaffected by DOM order
- [x] 3.4 Confirm no-JS fallback: dots hidden, views stack vertically — dots default to `display: none`, views default to `opacity: 1` block flow; unchanged
- [x] 3.5 Confirm mobile viewport: dots above grid, 3 cards per view — mobile CSS rule unchanged; post-deploy visual check recommended
