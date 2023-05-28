---
layout: post
title: "TFS Exclude binding from solution"
date: 2013-04-24

tags: tfs
categories: programming
---
It is not easy command to unbind solution/project of TFS.

But there is manual actions to do that:
1. Remove all *.vssscc and etc(Source Control files near your solution and projects file);
2. Remove all nodes in solution and project file with tag <scc />