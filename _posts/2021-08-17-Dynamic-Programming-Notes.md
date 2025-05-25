---
layout: post
title: Dynamic programming
date: 2021-08-17
categories: dynamic programming
---
# Dynamic programming

There are two main strategies to apply:

* memorization
* tabulation

## Memorization

All type of dynamic programs should be able to solve using recursion. Memorization technic just allows to make the simple straight solution more efficient by saving and reusing already calculated values on previous step of recursion. It means memorization dictionary should be added to the solution.

## Tabulation

The main idea is to have a table with solutions for each step of the problem starting from the obvious ones. So it is required to fill the table with initial results (e.g. fibonacci values for 1 and 2), and later reuse these results by calculating more complex high-level sub-tasks.

[More information](https://www.freecodecamp.org/news/learn-dynamic-programing-to-solve-coding-challenges/)
