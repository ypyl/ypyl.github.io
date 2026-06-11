## 1. CSS: Resolve hover transform conflict with wrapper element

- [x] 1.1 Add `.affix-wrapper` CSS class with `position: fixed`, `bottom`, `right`, `z-index`, and `transform-origin: bottom right`
- [x] 1.2 Move `position: fixed`, `bottom`, `right`, `z-index` from `.affix-button` to `.affix-wrapper` (and update mobile media query accordingly)
- [x] 1.3 Wrap the `<button>` in `<span class="affix-wrapper">` in `_includes/affix.html`

## 2. JS: Zoom detection via hidden ruler

- [x] 2.1 Add `getZoomRatio()` function that injects a hidden `1rem`-wide `<div>` into `document.body`, measures `offsetWidth`, and compares to the root font-size in px (parsed from `getComputedStyle`)
- [x] 2.2 Add `try/catch` fallback returning `1` if measurement fails
- [x] 2.3 Remove the ruler `<div>` from the DOM after measurement to keep the DOM clean

## 3. JS: Apply inverse transform

- [x] 3.1 Add `updateZoomCompensation()` function that calls `getZoomRatio()` and applies `transform: scale(1/zoomRatio)` to the `.affix-wrapper` element
- [x] 3.2 Skip applying transform when zoom ratio is `1` (no transform means better subpixel rendering at 100%)

## 4. JS: Flicker prevention on page load

- [x] 4.1 Set `.affix-wrapper` initial style to `visibility: hidden` inline in the HTML
- [x] 4.2 Call `updateZoomCompensation()` synchronously before the existing `onScroll()` initial check
- [x] 4.3 After measurement and transform, set `.affix-wrapper` `visibility` to `visible` (the button's own opacity still controls actual show/hide)

## 5. JS: Re-measure on zoom change

- [x] 5.1 Add `window.addEventListener('resize', updateZoomCompensation, { passive: true })` 
- [x] 5.2 Verify the resize listener does not conflict with the existing scroll listener

## 6. Cross-browser smoke test

- [x] 6.1 Test at 100%, 150%, 200% zoom in Chrome — button SHALL maintain ~48px visual size
- [x] 6.2 Test at 100%, 150%, 200% zoom in Firefox — button SHALL maintain ~48px visual size
- [x] 6.3 Test at 100%, 150%, 200% zoom in Edge — button SHALL maintain ~48px visual size
- [x] 6.4 Verify hover lift (`translateY(-2px)`) still works after the wrapper change
- [x] 6.5 Verify scroll-to-top click still works
- [x] 6.6 Verify show/hide at scroll threshold (200px) still works at different zoom levels
