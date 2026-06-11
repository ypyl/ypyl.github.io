## ADDED Requirements

### Requirement: Zoom ratio detection via hidden ruler
The system SHALL detect the browser zoom ratio by injecting a hidden `1rem` element, measuring its rendered `offsetWidth`, and dividing by the CSS-declared root font-size in pixels.

#### Scenario: Normal zoom (100%)
- **WHEN** the page loads at 100% browser zoom and the root font-size is 16px
- **THEN** the measured zoom ratio SHALL be 1.0

#### Scenario: Zoomed in (200%)
- **WHEN** the user sets browser zoom to 200%
- **THEN** the measured zoom ratio SHALL be 2.0 (±0.05 tolerance)

#### Scenario: Zoomed out (50%)
- **WHEN** the user sets browser zoom to 50%
- **THEN** the measured zoom ratio SHALL be 0.5 (±0.05 tolerance)

### Requirement: Inverse transform application
The system SHALL apply `transform: scale(1/zoomRatio)` to the scroll-to-top button using `transform-origin: bottom right` to maintain the button's corner anchoring.

#### Scenario: Button size at 200% zoom
- **WHEN** browser zoom is 200% and baseline CSS size is 48px
- **THEN** the button SHALL render at approximately 48px physical size (96px CSS × scale(0.5))

#### Scenario: Button size at 100% zoom
- **WHEN** browser zoom is 100%
- **THEN** the button SHALL render at exactly its CSS size (48px) with no transform applied

#### Scenario: Transform origin anchoring
- **WHEN** the inverse scale is applied
- **THEN** the button SHALL scale from its bottom-right corner, preserving its distance from the bottom and right edges

### Requirement: No visible flicker on page load
The system SHALL measure the zoom ratio and apply the inverse transform before the button becomes visible to the user.

#### Scenario: Cold page load
- **WHEN** the page loads and JavaScript executes
- **THEN** the button SHALL never appear at an uncorrected (zoomed) size

### Requirement: Re-measurement on zoom change
The system SHALL recalculate the zoom ratio and update the inverse transform when the browser zoom level changes.

#### Scenario: User zooms in
- **WHEN** the user presses Ctrl+Plus to increase zoom
- **THEN** the zoom ratio SHALL be recalculated and the button's inverse scale SHALL be updated within the same frame

#### Scenario: User zooms out
- **WHEN** the user presses Ctrl+Minus to decrease zoom
- **THEN** the zoom ratio SHALL be recalculated and the button's inverse scale SHALL be updated within the same frame

### Requirement: Graceful degradation without JavaScript
When JavaScript is disabled, the button SHALL fall back to its normal CSS sizing, scaling with browser zoom as it did before this change.

#### Scenario: JavaScript disabled
- **WHEN** JavaScript is disabled in the browser
- **THEN** the button SHALL render at its declared CSS size (48px), scaling proportionally with browser zoom
- **THEN** the button SHALL remain functional for show/hide and scroll-to-top click

### Requirement: Compatibility with existing show/hide behavior
The zoom-compensation logic SHALL coexist with the existing scroll-based opacity toggle without interfering with it.

#### Scenario: Button hidden above threshold
- **WHEN** the page is scrolled to the top (scrollY < 200px)
- **THEN** the button SHALL have `opacity: 0` and `pointer-events: none` regardless of zoom level

#### Scenario: Button visible below threshold
- **WHEN** the page is scrolled past 200px
- **THEN** the button SHALL have `opacity: 1`, `pointer-events: auto`, and the correct inverse scale applied for the current zoom level
