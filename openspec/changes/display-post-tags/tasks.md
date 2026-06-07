## 1. Move tag styles to global CSS

- [x] 1.1 Copy `.tags` and `.tag` style blocks from `assets/css/tools.css` to `assets/css/style.css`
- [x] 1.2 Remove `.tags` and `.tag` style blocks from `assets/css/tools.css`

## 2. Add tags to post page

- [x] 2.1 Add tag badge loop to `_layouts/post.html` in the post header, below date/author, guarded by `{% if page.tags.size > 0 %}`
- [x] 2.2 Verify: post with tags renders badges; post without tags renders no empty container

## 3. Add tags to home page

- [x] 3.1 Add tag badge loop to `_layouts/home.html` post list items, below the title link, guarded by `{% if post.tags.size > 0 %}`
- [x] 3.2 Verify: tagged posts show badges in list; untagged posts show no badges

## 4. Verify

- [x] 4.1 Verify tools page tag badges render correctly after CSS move (no visual regression)
- [x] 4.2 Spot-check a post page with tags and a post page without tags
- [ ] 4.3 Push to GitHub and verify GitHub Pages builds successfully
