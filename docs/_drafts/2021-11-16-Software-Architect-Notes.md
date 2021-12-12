---
layout: post
title: "Software Architect Notes"
date: 2010-10-16
draft: true
tags: software architect
categories: programming
---

# Software Architecture

## What is software architecture

Software architecture is the foundation for the software system.

It consists of different aspects:

- structures and elements of a system
- relationships between system's elements or components
- abstractions of a system

Benefits to having clean and explicit architecture:

- enabling and inhibiting quality attributes
- predicting system quality
- easing communication between all members/consumers of a system (stakeholders, owners, developers, and so on)
- allowing to more easily make changes
- providing a reusable model for other products
- imposing implementation constraints
- allowing to do cost and effort estimation
- simplifying training for new developers

Software architect main responsibilities are:

- technical decisions
- architecture of a system
- documentation
- following and improving software architecture approaches and principles within the organization

## Software architects in an organization

Software architect means different in different companies. Some companies already have software development methodologies and it is required to follow them. So each software architect should be familiar with them. If it is allowed to select the software development methodology, software architect should be able to explain his choice.

Software architect has the following responsibilities within an organization:

- tasks estimation
- projects planning
- controlling and monitoring
- communication with stakeholders and management
- risk management
- coordination of builds, development, and production environments
- supporting and introducing product lines (reusable architecture components)
- business domain understanding
- identifying and clarifying requirements to a software system

## Domain

Understanding the domain is a key part of being software architect. It is required to have general business knowledge and a good understanding of the organization's business. So it will be possible to design software architecture that solves business problems effectively.

DDD is a proven approach to modeling the domain:

- creating ubiquitous language to simplify and improve communication between everyone
- identify entities, value objects, aggregates, and root entities
- separating domain into subdomains and creating bounded contexts

Moreover, proper requirements engineering is an important part of creating a software architecture. Usually, it is not easy to get and elicit all requirements from stakeholders, but knowing requirements is crucial to designing a good solution.

## Quality attributes

The software must match the designed quality attributes, so software architect should properly identify and specify them so they can be tested and measured.

There are two main types of quality attributes: functional and non-functional.

Some important non-functional attributes:

- maintainability
- usability
- availability
- portability
- interoperability
- testability
- security
- performance

## Design

Software architecture design is an important part of the creation of software architectures. A good design can be validated, formally documented, and used by a development team.

There are two (three) main approaches:

- top-down
- bottom-up
- mix of the first two approaches

Architectural drivers guide the software architecture design and are inputs for the process of designing:

- design objectives (why the software is being designed?)
- primary functional requirements
- quality attributes scenarios (measurable properties of a software system)
- constraints (properties that must be satisfied)
- architectural concerns (interests of the software architect)

The following instruments can be used to design software architecture:

- patterns
- reference architectures (a template for an architecture)
- tactics (proven techniques to achieve required quality attribute)
- externally developed software

Documentation is another important phase of designing software architecture.

Architecture design is an iterative process:

- architectural analysis
- architectural synthesis
- architectural evaluation (and go back to the first step if required)

There are a set of architecture design processes that can be used:

- attribute-driven design (ADD)
- microsoft's technique for architecture and design
- architecture-centric design method (ACDM)
- architecture development method (ADM)

Tracking the progress of the designing process is important and can be combined with the creation of the documentation.

## Principles and practices

There are some principles and practices that will help to design orthogonal system which is extendable without breaking the existing functionality:

- loose coupling
- high cohesion

To reduce complexity of the system the following principles can be used and followed:

- KISS
- DRY
- information hiding
- YAGNI
- SoC (Separation of Concerns)
- SOLID (SRP, OCP, LSP, ISP, DIP)

To improve quality of the system the following practices can be used:

- unit testing
- pair programming
- reviewing artifacts

Also, software architecture patterns can be used to solve the common recurring problems.

## Software architecture patterns

