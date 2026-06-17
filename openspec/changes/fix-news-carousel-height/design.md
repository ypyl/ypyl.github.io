## Context

The home page news carousel renders up to 5 views, each containing 6 news cards in a 3-column grid. Currently, all views are stacked in the same CSS Grid cell (`grid-area: 1 / 1`) inside `.news-carousel-views`. This forces the grid row to the height of the tallest child view, creating empty space when a shorter view is active.

The site follows the Kami design system (warm parchment, ink-blue accent, whisper shadows, serif hierarchy). Design tokens are in `assets/css/style.css`, sections 2 and 11/11b.

The change is purely CSS — no HTML structure or JavaScript changes.

## Goals / Non-Goals

**Goals:**
- The carousel container height matches the natural height of the currently active view
- No empty gap between the last row of news cards and the navigation dots
- Crossfade transition (opacity, 300ms ease) preserved exactly as-is
- No-JavaScript fallback (views stack vertically) preserved exactly as-is
- No changes to HTML markup or JavaScript

**Non-Goals:**
- Animated/smooth height transition between views (instant height switch is acceptable and expected)
- Changes to card layout, grid columns, or responsive breakpoints
- Changes to dot navigation behavior or styling
- Any server-side or Jekyll template changes

## Decisions

### Decision 1: Use `position: absolute` / `position: relative` instead of CSS Grid stacking

**Chosen**: Replace grid stacking with absolute positioning for inactive views.

**Rationale**: CSS Grid row sizing is inherently based on the tallest item in the row. When all views share `grid-area: 1 / 1`, the row is sized to fit the maximum — there is no CSS-only way to make a grid row match only one child. Taking inactive views out of flow with `position: absolute` solves this cleanly: only the `position: relative` active view contributes to parent height.

**Alternatives considered**:
- **JS height measurement**: Sets container height explicitly via `scrollHeight`. More fragile (needs resize observer for responsive reflows), more code, and the site philosophy favors CSS solutions.
- **`display: none` toggling**: Would break the crossfade entirely (no transition on `display`).
- **`visibility: hidden` with absolute positioning**: Same effect as chosen approach but `visibility` interferes with `pointer-events` behavior.

### Decision 2: Keep `.news-carousel-views` as a container with `position: relative`

The parent retains `position: relative` to serve as the positioning context for absolutely-positioned inactive views. The active view uses `position: relative` (or `static`) to remain in flow and determine the container height.

### Decision 3: Preserve existing transition properties

The `opacity` and `pointer-events` toggle on `.active` class is preserved verbatim. The transition remains `opacity 0.3s ease` on `.news-view`. No new transitions are added — the height change on view switch is instant (the browser batches the class changes in a single JS tick, so the old view snaps to `absolute` and the new view snaps to `relative` simultaneously).

### Decision 4: No changes to mobile or no-JS behavior

- Mobile: Cards 4–6 hidden via `display: none` at ≤600px — still works since it's scoped to `.news-card:nth-child(n+4)` within `.news-view`.
- No-JS fallback: When `.news-carousel.ready` class is absent, `.news-view` defaults to `opacity: 1; pointer-events: auto` with no grid stacking. This is unchanged — views stack vertically as before.

## CSS Changes Summary

Only two files are affected, both in `assets/css/style.css`:

**Section 11b (News Carousel)** — modify `.news-carousel.ready .news-carousel-views` and `.news-carousel.ready .news-view`:

| Selector | Before | After |
|---|---|---|
| `.news-carousel.ready .news-carousel-views` | `display: grid; grid-template: 1fr / 1fr; margin-bottom: var(--space-md);` | `position: relative; margin-bottom: var(--space-md);` |
| `.news-carousel.ready .news-view` | `grid-area: 1 / 1; opacity: 0; pointer-events: none; transition: opacity 0.3s ease;` | `position: absolute; top: 0; left: 0; width: 100%; opacity: 0; pointer-events: none; transition: opacity 0.3s ease;` |
| `.news-carousel.ready .news-view.active` | `opacity: 1; pointer-events: auto;` | `position: relative; opacity: 1; pointer-events: auto;` |

**Section 11 (News Grid)** — remove the now-unnecessary `.news-carousel .news-grid { margin-bottom: 0; }` rule, as it was only needed to counteract the default `.news-grid` margin when the grid was inside the carousel stacking context. With absolute positioning, the `.news-grid` inside each `.news-view` can keep its natural `margin-bottom`.

## Risks / Trade-offs

- **[Low] Height "snap" on view switch**: The container height jumps instantly to match the new view, with no transition animation. This is intentional and actually the desired behavior — the page shouldn't hold onto empty space. The opacity crossfade on content provides a smooth visual cue.
  → Mitigation: Documented as intentional. Could be enhanced later with a `transition: height` if requested.
- **[Low] No visible regression**: All views are visually identical to before — the only difference is the container height behavior. Existing functional tests for dot navigation, crossfade, and mobile adaptation remain valid.
  → Mitigation: Manual visual verification across 2–3 view switches confirms the fix.
