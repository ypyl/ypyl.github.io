---
layout: post
title: Creating Blog using Blazor
date: 2023-06-29
categories: programming
tags: VSCode blazor dotnet SourceGenerator
---

# Introduction

Creating a blog using [Blazor](https://dotnet.microsoft.com/en-us/apps/aspnet/web-apps/blazor)  an interesting exercise to learn and explore the possibilities of Blazor. It's also fun to discover a new way to create a frontend app using [C#](https://learn.microsoft.com/en-us/dotnet/csharp/), [Razor](https://learn.microsoft.com/en-us/aspnet/core/mvc/views/razor?view=aspnetcore-7.0), [Wasm](https://webassembly.org/), [MudBlazor](https://www.mudblazor.com/) and [DotNet](https://dotnet.microsoft.com/en-us/).

[Blazor Wasm](https://learn.microsoft.com/en-us/aspnet/core/blazor/?view=aspnetcore-7.0#blazor-webassembly) has been chosen as the framework allows hosting the blog on [GitHub pages](https://pages.github.com/). Since Blazor is the latest feature of ASP.NET for building interactive web UIs, it provides a valuable opportunity to try and learn this cutting-edge technology.

This article is not a step-by-step guide on how to build the website/blog. You can explore the code and its history in [the source code repository](https://github.com/ypyl/ypyl.github.io). Instead, the article focuses on the issues and interesting scenarios encountered during the app's development process.

# Create projects and solution

## Used version of dotnet and VScode

[Dotnet version 7.0](https://dotnet.microsoft.com/en-us/download/dotnet/7.0) has been used and [VSCode](https://code.visualstudio.com/) has been the chosen development tool for creating the app. However, it's worth noting that there have been some challenges when developing Blazor apps using VSCode. For a smoother experience and full support of all Blazor features, it is recommended to use Visual Studio as the integrated development environment (IDE).

## How to create blazor wasm solution

To create a Blazor app, you can start by running the following command `dotnet new blazorwasm`. This command will generate a new Blazor application.

To create a project that generates HTML code from Markdown, you can use `dotnet new console` command. After creating the console project, you can follow the instructions provided in the [Source Generators article](https://learn.microsoft.com/en-us/dotnet/csharp/roslyn-sdk/source-generators-overview) to implement the necessary functionality.

Both of these projects can be combined in a single solution, allowing you to have a Blazor app alongside the project that generates HTML code from Markdown.

## Why source generator feature as it doesn't generate separate pages for each blog post

In the beginning, the idea was to generate a separate Blazor component page for each Markdown file. However, it was discovered that source generators do not support the generation of non-C# files. As a result, the decision was made to generate a single file that contains a map with the blog post names and their corresponding HTML content.

# SourceGenerator

## How to start with it and examples

Source generators were [introduced in .NET 5](https://devblogs.microsoft.com/dotnet/introducing-c-source-generators/). They are a type of code that runs during compilation and enables the creation of additional files that are compiled alongside the rest of the code. There is useful [cookbook](https://github.com/dotnet/roslyn/blob/main/docs/features/source-generators.cookbook.md) available that can be used to learn more about source generators.

## Source generator reload - dotnet build-server shutdown; dotnet clean; dotnet run

There is an interesting behavior regarding how source generators work. If you make changes to the code in the source generator project and expect those changes to immediately reflect in the project that uses the generated code, you will be disappointed. By simply running `dotnet build`` or `dotnet run``, the generated code in the target project will not be updated. In order to see the changes, you will need to execute the command dotnet build-server shutdown to clear the cache.

I encountered this issue and found a solution [here](https://learn.microsoft.com/en-us/answers/questions/1184090/looking-for-assistance-clearing-the-cache-for-upda).

To provide further context, the way it works is that the C# compiler, `csc.exe`, typically starts a "compilation server" named `VBCSCompiler.exe` to avoid the overhead of starting the process repeatedly. `csc.exe` forwards the parameters to `VBCSCompiler.exe` via interprocess communication (IPC) to perform the compilation. The compiler process, once loaded, will not pick up changes to the source generator DLL because it is loaded dynamically. Shutting down the `VBCSCompiler.exe` process or waiting for the idle timeout is necessary to load the updated DLL.

## Issue with debugging and tracing

## Using SourceGenerator to create Html from Md files

## Using TargetPathWithTargetPlatformMoniker

## Reference SourceGenerator from Blazor app

## Use AdditionalFiles to provide access to blogs from SourceGenerator

# Blazor app

## Using MudBlazor

### Adding styles to MudText title

## Copy assets and images to output folder

## Use Katex in blog posts

## Using recurssion to render folder structure

## Update blog title in a layout based on article

## Using MarkupString to render blog html

# GitHub actions to deplot blog to GitHub pages

# Summary

Source code is available by [link](https://github.com/ypyl/ypyl.github.io).
