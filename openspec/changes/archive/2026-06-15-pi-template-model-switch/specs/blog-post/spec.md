## ADDED Requirements

### Requirement: Blog post file

The blog post SHALL be created as `_posts/2026-06-15-template-model-switch.md` following Jekyll filename conventions.

#### Scenario: Post file exists

- **WHEN** the site builds on GitHub Pages
- **THEN** the post SHALL be rendered at `https://ypyl.github.io/2026/06/15/template-model-switch.html`

### Requirement: Frontmatter

The post SHALL include standard Jekyll frontmatter with `layout: post`, `title`, `date`, `tags`, and `categories`.

#### Scenario: Post appears in blog index

- **WHEN** a user visits the blog homepage
- **THEN** the post SHALL appear in the post listing with its title and date

#### Scenario: Post has correct tags

- **WHEN** the post is viewed
- **THEN** its tags SHALL include `pi`, `extension`, `prompt-engineering`, `open-source` as a YAML array

### Requirement: Content structure

The post SHALL explain the problem (manual model switching), introduce the extension as the solution, show a code snippet of how it works, provide a concrete demo with a prompt template, and link to the GitHub repository for installation.

#### Scenario: Reader understands the problem

- **WHEN** a reader finishes the introduction
- **THEN** they SHALL understand why manual model switching between prompt templates is friction

#### Scenario: Reader can install it

- **WHEN** a reader reaches the installation section
- **THEN** they SHALL find a `pi install` command and a link to the GitHub repository

### Requirement: Kami design compliance

The post SHALL follow the Kami design system used by the blog: warm parchment surface, single ink-blue accent, serif typography, and warm-toned grays.

#### Scenario: Post renders consistently

- **WHEN** the post is viewed on the blog
- **THEN** its visual styling SHALL match other posts on the site without custom overrides

### Requirement: Link to repository

The post SHALL include a hyperlink to `https://github.com/ypyl/pi-template-model-switch`.

#### Scenario: Reader navigates to the repo

- **WHEN** a reader clicks the repository link in the post
- **THEN** they SHALL arrive at the GitHub repository page
