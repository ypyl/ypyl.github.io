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

### D1: Zoom detection via outerWidth / innerWidth ratio

**Choice**: Use `window.outerWidth / window.innerWidth` as the zoom ratio.

**Why this works**: `window.outerWidth` reports in device pixels and does NOT change with Ctrl+/- browser zoom. `window.innerWidth` reports in CSS pixels and DOES scale with zoom. Their ratio directly yields the browser zoom factor — a maximized 1920px window has `outerWidth ≈ 1920` and at 200% zoom `innerWidth ≈ 960`, giving a ratio of 2.0. This works regardless of whether the page was loaded pre-zoomed or zoomed after load.

**Window chrome**: The window frame (title bar, borders) adds a small constant offset (~8-40px depending on OS). This creates a ~0.5-3% error in the ratio, which is negligible for a 48px button. A tolerance of ±0.02 (2%) is applied: ratios between 0.98 and 1.02 are treated as 1x (no transform applied).

**Window resize**: When the user resizes the window (not zoom), `outerWidth` and `innerWidth` both change by approximately the same amount, keeping the ratio near 1.0. No recalibration needed.

**Alternatives considered**:

| Method | Why rejected |
|---|---|
| Hidden 1rem ruler | Both `getComputedStyle` fontSize and `offsetWidth` return CSS pixels — ratio is always 1.0. |
| `devicePixelRatio` with baseline | Fails on pre-zoomed page loads (baseline already includes zoom). Also Firefox keeps DPR constant. |
| `visualViewport.scale` | Limited browser support. |

**Cross-browser**: `outerWidth`/`innerWidth` ratio works in Chrome, Firefox, Safari, and Edge. All major browsers keep `outerWidth` stable during zoom.

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
