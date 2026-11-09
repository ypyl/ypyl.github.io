# Working notes: ADR + OpenSpec + building software with code agents

**Status:** research notes, not a post. Do not publish.
**Created:** 2026-11-08
**Target:** a work note in `_posts/` describing the approach as practised, not OpenSpec as a tool.
**Related draft:** `_drafts/2026-06-05-adrs-meet-ai-driven-development-openspec-artifacts.md`

---

## 1. Purpose of this file

The June draft explains the artifact chain to a reader who has never seen an ADR. This file
holds everything gathered on 2026-11-08 while checking whether the blog already covered the
topic. It records what exists, what is missing, where each claim came from, and which pi
session to reopen for the raw conversation.

Anyone resuming the work note should read sections 3, 4, 5 and 8 first.

---

## 2. What the blog already has

Six published artefacts plus one draft touch the topic. All of them explain OpenSpec to other
people. None describes the working approach.

| File | Lines | Angle | Gap |
|---|---|---|---|
| `_posts/2026-05-16-spdd-vs-openspec.md` | 182 | SPDD (Fowler / Zhang Wei) vs OpenSpec, two SDD schools | tool comparison |
| `_posts/2026-06-03-openspec-vs-spec-kit-sdd.md` | 170 | OpenSpec vs GitHub Spec-Kit | tool comparison |
| `_posts/2026-06-23-openspec-scoping-check.md` | 97 | one change or many, `config.yaml` rules | single tactic |
| `_posts/2026-07-10-openspec-overview.md` | 33 | short overview, links to the deck | pointer only |
| `_posts/2026-08-17-openspec-local-viewer.md` | 22 | the viewer tool | side project |
| `_presentations/openspec-overview.md` | 176 | colleague talk | deck |
| `_presentations/openspec-vs-spec-kit-sdd.md` | 102 | colleague talk | deck |
| `_learning/structured-prompt-driven-development.md` | 8 | link card | metadata only |

The `categories: programming` vs `categories: ai` split is inconsistent across the OpenSpec
posts. Worth settling before adding another. `openspec-overview` and `openspec-vs-spec-kit-sdd`
use `programming`; `spdd-vs-openspec` and `openspec-scoping-check` use `ai`.

---

## 3. The June draft and its three problems

`_drafts/2026-06-05-adrs-meet-ai-driven-development-openspec-artifacts.md`, 104 lines,
"From PRDs to ADRs to OpenSpec: How AI-Driven Development Reshapes Engineering Artifacts".

### 3.1 It is a mapping essay, not a work note

It walks BRD, PRD, RFC, ADR, TDD, then maps each onto an OpenSpec artifact. Useful as
reference. It never says how the author works.

### 3.2 It is three months stale

Everything settled after June is absent:

- ADR before code, as a standing rule (settled 2026-08-03)
- `AGENTS.md` as the always-loaded policy carrier (settled 2026-08-01 to 2026-08-03)
- ADR first, then proposal, then apply (settled 2026-09-17)
- exemption for small non-behavioural changes (settled 2026-09-05)
- spec dedup after archive (raised 2026-10-09)
- the autopilot loop, goal-driven rather than change-driven (2026-10-09 to 2026-10-18)

### 3.3 One factual error, verified

The draft says:

> **OpenSpec** fills this gap with a **delta spec**: a `proposal.md` file that translates an
> ADR's abstract decision into concrete, file-level directives using three explicit blocks:
> `ADDED`, `MODIFIED`, and `REMOVED`.

Those blocks are not in `proposal.md`. They are the delta spec format, and they live in
`changes/<change>/specs/<capability>/spec.md`.

Proof from this repo,
`openspec/changes/archive/2025-07-10-add-github-source-link/specs/github-source-link/spec.md`:

```markdown
## ADDED Requirements

### Requirement: Post meta line shows View source link

Each blog post page SHALL display a "View source" link ...

#### Scenario: Post renders with View source link
- **WHEN** a visitor loads a blog post page
- **THEN** the post meta line SHALL contain a link with text "View source"
- **AND** the link href SHALL follow the format `https://github.com/ypyl/ypyl.github.io/blob/main/<page.path>`
```

So the shapes are:

- `proposal.md` carries `## Why`, `## What Changes`, `## Impact`
- `specs/<capability>/spec.md` carries `## ADDED|MODIFIED|REMOVED Requirements`, then
  `### Requirement:` with `#### Scenario:` blocks in WHEN / THEN / AND form
