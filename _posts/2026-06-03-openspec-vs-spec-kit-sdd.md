---
layout: post
title: "OpenSpec vs GitHub Spec-Kit for SDD — Delta Specs, Hidden Drift, and the Upgrade Trap"
date: 2026-06-03
categories: programming
tags: spec-driven-development, openspec, spec-kit, ai-coding, developer-tools
---

**Spec-driven development (SDD)** flips the traditional coding script: instead of writing code and letting specs rot, you write a **specification first** and let AI generate the implementation from it. Two open-source tools dominate the space — **OpenSpec** by Fission-AI and **GitHub Spec-Kit** by Microsoft/GitHub. Both promise predictable AI-generated code, but they take radically different approaches. Here's what you actually need to know before picking one.

## The Two Philosophies in One Command

OpenSpec runs on Node.js and embraces **delta specs** — you only describe what's *changing*, not the entire system. Initialize it in seconds:

```bash
npm install -g @fission-ai/openspec@latest
cd your-project
openspec init
```

Then inside your AI coding agent, you run `/opsx:propose add-dark-mode` and it generates a single change folder with four compact artifacts: `proposal.md`, `specs/`, `design.md`, and `tasks.md`. The catch: this delta model assumes the AI already understands your codebase well enough to infer the unchanged parts. On a large brownfield project with poor inline documentation, the agent may hallucinate the surrounding context and produce a spec that doesn't fit.

Spec-Kit uses Python's `uv` toolchain and follows a **constitution-first** model — you define immutable project principles before any feature work begins:

```bash
uv tool install specify-cli --from git+https://github.com/github/spec-kit.git@v0.1.6
specify init my-app --integration copilot
```

Then you invoke `/speckit.constitution`, `/speckit.specify`, `/speckit.plan`, and `/speckit.tasks` in strict sequence. Each phase generates a stack of markdown files — `spec.md`, `plan.md`, `research.md`, `data-model.md`, `contracts/`, `quickstart.md`, `tasks.md`, plus internal status files that land in your project root. The gotcha: these artifacts are highly repetitive. In hands-on testing, a medium-sized backend feature produced so much markdown that reviewing it took longer than plain AI-assisted coding without any spec framework. You end up auditing verbose AI-generated prose instead of reading code.

## Spec Drift: When the Plan Says One Thing and the Agent Does Another

Both tools suffer from a deeper problem: the AI doesn't always follow the spec it just helped write. While evaluating a feature that added an authorization layer to an existing service, Spec-Kit's research phase correctly identified the existing `AuthMiddleware` class and documented it — then the implementation agent ignored the fact that it was describing *preexisting code* and regenerated a duplicate from scratch, producing class collisions. The spec was correct; the agent just didn't respect it.

OpenSpec avoids this with its `openspec status` dashboard and the `/opsx:continue` command, which displays the artifact dependency graph and tells you exactly where you are. But it introduces its own trust issue: it sometimes adds rationale to decisions you never made. During proposal generation, it might write "we chose PostgreSQL for its JSONB support" when you never mentioned PostgreSQL. The fix in both cases is the same — treat the spec as a *living artifact*, not a contract. With OpenSpec, edit any markdown file directly and re-run `/opsx:apply`; it picks up from where you left off without regenerating everything. With Spec-Kit, you have to re-run the entire chain from the phase where you want changes, and all downstream artifacts get replaced wholesale.

## The Upgrade Trap and Customization Risk

Spec-Kit has a documented upgrade hazard: running `specify self upgrade` can overwrite your customized templates and scripts because user modifications and tool internals share the same file space. From the official docs: *"⚠️ Important Warning: Upgrade will overwrite customizations."* If you've tweaked the plan template to include your organization's security review checklist, one upgrade deletes it.

OpenSpec handles this better by separating user content (`openspec/specs/`, `openspec/changes/`) from tool runtime files. A `npm install -g @fission-ai/openspec@latest` followed by `openspec update` regenerates agent instructions without touching your specs. The trade-off: OpenSpec is more opinionated about where files live. If your monorepo already has a `specs/` directory at the repo root, you'll need to work around the naming collision.

## Which One Fits Your Workflow

If you're maintaining an existing codebase and want minimal ceremony — delta specs, parallel feature folders, and the ability to edit artifacts out of order — go with OpenSpec. Its fluid philosophy means you're never blocked waiting for a phase to complete. If you're starting a greenfield project and your team needs enforced architectural principles (a "constitution") that survive across all features, Spec-Kit's structured pipeline is worth the verbosity. But regardless of which you pick, the rule is the same: the spec is a conversation starter, not a document you sign and file away. Review every generated artifact skeptically, and don't let the agent's confidence trick you into shipping a plan it won't actually follow.

[OpenSpec on GitHub](https://github.com/Fission-AI/OpenSpec) · [GitHub Spec-Kit repo](https://github.com/github/spec-kit) · [Ran Isenberg's hands-on comparison](https://ranthebuilder.cloud/blog/i-tested-three-spec-driven-ai-tools-here-s-my-honest-take/) · [Martin Fowler: Understanding SDD Tools](https://martinfowler.com/articles/exploring-gen-ai/sdd-3-tools.html)
