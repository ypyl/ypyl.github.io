## Why

The news carousel on the home page uses CSS Grid stacking (`grid-area: 1 / 1`) to overlay all views for crossfading. This forces the grid container to the height of the tallest view in the collection. When a shorter view is active, a large empty gap appears between the news cards and the navigation dots — the container remains sized for the tallest view even though the visible content is much shorter. News items vary significantly in excerpt length (from 1–2 lines to full paragraphs), so this gap is visually jarring and wastes space.

## What Changes

- Replace the CSS Grid stacking approach for news views with a `position: absolute` / `position: relative` pattern
- Inactive views are taken out of flow (`position: absolute`), so only the active view (`position: relative`) contributes to the container height
- The crossfade transition (opacity 0 → 1, 300ms ease) is preserved
- No JavaScript changes needed — the existing dot-click handler just toggles `.active` classes
- No-JavaScript fallback (vertically stacked views) is preserved

## Capabilities

### New Capabilities

- `news-carousel-dynamic-height`: The carousel container height dynamically matches the currently active view's natural content height, eliminating empty space between content and navigation dots.

### Modified Capabilities

None. Existing crossfade, dot navigation, mobile adaptation, and no-JS fallback requirements are unchanged. This is a CSS-only implementation refinement.

## Impact

- **CSS**: `assets/css/style.css` — sections 11 (News Grid) and 11b (News Carousel). Remove `display: grid` / `grid-area: 1 / 1` from `.news-carousel-views` and `.news-view`, replace with `position: relative` / `position: absolute` pattern.
- **HTML/JS**: No changes. `_layouts/home.html` structure and inline script remain identical.
- **No breaking changes**. All existing functionality works as before; only the container height behavior improves.
