## ADDED Requirements

### Requirement: Post meta line shows View source link
Each blog post page SHALL display a "View source" link in the post meta line that points to the post's Markdown source file on GitHub (blob view).

#### Scenario: Post renders with View source link
- **WHEN** a visitor loads a blog post page
- **THEN** the post meta line SHALL contain a link with text "View source"
- **AND** the link href SHALL follow the format `https://github.com/ypyl/ypyl.github.io/blob/main/<page.path>`

#### Scenario: Link opens in same tab
- **WHEN** a visitor clicks the "View source" link
- **THEN** the GitHub page SHALL open in the same browser tab (default link behavior)

#### Scenario: Link is visually consistent with meta line
- **WHEN** a visitor views the post meta line
- **THEN** the "View source" link SHALL use the same visual styling as other meta line links
- **AND** it SHALL be separated from preceding meta items with a middle dot separator (`·`)
