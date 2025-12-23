---
layout: post
title: Building a Seamless Double-Click-to-Edit Component for Mantine
date: 2025-12-23
tags: react mantine typescript npm frontend web-development
categories: programming frontend
---

[mantine-double-click-editable](https://www.npmjs.com/package/mantine-double-click-editable) was recently published to NPM. It's a lightweight, Mantine-based text component that allows users to edit text directly in-place with a standard double-click, while solving one of the most annoying UX hurdles of inline editing: **precise caret placement**.

### The Problem: Clunky Inline Editing

Standard inline editing often feels "jumpy." Usually, you click an "Edit" button or double-click a field, and it replaces the text with an `<input>` or `<textarea>`. The cursor often defaults to the beginning or the end of the line, forcing the user to click *again* to get back to where they actually wanted to make a change.

The goal was to create something that felt more like a native text editor—where a double-click on a word lands the cursor exactly where the user clicked.

![Demo GIF](https://raw.githubusercontent.com/ypyl/DoubleClickEditable/main/msedge_g0RHaauDdc.gif)

### How It Works

The core of the component is built on Mantine's `Text` component with the `contentEditable` attribute.

To place the caret precisely, the `document.caretRangeFromPoint` API (and its Firefox equivalent) was used. This allows for translating the mouse coordinates of the double-click event directly into a text range.

```tsx
const handleDoubleClick = (e: React.MouseEvent<HTMLDivElement>) => {
  setIsEditable(true);
  const { clientX, clientY } = e;

  setTimeout(() => {
    const selection = window.getSelection();
    if (!selection) return;

    // Chromium/Safari support
    if (document.caretRangeFromPoint) {
      const range = document.caretRangeFromPoint(clientX, clientY);
      if (range) {
        selection.removeAllRanges();
        selection.addRange(range);
      }
    }
    // Firefox fallback
    else if ((document as any).caretPositionFromPoint) {
      const pos = (document as any).caretPositionFromPoint(clientX, clientY);
      if (pos) {
        const range = document.createRange();
        range.setStart(pos.offsetNode, pos.offset);
        range.collapse(true);
        selection.removeAllRanges();
        selection.addRange(range);
      }
    }
  }, 0);
};
```

By wrapping this in a `setTimeout(..., 0)`, we ensure the browser has finished transitioning the element to `contentEditable` before we attempt to manipulate the selection.

### Built with Mantine

Because the component extends Mantine's `Text` props, it works out-of-the-box with all the styling power Mantine provides: colors, font weights, and sizes just like with a regular `<Text />` component:

```tsx
<DoubleClickEditable
  c="blue"
  fw={700}
  fz="xl"
  onSave={(content) => console.log('Final text:', content)}
>
  Double-click me to see the magic!
</DoubleClickEditable>
```

### Publishing to NPM

The publication process was managed with Vite in library mode. `vite-plugin-dts` was used to handle the TypeScript declaration files, and the release process was automated with GitHub Actions.

You can check out the source code on GitHub: [ypyl/DoubleClickEditable](https://github.com/ypyl/DoubleClickEditable).

### Try It Out

If you're using Mantine in your project and need a quick, intuitive way to add inline editing, give it a try:

```bash
npm install mantine-double-click-editable
```

Feedback and contributions on GitHub are highly welcome!
