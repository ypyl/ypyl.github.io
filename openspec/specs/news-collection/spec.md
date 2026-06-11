## ADDED Requirements

### Requirement: News collection is registered in _config.yml
The system SHALL define a Jekyll collection named `news` stored in the `_news/` directory. The collection SHALL have `output: true` and use the permalink pattern `/news/:year/:month/:day/:title/`, deriving date components from each file's `date` frontmatter field and the title slug from the `title` field.

#### Scenario: Collection is accessible in Liquid
- **WHEN** Jekyll builds the site
- **THEN** `site.news` contains all entries from the `_news/` directory
- **AND** each entry outputs an HTML page at `/news/<year>/<month>/<day>/<title-slug>/`

#### Scenario: Collection entries are sorted by date
- **WHEN** `site.news` is accessed in Liquid with `| sort: "date" | reverse`
- **THEN** entries are ordered from newest to oldest

### Requirement: News entries are stored in _news directory
All news entry Markdown files SHALL reside directly in the `_news/` directory. No news files SHALL remain in `_posts/` or `_posts/news/`.

#### Scenario: News files are separate from blog posts
- **WHEN** listing `_news/` contents
- **THEN** all 186 news files are present
- **AND** no `.md` files remain in `_posts/news/`
- **AND** `site.posts` contains only regular blog posts (no news entries)

### Requirement: News frontmatter is minimal
Each news entry SHALL use only `layout: post`, `title`, and `date` frontmatter fields. The `categories` and `tags` fields SHALL NOT be present, as the collection membership replaces their function.

#### Scenario: News entry renders with minimal frontmatter
- **WHEN** a news file contains `layout: post`, `title: "...", date: YYYY-MM-DD`
- **AND** no `categories` or `tags` fields
- **THEN** the page renders correctly using the post layout
- **AND** the page is accessible at its `/news/...` permalink

#### Scenario: Old news files have categories and tags removed
- **WHEN** processing the migration
- **THEN** no news file contains `categories: news` or `tags: [news]` after migration

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
