---
layout: post
title: "Elm Patterns Reference"
date: 2026-07-01
tags: [elm, patterns, functional-programming, reference]
categories: programming
---

A condensed reference of common Elm patterns. Each entry gives you the core idea and code — enough for a coding agent or experienced Elm developer to apply the pattern without reading a full article. Grouped by concept rather than difficulty.

## Type Safety

Patterns that use Elm's type system to catch bugs at compile time.

### Type blindness

Wrap primitive values in custom types to prevent mixing them up (e.g. dollars and euros).

```elm
-- ❌ Both are Float, nothing stops dollars + euros
priceInDollars : Float
priceInEuros : Float

-- ✓ Compiler distinguishes them
type Dollar = Dollar Float
type Euro = Euro Float
```

### Minimize boolean usage

Replace booleans with custom types to make intent explicit and let the compiler enforce correctness.

```elm
-- ❌ What does True mean here?
bookFlight "ELM" True

-- ✓ Intent is self-documenting
type CustomerStatus = Premium | Regular | Economy
bookFlight "ELM" Premium
```

Return custom types instead of `Bool` to eliminate boolean blindness:

```elm
-- ❌ isValid returns Bool, nothing stops using invalid data
if isValid formData then submitForm formData else ...

-- ✓ Compiler enforces valid data at each step
case validate formData of
    Ok valid -> submitForm valid
    Err errors -> showErrors errors
```

### Named arguments

Use a record argument when function parameter order is ambiguous.

```elm
-- ❌ Which date is the subject?
isBefore : Date -> Date -> Bool

-- ✓ Self-documenting
isBefore : { subject : Date, comparedTo : Date } -> Bool
```

### Wrap early, unwrap late

Wrap primitive values in custom types at the boundary (decoding) and unwrap as late as possible.

```elm
-- ❌ Any Float can be passed
displayPrice : Float -> String

-- ✓ Only Dollars accepted
type Dollar = Dollar Float
displayPrice : Dollar -> String
displayPrice (Dollar price) = "USD$" ++ String.fromFloat price
```

### Make impossible states impossible

Replace boolean + maybe combos with custom types that only represent valid states.

```elm
-- ❌ isLoading=False + data=Nothing is a nonsense state
type alias Model = { isLoading : Bool, data : Maybe Data }

-- ✓ Only valid states can be represented
type RemoteData = Loading | Loaded Data
```

### Parse, don't validate

Return a validated type from your parser instead of a boolean check — the type system then guarantees correctness downstream.

```elm
-- ❌ After validation you still have the raw input type
validate : UserInput -> Bool

-- ✓ Parser returns a known-valid type
parse : UserInput -> Result String ValidUser
```

## Data Flow

Patterns for moving and transforming data through a pipeline.

### Unwrap Maybe and Result early

Pattern-match at the highest level so child views work with clean, unwrapped data.

```elm
-- ❌ Every child view must handle Maybe User
userInfo : Maybe User -> Html Msg
userActivity : Maybe User -> Html Msg

-- ✓ Unwrap once, pass concrete User down
case maybeUser of
    Just user -> div [] [ userInfo user, userActivity user ]
    Nothing -> div [] [ text "No user" ]
```

### The railway pattern

Chain operations that can fail using `Result.andThen`. On first failure the chain short-circuits to the error track.

```elm
process : String -> Result String TransformedData
process data =
    parseData data
        |> Result.andThen validateData
        |> Result.andThen transformData
```

For early-exit (not error) scenarios, define a custom `andThen` on your own two-track type.

### Pipeline builder

Build a function by piping through validators, using Elm's type-alias-as-constructor trick.

```elm
validateUser : User -> Result String User
validateUser user =
    Ok User
        |> validateName user.name
        |> validateAge user.age

validateName : String -> Result String (String -> a) -> Result String a
validateName name =
    Result.andThen
        (\constructor ->
            if String.isEmpty name then Err "Invalid name"
            else Ok (constructor name)
        )
```

