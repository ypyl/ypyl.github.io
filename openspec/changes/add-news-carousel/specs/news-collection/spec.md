## MODIFIED Requirements

### Requirement: Home page renders news from collection
The home page SHALL render news cards using `site.news` directly, without iterating `site.posts` or filtering by category. News rendering SHALL use a paginated carousel displaying up to 30 most recent entries across a maximum of 5 views (6 cards per view on desktop, 3 per view on mobile). When 6 or fewer news items exist, the carousel SHALL fall back to a single static grid matching the pre-carousel appearance.

#### Scenario: News cards render from collection
- **WHEN** the home page loads
- **THEN** a news carousel is displayed with up to 5 views of up to 6 cards each
- **AND** cards are sourced from `site.news` sorted by date descending
- **AND** the Liquid template does not reference `site.posts` for news content

#### Scenario: Carousel shows exactly 6 or fewer items as static grid
- **WHEN** `site.news` contains 6 or fewer entries
- **THEN** the carousel renders a single view with no navigation dots
- **AND** the appearance matches the pre-carousel static 6-card grid

#### Scenario: Blog post list excludes news
- **WHEN** the home page renders the post list
- **THEN** `site.posts` is iterated directly without category filtering
- **AND** no news entries appear in the post list
