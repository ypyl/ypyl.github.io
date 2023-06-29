---
layout: post
title: Install docker on raspberry PI
date: 2018-01-20

tags: docker
categories: administration
---
Need to run the next command:
```bat
curl -sSL https://get.docker.com | sh
```
After installation need to execute `sudo usermod -aG docker $USER` and relogin.
