---
layout: post
title: Creating Blog using Blazor
date: 2023-06-29
categories: programming
tags: VSCode blazor dotnet SourceGenerator
---

# Introduction

Creating a blog using [Blazor](https://dotnet.microsoft.com/en-us/apps/aspnet/web-apps/blazor) is an interesting exercise to learn and explore the possibilities of Blazor. It's also fun to discover a new way to create a frontend app using [C#](https://learn.microsoft.com/en-us/dotnet/csharp/), [Razor](https://learn.microsoft.com/en-us/aspnet/core/mvc/views/razor?view=aspnetcore-7.0), [Wasm](https://webassembly.org/), [MudBlazor](https://www.mudblazor.com/) and [DotNet](https://dotnet.microsoft.com/en-us/).

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

Debugging source generators can be challenging, but there is a trick that can help:

1. Add the following code at the beginning of the `Execute` method:

```csharp
while (!System.Diagnostics.Debugger.IsAttached)
    System.Threading.Thread.Sleep(500);
```

2. Ensure that your `launch.json` file has a configuration to attach to the running dotnet process. Add the following configuration:

```json
{
    "name": ".NET Core Attach",
    "type": "coreclr",
    "request": "attach"
},
```

3. Rebuild the entire project by running `dotnet build-server shutdown; dotnet clean; dotnet run` for your Blog web app. This command will pause at the building phase, allowing you time to attach to the building process. The process that needs to be attached to is typically `dotnet.exe`, which executes (`exec`) the VBCSCompiler.dll. For example, it could be something like `dotnet.exe exec C:\Program Files\dotnet\sdk\7.0.203\Roslyn\bincore\VBCSCompiler.dll`.

The solution was found on [StackOverflow](https://stackoverflow.com/questions/67227370/c-sharp-source-generators-debug-in-vscode).

## Using SourceGenerator to create Html from MD files

The `ArticleGenerator`` class contains the logic to convert Markdown files to their HTML representation. When the main project is built, a dictionary is created with article names as keys and HTML content and metadata as values. The [Markdig](https://github.com/xoofx/markdig) library is utilized to perform the conversion from Markdown to HTML.

## Using TargetPathWithTargetPlatformMoniker

Since Markdig is used in the source generator project, it is necessary to inform the main project during the build process that Markdig is required. This can be accomplished by following a similar approach as shown in this [sample](https://github.com/dotnet/roslyn-sdk/blob/main/samples/CSharp/SourceGenerators/SourceGeneratorSamples/CSharpSourceGeneratorSamples.csproj#L27).

To achieve this, the NuGet reference needs to be marked with `GeneratePathProperty="true"`, and the generated names, such as `PKGMarkdig`, `PKGMarkdown_ColorCode`, `PKGColorCode_Core`, and `PKGColorCode_Html`, should be used in the `TargetPathWithTargetPlatformMoniker` property. Note that the names should have a PKG prefix, and any dots should be replaced with underscores.

## Referencing SourceGenerator from Blazor app

To reference a source generator project in the project that will use it, you need to add manually additional attributes to the reference definition. These attributes include `ReferenceOutputAssembly` and `OutputItemType`. Additionally, the `IncludeAssets` attribute should have a value `all`.

```xml
<ProjectReference Include="..\Generators\Generators.csproj" ReferenceOutputAssembly="false" OutputItemType="Analyzer">
    <IncludeAssets>all</IncludeAssets>
</ProjectReference>
```

By including these attributes and setting `IncludeAssets` to `all`, you ensure that the source generator project is properly referenced and that its assets are available for use in the consuming project.

## Use AdditionalFiles to provide access to blogs from SourceGenerator

As Markdown files are converted to HTML during the compilation of the main Blazor project, it is necessary to reference them in the main project so that the source generator project can find them. To achieve this, you can use the special construct called [AdditionalFiles](https://github.com/dotnet/roslyn/blob/main/docs/analyzers/Using%20Additional%20Files.md).

Here's how you can include the Markdown files as AdditionalFiles in the project file (XML format):

```xml
<AdditionalFiles Include="..\Blog\**\*.md" />
```

By adding this line to the project file, you are telling the compiler to consider all Markdown files (*.md) located in the ..\Blog directory and its subdirectories as AdditionalFiles. These files will then be accessible to the source generator project during the build process.

# Blazor app

## Using MudBlazor

The decision has been made to use [MudBlazor](https://www.mudblazor.com/), a Blazor Component Library that offers a wide range of components and is very user-friendly. With MudBlazor, there is no need to invest significant time in creating raw HTML and CSS code. Instead, developers can leverage the well-designed and convenient components provided by the library, making the development process much more efficient and enjoyable.

### Adding styles to MudText title

To add custom styling to a MudBlazor component, you can use the `Style` property:

```cs
<MudText Typo="Typo.h5" Class="flex-grow-1" Style="white-space: nowrap; text-overflow: ellipsis; overflow: hidden;">@articleService.ArticleName</MudText>
```

In this example, the `Style` property is used to apply custom CSS styling to the `MudText` component. The specified CSS styles will make sure that the content of the `MudText` component does not wrap, and any overflow is hidden with an ellipsis. The `Class` property is also used here to apply additional CSS classes to the component for more styling options.

## Copy assets and images to output folder

To copy images and assets from the Blog folder to the output folder, you can use the following commands in the project file:

```xml
<ItemGroup>
    <ContentWithTargetPath Include="..\Blog\images\*.*">
        <Link>%(RecursiveDir)%(Filename)%(Extension)</Link>
        <CopyToOutputDirectory>PreserveNewest</CopyToOutputDirectory>
        <TargetPath>wwwroot/images/%(RecursiveDir)%(Filename)%(Extension)</TargetPath>
    </ContentWithTargetPath>
    <ContentWithTargetPath Include="..\Blog\assets\*.*">
        <Link>%(RecursiveDir)%(Filename)%(Extension)</Link>
        <CopyToOutputDirectory>PreserveNewest</CopyToOutputDirectory>
        <TargetPath>wwwroot/assets/%(RecursiveDir)%(Filename)%(Extension)</TargetPath>
    </ContentWithTargetPath>
</ItemGroup>
```

In this code snippet, the `ItemGroup` element is used to specify the content that needs to be copied. The `ContentWithTargetPath` item includes the source path for the images and assets in the Blog folder. The `Link` element specifies the target path for the copied files. The `CopyToOutputDirectory` element with the value `PreserveNewest` ensures that only newer files are copied. The `TargetPath` element sets the output path for the copied files within the `wwwroot/images` and `wwwroot/assets` folders, respectively.

## Use Katex in blog posts

To support Katex for displaying formulas in the blog app, you need to make modifications to the `index.html` file and the Blazor component.

- Include the Katex script in the head of the `index.html` file:

```html
<!-- The loading of KaTeX is deferred to speed up page rendering -->
<script defer src="https://cdn.jsdelivr.net/npm/katex@0.16.7/dist/katex.min.js" integrity="sha384-G0zcxDFp5LWZtDuRMnBkk3EphCK1lhEf4UEyEM693ka574TZGwo4IWwS6QLzM/2t" crossorigin="anonymous"></script>
```

- Add the following script to the end of the `index.html` file:

```js
window.callKatex = function() {
    var tex =  document.querySelectorAll('.math');
    for (const el of tex) {
        const txt = el.textContent.trim().replace(/^\\\(/, '').replace(/\\\)$/, '').replace(/^\\\[/, '').replace(/\\\]$/, '');
        const displayMode = txt.indexOf('begin') > -1;
        katex.render(txt, el, { displayMode: displayMode });
    }
};
```

- Call the `callKatex` function in the `OnAfterRenderAsync` method of the Blazor component:

```cs
protected override async Task OnAfterRenderAsync(bool firstRender)
{
    await JSRuntime.InvokeVoidAsync("callKatex");
    articleService.ArticleName = SelectedArticleName;
}
```

By following these steps, you will be able to support Katex and render formulas in your Blazor app.

## Using recurssion to render folder structure

The Blog app has a navigation panel on the left side that contains a list of articles. The articles are organized using folders, and this organization is represented as a hierarchy in the navigation.

`FolderComponent` is a Blazor component that renders the hierarchy by calling itself to render child folders:

```cs
@foreach (var folder in FolderValue.Folders)
{
    <FolderComponent FolderValue="@folder" SearchTerm="@SearchTerm"></FolderComponent>
}
```

In this code snippet, the `FolderComponent` iterates through the child folders of the current folder (FolderV`alue). For each child folder, it recursively calls itself (`FolderComponent`) to render the sub-folders and articles present in the hierarchy. The `SearchTerm` parameter is also passed to the child components, enabling search functionality within the folders and articles.

## Update blog title in a layout based on article

To notify the `MainLayout` component about changes in the article name, a service is used with an event that the `MainLayout` component subscribes to. Here's the corrected code:

In the service class:

```cs
public event Action OnArticleChange = () => {};
public string ArticleName
{
    get { return _articleName; }
    set {
        if (_articleName != value)
        {
            if (Blog.Articles.Value()[value].Item1.TryGetValue("title", out var title))
            {
                _articleName = title;
            }
            else
            {
                _articleName = value;
            }
            NotifyArticleNameChanged();
        }
    }
}

private void NotifyArticleNameChanged() => OnArticleChange?.Invoke();
```

In the `MainLayout` component:

```cs
protected override void OnInitialized()
{
    articleService.OnArticleChange += StateHasChanged;
}

public void Dispose()
{
    articleService.OnArticleChange -= StateHasChanged;
}
```

In the `Article` component:

```cs
protected override async Task OnAfterRenderAsync(bool firstRender)
{
    await JSRuntime.InvokeVoidAsync("callKatex");
    articleService.ArticleName = SelectedArticleName;
}
```

With these changes, when the `SelectedArticleName` changes in the `Article` component, it will trigger a change in the `MainLayout` component, causing it to re-render itself and reflect the updated article name.

## Using MarkupString to render blog html

As the HTML representation of the article is created during the building of the app, it is required to render the raw HTML in the `Article` component. To achieve that, you can use `MarkupString`:

```cs
@(new MarkupString(articleService.Content(SelectedArticleName)))
```

In this code snippet, `MarkupString` is used to render the HTML content obtained from the `articleService.Content(SelectedArticleName)` method. The `MarkupString` class allows you to render raw HTML as markup in a Blazor component. By wrapping the `articleService.Content(SelectedArticleName)` with `MarkupString`, the raw HTML will be displayed properly in the `Article` component.

# GitHub actions to deploy blog to GitHub pages

As it is a Blazor WebAssembly app, you can host it on [GitHub Pages](https://pages.github.com/). To achieve this, you need to build and publish the application using [GitHub Actions](https://github.com/features/actions). Below is the corrected YAML configuration for the GitHub Actions workflow:

```yaml
name: Deploy to GitHub Pages

# Run workflow on every push to the master branch
on:
  push:
    branches: [ master ]

jobs:
  deploy-to-github-pages:
    # use ubuntu-latest image to run steps on
    runs-on: ubuntu-latest
    steps:
    # uses GitHub's checkout action to checkout code form the master branch
    - uses: actions/checkout@v2

    - name: Setup .NET SDK
      uses: actions/setup-dotnet@v3
      with:
        dotnet-version: 7.0.x

    - name: Clear NuGet package cache
      run: dotnet nuget locals all --clear

    - name: Restore NuGet packages
      run: dotnet restore

    - name: Build
      run: dotnet build

    # publishes Blazor project to the release-folder
    - name: Publish .NET Core Project
      run: dotnet publish BlazorBlog/BlazorBlog.csproj -c Release -o release --nologo

    # copy index.html to 404.html to serve the same file when a file is not found
    - name: copy index.html to 404.html
      run: cp release/wwwroot/index.html release/wwwroot/404.html

    # add .nojekyll file to tell GitHub pages to not treat this as a Jekyll project. (Allow files and folders starting with an underscore)
    - name: Add .nojekyll file
      run: touch release/wwwroot/.nojekyll

    - name: Commit wwwroot to GitHub Pages
      uses: JamesIves/github-pages-deploy-action@v4
      with:
        branch: gh-pages
        folder: release/wwwroot
```

With this YAML configuration, the GitHub Actions workflow will trigger on pushes to the `main` branch. It will build the Blazor project, publish it to the `release` folder, and deploy the contents of the `release/wwwroot` directory to the `gh-pages` branch. This will host your Blazor WebAssembly app on GitHub Pages.

# Summary

In this article, I share my experience creating a blog application using Blazor WebAssembly, exploring its versatile capabilities with C#, Razor, WebAssembly, and .NET. Throughout the development process, I encountered and addressed various challenges, documenting the solutions for readers to learn from. Utilizing MudBlazor's component library expedited the frontend development, while source generators optimized file generation during compilation. I also navigated the integration of KaTeX for displaying formulas and managed Markdown files and assets seamlessly. Ultimately, the blog application was successfully hosted on GitHub Pages with GitHub Actions. This article serves as a concise guide for building a Blazor WebAssembly blog and offers insights into overcoming common hurdles, inspiring developers to explore Blazor's potential for dynamic web applications.

Source code is available by [link](https://github.com/ypyl/ypyl.github.io).
