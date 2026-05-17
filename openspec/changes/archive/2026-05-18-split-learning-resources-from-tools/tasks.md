## 1. Collection Setup

- [x] 1.1 Create `_learning/` directory
- [x] 1.2 Move all 38 files with `category: Learning Resource` from `_tools/` to `_learning/`
- [x] 1.3 Update `category` field in each migrated file from "Learning Resource" to the appropriate subcategory (Curated List, Course, Book, Guide, Tutorial, Research Paper)
- [x] 1.4 Add `learning` collection to `_config.yml` with `output: false`
- [x] 1.5 Verify no `category: Learning Resource` entries remain in `_tools/`

## 2. Page and Layout

- [x] 2.1 Create `learning.md` page with layout `learning`, title `Learning`, permalink `/learning/`
- [x] 2.2 Create `_includes/learning-item.html` — card template for a single resource (mirrors `tool-item.html` with classes `.resource`, `data-resource-name`, `data-resource-desc`, `data-resource-tags`)
- [x] 2.3 Create `_layouts/learning.html` — full page layout with search input, category grouping from `site.learning`, and client-side search JS (mirrors `tools.html`)

## 3. Styling

- [x] 3.1 Create `assets/css/learning.css` — card-grid styles using Kami design tokens, with class renames (`.resources-page`, `.resource-category`, `.resource-list`, `.resource`) and dark mode support

## 4. Documentation

- [x] 4.1 Update `AGENTS.md` — document `_learning/` collection with valid categories (Curated List, Course, Book, Guide, Tutorial, Research Paper) and tag guidelines for learning resources

## 5. Verification

- [x] 5.1 Confirm `learning.md` appears in header navigation automatically (via `site.pages`)
- [x] 5.2 Confirm `/tools/` page no longer shows "Learning Resource" category
- [x] 5.3 Confirm `/learning/` page shows all 38 entries in a "Learning Resource" category section with search and card grid
- [x] 5.4 Verify visual consistency — light mode, dark mode, card hover, responsive grid
