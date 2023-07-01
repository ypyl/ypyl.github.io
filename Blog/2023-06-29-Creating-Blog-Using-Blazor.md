---
layout: post
title: Creating Blog using Blazor
date: 2023-06-29
categories: programming
tags: VSCode blazor dotnet SourceGenerator
---

# Introduction

Creating a blog using [Blazor](https://dotnet.microsoft.com/en-us/apps/aspnet/web-apps/blazor)  an interesting exercise to learn and explore the possibilities of Blazor. It's also fun to discover a new way to create a frontend app using [C#](https://learn.microsoft.com/en-us/dotnet/csharp/), [Razor](https://learn.microsoft.com/en-us/aspnet/core/mvc/views/razor?view=aspnetcore-7.0), [Wasm](https://webassembly.org/) and [DotNet](https://dotnet.microsoft.com/en-us/).

[Blazor Wasm](https://learn.microsoft.com/en-us/aspnet/core/blazor/?view=aspnetcore-7.0#blazor-webassembly) has been chosen as the framework allows hosting the blog on [GitHub pages](https://pages.github.com/). Since Blazor is the latest feature of ASP.NET for building interactive web UIs, it provides a valuable opportunity to try and learn this cutting-edge technology.

This article is not a step-by-step guide on how to build the website/blog. You can explore the code and its history in [the source code repository](https://github.com/ypyl/ypyl.github.io). Instead, the article focuses on the issues and interesting scenarios encountered during the app's development process.

# Create projects and solution

# SourceGenerator

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
