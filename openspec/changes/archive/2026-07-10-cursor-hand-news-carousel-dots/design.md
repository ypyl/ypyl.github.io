## Context

The news carousel on the home page uses `<button>` elements as navigation dots. These dots are styled as circles using `::after` pseudo-elements with the actual visible dot. The parent container `.news-carousel-dots` currently sets `cursor: default`, which cascades to the child `.news-dot` buttons, suppressing the browser's default pointer cursor for interactive elements.

The fix is a single CSS property addition — no architectural changes required.

## Goals / Non-Goals

**Goals:**
- Show `cursor: pointer` (hand) when hovering over any news carousel navigation dot

**Non-Goals:**
- No changes to dot styling, sizing, colors, or layout
- No changes to click behavior, transitions, or JavaScript
- No new HTML structure or assets

## Decisions

**Decision: Add `cursor: pointer` directly to `.news-dot` rather than changing `.news-carousel-dots`**

- **Rationale**: The `.news-carousel-dots` container includes padding/gap areas between dots where a default cursor is appropriate. Overriding at the `.news-dot` level ensures only the clickable button area shows the pointer cursor.
- **Alternative considered**: Removing `cursor: default` from `.news-carousel-dots` entirely. Rejected because the gap/spacing areas between dots are not interactive and should retain the default cursor.

## Risks / Trade-offs

- **Risk**: None. This is a purely additive CSS declaration with no side effects.
