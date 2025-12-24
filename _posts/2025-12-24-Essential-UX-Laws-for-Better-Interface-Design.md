---
layout: post
title: Essential UX Laws for Better Interface Design
date: 2025-12-24
tags: ux design hci product-design usability
categories: design psychology
---

## Jakob’s Law

**Core premise**

Users carry expectations formed on familiar products to new ones that look similar; they expect the new site to behave like the ones they already know.

**Why it matters**

1. Most of a user’s time is spent on other sites, so they automatically apply existing mental models to your product.
2. Aligning with those models cuts cognitive load, letting users devote mental energy to their goals instead of learning the interface.
3. The law does **not** demand identical products—only that you respect established patterns unless you have a strong, test‑validated reason to deviate.

**Design guidance**

1. **Start with conventions** – Use widely‑adopted patterns for page structure, navigation, search, and common UI elements.
2. **Limit novelty** – Introduce new interactions only when they add clear value; keep the familiar version available for a transition period.
3. **Consistency** – Apply the same layout, labeling, and interaction rules across the product to reinforce users’ prior knowledge.
4. **When breaking the pattern** – Build a solid argument, prototype, and run usability tests to confirm users understand the change before full rollout.

**Outcome**

Familiar, pattern‑based design → lower mental effort → faster task completion → higher success rate and user satisfaction.

**Action step**

1. **Audit** your UI against industry‑standard patterns (navigation menus, search placement, form layouts, etc.).
2. **Identify** any novel elements; assess whether they solve a real problem.
3. **Prototype** deviations and run a quick usability test (e.g., think‑aloud or A/B) to verify comprehension.
4. **Iterate**: keep successful innovations, revert or refine those that cause confusion.

---

## Fitts’ Law

**Core premise**

Selection time = function (distance to target, size of target). Larger targets and shorter movement distances → faster, more accurate selections.

**Why it matters**

1. Directly predicts user effort for any interactive element.
2. Errors and delays increase when targets are small or spaced too closely.
3. Mobile devices amplify the effect because screen real‑estate is limited and input precision (finger) is lower than mouse.

**Design guidance**

1. **Size** – Make touch targets at least ≈ 44 × 44 dp (or larger) so users can hit them reliably.
2. **Spacing** – Keep a minimum clear distance (≈ 8 dp) between adjacent targets to avoid accidental activation.
3. **Placement** – Position frequently used targets in easily reachable zones (e.g., thumb‑reach area on phones, top‑left or top‑right corners on desktops).
4. **Consistency** – Apply the same sizing and spacing rules across the UI to build predictable motor patterns.
5. **Contextual scaling** – Enlarge targets that are farther from the typical cursor/finger start point (e.g., “Back” button at screen edge).

**Outcome**

Properly sized, spaced, and placed targets → reduced movement time → higher task‑completion speed → lower error rate → overall better usability.

**Action step**

1. **Audit** your UI for interactive elements and measure each element’s width/height across typical screens.
2. **Verify** targets meet platform guidelines (≥ 44 × 44 dp or platform‑specific minimum) and ensure inter‑element spacing ≥ 8 dp.
3. **Adjust** sizing, padding, and placement — enlarge targets, separate crowded elements, and relocate high‑frequency actions to ergonomic zones.
4. **Iterate** with quantitative metrics (time‑to‑tap, error rate, abandonment) and quick tests (think‑aloud, micro A/B) until performance goals are met.

---

## **Hick’s Law**

**Core premise**

Decision time = function (number of options, complexity of options). More choices + greater complexity → longer, harder decisions.

**Why it matters**

1. Decision time increases with the number and complexity of choices, raising the risk of decision paralysis or errors.
2. Slower decisions lengthen task‑completion time and reduce conversion and success rates.
3. Excessive options frustrate users and increase abandonment, especially among first‑time or low‑expertise users.

**Design guidance**

1. **Minimize options** – Show only the choices needed for the current goal; hide or defer secondary options.
2. **Chunk tasks** – Break complex flows into a series of simple, single‑choice steps to keep each decision trivial.
3. **Highlight recommended choices** – Use defaults, visual emphasis, or progressive disclosure to steer users toward the optimal option.
4. **Onboard gradually** – Introduce new features or settings incrementally, letting users master basics before confronting the full set of controls.
5. **Avoid over‑simplification** – Preserve essential functionality; don’t strip away options that are critical for power users or specific use cases.

**Outcome**

Fewer, clearer choices → lower cognitive load → faster, more confident decisions → higher task‑completion rates and user satisfaction.

**Action step**

1. **Audit the UI** – List every decision point and count the visible options.
2. **Trim** – Remove non‑essential choices or move them to secondary screens/menus.
3. **Sequence** – Redesign multi‑option flows into step‑by‑step wizards or progressive disclosure patterns.
4. **Add guidance** – Apply defaults, visual cues, or contextual hints to the most common/desired choice.
5. **Validate** – Run quick usability tests (time‑to‑decision, error rate, abandonment) and iterate until decision time is acceptably low.

