---
title: GitHub Copilot CLI - First Impressions
date: 2026-04-08
tags: github, copilot, ai, cli, learning
categories: programming
---

Started going through the [GitHub Copilot CLI for Beginners](https://github.com/github/copilot-cli-for-beginners/tree/main) course. Here are my notes so far.

<!--more-->

## Three Interaction Modes

| Mode | When to Use |
|------|-------------|
| **Plan** | Complex tasks - map out route, review stops, agree on plan before coding |
| **Interactive** | Exploration and iteration - ask questions, customize, get real-time feedback |
| **Programmatic** | Quick, specific tasks - automation, scripts, CI/CD, single-shot commands |

**Interactive mode syntax**: `copilot` - then just a normal conversation with the agent
**Plan mode syntax**: `/plan` (inside interactive session)
**Programmatic mode syntax**: `copilot -p "<prompt>"`

## Autopilot Mode

Fourth mode accessed via Shift+Tab. Copilot executes an entire plan without pausing for input after each step.

**Typical workflow**: Plan → Accept → Autopilot

### Permission Flags

- **`--allow-all` (or `--yolo`)**: Grants Copilot permission to use all tools, paths, and URLs without asking. Enter `/allow-all` or `/yolo` during session (note: these don't toggle - entering again doesn't disable).
- **`--no-ask-user`**: Suppresses clarifying questions. Agent makes decisions autonomously, but unlike autopilot, won't continue through successive steps requiring AI interaction.
- **`--max-autopilot-continues`**: Sets a maximum continuation limit to prevent runaway loops.

### Autopilot Programmatically

```bash
copilot --autopilot --yolo --max-autopilot-continues 10 -p "YOUR PROMPT HERE"
```

## Essential Slash Commands

Six commands cover 90% of daily use in interactive mode:

| Command | What It Does | When to Use |
|---------|-------------|-------------|
| `/clear` | Clear conversation and start fresh | When switching topics |
| `/help` | Show all available commands | When you forget a command |
| `/model` | Show or switch AI model | When you want to change the AI model |
| `/plan` | Plan your work out before coding | For more complex features |
| `/research` | Deep research using GitHub and web sources | When you need to investigate before coding |
| `/exit` | End the session | When you're done |

## Development Workflows

Five core workflows: **Code Review**, **Refactoring**, **Debugging**, **Test Generation**, **Git Integration**.

### Code Review
- Basic review with `@file` reference
- Input validation review (specific focus area)
- Cross-file project review (`@folder/`)
- Interactive code review (multi-turn conversation)
- Review checklist template (severity categorized)
- **`/review`** - Built-in code-review agent for staged/unstaged changes

### Refactoring
- Simple refactoring (type hints, patterns)
- Separate concerns across files
- Improve error handling (custom exceptions)
- Add documentation (docstrings)
- Safe refactoring with tests first

### Debugging
- Simple debugging ("Expected X but got Y" pattern)
- Bug Detective - AI finds related bugs
- Security audit ("Find all security vulnerabilities")
- Understanding an Error (stack trace + `@file`)
- Debugging with test case (exact input/output)
- Trace issue through code (multiple files)
- Understanding data issues (JSON + code)

### Test Generation
- Test explosion - 2 tests vs 15+ tests
- Unit tests (specific function)
- Test for specific scenarios (duplicates, empty, concurrent)
- Add tests to existing file (edge cases)

### Git Integration
- Generate commit messages: `copilot -p "Generate commit message for: $(git diff --staged)"`
- Explain changes: `copilot -p "Explain: $(git show HEAD --stat)"`
- PR description from branch changes
- **`/pr [view|create|fix|auto]`** - Work with pull requests
- **`/delegate`** - Hand off to background agent
- **`/diff`** - Review session changes before commit

### Research Before Plan
Use **`/research`** for deep investigation before coding. Copilot searches GitHub and web sources, returns summary with references. Works well *before* `/plan` - research the approach first, then plan.

## Agents

Custom agents are `.agent.md` files for specialized AI personas.

**Locations:**
- `.github/agents/` - Project-specific, team-shared
- `~/.copilot/agents/` - Global, personal

**Usage:**
- Interactive: `> /agent` to list and select agents
- Programmatic: `copilot --agent python-reviewer`

**Instruction files (auto-loaded):**
- `AGENTS.md` - Cross-platform, project root
- `.github/copilot-instructions.md` - GitHub Copilot specific
- `.github/instructions/*.instructions.md` - Granular, topic-specific

Use **`/init`** to scan project and generate configuration files automatically.

## Skills

Skills are folders with instructions, scripts, and resources that Copilot auto-loads when relevant.

**Skills vs Agents vs MCP:**
| Feature | What It Does | When to Use |
|---------|-------------|-------------|
| Agents | Changes how AI thinks | Need specialized expertise across many tasks |
| Skills | Provides task-specific instructions | Specific, repeatable tasks with detailed steps |
| MCP | Connects external services | Need live data from APIs |

**`/skills` commands:**
- `/skills list` - Show all installed skills
- `/skills info <name>` - Get details about a specific skill
- `/skills add <name>` - Enable a skill from repository or marketplace
- `/skills remove <name>` - Disable or uninstall a skill
- `/skills reload` - Reload after editing SKILL.md files

Find skills at [agentskills.io](https://agentskills.io/home) and [Awesome Copilot](https://github.com/github/awesome-copilot).

**Plugins** are installable packages that bundle skills, agents, and MCP server configurations together - like an "app store" for Copilot CLI extensions.

## Worth Knowing

**Closed source, but open SDKs**: The Copilot CLI itself is closed source, but GitHub provides [open-source SDKs](https://github.com/github/copilot-sdk) that allow programmatic access to Copilot in custom applications.

## Context and Conversations

- **File references**: Use `@` to reference files, `@folder/` for all files in a folder
- **Wildcards**: `@folder/*.py` for all in folder, `@**/test_*.py` recursive; `@image.png` for images
- **Sessions auto-save**: `copilot --continue` to resume last session, `copilot --resume` to pick from list
- **Switch session without restart**: `/resume` inside active session
- **Rename session**: `/rename new-name` for easier identification later
- **Context management**: `/context` shows token usage; `/new` saves and starts fresh, `/clear` discards; `/rewind` rolls back
- **Session info**: `/session` shows workspace summary, `/usage` shows metrics
- **Access outside cwd**: Use `/add-dir /path/to/directory` to grant access if "Permission denied" or "File not found"
- **Share session**: `/share file|gist|html` exports session as markdown, gist, or interactive HTML

## Resources

- [Official GitHub Copilot CLI documentation](https://docs.github.com/en/copilot/how-tos/copilot-cli)
- [GitHub Copilot CLI command reference](https://docs.github.com/en/copilot/reference/cli-command-reference)
- [Course repo](https://github.com/github/copilot-cli-for-beginners/tree/main)
- [Awesome Copilot](https://github.com/github/awesome-copilot) - Community skills, agents, and resources
