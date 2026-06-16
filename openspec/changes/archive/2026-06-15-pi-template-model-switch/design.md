## Context

Pi is a terminal coding harness that supports extensions — TypeScript modules that hook into pi's event system to add tools, intercept commands, modify prompts, and more. Extensions can be shared as **pi packages**: npm or git repositories with a `package.json` containing a `pi` manifest key that points to extension directories.

The `template-model-switch` extension already exists and works at `~/.pi/agent/extensions/template-model-switch.ts`. It intercepts prompt template invocations (e.g., `/review`), reads a `model:` field from the template's YAML frontmatter, switches pi to that model for the duration of that single prompt execution, then restores the original model afterward.

This change publishes the extension as a standalone GitHub repository structured as a pi package, and writes a companion blog post on `ypyl.github.io`.

**Constraints:**
- The extension code itself is feature-complete and does not need modification
- The GitHub CLI (`gh`) is authenticated as `ypyl` with repo scope
- The blog repo follows the Kami design system and Jekyll conventions

## Goals / Non-Goals

**Goals:**
- Create a clean, minimal GitHub repo at `github.com/ypyl/pi-template-model-switch`
- Structure it as a valid pi package installable via `pi install git:github.com/ypyl/pi-template-model-switch`
- Include a README with installation, usage, and a concrete example
- Include a LICENSE file (MIT, matching pi's license)
- Write a blog post explaining the motivation, design, and usage

**Non-Goals:**
- Modifying the extension code (it works as-is)
- Including prompts, skills, or other pi customizations (scope is just this extension)
- Adding tests, CI, or npm publication (those can be follow-up work)
- Publishing to npm registry (git-only installation is sufficient)

## Decisions

### 1. Repo structure: flat with `extensions/` directory

The extension is a single file, but pi package convention uses an `extensions/` directory (even for one file) to allow future additions.

```
pi-template-model-switch/
├── README.md
├── LICENSE
├── .gitignore
├── package.json
└── extensions/
    └── template-model-switch.ts
```

**Alternative considered:** Flat structure with just `template-model-switch.ts` at repo root. Rejected because `package.json`'s `pi.extensions` array expects directory paths, and the convention is to separate code from metadata.

### 2. `package.json` design: minimal manifest

```json
{
  "name": "pi-template-model-switch",
  "version": "1.0.0",
  "description": "Pi extension: switch LLM models per prompt template via YAML frontmatter",
  "keywords": ["pi-package", "pi-extension"],
  "license": "MIT",
  "pi": {
    "extensions": ["./extensions"]
  }
}
```

- `"pi-package"` keyword makes it discoverable on npmjs.com
- `"pi-extension"` keyword indicates it provides extensions (not prompts/skills/themes)
- No `"files"` field needed — git repos install the whole tree
- No dependencies — the extension uses only type imports from `@earendil-works/pi-coding-agent` which are resolved by pi's runtime

### 3. README sections: problem, install, usage, example

The README is the package's documentation. It should be self-contained and useful to someone who finds the repo without the blog post.

Sections:
1. **What it does** — one-paragraph summary
2. **Installation** — `pi install git:github.com/ypyl/pi-template-model-switch`
3. **Usage** — how to add `model:` to prompt template frontmatter, format (`provider/model-id`)
4. **Example** — a complete prompt template showing before/after
5. **How it works** — brief technical explanation (hooks `input`, `before_agent_start`, `agent_end`)
6. **License** — MIT

### 4. Blog post: standalone explanation, not a duplicate of README

The blog post tells a story: the problem, the discovery, the solution. It links to the repo for installation details rather than duplicating them.

Structure:
- Title: something like "Per-Prompt Model Switching for Pi Coding Agent"
- Intro: the friction of manual model switching
- The extension: how it works, with code snippets
- Demo: a concrete example with a prompt template
- Installation: one-liner with link to repo
- Closing thoughts

### 5. License: MIT

Pi itself is MIT. The extension is a derivative work that runs within pi's extension API. MIT keeps it simple and compatible.

## Risks / Trade-offs

- **Risk:** `pi.getCommands()` API used by the extension may change in future pi versions → **Mitigation:** The extension is simple enough to adapt quickly; pi's extension API is documented and versioned
- **Trade-off:** Not publishing to npm means discovery is limited to GitHub search and the blog post → **Acceptable:** The blog post is the primary discovery mechanism, and npm publication can be added later
- **Risk:** The `model:` format (`provider/model-id`) has no validation against available models → **Mitigation:** The extension already handles this gracefully — it notifies the user with an error if the model isn't found
