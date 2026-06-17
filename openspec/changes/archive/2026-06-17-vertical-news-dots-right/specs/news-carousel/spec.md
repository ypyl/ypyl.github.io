## MODIFIED Requirements

### Requirement: Dot navigation controls view switching
The carousel SHALL include clickable navigation dots. On desktop viewports (wider than 600px), dots SHALL render as a vertical column to the right of the news grid. On mobile viewports (600px or narrower), dots SHALL render as a horizontal row below the news grid. Each dot corresponds to one view. Clicking a dot SHALL switch the visible view to the corresponding news card group.

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
