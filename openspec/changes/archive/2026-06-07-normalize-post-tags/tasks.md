## 1. Prepare

- [x] 1.1 Commit or stash any pending changes to ensure clean rollback point
- [x] 1.2 Create the PowerShell normalization script at `normalize-tags.ps1`

## 2. Automate: Fix comma-separated and single-word posts

- [x] 2.1 Run script to convert all comma-separated tags to YAML arrays (e.g., `tags: dotnet, ai` → `tags: [dotnet, ai]`)
- [x] 2.2 Run script to wrap all single-word tags in array brackets (e.g., `tags: dotnet` → `tags: [dotnet]`)
- [x] 2.3 Verify YAML arrays with quoted strings are preserved (e.g., `tags: ["high-load"]`)
- [x] 2.4 Verify posts without tags are unchanged
- [x] 2.5 Verify news posts (`tags: news` → `tags: [news]`) are all converted

## 3. Verify

- [x] 3.1 Run `git diff --stat` to confirm expected number of files changed (~215 single-word + ~30 comma-separated = ~245 files)
- [x] 3.2 Spot-check 10% of changed files across both `_posts/` and `_posts/news/` for correct transformation
- [x] 3.3 Verify no other frontmatter fields were modified or corrupted
- [x] 3.4 Push to GitHub and verify GitHub Pages builds successfully

## 4. Update documentation

- [x] 4.1 Update AGENTS.md post frontmatter example to show `tags: [tag1, tag2]` instead of `tags: tag1, tag2`
- [x] 4.2 Add a note in AGENTS.md that tags must use YAML array format (consistent with tools collection)
