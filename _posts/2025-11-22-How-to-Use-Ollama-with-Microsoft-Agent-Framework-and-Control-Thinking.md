---
layout: post
title: How to Use Ollama with Microsoft Agent Framework and Control Thinking
date: 2025-11-22
tags: microsoft-agent-framework chatbot ai-agents llm dotnet how-to
categories: programming genai agent framework
---

This post demonstrates how to integrate Ollama with the [Microsoft Agent Framework](https://github.com/microsoft/agent-framework) to create a simple AI agent and how to control the agent's "thinking" process, which can be useful for performance tuning or simplifying interactions.

### Setting up the Agent with Ollama

First, you need to have Ollama running. Then, you can use the `OllamaApiClient` from the `OllamaSharp` library to connect to your local Ollama instance and specify the model you want to use.

The following C# code snippet shows how to create an agent that uses a local Ollama model and how to run it with the thinking process disabled.

```csharp
using Microsoft.Agents.AI;
using Microsoft.Extensions.AI;
using OllamaSharp;

var endpoint = "http://localhost:11434";
var modelName = "qwen3:8b";

AIAgent agent = new OllamaApiClient(
    new Uri(endpoint),
    modelName)
    .CreateAIAgent(instructions: "You are good at telling jokes.", name: "Joker");

var options = new ChatClientAgentRunOptions
{
    ChatOptions = new() { AdditionalProperties = new() { ["think"] = false } },
};

// Invoke the agent and output the text result.
var result = await agent.RunAsync(message: "Tell me a joke about a pirate.", options: options);

Console.WriteLine(result);
```

### Controlling the "Think" Process

One of the interesting features is the ability to control whether the agent goes through a "thinking" step before producing a response - [more info](https://docs.ollama.com/capabilities/thinking#enable-thinking-in-api-calls). This is done via the `ChatClientAgentRunOptions`.

By creating an `ChatClientAgentRunOptions` object and adding a property `["think"] = "false"` to its `ChatOptions.AdditionalProperties`, you can instruct the agent to provide a direct response without the intermediate thinking step.

```csharp
var options = new ChatClientAgentRunOptions
{
    ChatOptions = new() { AdditionalProperties = new() { ["think"] = false } },
};
```

This can be useful in scenarios where you need a quicker, more direct response and want to minimize latency. When the "think" process is enabled (which is the default behavior), the agent might perform additional reasoning steps, which can lead to more elaborate or well-structured answers but also adds overhead.

### Running the Agent

Finally, you invoke the agent using `RunAsync` and pass the message and the `options` object.

```csharp
var result = await agent.RunAsync(message: "Tell me a joke about a pirate.", options: options);
Console.WriteLine(result);
```

> **Note:** `var options = new ChatClientAgentRunOptions { AdditionalProperties = new AdditionalPropertiesDictionary { ["think"] = false } };` is not working currently as you might expect - [more info](https://github.com/microsoft/agent-framework/issues/2399)

> **Note:** `qwen3:4b` ignore think parameter
