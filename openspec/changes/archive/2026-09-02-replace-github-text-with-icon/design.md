## Context

The header navigation (`_includes/header.html`) contains a text link "GitHub" using the `page-link` class. The navigation already uses inline SVGs for other elements (hamburger menu icon, theme toggle sun/moon icons, presentation link icon). This change follows the same pattern.

The openspec-viewer project uses the same GitHub octocat SVG with proven styling. We adopt that approach.

## Goals / Non-Goals

**Goals:**
- Replace text "GitHub" with an inline SVG octocat icon
- Maintain visual consistency with existing nav styling
- Preserve accessibility (title, aria-label)
- Work in both light and dark themes

**Non-Goals:**
- Changing the link target (still goes to `https://github.com/ypyl`)
- Adding animation or interaction effects beyond hover color change
- Creating a reusable icon component (overkill for one icon)

## Decisions

### SVG Source
**Decision**: Use the standard GitHub octocat SVG path from openspec-viewer.

**Rationale**: Proven path, clean rendering at small sizes, matches GitHub's own branding. The path data is:
```
M6.766 11.328c-2.063-.25-3.516-1.734-3.516-3.656 0-.781.281-1.625.75-2.188...
```

**Alternative considered**: External `<img>` or icon font. Rejected because inline SVG allows `currentColor` inheritance and avoids extra HTTP requests.

### Icon Size
**Decision**: 16×16px viewBox rendered at 16×16px.

**Rationale**: The nav text is 15px font size. A 16px icon sits naturally alongside without dominating. The openspec-viewer uses 20px but its rail has 40px buttons; our nav links are smaller.

### CSS Approach
**Decision**: Add a dedicated `.github-icon` class for sizing, keep `page-link` for layout.

**Rationale**: The `.page-link` class sets `line-height: 56px` which vertically centers text. The SVG needs explicit dimensions. A separate class avoids overriding shared styles.

**Alternative considered**: Style via `.page-link svg`. Rejected because it would affect any future SVGs added to nav links.

### Color Inheritance
**Decision**: Use `fill="currentColor"` on the SVG.

**Rationale**: The existing nav links change color on hover via `color: var(--brand)`. With `currentColor`, the SVG automatically inherits this behavior without additional CSS.

## Risks / Trade-offs

**[Risk] Icon too small on mobile** → Mitigation: Mobile nav (`max-width: 600px`) displays links as block elements with larger touch targets. The icon at 16px is still tappable within the 56px line-height.

**[Risk] Visual weight mismatch** → Mitigation: The icon is roughly the same visual weight as 6 characters of text. It won't feel "heavier" than the other nav links.

**[Risk] Users expect text** → Mitigation: The GitHub octocat is universally recognized. The `title` attribute provides tooltip confirmation on hover.
