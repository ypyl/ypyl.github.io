## ADDED Requirements

### Requirement: Header navigation shows GitHub link
The site header SHALL display a "GitHub" link in the navigation bar, positioned after the dynamically-generated page links, that points to the site's GitHub repository.

#### Scenario: GitHub link visible on all pages
- **WHEN** a visitor loads any page on the site
- **THEN** the header navigation SHALL contain a link with text "GitHub"
- **AND** the link href SHALL be `https://github.com/ypyl`

#### Scenario: GitHub link matches nav styling
- **WHEN** a visitor views the header navigation
- **THEN** the "GitHub" link SHALL use the same CSS class (`page-link`) as other navigation links
- **AND** it SHALL appear after all page-based navigation links

#### Scenario: GitHub link opens repository
- **WHEN** a visitor clicks the "GitHub" link
- **THEN** the browser SHALL navigate to the site's GitHub repository page in the same tab
