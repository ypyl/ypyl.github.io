## Why

The home page currently shows only the 6 most recent news items, but the site has 194+ entries. Visitors can't browse older news without navigating to individual pages. A paginated carousel exposes more content without cluttering the page.

## What Changes

- Replace the static 6-card news grid with a paginated carousel showing up to 30 recent news across 5 views (6 per view, or fewer on mobile).
- Add dot navigation below the grid — small circles representing each view, clickable to jump between pages. Dot count is dynamic (max 5), based on actual item count.
- Use a crossfade transition between views for a polished, Kami-aligned feel.
- On mobile, reduce cards per view to 3–4 to avoid excessive vertical stacking.
- No section heading is added — the news section remains unlabeled as it is today.

## Capabilities

### New Capabilities

- `news-carousel`: A paginated, dot-navigated carousel for the home page news section. Supports up to 5 views of 6 cards each (adaptive on mobile), crossfade transitions, and dynamic dot count based on available items.

### Modified Capabilities

- `news-collection`: The requirement "Home page renders news from collection — News cards SHALL display the 6 most recent entries" changes to display up to 30 most recent entries via a paginated carousel with 6 cards per view.

## Impact

- `_layouts/home.html` — news rendering loop (expanded from 6 to 30, wrapped in carousel markup)
- `assets/css/style.css` — new carousel and dots styles, modified grid responsive rules for mobile per-view count
- No new JS files — a small inline script (~10 lines) in the layout handles dot-to-view navigation
- No new dependencies, no API changes, no breaking changes to news entry format
