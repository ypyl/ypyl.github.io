---
layout: post
title: Some bullet points related to security
date: 2024-04-14
categories: programming
tags: secure design architecture
---

# Secure by Design notes

Some notes after reading the book mentioned in the reference section.

**1. Software Security Design and Practices:**
- Security is best approached as a concern to be addressed rather than a checklist of features.
- It's impractical to rely solely on constant vigilance for security during development; instead, seek design practices that inherently lead to secure solutions.
- All decision-making activities in software development should be considered part of the design process, applicable at all levels from code to architecture.
- Traditional software security approaches struggle because they require developers to be security experts and anticipate every possible vulnerability.
- Shifting focus to design allows for achieving high software security without constant explicit consideration of security.
- Strong design emphasis leads to more secure code compared to traditional security approaches.
- XML parsers are inherently vulnerable to entity attacks due to the nature of XML.
- Using generic types for specific data can introduce security vulnerabilities.
- Understanding underlying parser implementations is crucial for configuring XML parsers securely.
- Secure design promotes security in-depth by incorporating multiple layers of security.
- Incomplete, missing, or shallow modeling can result in a design with security flaws.
- Security flaws, such as broken business integrity, can persist in production, causing financial losses for the enterprise.
- Conscious and explicit design leads to a more robust solution.

**2. Domain Modeling:**
- Building domain models promotes deep learning about the domain and serves as a language for system communication.
- A domain model should be strict, unambiguous, and adhere to a ubiquitous language for common understanding among the team, including domain experts.
- Multiple possible models exist when creating a domain model, and it's crucial to choose one that captures only essential aspects.
- Entities, value objects, and aggregates are fundamental components of the domain model, with entities possessing consistent identity throughout their lifecycle.
- Aggregates group model objects, maintain invariants among them, and have an aggregate root, which holds global identity and is typically the same as the aggregate in code.
- Bounded context defines where the model's semantics hold, and changes in semantics indicate a boundary shift, aided by Conway's Law.
- Domain primitives are the foundation of the domain model, immutable, and must be valid within the current domain.
- Using domain primitives simplifies and secures the codebase, hardening APIs and mitigating sensitive data leakage.
- Entities are recommended for handling mutable states, ensuring consistency upon creation, and can be partially immutable.
- Attributes' integrity should be protected during access, with careful design for multithreaded environments.
- The builder pattern is useful for constructing entities with complex constraints, and state handling is improved when extracted to a separate object.
- Careful design is required for multithreaded environments with high capacity, with options like database locking or entity snapshots improving availability.
- Introduce domain primitives at semantic boundaries of context and clarify defensive code constructs using contracts.
- Ensure domain primitives cover entire conceptual wholes and address domain types lacking proper validation with secure entities.

**3. Security Testing and Practices:**
- Protect against Domain DoS attacks by verifying origin IP or requiring access keys, performing data size checks, and using simple regular expressions for lexical content checks.
- Organize validation breakdown into origin checks, data size checks, lexical content checks, syntax format checks, and semantic checks.
- Prioritize validation order, with earlier, more economical checks protecting later, more resource-intensive steps.
- Segment tests into normal testing, boundary testing, invalid input testing, and extreme input testing to incorporate security into unit test suites efficiently.
- Optimize regular expression usage by checking input length before execution to avoid inefficient backtracking.
- Mitigate security vulnerabilities by verifying feature toggle mechanisms with automated tests, testing all possible combinations, and keeping toggles to a minimum to manage combinatory complexity.
- Ensure auditability and record-keeping of toggle mechanisms.
- Integrate automated security tests into build pipelines for regular penetration testing to maintain system security.
- Consider availability as a critical security aspect in system design, simulating DoS attacks to reveal weaknesses.
- Address security issues stemming from misconfiguration through identification of configuration hot spots and testing default tool behavior.
- Implement the fail-fast principle by checking preconditions early in each method to control failures.
- Mitigate security bugs caused by ambiguous parameters in APIs through direct, discovery, or new API approaches.
- Prevent second-order injection attacks by avoiding logging unchecked user input and limiting access to sensitive values.
- Utilize explicit accessor methods for data intended for logging to avoid accidental inclusion of new fields in logs, enhancing security.
- Leverage tests to uncover weaknesses in code, especially through invalid and extreme input tests.

