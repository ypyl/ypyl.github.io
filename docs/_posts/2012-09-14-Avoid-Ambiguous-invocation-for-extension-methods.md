---
layout: post
title: "Avoid Ambiguous invocation for extension methods"
date: 2012-09-14
tags: dotnet asp net mvc
categories: programming
---

I m using ASP.NET MVC. And work with my view, where I have the View

![example](/images/invocation.png)

It gives me the exception

```text
The call is ambiguous between the following methods or properties: 'GMP.MvcWebSite.StringExtensions.TrimOrEmpty(string)' and 'System.StringHelper.TrimOrEmpty(string)'
```

So i just rebuild my view as

![example2](/images/invocation2.png)
