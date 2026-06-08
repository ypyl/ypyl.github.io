## Context

The scroll-to-top button (`_includes/affix.html`) is a fixed-position `<button>` with pixel-based sizing (`48px × 48px`). Browser zoom (Ctrl+/-) applies a viewport-level transform that uniformly scales all CSS `px` values — there is no CSS property to opt an individual element out of this behavior. At 200% zoom the button renders at 96px on screen, which is disproportionately large for a utility control.

The existing button uses a vanilla JS scroll listener for show/hide behavior. The zoom-compensation logic extends this same script block without adding external dependencies.

## Goals / Non-Goals

**Goals:**
- Keep the scroll-to-top button at a constant visual size (~48px physical) at all browser zoom levels
- Work cross-browser (Chrome, Firefox, Safari, Edge)
- Gracefully degrade if JS is disabled (button scales normally — no worse than today)
- Avoid flicker on page load (button should appear at correct size from first paint)

**Non-Goals:**
- Lock the button's *position* against zoom (bottom/right offset will naturally shrink as zoom increases — this is acceptable and arguably desirable)
- Perfect precision at fractional zoom levels (±5% tolerance is fine)
- Handle mobile pinch-zoom (the blog is primarily desktop-read; pinch-zoom behavior is undefined)
- Solve zoom-locking for any other UI element

## Decisions

### D1: Zoom detection via devicePixelRatio with baseline

**Choice**: Record `window.devicePixelRatio` at page load as a baseline (`baseDPR`), then on each measurement divide the current `devicePixelRatio` by the baseline to get the zoom ratio.

**Why this works**: In Chrome, Edge, and Safari, `devicePixelRatio` changes proportionally with browser zoom (Ctrl+/-). By ratioing against a baseline captured at page load, retina displays are automatically normalized — a MacBook with DPR=2 at 100% zoom has `baseDPR=2` and at 200% zoom has `currentDPR=4`, yielding a correct zoom ratio of 2.

**Alternatives considered**:

| Method | Why rejected |
|---|---|
| Hidden 1rem ruler | Both `getComputedStyle` fontSize and `offsetWidth` return CSS pixels, which scale uniformly with browser zoom. The ratio is always 1.0 — no zoom information is captured. |
| `visualViewport.scale` | Firefox doesn't support the `scale` property. Chrome uses it for pinch-zoom but not consistently for Ctrl+/-. |
| `outerWidth / innerWidth` | Ratio depends on window chrome size (title bar, borders), varies by OS and browser, and changes on window resize. Unreliable. |

**Firefox limitation**: Firefox keeps `devicePixelRatio` constant regardless of Ctrl+/- zoom. In Firefox the zoom ratio is always 1 and the button scales normally with the page — graceful degradation to today's behavior.

### D2: Baseline button size stays at 48px

**Choice**: Keep the CSS `width: 48px; height: 48px` unchanged.

**Rationale**: The zoom-compensation JS normalizes the rendered size back to 48px physical at all zoom levels. Reducing the CSS size would make the button smaller at 100% zoom but would still require the JS fix to prevent scaling at higher zoom. Keeping 48px ensures a comfortable touch/click target at the "normal" zoom level where most interaction happens.

### D3: Apply inverse scale via transform, not width/height override

**Choice**: Use `transform: scale(1/zoomRatio)` rather than dynamically setting `width`/`height`.

**Rationale**:
- `transform` doesn't trigger layout reflow — only a compositor-layer repaint
- The SVG icon inside the button scales proportionally without separate handling
- `transform-origin: bottom right` keeps the button anchored to its corner position as it scales

### D4: visibility:hidden pre-measurement to prevent flicker

**Choice**: Start the button with `visibility: hidden`, measure zoom on first script execution, apply the inverse transform, then set `visibility: visible`. The existing opacity-based show/hide logic is unchanged — the button only becomes visible when `scrollY > 200px`.

**Alternative**: Start visible and accept one-frame flicker. Rejected because the flicker is noticeable on slower connections.

### D5: Re-measure on resize only (not scroll)

**Choice**: Attach the zoom recalculation to `window.resize` with `{passive: true}`. Do not recalculate on scroll.

**Rationale**: Browser zoom changes always trigger a resize event. Scrolling does not. Debouncing is unnecessary — `resize` fires at most a few times per zoom step, and the measurement is trivial (single DOM read + style write).

## Risks / Trade-offs

| Risk | Mitigation |
|---|---|
| **Fractional zoom blur**: At non-integer inverse scales (e.g., scale(0.75) for 133% zoom), the button may render with slight subpixel blur | Acceptable. At 48px the blur is imperceptible. The SVG icon re-renders cleanly at any scale. |
| **JS disabled**: No zoom compensation applied, button scales with page | Graceful degradation — same behavior as today. This is an enhancement, not a requirement. |
| **Ruler measurement fails**: If `getComputedStyle` returns an unexpected value (e.g., `font-size: medium`) | Fallback to `scale(1)` (no compensation). Add a `try/catch` around the measurement. |
| **Position drift**: `bottom`/`right` offsets still scale with zoom — at 200% zoom the button hugs the corner ~8px closer | Acceptable behavior. The button stays near the corner; precise positioning is not critical. |
| **Interaction with existing hover transform**: The existing `:hover { transform: translateY(-2px) }` will be overridden by the inline JS `transform: scale(...)` | Fix: move the hover lift to a wrapper element, or use `translateY` on the button and let the JS compensate by setting `transform` on a wrapper span. See implementation notes. |
