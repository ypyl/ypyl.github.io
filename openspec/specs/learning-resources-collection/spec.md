## ADDED Requirements

### Requirement: Learning Resources Jekyll Collection
The system SHALL define a Jekyll collection named `learning` stored in the `_learning/` directory. Each entry SHALL use frontmatter fields: `name` (string), `link` (URL string), `category` (string), `tags` (array of strings), `description` (string). The collection SHALL have `output: false` — individual entry pages are not rendered.

#### Scenario: Collection is registered in _config.yml
- **WHEN** Jekyll builds the site
- **THEN** `site.learning` contains all entries from `_learning/` directory, sorted and accessible by their frontmatter fields

#### Scenario: Entry frontmatter is valid
- **WHEN** a learning resource file is created with name, link, category, tags, and description
- **THEN** the entry is accessible via `site.learning` with all fields populated as specified

#### Scenario: Individual pages are not generated
- **WHEN** a learning resource file exists at `_learning/example.md`
- **THEN** no HTML page is generated at `/learning/example.html`

### Requirement: Migration of Existing Learning Resources
The system SHALL contain exactly the same 38 learning resource entries currently in `_tools/` with `category: Learning Resource`, now located in `_learning/`. The `category` field SHALL be updated from "Learning Resource" to one of the valid subcategories: Curated List, Course, Book, Guide, Tutorial, or Research Paper. All other frontmatter fields (`name`, `link`, `tags`, `description`) SHALL remain unchanged. No entry with `category: Learning Resource` SHALL remain in `_tools/`.

#### Scenario: All 38 entries are moved
- **WHEN** the migration is complete
- **THEN** `_tools/` contains zero files with `category: Learning Resource`
- **AND** `_learning/` contains 38 files, each with a `category` from the valid subcategory set

#### Scenario: Frontmatter is preserved except category
- **WHEN** a learning resource is migrated from `_tools/` to `_learning/`
- **THEN** `name`, `link`, `tags`, and `description` match the original values exactly
- **AND** `category` is one of: Curated List, Course, Book, Guide, Tutorial, Research Paper

#### Scenario: Tools collection excludes learning resources
- **WHEN** the Tools page renders `site.tools` grouped by category
- **THEN** "Learning Resource" does not appear as a category section
