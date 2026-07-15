## Context

The site is a Jekyll blog hosted on GitHub Pages (`ypyl/ypyl.github.io`). Posts are in `_posts/` rendered via `_layouts/post.html`. The `_config.yml` already has `github_username: ypyl`. Jekyll exposes `page.path` with the source file path relative to the repo root (e.g., `_posts/2025-01-15-my-post.md`).

No new dependencies, config changes, or external services needed.

## Goals / Non-Goals

**Goals:**
- Add a "View source" link to each blog post's meta line, linking to the Markdown source on GitHub (blob view)
- Use existing `site.github_username` and `page.path` — no new config

**Non-Goals:**
- "Edit this page" link (edit view on GitHub)
- Link for non-post pages (tools, learning, news, presentations)
- History/blame links

## Decisions

**Decision: Plain "View source" text link in the meta line**

The link goes at the end of the existing `<p class="post-meta">` in `_layouts/post.html`, after tags, separated by `·`. It links to:

```
https://github.com/{{ site.github_username }}/{{ site.github_username }}.github.io/blob/main/{{ page.path }}
```

**Alternatives considered:**
- *GitHub icon (Octicon SVG)*: More visual but adds inline SVG noise. A text link is simpler and consistent with the existing meta-line style.
- *Floating corner button*: Over-engineered for the goal; the meta line is already where metadata lives.
- *Edit link instead of View*: User explicitly wants View for copy-paste of Markdown; Edit adds friction (forces fork flow).

**Decision: Use `page.path` directly — no collection-specific logic**

`page.path` works for all Jekyll pages/collections. Scoping to only posts is handled naturally by only adding the link in `post.html`.

## Risks / Trade-offs

- [Link rot if repo is renamed] → Mitigation: renaming a GitHub Pages user repo breaks the entire site anyway, so this is not a new risk.
- [Link visible on local dev without GitHub context] → Mitigation: the link always points to GitHub, which is correct for the published site. Local dev is read-only preview anyway.
