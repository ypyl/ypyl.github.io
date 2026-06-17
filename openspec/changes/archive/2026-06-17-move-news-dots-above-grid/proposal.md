## Why

The navigation dots are currently positioned below the news grid. This creates a UX disconnect: users must scroll past the card content to discover the navigation controls. Moving dots above the grid puts navigation where users expect it — similar to tab bars, carousel indicators, and section headers. It also keeps the dots in a fixed visual position regardless of which view is active (the grid height varies, so dots below shift vertically between views).

## What Changes

- Move the `.news-carousel-dots` HTML block before `.news-carousel-views` in `_layouts/home.html`
- Adjust CSS spacing: remove `margin-bottom` from `.news-carousel-views` and add `margin-bottom` to `.news-carousel-dots` so spacing between dots and grid is maintained
- No JavaScript changes — dot click handlers are unaffected by DOM order
- No visual style changes to dots or cards themselves

## Capabilities

### New Capabilities

None. This is a layout refinement, not a new feature.

### Modified Capabilities

- `news-carousel`: The requirement "Dot navigation controls view switching" currently states dots are positioned "below the news grid." This requirement is modified to position dots above the news grid.

## Impact

- **HTML**: `_layouts/home.html` — reorder `.news-carousel-dots` block before `.news-carousel-views`
- **CSS**: `assets/css/style.css` — swap margin direction on `.news-carousel-views` and add spacing to `.news-carousel-dots`
- **No breaking changes**. Dot navigation, crossfade, mobile adaptation, and no-JS fallback all function identically.
