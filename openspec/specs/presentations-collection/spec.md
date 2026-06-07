## ADDED Requirements

### Requirement: Presentations Jekyll Collection
The system SHALL define a Jekyll collection named `presentations` stored in the `_presentations/` directory. Each entry SHALL use the `presentation` layout and have `output: true` with a `permalink` pattern of `/present/:name/`.

#### Scenario: Collection is registered in _config.yml
- **WHEN** Jekyll builds the site
- **THEN** `site.presentations` contains all entries from `_presentations/` directory
- **AND** each entry outputs an HTML page at `/present/<slug>/`

#### Scenario: Entry uses presentation layout
- **WHEN** a presentation file is created with `layout: presentation`
- **THEN** the page renders using `_layouts/presentation.html`

#### Scenario: Files without presentation layout are excluded
- **WHEN** a Markdown file exists in `_presentations/` without `layout: presentation`
- **THEN** the file is still collected and output by Jekyll (collection behavior)
- **AND** the page renders with the default layout

### Requirement: Post-to-presentation association via slug
The system SHALL associate a presentation with a blog post when the presentation filename matches the post slug (post filename without date prefix and `.md` extension). When a match exists, a `[🖥️ Present]` link SHALL appear in the site navigation on the post page.

#### Scenario: Matching slugs create association
- **WHEN** a post file `_posts/2026-05-25-Example-Post.md` (slug: `Example-Post`) exists
- **AND** a presentation file `_presentations/Example-Post.md` exists
- **THEN** `pres.slug == page.slug` evaluates to `true` in Liquid
- **AND** the `[🖥️ Present]` link appears on the post page

#### Scenario: Non-matching slugs show no link
- **WHEN** a post file has slug `My-Article`
- **AND** no presentation file is named `My-Article.md`
- **THEN** no `[🖥️ Present]` link appears on the post page

#### Scenario: Presentation with different name is still accessible
- **WHEN** a presentation file exists at `_presentations/Different-Name.md`
- **AND** no post matches the slug `Different-Name`
- **THEN** the presentation page is still accessible at `/present/Different-Name/`
- **AND** no automatic link appears from any post page

### Requirement: Presentation file frontmatter
Each presentation file SHALL use minimal frontmatter: `layout: presentation`. No additional fields are required for the presentation to render.

#### Scenario: Minimal frontmatter renders presentation
- **WHEN** a presentation file contains only `layout: presentation` in frontmatter
- **AND** contains Markdown content with `---` slide separators
- **THEN** Jekyll renders the page at `/present/<slug>/` using the presentation layout

#### Scenario: Additional frontmatter fields pass through
- **WHEN** a presentation file includes optional frontmatter fields (e.g., `title`)
- **THEN** the fields are available as `page.<field>` in the layout
- **AND** the presentation still renders correctly
