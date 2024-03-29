---
layout: post
title: Add dotnet command to alias of bash
date: 2016-09-19
tags: linux dotnet
categories: programming
---
### Dotnet commands
I am using sample application created by `dotnet run`.

To run dotnet core application on Ubuntu with **compilation**:

```docker
dotnet run -p {pathToFolderWithProjectJson} -- {arguments}
```

To run compiled application:

```bat
dotnet {pathToCompiledDll} {arguments}
```
where `pathToCompiledDll` is a path to dll (it is in `/bin/Debug/netcoreapp1.1/` be default).

### Bash commands

To add dotnet alias (shortcut) to bash set of commands:

```bat
nano ~/.bash_aliases
```

And you need to add the next line to this file (bash_aliases):

```bat
alias helloWorld="dotnet run -p ~/projects/helloWorld -- ~/Peter"
```
where
* ~/projects/helloWorld is path to the project;
* ~/Peter is an argument passed to the application;

Now you are able to run `helloWorld` in bash terminal after restarting it. You should see *Hello World*.

Links:
* [How do I create shortcut commands in the Ubuntu terminal?](http://stackoverflow.com/questions/5658781/how-do-i-create-shortcut-commands-in-the-ubuntu-terminal)
* [dotnet run](https://docs.microsoft.com/en-us/dotnet/articles/core/tools/dotnet-run)
* [How to create a permanent “alias”?](http://askubuntu.com/questions/1414/how-to-create-a-permanent-alias#5278)
