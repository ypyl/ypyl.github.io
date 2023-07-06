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

- https://learn.microsoft.com/en-us/dotnet/csharp/roslyn-sdk/source-generators-overview
- https://github.com/dotnet/roslyn/blob/main/docs/features/source-generators.cookbook.md
- https://devblogs.microsoft.com/dotnet/new-c-source-generator-samples/

## Source generator reload - dotnet build-server shutdown; dotnet clean; dotnet run

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
