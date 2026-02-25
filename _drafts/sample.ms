import { createPluginRegistration } from "@embedpdf/core";
import { EmbedPDF } from "@embedpdf/core/react";
import { usePdfiumEngine } from "@embedpdf/engines/react";

// Import the essential plugins
import { Viewport, ViewportPluginPackage } from "@embedpdf/plugin-viewport/react";
import { Scroller, ScrollPluginPackage } from "@embedpdf/plugin-scroll/react";
import { DocumentManagerPluginPackage } from "@embedpdf/plugin-document-manager/react";
import { RenderLayer, RenderPluginPackage } from "@embedpdf/plugin-render/react";
import { AnnotationPluginPackage, useAnnotation } from "@embedpdf/plugin-annotation/react";
import { InteractionManagerPluginPackage, PagePointerProvider } from "@embedpdf/plugin-interaction-manager/react";
import { AnnotationLayer } from "@embedpdf/plugin-annotation/react";
import { PdfAnnotationSubtype, type Rect } from '@embedpdf/models';
import { SelectionPluginPackage } from '@embedpdf/plugin-selection/react'
// Your highlights from Azure Document Intelligence
const azureHighlights = [
  {
    pageIndex: 0, // 0-based page
    pageHeightPts: 792, // Letter page = 792pts tall
    boundingBox: [1.2, 2.3, 3.4, 2.3, 3.4, 2.6, 1.2, 2.6], // inches polygon
  },
];

// pageHeightInPoints: the PDF page height in pts (e.g. 792 for Letter)
function azureBBoxToRect(boundingBox: number[], pageHeightPts: number): Rect {
  const pts = 72;
  const xs = [boundingBox[0], boundingBox[2], boundingBox[4], boundingBox[6]].map(x => x * pts);
  const ys = [boundingBox[1], boundingBox[3], boundingBox[5], boundingBox[7]].map(y => y * pts);

  const left   = Math.min(...xs);
  const top    = Math.min(...ys);
  const right  = Math.max(...xs);
  const bottom = Math.max(...ys);

  return {
    origin: {
      x: left,
      y: pageHeightPts - bottom,  // flip Y: PDF origin is bottom-left
    },
    size: {
      width:  right - left,
      height: bottom - top,
    },
  };
}

const plugins = [
  createPluginRegistration(DocumentManagerPluginPackage, {
    initialDocuments: [{ url: "2.pdf" }],
  }),

  // Register dependencies first
  createPluginRegistration(InteractionManagerPluginPackage),
  createPluginRegistration(ViewportPluginPackage),
  createPluginRegistration(ScrollPluginPackage),
  createPluginRegistration(RenderPluginPackage),
  createPluginRegistration(AnnotationPluginPackage),
  createPluginRegistration(SelectionPluginPackage),
];

// Inner component so it can use hooks inside EmbedPDF context
function HighlightLoader({ documentId }: { documentId: string }) {
  const { provides } = useAnnotation(documentId);

  const applyHighlights = () => {
    if (!provides) return;

    for (const h of azureHighlights) {
      const rect = azureBBoxToRect(h.boundingBox, h.pageHeightPts);

      provides.createAnnotation(h.pageIndex, {
        id: "unqiueue-id-for-this-annotation", // generate a unique ID in production
        type: PdfAnnotationSubtype.HIGHLIGHT,
        rect, // bounding box of the entire annotation
        segmentRects: [rect], // one rect per line of highlighted text
        color: "#FFFF00",
        opacity: 0.4,
        pageIndex: h.pageIndex,
      });
    }
  };

  return <button onClick={applyHighlights}>Highlight from Azure DI</button>;
}

export function PDFViewer2() {
  const { engine, isLoading } = usePdfiumEngine();

  if (isLoading || !engine) return <div>Loading engine...</div>;

  return (
    <div style={{ height: "100vh", display: "flex", flexDirection: "column" }}>
      <EmbedPDF engine={engine} plugins={plugins}>
        {({ activeDocumentId }) =>
          activeDocumentId && (
            <>
              <HighlightLoader documentId={activeDocumentId} />
              <Viewport documentId={activeDocumentId} style={{ flex: 1 }}>
                <Scroller
                  documentId={activeDocumentId}
                  renderPage={({ pageIndex }) => (
                    <PagePointerProvider documentId={activeDocumentId} pageIndex={pageIndex}>
                      <RenderLayer documentId={activeDocumentId} pageIndex={pageIndex} />
                      {/* Annotation Layer on top */}
                      <AnnotationLayer documentId={activeDocumentId} pageIndex={pageIndex} />
                    </PagePointerProvider>
                  )}
                />
              </Viewport>
            </>
          )
        }
      </EmbedPDF>
    </div>
  );
}
