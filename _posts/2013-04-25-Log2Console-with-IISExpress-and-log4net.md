---
layout: post
title: Log2Console with IISExpress and log4net
date: 2013-04-25

tags: dotnet
categories: programming
---
Log2Console with IISExpress and log4net
I have found that log4net and log2console don't work correctly (using IISExpress) with each other after trying using default configuration from [http://log2console.codeplex.com/wikipage?title=ClientConfiguration](http://log2console.codeplex.com/wikipage?title=ClientConfiguration.).

But i have found new configuration for log4net and log2console ([log4net](http://logging.apache.org/log4net/release/config-examples.html#udpappender)) and

![example1](/assets/log2net1.png)

Seems work fine:

![example2](/assets/log2net2.png)
