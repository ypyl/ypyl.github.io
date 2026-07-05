## Why

On mobile viewports (≤600px), when the hamburger menu is expanded, the navigation dropdown ("Learning", "Tools") renders underneath the news carousel cards. Users cannot see or click the menu links because the news cards — positioned later in the DOM with `position: relative` — stack on top. This is a z-index bug: both `.site-header` and `.news-carousel-views` have `position: relative` with `z-index: auto`, and CSS stacks them in DOM order (later wins).

## What Changes

- Add `z-index: 10` to `.site-header` in `assets/css/style.css`, lifting the header and its absolute-positioned nav dropdown above the main content area.

## Capabilities

### New Capabilities

None — this is a CSS bug fix, not a new feature.

### Modified Capabilities

None — no spec-level requirement changes.

## Impact

- `assets/css/style.css` — one line added to the `.site-header` rule
