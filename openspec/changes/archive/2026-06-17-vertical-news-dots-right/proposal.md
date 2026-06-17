## Why

The horizontal dot bar above the news grid creates a visual disconnect: dots shift vertically as grid height changes between views, and they consume vertical space that pushes content down. A vertical dot column anchored to the right of the grid solves both problems — dots stay in a fixed position regardless of active view height, and they use the natural horizontal whitespace rather than competing for vertical real estate. This follows the side-indicator pattern common in gallery carousels and media players.

## What Changes

- Move dots from a horizontal bar above the grid to a vertical column on the right side of the news cards
- Reorder HTML: views first, dots second (matching visual order for accessibility)
- Convert `.news-carousel` to a flex row layout (views take remaining space, dots form a narrow column)
- Change `.news-carousel-dots` from horizontal (`flex-direction: row`, `justify-content: center`) to vertical (`flex-direction: column`)
- On mobile (≤600px), switch dots back to a horizontal row below the cards
- Remove the `margin-bottom` spacing on dots (no longer needed in vertical mode)

## Capabilities

### New Capabilities

- `news-carousel-vertical-dots`: Dots render as a vertical column anchored to the right of the news grid on desktop, switching to a horizontal row on mobile viewports.

### Modified Capabilities

- `news-carousel`: The "Dot navigation controls view switching" requirement is modified — dots position changes from "above the news grid" to "to the right of the news grid on desktop, below on mobile."

## Impact

- **HTML**: `_layouts/home.html` — reverse order of dots and views blocks (views first, dots after)
- **CSS**: `assets/css/style.css` — add flexbox to `.news-carousel`, change dot layout direction, add mobile horizontal fallback, remove obsolete `margin-bottom` from dots
- **No breaking changes** to dot navigation, crossfade, or no-JS fallback
