---
layout: post
title: "Simple question to check understanding of recursion in .NET"
date: 2016-12-19

tags: dotnet interview
categories: programming
---
There is the next code:

```cs
public static void Main(string[] args) {
    Console.WriteLine(Test());
}

public static int Test(int index = 0) {
    if (index > 10) return 0;
    try {
        for (var i=0; i< 10; i++) {
            index++;
        }
    }
    catch {
        Console.Write("Error");
    }
    for (var j = 1; j < 10; j++)
        index += Test(index);
    return index;
}

```

Is it working? What is the output?