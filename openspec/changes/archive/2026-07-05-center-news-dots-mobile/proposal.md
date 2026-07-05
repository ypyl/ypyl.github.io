## Why

On mobile viewports (≤600px), the news carousel's navigation dots are left-aligned below the single-column card grid. The `news-carousel-vertical-dots` spec explicitly requires them to be "centered below the news card grid." This is a visual implementation bug — the CSS produces left-aligned dots that don't match the specification or standard carousel UX patterns.

## What Changes

- Add `width: 100%` to `.news-carousel-dots` within the existing mobile media query (≤600px), so the flex container stretches full-width and the already-present `justify-content: center` properly centers the dots.

## Capabilities

### New Capabilities

None — this is a CSS bug fix, not a new feature.

### Modified Capabilities

None — the `news-carousel-vertical-dots` spec already requires centering on mobile. No spec-level requirement changes.

## Impact

- `assets/css/style.css` — one line added inside the existing `@media screen and (max-width: 600px)` block
