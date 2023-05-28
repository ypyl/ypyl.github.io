---
layout: post
title: "MVC indefinitely loads the page and call the controller cyclical"
date: 2012-01-31

tags: dotnet web
categories: programming
---
I have found that the page load indefinitely. And the reason was I add @Html.RenderAction to my default layout.

So the solution of the problem to add the next code to rendered view:

```css
@{
      Layout = null;
}
```
 
As I understand, MVC try to load the next without this code:

Layout -> MyView -> Layout -> MyView -> ....

With code above:

Layout -> MyView { -> null}

So it's happens.