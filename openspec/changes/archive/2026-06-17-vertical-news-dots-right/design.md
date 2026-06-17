## Context

The news carousel on the home page currently displays navigation dots as a horizontal bar above the 3-column news card grid. This was recently changed from "below" to "above" in `move-news-dots-above-grid`. The carousel uses absolute positioning for inactive views (from `fix-news-carousel-height`) so active-view height varies. Moving dots to a vertical column on the right side gives them a fixed position independent of view height, follows side-indicator carousel patterns, and uses horizontal whitespace efficiently.

The change involves HTML reordering, flexbox layout on the carousel container, and a mobile media query to restore horizontal dots.

## Goals / Non-Goals

**Goals:**
- On desktop (≥601px), dots render as a vertical column to the right of the news card grid
- Dots maintain fixed position — no vertical shift as active view changes
- On mobile (≤600px), dots revert to a horizontal row below the cards
- DOM order matches visual order (views → dots) for accessibility
- Dot click behavior, crossfade, and no-JS fallback all preserved

**Non-Goals:**
- Changing dot appearance (size, shape, color)
- Changing the number of dots or views
- Adding scroll behavior or auto-rotation
- Modifying the card grid columns or responsive breakpoints

## Decisions

### Decision 1: Flexbox row on `.news-carousel` for desktop layout

**Chosen**: `display: flex; flex-direction: row; align-items: flex-start; gap: var(--space-lg)` on `.news-carousel` for viewports above 600px.

**Rationale**: Flexbox naturally splits the container into two adjacent regions — views on the left (flex: 1), dots on the right (flex-shrink: 0). The `align-items: flex-start` keeps both children top-aligned. This is simpler than CSS Grid for a two-column split and handles the vertical dot stacking within the second column trivially.

**Alternative considered**: CSS Grid with `grid-template-columns: 1fr auto`. Works identically but adds unnecessary complexity for a simple side-by-side layout.

### Decision 2: Vertical dot stacking via `flex-direction: column`

**Chosen**: `.news-carousel-dots` uses `flex-direction: column; align-items: center;` to stack dots vertically with equal spacing.

**Rationale**: The dot buttons are already 22×22px with 10px pseudo-element circles. Stacking them vertically is a natural fit — no change to the dots themselves, just the container direction. The `gap: var(--space-md)` spacing (8px) between dots carries over from the horizontal layout.

### Decision 3: Views before dots in DOM order

**Chosen**: Reverse the recent HTML order change — `.news-carousel-views` first, `.news-carousel-dots` second.

**Rationale**: Visual order should match DOM order for keyboard navigation and screen readers. With dots on the right, the natural reading order is: grid content → navigation indicator. This also matches the flexbox visual layout (first child = left, second child = right).

### Decision 4: Mobile fallback — horizontal dots below

**Chosen**: At ≤600px, use a media query to override the flexbox row on `.news-carousel` and the column direction on dots:

```css
.news-carousel { flex-direction: column; }
.news-carousel-dots { flex-direction: row; justify-content: center; }
```

**Rationale**: At single-column card layout, a vertical dot column wastes horizontal space and creates an awkward tall-narrow layout. Switching to a horizontal bar below the cards uses the available width and follows the original carousel pattern on mobile.

## Risks / Trade-offs

- **[Low] Cards become slightly narrower on desktop**: The dot column consumes ~30px of width. On a 900px max-width container with 3 columns, this is a ~3% reduction per card. Negligible visual impact.
  → Mitigation: The `gap` between carousel regions ensures consistent spacing.

- **[Low] Mobile DOM order**: Views come before dots in DOM. On mobile where dots go below, this matches naturally (cards above dots). No reordering needed.
