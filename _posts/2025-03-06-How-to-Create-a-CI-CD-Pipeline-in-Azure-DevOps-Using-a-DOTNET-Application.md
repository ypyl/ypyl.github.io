---
layout: post
title: How to Create a CI/CD Pipeline in Azure DevOps Using a .NET Application
date: 2025-03-06

tags: azure-devops dotnet ci-cd pipeline automation
categories: programming devops
---

# How to Create a CI/CD Pipeline in Azure DevOps Using a .NET Application

In this guide, we will walk through setting up a CI/CD pipeline in Azure DevOps using a .NET console application. Leveraging C# and .NET for orchestrating build and deployment processes offers several benefits:

- it allows you to keep all CI/CD logic centralized and changeable in one place
- enables running deployments directly from a local machine
- and ensures consistent deployment logic across all environments
- allows the creation of a self-deployable package.

This approach provides a flexible and programmable method to efficiently manage your CI/CD workflow.

## Prerequisites

- Azure DevOps account
- .NET SDK installed
- Basic understanding of CI/CD concepts

## Step 1: Create the .NET Console Application

First, we need to create a .NET console application that will be responsible for defining and executing build and deployment targets.

```csharp
using Microsoft.Extensions.Configuration; // Import the Microsoft configuration library for handling configuration settings.
using static Bullseye.Targets; // Import Bullseye targets for defining and executing build and deployment tasks.

// Set up the configuration sources for the application.
IConfigurationRoot config = new ConfigurationBuilder()
  .AddEnvironmentVariables() // Include environment variables in the configuration.
  .AddUserSecrets<Program>() // Include user secrets, useful for sensitive data during local exection.
  .AddCommandLine(args) // Include command-line arguments in the configuration.
  .Build(); // Build the configuration to make it ready for use.

// Bind the configuration to the Settings object.
var settings = config.Get<Settings>() ?? throw new InvalidOperationException();

// Define directories for storing artifacts and determining the source directory.
var aritfactFolder = "artifacts";
var sourcesDirectory = Directory.GetCurrentDirectory();
var gitFolder = ".git";
var artifactDirectory = Directory.Exists(Path.Combine(sourcesDirectory, gitFolder))
    ? Path.Combine(sourcesDirectory, aritfactFolder) // We are running tool from repository, local execution or CI.
    : sourcesDirectory; // Otherwise, use the current directory. CD part is running.

Console.WriteLine("The build artifacts are located in the following directory: " + artifactDirectory);

// Set up build and deployment targets using the defined settings and directories.
BuildTargets.SetUp(settings, artifactDirectory, sourcesDirectory); // Configure build targets (CI part)
DeployTargets.SetUp(settings, artifactDirectory); // Configure deployment targets (CD part)

// Run the specified targets and exit the program.
// Allows to run any target(s)
await RunTargetsAndExitAsync(
    settings.BuildArgs.Split(" ", StringSplitOptions.RemoveEmptyEntries), // Split build arguments into tokens.
    ex => ex is SimpleExec.ExitCodeException);
```

## Step 2: Define Build Targets

The build logic is encapsulated within separate classes. Here, we define the build targets that will handle tasks like compiling the application, running tests, and creating deployment packages.

```csharp
// BuildTargets.cs file
using System.IO.Compression;
using static Bullseye.Targets;
using static SimpleExec.Command;

public static class BuildTargets
{
  public static void SetUp(Settings settings, string artifactDirectory, string sourcesDirectory)
  {
    Target("restore",
        () => RunAsync("dotnet", "restore", workingDirectory: sourcesDirectory));

    Target("build",
        () => RunAsync("dotnet", "build --configuration Release", workingDirectory: sourcesDirectory));

    Target("test",
        () => RunAsync("dotnet", "test --configuration Release", workingDirectory: sourcesDirectory));

    Target("publish",
        () => RunAsync("dotnet", $"publish --configuration Release --output {artifactDirectory}", workingDirectory: sourcesDirectory));

    Target("zip",
        () => ZipFile.CreateFromDirectory(artifactDirectory, $"{artifactDirectory}/deploy.zip"));

    Target("default",
        DependsOn("restore", "build", "test", "publish", "zip"));
  }
}
```

## Step 3: Define Deployment Logic

Define deployment targets for deploying the application to the desired environment.

```csharp
// DeployTargets.cs
using static Bullseye.Targets;
using static SimpleExec.Command;

public static class DeployTargets
{
  public static void SetUp(Settings settings, string artifactDirectory)
  {
    Target("deploy",
      async () =>
      {
          var deployPackage = $"{artifactDirectory}/deploy.zip";
          // Add your deployment logic here, e.g., upload to Azure, deploy to Kubernetes, etc.
          Console.WriteLine($"Deploying {deployPackage} to the environment.");
          await Task.CompletedTask; // Replace with actual deployment commands
      });
  }
}
```

## Step 4: Integrate with Azure DevOps - Pipelines

Create a YAML pipeline in Azure DevOps to call the .NET console application commands.

```yml
# CI Pipeline YAML
trigger: none

parameters:
- name: buildArgs
  displayName: buildArgs
  type: string
  default: default

stages:
- stage: 'Build'
  jobs:
  - job: 'Build'
    pool: 'YourAzureDevOpsPool'  # Specify the agent pool for running the job
    steps:
    - script: 'dotnet run'
      displayName: 'Build artifacts'
      workingDirectory: '$(Build.SourcesDirectory)'
      env:
        BUILDARGS: ${{ parameters.buildArgs }}  # Pass build arguments to the environment, e.g. 'default'

    - script: |
        # Publish the .NET application as a self-contained executable for Linux
        # It is used during deployment, so it is not need to install dotnet to do deployment
        dotnet publish -c $(buildConfiguration) -r linux-x64 --self-contained true --output $(Build.SourcesDirectory)/artifacts
      displayName: 'Publish .NET Application'  # Display name for the publish step
      workingDirectory: '$(Build.SourcesDirectory)'  # Set the working directory for the publish step

    # Additional steps can be added here like updating versions, publishing code coverage
```

## Step 4: Integrate with Azure DevOps - Releases

In Azure DevOps, create a new release pipeline by navigating to "Releases". Add the build artifact from your CI pipeline as the source. Define a stage for your deployment environment, such as DEV, and add an task. Configure the task to run a bash script inline, which grants execution permissions and executes the deployment script. Below is the inline script snippet for the task:

```sh
chmod +x ./build
./build BuildArgs=deploy
```

## Conclusion

This pipeline allows you to automate the build and deployment processes, ensuring a streamlined and efficient workflow. The use of powerful libraries like [Bullseye](https://github.com/adamralph/bullseye) for defining targets and workflows, and [SimpleExec](https://github.com/adamralph/simple-exec) for executing commands, provides a robust and flexible infrastructure for managing your CI/CD operations using C# code.

These tools enable you to customize and extend your pipelines easily, adapting to various project needs and environments.
