---
layout: post
title: Semantic Kernel learning
date: 2023-12-27
categories: programming
tags: ai semantic kernel openai
---

# [How to write better prompts for OpenAI Codex](https://platform.openai.com/docs/guides/prompt-engineering/six-strategies-for-getting-better-results)

- This page provides tips and tricks for writing effective prompts for OpenAI Codex, a system that can generate code from natural language.
- The page explains how to use the following strategies: providing clear instructions, using examples, specifying input and output formats, using comments, and handling errors.
- The page also includes a list of resources, such as the OpenAI Cookbook, the Codex Playground, and the Codex API documentation, to help users get started with Codex.


# [Semantic Kernel 1.0: Function Calling](https://www.developerscantina.com/p/semantic-kernel-function-calling/)

- This page explains how Semantic Kernel 1.0 supports function calling, a feature that allows an LLM to orchestrate complex AI workflows by calling external functions.
- The page shows how to use function calling in two ways: manually, where the developer is in charge of calling the functions and passing the results to the LLM, and automatically, where Semantic Kernel handles everything for the developer.
- The page also compares function calling with planners, another feature of Semantic Kernel that enables AI orchestration, and provides code samples and a GitHub repository to demonstrate the concepts.

# [General Guidelines: Azure Core](https://azure.github.io/azure-sdk/general_azurecore.html)

- This page defines the requirements for the Azure Core library, which provides cross-cutting services to other client libraries.
- The page covers topics such as HTTP pipeline, authentication, logging, configuration, distributed tracing, and proxy support.
- The page also specifies the common environment variables, global configuration keys, and token credential types that are used by the Azure SDK.

# [Creating native functions for AI to call](https://learn.microsoft.com/en-us/semantic-kernel/agents/plugins/using-the-kernelfunction-decorator?tabs=Csharp)

- This page explains how to create native functions that can perform tasks that large language models cannot do easily on their own, such as arithmetic.
- It shows how to create a MathPlugin class that contains various math functions, and how to use the KernelFunction decorator to register them with the kernel.
- It also demonstrates how to import and run the native functions using the kernel, and how to enable the AI to automatically call them within the chat loop.

# [OpenAI API Documentation](https://platform.openai.com/docs/api-reference/chat/create)

- The OpenAI API provides access to various models and features, such as text generation, image generation, fine-tuning, and moderation.
- The API documentation explains how to use the endpoints, parameters, and objects for each feature, and provides examples and code snippets.
- The API documentation also includes guides, tutorials, and resources to help developers get started and make the most of the OpenAI platform.

# [MathSolver.cs](https://github.com/MicrosoftDocs/semantic-kernel-docs/blob/main/samples/dotnet/11-Planner/plugins/MathSolver.cs)

- A class that implements the `IPlugin` interface and provides a method to solve mathematical expressions using the `MathNet.Symbolics` library
- A constructor that takes a `SemanticKernel` instance and registers the `SolveAsync` function as a prompt handler for the `math` keyword
- A `SolveAsync` function that takes a `PromptContext` instance and returns a `PromptResult` instance with the evaluated expression or an error message

# [ConsoleChat.cs at main · microsoft/semantic-kernel-starters](https://github.com/microsoft/semantic-kernel-starters/blob/main/sk-csharp-console-chat/ConsoleChat.cs)

- This is a C# source code file that defines the **ConsoleChat** class, which implements the **IChat** interface for console-based chat applications.
- The **ConsoleChat** class has a constructor that takes a **ChatGPTPlugin** object as a parameter, and three methods: **StartAsync**, **StopAsync**, and **ExecuteAsync**.
- The **StartAsync** method initializes the chat session and prints a welcome message, the **StopAsync** method terminates the chat session and prints a goodbye message, and the **ExecuteAsync** method handles the user input and output using the **ChatGPTPlugin** object.

# [Introducing the v1.0.0 Beta1 for the .NET Semantic Kernel SDK](https://devblogs.microsoft.com/semantic-kernel/introducing-the-v1-0-0-beta1-for-the-net-semantic-kernel-sdk/)

- This page announces the release of Semantic Kernel SDK v1.0.0-beta1, which has several breaking changes from previous versions.
- The page provides a detailed migration guide for developers using the .NET version of Semantic Kernel, covering topics such as package renaming, class renaming, function invocation, and AI service configuration.
- The page also includes some examples and tests that have been updated to use the new SDK, as well as an image of the Chat Copilot Release 0.5 that integrates Semantic Memory.

