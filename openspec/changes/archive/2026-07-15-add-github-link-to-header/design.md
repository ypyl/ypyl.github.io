## Context

The site header (`_includes/header.html`) renders navigation links by iterating over `site.pages` inside a `.trigger` div. Currently this produces three links: Home, Learning, Tools. The `_config.yml` already has `github_username: ypyl`.

Since the GitHub link is an external URL (not a Jekyll page), it can't be added via the existing page-driven loop. It must be a hardcoded `<a>` element inserted after the loop inside the `.trigger` div.

## Goals / Non-Goals

**Goals:**
- Add a "GitHub" link to the header navigation, after existing page links
- Link to `https://github.com/{{ site.github_username }}` (resolves to `https://github.com/ypyl`)
- Match the visual style of existing `.page-link` elements

**Non-Goals:**
- GitHub icon or octicon branding
- Additional external links (LinkedIn, etc.)
- Modifying the mobile nav behavior

## Decisions

**Decision: Hardcoded `<a>` with class `page-link` after the dynamic loop**

The link is placed inside `.trigger`, immediately after `{% endfor %}`, using the existing `.page-link` CSS class for visual consistency. The URL uses `site.github_username` from config, matching the pattern used in the earlier `add-github-source-link` change.

**Alternatives considered:**
- *Create a Jekyll page with external redirect*: Over-engineered for a single link; adds an unnecessary page file.
- *Add a custom collection for external nav links*: Unwarranted abstraction for one link.
- *Use an SVG icon instead of text*: More visual noise; text "GitHub" is clear and consistent with other nav items.

## Risks / Trade-offs

- [Link breaks if `github_username` changes] → Mitigation: the config key is stable; changing it would also break the GitHub Pages URL itself.
- [Mobile nav overflow with extra link] → Mitigation: the current nav only has 3 links; adding a 4th is well within the responsive layout's capacity.
