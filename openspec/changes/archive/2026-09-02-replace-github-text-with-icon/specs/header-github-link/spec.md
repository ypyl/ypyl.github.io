## MODIFIED Requirements

### Requirement: Header navigation shows GitHub link
The site header SHALL display a GitHub link in the navigation bar, positioned after the dynamically-generated page links, that points to the user's GitHub profile. The link SHALL render as an inline SVG icon (GitHub octocat logo), not as text.

#### Scenario: GitHub icon visible on all pages
- **WHEN** a visitor loads any page on the site
- **THEN** the header navigation SHALL contain a link with an inline SVG icon
- **AND** the SVG SHALL render the GitHub octocat logo at 16×16px
- **AND** the link href SHALL be `https://github.com/ypyl`

#### Scenario: GitHub icon matches nav styling
- **WHEN** a visitor views the header navigation
- **THEN** the GitHub icon link SHALL use the same CSS class (`page-link`) as other navigation links
- **AND** it SHALL appear after all page-based navigation links

#### Scenario: GitHub icon inherits color
- **WHEN** a visitor views the header in light or dark mode
- **THEN** the SVG icon SHALL use `fill="currentColor"` to inherit the text color from CSS
- **AND** it SHALL change color on hover to match other nav links

#### Scenario: GitHub icon accessibility
- **WHEN** a visitor uses assistive technology
- **THEN** the SVG element SHALL have `aria-hidden="true"`
- **AND** the anchor element SHALL have `title="GitHub"` and `aria-label="GitHub"`

#### Scenario: GitHub icon opens profile
- **WHEN** a visitor clicks the GitHub icon
- **THEN** the browser SHALL navigate to the user's GitHub profile page in the same tab