[Software architecture patterns](https://en.wikipedia.org/wiki/Architectural_pattern) provide solutions for common and recurring problems in software designing process. They give the high level structure of the system that can be understandable by other developers as such patterns are commonly used.

There are a lot of patterns, but the most commonly used:

- [layered architecture](https://en.wikipedia.org/wiki/Multitier_architecture)
- [event-driven architecture](https://en.wikipedia.org/wiki/Event-driven_architecture)
- [model-view-controller pattern (MVC)](https://en.wikipedia.org/wiki/Model%E2%80%93view%E2%80%93controller)
- [model-view-presenter pattern (MVP)](https://en.wikipedia.org/wiki/Model%E2%80%93view%E2%80%93presenter)
- [model-view-viewmodel pattern (MVVM)](https://en.wikipedia.org/wiki/Model%E2%80%93view%E2%80%93viewmodel)
- [command query responsibility segregation pattern](https://docs.microsoft.com/en-us/azure/architecture/patterns/cqrs) (CQRS)
- [service-oriented architecture (SOA)](https://docs.microsoft.com/en-us/dotnet/architecture/microservices/architect-microservice-container-applications/service-oriented-architecture)

There are a set of quite 'new' architectures and principles which are worked well for applications deployed to the cloud:

- [monolithic architecture](https://docs.microsoft.com/en-us/dotnet/architecture/containerized-lifecycle/design-develop-containerized-apps/monolithic-applications)
- [microservice](https://en.wikipedia.org/wiki/Microservices) architecture
- [serverless architecture](https://docs.microsoft.com/en-us/dotnet/architecture/serverless/serverless-architecture) (FaaS, BaaS)
- [cloud-native applications](https://azure.microsoft.com/en-us/overview/cloudnative/) ([twelve-factor app methodology](https://12factor.net/))

## Cross-cutting concerns

[Cross-cutting concerns](https://en.wikipedia.org/wiki/Cross-cutting_concern) represent functionality which are used by multiple part/layers of the application (e.g. security, logging, caching, error handling, configuration, monitoring). So logic of such concerns are called by different components.

The following methods are used to implement and use the logic of cross-cutting concerns:

- [dependency injection](https://en.wikipedia.org/wiki/Dependency_injection)
- [decorator pattern](https://en.wikipedia.org/wiki/Decorator_pattern)
- [aspect-oriented programming](https://en.wikipedia.org/wiki/Aspect-oriented_programming)
- [sidecar pattern](https://docs.microsoft.com/en-us/azure/architecture/patterns/sidecar) for microservices
- ready-to-use solutions like [Darp](https://dapr.io/) for mircoservices

## Performance

Before working on [performance](https://en.wikipedia.org/wiki/Performance_engineering) improvement and optimizations, need to make sure that the code is correct. But the software system should be enough fast as it affects user experience.

Performance is one of the quality attributes and it should be documented. It also should be testable and measurable. Usually improving performance is done using an iterative way: monitoring, profiling, analyzing, implementing.

There are a set of terms related to performance:

- [latency](https://en.wikipedia.org/wiki/Latency_%28engineering%29)
- [throughput](https://en.wikipedia.org/wiki/Throughput)
- [bandwidth](https://en.wikipedia.org/wiki/Bandwidth_(computing))
- processing time
- [response time](https://en.wikipedia.org/wiki/Response_time_(technology))
- [workload](https://en.wikipedia.org/wiki/Workload)
- utilization

There are a couple of standard mechanics and ways to improve performance:

- [server-side caching](https://docs.microsoft.com/en-us/azure/architecture/best-practices/caching)
- [HTTP caching](https://developer.mozilla.org/en-US/docs/Web/HTTP/Caching)
- [compression](https://developer.mozilla.org/en-US/docs/Web/HTTP/Compression)
- [minifying and bundling](https://docs.microsoft.com/en-us/aspnet/mvc/overview/performance/bundling-and-minification)
- [HTTP/2](https://developers.google.com/web/fundamentals/performance/http2/)
- [CDN](https://en.wikipedia.org/wiki/Content_delivery_network)
- [tweaking database performance](https://en.wikipedia.org/wiki/Database_tuning)

## Security

[Security](https://en.wikipedia.org/wiki/Computer_security) is important part of any software application. There is [CIA triad](https://en.wikipedia.org/wiki/Information_security) which summarize the attributes that the system can have:

- confidentiality
- integrity
- availability

Software architect should try to keep a balance between these attributes.

There is [threat modeling](https://en.wikipedia.org/wiki/Threat_model) which is structured approach to analyzing security for an application (STRIDE threat model, [DREAD risk assessment model](https://en.wikipedia.org/wiki/DREAD_(risk_assessment_model))). Also there are techniques to create an application that are secure by design:

- minimize [attack surface](https://en.wikipedia.org/wiki/Attack_surface)
- [defence in depth](https://en.wikipedia.org/wiki/Defense_in_depth_(computing))
- [principle of least privilege](https://en.wikipedia.org/wiki/Principle_of_least_privilege)
- [avoiding security by obscurity](https://en.wikipedia.org/wiki/Security_through_obscurity)
- [keep software designs simple](https://en.wikipedia.org/wiki/KISS_principle)
- [security by default](https://en.wikipedia.org/wiki/Secure_by_default)
- [default deny](https://en.wikipedia.org/wiki/Computer_access_control)
- [validating input](https://en.wikipedia.org/wiki/Data_validation)
- secure the weakest link
- security must be usable
- [fail securely](https://en.wikipedia.org/wiki/Fail-safe)

[Cryptography](https://en.wikipedia.org/wiki/Cryptography) is the study which allows to keep information in secret. [IAM (Identity and access management)](https://en.wikipedia.org/wiki/Identity_management) allows to centralize information related to the application security.

[Most common web application risks](https://owasp.org/Top10/):
- [injection](https://owasp.org/Top10/A03_2021-Injection/)
- [broken authentication](https://owasp.org/Top10/A07_2021-Identification_and_Authentication_Failures/)
- sensitive data exposure
- XML external entity (XXE) attack
- [broken access control](https://owasp.org/Top10/A01_2021-Broken_Access_Control/)
- [security misconfiguration](https://owasp.org/Top10/A05_2021-Security_Misconfiguration/)
- [cross-site scripting (XSS)](https://cheatsheetseries.owasp.org/cheatsheets/Cross_Site_Scripting_Prevention_Cheat_Sheet.html)
- [insecure deserialization](https://cheatsheetseries.owasp.org/cheatsheets/Deserialization_Cheat_Sheet.html)
- [using components with known vulnerable components](https://owasp.org/Top10/A06_2021-Vulnerable_and_Outdated_Components/)
- [insufficient logging and monitoring](https://owasp.org/Top10/A09_2021-Security_Logging_and_Monitoring_Failures/)
- [unvalidated redirects and forwards](https://cheatsheetseries.owasp.org/cheatsheets/Unvalidated_Redirects_and_Forwards_Cheat_Sheet.html)

## Documenting and reviewing

Documenting is an important part of delivering architecture. It allows to educate, train other team members, share architectural knowledge and provide entry points for review.

There are a set of techniques to describe an architecture in an understandable way, e.g using UML and architectural view models (like [4 + 1](https://en.wikipedia.org/wiki/4%2B1_architectural_view_model)). Some additional information can be found [here](https://ypyl.github.io/documentation/2019/08/01/New-Project.html#architecture).

Reviewing architecture methods:

- [software architecture analysis method (SAAM)](https://en.wikipedia.org/wiki/Software_architecture_analysis_method)
- [architecture tradeoff analysis method (ATAM)](https://en.wikipedia.org/wiki/Architecture_tradeoff_analysis_method)
- active design review (ADR)
- [active reviews of intermediate designs (ARID)](https://en.wikipedia.org/wiki/ARID)


## DevOps

[DevOps](https://en.wikipedia.org/wiki/DevOps) allows to build, integrate, deploy and deliver the application in fast, automatic and incremental way. It allows to follow 'fail fast' principle during releasing a new version of the application and roll back to the previous version quickly in case of the failed released version.

- [Continuous integration](https://en.wikipedia.org/wiki/Continuous_integration), with automatic builds and tests, allows to validate changes quickly.
- [Continuous delivery](https://en.wikipedia.org/wiki/Continuous_delivery) allows to deliver artifacts that can be deployed to the production at any time automatically.
- [Continuous deploy](https://en.wikipedia.org/wiki/Continuous_deployment) allows to push new versions of the application to the production in automatic way.

Needs of DevOps should be considered during the building of software application design. The following [quality attributes](https://en.wikipedia.org/wiki/DevOps#Architecturally_significant_requirements) are important for DevOps:

- testability
- deployability
- maintainability

## [Cloud models](https://en.wikipedia.org/wiki/Cloud_computing#Service_models)

- Infrastructure as a Service (IaaS)
- Containers as a Service (CaaS)
- Platform as a Service (PaaS)
- Serverless/Function as a Service
- Software as a Service (SaaS)

Notes have been created after reading [Software Architect's Handbook](https://www.amazon.com/Software-Architects-Handbook-implementing-architecture/dp/1788624068)
[]
