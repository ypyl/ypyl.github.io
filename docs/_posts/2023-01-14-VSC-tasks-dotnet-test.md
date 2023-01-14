---
layout: post
title: "Visual Studio Code tasks. Run dotnet test with xUnit, coverlet and reportgenerator."
date: 2023-01-14
tags: dotnet, test, xUnit, vcs, coverlet, reportgenerator
categories: dotnet, programming
---

There is a way to run one task that will run:
1. all tests
2. run code coverege tool
3. generate html report for code coverage

There is a `tasks.json` configuration that using PowerShell to run some utility tasks (like deleting old tests):

```json
{
    "version": "2.0.0",
    "tasks": [
        {
            "label": "run-tests",
            "command": "dotnet",
            "hide": true,
            "type": "process",
            "args": [
                "test",
                "--collect:\"XPlat Code Coverage\""
            ],
            "options": {
                "cwd": "${workspaceFolder}/MyProject.Tests/"
            },
            "problemMatcher": "$msCompile",
            "dependsOn": [ "delete-tests" ]
        },
        {
            "label": "generate-report",
            "command": "reportgenerator",
            "hide": true,
            "type": "process",
            "args": [
                "-reports:\"${workspaceFolder}\\MyProject.Tests\\TestResults\\coverage.cobertura.xml\"",
                "-targetdir:\"coveragereport\"",
                "-reporttypes:Html"
            ],
            "options": {
                "cwd": "${workspaceFolder}/MyProject.Tests/"
            },
            "problemMatcher": [],
            "dependsOn" : [ "extract-tests" ]
        },
        {
            "label": "test",
            "type": "shell",
            "command": "start msedge ${workspaceFolder}/MyProject.Tests\\coveragereport\\index.html",
            "problemMatcher": [],
            "dependsOn": [ "generate-report" ]
        },
        {
            "label": "delete-tests",
            "type": "shell",
            "hide": true,
            "command": "If (Test-Path ${workspaceFolder}\\MyProject.Tests\\TestResults) { Remove-Item -Path ${workspaceFolder}\\MyProject.Tests\\TestResults -Force -Recurse }",
            "problemMatcher": []
        },
        {
            "label": "extract-tests",
            "type": "shell",
            "hide": true,
            "command": "Get-ChildItem -Path ${workspaceFolder}\\MyProject.Tests\\TestResults -Recurse -File | Move-Item -Destination ${workspaceFolder}\\MyProject.Tests\\TestResults",
            "problemMatcher": [],
            "dependsOn": [ "run-tests" ]
        }
    ]
}
```
