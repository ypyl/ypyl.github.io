## ADDED Requirements

### Requirement: Presentation page loads Reveal.js
The system SHALL load Reveal.js 5.x from CDN on the presentation page, including RevealMarkdown, RevealHighlight, and RevealNotes plugins. The slides SHALL initialize automatically when the page loads.

#### Scenario: Reveal.js initializes successfully
- **WHEN** a user navigates to `/present/<slug>/`
- **THEN** Reveal.js loads from CDN
- **AND** slides render in the viewport
- **AND** keyboard navigation (arrow keys, Space) is functional

#### Scenario: CDN fails to load
- **WHEN** Reveal.js fails to load from CDN
- **THEN** a fallback message is displayed indicating the presentation could not be loaded

### Requirement: Slide separation via Markdown horizontal rules
The system SHALL treat `---` separators in the presentation Markdown as slide boundaries. When Jekyll renders `---` to `<hr />`, the presentation layout SHALL split the HTML content on `<hr />` elements and wrap each segment in a `<section>` element.

#### Scenario: Multiple slides render
- **WHEN** a presentation file contains content separated by three `---` separators
- **THEN** four `<section>` elements are created (one per content segment)
- **AND** Reveal.js displays them as individual slides

#### Scenario: Single-slide presentation
- **WHEN** a presentation file contains no `---` separators
- **THEN** one `<section>` element wraps all content
- **AND** Reveal.js displays it as a single slide

#### Scenario: Empty segments are excluded
- **WHEN** a separator creates an empty content segment
- **THEN** no empty `<section>` is created for that segment

### Requirement: Speaker notes via blockquote convention
The system SHALL recognize blockquotes starting with `**Notes:**` as speaker notes. The presentation layout SHALL convert these into `<aside class="notes">` elements within their parent `<section>`, making them visible only in Reveal.js presenter mode.

#### Scenario: Notes blockquote converts to aside
- **WHEN** slide content contains `> **Notes:** Remember to mention X`
- **THEN** the rendered slide `<section>` contains `<aside class="notes">Remember to mention X</aside>`
- **AND** the notes text is NOT visible on the audience-facing slide

#### Scenario: Presenter mode displays notes
- **WHEN** a slide contains an `<aside class="notes">` with speaker notes
- **AND** the presenter presses `S` to open presenter mode
- **THEN** the speaker notes appear in the presenter console window

#### Scenario: Regular blockquotes are not converted
- **WHEN** slide content contains a blockquote that does NOT start with `**Notes:**`
- **THEN** the blockquote renders as normal content visible on the slide

### Requirement: Light and dark theme support
The system SHALL support both light and dark themes for presentation slides. On load, the presentation page SHALL read the user's theme preference from `localStorage` (key: `theme`) or the system `prefers-color-scheme` media query, and apply the corresponding `data-theme` attribute to the Reveal.js container.

#### Scenario: Light theme from localStorage
- **WHEN** `localStorage.getItem('theme')` returns `'light'`
- **THEN** the presentation container has `data-theme="light"`
- **AND** slides render with the light Kami color scheme

#### Scenario: Dark theme from localStorage
- **WHEN** `localStorage.getItem('theme')` returns `'dark'`
- **THEN** the presentation container has `data-theme="dark"`
- **AND** slides render with the dark Kami color scheme

#### Scenario: Theme toggle switches between modes
- **WHEN** the user clicks the theme toggle button on the presentation page
- **THEN** the `data-theme` attribute toggles between `"light"` and `"dark"`
- **AND** the preference is saved to `localStorage` under key `theme`
- **AND** slide styling updates without page reload

#### Scenario: System preference fallback
- **WHEN** no theme preference is stored in `localStorage`
- **AND** the system `prefers-color-scheme` is `dark`
- **THEN** the presentation renders in dark mode

### Requirement: Slide styling with Kami design tokens
The system SHALL style Reveal.js slides using the blog's Kami design system tokens. Slide backgrounds, text colors, link colors, code blocks, and heading styles SHALL match the blog's visual identity in both light and dark modes.

#### Scenario: Light mode slide styling
- **WHEN** a presentation is in light mode
- **THEN** slides use parchment background (`#f5f4ed`), near-black text, ink-blue (`#1B365D`) accents for links and headings
- **AND** code blocks use the monospace font and ivory background consistent with the blog

#### Scenario: Dark mode slide styling
- **WHEN** a presentation is in dark mode
- **THEN** slides use deep dark background (`#141413`), light text, muted blue accents
- **AND** code blocks adapt to dark background colors consistent with the blog's dark mode

#### Scenario: Serif typography on slides
- **WHEN** slides render
- **THEN** headings use the Charter/Georgia serif font stack (`var(--serif)`)
- **AND** body text uses the same serif font stack
- **AND** code uses JetBrains Mono monospace font stack
- **AND** no italic styling is applied

### Requirement: Post page renders with post layout
The system SHALL provide a `_layouts/post.html` that extends the `default` layout and renders blog post content with the standard Minima post structure: an `<article>` with `post-header` (title, date) and `post-content` (body).

#### Scenario: Post page renders title and date
- **WHEN** a user navigates to a blog post
- **THEN** the page displays the post title in an `<h1>` within a `post-header` element
- **AND** the post date is displayed below the title

#### Scenario: Post page renders content
- **WHEN** a user navigates to a blog post
- **THEN** the post body renders within a `div.post-content` element
- **AND** Markdown formatting (headings, code blocks, tables, lists) is preserved

### Requirement: Present link in site navigation
The system SHALL render a `[🖥️ Present]` link in the site navigation bar when viewing a blog post that has a matching companion presentation. The link SHALL navigate to the presentation page at `/present/<slug>/`.

#### Scenario: Link appears for post with companion slides
- **WHEN** viewing a post page whose slug matches a `_presentations/` file slug
- **THEN** the site nav includes a `🖥️ Present` link pointing to `/present/<slug>/`

#### Scenario: Link does not appear for post without companion slides
- **WHEN** viewing a post page with no matching presentation file
- **THEN** no `🖥️ Present` link appears in the site nav

#### Scenario: Link does not appear on non-post pages
- **WHEN** viewing the home page, tools page, or learning page
- **THEN** no `🖥️ Present` link appears in the site nav

### Requirement: Reveal.js presenter mode
The system SHALL support Reveal.js presenter mode. When the presenter presses `S` during a presentation, a separate browser window SHALL open showing speaker notes, a timer, and a preview of the next slide.

#### Scenario: Presenter mode opens
- **WHEN** the presenter presses `S` while viewing a presentation
- **THEN** a new browser window opens displaying the presenter console
- **AND** speaker notes (from `<aside class="notes">`) are visible in the presenter console
- **AND** a live timer is displayed
- **AND** a preview of the next slide is shown

#### Scenario: Presenter mode syncs with audience window
- **WHEN** the presenter advances slides in the presenter console
- **THEN** the audience window advances to the same slide
