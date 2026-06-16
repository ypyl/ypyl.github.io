## 1. Revert markdown changes

- [x] 1.1 Restore `bash` language identifier on 3 fenced code blocks in `_presentations/eval-cli-cross-platform-ai-evaluation.md` (single scenario, multi-run, aieval report blocks)
- [x] 1.2 Restore `typescript` and `tsx` language identifiers on 2 fenced code blocks in `_presentations/React-SPA-v2.md` (API client, component rules blocks)

## 2. Add JS bridge to presentation layout

- [x] 2.1 In `_layouts/presentation.html`, insert bridge code block after the section-building `forEach` loop and before `Reveal.initialize()`: query all `pre code` elements, find Kramdown wrapper via `closest('div.highlighter-rouge')`, extract language from `language-*` class, strip Rouge spans via `code.textContent = code.textContent` when children exist, add `language-<name>` class to `<code>` element
- [x] 2.2 Ensure `language-plaintext` blocks are skipped (no language class added, no stripping needed)

## 3. Update documentation

- [x] 3.1 Update `AGENTS.md` presentation code block guidance: remove "bare fences only" rule, replace with note that language identifiers are used and a JS bridge strips Rouge spans for highlight.js

## 4. Verify

- [x] 4.1 Verify locally that bash code blocks in `eval-cli-cross-platform-ai-evaluation` render with highlight.js spans (`.hljs-built_in`, `.hljs-string`, `.hljs-variable`) and no Rouge spans (`.nb`, `.nt`, `.se`)
- [x] 4.2 Verify that `dotnet aieval report` block renders with bash grammar (plain text for unrecognized commands) not Elm detection (no red `port` keyword)
- [x] 4.3 Verify TypeScript/TSX blocks in `React-SPA-v2` render with TypeScript grammar
- [x] 4.4 Verify bare fenced blocks (directory listings, output samples) continue to work via auto-detection
- [x] 4.5 Verify blog post code blocks are unaffected (still use Rouge highlighting)
