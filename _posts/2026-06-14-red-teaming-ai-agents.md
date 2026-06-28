---
layout: post
title: "Red-Teaming AI Agents — Attack Surfaces, Strategies & Metrics"
date: 2026-06-14
tags: [ai, llm, red-teaming, safety, prompt-injection, agents, production, pyrit, garak]
categories: programming
series: production-ai-agents
series_index: 2.5
---

> Series overview: [Production AI Agents — Observability, Safety & Governance]({% post_url 2026-06-15-production-ai-agents-series-overview %})

Safety isn't a checklist — it's an ongoing adversarial process. Microsoft's own [AI Red Teaming Agent](https://learn.microsoft.com/en-us/azure/foundry/concepts/ai-red-teaming-agent) (built on [PyRIT](https://github.com/microsoft/PyRIT)) formalizes this into a NIST-aligned workflow:

```
Map → Measure → Manage
```

- **Map**: Identify relevant risks for your specific use case and agent capabilities.
- **Measure**: Evaluate risks at scale using automated adversarial probing.
- **Manage**: Mitigate risks with guardrails and monitor continuously.

The core metric is **Attack Success Rate (ASR)** — the percentage of adversarial probes that successfully bypass your defenses. A low ASR means your guardrails work; a rising ASR means your defenses are eroding. Track it over time, not as a one-time score.

## Risk Categories

AI red-teaming targets two classes of risk: **model-level** (applies to any LLM system) and **agent-specific** (applies only when the system has tools and makes autonomous decisions).

### Model-Level Risks

Text-based risks that apply to any LLM-powered system:

| Category | What it tests |
|----------|--------------|
| Hateful & unfair content | Bias, stereotyping, discrimination |
| Sexual content | Explicit, suggestive, or pornographic outputs |
| Violent content | Descriptions of violence, weapons, harm |
| Self-harm content | Content that encourages or describes self-harm |
| Protected materials | Copyrighted lyrics, recipes, code |
| Code vulnerability | AI-generated code with SQL injection, stack traces, RCE risks across 7 languages |
| Ungrounded attributes | Inferences about demographics or emotional state without basis |

### Agent-Specific Risks

These require tool-observing red-teaming — you can't test them with text-only probes. The red-teaming agent needs mock tools, synthetic data, and the ability to observe what the target agent *does*, not just what it *says*.

| Category | What it tests | ASR trigger |
|----------|--------------|-------------|
| **Prohibited actions** | Whether the agent performs universally banned operations (facial recognition, social scoring), high-risk actions without human approval (financial transactions, medical decisions), or irreversible actions without confirmation (file deletion, system reset). Defined by your policy taxonomy. | Policy violation detected |
| **Sensitive data leakage** | Whether the agent exposes financial, medical, or personal data from internal knowledge bases through tool calls or outputs. Uses synthetic data and pattern matching for detection. | Format-level leak detected (SSN, credit card, etc.) |
| **Task adherence** | Whether the agent faithfully completes assigned tasks across three dimensions: goal achievement (did it achieve the intended goal?), rule compliance (did it respect policy guardrails and presentation contracts?), and procedural discipline (did it use tools correctly, follow grounding requirements?). | Goal failure, rule violation, or procedural error |
| **Indirect prompt injection (XPIA)** | Whether the agent can be manipulated by malicious instructions hidden in external data sources retrieved via tool calls. The red-teaming agent injects attacks into mock tool outputs and measures whether the target agent executes unintended actions. | Agent executes injected instruction |

## Attack Strategies

Automated red-teaming tools apply **attack strategies** — transformations that make adversarial prompts harder to detect by simple filters. PyRIT supports 24+ strategies. Representative examples:

