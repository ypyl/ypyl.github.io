## 1. CSS: Home page lighter tags

- [x] 1.1 Add `.post-list .tag` override to `assets/css/style.css` with smaller font-size (0.625rem), tighter padding (2px 6px), and light background (`var(--tag-bg-light)`)

## 2. CSS: Post page inline tags

- [x] 2.1 Add `.post-meta .tags` override to `assets/css/style.css` with `display: inline-flex`

## 3. Layout: Post page inline tags

- [x] 3.1 Move tag loop inside `<p class="post-meta">` in `_layouts/post.html`, after the date/author block, with `·` separator
- [x] 3.2 Remove the standalone `<div class="tags">` block from post.html

## 4. Verify

- [x] 4.1 Verify post page: tags appear inline after date with `·` separator
- [x] 4.2 Verify home page: tags use lighter tier (smaller, lighter bg)
- [x] 4.3 Verify tools page: tags unaffected by overrides
- [ ] 4.4 Push to GitHub and verify GitHub Pages builds successfully
