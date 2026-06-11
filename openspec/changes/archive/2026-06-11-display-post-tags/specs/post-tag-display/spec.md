## ADDED Requirements

### Requirement: Tag CSS is globally available
The system SHALL define `.tags` and `.tag` CSS classes in `assets/css/style.css`, using Kami design tokens for colors and the sans-serif font stack. These styles SHALL be available on all pages, not just the tools page.

#### Scenario: Tag styles render on post pages
- **WHEN** a post page loads with tag badges
- **THEN** tags render as uppercase sans-serif badges with brand color on tag background
- **AND** tags use flex-wrap layout with 6px gap

#### Scenario: Tag styles render on home page
- **WHEN** the home page loads with tagged posts
- **THEN** tag badges appear in post list items with the same styling as post pages

#### Scenario: Tools page tags still render
- **WHEN** the tools page loads
- **THEN** tool card tag badges render identically to before the CSS move

### Requirement: Post page displays tag badges
The post layout SHALL render tag badges in the post header below the date and author metadata. Each tag SHALL appear as a `<span class="tag">` element inside a `<div class="tags">` container. Posts without tags SHALL NOT render an empty tags container.

#### Scenario: Post with tags shows badges
- **WHEN** a post has `tags: [dotnet, ai, caching]`
- **THEN** the post header displays three tag badges: "dotnet", "ai", "caching"
- **AND** badges appear in the `<div class="tags">` wrapper below the date

#### Scenario: Post without tags shows nothing
- **WHEN** a post has no `tags` field or an empty tags array
- **THEN** no `<div class="tags">` element is rendered in the post header

### Requirement: Home page post list displays tag badges
The home page SHALL render tag badges for each post in the post list. Each post's tags SHALL appear below the post title link inside a `<div class="tags">` container. Posts without tags SHALL NOT render an empty tags container.

#### Scenario: Tagged post in list shows badges
- **WHEN** the home page renders a post with `tags: [javascript, react]`
- **THEN** two tag badges appear below the post title: "javascript", "react"

#### Scenario: Untagged post in list shows no badges
- **WHEN** the home page renders a post with no tags
- **THEN** no `<div class="tags">` element is rendered for that post list item
