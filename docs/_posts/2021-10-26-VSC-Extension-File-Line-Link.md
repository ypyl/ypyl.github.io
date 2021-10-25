---
layout: post
title: 'VSC File Line link'
date: 2021-10-26
categories: visual studio code extension file line link programming
---

# Visual Studio Extension - File Line link

## Start

To create initial structure of the project need to visit and follow instructions from [Your First Extension](https://code.visualstudio.com/api/get-started/your-first-extension).

## Package.json

The package json nodes should be updated:

* publisher
* activationEvents
* contributes

```json
{
	"name": "full-path",
	"displayName": "full-path",
	"description": "",
	"publisher": "ypyl",
	"version": "0.0.1",
	"engines": {
		"vscode": "^1.56.0"
	},
	"categories": [
		"Other"
	],
	"activationEvents": [
		"onCommand:full-path.copy"
	],
	"main": "./out/extension.js",
	"contributes": {
		"keybindings": [
			{
			  "command": "full-path.copy",
			  "key": "Alt+L"
			}
		],
		"commands": [
			{
				"command": "full-path.copy",
				"title": "Relative full path"
			}
		]
	},
	"scripts": {
		"vscode:prepublish": "npm run compile",
		"compile": "tsc -p ./",
		"watch": "tsc -watch -p ./",
		"pretest": "npm run compile && npm run lint",
		"lint": "eslint src --ext ts",
		"test": "node ./out/test/runTest.js"
	},
	"devDependencies": {
		"@types/vscode": "^1.56.0",
		"@types/glob": "^7.1.4",
		"@types/mocha": "^9.0.0",
		"@types/node": "14.x",
		"@typescript-eslint/eslint-plugin": "^4.31.1",
		"@typescript-eslint/parser": "^4.31.1",
		"eslint": "^7.32.0",
		"glob": "^7.1.7",
		"mocha": "^9.1.1",
		"typescript": "^4.4.3",
		"@vscode/test-electron": "^1.6.2"
	}
}
```

## Extension code

The following code gets the active text editor, grabs its relative path and selected line, and combine these inputs to relative file line link, like `folder/subfolder/file-name.ext:line-number`. After this, it puts the result to the clipboard.

```ts
import * as vscode from 'vscode';

export function activate(context: vscode.ExtensionContext) {
    const command = 'full-path.copy';

    const copyPathHandler = () => {
        try {
            const editor = vscode.window.activeTextEditor;
            if (!editor) {
                return;
            }
            // Get absolute path
            const absolutePath = editor.document.uri;
            if (!absolutePath) {
                return;
            }

            const relativePath = vscode.workspace.asRelativePath(absolutePath).replace(/\\/g, "/");

            // Get current line number
            const selectedLine = editor.selection.active.line + 1;

            // Join relative path and line numbers
            const pathLine = relativePath + ':' + selectedLine;

            vscode.env.clipboard.writeText(pathLine);
        } catch (e) {
            vscode.window.showErrorMessage((e as Error).message);
        }
    };

    context.subscriptions.push(vscode.commands.registerCommand(command, copyPathHandler));
}

// this method is called when your extension is deactivated
export function deactivate() {}
```

## Publish

To publish extension need to follow instructions from [Publishing Extensions](https://code.visualstudio.com/api/working-with-extensions/publishing-extension)

## Summary

These simple and straightforward steps allow to create a small extension to copy the file line link to the clipboard.
