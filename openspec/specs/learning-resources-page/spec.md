## ADDED Requirements

### Requirement: Learning Resources Page
The system SHALL provide a page at `/learning/` that displays all entries from the `_learning/` collection, grouped by their `category` field, in a responsive card grid matching the visual design of the `/tools/` page.

#### Scenario: Page is accessible at /learning/
- **WHEN** a user navigates to `/learning/`
- **THEN** the page renders with the site header, footer, and a card grid of learning resource entries grouped by category

#### Scenario: Categories are displayed as sections
- **WHEN** the page loads
- **THEN** each unique `category` value from `_learning/` entries appears as a section heading
- **AND** entries within each section are sorted alphabetically by `name`

### Requirement: Learning Resources Search
The system SHALL provide a client-side text search input on the `/learning/` page that filters visible entries by matching against the entry's name, description, and tags (case-insensitive).

#### Scenario: Search matches an entry by name
- **WHEN** a user types "python" in the search input
- **THEN** only entries whose name, description, or tags contain "python" (case-insensitive) remain visible
- **AND** category sections with no visible entries are hidden

#### Scenario: Search is cleared
- **WHEN** a user clears the search input
- **THEN** all entries and all category sections become visible again

#### Scenario: No results found
- **WHEN** a user types a query that matches no entries
- **THEN** a "No resources match your search" message is displayed

### Requirement: Learning Resources Card Display
Each learning resource entry SHALL render as a card containing: the entry `name` as a hyperlink to its `link` URL (opening in a new tab), the `description` text, and `tags` rendered as styled label elements.

#### Scenario: Card renders all fields
- **WHEN** a learning resource card is displayed
- **THEN** the name links to the external URL with `target="_blank"`
- **AND** the description appears below the name
- **AND** all tags appear as styled labels below the description

#### Scenario: Card hover interaction
- **WHEN** a user hovers over a learning resource card
- **THEN** the card displays a subtle elevation (box-shadow increase) and slight upward transform, matching the tools card behavior

### Requirement: Visual Design Consistency
The `/learning/` page SHALL use the same Kami design system tokens (serif fonts, parchment background, brand colors, dark mode support, responsive breakpoints) as the rest of the site.

#### Scenario: Light mode
- **WHEN** the site is in light mode
- **THEN** the learning page renders with parchment background, serif headings, and ink-blue accent colors consistent with the Tools page

#### Scenario: Dark mode
- **WHEN** the site is in dark mode
- **THEN** the learning page renders with dark surface colors, adjusted text, and adjusted tag backgrounds consistent with the Tools page in dark mode

#### Scenario: Responsive grid
- **WHEN** the viewport is below 600px width
- **THEN** cards stack in a single column
- **AND** when the viewport is above 600px, cards flow into a multi-column auto-fill grid