---

## Miller’s Law

**Core premise**

The average person can keep only about 7 (± 2) items in working memory at once.

**Why it matters**

1. Cognitive capacity is limited; overload directly harms task performance.
2. Content that exceeds this capacity forces users to split attention, increasing errors and time‑to‑completion.
3. The exact limit varies with the individual’s prior knowledge and situational context, so a one‑size‑seven rule is a misconception.

**Design guidance**

1. **Don’t treat “7 ± 2” as a hard ceiling** for navigation items or UI elements.
2. **Chunk information** into small, self‑contained blocks that can be processed and remembered easily.
3. **Tailor chunk size** to the target audience’s expertise and the task’s context (e.g., expert users can handle larger groups).
4. **Use visual hierarchy and spacing** to reinforce the boundaries between chunks.
5. **When you must present many options**, employ progressive disclosure, filters, or grouping to keep the immediate view within the memory limit.

**Outcome**

Proper chunking → lower cognitive load → faster comprehension → higher success rates and satisfaction.

**Action step**

1. **Audit existing screens** for dense clusters of information; count visible items per visual group.
2. **Re‑organize** any group exceeding ~7 ± 2 into sub‑groups or collapsible sections.
3. **Validate** with quick usability tests (think‑aloud, short tasks) to confirm users can recall and act on the presented items without strain.
4. **Iterate** the chunking strategy based on test feedback; document the grouping rules for future design work.

---

## Postel’s Law

**Core premise**

Be conservative in what you do; be liberal in what you accept from others.

**Why it matters**

1. Users interact with a wide range of devices, input methods, network conditions, and assistive technologies.
2. A product that is reliable (conservative) and tolerant of varied inputs (liberal) reduces user error, frustration, and abandonment.

**Design guidance**

1. **Conservative implementation** – Build interfaces and services that are stable, simple, and work for the broadest possible audience (any screen size, bandwidth, input modality).
2. **Liberal input handling** – Accept data from any device, format, language, or interaction style; validate and normalize internally while providing clear feedback.
3. **Anticipate edge cases** – Map possible user actions, data variations, and environmental constraints; design fallback paths and graceful degradation.
4. **Feedback loops** – When rejecting or correcting input, explain why and how to fix it, preserving the user’s intent.

**Outcome**

Robust, adaptable UX → fewer errors, higher satisfaction, broader accessibility, and easier maintenance.

**Action steps**

1. **Audit** current interfaces for reliability (error rates, crashes) and input tolerance (supported devices, formats).
2. **Define** the minimal functional baseline that must work for all users (conservative core).
3. **Catalog** all possible input variations (keyboard, touch, voice, assistive tech, low‑bandwidth).
4. **Implement** normalization and validation layers that accept the widest range of inputs while preserving core functionality.
5. **Test** with diverse user groups and devices; iterate on any failure points.

---

## Peak‑End Rule

**Core premise**

Users judge an experience primarily by its most intense (peak) emotional moment and its final moment, rather than by the sum or average of all moments.

**Why it matters**

1. Memory is biased toward peaks and ends, shaping overall satisfaction.
2. Negative peaks are remembered more vividly than positive ones, affecting repeat usage and word‑of‑mouth.

**Design guidance**

1. **Focus on peak and end moments** – Identify and prioritize the most emotionally charged and the concluding points of the user journey.
2. **Highlight value‑rich moments** – Pinpoint when the product is most useful/interesting and design to delight users there.
3. **Mitigate negative peaks** – Reduce or reframe painful moments because they dominate recall.

**Outcome**

Positive peak + positive end → favorable overall memory → higher likelihood of reuse and recommendation.

**Action step**

1. **Analyze** the user flow to locate peak and end moments.
2. **Design** targeted positive experiences for those moments.

---

## The Aesthetic Effect in Usability

**Core premise**

An aesthetically pleasing design triggers a positive emotional response, causing users to *perceive* the product as more usable and to trust it more.

**Why it matters**

1. Aesthetic appeal increases perceived usability and user trust.
2. Attractive design can hide small usability issues, making them harder to detect in testing.
3. Positive affect from good design improves users’ cognitive processing and confidence in the product.

**Design guidance**

1. **Leverage aesthetics deliberately** – Use visual appeal to create a good first‑impression, but don’t rely on it to hide flaws.
2. **Validate usability separately** – Run usability tests regardless of how attractive the UI looks; look for hidden friction.
3. **Balance form and function** – Ensure that every aesthetic choice supports or at least does not impede core tasks.
4. **Iterate on feedback** – If testing reveals usability problems, improve functionality even if the design remains attractive.

**Outcome**

Attractive design → positive affect → higher perceived usability & trust → risk of unnoticed minor usability problems.

**Action steps**

1. **Audit** the visual design for aesthetic consistency and appeal.
2. **Conduct usability testing** (think‑aloud, task‑based) to surface any hidden issues.
3. **Prioritize fixes** for any usability problems uncovered, even if the UI looks good.
4. **Iterate**: refine visual elements only after functional issues are resolved.

