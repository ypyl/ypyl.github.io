## 1. CSS — Carousel Layout and Transitions

- [x] 1.1 Add `.news-carousel` container styles to `assets/css/style.css`
- [x] 1.2 Add `.news-carousel-views` CSS grid stacking (all views in single grid cell: `grid-area: 1 / 1`)
- [x] 1.3 Add `.news-view` styles with opacity transition (300ms ease), `pointer-events: none` when inactive
- [x] 1.4 Add `.news-view.active` styles: `opacity: 1`, `pointer-events: auto`

## 2. CSS — Dots Styling

- [x] 2.1 Add `.news-carousel-dots` container styles (centered row, spacing via Kami tokens)
- [x] 2.2 Add `.news-dot` button styles: 8px circle, `var(--border-color)` fill, no border, cursor pointer
- [x] 2.3 Add `.news-dot.active` styles: `var(--brand)` fill
- [x] 2.4 Add `.news-dot:hover` state for inactive dots (slightly darker border-color)

## 3. CSS — Mobile Adaptation

- [x] 3.1 Modify existing `@media (max-width: 600px)` rule for `.news-grid` to single column
- [x] 3.2 Add CSS rule to hide cards 4–6 within each `.news-view` on mobile (`:nth-child(n+4) { display: none }`)
- [x] 3.3 Ensure dot size and spacing remain unchanged on mobile

## 4. Liquid Template — Home Page

- [x] 4.1 Replace the static `{%- for item in news_items limit: 6 -%}` loop with dynamic carousel markup
- [x] 4.2 Compute view count: `ceil(news_items.size / 6.0)`, capped at 5
- [x] 4.3 Render views: outer loop for views, inner loop for 6 cards each using `offset` parameter
- [x] 4.4 Render dots: loop matching view count, first dot has `.active` class
- [x] 4.5 Conditionally hide dots when only 1 view (6 or fewer items)
- [x] 4.6 Include inline JavaScript for dot click handling (~10 lines)

## 5. JavaScript — Dot Navigation

- [x] 5.1 Add inline `<script>` at end of home.html carousel markup
- [x] 5.2 Implement click handler: remove `.active` from all dots and views, add to clicked pair
- [x] 5.3 Hide all dots on load if JS is unavailable (CSS default, JS shows them)

## 6. Verification

- [x] 6.1 Run `jekyll build` and verify no Liquid errors
- [x] 6.2 Verify 30 news cards render across 5 views with correct content
- [x] 6.3 Test dot navigation — confirm crossfade works and correct cards display
- [x] 6.4 Test with fewer than 30 items — confirm dynamic dot count
- [x] 6.5 Test with 6 or fewer items — confirm static grid fallback, no dots
- [x] 6.6 Test on mobile viewport — confirm 3 cards per view, single column
- [x] 6.7 Test with JavaScript disabled — confirm all cards visible, no dots shown
- [x] 6.8 Test dark mode — confirm dots follow theme variables
