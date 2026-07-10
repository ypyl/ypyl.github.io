## Why

The news carousel navigation dots are clickable `<button>` elements, but they show the default text-selection cursor instead of a hand/pointer cursor. This happens because the parent `.news-carousel-dots` container sets `cursor: default`. Users don't get the visual affordance that dots are interactive, degrading the UX.

## What Changes

- Add `cursor: pointer` to `.news-dot` in the carousel CSS to override the parent's `cursor: default`

## Capabilities

### New Capabilities

- `news-carousel-dot-cursor`: Navigation dots in the news carousel show a pointer (hand) cursor on hover, providing a standard interactive element affordance

### Modified Capabilities

None. This is purely a presentational addition — it does not change any existing behavioral requirements.

## Impact

- **CSS**: `assets/css/style.css` — add one `cursor: pointer` declaration to the `.news-dot` rule
- No JavaScript, HTML, or dependency changes
- No breaking changes
