## 1. Create GitHub Repository

- [x] 1.1 Create local directory for the pi package repo
- [x] 1.2 Initialize git repository in the new directory
- [x] 1.3 Create the GitHub repository via `gh repo create ypyl/pi-template-model-switch --public`

## 2. Extension Package Files

- [x] 2.1 Create `package.json` with pi manifest declaring `extensions/` directory, `pi-package` and `pi-extension` keywords, and MIT license
- [x] 2.2 Copy `template-model-switch.ts` from `~/.pi/agent/extensions/` into `extensions/` directory (verbatim, no modifications)
- [x] 2.3 Create `LICENSE` file with MIT license text
- [x] 2.4 Create `.gitignore` (exclude `node_modules/`, `.DS_Store`)
- [x] 2.5 Write `README.md` with sections: what it does, installation, usage, example, how it works, license

## 3. Push and Verify

- [x] 3.1 Commit all files with a descriptive message
- [x] 3.2 Push to GitHub and verify the repository is visible at `https://github.com/ypyl/pi-template-model-switch`

## 4. Blog Post

- [x] 4.1 Create `_posts/2026-06-15-template-model-switch.md` with Jekyll frontmatter (`layout: post`, `title`, `date: 2026-06-15`, `tags: [pi, extension, prompt-engineering, open-source]`, `categories: ai`)
- [x] 4.2 Write post content: problem statement, extension introduction, code snippet, demo with prompt template, installation link to repo
- [x] 4.3 Verify post renders correctly by checking frontmatter format and Kami design compliance