**4. Resilience and Availability:**
- Ensure data integrity by maintaining consistency and accuracy throughout the data lifecycle.
- Guarantee data availability by ensuring obtainability and accessibility at expected performance levels.
- Leverage immutable values, which are safe to share between threads without locking or blocking.
- Benefit from immutability by solving data availability issues through scalability and preventing data integrity issues by disallowing change.
- Utilize contracts to effectively clarify the responsibilities of objects and methods, enhancing resilience and availability.

**5. Microservices Security:**
- Ensure a good microservice possesses an independent runtime for independent updates and is resilient to other services being down.
- Treat each microservice as a bounded context to aid in designing more secure APIs and apply secure design principles like domain primitives and context mapping.
- Mitigate security pitfalls by only exposing domain operations in APIs, using explicit context mapping between services, and paying extra attention to evolving APIs.
- Assess confidentiality, integrity, availability, and traceability (CIA-T) across all services and identify sensitive data that needs to be secured.
- Emphasize the integrity of log data through normalization and categorization, requiring extensive domain knowledge.
- Uniquely identify services by their name, version number, and instance ID, and ensure traceability of transactions across systems.
- Utilize a logger with a domain-oriented API to consider the confidentiality of log data and prevent accidental information leakage by avoiding intermixing sensitive and nonsensitive data in the same log.

Here's the adjusted bullet list:

**6. Secure Software Development Practices:**
- Adopt a design strategy of separating business exceptions from technical exceptions to maintain clarity and consistency.
- Avoid mixing technical and business exceptions using the same type to prevent confusion and maintain code integrity.
- Ensure good design practice by never including business data in technical exceptions to safeguard sensitive information.
- Design software systems to handle failures gracefully, treating them as normal, unexceptional results.
- Prioritize availability as a crucial security goal, enhancing system resilience and responsiveness to improve overall security.
- Implement design patterns like circuit breakers, bulkheads, and timeouts to enhance system availability and mitigate potential failures.
- Minimize risk by avoiding repairing data before validation and never echoing input verbatim to prevent security vulnerabilities.
- Apply twelve-factor app and cloud-native concepts for enhanced application and system security, emphasizing stateless processes and backing services.
- Separate code and configuration to facilitate deployment across multiple environments without rebuilding, while avoiding storing sensitive data in resource files.
- Manage environment-dependent configuration as part of the environment and incorporate administration tasks into the solution.
- Enhance security by avoiding local file logging and opting for centralized logging services.
- Improve security and availability through service discovery, supporting dynamic system changes.
- Implement the three R's (rotate, repave, repair) for improved security, particularly in cloud-based applications.
- Apply the DRY principle to repeated representation of knowledge rather than text, avoiding unnecessary dependencies.
- Conduct code security reviews as a recurring part of secure software development and invest in tooling for quick access to security vulnerability information.
- Establish a proactive strategy for addressing security vulnerabilities in regular development cycles, incorporating pen tests to challenge design and detect issues.
- Utilize feedback generated by pen tests to improve strategies and processes and consider bug bounty programs for continuous pen testing, acknowledging their complexity and resource demands.

**7. Knowledge Acquisition and Application:**
- Initiate deliberate discovery early to gain deep insights into the domain, focusing on specificity initially and abstracting later for clarity.
- Expand expertise by gathering knowledge from adjacent domains, facilitating cross-disciplinary problem-solving.
- Prioritize semantic consistency by refactoring names when semantics change, especially outside bounded contexts, to maintain clarity and cohesion.
- Acquire knowledge in the field of security to effectively address security problems, applying insights from different domains to enhance security measures.
- Recognize the distinction between incident handling and problem resolution, involving the team in incident handling and focusing on learning and resilience to improve security posture.

References: [Secure By Design](https://www.amazon.com/Secure-Design-Daniel-Deogun/dp/1617294357)
