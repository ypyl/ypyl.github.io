---
layout: presentation
---

# Spec-Driven Development — OpenSpec vs GitHub Spec-Kit

Two tools, two philosophies. One decision.

- **OpenSpec** (Fission-AI) — fluid, delta specs, brownfield-first
- **Spec-Kit** (GitHub) — structured, constitution-first, greenfield-optimized

> **Notes:** Both are MIT-licensed open source. OpenSpec is Node.js/npm. Spec-Kit is Python/uv. Both support 20+ AI coding agents.

---

# OpenSpec — Delta Specs, Minimal Ceremony

```bash
npm install -g @fission-ai/openspec@latest
openspec init
```

Workflow:
```
/opsx:propose add-dark-mode
/opsx:apply
/opsx:archive
```

4 artifacts per change: `proposal.md` · `specs/` · `design.md` · `tasks.md`

<br>

| The Catch |
|-----------|
| Delta model assumes AI understands **unchanged** parts of codebase |
| Poor docs → hallucinated context → spec that doesn't fit |

> **Notes:** Brownfield-first means you only describe what's changing. But on large codebases without good inline docs, the agent may invent surrounding behavior. Always review the proposal against actual code before applying.

---

# Spec-Kit — Constitution-First, Full Specs

```bash
uv tool install specify-cli --from git+https://github.com/github/spec-kit.git@v0.1.6
specify init my-app --integration copilot
```

Workflow:
```
/speckit.constitution
/speckit.specify
/speckit.plan
/speckit.tasks
/speckit.implement
```

7+ artifacts per feature: `spec.md` · `plan.md` · `tasks.md` · `research.md` · `data-model.md` · `contracts/` · `quickstart.md`

<br>

| The Gotcha |
|------------|
| Markdown review overload — auditing AI prose takes longer than plain AI coding |
| Internal status files land in project root, not meant for human review |

> **Notes:** In hands-on testing, a medium feature produced so many repetitive markdown files that reviewing them consumed more time than implementing without the framework. Spec-Kit also has no built-in help or status command.

---

# Spec Drift — The AI Doesn't Follow Its Own Plan

Both tools suffer: the spec is correct, the agent ignores it.

| Tool | Drift Example |
|------|--------------|
| Spec-Kit | Research phase documents `AuthMiddleware` as existing code → implementation regenerates it, creating duplicates |
| OpenSpec | Proposal writes "PostgreSQL chosen for JSONB support" — you never mentioned PostgreSQL |

**The fix:** treat the spec as a living artifact, not a contract.

```bash
# OpenSpec — edit any file, pick up where you left off
vim openspec/changes/add-auth/specs/auth.md
/opsx:apply    # continues from current state, no re-generation

# Spec-Kit — re-run the entire phase chain
vim specs/003-auth/spec.md
/speckit.plan  # regenerates ALL downstream artifacts
```

> **Notes:** OpenSpec's fluid model is the key advantage here. Spec-Kit's rigid phases mean one edit cascades into regenerating everything. Neither tool guarantees the agent will follow instructions — skeptical review is always required.

---

# The Upgrade Trap

| Issue | Spec-Kit | OpenSpec |
|-------|----------|----------|
| Upgrade overwrites customizations | ⚠️ Yes — documented warning | ✅ No — separate dirs |
| Template safety | Templates share space with tool internals | User content in `openspec/`, runtime separate |
| Fix | Back up `.specify/templates/` before `specify self upgrade` | `npm install -g @fission-ai/openspec@latest && openspec update` |

```bash
# Spec-Kit upgrade — DANGER: wipes custom templates
specify self upgrade   # ⚠️ "Upgrade will overwrite customizations"
```

```bash
# OpenSpec upgrade — safe, only regenerates agent instructions
npm install -g @fission-ai/openspec@latest
openspec update        # doesn't touch openspec/specs/ or openspec/changes/
```

> **Notes:** If your org has a security review checklist baked into Spec-Kit templates, one upgrade deletes it. OpenSpec's trade-off: opinionated about directory naming — can collide if your monorepo already has a `specs/` folder at root.

---

# Hands-On Scorecard — Same Feature, Same Codebase

Evaluated by Ran Isenberg, Senior Engineer at Palo Alto Networks (Feb 2026)

| Dimension | OpenSpec | Spec-Kit |
|-----------|:--------:|:--------:|
| Developer experience | 4/5 | 3/5 |
| Human review checkpoints | 5/5 | 3/5 |
| Workflow visibility | 4/5 | 2/5 |
| Parallel development | 5/5 | 5/5 |
| Mid-feature course correction | 4/5 | 2/5 |
| Planning time | 3 hrs | 4 hrs |
| Implementation time | 1 day | 1 day |
| **Overall score** | **4.00** | **2.77** |

> **Notes:** Spec-Kit has no help command — you don't know where you are. OpenSpec always suggests next step via `/opsx:continue`. Both shipped a working PR in similar total time. The gap is in the experience — not the output.

---

# Which One Fits Your Workflow?

| Choose OpenSpec when | Choose Spec-Kit when |
|----------------------|----------------------|
| Modifying an existing codebase | Starting greenfield from scratch |
| You want fluid, out-of-order editing | Your org needs enforced architectural rules |
| Parallel features on the same codebase | You need a project constitution |
| Minimal ceremony, fast iteration | Deep customization via extensions & presets |
| You already have OpenSpec skills (pi, Cursor) | You're in GitHub Copilot ecosystem |

<br>

| **The universal rule** |
|-------------------------|
| Specs are **conversation starters**, not signed contracts |
| Review every generated artifact skeptically |
| Don't trust the agent's confidence — verify against code |

> **Notes:** These tools are fast-moving. Features land, break, and get patched between evaluation cycles. Abstract your tool choice with custom skills so you can swap engines without breaking your team's workflow.

---

# References

- [Fission-AI/OpenSpec](https://github.com/Fission-AI/OpenSpec)
- [github/spec-kit](https://github.com/github/spec-kit)
- [Ran Isenberg — Hands-on comparison](https://ranthebuilder.cloud/blog/i-tested-three-spec-driven-ai-tools-here-s-my-honest-take/)
- [Martin Fowler — Understanding SDD Tools (Kiro, Spec-Kit, Tessl)](https://martinfowler.com/articles/exploring-gen-ai/sdd-3-tools.html)

> **Notes:** Slide deck created June 2026. Versions tested: OpenSpec v1.2.0, Spec-Kit v0.1.6. Tools evolve rapidly — verify current state before deciding.
