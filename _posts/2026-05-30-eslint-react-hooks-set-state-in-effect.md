---
layout: post
title: "Why ESLint Says Don't Call setState in useEffect — and What to Do Instead"
date: 2026-05-30
tags: react, eslint, hooks, typescript, frontend
categories: programming
---

You're building an accordion component. When the user opens a panel, you want to show a loading spinner for 300ms before revealing heavy content (say, a 1000-row table). The spinner uses CSS animation on the compositor thread, so it keeps spinning smoothly even while React blocks the main thread rendering all those rows.

You write this:

```tsx
const [ready, setReady] = useState(false);

useEffect(() => {
  if (isActive) {
    setReady(false);                                 // reset the spinner
    const id = setTimeout(() => setReady(true), 300); // reveal after 300ms
    return () => clearTimeout(id);
  }
}, [isActive]);
```

ESLint immediately flags line 3:

```
Error: Calling setState synchronously within an effect can trigger
cascading renders. (react-hooks/set-state-in-effect)
```

You're annoyed. The code *works*. The spinner shows, the table renders, nobody sees a flash of unstyled content. What's the problem? And how do you fix it without setting off a different ESLint rule?

---

## The Rule: `react-hooks/set-state-in-effect`

This rule says: **don't call `setState` directly in the synchronous body of a `useEffect`.**

"Body" means the code that runs immediately when the effect fires — not inside callbacks passed to `setTimeout`, event listeners, or promise chains.

```tsx
useEffect(() => {
  setState(x);           // ← VIOLATION: synchronous, in the effect body

  setTimeout(() => {
    setState(y);         // ← OK: inside an async callback — the effect isn't
  }, 300);               //        directly orchestrating this state change
}, [dep]);
```

---

## Why Cascading Renders Matter

React's data flow has a clear sequence:

```
User interaction → Render → Commit to DOM (paint) → Effects run
                                                   ↑
                                             Syncing with external
                                             systems belongs here
```

When you call `setState` synchronously in an effect, you create an extra cycle:

```
    ┌──────────────────────────────────────────┐
    │                                          │
    ▼                                          │
Render #1: spinner + placeholder               │
    │                                          │
    ▼                                          │
Commit + Paint: user sees spinner              │
    │                                          │
    ▼                                          │
Effect fires: setReady(false) ─────────────────┘
    │                            
    ▼                            
Render #2: this render exists                
    │            ONLY because the effect       
    ▼            called setState. The user     
Commit + Paint  didn't do anything new.        
                 This is a cascading render.
```

The problem isn't just one extra render. It's that:

- **The render is unnecessary.** React could have produced the correct output in Render #1 if the state had been right to begin with.
- **It can cause visible flicker.** Between Render #1 and Render #2, the user might briefly see an intermediate state.
- **It violates the intended use of effects.** Effects are for synchronizing with *external* systems (browser APIs, network, timers). Orchestrating React's own state transitions inside effects muddies the separation between "what React controls" and "what the outside world controls."

