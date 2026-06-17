## ADDED Requirements

### Requirement: Vertical dot layout on desktop
On viewports wider than 600px, the news carousel navigation dots SHALL render as a vertical column positioned to the right of the news card grid. Dots SHALL be stacked vertically with equal spacing and centered within their column.

#### Scenario: Desktop layout with 5 views
- **WHEN** the home page loads on a viewport wider than 600px with 5 news views
- **THEN** 5 navigation dots are displayed as a vertical column to the right of the card grid
- **AND** dots are stacked with equal vertical spacing
- **AND** the active dot uses the brand color, inactive dots use border color

#### Scenario: Dots remain fixed when view height changes
- **WHEN** the user switches between views with different content heights
- **THEN** the dot column position does not shift vertically
- **AND** the grid width adjusts as needed while the dot column maintains its position

### Requirement: Horizontal dot layout on mobile
On viewports 600px or narrower, the navigation dots SHALL render as a horizontal row centered below the news card grid.

#### Scenario: Mobile layout with 3 views
- **WHEN** the home page loads on a viewport 600px or narrower with 3 news views
- **THEN** navigation dots are displayed as a horizontal row centered below the single-column card grid
- **AND** dots are separated horizontally with equal spacing

### Requirement: DOM order matches visual order
The DOM order of carousel elements SHALL place the news views before the navigation dots, matching the left-to-right visual order on desktop (views left, dots right) and the top-to-bottom visual order on mobile (views top, dots bottom).

#### Scenario: Keyboard navigation follows visual order
- **WHEN** a user tabs through the carousel
- **THEN** focus moves through news card links before reaching the navigation dots
- **AND** this order matches the visual layout on both desktop and mobile
