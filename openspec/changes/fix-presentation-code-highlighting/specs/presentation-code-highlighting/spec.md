## ADDED Requirements

### Requirement: highlight.js receives language hint from Kramdown wrapper
The presentation layout SHALL extract the language identifier from Kramdown's wrapper `div.language-*` class on fenced code blocks and pass it to highlight.js by adding `language-<name>` to the `<code>` element before Reveal.js initializes. highlight.js SHALL use this class to select the correct grammar, avoiding auto-detection.

#### Scenario: Bash code block receives bash grammar
- **WHEN** a presentation file contains a fenced code block with `bash` language identifier
- **AND** the page loads
- **THEN** the `<code>` element has class `language-bash` before highlight.js runs
- **AND** highlight.js applies the bash grammar
- **AND** recognized bash built-ins (e.g., `rm`, `echo`) receive `.hljs-built_in` class

#### Scenario: TypeScript code block receives typescript grammar
- **WHEN** a presentation file contains a fenced code block with `typescript` identifier
- **AND** the page loads
- **THEN** the `<code>` element has class `language-typescript` before highlight.js runs
- **AND** highlight.js applies the TypeScript grammar

#### Scenario: TSX code block resolves to TypeScript via alias
- **WHEN** a presentation file contains a fenced code block with `tsx` identifier
- **AND** the page loads
- **THEN** the `<code>` element has class `language-tsx` before highlight.js runs
- **AND** highlight.js resolves `tsx` as a TypeScript alias and applies the TypeScript grammar

### Requirement: Rouge pre-highlighting is stripped before highlight.js runs
The presentation layout SHALL strip all child `<span>` elements from `<pre><code>` blocks that were pre-highlighted by Kramdown/Rouge before highlight.js processes them. The stripping SHALL preserve the exact code text content including whitespace, newlines, and special characters.

#### Scenario: Rouge spans removed from bash block
- **WHEN** Jekyll serves a presentation with a `bash` fenced code block containing `rm -rf eval-results`
- **AND** Kramdown produces `<code><span class="nb">rm</span> <span class="nt">-rf</span> eval-results</code>`
- **THEN** before highlight.js runs, the `<code>` element contains only the text `rm -rf eval-results` with no child `<span>` elements
- **AND** highlight.js can tokenize the clean text

#### Scenario: Code text content is preserved exactly
- **WHEN** a code block contains special characters (`<`, `>`, `&`, `$`)
- **AND** Rouge has wrapped portions in `<span>` elements
- **THEN** after stripping, the text content matches the original code exactly
- **AND** highlight.js correctly highlights the restored tokens

#### Scenario: Already-plain code blocks are not modified
- **WHEN** a code block already has no child elements (no Rouge spans)
- **AND** it has a language identifier
- **THEN** the language class is still added to the `<code>` element
- **AND** the text content is not modified

### Requirement: Bare fenced blocks auto-detect language
The presentation layout SHALL NOT add a language class to `<code>` elements where Kramdown's wrapper has `language-plaintext` (indicating no language identifier was specified in the markdown). These blocks SHALL fall through to highlight.js auto-detection.

#### Scenario: No language identifier preserves auto-detection
- **WHEN** a presentation file contains a fenced code block with no language identifier (bare ```` ``` ````)
- **AND** Kramdown outputs `class="language-plaintext"` on the wrapper
- **THEN** no language class is added to the `<code>` element
- **AND** highlight.js auto-detects the language

#### Scenario: Directory listing blocks are not forced to a language
- **WHEN** a presentation contains a bare fenced block with a directory tree listing
- **THEN** the block is not forced to any specific language grammar
- **AND** auto-detection runs as it would without the bridge

### Requirement: Bridge does not affect non-presentation pages
The JS bridge code SHALL exist only in `_layouts/presentation.html`. Code blocks on blog posts, tools pages, learning pages, and other non-presentation layouts SHALL continue to use Jekyll/Rouge's default highlighting without modification.

#### Scenario: Blog post code blocks use Rouge
- **WHEN** a user views a blog post containing ````bash ` fenced code blocks
- **THEN** the code blocks render with Rouge syntax highlighting (Kami palette)
- **AND** no JS bridge processing occurs

#### Scenario: Tools page code blocks use Rouge
- **WHEN** a user views the tools page with code blocks
- **THEN** the code blocks render with Rouge syntax highlighting
- **AND** no JS bridge processing occurs
