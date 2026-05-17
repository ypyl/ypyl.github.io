## Context

The site currently has a single `_tools/` Jekyll collection holding 2,770 lines across tool entries. One category — "Learning Resource" — accounts for 38 entries and represents a different kind of content: things you learn FROM (courses, books, tutorials, curated lists) vs. things you USE (frameworks, libraries, applications). Both types share the `/tools/` page, interleaved in category sections.

The site uses the `minima` theme with custom layouts, a warm parchment design system (Kami), and a card-grid pattern for tools with client-side search. Header navigation auto-generates from all pages (no `header_pages` config override).

## Goals / Non-Goals

**Goals:**
- Move all 38 "Learning Resource" entries from `_tools/` to a new `_learning/` Jekyll collection
- Create a dedicated `/learning/` page with the same card-grid UX (search, categories, cards)
- Keep the `/tools/` page working exactly as before (minus the Learning Resource category)
- No visual regressions — same Kami design system, same card hover effects, same dark mode
- No changes to individual entry frontmatter (same fields, same values)

**Non-Goals:**
- Re-categorizing individual learning entries into subcategories
- Changing the tools page layout or behavior
- Adding filters beyond the existing search
- Server-side changes (this is a static Jekyll site)

## Decisions

### Decision 1: New collection `_learning/` (not a subdirectory of `_tools/`)

**Rationale**: Jekyll collections are the idiomatic way to group related content. A separate collection keeps `_tools/` and `_learning/` independent — they can evolve separately (different frontmatter defaults, different layouts, different output config) without coupling.

**Alternative considered**: Keep everything in `_tools/` and filter by category at the page level. Rejected because this doesn't solve the navigation problem — users still land on a single page with everything mixed together.

### Decision 2: Dedicated include `_includes/learning-item.html` (not shared with tools)

**Rationale**: While `tool-item.html` and `learning-item.html` are structurally identical, sharing a single include couples the two collections. If one evolves (e.g., learning resources add an author field, or tools add a version badge), the shared include needs conditional logic. Two small, independent includes are simpler and safer.

**Alternative considered**: Rename `tool-item.html` to `resource-item.html` and use it for both. Rejected because the CSS class names and data attributes differ (`.tool` vs `.resource`, `data-tool-*` vs `data-resource-*`), making the shared include need parameterization or conditionals.

### Decision 3: Independent CSS file `assets/css/learning.css`

**Rationale**: `tools.css` uses class names like `.tools-page`, `.tool-category`, `.tool`, `.tool-list`. Copying and renaming to `.resources-page`, `.resource-category`, `.resource`, `.resource-list` avoids selector collisions and keeps each page's styling self-contained. This is ~100 lines of CSS — the duplication is negligible.

**Alternative considered**: Abstract shared card-grid styles into a base class and use composition. Rejected as over-engineering for a static blog with manual CSS; the duplication is minimal and maintenance is simpler with independent files.

### Decision 4: No header navigation changes needed

**Rationale**: The header (`_includes/header.html`) auto-generates navigation from `site.pages`. Since `learning.md` will be a standard page with `title: Learning`, it appears in the nav automatically — just as `tools.md` (title: Tools) does today. No `header_pages` config exists to override this behavior.

### Decision 5: Introduce subcategories for learning resources

**Rationale**: Having 38 entries under a single "Learning Resource" heading makes the page feel like an undifferentiated dump. Introducing subcategories groups related resources, making browsing and discovery easier. The `category` field in each entry's frontmatter is updated during migration.

**Subcategories**:
- **Curated List** — Aggregations of links/tools/resources (awesome-* repos, curated collections)
- **Course** — Structured learning with progressive sessions, workshops, hands-on platforms
- **Book** — Full-length textbooks or book companion repositories
- **Guide** — Articles, blog posts, documentation guides on a specific topic
- **Tutorial** — Step-by-step how-to walkthroughs of a specific technique
- **Research Paper** — Academic research or deep technical analysis

## Risks / Trade-offs

- **[Risk] Subcategory assignment is subjective** → Mitigation: Categories are broad and overlap is minimal. If an entry fits multiple categories, the primary intent is used. Categories can be adjusted later by editing a single frontmatter field.
- **[Risk] Broken links to `/tools/` for learning resources** → Mitigation: No individual pages are rendered for tools or learning entries (`output: false` for both collections). External links point to the source URLs (GitHub, blogs, etc.), not to site pages. No redirects needed.
- **[Risk] Search index differences** → Mitigation: Client-side search filters visible cards by name, description, and tags. The same logic works on both pages. No indexing or SEO impact since collection entries have `output: false`.

## Migration Plan

1. Create `_learning/` directory and move 38 files from `_tools/` to `_learning/`
2. Add `learning` collection to `_config.yml`
3. Create `_includes/learning-item.html` (copy of `tool-item.html` with renamed classes/attributes)
4. Create `_layouts/learning.html` (copy of `tools.html` with renamed classes/variables)
5. Create `assets/css/learning.css` (copy of `tools.css` with renamed selectors)
6. Create `learning.md` page
7. Update AGENTS.md
8. Verify locally or push to GitHub Pages for live validation

**Rollback**: Revert the git commit. No data migration or state to unwind.

## Open Questions

None — all decisions resolved.