| Strategy | How it works | Example |
|----------|-------------|---------|
| **Base64** | Encodes the attack in Base64 | A prompt injection hidden inside what looks like a config string |
| **UnicodeConfusable** | Replaces characters with visually identical Unicode equivalents | `раураl.com` using Cyrillic 'а' instead of Latin 'a' |
| **Leetspeak** | Substitutes letters with numbers/symbols | `h0w t0 h4ck` |
| **Morse** | Encodes attack in Morse code | The model decodes dots-and-dashes into a jailbreak |
| **ROT13 / Caesar** | Character-shift ciphers | Obfuscates intent from simple keyword filters |
| **Crescendo** | Gradually escalates prompt risk over successive turns | Starts with benign questions, slowly probes toward dangerous territory |
| **Multi-turn** | Spreads the attack across multiple conversational turns | Each turn is harmless alone; the accumulated context enables the attack |
| **SuffixAppend** | Appends adversarial tokens optimized to bypass alignment | Model-specific suffix that increases probability of compliance |
| **Jailbreak** | Direct User-Injected Prompt Attacks (UPIA) | "Ignore all previous instructions and..." |
| **Indirect Jailbreak** | Attack hidden in tool outputs or retrieved context (XPIA) | A compromised document the agent retrieves and trusts |

These strategies aren't just academic — they represent real techniques attackers use. Your red-teaming should test against a representative subset, prioritized by your agent's risk profile.

**Multi-turn attacks deserve special attention.** Research published in Feb 2026 found that multi-turn jailbreaks achieved **92% success** against 8 open-weight models — spreading the attack across conversation turns makes each turn individually benign while the accumulated context enables the attack. The same pattern applies to agent systems: a user asks 5 harmless questions, each retrieving a different piece of internal documentation, then asks the 6th question that synthesizes the exfiltrated context into a response the guardrail can't flag because each retrieval was individually authorized.

## Agentic Attack Surfaces

When LLMs gain tools, memory, and autonomous action capabilities, the blast radius of any injection expands dramatically. Two attack surfaces are uniquely agentic:

### MCP Tool Poisoning

