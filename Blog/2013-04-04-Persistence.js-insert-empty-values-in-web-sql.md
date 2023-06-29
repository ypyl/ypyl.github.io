---
layout: post
title: Persistence.js insert empty values in web sql
date: 2013-04-04

tags: javascript
categories: programming
---
I am starting to work with persistence.js library and open the problem to me: it saves empty data to web sql

![example1](./images/persistance1.png)

After investigating the problem i have found this question  and just want to clear and reproduce it in my notes. Thanks guys from this question :)
The problem connected with this js file: persistence.jquery. If it used, we should rewrite code such as:

![example2](./images/persistance2.png)

Thanks.
