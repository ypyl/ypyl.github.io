## ADDED Requirements

### Requirement: Post tags use YAML array format
Every post in `_posts/` and `_posts/news/` that has a `tags` frontmatter field SHALL use the YAML array (flow sequence) format: `tags: [tag1, tag2]`.

#### Scenario: Single tag post
- **WHEN** a post has `tags: dotnet`
- **THEN** the tags field is transformed to `tags: [dotnet]`

#### Scenario: Comma-separated tags
- **WHEN** a post has `tags: dotnet, ai, caching`
- **THEN** the tags field is transformed to `tags: [dotnet, ai, caching]`
- **AND** each tag is trimmed of leading/trailing whitespace

#### Scenario: YAML array tags
- **WHEN** a post already has `tags: [dotnet, sql]` or `tags: ["high-load"]`
- **THEN** the tags field is left unchanged

#### Scenario: Post without tags
- **WHEN** a post has no `tags` field in frontmatter
- **THEN** no tags field is added and the file is left unchanged

### Requirement: AGENTS.md mandates YAML array format
The AGENTS.md documentation SHALL specify that post tags must use YAML array format: `tags: [tag1, tag2]`.

#### Scenario: Tag guidelines are clear
- **WHEN** a contributor reads the AGENTS.md post frontmatter section
- **THEN** the tags format example shows `tags: [tag1, tag2]`
- **AND** comma-separated and bare-string formats are not shown as valid
