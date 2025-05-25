---
layout: post
title: Simple question to check understanding of multithreading
date: 2017-03-05

tags: dotnet interview
categories: programming
---
How to do planning of two threads so none of them can leave a circle (X is global value and default value is 0)?

```cs
while (X == 0)
{
    X = 1 - X;
}
```