# [Release Candidate 1 for the Semantic Kernel .NET SDK is now live.](https://devblogs.microsoft.com/semantic-kernel/release-candidate-1-for-the-semantic-kernel-net-sdk-is-now-live/)

- This page introduces the new features and improvements of Semantic Kernel v1.0.0 RC1, a .NET SDK for building AI applications with OpenAI and Hugging Face models.
- The page highlights how Semantic Kernel simplifies function calling with OpenAI, allows streaming responses from the kernel, and supports prompt YAML files and Handlebars templates.
- The page also provides code examples and links to starter apps for Semantic Kernel, and invites developers to join the hackathon and give feedback on the SDK.

# [Semantic Kernel’s Ignite release: Beta8 for the .NET SDK](https://devblogs.microsoft.com/semantic-kernel/semantic-kernels-ignite-release-beta8-for-the-net-sdk/#start-developing-openai-assistants-with-semantic-kernel)

- This page announces the new gen-4 and gen-5 planners developed by the Semantic Kernel team, which can handle more functions and logic with fewer tokens.
- The page also explains how to migrate from the previous planners (Action, Sequential, and V1 Stepwise) to the new ones (Handlebars and Function Calling Stepwise) using a migration guide.
- The page also provides some updates on the Semantic Kernel .NET SDK, the v1.0.0 release, and the AI hackathon.

Here is a memo for this page:

# [Say hello to Semantic Kernel V1.0.1](https://devblogs.microsoft.com/semantic-kernel/semantic-kernel-v1-0-1-has-arrived-to-help-you-build-agents/)

- This page announces the release of Semantic Kernel V1.0.1 for C#, a .NET SDK for building AI agents for AI powered applications[^1^][1].
- The page provides information on how to get started with Semantic Kernel, such as updated documentation, a step-by-step guide, and a Discord channel for help.
- The page also lists the stable, preview, and alpha NuGet packages for Semantic Kernel, and invites the community to contribute to the project by building AI connectors for various models.

# [Automatically orchestrate AI with planners](https://learn.microsoft.com/en-us/semantic-kernel/agents/planners/?tabs=Csharp)

- A planner is a function that takes a user's ask and returns back a plan on how to accomplish the request using AI and plugins
- Planners can automatically recombine functions to create workflows for complex scenarios, such as solving math problems or creating reminders.
- Planners have advantages and disadvantages, such as performance, cost, and flexibility, and can be optimized by pre-creating plans for common scenarios.

# [MathPlugin.cs](https://github.com/MicrosoftDocs/semantic-kernel-docs/blob/main/samples/dotnet/08-Creating-Functions-For-AI/plugins/MathPlugin.cs)

- This page is a code file named MathPlugin.cs that contains a class called MathPlugin with various mathematical functions.
- The functions include basic arithmetic operations, trigonometric functions, logarithmic functions, and rounding functions.
- The code file is part of a sample project called 08-Creating-Functions-For-AI, which demonstrates how to create custom functions for AI services using Semantic Kernel.

# [KernelPromptTemplate.cs](https://github.com/microsoft/semantic-kernel/blob/main/dotnet/src/SemanticKernel.Core/PromptTemplate/KernelPromptTemplate.cs)

- A class that defines a template for generating prompts for semantic kernels
- It uses a template engine to render the prompt based on the input variables and the output blocks
- It also provides methods to extract the blocks from the prompt and to add missing input variables

# [NuGet Gallery | Packages matching Microsoft.SemanticKernel](https://www.nuget.org/packages?page=2&q=Microsoft.SemanticKerne&sortBy=relevance)

- This page shows 61 packages related to Microsoft Semantic Kernel, a framework for building AI applications with semantic memory and reasoning capabilities.
- The packages include connectors, plugins, planners, functions, and template engines for various AI services and data sources, such as OpenAI, Weaviate, DuckDB, Kusto, Milvus, Qdrant, MongoDB, SQLite, OpenAPI, YAML, Markdown, and Handlebars.
- The page also provides information about the downloads, updates, and descriptions of each package, as well as links to contact, FAQ, status, and privacy statement of NuGet.org.

---

# Prompt to generate such summary for the page:

Please create a memo for this page that will be used later to find this page.
The first must be the title and the link to this page. After there should be 3 sentences formated as a list.
Do not add any cross-links, references or sources links in the output.
Format result as markdown code.

There is an example of markdown code:
```md
# [PAGE_TITLE](LINK_TO_THE_PAGE)

- Sentence summary one
- Sentence summary two
- Sentence  summary three
```
