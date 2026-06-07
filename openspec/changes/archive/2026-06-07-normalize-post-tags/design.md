## Context

The blog has ~310 posts across `_posts/` and `_posts/news/` with tag frontmatter in multiple inconsistent formats. Jekyll's frontmatter spec (jekyllrb.com/docs/front-matter/) defines two valid tag formats: YAML lists (`tags: [a, b]`) and space-separated strings (`tags: a b`). Comma-separated strings (`tags: a, b`) are NOT valid Jekyll — they produce a single tag containing literal commas. The tools collection (`_tools/`) already uses YAML array format consistently.

No local Jekyll build is available. GitHub Pages auto-builds on push to main, so breakage would only be detected post-push.

## Goals / Non-Goals

**Goals:**
- Convert all post tag frontmatter to YAML array format: `tags: [tag1, tag2]`
- Fix ~30 comma-separated posts that are currently broken in Jekyll
- Ensure single-word tags are wrapped in arrays (e.g., `tags: dotnet` → `tags: [dotnet]`)
- Preserve semantic content — don't lose or alter tag meaning
- Update AGENTS.md to mandate the canonical format

**Non-Goals:**
- Displaying tags on pages (separate follow-up change)
- Creating tag archive pages or tag-based navigation
- Normalizing space-separated posts with multi-word tag ambiguity (optional phase)
- Changing tools or learning resource frontmatter (already consistent)
- Running Jekyll locally for validation

## Decisions

### Decision 1: YAML array format as canonical target

**Rationale:** YAML arrays are unambiguous (no space-vs-comma confusion), match the existing tools collection convention, and are explicitly supported by Jekyll. Space-separated strings are also valid Jekyll but introduce ambiguity for multi-word tags like "machine learning".

**Alternatives considered:**
- Space-separated strings: Valid Jekyll but ambiguous for multi-word phrases; requires harder heuristic splitting
- Comma-separated strings: Not valid Jekyll; ruled out immediately

### Decision 2: Three-phase normalization approach

1. **Automated fix for comma-separated posts** — Split on `, ` into array items, trim whitespace. These are unambiguously broken.
2. **Automated wrap for single-word posts** — Wrap single-word tags in array brackets. Purely mechanical.
3. **Optional manual review for space-separated posts** — These already work in Jekyll (split on spaces). Only need review if the space-split creates semantically wrong splits (e.g., "machine learning" → "machine" + "learning").

### Decision 3: No Jekyll validation in the change

Since there's no local Jekyll build, validation is limited to manual inspection. The change is a pure data transformation with no code or template changes, so risk of breaking the build is minimal. If a YAML syntax error slips through, GitHub Pages build logs will report it.

### Decision 4: Use PowerShell script for transformation

Windows environment. A PowerShell script will:
- Scan all `_posts/*.md` and `_posts/news/*.md` files
- Parse YAML frontmatter, extract tags field
- Detect format (single-word, comma-separated, space-separated, YAML array)
- Transform to YAML array format
- Write back the frontmatter block

## Risks / Trade-offs

- **[Risk] Mass file edit corrupts frontmatter** → Mitigation: Script only touches the `tags:` line; version control allows rollback; commit before running.
- **[Risk] Space-separated posts split intended compound tags** → Mitigation: Separate optional review phase; can defer or skip entirely since these already work in Jekyll.
- **[Risk] Edge cases missed** (posts without tags, empty tags, unusual characters) → Mitigation: Script handles missing tags gracefully; manual spot-check of 10% of files after transformation.
- **[Risk] Multi-line YAML frontmatter** → Mitigation: Script uses regex anchored to `^tags:` line, won't touch other frontmatter lines.
