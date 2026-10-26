---
layout: post
title: "Agent-to-UI in ASP.NET Core — Let the Model Write the Widget, Then Sandbox It"
date: 2026-10-26
categories: programming
tags: [ai-agents, dotnet, aspnet, llm, sandboxing]
---

**widget-chat** is a small ASP.NET Core app where the model does not fill in a component: it **writes the widget**. A reply carries prose plus CSS, markup and JavaScript, and the browser applies each part as it completes, inside an iframe with an opaque origin. It exists to answer a narrow question: how does CopilotKit's **OpenIntelligentUI** get a rotatable Three.js scene into a chat bubble, and what does that take in **ASP.NET Core** with **Microsoft Agent Framework** and almost no code. Two decisions carry the whole design, the **wire protocol** and the **isolation**, and both are places where the obvious version fails quietly.

The wire format is plain text, not a tool call. The reply is prose, then one block in a fixed order:

```
Two sentences of prose.
@@HEIGHT:420@@
@@CSS@@   ...widget styles...
@@HTML@@  ...widget markup...
@@JS@@    ...widget behaviour...
@@END@@
```

The order is load-bearing. CSS arrives first so nothing flashes unstyled, markup morphs in as it streams, and behaviour runs once at the end so the user watches the last piece take effect. On the server that is a `ChatClientAgent` plus a loop, streamed as newline-delimited JSON:

```csharp
http.Response.ContentType = "application/x-ndjson; charset=utf-8";
http.Features.Get<IHttpResponseBodyFeature>()?.DisableBuffering();

await foreach (var update in agent.RunStreamingAsync(message, lease.Session, cancellationToken: ct))
{
    if (string.IsNullOrEmpty(update.Text)) continue;
    foreach (var chunk in parser.Push(update.Text))
        await writer.WriteChunkAsync(chunk, ct);   // {"type":"html","delta":"<div class=\"stage\""}
}
```

The catch: `RunStreamingAsync` streaming is not enough. Without `DisableBuffering()` Kestrel (or the reverse proxy in front of it) holds the response until the run finishes, and the client gets the entire widget at once. The app disables buffering and flushes after every line. That one call is what makes a 40-second widget generation look live instead of frozen.

Another gotcha: a token stream cuts markers anywhere, so `@@HTML@@` can arrive as `@@HT` then `ML@@`. `WidgetStreamParser` keeps a buffer and, when the tail could still become a marker, holds that tail back instead of emitting it:

```csharp
if (partial >= 0)                  // text[partial..] might be the start of "@@HTML@@"
{
    if (partial > 0) Emit(output, text[..partial]);
    _buffer.Remove(0, partial);    // keep the candidate, send the rest now
    return;
}
```

The tests feed a sample fixture in at every possible split index (`SplittingAnywhereYieldsTheSameResult`), because this bug only appears intermittently in production otherwise. Two smaller parser rules earn their keep: the height marker must close at the newline, or a forgotten `@@` swallows the next marker and leaks its name into prose; and a leading markdown fence the model was told not to write gets stripped from each channel.

For isolation, the entire boundary is one attribute. `sandbox="allow-scripts"` without `allow-same-origin` gives the document an opaque origin: no cookies, no storage, no same-origin fetch, and the parent cannot even read `contentWindow.document`. A CSP inside the frame narrows it further to one CDN:

```html
<iframe sandbox="allow-scripts" referrerpolicy="no-referrer" srcdoc="...bootstrap document..."></iframe>
```

```
default-src 'none';
script-src 'unsafe-inline' 'unsafe-eval' https://esm.sh;
img-src data: blob:;  font-src data:;  connect-src https://esm.sh;
```

Two constraints shape the client. **`srcdoc` re-navigates the frame**, so per-chunk updates cannot go through it: the frame loads one bootstrap document, and the host pushes `css`, `html` and `js` operations over `postMessage`. Updates go through a positional DOM **morph** rather than `innerHTML =`, so the canvas, focus and scroll position survive each chunk. And because the frame's origin is opaque, `'self'` matches nothing, so the host cannot load a runtime script into it. The runtime travels as text and is serialised with `toString()`:

```js
var STAGE_RUNTIME =
  "window.__morphDom = " + morphDom.toString() + ";" +
  "window.__createStage = " + createStage.toString() + ";" +
  "window.stage = { mount: window.__mountStage };";
```

That is the subtle trap: **every injected function must be self-contained**. A helper that closes over a variable in `widget-host.js` compiles fine on the host, is stringified without the closure, and fails only inside the frame at runtime. The consequence is that `'unsafe-eval'` is unavoidable, and the frame can run any script it is given. Isolation here means "cannot reach your page, storage or APIs", not "cannot run code". For the 3D case the injected `window.stage` owns the renderer, camera, lights, pixel ratio, resize observer and the single animation loop, so generated code never writes that boilerplate: `const stage = window.stage.mount({ THREE, OrbitControls, canvas })` then `stage.add(...)`, `stage.onFrame(...)`, `stage.onDispose(...)`.

The host-page CSP is the other non-obvious trap. An `srcdoc` frame **inherits its parent's policy**, so a `script-src` on the host page also applies inside the frame and blocks both the runtime and the importmap CDN. The host page here has no CSP; it never inserts generated HTML, so it does not need one. The bridge back to the chat (`host.sendPrompt`, `host.openLink`) is not a security boundary either, since code in the frame can post the same messages. The host validates every argument and enforces the real limit, five widget-started prompts per page load.

So what differs from just streaming AG-UI at a component registry? With AG-UI the frontend already owns the components; the stream carries state and events, and a `BarChart({ data })` renders. That holds until someone asks for a diagram of a hip joint, and every new shape becomes a component, a schema, a renderer and a deploy. Here the payload is the UI itself, which is why there is no schema validation and the prompt plus parser absorb that cost. Tool calling as the transport was tried first: a `generateSandboxedUi(css, html, js)` call means reassembling partial JSON for no ordering guarantee a text stream does not already give. A second experiment, a reference-widget tool that handed the model a working example, cost **3.7× the input tokens**, a second reasoning round, and roughly one empty reply in three, so it was deleted and grounding moved into the prompt.

The price is real: 35 to 65 seconds and about 12k tokens per widget, plus a browser that keeps only 8 to 16 live WebGL contexts. The app pauses animation through an `IntersectionObserver` and releases off-screen widgets after twelve are alive, keeping their downloaded standalone HTML working. Typed components still win wherever the shape is known. Generated UI wins where the variety is the point.

[widget-chat (GitHub)](https://github.com/ypyl/widget-chat) · [OpenIntelligentUI](https://github.com/CopilotKit/OpenIntelligentUI) · [Microsoft Agent Framework](https://learn.microsoft.com/en-us/agent-framework/) · [MDN: iframe sandbox](https://developer.mozilla.org/en-US/docs/Web/HTML/Reference/Elements/iframe)