**Caveat**: fields of the same type can be swapped silently. Keep field order in the pipeline matching the type alias definition.

## Composition & Configuration

Patterns for assembling values, views, and behaviour from smaller parts.

### The builder pattern

Expose a constructor with defaults plus `with*` modifiers so callers don't break when new fields are added.

```elm
-- ❌ Every caller must supply every field, breaks on new fields
Button.btn { isEnabled = True, label = "Click", hexColor = "#ABC" }

-- ✓ Callers only set what they need to change
Button.newArgs "Click"
    |> Button.withHexColor "#123"
    |> Button.btn
```

### Arguments list

Pass a list of configuration values (all the same opaque type) to a function. Used by `elm/html`, `elm/svg`, `elm-css`, `elm-ui`.

```elm
-- Functions return a common opaque type
scale : Float -> Float -> Float -> Attribute
rotation : Int -> Int -> Int -> Attribute

-- Caller builds a list
shape
    [ scale 0.5 0.5 0.5
    , position 0 -6 -13
    , rotation -90 0 0
    ]
```

Best when all arguments are optional and the returned type is opaque.

### Combinators

Combine values of the same type to produce a value of the same type — enabling endless composition.

```elm
and : Filter -> Filter -> Filter  -- (a AND (b OR c))
```

Good candidates: `Html`, `Cmd.batch`, parsers, JSON decoders/encoders, filters, validations — anything tree-shaped.

### Conditional rendering

Three ways to conditionally show/hide an element in `view`:

```elm
-- 1. Maybe + filterMap
[ Just header, maybeBanner showBanner, Just footer ]
    |> List.filterMap identity

-- 2. No-op (text "")
maybeBanner showBanner = if showBanner then bannerElement else text ""

-- 3. List concatenation
[ header ] ++ maybeBanner showBanner ++ [ footer ]
maybeBanner showBanner = if showBanner then [ bannerElement ] else []
```

## Module Design

Patterns for encapsulation, invariants, and maintaining consistency.

### Opaque types

Expose the type but not its constructor from a module. Callers must use provided functions to create and modify values.

```elm
module Lib exposing (Config, newConfig, withSize)

type Config = Config { size : Int, style : Style }

newConfig : Config
withSize : Int -> Config -> Config
```

Useful for hiding implementation details and preventing breaking changes in packages.

### Opaque types for enforcing invariants

Use opaque types to guarantee data always satisfies a rule (e.g. a list that is always sorted).

```elm
module SortedList exposing (SortedList, new, add)

type SortedList comparable = SortedList (List comparable)

new : SortedList comparable
new = SortedList []

add : comparable -> SortedList comparable -> SortedList comparable
```

Only this module can add items, so the sort invariant can't be broken from outside.

### Type iterator

Keep a list of all custom-type variants in sync with the definition. Use an `elm-review` rule or a recursive `case` that the compiler checks for exhaustiveness.

```elm
-- ❌ Adding Blue won't warn that `all` is missing it
type Color = Red | Yellow | Green | Blue
all : List Color
all = [ Red, Yellow, Green ]

-- ✓ Compiler warns until Blue is handled (no wildcard!)
next : List Color -> List Color
next list =
    case List.head list of
        Nothing -> Red :: list |> next
        Just Red -> Yellow :: list |> next
        Just Yellow -> Green :: list |> next
        Just Green -> Blue :: list |> next
        Just Blue -> list
```

Never use `_` wildcard matching in the `next` function — it defeats the compiler check.

## Advanced Types

Patterns using phantom types and type-level state machines.

### Phantom types

Add an unused type variable to restrict what functions accept and return.

```elm
type Users a = Users (List User)

activeUsers : List User -> Users Active
activeUsers users = users |> List.filter .isActive |> Users

-- This view only accepts active users
activeUsersView : Users Active -> Html msg
```

Useful for state machines, process flows, and validation gating.

