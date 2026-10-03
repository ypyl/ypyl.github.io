---
layout: post
title: "Anthropic Opens Claude Code to TypeScript Mods"
date: 2026-10-03
---

Anthropic has introduced **mods** in Claude Code: small TypeScript functions that change how the assistant behaves in the CLI and the desktop app. Where hooks could only react to an event, a mod can run before, after, or instead of it, letting it rewrite a prompt before it reaches the model, block or retry a tool call, approve or deny a permission request, redact secrets from tool output, or add and replace interface elements such as buttons and inputs. Mods ship inside plugins and are not sandboxed, so they get the same access to the machine as Claude Code itself, and Anthropic advises installing them only from sources you trust; Claude Code can also write a mod on request and hot reload it without restarting the session. On Team and Enterprise plans, a built-in `sec-default` mod loads first and blocks user-installed mods from overriding permission deny rules, and the built-in `/diff` feature now ships as a mod that admins can turn off or replace.

**Related:** [Claude Code Makes Auto Mode Default](/news/2026/08/11/claude-code-auto-mode-default/), [Anthropic Launches Claude Code Security](/news/2026/02/21/anthropic-launches-claude-code-security/)

[Customize Claude Code with mods](https://claude.com/blog/claude-code-mods)
