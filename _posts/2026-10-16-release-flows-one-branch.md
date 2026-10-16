---
layout: post
title: "Release Flows for a One-Branch Model"
date: 2026-10-16
categories: deployment
tags: [release-engineering, trunk-based-development, git, ci-cd, azure-pipelines, branching]
---

One long-lived branch, `dev`, is the trunk. Work merges there, the trunk stays green, and a release is a tag on that trunk instead of a branch of its own. The artifact describes itself: a container image carries the commit, the version, and the source in its OCI labels, so no flow has to keep a separate deploy record.

The graph below is the whole process for that model. Each tab is one flow: deploy to DEV, normal release, roll back, hotfix from trunk, hotfix from tag. The first tab is the decision tree that routes four situations to one of those five. The same question answers most of them: is the product folder on the trunk the folder that shipped? Hover a dotted term and it explains itself. Inside a flow the stepper and the commit graph move together, so selecting a step shows the commit, tag, and environment it touches.

<iframe id="flows-graph" src="/assets/release-flows/one-branch/flows.graph.html" title="Interactive release-flow graph: decision tree, deploy to DEV, normal release, roll back, hotfix from trunk, hotfix from tag" style="display:block;width:100%;height:900px;border:1px solid var(--border-color);border-radius:8px;background:var(--surface)"></iframe>

<script>
(function () {
  var frame = document.getElementById("flows-graph");
  if (!frame) return;
  frame.addEventListener("load", function () {
    var doc = frame.contentDocument;
    if (!doc || !doc.body) return;
    var content = doc.querySelector(".wrap") || doc.body;
    function fit() {
      frame.style.height = Math.max(480, content.scrollHeight) + "px";
    }
    if (window.ResizeObserver) new ResizeObserver(fit).observe(content);
    fit();
  });
})();
</script>