The React docs are explicit about this: [You Might Not Need an Effect](https://react.dev/learn/you-might-not-need-an-effect).

---

## Why the Obvious Fixes Don't Work

### Attempt 1: Move the reset to the `else` branch

```tsx
useEffect(() => {
  if (isActive) {
    const id = setTimeout(() => setReady(true), 300);
    return () => clearTimeout(id);
  } else {
    setReady(false); // ← still synchronous, still flagged
  }
}, [isActive]);
```

The rule doesn't care which branch you're in. Any `setState` in the synchronous path of the effect callback gets flagged. And there's a timing problem: when the panel reopens, `ready` might still be `true` from the previous open — the `else` branch runs when the panel *closes*, not when it *opens*, so there's a one-frame window where stale content flashes before the timer resets things.

### Attempt 2: Use a ref to detect transitions during render

```tsx
const prevActive = useRef(isActive);

if (!isActive && prevActive.current) { // ← react-hooks/refs violation
  setReady(false);
}
prevActive.current = isActive;
```

Now a **second** ESLint rule fires: `react-hooks/refs`. It forbids reading `ref.current` during render. Refs are escape hatches — mutable values that exist outside React's data flow. Reading them during render makes your component's output depend on hidden state that React can't track, which breaks the purity of the render function.

Two rules, two blocked escape hatches. Now what?

---

## The Mental Model Shift

The insight: **you don't need to *set* `ready` to `false`. You need `ready` to *start* as `false` when the panel opens.** The distinction matters.

React already gives you a mechanism for this: **component mount**. When a component mounts, `useState(false)` gives you fresh state. No `setState` call needed. The trick is ensuring the component actually remounts when the panel opens.

Enter the **`key` prop**. In React, a changing `key` tells the reconciler: "this is a different component instance — destroy the old one and create a new one." Mount, unmount, remount. Fresh state, every time.

```
Key on isActive changes
        │
        ▼
React unmounts old <InnerComponent>
   ├── destroys DOM nodes
   ├── discards state (ready goes away)
   └── runs cleanup (clears timeout)
        │
        ▼
React mounts new <InnerComponent>
   ├── useState(false) → ready = false  ← fresh, free, no setState
   ├── useEffect fires
   │      └── setTimeout(() => setReady(true), 300)  ← async callback, OK
   └── commit + paint: LoadingOverlay visible
```

---

## The Solution: Split the Component Around the Key Boundary

Extract the stateful logic into an inner component keyed on `isActive`. The outer component handles hooks and pass-through; the inner component owns `ready` and gets a fresh mount every time the panel toggles.

```tsx
// Outer component — hooks live here, no resettable state
export function DocumentSection({ label, children }: DocumentSectionProps) {
  const { execute } = useActions();
  const { state } = useStateStore();
  const isEditing = (state.editingSection as string | undefined) === label;
  const activePanel = useActivePanel();
  const isActive = activePanel === label;

  // key={String(isActive)} forces remount when panel opens/closes
  return (
    <DocumentSectionContent
      key={String(isActive)}
      label={label}
      isActive={isActive}
      isEditing={isEditing}
      execute={execute}
    >
      {children}
    </DocumentSectionContent>
  );
}

// Inner component — owns the resettable ready state
function DocumentSectionContent({
  label, isActive, isEditing, execute, children
}: InnerProps) {
  const [ready, setReady] = useState(false); // fresh on every mount

  // Only async setState here — no synchronous calls in the effect body
  useEffect(() => {
    if (isActive) {
      const id = setTimeout(() => setReady(true), 300);
      return () => clearTimeout(id);
    }
  }, [isActive]);

  return (
    <Accordion.Item value={label}>
      {/* ... panel content, gated by ready ... */}
    </Accordion.Item>
  );
}
```

**What changed:**

| Before | After |
|---|---|
| One component, `ready` in the same scope as everything else | Two components — outer handles hooks, inner owns `ready` |
| `setReady(false)` in effect body → ESLint violation | No synchronous `setState` anywhere |
| `ready` reset required manual state management | `ready` resets automatically via remount |
| Effect had three responsibilities | Effect has one: start a timer |

**What didn't change:**

- The spinner still appears for 300ms on every panel open
- The compositor-thread animation keeps spinning during heavy renders
- No flash of content — `ready` starts `false` on mount, so the first paint always shows the overlay

---

## When Should You Reach for This Pattern?

The `key`-based remount isn't always the right answer. It's a trade-off:

| ✅ Good for | ❌ Not good for |
|---|---|
| State that should reset completely on a prop/context change | Components with expensive mount logic (data fetching, subscriptions) |
| Transient UI state (loading states, animation triggers, form drafts that should clear on navigation) | State you want to preserve across toggle (scroll position, user input) |
| Cases where the alternative is fighting ESLint rules or writing fragile ref-based logic | Components where a simple `useEffect` cleanup function is sufficient |

In this specific case — a loading overlay that must show every time a panel opens, with a timer-driven transition — the remount pattern is a natural fit. The component's identity is tied to its active state: a closed panel and an open panel are fundamentally different things.

---

## The Bigger Lesson

ESLint rules aren't arbitrary style preferences. `react-hooks/set-state-in-effect` exists because calling `setState` in an effect body is usually a symptom of a deeper structural problem — state that should be derived, reset during render, or tied to a component's lifecycle rather than manually managed in an effect.

If you find yourself suppressing this rule, ask: *why does this state need to change here, and is there a way React's existing mechanisms (mount, unmount, `key`, derived state) can handle it instead?* The answer is often yes.

---

## Links

- [React docs: You Might Not Need an Effect](https://react.dev/learn/you-might-not-need-an-effect)
- [React docs: useRef](https://react.dev/reference/react/useRef)
- [ESLint plugin: react-hooks rules](https://www.npmjs.com/package/eslint-plugin-react-hooks)
- [React docs: Understanding `key`](https://react.dev/learn/rendering-lists#keeping-list-items-in-order-with-key)
- [React docs: Resetting state with a key](https://react.dev/learn/preserving-and-resetting-state#option-2-resetting-state-with-a-key)
