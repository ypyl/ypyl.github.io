## Context

The news carousel uses a flexbox layout that adapts between desktop and mobile via a `max-width: 600px` media query. On desktop, the carousel is a row layout (views left, vertical dots right). On mobile, it switches to a column layout (views top, horizontal dots bottom).

The base `.news-carousel` rule sets `align-items: flex-start`. This is correct for the desktop row layout (keeps views and dots top-aligned). However, on mobile where `flex-direction` becomes `column`, this same `align-items: flex-start` controls the *cross-axis* (horizontal), causing child elements to shrink-wrap to content width and align left.

The `.news-carousel-dots` container — only as wide as its 3 small dot buttons plus gaps (~82px) — gets pinned to the left. The existing `justify-content: center` on the dots container has no visible effect because there's no extra horizontal space to distribute within a shrink-wrapped container.

The `news-carousel-vertical-dots` spec already requires mobile dots to be centered. This is purely a CSS bug.

## Goals / Non-Goals

**Goals:**
- Center the navigation dots horizontally on mobile viewports (≤600px)
- Zero side effects on desktop layout
- Minimal change — one CSS line

**Non-Goals:**
- Changing dot appearance, count, or click behavior
- Modifying the crossfade transition or view switching
- Altering the desktop layout in any way

## Decisions

### Decision: Add `width: 100%` to `.news-carousel-dots` on mobile

**Chosen**: Add `width: 100%` to the `.news-carousel.ready .news-carousel-dots` rule inside the `@media screen and (max-width: 600px)` block.

```css
.news-carousel.ready .news-carousel-dots {
  flex-direction: row;
  justify-content: center;
  margin-top: var(--space-md);
  width: 100%;   /* ← ADD */
}
```

**Rationale**: The dots container already has `justify-content: center` set. By adding `width: 100%`, the container stretches to full width of its parent, giving `justify-content: center` the horizontal space it needs to actually center the dots. This is the most targeted fix — it doesn't touch the parent's flexbox alignment (`align-items`) and can't affect any other child element.

**Alternatives considered**:

| Approach | Code | Verdict |
|----------|------|---------|
| `align-self: center` on dots | `align-self: center;` | Also valid, equally minimal. But `width: 100%` is more idiomatic for "stretch then center content" patterns. |
| `align-items: center` on parent | Change `.news-carousel` to `align-items: center` on mobile | Riskier — in a column layout, this centers ALL children cross-axis. Could theoretically shrink the views container from full width (though in practice the grid content forces full width). Unnecessary scope. |
| `margin: 0 auto` on dots | `margin-left: auto; margin-right: auto;` | Works for block-level centering but the dots container is `display: flex` (not block). `margin: auto` in flexbox has different semantics and may not produce the desired result. |
| `justify-items` on parent grid | Switch parent to CSS Grid | Overkill for a one-line fix. The existing flexbox layout is correct for all other aspects. |

## Risks / Trade-offs

- **[None] Desktop unaffected**: The change is scoped inside `@media (max-width: 600px)`, which only activates on mobile. Desktop dots remain vertically stacked on the right, unchanged.
- **[None] No JS dependency**: Pure CSS fix, no JavaScript changes needed.
- **[None] No-JS fallback untouched**: When JS is disabled, `.news-carousel-dots` is `display: none` via CSS, so the `width: 100%` has no effect.
