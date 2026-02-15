---
layout: post
title: "Tinker: Saving and Loading Checkpoints"
date: 2026-02-15
tags: tinker, llm, finetuning
categories: ai, programming
---

## What *weights* are

*Definition* – Learned numeric parameters inside a neural network that scale inputs to produce outputs.

*Analogy* – Knobs the model turns during training.

### Tiny‑model illustration
```python
def tiny_model(happy_count, sad_count):
    weight_happy = 2.0      # weight
    weight_sad   = -3.5     # weight
    bias         = 0.1     # weight (bias)

    score = happy_count * weight_happy + sad_count * weight_sad + bias
    return score
```
The three numbers (`2.0`, `‑3.5`, `0.1`) are **weights** (including bias).
Real models contain millions‑to‑billions of such values.

### Tinker API
* `save_weights_for_sampler()` → **stores only the weights** (model parameters).
  → Fast, lightweight, inference‑ready.

---

## What *optimizer state* is

*Definition* – Auxiliary values kept by the optimizer to decide how to update weights (e.g., momentum, variance, learning‑rate schedule, step counters).

*Not part of the model* – They belong to the training machinery.

### Adam‑optimizer illustration
```python
optimizer_state = {
    "weight_happy": {"m": 0.004, "v": 0.00002},
    "weight_sad":   {"m": -0.003, "v": 0.00001},
    "step": 1200
}
```
`m` = momentum, `v` = variance, `step` = global update counter.

### Tinker API
* `save_state()` → **stores weights + optimizer state** → enables exact training resume.

---

## Summary Table
| Concept          | Contains                               | Purpose                              | Saved by                |
|------------------|----------------------------------------|--------------------------------------|------------------------|
| **Weights**      | Learned tensors (including bias)       | Run the model (inference)            | `save_weights_for_sampler()` |
| **Optimizer state** | Momentum, variance, LR schedule, step counters | Continue training correctly          | `save_state()`               |
