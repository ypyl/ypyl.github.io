## Why

Presentation code blocks lack proper syntax highlighting. Jekyll's Kramdown processor pre-highlights fenced code blocks with Rouge before the page reaches the browser, producing `<span class="nb">` etc. that conflict with Reveal.js's highlight.js plugin. If language identifiers are removed to avoid Rouge, highlight.js auto-detects the wrong language (e.g., Elm instead of bash), causing bizarre token coloring. The presentation needs a bridge that strips Rouge's pre-highlighting and hands clean text with language hints to highlight.js.

## What Changes

- Add a JavaScript bridge in `_layouts/presentation.html` that runs between slide injection and Reveal.js initialization: strips Rouge spans from `<pre><code>` blocks, and copies the language identifier from Kramdown's wrapper div (e.g., `language-bash`) to the `<code>` element as a highlight.js hint
- Revert `_presentations/` files to use language identifiers on fenced code blocks (`bash`, `typescript`, `tsx`)
- Update `AGENTS.md` documentation to reflect that language identifiers should be used on presentation code blocks (JS bridge handles the rest)

## Capabilities

### New Capabilities
- `presentation-code-highlighting`: Proper syntax highlighting in presentation slides using highlight.js with the monokai theme, bridged from Jekyll/Rouge's pre-processed output

### Modified Capabilities
<!-- None — this change adds new capability without changing existing spec requirements -->

## Impact

- `_layouts/presentation.html` — new JS block (~15 lines) between slide building and `Reveal.initialize()`
- `_presentations/eval-cli-cross-platform-ai-evaluation.md` — revert 3 code blocks to use `bash` language identifier
- `_presentations/React-SPA-v2.md` — revert 2 code blocks to use `typescript` and `tsx` language identifiers
- `AGENTS.md` — update presentation code block guidance
- No API, dependency, or config changes
