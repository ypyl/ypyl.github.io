---
layout: post
title: 'Smart TV (Tizen) Link opener'
date: 2021-07-16
categories: documentation
---

# Introduction

Simple Tizen Smart TV Xamarin Form .NET application to receive http links via HTTP PUT, open them into a browser.

## Requirements

# Install Tizen .Net

Download and install [Tizen SDK](https://download.tizen.org/sdk/Installer/Latest/)

# Visual Studio Code

Check and install [extension](https://docs.tizen.org/application/vscode-ext/dotnet/)

# Developer mode on Smart TV

Open Apps and enter '12345'. Enable developer mode and enter the ip address of your developer machine (that will be used to deliver installation package to TV).

## Application

# Template

`dotnet tizen new Tizen.NET.Template55.Cross.NETStandard -v tizen-5.5` to create Xamarin.Forms application. Remove all projects which are not connected to TV.

# Add smart TV device

Run the following command in `C:\tizen-studio\tools`:

```
.\sdb.exe connect 192.168.1.4
```

where `192.168.1.4` is IP of smart TV device

# Test

Check that `C:\tizen-studio\tools\ide\bin` is in Path of environment settings.

Build the project by `tizen build-cs` and run `tizen install -n <path-to-tpk>` where `<path-to-tpk>` is showed during the building.

The installation of an application should be started.

# Link opener

Add permission to `tizen-manifest.xml` to allow launching external application.

```
  <privileges>
    <privilege>http://tizen.org/privilege/appmanager.launch</privilege>
  </privileges>
```

The code to open a link into a browser

```cs
Tizen.Applications.AppControl.SendLaunchRequest(new Tizen.Applications.AppControl
{
    Operation = Tizen.Applications.AppControlOperations.View,
    Uri = link,
    LaunchMode = Tizen.Applications.AppControlLaunchMode.Single
});
```

# Summary

[Repository link](https://github.com/ypyl/smart-tv-link-opener)
