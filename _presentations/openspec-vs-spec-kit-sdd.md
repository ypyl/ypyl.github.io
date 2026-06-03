---
layout: presentation
---

# OpenSpec vs GitHub Spec-Kit

Two tools. Two philosophies.

- **OpenSpec** — fluid, delta specs, brownfield
- **Spec-Kit** — structured, full specs, greenfield

> **Notes:** Both MIT-licensed. OpenSpec is Node.js, Spec-Kit is Python. Both support 20+ AI coding agents.

---

# At a Glance

| | OpenSpec | Spec-Kit |
|---|---|---|
| **Spec model** | Delta only | Full description |
| **Phase gates** | None | Sequential |
| **Files per change** | ~4 | 7+ |
| **Brownfield** | First-class | Afterthought |

> **Notes:** Delta specs = you only describe what's changing. Spec-Kit's full specs mean more markdown to review — on a medium feature, review took longer than implementing without the framework.

---

# Hands-On Scorecard

Same feature, same codebase (Isenberg, Feb 2026)

| Dimension | OpenSpec | Spec-Kit |
|-----------|:--------:|:--------:|
| Spec quality | 4/5 | 2/5 |
| Dev experience | 4/5 | 3/5 |
| Mid-feature correction | 4/5 | 2/5 |
| **Overall** | **4.0** | **2.8** |

> **Notes:** Both shipped working PRs in ~1 day. The gap is experience, not output. Spec-Kit has no help command — you don't know where you are in the workflow.

---

# Spec Drift

Agent ignores its own spec.

| | Issue |
|---|---|
| Spec-Kit | Documents existing code → regenerates duplicates |
| OpenSpec | Invents rationale you never gave |

- **OpenSpec**: edit, re-apply — no re-generation
- **Spec-Kit**: edit, re-run phase — everything downstream resets

> **Notes:** OpenSpec's fluid model is the key advantage. Spec-Kit's rigidity makes course correction expensive.

---

# The Upgrade Trap

**Spec-Kit**: `specify self upgrade` wipes custom templates.

**OpenSpec**: `openspec update` never touches your specs.

> **Notes:** Spec-Kit explicitly warns "Upgrade will overwrite customizations." If your org customized templates, they're gone on upgrade. OpenSpec keeps user content and runtime separate.

---

# Community Health

| | OpenSpec | Spec-Kit |
|---|---|---|
| Stars | Growing | 108k |
| Commits (90d) | 158 | 37 |
| PR median age | 30 days | 62 days |
| Bus factor | 1 | 2 |

> **Notes:** Spec-Kit has the brand. OpenSpec has more active development. Both have thin bus factors — you're betting on small teams.

---

# Pick Your Tool

| OpenSpec | Spec-Kit |
|----------|----------|
| Existing codebases | New projects |
| Fluid editing | Enforced rules |
| Minimal ceremony | Deep customization |

Specs are starting points, not contracts.

> **Notes:** Abstract your choice behind custom skills so you can swap tools later without retraining your team.

---

# References

- [Fission-AI/OpenSpec](https://github.com/Fission-AI/OpenSpec)
- [github/spec-kit](https://github.com/github/spec-kit)
- [Isenberg comparison](https://ranthebuilder.cloud/blog/i-tested-three-spec-driven-ai-tools-here-s-my-honest-take/)
- [Martin Fowler on SDD](https://martinfowler.com/articles/exploring-gen-ai/sdd-3-tools.html)