---

## Effect of Visual Contrast

**Core premise**

The effect of visual contrast (or accent) is crucial for directing user attention in interfaces.

**Why it matters**

1. High‑contrast visual elements attract user attention quickly.
2. Proper contrast helps users focus on critical information or actions.
3. Overusing contrast creates visual competition, making it harder for users to locate what they need.

**Design guidance**

1. **Use contrast purposefully** – Reserve strong contrast for primary actions and critical info.
2. **Limit competing accents** – Keep one (or at most two) strong accents per screen.
3. **Ensure accessibility** – Meet WCAG ratios and avoid relying on color or flashing effects.

**Outcome**

Effective contrast → clearer guidance, faster information retrieval, higher task success.
Misused contrast → user confusion, increased cognitive load.

**Action steps**

1. **Audit** the interface for current contrast usage.
2. **Prioritize** contrast on essential elements; remove or tone down non‑essential accents.
3. **Test** with a diverse user group, including people with vision defects or motion sensitivity, to verify that contrast aids rather than hinders.

---

## Tesler’s Law (Law of Conservation of Complexity)

**Core premise**

Every system contains a minimum amount of intrinsic complexity that cannot be eliminated; it can only be moved from one part of the system to another (e.g., from the user to the back‑end or to the design process).

**Why it matters**

1. Designers and developers must decide where unavoidable complexity lives; leaving it on users creates confusion, frustration, and poor UX.
2. Some complexity is inherent to the domain; the goal is to manage it rather than eliminate it.
3. Moving complexity into the system helps only when the interface still communicates meaning; over‑simplification harms functionality and user understanding.

**Design guidance**

1. **Shift complexity away from users** – Automate calculations, handle edge‑cases in the back‑end, provide sensible defaults, and expose only the necessary controls.
2. **Preserve meaning** – Keep the interface expressive enough that users can still see *what* the system does and *why* it behaves a certain way.
3. **Map the “complex kernel”** – Identify the irreducible part of the process (e.g., tax rules, shipping logistics, security protocols) and decide which side will bear it.
4. **Avoid “over‑simplification”** – Do not strip away essential steps or information just to make the UI look minimal; this creates hidden complexity that users must infer.
5. **Iterate with feedback** – Test whether the relocated complexity is truly hidden from users or merely shifted to a confusing UI element.

**Outcome**

Properly managed complexity → lower cognitive load for users → clearer mental model → higher task success, satisfaction, and reduced support costs.

**Action steps**

1. **Audit** the current workflow and list all functional requirements that constitute the system’s inherent complexity.
2. **Classify** each item: *must stay in the UI* vs. *can be handled by the system*.
3. **Redesign** UI elements to hide or automate the “system‑side” complexity (e.g., auto‑fill, validation, progressive disclosure).
4. **Validate** with usability testing: check that users can complete core tasks without encountering hidden hurdles or loss of meaning.

---

## Doherty Threshold

**Core premise**

System performance—specifically response time under 400 ms—is a functional design element. When the computer and user interact faster than this threshold, neither has to wait for the other, keeping attention and productivity high.

**Why it matters**

1. Interactions ≤ 400 ms keep users focused; delays > 100 ms are perceptible, > 300 ms feel sluggish, and > 1 s cause attention drift.
2. Fast feedback produces positive emotional responses; slow responses increase frustration, cognitive load, and abandonment risk.
3. Users judge systems by perceived speed; visual cues (animations, progress bars) make unavoidable waits feel more tolerable.

**Design guidance**

1. **Target ≤ 400 ms** for all user‑initiated actions (clicks, form submissions, navigation).
2. **Provide immediate feedback** – show spinners, skeleton screens, or subtle animations the moment a request starts.
3. **Use progress indicators** – even approximate bars give users a sense of progress and reduce perceived wait time.
4. **Strategic delay** – if a process finishes too quickly to be trusted, a brief, purposeful pause can increase perceived reliability (but never exceed the 400 ms ceiling).
5. **Optimize page weight** – keep assets lean; large pages (≈2 MB desktop, ≈1.7 MB mobile) inflate load times and push response beyond the threshold.

**Outcome**

Systems that consistently respond within the Doherty Threshold deliver smoother user experiences, higher task‑completion rates, lower cognitive load, and stronger user trust and satisfaction.

**Action steps**

1. **Audit current response times** across key interactions; log any that exceed 400 ms.
2. **Identify performance bottlenecks** (large assets, blocking scripts, server latency).
3. **Implement perceived‑performance techniques**: skeleton UI, progress bars, micro‑animations.
4. **Apply purposeful micro‑delays only where trust is at stake**, ensuring the total still stays ≤ 400 ms.
5. **Test** with real users (think‑aloud, timed tasks) to verify that perceived speed meets expectations.
6. **Iterate**: shrink page weight, refine feedback mechanisms, and re‑measure until the 400 ms target is met across the board.


## References

[Laws of UX](https://www.amazon.com/Laws-UX-Psychology-Products-Services/dp/149205531X)
