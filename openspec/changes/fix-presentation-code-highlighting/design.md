## Context

The presentation page uses two highlighting systems that conflict:

1. **Jekyll/Kramdown** pre-processes fenced code blocks with language identifiers (e.g., ````bash`) using **Rouge**, producing DOM spans like `<span class="nb">rm</span>`. This happens at build time and is served in the HTML as `{{ content }}`.

2. **Reveal.js highlight plugin** (highlight.js + monokai theme) expects bare `<pre><code>` elements and applies its own `<span class="hljs-*">` elements at runtime.

When both run, highlight.js skips code blocks that already have child elements (Rouge spans), leaving Rouge's spans in the DOM with no matching monokai CSS — resulting in either monochrome Kami-palette styling (if `style.css` is loaded) or unstyled tokens.

If language identifiers are removed from markdown to prevent Rouge from running, highlight.js auto-detects the wrong language (e.g., `dotnet aieval report` detects as Elm, not bash), causing incorrect token coloring.

The JS bridge approach keeps language identifiers so Jekyll/Kramdown preserves the language hint in the wrapper `div.language-*` class, then strips Rouge spans and passes the language to highlight.js at runtime.

## Goals / Non-Goals

**Goals:**
- Code blocks in presentations render with highlight.js + monokai theme using the correct language
- Language identifiers (`bash`, `typescript`, `tsx`) in markdown drive which grammar highlight.js uses
- No visible Rouge spans or incorrect token colors in the DOM
- No changes to how code blocks work on non-presentation pages (blog posts, tools, learning)
- Bare fenced blocks (no language) continue to auto-detect

**Non-Goals:**
- Changing the monokai theme or adding light-mode code block theming (pre-existing concern)
- Fixing highlight.js auto-detection quality for bare blocks
- Changing how Jekyll/Kramdown processes code blocks site-wide
- Adding a build step or npm dependency

## Decisions

### Bridge placement: after section injection, before `Reveal.initialize()`

**Rationale**: At this point all `<section>` elements are in the DOM (added by the `forEach` loop), so `querySelectorAll` finds them. highlight.js hasn't run yet because `Reveal.initialize()` hasn't been called. Single pass, no timing issues.

**Alternative considered**: Pre-process the raw HTML string before splitting into sections. Rejected — regex on arbitrary HTML is fragile (nested divs, edge cases with string content containing div-like patterns).

### Language extraction: from Kramdown's wrapper `div.language-*` class

Kramdown wraps fenced code blocks as:
```html
<div class="language-bash highlighter-rouge">
  <div class="highlight">
    <pre class="highlight">
      <code><span class="nb">rm</span> ...</code>
    </pre>
  </div>
</div>
```

The JS uses `code.closest('div.highlighter-rouge')` to find the wrapper, then regex `/(language-(\w[\w-]*))/` to extract the language name. This couples to Kramdown's wrapper class but is a stable convention — `highlighter-rouge` has been the Kramdown wrapper class for years.

**Alternative considered**: Parse language from `<code>` element's own class. Rejected — Kramdown doesn't add language classes directly to `<code>`, and `class="language-plaintext highlighter-rouge"` appears on the wrapper div only.

### Rouge span removal: `code.textContent = code.textContent`

Setting `textContent` strips all child elements (Rouge `<span>` nodes) and replaces them with a single text node containing the exact code text. Whitespace, newlines, and special characters are preserved exactly.

**Alternative considered**: `code.innerHTML = code.textContent`. Rejected — this HTML-encodes `<`, `>`, `&` characters in the code text, which changes what highlight.js processes.

**Alternative considered**: Remove child spans one-by-one via `while (code.firstChild) code.removeChild(code.firstChild)` then append a text node. Rejected — more code for the same result; `textContent` setter is idempotent and well-tested.

### Plaintext guard: skip when language is `plaintext`

When a code block has no language identifier in markdown, Kramdown outputs `class="language-plaintext"` on the wrapper. The bridge skips these blocks entirely — no stripping, no language class added. highlight.js auto-detects as before. This preserves the existing behavior for output blocks, directory listings, and other non-code fenced blocks in presentations.

### Language alias compatibility

highlight.js registers common aliases (e.g., `tsx` → `typescript`, `sh` → `bash`). No mapping layer needed — the language name from Kramdown is passed directly to `code.classList.add('language-' + lang)` and highlight.js resolves it.

## Risks / Trade-offs

**[Risk] Kramdown output structure changes** → Bridge silently fails (no language class added), code falls through to auto-detection. Mitigation: `if (!wrapper) return;` guard makes failure graceful. Kramdown's wrapper structure has been stable for years; risk is low.

**[Risk] `code.children.length > 0` check misses edge case** → A code block with only a text node child but no spans would still get language hint (correct). A code block with Rouge spans in nested wrappers (non-standard) might not be detected. Mitigation: Rouge always produces flat `<span>` children inside `<code>`; this is a stable invariant.

**[Risk] Non-Rouge pre-highlighted code** → If someone adds raw HTML `<pre><code>` with pre-existing spans (not Rouge), `textContent` stripping would destroy their manual highlighting. Mitigation: only blocks inside `div.highlighter-rouge` are touched; raw HTML blocks without Kramdown wrappers are skipped.

**[Trade-off] Sparse bash highlighting on non-built-in commands** → bash grammar only highlights known built-ins (`rm`, `cd`, `echo`), strings, and variables. Commands like `dotnet` or `eval-cli` get no highlight color. This is correct but looks plain. The alternative (auto-detection → Elm) gave wrong but colorful results. Chose correctness.

**[Trade-off] Coupling to Kramdown** → The bridge depends on Kramdown's wrapper classes. If the site ever switches markdown processors, this code needs updating. Scope is contained to one JS block in one layout file; low maintenance burden.
