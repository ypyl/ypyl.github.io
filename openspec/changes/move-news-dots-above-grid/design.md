## Context

The news carousel on the home page currently positions navigation dots below the grid in both DOM order and visual layout. Since the recent dynamic-height fix, the grid height varies between views, causing the dots to shift vertically as users navigate. Placing dots above the grid gives them a fixed position and follows common carousel UX conventions where navigation indicators sit above or alongside content.

The change involves reordering the Liquid template in `_layouts/home.html` and adjusting CSS spacing.

## Goals / Non-Goals

**Goals:**
- Navigation dots appear visually above the news card grid
- Dots maintain a fixed vertical position regardless of which view is active
- Consistent spacing between dots and grid (equivalent to previous spacing below)
- No changes to dot behavior, styling, or interactivity

**Non-Goals:**
- Changing dot appearance (size, color, shape)
- Adding dot labels or text
- Changing the crossfade transition
- Modifying mobile or no-JS behavior

## Decisions

### Decision 1: Move dots in DOM order, not CSS-only repositioning

**Chosen**: Reorder the HTML in `_layouts/home.html` — put `.news-carousel-dots` before `.news-carousel-views`.

**Rationale**: CSS-only repositioning (e.g., `order`, `flex-direction: column-reverse`, or absolute positioning) would create a mismatch between DOM order and visual order, hurting accessibility and keyboard navigation. Reordering the DOM is the simplest, most correct approach. The Liquid template change is trivial — move one block above the other.

**Alternative considered**: CSS `order` property on flex children. Rejected because it disconnects tab order from visual order, breaking keyboard navigation for dots.

### Decision 2: Adjust spacing via margin swap

Currently `.news-carousel-views` has `margin-bottom: var(--space-md)` to create space between the grid and dots below. After reordering, this margin should be on the dots element (which is now above the grid).

**Change**: Remove `margin-bottom` from `.news-carousel-views` and add `margin-bottom: var(--space-md)` to `.news-carousel-dots` (when visible).

### Decision 3: Keep dots CSS identical otherwise

The `.news-carousel-dots` rule already has `padding: var(--space-sm) 0` (symmetric top/bottom) and `display: none` by default. These are fine as-is. The only addition is `margin-bottom` for spacing from the grid below.

## CSS Changes Summary

| Selector | Change |
|---|---|
| `.news-carousel.ready .news-carousel-views` | Remove `margin-bottom: var(--space-md)` |
| `.news-carousel-dots` (when visible) | Add `margin-bottom: var(--space-md)` |

## Risks / Trade-offs

- **[Low] Existing users may expect dots at the bottom**: This is a deliberate UX improvement. The change is subtle — navigation remains in the same visual area, just repositioned.
  → Mitigation: The dots remain visually prominent and easy to discover. This follows common carousel patterns.

- **[None] No functional regression**: Dot click handlers target elements by `data-dot` attribute, independent of DOM position. The JS is unaffected.