- `design.md` carries the micro design for that one change
- `tasks.md` carries the ordered checklist

The `ADDED` block also holds `### Requirement:` headers and `#### Scenario:` blocks, so the
draft's claim that a delta spec gives "file-level directives" is wrong twice: wrong file, and
wrong granularity. OpenSpec specs describe behaviour, `tasks.md` describes files.

---

## 4. Session log

`~/.pi/agent/sessions` holds 1689 session files, about 1.1 GB. Searching for the string
"openspec" matches 1287 of them, which is useless: the `/opsx-*` slash commands inject the
full skill prompt into the transcript as a user message. Filtering out messages starting with
`Enter explore mode`, `Propose a new change`, `Implement tasks from an OpenSpec change`,
`Archive a completed change`, `Sync delta specs`, `Update an OpenSpec change`, leaves 21
sessions with a genuine ADR mention and 35 with a genuine SDD or ADR mention.

Paths below are relative to `C:\Users\ypyl\.pi\agent\sessions\`.

### 4.1 SDD and tooling research

| Date | Session | What was discussed |
|---|---|---|
| 2026-05-16 | `--C--Users-ypyl-projects-yauhenpylgithubio--\2026-05-16T20-23-35-932Z_019e3275-68fc-7623-a536-700d7fde8bf5.jsonl` | "compare please martinfowler.com/articles/structured-prompt-driven and github.com/Fission-AI/OpenSpec ... I want to compare both of approaches to do SDD". Became `_posts/2026-05-16-spdd-vs-openspec.md`. |
| 2026-06-03 | `--C--Users-ypyl-projects-yauhenpylgithubio--\2026-06-03T17-56-42-813Z_019e8ea1-66bd-7a85-b1c1-bd20dd115c33.jsonl` | "compare openspec with git-spec for SDD development". Then: "I dont liek the article. I want you to write the comparison how you provided me in chat", "do not add fluff and water like 'flips the traditional coding script' and other bullshit". The voice rule for these posts originates here. |
| 2026-07-10 | `--C--Users-ypyl-projects-yauhenpylgithubio--\2026-07-10T19-03-39-634Z_019f4d69-fd72-7572-90ae-fbbe9949d274.jsonl` | Deck for colleagues. Key turn: "I know that there is an issue if code changed outside the openspec (so spec is wrong). What to do in this case?" then "I want you to research. What If Code Changes Outside OpenSpec? is saying when Code is correct, spec is stale when you are in the middle of the process. But what I opened project and see that spec in spec folder is incorrect". Also "what is spec-driven schema", "explain how level 2 is working". This is the spec-drift question and it is still unanswered in writing. |
| 2026-09-01 | `--C--Users-ypyl-projects-yauhenpylgithubio--\2026-09-01T17-44-36-843Z_01a05e12-aaeb-731e-9d10-932c9e66be7e.jsonl` | "it is spec driven development also". |

### 4.2 Scoping

| Date | Session | What was discussed |
|---|---|---|
| 2026-06-23 | `--C--Users-ypyl-projects-yauhenpylgithubio--\2026-06-23T21-33-57-414Z_019ef667-7b26-74ba-a0e4-e454ae360d10.jsonl` | "I am using openspec SDD for ai coding agents. But how to understand if the task is ok for one spec or should be divided into smaller one?" Then the challenge: "it looks weird that I need to include it in Agents.md which will be loaded into any context. It should be only for explore command, no?" and "where are these 2 signal comes from?" and "what will happen if there are 2 signals? proposal will not be created or what?". Became `_posts/2026-06-23-openspec-scoping-check.md`. |

### 4.3 Writing for code agents rather than humans

| Date | Session | What was discussed |
|---|---|---|
| 2026-05-25 | `--C--Users-ypyl-projects-yauhenpylgithubio--\2026-05-25T19-27-52-624Z_019e609b-a130-7a16-a786-6ac01198aa59.jsonl` | "do not add code examples or extra explanation to the article, the audience of this article is code agent LLM, so it should be short and direct", "remember it will act as LLM code agent instruction so it should be direct and without extra details". |
| 2026-07-10 | `--C--Users-ypyl-projects-yauhenpylgithubio--\2026-07-10T21-09-31-214Z_019f4ddd-37ce-7fc9-b00e-8ea5c1bfa2ae.jsonl` | "the primary audience of the post will code agents, so make it minimalistic, avoid providing code snippets", "Remember this file is for code agents, not humans". |

This is a distinct idea from the artifact chain and it is not covered anywhere on the blog:
the same document has a different shape depending on whether a human or an agent reads it.

### 4.4 ADR discipline

The rule originates in the `aidkit` project on 2026-08-03. Everything after that is
application, not invention.

| Date | Session | What was discussed |
|---|---|---|
| 2026-08-01 to 08-05 | `--C--Users-ypyl-projects-aidkit--\2026-08-01T18-49-02-459Z_019fbea8-82fb-7d94-a8cf-7eb95bbb2e35.jsonl` | 77 real user messages, the origin. Sequence: "follow PL instruction and mark it as decision - formulate it ias ADR from programming for future reference if requeired", then "store adr into separate file, let's also update @AGENTS.md to save this -> any significant decision goes to ADR file", then "create @AGENTS.md file and note that ACR should be updated when there is architectrual decision", then "revert changes in C:/Users/ypyl/.pi/agent/AGENTS.md, create a separate in this folder", then "update @AGENTS.md that openspec is used to write code, no direct changes code without proposal". Also "never creates proposal manuallly, only via skill or command" and "you mixes 2 different features in one proposal, let's recreate the whole thing and create 2 proposal". |
| 2026-08-03 | `--C--Users-ypyl-projects-aidkit--\2026-08-03T20-15-09-566Z_019fc944-12fe-7d4c-8063-da1c585635cd.jsonl` | Continuation of the same project, same rule text. |
| 2026-08-04 | `--C--Users-ypyl-projects-mini-render--\2026-08-04T19-25-28-500Z_019fce3c-f234-72fb-9131-eead0530c08a.jsonl` | "nope, let's store it as ADR statement". Smallest possible instance of the rule. |
| 2026-08-31 | `--C--Users-ypyl-projects-theta--\2026-08-31T18-13-19-754Z_01a05906-9909-78ba-82e3-be7ad7cec99a.jsonl` | "let's create folder for this project in ~\projects\pharos create doc folder in it and dump all our decisions their as ADR files". |
| 2026-08-31 | `--C--Users-ypyl-projects-pharos--\2026-08-31T18-40-07-832Z_01a0591f-2298-7614-9d63-82a678908c41.jsonl` | "update @AGENTS.md that all significant changes will be done via openspec tool (Spec Driven Development) and there is openspec folder". Compact statement of the policy. |
| 2026-09-03 | `--C--Users-ypyl-projects-folio--\2026-09-03T18-22-53-526Z_01a06882-6e56-7400-b8a2-777f1010ae81.jsonl` | "create adr folder and extract adr from docs folder", "create AGENTS.md with basic tech stac of the project and info about research and adr folder", "update @AGENTS.md that we are using openspec for any change". Retrofit case: ADRs existed in `docs/` and were pulled out into their own folder. |
| 2026-09-03 | `--C--Users-ypyl-projects-folio--\2026-09-03T20-58-20-675Z_01a06910-c083-74bf-b14d-021db4908a23.jsonl` | "let's use github.com/tw93/Kami for design (create adr for it also)". |
| 2026-09-03 | `--C--Users-ypyl-projects-folio--\2026-09-03T21-26-18-230Z_01a0692a-5976-7d0b-ad55-4a50cded4265.jsonl` | "`@adr/0011-kami-design-language.md` analyze this one and let's create design.md with instructions about the design, copy relevant information from kami repository and reference it in @AGENTS.md". The ADR to agent-instruction path, which the June draft does not describe. |
| 2026-09-03 to 09-04 | `--C--Users-ypyl-projects-folio--\2026-09-03T21-28-52-904Z_01a0692c-b5a8-7e43-9f49-7d5226d7ac1a.jsonl` | "do not think about supporting any migration we are creating new app -> record it in @AGENTS.md andupdate any adr if required". |
| 2026-09-05 | `--C--Users-ypyl-projects-folio--\2026-09-05T09-43-04-156Z_01a070f3-3cdc-7741-9ba8-c767357aba0c.jsonl` | "based on our explorarion, should we create also new ADR files?", "add also to extract adr and propose to create during explore". ADRs move from output to a step inside explore. |
| 2026-09-05 to 09-06 | `--C--Users-ypyl-projects-folio--\2026-09-05T21-41-06-336Z_01a07384-9ea0-7741-9ba8-c77c92aa876b.jsonl` | "any adr to create based on tha last proposal?", and the exemption: "update @AGENTS.md, that small fixes/refactros if it is not behavior change can be done directly without openspec". |
| 2026-09-16 | `--C--Users-ypyl-projects-folio--\2026-09-16T21-02-20-736Z_01a0ac07-163f-75f1-b979-e031c86e36dc.jsonl` | "ok, let's revert this change and leave it as it is, record it in adr why is like that". An ADR as a record of a rejected change. |
| 2026-09-17 to 09-18 | `--C--Users-ypyl-projects-folio--\2026-09-17T19-57-35-699Z_01a0b0f2-2a52-7501-b6ee-fd4554e9795d.jsonl` | "let's create adr first and then proposal, apply proposal and archive it after you test". Then `--C--Users-ypyl-projects-folio--\2026-09-18T20-17-34-419Z_01a0b62a-d0d3-7501-b6ee-fd4a7be42bb2.jsonl`: "yes, exit explore mode, create proposal, adr if needed, apply proposal, test it, archive and commit/push". This is the full loop stated in one sentence. |
| 2026-10-25 | `--C--Users-ypyl-projects-folio--\2026-10-25T17-22-56-450Z_01a17416-3b41-714e-a3b2-7c71a0de6533.jsonl` | "ok, let's add it, add adr probably if it makes sense and go to implementation". |
| 2026-11-08 | `--C--Users-ypyl-temp-folder-ai--\2026-11-08T11-29-32-824Z_01a1baeb-b896-7615-a78b-099dcdfb2570.jsonl` | "no, let's save our discussiin, findings in ~/projects/theta this extention will be aprt of theta project Add ADR folder or docs folder or something like that to store main pointsa". Rule applied to a research conversation, not just to code. |

### 4.5 Autonomy

| Date | Session | What was discussed |
|---|---|---|
| 2026-10-09 to 10-10 | `--C--Users-ypyl-temp-folder-ai-release-flows--\2026-10-10T06-10-57-286Z_01a1246f-9e85-71a8-844f-6e59c38e1519.jsonl` | 96 real user messages, the design argument for the autonomous pipeline. "user describe the problem with as much details as understand the problem, agent grind user for additional details", "it should never escalate or ask questions before the task is completed in one way or another", "model can grind only at the begining after user states its tasks and it is all", "someone also should init openspec to start implementation", "there is also should be a step which will simplify, deduplicate specs after archive as openspec tents to complicate/duplicate specs". |
| 2026-10-13 to 10-18 | `--C--Users-ypyl-projects-openspec-autopilot--\2026-10-13T20-48-24-846Z_01a13706-090d-747f-b6b1-cc066fb77df3.jsonl` | Landscape research and the goal-driven vs change-driven distinction. "withing grind step it is important to clarify how it is possible to check the final result in way or another", "Will it make sense to have self-grind by agent? after each plan step?", "I don't see critic in Pipeline in readme". |
| 2026-09-12 to 09-13 | `--C--Users-ypyl-projects-claude-certifications--\2026-09-12T09-31-37-426Z_01a094f5-4651-708f-a7ef-3eb64236cac5.jsonl` | Pasted Claude architect course transcript, 171 user messages. Reference material on decomposition and agent scope, not original thinking. Do not cite as an opinion. |
| 2026-10-18 | `--C--Users-ypyl-mega-CV--\2026-10-18T18-02-54-962Z_01a1502e-5071-70f8-9a49-0b11615b6099.jsonl` | CV work: "how it is better to improve their with AI - Spec Driven development". Shows SDD being pitched outside engineering. |

### 4.6 Notes cleanup

| Date | Session | What was discussed |
|---|---|---|
| 2026-07-29 | `--C--Users-ypyl-mega-logseq-pages--\2026-07-29T21-07-20-430Z_019fafb4-0ced-7e1c-83c8-72f59e4facff.jsonl` | Single message: "please review and clean up `openspec, PRD, ADR, TDD and so on.md`". That Logseq page is at `~/mega/logseq/pages/openspec, PRD, ADR, TDD and so on.md` and holds the BRD to code pipeline diagram plus the PRD vs ADR table. Closest existing prose to the work note, and it is a private note, not a post. |

---

## 5. The rule set as it currently stands

Distilled from section 4.4. This is the content the work note needs to carry.

1. **A decision gets recorded before or with the code.** Trigger: anything architectural, a
   data model choice, a scope change, a tech-stack change, or something hard to reverse.
2. **ADRs are append-only and numbered.** Format: Status, Context, Decision, Consequences.
   Include a `Deferred` note when part of the decision is postponed.
3. **`AGENTS.md` is the carrier, not the archive.** It holds the pointer ("any significant
   decision goes to the ADR file"), the ADR does not live inside it. Project-level file wins
   over the global one at `~/.pi/agent/AGENTS.md`.
4. **OpenSpec is the only path to implementation.** Wording that survived: "openspec is used
   to write code, no direct changes code without proposal", "never create proposal manually,
   only via skill or command".
5. **Order is ADR, proposal, apply, archive.** Not the reverse. The ADR settles what and why,
   the proposal settles the delta, the tasks settle the order.
6. **Exemption:** small fixes and refactors with no behaviour change skip OpenSpec.
7. **One proposal, one intent.** If two of the split signals fire, split it.
8. **ADRs are also a step inside explore.** Explore proposes new ADRs; it does not only
   extract them afterwards.
9. **An ADR can record a rejection.** "leave it as it is, record it in adr why is like that".

---

## 6. Contradictions and drift found

These need a decision before the work note is written, not after.

1. **`openspec/config.yaml` in this repo is bare.** It contains one line, `schema: spec-driven`.
   The published post `2026-06-23-openspec-scoping-check.md` recommends `context:` and
   `rules.proposal:` in that file and says "config.yaml alone is sufficient". The repo does not
   follow its own advice. Either the repo config gets the rules, or the post is wrong about
   where the rule belongs.
2. **The policy is practised but not written down in the repos that practise it.**
   `~/.pi/agent/ADR-RULE.md` existed as a standalone file, written 2026-08-03 21:36 during the
   `aidkit` session, and nothing linked to it. `~/.pi/agent/AGENTS.md` contains no ADR or
   OpenSpec text at all. This repo's `AGENTS.md` mentions `spec-driven-development` exactly
   once, as a tag alias (line 190), never as a workflow rule, yet the repo has 19 archived
   OpenSpec changes under `openspec/changes/archive/`. Verified 2026-11-08.
3. **The `aidkit` session asked to revert the global `AGENTS.md` change** and keep the rule
   project-local. That produced `ADR-RULE.md` as an orphan. **Resolved 2026-11-08:** the orphan
   was deleted. Nothing referenced it. The rule now lives only in section 5 of this file, which
   means the work note is the only place it survives. Do not lose it.
4. **Two category conventions** for OpenSpec posts, `ai` and `programming`.

---

## 7. Reference artefacts on disk

| Path | What it is |
|---|---|
| `~/.pi/agent/ADR-RULE.md` | **Deleted 2026-11-08.** Was the ADR rule as extracted, 1654 bytes, mtime 2026-08-03 21:36, orphaned: no reference in `AGENTS.md`, prompts, skills or settings. Content survives in section 5 of this file. |
| `~/projects/yauhenpylgithubio/openspec/changes/archive/` | 19 archived changes, real examples of proposal, design, tasks and delta spec. Best source of authentic samples for the post. |
| `~/projects/yauhenpylgithubio/openspec/config.yaml` | One line, `schema: spec-driven`. See section 8. |
| `~/mega/logseq/pages/openspec, PRD, ADR, TDD and so on.md` | Private note, the artifact pipeline diagram and the PRD vs ADR table. The June draft's source. |
| `~/mega/logseq/pages/OpenSpec_ Organizing Specifications by Domain and Capability.md.md` | 929 lines, the domain/capability spec layout, with a ready `config.yaml` block and schema forks. Finished material, unpublished. See section 8.4. |
| `_drafts/2026-06-05-adrs-meet-ai-driven-development-openspec-artifacts.md` | The draft being revised. |

---

## 8. Addendum 2026-11-08: `config.yaml` and the spec folder layout

Added after the main pass. This is the part most likely to be forgotten, and it is the only
part where finished material already exists.

### 8.1 The question

Does any existing `openspec/config.yaml` under `~/projects` already describe a different spec
folder structure, a domain / feature / spec hierarchy instead of the flat default?

### 8.2 Answer: no. All of them are flat.

Audited all 14 `openspec/config.yaml` under `~/projects`, plus 10 under `~/temp-folder-ai`.
Grouped by what they actually contain:

| Config | Size | Structure guidance |
|---|---|---|
| `yauhenpylgithubio` | 1 line | none, `schema: spec-driven` and nothing else |
| `aidkit`, `mantine-chat-ui`, `polygon-merge`, `rdb`, `search-with-source`, `theta`, `vs-code-copilot-stats` | 573 to 922 B | untouched default template, every example still commented out |
| `ai-eval-cli`, `mini-render`, `llmclip` | 1.5 to 4.4 KB | rich `context` and `rules`, but about stack, layering and artifact quality |
| `openspec-viewer`, `folio`, `gym-pwa` | 3.0 to 4.0 KB | richest. Closest approximation, see 8.5 |

The filesystem agrees. Every `spec.md` in every project sits exactly one directory under
`specs/`, for example `openspec/specs/checklist-engine/spec.md`. A search for any `spec.md`
nested deeper than `specs/<one-dir>/spec.md` returns nothing across `~/projects`,
`~/temp-folder-ai` and `~/mega`.

### 8.3 Nested paths need no config at all

The installed `@fission-ai/openspec` is 1.14.1. Its built-in `spec-driven` schema instruction
already permits the two-level form:

> `<capability-path>` is the spec directory relative to `specs/` (for example, `user-auth` or
> `identity/user-auth`). Preserve the full path... use kebab-case for path segments you
> introduce and follow the project's existing spec organization.

So `openspec/specs/<domain>/<capability>/spec.md` works today with an empty config. What the
config does is bias the agent to prefer that tier. The distinction matters for the write-up:
the gap is preference and consistency, not capability. Writing the config unlocks nothing. It
stops the agent defaulting to flat.

### 8.4 The finished material already exists, in a note

`~/mega/logseq/pages/OpenSpec_ Organizing Specifications by Domain and Capability.md.md`,
929 lines. This is the missing write-up. Contents:

| Section | Content |
|---|---|
| 4 | Structure diagram for `openspec/specs/<domain>/<capability>/spec.md`, and the change-side mirror `changes/<change>/specs/<domain>/<capability>/spec.md` |
| 5 | Domain vs capability vs requirement vs scenario vs change, each defined |
| 6 | A ready `config.yaml` snippet: a long `context` block plus `rules.specs:` |
| 7 | `openspec schema fork spec-driven domain-driven` for when config is not enough |
| 8 | How to rewrite the schema's spec instruction to enforce the decision process |
| end | Which knob does what: `context` for structure, `rules` for enforcement |

Condensed from section 6:

```yaml
schema: spec-driven

