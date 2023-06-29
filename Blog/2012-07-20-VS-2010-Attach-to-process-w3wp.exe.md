---
layout: post
title: VS 2010 Attach to process w3wp.exe
date: 2012-07-20

tags: vs
categories: programming
---
Need to create the macros:

```vb
Public Module AttachToProcess
Public Function AttachToProcess(ByVal ProcessName As String) As Boolean
Dim proc As EnvDTE.Process
Dim attached As Boolean
For Each proc In DTE.Debugger.LocalProcesses
If (Right(proc.Name, Len(ProcessName)) = ProcessName) Then
proc.Attach()
attached = True
End If
Next

Return attached
End Function

Sub AttachToW3WP()
If Not AttachToProcess("w3wp.exe") Then
System.Windows.Forms.MessageBox.Show("Cannot attach to process")
End If
End Sub
End Module
```

[Link](http://habrahabr.ru/post/131937/)
