---
layout: post
title: "Nancy sample application"
date: 2017-02-12

tags: dotnet web
categories: programming
---
Hi there, 

Just trying to create a simple service running on server. This service should report his status by HTTP and should have a nice log mechanism. So I finished my investigation to use [NetCore](https://www.microsoft.com/net/core) + [Nancy](http://nancyfx.org/) + [Serilog](https://serilog.net/) + [Newtonsoft.Json](http://www.newtonsoft.com/json) + [FakeItEasy](https://github.com/FakeItEasy/FakeItEasy) + [App Metrics](https://github.com/alhardy/AppMetrics).

Please find the source code of sample application [here](https://github.com/eapyl/nancy-netcore-sample).

Want to mention that I had to install the next lib:

```bat
sudo apt-get install libunwind8
```

to actually run my service on remote server - Ubuntu 16.04 x64. There was the next error: `Failed to load libcoreclr.so libunwind.so.8 cannot open shared object file”` without this library.

Also project.json is set up to build self-contained application:

```bat
dotnet publish -c Release -r ubuntu.16.04-x64 -o packages/ubuntu
```

Thanks.