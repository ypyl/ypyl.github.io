## ADDED Requirements

### Requirement: Dot buttons show pointer cursor on hover
The news carousel navigation dot buttons SHALL display a pointer (hand) cursor when the user hovers over them, providing a standard visual affordance that the element is clickable.

#### Scenario: User hovers over a navigation dot
- **WHEN** the user moves the mouse cursor over any `.news-dot` button
- **THEN** the cursor changes to `pointer` (hand icon)

#### Scenario: User hovers over dot container gap area
- **WHEN** the user moves the mouse cursor over the spacing/gap area between dots within `.news-carousel-dots`
- **THEN** the cursor remains `default` (arrow), not pointer
