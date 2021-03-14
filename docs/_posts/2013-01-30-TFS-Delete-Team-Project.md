---
layout: post
title: "TFS Delete Team Project"
date: 2013-01-30

tags: tfs
categories: programming
---
It is not so easy to find the steps to delete project from TFS.

So, according to [http://stackoverflow.com/questions/13635889/delete-team-project-from-free-team-foundation-service](http://stackoverflow.com/questions/13635889/delete-team-project-from-free-team-foundation-service).

You can use the following command from the "Developer Command Prompt":

```none
TfsDeleteProject /collection:https://mytfs.visualstudio.com/DefaultCollection MyProject
```

Thank you