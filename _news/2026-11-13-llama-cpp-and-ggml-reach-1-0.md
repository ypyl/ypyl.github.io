---
layout: post
title: "llama.cpp and ggml Reach 1.0 to Promise a Stable C API for Downstream Apps"
date: 2026-11-13
categories: news
---

After 3.5 years of development, the Hugging Face team behind **llama.cpp** has tagged **v1.0.0** for both `libllama` and the tensor library under it, **ggml v1.0.0**. There is no new feature to announce here: the milestone is a contract, and the contract is the point. Every downstream binding, desktop app and packaging pipeline builds against the two public C headers in `include/`, and until now a breaking change could land in any release.

From 1.0 the semantic version rules are explicit:

- **MAJOR**: a breaking change to the public C API in `./include`
- **MINOR**: a backward-compatible feature, backend update, model support or API addition
- **PATCH**: a bug fix with no API change

API and ABI incompatibilities now show up as major version bumps, so a consumer can pin a version line instead of chasing `master`.

Distribution is already broad, which is why the guarantee matters: pre-release artifacts for Linux, macOS, Windows, Android and iOS, plus Docker, Debian, Homebrew, conda-forge, MacPorts, Winget and an XCFramework for iOS and macOS. The library covers more than **150 model architecture variants** and all mainstream speculative decoding methods, with ggml backends compiled as dynamic libraries that can be loaded at runtime.

The release post also details the testing setup behind that promise: CI runs on self-hosted runners spanning NVIDIA, AMD, Qualcomm, Arm, IBM and Intel hardware, scaled with **HF Buckets** and **HF Jobs**, on the theory that a stable ABI is worth only as much as the coverage that catches an accidental break.

**Related:** [PrismML Releases Bonsai 27B, Extreme Compression for Local AI](/news/2026/07/16/prismml-bonsai-27b-extreme-compression/), [Qwen3.8-Flash Now Run Locally via Unsloth GGUF](/news/2026/08/26/qwen3-8-flash-local-unsloth-gguf/)

[llama.cpp v1.0.0 announcement](https://huggingface.co/blog/ggml-org/llama-cpp-v1) · [llama.cpp release](https://github.com/ggml-org/llama.cpp/releases/tag/v1.0.0) · [ggml release](https://github.com/ggml-org/ggml/releases/tag/v1.0.0)
