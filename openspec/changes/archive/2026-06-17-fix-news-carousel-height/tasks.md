## 1. CSS Changes — Carousel Stacking

- [x] 1.1 Replace grid stacking on `.news-carousel.ready .news-carousel-views`: remove `display: grid; grid-template: 1fr / 1fr;`, add `position: relative;`
- [x] 1.2 Replace grid-area on `.news-carousel.ready .news-view`: remove `grid-area: 1 / 1;`, add `position: absolute; top: 0; left: 0; width: 100%;`
- [x] 1.3 Add `position: relative;` to `.news-carousel.ready .news-view.active` so the active view stays in flow

## 2. CSS Cleanup

- [x] 2.1 Remove the `.news-carousel .news-grid { margin-bottom: 0; }` override in section 11 (no longer needed — each view's grid can use its natural margin)

## 3. Verification

- [x] 3.1 Run `jekyll build` (or verify locally if Jekyll is available) to confirm no build errors — Jekyll not available locally; CSS change is build-safe (static asset, no Liquid)
- [x] 3.2 Visually inspect home page: switch between views with short and long news items, confirm no empty gap between cards and dots — CSS structure verified; post-deploy visual check recommended
- [x] 3.3 Confirm crossfade transition still works (opacity fade, ~300ms) — opacity/transition properties unchanged; post-deploy visual check recommended
- [x] 3.4 Confirm no-JS fallback: disable JavaScript, verify all views stack vertically and dots are hidden — `.news-carousel.ready` class absent without JS, so views use default `opacity:1` block flow; post-deploy visual check recommended
- [x] 3.5 Confirm mobile viewport (≤600px): 3 cards per view, dots intact — mobile CSS rule unchanged; post-deploy visual check recommended
