## Context

On mobile viewports (≤600px), the site navigation uses a hamburger menu with an absolute-positioned dropdown (`.site-nav` with `position: absolute` inside `.site-header` with `position: relative`). When expanded, the dropdown extends below the header bar into the main content area.

The home page's news carousel uses `.news-carousel.ready .news-carousel-views { position: relative; }` as a positioning anchor for the crossfade transition (inactive views use `position: absolute`).

Both `.site-header` and `.news-carousel-views` are positioned elements with `z-index: auto`. In CSS, positioned elements with `z-index: auto` stack in DOM order — later elements appear on top. Since `<main>` (containing the carousel) follows `<header>` in the DOM, the news cards cover the nav dropdown.

The only existing z-index in the stylesheet is `z-index: 100` on the scroll-to-top affix button. No other elements have explicit z-index values.

## Goals / Non-Goals

**Goals:**
- Mobile nav dropdown appears above news cards and all other main content
- Desktop layout unaffected
- Zero side effects on existing stacking behaviors (affix button, dark mode, etc.)

**Non-Goals:**
- Changing nav menu appearance or behavior
- Modifying the carousel crossfade mechanism
- Adding z-index to other elements

## Decisions

### Decision: Add `z-index: 10` to `.site-header`

**Chosen**: Add `z-index: 10;` to the `.site-header` rule.

```css
.site-header {
  position: relative;
  z-index: 10;   /* ← ADD */
  min-height: 56px;
  ...
}
```

**Rationale**: This creates a stacking context for the header, lifting the entire header (including its absolute-positioned nav dropdown) above the main content area. Since the header is above the content in normal document flow, `z-index` only affects overlapping positioned descendants (the nav menu) — there is no visual side effect on the header bar itself.

The value `10` was chosen because:
- It's above `z-index: auto` (0) — the default for all other elements
- It's below `z-index: 100` — the scroll-to-top button, which must remain on top
- It leaves room for future intermediate stacking layers (e.g., modals at 50)

**Alternatives considered**:

| Approach | Code | Verdict |
|----------|------|---------|
| `z-index` on `.site-nav` (mobile) | Add `z-index: 10` inside `@media (max-width: 600px)` for `.site-nav` | Works — since `.site-header` has `z-index: auto` (no stacking context), `.site-nav`'s z-index participates in the root context. But more verbose (media query) and less future-proof (other header children might need stacking fixes). |
| Remove `position: relative` from carousel | Change crossfade to not rely on positioning context | Much larger change. The absolute positioning is the correct approach for the crossfade — don't break a working pattern. |
| `z-index: 1` instead of 10 | `z-index: 1;` | Works identically. Using `10` is a non-semantic convention that leaves headroom for future intermediate layers. |

## Risks / Trade-offs

- **[None] Desktop unaffected**: The header already visually sits above content. `z-index` only changes the stacking of overlapping elements, and there is no overlap on desktop (the nav is inline, not a dropdown).
- **[None] Affix button unaffected**: The scroll-to-top button at `z-index: 100` remains highest. Verified by value comparison.
- **[None] No new stacking context conflicts**: No other elements in the stylesheet have explicit z-index values except the affix button.
