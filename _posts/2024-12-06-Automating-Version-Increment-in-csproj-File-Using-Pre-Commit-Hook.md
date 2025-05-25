---
layout: post
title: Automating Version Increment in a csproj File Using a Pre-Commit Hook
date: 2024-12-06

tags: csproj versioning git pre-commit bash programming automation tech gpt-4o
categories: programming
---

# Automating Version Increment in a csproj File Using a Pre-Commit Hook

When working on software projects, maintaining an accurate version number in the `csproj` file ensures proper tracking of builds and releases. This guide explains how to automate the process of incrementing the patch version in a `csproj` file using a `pre-commit` hook.

## Overview
The pre-commit hook ensures that every commit updates the version in the `csproj` file automatically. The following steps describe the implementation.

## Step 1: Write the Pre-Commit Hook
The `pre-commit` file ensures the script is executed before each commit. Below is an example:

```bash
#!/bin/bash
./update_version.sh
git add example.csproj
```

This script calls `update_version.sh` to update the version and then stages the modified `example.csproj` file for the commit.

## Step 2: Create the Version Update Script
The `update_version.sh` script parses the `csproj` file, increments the patch version, and updates the version in the file. Below is the script:

```bash
#!/bin/bash
csprojFile="example.csproj"
versionLine=$(grep -oPm1 "(?<=<Version>)[^<]+" "$csprojFile")

if [ -z "$versionLine" ]; then
    echo "Version node not found in the project file" >&2
    exit 1
fi

version="$versionLine"
IFS='.' read -r -a versionParts <<< "$version"
if [ ${#versionParts[@]} -lt 3 ]; then
    echo "Version format is incorrect. Expected format: major.minor.patch" >&2
    exit 1
fi

versionParts[2]=$((versionParts[2] + 1))
newVersion="${versionParts[0]}.${versionParts[1]}.${versionParts[2]}"
sed -i "s/<Version>$version<\/Version>/<Version>$newVersion<\/Version>/" "$csprojFile"
exit 0
```

## Explanation of the Script
1. **Locate the `<Version>` tag:**
   The script extracts the version number using `grep` with a Perl-compatible regular expression (PCRE).
   ```bash
   versionLine=$(grep -oPm1 "(?<=<Version>)[^<]+" "$csprojFile")
   ```
2. **Validate the format:**
   The script ensures the version follows the `major.minor.patch` format and handles errors if the tag is missing or misformatted.
3. **Increment the patch version:**
   The script splits the version string, increments the patch number, and constructs the new version.
4. **Update the file:**
   Using `sed`, the script replaces the old version in the `csproj` file with the new version.

## Step 3: Make Scripts Executable
Ensure both scripts are executable by running:
```bash
chmod +x pre-commit update_version.sh
```

## Step 4: Install the Pre-Commit Hook
Move the `pre-commit` file to the Git hooks directory:
```bash
mv pre-commit .git/hooks/pre-commit
```

## Step 5: Test the Automation
1. Modify some files in your repository.
2. Stage the changes and commit.
   ```bash
   git commit -m "Test version update"
   ```
3. Confirm that the `example.csproj` file's version has been updated.

## Conclusion
Using a `pre-commit` hook to automatically increment the version in your `csproj` file streamlines the process of maintaining consistent versioning. This ensures your build pipeline always operates on updated version numbers without manual intervention.
