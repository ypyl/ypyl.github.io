---
layout: post
title: How to Resolve SignalR Server-to-Client Sending Cancellation Issue
date: 2024-03-30
categories: programming
tags: SignalR, .NET, Streaming, Server-to-Client Communication, Cancellation Issue
---

## Overview

There exists an issue concerning the cancellation of SignalR server-to-client requests. This problem often arises within Azure Functions due to a particular code structure. Let's delve into the specifics:

```csharp
public async Task<IActionResult> PostMessage([HttpTrigger(AuthorizationLevel.Function, "post", Route = "{userId}/data")] HttpRequestData req, Guid userId,
       CancellationToken cancellationToken)
{
    // some code
    await foreach (var result in _getCompletion.Handle(command, cancellationToken))
    {
        _logger.LogTrace("{method} Received answer from underlying layer: {completinResult.Content}.", nameof(method), result.Content);
        cancellationToken.ThrowIfCancellationRequested();
        // some code
        _logger.LogTrace("{method} Sending result to client.", nameof(method));
        await MessageHubContext.Clients.User(userId.ToString()).SendAsync(NewPartOfResultReceived, result, cancellationToken);
        _logger.LogTrace("{method} Sent answer to client", nameof(method));
    };
    // some code
    return new JsonResult(finalResult);
}
```

The core of the issue lies in SignalR's behavior, wherein even after cancellation of the token (utilizing [AbortSignal.abort()](https://developer.mozilla.org/en-US/docs/Web/API/AbortSignal/abort_static)), SignalR persists in sending data to the client.

## Identifying the Issue

It appears that SignalR lacks built-in support for cancellation in such scenarios. This observation has been documented in a GitHub ticket: [dotnet/aspnetcore#11542](https://github.com/dotnet/aspnetcore/issues/11542).

## Proposed Solution

To tackle this issue, one potential solution involves halting the reception of data for connections associated with an 'aborted' state in the [AbortSignal](https://developer.mozilla.org/en-US/docs/Web/API/AbortSignal). Rather than resolving the problem on the backend, it could be more feasible to address it on the client side.

Here's a TypeScript snippet illustrating how you can check for an aborted signal and prevent further processing if found:

```ts
// some code
// state.operations contains current ongoing operations
const targetOperation = state.operations.find(x => x.id === allData[targetData].operationId);
if (targetOperation === undefined || targetOperation?.controller?.signal.aborted) {
    return;
}
// adding data to the target
```

By implementing a mechanism on the client side that ceases data reception upon detection of an aborted AbortSignal, the issue can be effectively mitigated.
