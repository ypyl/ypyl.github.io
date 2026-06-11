## ADDED Requirements

### Requirement: News carousel renders on the home page
The home page SHALL render a paginated news carousel in place of the existing static 6-card grid. The carousel SHALL display up to 30 most recent news items (from `site.news` sorted by date descending) grouped into views of 6 cards each, with a maximum of 5 views.

#### Scenario: Home page shows carousel with 30+ news items
- **WHEN** `site.news` contains 30 or more entries
- **THEN** the carousel renders 5 views, each containing 6 news cards
- **AND** 5 navigation dots are displayed below the grid

#### Scenario: Home page shows carousel with fewer than 30 items
- **WHEN** `site.news` contains between 7 and 29 entries
- **THEN** the carousel renders `ceil(news_count / 6)` views, capped at 5
- **AND** the number of navigation dots matches the number of views

#### Scenario: Home page falls back to static grid with 6 or fewer items
- **WHEN** `site.news` contains 6 or fewer entries
- **THEN** the carousel renders a single view with no navigation dots
- **AND** the layout is visually identical to the pre-carousel static grid

### Requirement: Dot navigation controls view switching
The carousel SHALL include clickable navigation dots below the news grid. Each dot corresponds to one view. Clicking a dot SHALL switch the visible view to the corresponding news card group.

#### Scenario: User clicks an inactive dot
- **WHEN** the user clicks a dot that is not currently active
- **THEN** the previously active dot and view lose their active state
- **AND** the clicked dot and its corresponding view become active
- **AND** a crossfade transition animates the view change

#### Scenario: Keyboard navigation on dots
- **WHEN** a dot receives focus and the user presses Enter or Space
- **THEN** the carousel switches to that dot's corresponding view

#### Scenario: Dots are not displayed when only one view exists
- **WHEN** the carousel has a single view (6 or fewer news items)
- **THEN** no dot navigation elements are rendered

### Requirement: Crossfade transition between views
View switching SHALL use an opacity-based crossfade transition. The active view SHALL have `opacity: 1` and `pointer-events: auto`. Inactive views SHALL have `opacity: 0` and `pointer-events: none`. The transition duration SHALL be approximately 300ms with an ease timing function.

#### Scenario: Transition from view 1 to view 3
- **WHEN** the user clicks dot 3 while view 1 is active
- **THEN** view 1 fades out (opacity 1 → 0) over ~300ms
- **AND** view 3 fades in (opacity 0 → 1) over ~300ms
- **AND** no horizontal or vertical movement occurs

### Requirement: Dots use Kami design tokens
Navigation dots SHALL be circular elements 8px in diameter. Inactive dots SHALL use `var(--border-color)` for fill. The active dot SHALL use `var(--brand)` for fill. Dots SHALL be `<button>` elements with descriptive `aria-label` attributes.

#### Scenario: Dots render with correct styling
- **WHEN** the carousel loads with 5 views
- **THEN** 5 circular dots are displayed in a horizontal row centered below the grid
- **AND** the first dot uses `var(--brand)` fill
- **AND** the remaining dots use `var(--border-color)` fill
- **AND** dots are separated by 8px spacing

### Requirement: Mobile adaptation reduces cards per view
On viewports 600px or narrower, each carousel view SHALL display 3 news cards instead of 6. The card grid SHALL use a single-column layout. The number of views SHALL remain unchanged (max 5), with dots adapting accordingly.

#### Scenario: Mobile viewport shows 3 cards per view
- **WHEN** the viewport width is 600px or less
- **THEN** each active carousel view displays exactly 3 news cards
- **AND** the 4th through 6th cards in each view are hidden via CSS

### Requirement: No-JavaScript fallback
When JavaScript is unavailable or fails to execute, all news views SHALL stack vertically as a single scrollable list. Navigation dots SHALL be hidden. All 30 rendered news cards SHALL be visible and readable.

#### Scenario: Page loads without JavaScript
- **WHEN** the browser loads the home page with JavaScript disabled
- **THEN** all news card views are visible in a vertical stack
- **AND** no navigation dots are displayed
- **AND** all card content is readable and links are functional