context: |
  Specification organization:
  OpenSpec specifications are organized by business domain and capability.
  Canonical structure:
    openspec/specs/<domain>/<capability>/spec.md
  Domains represent stable business areas, bounded contexts, or major product areas.
  Capabilities represent cohesive business behavior within a domain.
  Before creating a new specification:
  1. Inspect existing specifications.
  2. Identify the domain that owns the behavior.
  3. Identify whether an existing capability already covers the behavior.
  4. Extend an existing capability specification when appropriate.
  5. Create a new capability only when the behavior is genuinely distinct.
  6. Create a new domain only when no existing domain is appropriate.
  A new ticket, user story, API endpoint, or implementation task does not
  automatically justify creating a new specification.

rules:
  specs:
    - Organize specifications as specs/<domain>/<capability>/spec.md.
    - Reuse existing domain and capability specifications whenever possible.
    - Do not create a new specification solely because a new change exists.
    - Inspect existing specifications before creating a new specification.
    - Prefer extending an existing capability when the behavior belongs to it.
    - Keep specifications implementation-independent.
```

`openspec schema fork` verified present in 1.14.1. `openspec schema --help` lists `fork`,
`init`, `validate` and `which`, and marks the group experimental.

### 8.5 Closest current approximation

Two projects already steer structure without naming a domain tier:

- `gym-pwa` proposal rule: "Preserve an existing capability's spec path; new capabilities are
  listed in PLAN.md under Phase 2"
- `mini-render` proposal rule: "Check `openspec/specs/` for existing spec names before filling
  Capabilities"

Neither introduces a domain level. Both are the weaker version of the same idea as 8.4.

### 8.6 Two independent follow-ups, do not conflate them

1. **Write the post.** The 929-line note is finished material in a private Logseq page.
   Extract it as `openspec-organizing-specs-by-domain`. Separate from the ADR work note, and
   the one that would teach a reader something new, because nothing published covers nesting.
2. **Adopt it here.** Put the section 6 block into this repo's `openspec/config.yaml`. The file
   is one line while `2026-06-23-openspec-scoping-check.md` tells readers the file is
   sufficient. Fork a `domain-driven` schema afterwards only if config proves too weak.

Doing 2 without 1 leaves the repo inconsistent with its own advice and nothing to point at for
why the config looks the way it does.

---

## 9. Outline for the work note

Deliberately different from the June draft. That one explains the artifacts. This one explains
the operating rules and links to the draft for the artifact table.

1. **Open with the rule, not the definition.** "No code lands without a decision and a spec."
   One sentence, then the exemption, because the exemption is what makes the rule survivable.
2. **The three documents and what each one is allowed to answer.** ADR answers why, proposal
   answers what changes, `tasks.md` answers in what order. One table, three rows.
3. **The loop.** explore, propose, apply, archive. Where the ADR lands in each step. Where
   explore stops being useful.
4. **The carrier.** `AGENTS.md` holds the pointer, not the content. Show the two lines that do
   the work.
5. **Scoping.** Link `2026-06-23-openspec-scoping-check.md` rather than repeat it.
6. **What breaks.** Spec drift. Code correct, spec stale. The 2026-07-10 question. Honest
   section, no tidy answer available yet.
7. **Where this does not apply.** Small refactors, spikes, throwaway scripts. Protects the
   reader from cargo-culting the whole pipeline onto a 40-line script.
8. **The autonomous variant.** One paragraph, link the pipeline, mark it experimental.

Length target: 900 to 1200 words. No tutorial, no install instructions, no
`npm install @fission-ai/openspec`.

---

## 10. Open questions to settle before writing

1. Does the work note go in `_posts/` as a new entry, or does the June draft get rewritten in
   place? New post is cleaner: the draft is a mapping essay and deserves to survive as one.
   If a new post, the June draft can be published too, after the delta-spec fix, as the
   companion reference.
2. Which category: `ai` or `programming`? Section 6.4 has to be settled anyway.
3. Is the ADR rule phrased as personal practice or as a recommendation? The sessions show it as
   a personal rule enforced through `AGENTS.md`. Writing it as a recommendation invites
   disagreement about a rule that was never offered as generic advice.
4. Does the autopilot belong in the same post, or a separate one? It is a different claim
   (goal-driven vs change-driven), and it is unfinished.
5. Is the spec-drift section honest enough to publish if there is no answer? Option: publish it
   as the open problem and invite comments. It is the most interesting part and the blog has no
   comment system.

---

## 11. How to resume

Dump the real user messages from a session (filters out the injected `/opsx-*` skill prompts):

```python
import json, sys
SKILL = ("Enter explore mode", "Propose a new change",
         "Implement tasks from an OpenSpec change", "Archive a completed change",
         "Sync delta specs", "Update an OpenSpec change")
with open(sys.argv[1], encoding="utf-8", errors="replace") as fh:
    for line in fh:
        if '"user"' not in line:
            continue
        try:
            rec = json.loads(line)
        except Exception:
            continue
        if rec.get("type") != "message":
            continue
        msg = rec.get("message") or {}
        if msg.get("role") != "user":
            continue
        c = msg.get("content")
        parts = [c] if isinstance(c, str) else [
            p.get("text") or "" for p in c
            if isinstance(p, dict) and p.get("type") == "text"]
        text = "\n".join(parts).strip()
        if not text or text.startswith(SKILL):
            continue
        print(f"[{rec.get('timestamp','')[:16]}] {' '.join(text.split())[:400]}")
```

Run it as `python dump.py "<session file>"`.

Search across all sessions, excluding the skill noise:

```bash
cd ~/.pi/agent/sessions
grep -rl "ADR" --include="*.jsonl" . | while read -r f; do
  echo "$f"
done
```

Beware: a raw `grep -c openspec` returns 1287 of 1689 files and means nothing.