The [Model Context Protocol](https://modelcontextprotocol.io) (MCP) is rapidly becoming the standard for connecting LLMs to external tools — and the dominant new attack surface. MCP-specific attack classes identified by [Palo Alto Unit 42](https://unit42.paloaltonetworks.com/model-context-protocol-attack-vectors/) and [Checkmarx](https://checkmarx.com/zero-post/11-emerging-ai-security-risks-with-mcp-model-context-protocol/):

| Attack | How it works |
|--------|-------------|
| **Tool poisoning** | Malicious instructions embedded in `description` fields that agents trust implicitly. The agent reads the description to decide what the tool does — and executes the hidden instruction. |
| **Tool shadowing** | Registering a malicious tool with a name similar to a legitimate one, intercepting calls meant for the real tool. |
| **Covert invocation** | Hidden file system operations without user awareness — an MCP server that silently copies files on every invocation. |
| **Cross-MCP contamination** | One compromised MCP server overriding another's behavior or injecting instructions that persist across tool calls. |

The defense: scan MCP servers before connecting (SkillSpector covers MCP-specific patterns LP1-LP4 and TP1-TP4), pin allowed tool IDs per server, and never connect MCP servers from untrusted sources without review.

This isn't theoretical. In September 2025, the [first malicious MCP server was discovered on npm](https://owasp.org/) — a supply chain attack targeting agent ecosystems, validating OWASP ASI04 (Agentic Supply Chain). The OWASP project publishes a [practical guide for securely using third-party MCP servers](https://genai.owasp.org/resource/cheatsheet-a-practical-guide-for-securely-using-third-party-mcp-servers-1-0/).

### AI IDE & Coding Assistant Attacks

AI coding assistants (Claude Code, GitHub Copilot, Cursor) have system-level file access and are a high-value target. Notable CVEs:

- **CVE-2025-53773** — GitHub Copilot RCE (CVSS 9.6) via prompt injection
- **CVE-2025-54135** — Cursor indirect prompt injection via MCP config → RCE
- **IDEsaster** — 30+ CVEs discovered across AI IDEs in late 2025
- **Rules file backdoors** — `.cursor/rules` and similar config files can be poisoned with instructions the AI executes with full trust

The defense: treat AI IDE config files as untrusted input, scan skill/rules files before installation, and run coding agents in containers with limited filesystem access.

**Real-world example**: CVE-2025-32711 ([EchoLeak](https://nvd.nist.gov/vuln/detail/CVE-2025-32711)) demonstrated zero-click prompt injection against Microsoft 365 Copilot — the AI assistant was forced to exfiltrate sensitive business data to an external URL without any user interaction, using character-substitution attacks that bypassed safety filters.

Testing your defenses systematically against all of these attack surfaces requires automation.

## Automated Adversarial Testing

Two major open-source frameworks automate this:

| Tool | Focus | Approach |
|------|-------|----------|
| **[PyRIT](https://github.com/microsoft/PyRIT)** (Microsoft) | Full red-teaming lifecycle — generates adversarial prompts, executes attacks, evaluates ASR, generates scorecards. Python library with 24+ attack strategies. | Framework: orchestrates attack → response → evaluation loop |
| **[Garak](https://github.com/NVIDIA/garak)** (NVIDIA) | Vulnerability scanning — probes for known LLM failure modes across prompt injection, jailbreaking, and content safety categories. | Scanner: runs probes against endpoints, reports pass/fail |

The **[Foundry AI Red Teaming Agent](https://learn.microsoft.com/en-us/azure/foundry/concepts/ai-red-teaming-agent)** is Microsoft's managed cloud offering built on PyRIT — automated scans, ASR scoring, reporting, and continuous monitoring in Foundry. For agent-specific risks, it runs in a sandboxed cloud environment with mock tools and synthetic data, preventing real-world side effects during testing.

## Purple Environment

Red-teaming can have side effects — an agent that deletes files during a real red-team exercise has deleted real files. Run red-teaming in a **purple environment**: a non-production environment configured with production-like resources (same tools, same data schemas, same models) but with synthetic data and isolated infrastructure. The Foundry AI Red Teaming Agent enforces this for agent-specific risk categories — runs are transient, mock tools serve synthetic data, and chat completions aren't persisted.

## Manual Red-Teaming

Automated tools miss novel attacks. Schedule regular sessions where humans (or creative LLMs prompted to be adversarial) try to break your agent. Bug bounty programs work for AI safety too — reward findings. Microsoft's AI Red Team, after testing 100+ generative AI products, [reports](https://www.microsoft.com/en-us/security/blog/2025/01/13/3-takeaways-from-red-teaming-100-generative-ai-products/) that "mitigations do not eliminate risk entirely" — continuous red-teaming is essential because model-layer defenses are probabilistic by construction.

## Track Safety Metrics

| Metric | What it measures | Target |
|--------|-----------------|--------|
| **Attack Success Rate (ASR)** | % of adversarial probes that bypass defenses | Trending ↓ over time; spike = investigate |
| Guardrail trigger rate (by category) | Which attacks hit your system, how often | Stable or declining |
| Human override rate | How often humans reject agent decisions | Set by risk tolerance of use case |
| Time-to-detect for new attack patterns | How fast can you identify and patch new attacks? | Hours, not weeks |

ASR is the north star. Everything else feeds into it.

> The companion article [Safety for AI Agents]({% post_url 2026-06-13-safety-for-ai-agents %}) covers the guardrail architectures, threat models, and defense-in-depth patterns that red-teaming validates. Together they form a continuous loop: red-teaming finds gaps → guardrails close them → red-teaming verifies the fix.

## References

- [PyRIT: Python Risk Identification Tool](https://github.com/microsoft/PyRIT) — Microsoft's open-source red-teaming framework
- [Garak: LLM vulnerability scanner](https://github.com/NVIDIA/garak)
- [Foundry AI Red Teaming Agent](https://learn.microsoft.com/en-us/azure/foundry/concepts/ai-red-teaming-agent) — Managed cloud red-teaming built on PyRIT
- [Palo Alto Unit 42: MCP Attack Vectors](https://unit42.paloaltonetworks.com/model-context-protocol-attack-vectors/) — Three critical MCP attack classes (Dec 2025)
- [OWASP Cheat Sheet: Securely Using Third-Party MCP Servers](https://genai.owasp.org/resource/cheatsheet-a-practical-guide-for-securely-using-third-party-mcp-servers-1-0/)
- [Prompt Injection Attacks on Agentic Coding Assistants (SoK, arXiv 2026)](https://arxiv.org/html/2601.17548v1) — Meta-analysis of 78 studies; >85% attack success against SOTA defenses
- [Lessons from Red Teaming 100 Generative AI Products](https://www.microsoft.com/en-us/security/blog/2025/01/13/3-takeaways-from-red-teaming-100-generative-ai-products/) — Microsoft Security Blog
- [OWASP Top 10 for Agentic Applications (2026)](https://genai.owasp.org/resource/owasp-top-10-for-agentic-applications-for-2026/)