### Process flow using phantom types

Enforce a state machine at the type level so you can't skip or reorder steps.

```elm
type Step step = Step Order

type Start = Start
type OrderWithTotal = OrderWithTotal
type Done = Done

-- Transitions are type-restricted
setTotal : Int -> Step Start -> Step OrderWithTotal
adjustQuantityFromTotal : Step OrderWithTotal -> Step Done

-- Only valid flows compile
flow total order =
    start order
        |> setTotal total
        |> adjustQuantityFromTotal
        |> done
```

## Architecture

Patterns for scaling Elm applications with The Elm Architecture (TEA).

### Reusable views

Build a view function that takes message constructors and state as arguments.

```elm
type alias Args msg =
    { currentDate : Date
    , isOpen : Bool
    , onOpen : msg
    , onClose : msg
    , onSelectDate : Date -> msg
    }

calendar : Args msg -> Html msg
```

Use the builder pattern for complex reusable views with many optional settings.

### Nested TEA

Break a large app into child modules, each with its own `Model`, `Msg`, `update`, `view`. The parent wraps the child's `Msg` and uses `Html.map`.

```elm
-- Parent
type Msg = Increment | Sub Sub.Msg

update msg model =
    case msg of
        Sub subMsg ->
            { model | subModel = Sub.update subMsg model.subModel }
        ...

view model =
    div []
        [ ...
        , Sub.view model.subModel |> Html.map Sub
        ]
```

Use sparingly — adds boilerplate and makes child-to-parent communication harder.

### Child outcome

When using Nested TEA, return a third value from the child's `update` to communicate results to the parent.

```elm
type Outcome = OutcomeNone | OutcomeDateUpdated Date

update : Msg -> Model -> ( Model, Cmd Msg, Outcome )
```

The parent inspects the outcome and reacts.

### Translator

Make child views generic so they can produce messages for both themselves and their parent — no more `Html.map` limitation.

```elm
-- Child: view takes a toSelf constructor and any parent messages
type alias Args msg =
    { toSelf : Msg -> msg
    , onSave : msg
    }

view : Model -> Args msg -> Html msg

-- Parent wires it up
Child.view model.childModel
    { toSelf = ChildMsg
    , onSave = OnSave
    }
```

### Global actions

When deeply nested modules need to communicate with the root (e.g. show notification, sign out), return a list of actions alongside the model and commands.

```elm
type Action = OpenSuccessNotification String | SignOut | ...

update : Msg -> Model -> ( Model, Cmd Msg, List Action )
```

The root `update` processes the action list. Mimic `Cmd.batch` for combining actions, and add `Actions.map` for actions that carry a child message.

### The effects pattern

Replace `Cmd Msg` in `update` with a custom `Effect` type for testability.

```elm
type Effect = SaveUser User | LogoutUser | LoadData | ...

update : Msg -> Model -> ( Model, List Effect )

-- Convert to real Cmd only at the app boundary
runEffects : List Effect -> Cmd Msg
```

This lets you inspect and assert on effects in tests. Used by `elm-program-test`.

### Update return pipeline

Break complex `update` branches into small single-concern functions piped with `andThen`.

```elm
SeeReport report ->
    ( model, Cmd.none )
        |> andThen (setStageToReportVisible report)
        |> andThen loadMoreDataIfNeeded
        |> andThen addKeyInUrl
        |> andThen trackSeeReportEvent

andThen : (model -> (model, Cmd msg)) -> (model, Cmd msg) -> (model, Cmd msg)
andThen fn ( model, cmd ) =
    let ( nextModel, nextCmd ) = fn model
    in ( nextModel, Cmd.batch [ cmd, nextCmd ] )
```

Each pipeline function handles one concern, making update branches readable and testable.

---

*Patterns compiled from the Elm community. Originally inspired by [Elm Patterns](https://sporto.github.io/elm-patterns/) by Sebastian Porto.*
