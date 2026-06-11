## ADDED Requirements

### Requirement: Post page tags render inline after date
On the post page, tag badges SHALL appear on the same line as the date, separated from the date by a `·` bullet. The tags wrapper SHALL use `display: inline-flex` to remain on the date line while preserving gap and wrap behavior between individual tags.

#### Scenario: Tags inline with date
- **WHEN** a post has `tags: [dotnet, ai, caching]`
- **THEN** the post header displays `Jan 6, 2026 · dotnet ai caching` on one line
- **AND** tag badges use the default Kami tier (`--tag-bg` background, standard size)

#### Scenario: Tags wrap when line overflows
- **WHEN** a post has many tags that exceed the line width
- **THEN** tags wrap to the next line below the date
- **AND** the `·` separator remains before the first tag, not on the wrapped line

#### Scenario: Post with date and author shows tags after both
- **WHEN** a post has an author and tags
- **THEN** the meta line reads `Jan 6, 2026 • Author Name · dotnet ai`
- **AND** the `·` separates tags from the preceding metadata

### Requirement: Home page tags use lightest Kami tier
On the home page post list, tag badges SHALL use the Kami lightest solid tier: `--tag-bg-light` background (`#EEF2F7`), smaller font size (0.625rem / 10px), and tighter padding (2px 6px). This SHALL be applied via `.post-list .tag` CSS override, scoped to post list items only.

#### Scenario: Home page tags are visually lighter
- **WHEN** the home page renders a post with tags
- **THEN** tag badges use `#EEF2F7` background (lighter than post page `#E4ECF5`)
- **AND** tag font size is 0.625rem (smaller than default 0.6875rem)
- **AND** tag padding is 2px 6px (tighter than default 3px 8px)

#### Scenario: Post page tags are unaffected
- **WHEN** viewing a post page with tags
- **THEN** tags continue to use the default Kami tier (`--tag-bg`, standard size)
- **AND** the `.post-list .tag` override does not apply to post pages
