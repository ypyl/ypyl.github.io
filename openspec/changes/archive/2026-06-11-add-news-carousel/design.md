## Context

The home page (`_layouts/home.html`) renders news via a static 3-column CSS grid showing exactly 6 cards. With 194+ news items this limits surface content. The site uses the Kami design system (warm parchment palette, serif hierarchy, ink-blue accent, whisper shadows) with all custom CSS in `assets/css/style.css`. No JS directory exists — all scripting is inline in layouts (e.g., theme toggle). Jekyll builds statically with GitHub Pages.

## Goals / Non-Goals

**Goals:**
- Show up to 30 most recent news items on the home page via manual dot navigation
- 5 views max, 6 cards per view on desktop, fewer on mobile
- Dynamic dot count — fewer items means fewer dots, ≤6 items means no carousel at all
- Crossfade transition between views (opacity, ~300ms)
- Follow Kami design tokens for all new UI elements
- Degrade gracefully: all views stack vertically if JS is disabled

**Non-Goals:**
- Auto-scroll / auto-play
- Swipe / touch gestures (clicks only)
- Prev/Next arrow buttons
- URL fragment or history state changes on navigation
- Infinite scroll or "load more" patterns
- Changes to individual news card styling

## Decisions

### 1. CSS Grid Stacking for View Switching

All 5 views occupy the same CSS grid cell (`grid-area: 1 / 1`). Only the active view has `opacity: 1` and `pointer-events: auto`. Inactive views have `opacity: 0` and `pointer-events: none`. This ensures the container height matches the active view and enables a smooth crossfade.

**Alternatives considered:**
- *`display: none` / `display: grid` toggle*: No transition possible, jarring swap.
- *`position: absolute` layering*: Requires JS to set container height, more fragile.
- *Horizontal scroll-snap*: Unnatural for a 2D grid of cards, violates "not a scroll" intent.

### 2. Minimal Inline JavaScript

A script of ~10 lines handles dot clicks: remove `.active` from all dots and views, add `.active` to the clicked dot and matching `[data-view]` element. No framework, no external file, no build step.

**Alternatives considered:**
- *CSS-only radio button hack*: Requires 5 explicit selector chains, fragile to layout changes, dots must be DOM siblings in specific position. Rejected for maintainability.
- *`:target` with anchors*: Changes URL fragment, causes page jumps, initial load shows all or none. Rejected for UX.

### 3. Jekyll Liquid Rendering

The template iterates over `site.news` sorted by date, rendering all HTML for 30 items (5 groups of 6). Each group is wrapped in a `<div class="news-view">` with an `active` class on the first view. Dots are rendered as `<button>` elements. The markup handles variable counts: if there are only 12 items, 2 views and 2 dots render.

```
{%- assign views = news_items.size | divided_by: 6.0 | ceil | at_most: 5 -%}
```

This computes view count dynamically, capped at 5.

### 4. Mobile Adaptation

On screens ≤600px, the grid becomes single-column and cards per view reduces to 3 (via showing only the first 3 cards in each view group). This means mobile users see up to 15 news items across 5 views rather than 30 across 5 views of 6 cards. The same Liquid loop renders all cards; CSS `:nth-child(n+4)` hides cards 4–6 within each view on mobile.

**Alternative considered:**
- *4 cards per mobile view*: Still quite tall (4 cards + dots ≈ one full viewport). 3 felt better balanced.
- *Varying view count on mobile*: Changing the number of views between screen sizes requires JS or complex CSS. Keeping 5 max views with fewer cards per view is simpler.

### 5. No-Script Fallback

If JS fails to load or is disabled, all `.news-view` elements lose their `opacity: 0` styling (controlled by `.active` class which isn't applied). The views stack vertically — all 30 cards become visible as one long grid. Dots are hidden (`display: none` by default, JS makes them visible). The page remains fully readable.

### 6. Dots as Buttons

Dots are `<button>` elements, not `<a>` links or `<span>` elements. This is semantically correct (they trigger an action, not navigation) and accessible (focusable, keyboard-operable). Each button gets an `aria-label="News page N"`.

## Risks / Trade-offs

**[Risk] 30 cards increase page size** → Each card is ~200 bytes of HTML. 30 cards ≈ 6KB uncompressed. Negligible impact given the site's overall size and gzip compression. Mitigation: none needed.

**[Risk] Mobile: 3 cards per view might feel limiting** → Users can click dots to see more. The trade-off prioritizes avoiding excessive vertical scrolling. If feedback indicates 4 is preferred, it's a one-line CSS change.

**[Trade-off] Fade transition means no directional feedback** → Unlike a slide, the fade doesn't indicate "which way" you went. With only 5 views and no auto-scroll, this is acceptable — users click a specific dot, not "next/prev."

## Open Questions

None — all design decisions resolved.
