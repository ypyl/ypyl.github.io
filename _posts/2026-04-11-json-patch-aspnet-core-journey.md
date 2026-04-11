---
layout: post
title: "JSON Patch in ASP.NET Core: A Journey Through Three Approaches"
date: 2026-04-11
tags: asp-net-core, json, json-patch, dotnet
categories: programming
---

This post documents the process of creating a JSON Patch demo application in ASP.NET Core 10, exploring different approaches, their limitations, and the eventual solution.

**Goal:** Create a minimal API that applies JSON Patch operations to dynamically loaded JSON data (no predefined C# types) loaded from a "database".

## Project Setup

Environment:
- .NET 10.0 SDK
- ASP.NET Core Web API
- Minimal API (single Program.cs)

Initial project file:

```xml
<Project Sdk="Microsoft.NET.Sdk.Web">
  <PropertyGroup>
    <TargetFramework>net10.0</TargetFramework>
    <Nullable>enable</Nullable>
    <ImplicitUsings>enable</ImplicitUsings>
  </PropertyGroup>
  <ItemGroup>
    <PackageReference Include="Microsoft.AspNetCore.Mvc.NewtonsoftJson" Version="10.0.0-*" />
  </ItemGroup>
</Project>
```

## Approach 1: System.Text.Json (Failed)

### Documentation Claim

The Microsoft documentation for .NET 10 states:
> *"JSON Patch support in ASP.NET Core web API is based on System.Text.Json serialization"*

It suggested using `Microsoft.AspNetCore.JsonPatch.SystemTextJson` package.

### Attempted Code

```csharp
using Microsoft.AspNetCore.JsonPatch.SystemTextJson;
using System.Text.Json;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddControllers();

var app = builder.Build();

var jsonData = """
{
    "firstName": "John",
    "lastName": "Doe",
    "email": "johndoe@gmail.com"
}
""";

var jsonPatch = """
[
    { "op": "replace", "path": "/firstName", "value": "Jane" }
]
""";

app.MapGet("/patch", () =>
{
    dynamic data = JsonSerializer.Deserialize<ExpandoObject>(jsonData)!;
    var patchDoc = JsonSerializer.Deserialize<JsonPatchDocument<ExpandoObject>>(jsonPatch)!;
    patchDoc.ApplyTo(data);
    return Results.Ok(data);
});

app.Run();
```

### Error

```
Build FAILED:
The type or namespace name 'ExpandoObject' could not be found
```

After fixing imports, another issue emerged:

```
'JsonPatchDocument<>' has no applicable method named 'ApplyTo' but appears to have an extension method by that name.
Extension methods cannot be dynamically dispatched.
```

### Root Cause

**System.Text.Json implementation does NOT support dynamic types.**

From Microsoft documentation:
> *"The implementation of JsonPatchDocument<TModel> based on System.Text.Json serialization isn't a drop-in replacement for the legacy Newtonsoft.Json-based implementation. It doesn't support dynamic types, for example ExpandoObject."*

### Verdict

**System.Text.Json approach does NOT work for schema-less JSON.**

## Approach 2: Newtonsoft.Json with ExpandoObject (Issues)

### Package Required

```xml
<PackageReference Include="Microsoft.AspNetCore.Mvc.NewtonsoftJson" Version="10.0.0-*" />
```

### Attempted Code

```csharp
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddControllers()
    .AddNewtonsoftJson();

var app = builder.Build();

app.MapGet("/patch", () =>
{
    dynamic data = JsonConvert.DeserializeObject<ExpandoObject>(jsonData, new ExpandoObjectConverter())!;
    var patchDoc = JsonConvert.DeserializeObject<JsonPatchDocument>(jsonPatch)!;
    patchDoc.ApplyTo(data);
    return Results.Ok(data);
});
```

### Problem

The `ExpandoObjectConverter` doesn't properly handle nested complex objects in arrays.

**Input JSON:**
```json
{
    "phoneNumbers": [
        { "number": "123-456-7890", "type": "Mobile" }
    ]
}
```

**After Patch:**
```json
{
    "phoneNumbers": [
        { "number": [], "type": [] }
    ]
}
```

### Root Cause

`ExpandoObjectConverter` serializes nested objects as empty property bags.

### Verdict

**Newtonsoft.Json with ExpandoObject does NOT properly handle nested JSON structures.**

## Approach 3: Newtonsoft.Json with JToken (Success)

### Solution

Use `JToken` (from Newtonsoft.Json.Linq) instead of `ExpandoObject`.

### Working Code

```csharp
using Microsoft.AspNetCore.JsonPatch;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddControllers()
    .AddNewtonsoftJson();

var app = builder.Build();

var jsonData = """
{
    "firstName": "John",
    "lastName": "Doe",
    "email": "johndoe@gmail.com",
    "phoneNumbers": [
        { "number": "123-456-7890", "type": "Mobile" }
    ],
    "address": {
        "street": "123 Main St",
        "city": "Anytown",
        "state": "TX"
    }
}
""";

var jsonPatch = """
[
    { "op": "replace", "path": "/firstName", "value": "Jane" },
    { "op": "remove", "path": "/email" },
    { "op": "add", "path": "/address/zipCode", "value": "90210" },
    { "op": "add", "path": "/phoneNumbers/-", "value": { "number": "987-654-3210", "type": "Work" } }
]
""";

app.MapGet("/patch", () =>
{
    var token = JToken.Parse(jsonData);
    var patchDoc = JsonConvert.DeserializeObject<JsonPatchDocument<JToken>>(jsonPatch)!;
    patchDoc.ApplyTo(token);
    return token.ToString();
});

app.Run();
```

### Result

```json
{
    "firstName": "Jane",
    "lastName": "Doe",
    "phoneNumbers": [
        { "number": "123-456-7890", "type": "Mobile" },
        { "number": "987-654-3210", "type": "Work" }
    ],
    "address": {
        "street": "123 Main St",
        "city": "Anytown",
        "state": "TX",
        "zipCode": "90210"
    }
}
```

### Verdict

**Newtonsoft.Json with JToken WORKS perfectly for schema-less JSON.**

## Error Handling

### Requirement

What if a patch operation targets a non-existent path? We want to:
- Catch/log the error
- Continue applying remaining patch operations
- Return both the patched data and any errors

### Implementation

```csharp
var jsonPatch1 = """
[
    { "op": "replace", "path": "/firstName", "value": "Jane" },
    { "op": "replace", "path": "/email", "value": "jane.doe@example.com" }
]
""";

var jsonPatch2 = """
[
    { "op": "remove", "path": "/email" },
    { "op": "remove", "path": "/nonExistentField" },
    { "op": "add", "path": "/address/zipCode", "value": "90210" },
    { "op": "add", "path": "/phoneNumbers/-", "value": { "number": "987-654-3210", "type": "Work" } }
]
""";

app.MapGet("/patch", () =>
{
    var token = JToken.Parse(jsonData);
    var errors = new List<string>();

    var patchDoc1 = JsonConvert.DeserializeObject<JsonPatchDocument<JToken>>(jsonPatch1)!;
    patchDoc1.ApplyTo(token, error =>
    {
        errors.Add($"Patch 1 Error: {error.ErrorMessage}");
    });

    var patchDoc2 = JsonConvert.DeserializeObject<JsonPatchDocument<JToken>>(jsonPatch2)!;
    patchDoc2.ApplyTo(token, error =>
    {
        errors.Add($"Patch 2 Error: {error.ErrorMessage}");
    });

    var result = new { data = token, errors = errors };
    return Results.Text(JsonConvert.SerializeObject(result), "application/json");
});
```

### Response

```json
{
    "data": {
        "firstName": "Jane",
        "lastName": "Doe",
        "phoneNumbers": [...],
        "address": {...}
    },
    "errors": [
        "Patch 2 Error: The target location specified by path segment 'nonExistentField' was not found."
    ]
}
```

The error was caught, logged, but processing continued!

## Key Findings

### Comparison Table

| Aspect | System.Text.Json | Newtonsoft.Json |
|--------|-----------------|-----------------|
| **Package** | `Microsoft.AspNetCore.JsonPatch.SystemTextJson` | `Microsoft.AspNetCore.Mvc.NewtonsoftJson` |
| **Dynamic Types** | Not Supported | Supported |
| **ExpandoObject** | Fails | Partial (nested objects fail) |
| **JToken/JObject** | Fails | Works |
| **Minimal API Compatible** | Yes | Yes |
| **Error Handling** | Via ModelState | Via error callback |

### Supported JSON Patch Operations

All RFC 6902 operations work with JToken:

| Operation | Description | Works with JToken |
|-----------|-------------|-------------------|
| `add` | Add property or array element | Yes |
| `remove` | Remove property or array element | Yes |
| `replace` | Replace value | Yes |
| `move` | Move value | Yes |
| `copy` | Copy value | Yes |
| `test` | Test value (atomic check) | Yes |

### Important Notes

1. **System.Text.Json in .NET 10 does NOT support dynamic JSON patching** - despite documentation suggesting it as the new approach, it only works with strongly-typed POCOs.

2. **Newtonsoft.Json is required for schema-less JSON** - the legacy package is still necessary for dynamic/ExpandoObject/JToken scenarios.

3. **Error handling is per-patch, not per-operation** - using the error callback allows continuing after errors while collecting all errors.

4. **JToken maintains JSON structure correctly** - unlike ExpandoObject which flattens nested objects.

### Use Cases for This Approach

- Patching JSON stored in databases without predefined schemas
- Applying stored patch operations to dynamic content
- Scenarios where JSON structure is unknown at compile time
- ETL pipelines with JSON transformations

## Conclusion

Creating a JSON Patch demo with dynamic JSON in ASP.NET Core .NET 10 requires Newtonsoft.Json due to System.Text.Json's lack of support for dynamic types. The solution uses `JToken` from Newtonsoft.Json.Linq, which properly handles nested JSON structures and supports all JSON Patch operations.

Error handling allows graceful failure recovery, making it suitable for production scenarios where patch documents may contain invalid operations for certain JSON structures.

## References

- [RFC 6902 - JSON Patch](https://datatracker.ietf.org/doc/html/rfc6902)
- [Microsoft JSON Patch Documentation](https://learn.microsoft.com/en-us/aspnet/core/web-api/jsonpatch)
- [Newtonsoft.Json](https://www.newtonsoft.com/json)
