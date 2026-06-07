## Context

Tags were recently added to post pages and the home page post list. Both use the same global `.tag` style (default Kami tier: `#E4ECF5` background, 0.6875rem/11px, 3px/8px padding). On the post page, tags render in a separate `<div class="tags">` below the date line. On the home page, tags appear below the post title in the same style.

The Kami design system (tw93/Kami) defines three tag tiers for different contexts. The site already has both `--tag-bg` (#E4ECF5, default tier) and `--tag-bg-light` (#EEF2F7, lightest tier) defined as CSS custom properties.

## Goals / Non-Goals

**Goals:**
- Move post page tags inline after the date with a `·` separator
- Apply the Kami lightest tag tier to home page post list tags
- Keep post page tags at the default Kami tier

**Non-Goals:**
- Changing tag design on the tools page
- Making tags clickable
- Adding tags to news cards

## Decisions

### Decision 1: Post page tags inline via `.post-meta .tags { display: inline-flex }`

Moving the tag loop inside the `<p class="post-meta">` paragraph and using `display: inline-flex` on the wrapper keeps tags on the same line as the date. The `·` separator follows the same pattern as the existing author separator.

**Alternatives considered:**
- Wrap each `<span class="tag">` directly in the `<p>` without a `.tags` wrapper — loses the flex gap between tags
- Use `display: inline` on `.tags` — loses flex-wrap and gap benefits

### Decision 2: Home page tags use Kami lightest tier

Overriding `.post-list .tag` with `background: var(--tag-bg-light)`, `font-size: 0.625rem`, and `padding: 2px 6px` applies the Kami "lightest solid" tier. This makes home page tags visually subordinate to post titles while maintaining readability.

### Decision 3: Scoped CSS overrides, not modifier classes

Using `.post-list .tag` and `.post-meta .tags` selectors scopes the overrides to specific contexts without changing the markup. This keeps the Liquid templates clean and avoids adding modifier classes that would need to be maintained across layouts.

## Risks / Trade-offs

- **[Risk] Long tag lists wrap to next line on post page** → Mitigation: `.tags` already has `flex-wrap: wrap`; inline-flex preserves this behavior. Tags will naturally wrap below the date if needed.
- **[Risk] Home page tags might be too small** → Mitigation: 10px (0.625rem) aligns with Kami's "Label" tier (9pt print ≈ 12px screen, lightest variant slightly smaller). Readable and intentional.
