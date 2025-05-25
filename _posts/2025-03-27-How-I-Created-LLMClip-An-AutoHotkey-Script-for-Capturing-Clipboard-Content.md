---
layout: post
title: How I Created LLMClip – An AutoHotkey Script for Capturing Clipboard Content
date: 2025-03-27

tags: autohotkey clipboard automation llm productivity
categories: programming automation
---

## Introduction

LLMClip is an AutoHotkey (AHK) script I developed to streamline the process of gathering clipboard data and passing it as context to a Large Language Model (LLM) like ChatGPT. The goal was to seamlessly capture text, file paths, and folder paths while preserving their structure and content, making it easier to work with LLMs for code generation, debugging, and research.

I built this tool to solve a specific problem: when copying multiple pieces of information from different sources, it becomes cumbersome to manually paste them into ChatGPT in a structured format. LLMClip automates this process, ensuring a smooth workflow.

## Key Features

- **Start and stop clipboard recording** with a simple toggle.
- **Capture and store** plain text, file paths, and folder paths.
- **Automatically detect clipboard content from Visual Studio Code** and extract file URIs.
- **Show copied items** in a list and allow deletion of individual items.
- **Pass the structured data** as a single text value to an LLM.

## Why AutoHotkey?

I chose AutoHotkey for its lightweight nature and powerful automation capabilities. It allowed me to easily hook into clipboard events, manage a GUI for controlling recordings, and create an intuitive tray menu.

---

## How I Built LLMClip

### Step 1: Setting Up the Clipboard Monitoring

AutoHotkey provides an `OnClipboardChange` function, which I used to detect clipboard updates. It is checking context from clipboard and also check if it comes from Visual Studio Code.

```ahk
OnClipboardChange ClipChanged

ClipChanged(DataType) {
    ...
}
```

### Step 2: Handling Text, File Paths and Folders

If the copied item is a file or folder path, the script needs to process it differently.

```ahk
...
if (DirExist(item)) {
    recordedText .= "`n======`n" ProcessFolder(item)
} else if (FileExist(item)) {
    recordedText .= "`n======`n" ProcessFile(item)
} else {
    recordedText .= "`n======`n" item
}
...


ProcessFolder(FolderPath) {
    folderText := FolderPath
    Loop Files, FolderPath "\*.*", "R"  ; Recursively loop through all files
    {
        folderText .= "`n======`n" ProcessFile(A_LoopFileFullPath)
    }
    return folderText
}

ProcessFile(FilePath) {
    try {
        content := FileRead(FilePath)
        return FilePath "`n------`n" content
    } catch {
        return FilePath "`n------`n[Error reading file content]"
    }
}
```

This allows LLMClip to extract relevant text from files while preserving their paths.

### Step 3: Creating the Tray Menu

For easy control, I implemented a system tray menu with options to start/stop recording and show recorded text.

```ahk
A_TrayMenu.Add("Start Recording", StartRecording)
A_TrayMenu.Add("Stop Recording", StopRecording)
A_TrayMenu.Add("Show Copied Text", ShowCopiedText)
A_TrayMenu.Add("Exit", ExitApp)
```

The script also changes the tray icon to indicate whether recording is active, improving usability.

### Step 4: Passing Data to the LLM

Once the user stops recording, the script concatenates all recorded clipboard content into a structured format and copies it to the clipboard, ready to be pasted into ChatGPT.

```ahk
StopRecording(*) {
    global isRecording, txtFromClipboardArray
    isRecording := false
    recordedText := "`n".Join(txtFromClipboardArray)
    A_Clipboard := recordedText  ; Copy to clipboard
}
```

---

## Results and Use Cases

Since implementing LLMClip, my workflow has become much more efficient. I can copy code snippets, log outputs, file paths, and text from various sources without worrying about manually structuring them before sending them to ChatGPT.

Some use cases where LLMClip has been particularly useful:

- **Debugging**: Quickly collecting error messages, stack traces, and related files.
- **Research**: Copying multiple texts from different articles.
- **Code Assistance**: Gathering various snippets for better context when requesting code fixes.

---

## Source Code

[llmclip](https://github.com/ypyl/llmclip/tree/main)
