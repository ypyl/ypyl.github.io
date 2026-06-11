## Context

Post tag data is now normalized to consistent YAML array format (`tags: [dotnet, ai, caching]`) across all ~57 posts with tags. The tools page already renders tag badges visually using `.tag` / `.tags` CSS classes defined in `assets/css/tools.css`. These styles use Kami design tokens (`var(--tag-bg)`, `var(--brand)`) that work in both light and dark modes. The post and home page layouts do not reference tags at all.

## Goals / Non-Goals

**Goals:**
- Display tag badges on post pages (in header, below date)
- Display tag badges on home page post list items (below title)
- Make `.tag` / `.tags` styles globally available via `style.css`
- Skip rendering tags on posts that have none
- Match the existing tools page tag visual style exactly

**Non-Goals:**
- Clickable tags or tag-based navigation
- Tag archive pages or filtering
- Displaying tags on news cards (news no longer have tags)
- Changing the tag badge design from what tools page uses

## Decisions

### Decision 1: Move tag CSS to style.css

Move the `.tags` and `.tag` style blocks from `tools.css` to `style.css`. The styles are design-token-based and work globally. The tools page already includes `style.css` as the base stylesheet, so removing them from `tools.css` won't break the tools page. Having them in `style.css` makes them available to post and home layouts without adding extra stylesheet references.

**Alternatives considered:**
- Duplicate in both files — DRY violation, maintenance burden
- Import `tools.css` on post pages — unnecessary coupling, tools.css contains tool-specific styles beyond tags

### Decision 2: Use {% for tag in page.tags %} / {% for tag in post.tags %}

Jekyll's `post.tags` (or `page.tags` in post layout) is always an array after normalization. The loop is straightforward:

```liquid
{% if page.tags.size > 0 %}
  <div class="tags">
    {% for tag in page.tags %}
      <span class="tag">{{ tag }}</span>
    {% endfor %}
  </div>
{% endif %}
```

The guard (`if page.tags.size > 0`) prevents rendering an empty `<div class="tags">` on posts without tags.

### Decision 3: Tag placement

- **Post page**: In `post-header`, after the `<p class="post-meta">` (date/author). Tags are metadata, so they belong in the header with the date.
- **Home page**: In post list items, below the `<h3>` title link. The date already appears above the title; tags below the title create a natural sandwich: date → title → tags.

## Risks / Trade-offs

- **[Risk] Posts without tags still show empty tag area** → Mitigation: Guard with `if page.tags.size > 0`.
- **[Risk] Long tag lists wrap unattractively** → Mitigation: `.tags` uses `flex-wrap: wrap` with `gap: 6px`; multiline wrapping is already handled by the tools card layout.
- **[Risk] tools.css still references `.tags`/`.tag` after removal** → Mitigation: The tools page includes `style.css` as its base stylesheet, so the moved styles remain available. Verify tools page renders correctly after the move.
