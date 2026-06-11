## Why

The scroll-to-top button (`.affix-button`) scales with browser zoom because its dimensions are defined in `px` units and browser zoom applies a uniform viewport transform. At 200% zoom — a common reading level — the 48px button balloons to 96px on screen, dominating the corner and feeling disproportionately large against the reading experience. The button should maintain a constant physical size regardless of zoom level, staying discreet and unobtrusive.

## What Changes

- Add a JavaScript zoom-detection mechanism that measures the actual rendered size of a known CSS unit (`1rem`) against its expected value, yielding the browser zoom ratio
- Apply an inverse CSS `transform: scale()` to the `.affix-button` element so it compensates for browser zoom and maintains a constant visual size
- Re-measure on window resize (browser zoom changes trigger resize events)
- Optionally reduce the baseline button size from 48px to 36px for a less prominent starting point

## Capabilities

### New Capabilities
- `scroll-button-zoom-lock`: JavaScript-based zoom compensation that keeps the fixed-position scroll-to-top button at a constant rendered size across all browser zoom levels, using a hidden-ruler measurement technique for cross-browser compatibility

### Modified Capabilities
<!-- None — no existing specs are affected. -->

## Impact

- **Affected files**: `_includes/affix.html` (add zoom-compensation JS), `assets/css/style.css` (optional: adjust baseline button size)
- **Dependencies**: None. Pure vanilla JS, no external libraries.
- **Risk**: Low. The button is a cosmetic UI element. If JS fails or is disabled, the button gracefully degrades to its normal zoom-scaling behavior.
