---
layout: post
title: "Rendering and Highlighting PDFs in React with react-pdf"
date: 2026-02-22
tags: react pdf pdfjs ocr highlighting document-viewer frontend
categories: programming web development
---

# Rendering and Highlighting PDFs in React with react-pdf

## Overview

There are two fundamentally different PDF use cases in web applications:

1. **Generate PDFs from structured data** (e.g., JSON → PDF).
2. **Render and interact with existing PDF documents** (e.g., viewing, pagination, text highlighting, OCR overlays).

While exploring [json-render.dev](https://json-render.dev/docs/api/react-pdf) React PDF renderer, it became clear that it relies on [@react-pdf/renderer](https://github.com/diegomura/react-pdf/tree/master) (from **react-pdf**) for PDF generation.

However, for rendering and interacting with *existing* PDFs in a web app, the correct tool is [react-pdf](https://github.com/wojtekmaj/react-pdf), which is a React wrapper around **PDF.js**.

This article documents the distinction and shows how to render a PDF with:

* Pagination
* Text-layer highlighting
* Coordinate-based OCR overlays

---

## The Discovery

Initial exploration:

* `json-render.dev` → generates PDFs from JSON specs.
* Under the hood: `@react-pdf/renderer`.
* Good for document creation.
* Not designed for rendering existing PDFs interactively in the browser.

Then discovered:

* `wojtekmaj/react-pdf`
* Wraps PDF.js
* Designed for **displaying and interacting with existing PDFs**
* Supports:

  * Text layer
  * Annotation layer
  * Page rendering
  * Pagination
  * Custom text rendering

This enables highlighting both:

* Native PDF text (via `customTextRenderer`)
* Arbitrary regions (e.g., OCR bounding boxes)

---

## How It Works

### Mental Model

**react-pdf (wojtekmaj)** wraps PDF.js and exposes:

* `<Document />` → Loads PDF
* `<Page />` → Renders a page
* Text layer (optional)
* Annotation layer (optional)

Rendering layers:

```
Canvas layer     → actual PDF page raster
Text layer       → selectable/searchable text
Annotation layer → links, annotations
Custom overlays  → your absolute-positioned divs
```

### Text Highlighting

PDF.js extracts text into spans in the text layer.

You can override rendering with:

```tsx
customTextRenderer={({ str }) => highlightText(str)}
```

This allows injecting HTML like `<mark>`.

### OCR-Based Highlighting

OCR systems often return coordinates in:

* **PDF points**
* 1 point = 1/72 inch
* Origin: bottom-left (PDF coordinate system)

In this example, coordinates are directly mapped assuming:

* Page scale = 1
* Top-left origin adjustment already handled (inferred — verify if Y-axis inversion is required depending on PDF)

---

## Implementation

### Installation

```bash
npm install react-pdf
```

### Worker Setup

Required for PDF.js:

```ts
pdfjs.GlobalWorkerOptions.workerSrc = new URL(
  "pdfjs-dist/build/pdf.worker.min.mjs",
  import.meta.url
).toString();
```

Without this, rendering fails.

---

### Full Example

{% raw %}
```tsx
import "./App.css";
import { useState } from "react";
import { Document, Page, Thumbnail, pdfjs } from "react-pdf";
import "react-pdf/dist/Page/AnnotationLayer.css";
import "react-pdf/dist/Page/TextLayer.css";

pdfjs.GlobalWorkerOptions.workerSrc = new URL(
  "pdfjs-dist/build/pdf.worker.min.mjs",
  import.meta.url
).toString();

function App() {
  const [numPages, setNumPages] = useState<number>();
  const [pageNumber, setPageNumber] = useState<number>(3);
  const scale = 1;

  function onDocumentLoadSuccess({ numPages }: { numPages: number }): void {
    setNumPages(numPages);
  }

  const highlightText = (text: string) => {
    return text.replace(/Kernel GroupChat/g, "<mark>$&</mark>");
  };

  // OCR data: coordinates are in PDF points (1/72 inch)
  const ocrHighlights = [
    { page: 3, x: 100, y: 200, width: 150, height: 20, text: "Sample Text" },
    { page: 3, x: 100, y: 250, width: 200, height: 20, text: "Another highlight" },
  ];

  return (
    <div style={{ display: "flex", gap: "20px" }}>
      <Document file="/MCP.pdf" onLoadSuccess={onDocumentLoadSuccess}>
        <div style={{ position: "relative" }}>
          <Page
            pageNumber={pageNumber}
            scale={scale}
            customTextRenderer={({ str }) => highlightText(str)}
          />

          {ocrHighlights
            .filter((h) => h.page === pageNumber)
            .map((highlight, i) => (
              <div
                key={i}
                style={{
                  position: "absolute",
                  left: `${highlight.x * scale}px`,
                  top: `${highlight.y * scale}px`,
                  width: `${highlight.width * scale}px`,
                  height: `${highlight.height * scale}px`,
                  backgroundColor: "yellow",
                  opacity: 0.4,
                  pointerEvents: "none",
                }}
                title={highlight.text}
              />
            ))}
        </div>

        <p>
          <button
            onClick={() => setPageNumber(pageNumber - 1)}
            disabled={pageNumber <= 1}
          >
            Previous
          </button>

          <span>
            Page {pageNumber} of {numPages}
          </span>

          <button
            onClick={() => setPageNumber(pageNumber + 1)}
            disabled={pageNumber >= (numPages || 1)}
          >
            Next
          </button>
        </p>
      </Document>
    </div>
  );
}

export default App;
```
{% endraw %}

---

## Gotchas & Observations

### 1. Worker Configuration Is Mandatory

If `workerSrc` is not configured, rendering silently fails.

---

### 2. Text Highlighting Is String-Based

`customTextRenderer` operates on text fragments.

Implications:

* Regex must match fragment boundaries
* Long phrases may be split across spans
* Highlighting multi-line text is non-trivial

---

### 3. OCR Coordinates Require Careful Mapping

OCR systems usually return:

* Bottom-left origin
* PDF point units

You may need to:

* Invert Y-axis:

  ```
  top = pageHeight - y - height
  ```
* Apply scaling correctly

The current example assumes alignment is already correct. This must be validated per document.

---

### 4. Scale Synchronization

When changing `scale`:

* All overlay coordinates must be multiplied by the same scale.
* Otherwise, overlays drift.

---

### 5. Performance

Rendering large PDFs:

* Each page renders to canvas
* Text layer adds DOM nodes
* Multiple pages increase memory pressure

For heavy documents:

* Render one page at a time
* Avoid rendering thumbnails unless needed

---

## Conclusion

Use cases break down clearly:

| Use Case                            | Tool                  |
| ----------------------------------- | --------------------- |
| Generate PDFs from React components | `@react-pdf/renderer` |
| Render existing PDFs in browser     | `wojtekmaj/react-pdf` |

If you need:

* Text selection
* Highlighting
* OCR overlays
* Pagination
* Interactive viewing

→ Use `react-pdf` (PDF.js wrapper).

If you need:

* Programmatic document generation
* Declarative PDF layouts

→ Use `@react-pdf/renderer`.

### Next Steps

* Implement coordinate normalization layer for OCR alignment.
* Add search indexing across pages.
* Explore virtualized multi-page rendering for large documents.
* Evaluate text-layer extraction for semantic indexing.
