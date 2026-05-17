## Why

Learning resources (curated lists, tutorials, courses, textbooks) are currently mixed in with tools under the "Learning Resource" category on the Tools page. This blurs the distinction between tools (things you use) and learning resources (things you learn from). Users browsing /tools/ see them interleaved, making both sets harder to navigate. Splitting them into a dedicated Learning Resources section with its own tab gives each collection proper focus and improves discoverability.

## What Changes

- **New `_learning/` directory** — a Jekyll collection for learning resources, mirroring `_tools/`
- **Move 38 files** with `category: Learning Resource` from `_tools/` to `_learning/`
- **New page `learning.md`** at `/learning/` URL with search and category-based browsing
- **New layout `_layouts/learning.html`** — mirrors `tools.html` layout (search, category sections, card grid)
- **Reusable include** — rename `_includes/tool-item.html` to `_includes/resource-item.html` (used by both tools and learning pages), or create a new one for learning
- **CSS** — `assets/css/learning.css` with the same card-grid styling as tools.css
- **Update `_config.yml`** — add `learning` collection (mirroring `tools` config)
- **Update header navigation** — add "Learning" tab in `_includes/header.html`
- **Update AGENTS.md** — document the new collection and category conventions
- **Remove "Learning Resource" category** — it will no longer appear in the Tools page since all entries are moved

## Capabilities

### New Capabilities

- `learning-resources-collection`: A Jekyll collection (`_learning/`) that stores learning resource entries with frontmatter: name, link, category, tags, description. Rendered as cards with search and category grouping on the `/learning/` page.
- `learning-resources-page`: A dedicated page at `/learning/` with search filtering, category sections, and a responsive card grid matching the Tools page look and feel.

### Modified Capabilities

None — no existing capabilities are changed; Learning Resource entries are moved wholesale out of tools, and the tools collection remains unchanged.

## Impact

- **38 files moved** from `_tools/` to `_learning/` (all entries with `category: Learning Resource`)
- **`_config.yml`** — new `learning` collection declaration
- **New files**: `learning.md`, `_layouts/learning.html`, `assets/css/learning.css`
- **`_includes/header.html`** — add nav link for Learning Resources
- **`_includes/tool-item.html`** — may be renamed to `resource-item.html` (shared) or duplicated
- **`assets/css/tools.css`** — unchanged (no longer has Learning Resource category, but CSS is category-agnostic)
- **AGENTS.md** — add `_learning/` collection documentation
