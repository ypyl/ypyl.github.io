## ADDED Requirements

### Requirement: Carousel container height matches active view
The news carousel container SHALL size its height to match the natural content height of the currently active view, not the tallest view in the collection.

#### Scenario: Switching from a tall view to a short view
- **WHEN** the user clicks a navigation dot to switch from a view with long news excerpts to a view with short news excerpts
- **THEN** the carousel container height shrinks to match the active view's content
- **AND** the navigation dots are positioned immediately below the active view's last row of cards with no large empty gap

#### Scenario: Switching from a short view to a tall view
- **WHEN** the user clicks a navigation dot to switch from a view with short news excerpts to a view with long news excerpts
- **THEN** the carousel container height expands to accommodate the taller active view's content
- **AND** no content is clipped or hidden

#### Scenario: Initial page load with tall first view
- **WHEN** the home page loads with the first view active and subsequent views have shorter content
- **THEN** the carousel container height matches the first (active) view's content height
- **AND** no unnecessary empty space appears below the news cards

### Requirement: Inactive views do not affect container layout
Inactive news views SHALL be removed from the normal document flow so they do not contribute to the parent container's computed height.

#### Scenario: Multiple hidden views with varying heights
- **WHEN** the carousel has 5 views with significantly different content heights
- **THEN** only the active view's height determines the `.news-carousel-views` container height
- **AND** hidden views, despite being rendered in the DOM, do not stretch the container

### Requirement: Crossfade transition preserved
The opacity-based crossfade transition between views SHALL continue to function as before. The active view SHALL have `opacity: 1` and `pointer-events: auto`. Inactive views SHALL have `opacity: 0` and `pointer-events: none`. The transition duration SHALL be approximately 300ms with an ease timing function.

#### Scenario: Visual crossfade on view switch
- **WHEN** the user clicks a navigation dot
- **THEN** the outgoing view fades out over ~300ms
- **AND** the incoming view fades in over ~300ms
- **AND** the container height adjusts to the incoming view's content height

### Requirement: No-JavaScript fallback preserved
When JavaScript is unavailable or fails to execute, all news views SHALL stack vertically as a single scrollable list with navigation dots hidden, matching the existing no-JS behavior exactly.

#### Scenario: Page loads without JavaScript
- **WHEN** the browser loads the home page with JavaScript disabled
- **THEN** all news card views are visible in a vertical stack
- **AND** no navigation dots are displayed
- **AND** no absolute positioning or stacking effects are applied
