## Why

Pi lacks built-in per-prompt model switching. When you invoke `/review` you might want Claude Sonnet, but `/brainstorm` benefits from a reasoning model. Without this extension, you have to manually switch with `/model` before and after every prompt invocation — tedious and error-prone. The `template-model-switch` extension solves this by reading a `model:` field from prompt template YAML frontmatter and swapping models automatically for that single execution.

Publishing this extension as a standalone GitHub repo (a pi package) makes it installable by anyone, and the companion blog post documents the pattern for the pi community.

## What Changes

- Create a new GitHub repository `pi-template-model-switch` containing:
  - The `template-model-switch.ts` extension (existing, already working)
  - A `package.json` declaring it as a pi package with the `pi-package` keyword
  - README.md with installation and usage instructions
  - LICENSE (MIT)
- Create a blog post `_posts/YYYY-MM-DD-template-model-switch.md` explaining the problem, the extension's design, and how to install it

## Capabilities

### New Capabilities

- `pi-extension-package`: The GitHub repository itself — a properly structured pi package that anyone can install via `pi install git:github.com/ypyl/pi-template-model-switch`
- `blog-post`: A blog post on `ypyl.github.io` that explains the extension, its motivation, and usage

### Modified Capabilities

None. This is a new, self-contained change — no existing specs are affected.

## Impact

- New GitHub repo created under `ypyl/pi-template-model-switch` (via `gh repo create`)
- New file added to this blog repo: `_posts/2026-06-15-template-model-switch.md`
- No code changes to the blog site itself
