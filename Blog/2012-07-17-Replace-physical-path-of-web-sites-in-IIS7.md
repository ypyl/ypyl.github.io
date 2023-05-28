---
layout: post
title: "Replace physical path of web sites in IIS7"
date: 2012-07-17

tags: iis
categories: administration
---
```powershell
param([String]$numb)

[Void][Reflection.Assembly]::LoadWithPartialName("Microsoft.Web.Administration")

$siteName = "graph.vrpinc.com"
##$serverIP = "your ip address"
$newPath = "D:\Projects\gmp"+$numb+"\GMP.WebSite"

$serverManager = New-Object Microsoft.Web.Administration.ServerManager
## $serverManager = [Microsoft.Web.Administration.ServerManager]::OpenRemote($serverIP)
$site = $serverManager.Sites | where { $_.Name -eq $siteName }
$rootApp = $site.Applications | where { $_.Path -eq "/" }
$rootVdir = $rootApp.VirtualDirectories | where { $_.Path -eq "/" }
$rootVdir.PhysicalPath = $newPath
$serverManager.CommitChanges()

$siteName = "gmp3.vrpinc.com"
$newPath = "D:\Projects\gmp"+$numb+"\GMP.MvcWebSite"

$serverManager = New-Object Microsoft.Web.Administration.ServerManager
## $serverManager = [Microsoft.Web.Administration.ServerManager]::OpenRemote($serverIP)
$site = $serverManager.Sites | where { $_.Name -eq $siteName }
$rootApp = $site.Applications | where { $_.Path -eq "/" }
$rootVdir = $rootApp.VirtualDirectories | where { $_.Path -eq "/" }
$rootVdir.PhysicalPath = $newPath
$serverManager.CommitChanges()

$siteName = "GMPServices"
$newPath = "D:\Projects\gmp"+$numb+"\GMP.Services"

$serverManager = New-Object Microsoft.Web.Administration.ServerManager
## $serverManager = [Microsoft.Web.Administration.ServerManager]::OpenRemote($serverIP)
$site = $serverManager.Sites | where { $_.Name -eq $siteName }
$rootApp = $site.Applications | where { $_.Path -eq "/" }
$rootVdir = $rootApp.VirtualDirectories | where { $_.Path -eq "/" }
$rootVdir.PhysicalPath = $newPath
$serverManager.CommitChanges()
```

Bat file to call PowerShell file

```bat
@echo off

set /p delBuild=Enter the number of gmp project?
powershell -noprofile Set-ExecutionPolicy Unrestricted
powershell .\setUpSite.ps1 -numb %delBuild%
```

One variable to set current version of projects!
Thanks!