## 1. Configure collection

- [x] 1.1 Add `news` collection to `_config.yml` with `output: true` and permalink `/news/:year/:month/:day/:title/`
- [x] 1.2 Create `_news/` directory at repository root

## 2. Migrate files

- [x] 2.1 Move all 186 `.md` files from `_posts/news/` to `_news/`
- [x] 2.2 Remove `categories: news` line from each file's frontmatter
- [x] 2.3 Remove `tags: [news]` line from each file's frontmatter
- [x] 2.4 Remove empty `_posts/news/` directory

## 3. Update templates

- [x] 3.1 Replace news filtering loop in `_layouts/home.html` with direct `site.news` access
- [x] 3.2 Remove `categories contains "news"` filter from blog post list loop in `home.html`

## 4. Update documentation

- [x] 4.1 Update AGENTS.md content structure section: replace `_posts/news/` with `_news/` collection
- [x] 4.2 Update AGENTS.md "New news" instructions to reflect `_news/` directory and stripped frontmatter

## 5. Verify

- [x] 5.1 Verify `_news/` contains exactly 186 files
- [x] 5.2 Verify no `categories: news` or `tags: [news]` remain in any `_news/` file
- [x] 5.3 Verify `_posts/news/` directory no longer exists
- [x] 5.4 Push to GitHub and verify GitHub Pages builds successfully
