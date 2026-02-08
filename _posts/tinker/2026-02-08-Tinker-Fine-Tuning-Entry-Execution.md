---
layout: post
title: "Tinker: Fine-Tuning Entry & Execution"
date: 2026-02-08

tags: tinker, llm, finetuning, qwen3-vl
categories: ai, programming
---

Received $150 credit from [Tinker](https://thinkingmachines.ai/tinker/) to test the platform. Lacking prior LLM fine-tuning experience, I am following their official tutorial and documenting the workflow.

## Initialization

1. **Authentication:** Log in to the [console](https://tinker-console.thinkingmachines.ai/) and retrieve the API key.
2. **Documentation:** Accessed the [Training & Sampling](https://tinker-docs.thinkingmachines.ai/training-sampling) guide.
3. **Execution:** Copied and executed the initialization code.

There are some additional notes based on questions I had while reviewing and executing the example.

## Lora training client

```py
base_model = "Qwen/Qwen3-VL-30B-A3B-Instruct"
training_client = service_client.create_lora_training_client(
    base_model=base_model
)
```

Standard fine-tuning is inefficient; it requires updating every single parameter in a model. For a 30B parameter model, that means tracking 30 billion gradients. **LoRA (Low-Rank Adaptation)** bypasses this by freezing the original weights and training only a tiny fraction of new ones.

### How LoRA Works

LoRA operates on the mathematical premise that the change in weights during learning has a **"low intrinsic dimension."** In plain English: you don't need to move every knob to change how a model thinks; you only need to adjust a few key "vectors" of influence.

1. **Freeze the Base:** The original weights () of the Qwen3 model become read-only. They never change during training.
2. **Inject Adapters:** Two small, specialized matrices (A and B) are placed alongside the original layers.
3. **Train the Delta:** During training, only these small matrices are updated. They learn the "delta" (the difference) between the base model's knowledge and your specific task.
4. **Recombine:** When running the model, the output is the sum of the frozen base layer and the output of the trained adapters.

### The Immediate Benefits

* **VRAM Reduction:** Since you aren't calculating gradients for billions of parameters, you can train massive models on consumer-grade hardware or significantly smaller cloud instances.
* **Portability:** Instead of saving a new 60GB model file, you only save the "adapter" weights, which are typically only **10MB to 300MB**.
* **Zero Latency Inference:** The adapter matrices can be mathematically merged back into the base weights after training, meaning the final model runs just as fast as the original.

## Add special tokens in encode

```py
prompt_tokens = tokenizer.encode(prompt, add_special_tokens=True)
```

### 1. Model Conditioning and Structure

Special tokens provide the "structural scaffolding" the model needs to process information correctly.

* **Signaling the Start:** Tokens like "Beginning-of-sequence" tell the model exactly where an instruction or prompt begins.
* **Defining Boundaries:** They distinguish between different parts of a data sequence (e.g., separating the instruction from the response).
* **Proper Formatting:** They ensure the input matches the specific format the model was originally trained on, allowing it to interpret the context (conditioning) accurately.

### 2. Training Logic and Loss Calculation

In the context of the Tinker API, special tokens are essential for how the system handles the relationship between prompts and completions:

* **Weight Assignment:** Training data is split into **Prompts** (where tokens have weight 0, meaning the model isn't penalized for them) and **Completions** (where tokens have weight 1, used for calculating loss).
* **Selective Insertion:** * `add_special_tokens=True` is used for **Prompts** to ensure the model understands the start of an instruction.
* `add_special_tokens=False` is used for **Completions** to ensure the model learns to generate only the "pure" output text without redundant structural markers.


### 3. Execution Control

Special tokens handle the mechanics of the conversation:

* **Termination:** "End-of-sequence" tokens signal when the model should stop generating text.
* **Chat Dynamics:** They manage chat-specific formatting, ensuring the model knows which entity is speaking.

**Fundamental Truth:** Without these tokens, the model lacks the necessary metadata to distinguish between the input it must follow and the output it must generate, leading to structural collapse during training or inference.


## Prompt and Completion weights

```py
prompt_weights = [0] * len(prompt_tokens)
completion_weights = [1] * len(completion_tokens)
```

### The Core Logic

The weights act as a **filter** for the model's learning process. During training, the model attempts to predict the next token for the entire sequence, but the weights tell the system which predictions actually matter.

* **`prompt_weights = [0]`**: Assigning a weight of 0 tells the training algorithm to **ignore the loss** for these tokens. The model uses the prompt as context but is not penalized if it fails to "predict" the input. This prevents the model from wasting capacity memorizing your instructions or parroting user input.
* **`completion_weights = [1]`**: Assigning a weight of 1 tells the algorithm to **calculate loss** here. This is the "graded" portion of the exam. The model is forced to adjust its internal parameters so that its predicted output matches these specific tokens.

---

### Why this structure is used:

| Component | Logic | Result |
| --- | --- | --- |
| **Prompt** | Weight 0 (Context) | Model treats it as "given" information. |
| **Completion** | Weight 1 (Target) | Model learns to generate this specific response. |

### The "Student/Teacher" Fundamental

Think of it as a **Question and Answer** sheet:

1. The **Prompt** is the **Question**: You don't grade a student on their ability to rewrite the question; you provide it only so they know what to solve.
2. The **Completion** is the **Answer**: This is the only part that determines the grade.

By combining these arrays (`[0, 0, 0] + [1, 1, 1]`), you create a **mask** that focuses the model’s "intellectual effort" exclusively on the desired output.

## Token Shifting and Alignment

```py
input_tokens = tokens[:-1]
target_tokens = tokens[1:]
weights = weights[1:]
```

### The Core Logic

This shift implements **Causal Language Modeling**. Since the model's task is to predict the future based on the past, the input and the target cannot be identical at the same time step.

* **`tokens[:-1]`**: Removes the final token. The model cannot predict what comes after the end of the data, so the last token has no "target."
* **`tokens[1:]`**: Removes the first token. The model has no prior context to predict the very first token, so the first token has no "input."
* **`weights[1:]`**: Aligns the importance mask with the shifted targets. This ensures the model is graded only on the correct completion tokens.

---

### Why this structure is used:

| Operation | Action | Fundamental Truth |
| --- | --- | --- |
| **Input Slice** | Drop Last | You can't predict what doesn't exist yet. |
| **Target Slice** | Drop First | You can't predict the start from nothing. |
| **Weight Slice** | Align Mask | Loss must be calculated on the *result*, not the *source*. |

### The "Temporal Shift" Fundamental

Think of this as a **Relay Race**:

1. **Input (The Runner):** At any given index, the input is the token currently "holding the baton."
2. **Target (The Receiver):** The target is the token that *should* be receiving the baton next.

By shifting the arrays, you align **Token 1** (Input) with **Token 2** (Target). Without this shift, you would be asking the model to predict **Token 1** while it is already looking at **Token 1**, which teaches it nothing but how to copy.

By slicing `weights[1:]`, you ensure that if **Token 4** was the start of your completion, the "Weight 1" (the grade) is applied exactly when the model is tasked with predicting **Token 4**.

## Training Execution (Forward and Backward Pass)

```py
fwdbwd_future = training_client.forward_backward(processed_examples, "cross_entropy")
```

### The Core Logic

This line initiates the fundamental mathematical heavy-lifting of the training process. It handles the "learning" calculation without actually updating the model's memory yet.

* **`forward_backward`**: Executes two distinct operations. The **Forward Pass** runs data through the model to see what it predicts (logprobs). The **Backward Pass** calculates the "gradients"—the mathematical instructions on how to adjust the model to reduce error.
* **`processed_examples`**: These are the tokenized sequences, targets, and weights created in the previous steps. They provide the context and the "answer key" for the model.
* **`"cross_entropy"`**: The specific loss function used to measure the distance between the model's guess and the correct target token.

---

### Why this structure is used:

| Component | Role | Fundamental Truth |
| --- | --- | --- |
| **`fwdbwd_future`** | Async Placeholder | Computation happens on the server; the code continues without waiting. |
| **Gradients** | Direction of Change | Tells the model *how* it was wrong, but doesn't fix it yet. |
| **Separation** | Phase 1 of 2 | Decoupling calculation from updating allows for more stable training. |

### The "Drafting" Fundamental

Think of this as **The Grading Phase**:

1. **The Forward Pass:** The student (the model) takes the test.
2. **The Backward Pass:** The teacher (the API) marks exactly where the student went wrong and writes down how to fix those specific mistakes.
3. **The Future:** This is like submitting the test to a grading center. You get a "receipt" (`future`), but you have to wait (`.result()`) to get the actual grades back.

By keeping this separate from `optim_step`, the system ensures that the model doesn't change its "mind" in the middle of a batch of questions—it waits until it has analyzed all the mistakes before making any actual adjustments to its weights.

## Sequence Logprobs

```py
logprobs = np.concatenate([output['logprobs'].tolist() for output in fwdbwd_result.loss_fn_outputs])
```

### The Core Logic

Logprobs (Logarithmic Probabilities) represent the model's "confidence" score for every possible word in its dictionary, at every single point in a sentence.

* **Vocabulary-Wide:** For every single position in your sequence, the model doesn't just guess one word; it assigns a score to **every** word it knows (the entire vocabulary).
* **Log Space:** We use logarithms because raw probabilities (like ) are too small for computers to handle reliably. Adding logprobs is mathematically equivalent to multiplying raw probabilities, which is more numerically stable.
* **The Matrix:** If your sequence has  tokens and your vocabulary is  words, the logprobs are a  grid of numbers.

---

### Why this structure is used:

| Concept | Action | Fundamental Truth |
| --- | --- | --- |
| **Distribution** | Score every word | The model evaluates all possibilities, not just the "right" one. |
| **Log Format** | Stability | Prevents "underflow" (numbers becoming so small they turn into zero). |
| **Extraction** | Pick the target | Loss is only calculated by looking at the score of the *correct* word. |

### The "Multiple Choice" Fundamental

Think of this as a **Massive Multiple Choice Test**:

1. **The Question:** For every blank space in a sentence, the student is given a list of  possible answers.
2. **The Student's Work (Logprobs):** Instead of just circling one answer, the student writes a "confidence score" next to every single one of the  options.
3. **The Grading (Loss):** The teacher looks **only** at the score the student gave to the *correct* answer. If the student gave the correct answer a high score (close to  in log space), the loss is low. If they gave it a low score, the loss is high.

**Fundamental Truth:** Logprobs are the "raw thoughts" of the model before they are filtered. During training, we extract the specific "thought" related to our target token and ignore the other  scores at that position.

## Loss Calculation and Weighting

### The Core Logic

To understand the relationship between **Logprobs**, **Target Tokens**, and **Weights**, you must distinguish between what the model "guesses" and what the "answer key" requires.

* **Logprobs (The Model's Guess):** A distribution over the entire vocabulary for every position. The model doesn't "pick" a token; it assigns a score to *every* possible token.
* **Target Tokens (The Answer Key):** This is a single ID at each position. It tells the system: "At this spot, the correct answer was Token ID 42."
* **The Intersection (Selecting the Logprob):** The system ignores the scores for the wrong tokens. it only looks at the logprob the model assigned to the **correct** target token.
* **The Weight (The Grading Mask):** A multiplier (0 or 1). Even if the model gets a "grade" for a token, the weight determines if that grade actually affects the model's training.

---

### Why this structure is used:

| Component | Nature | Fundamental Truth |
| --- | --- | --- |
| **Logprobs** | Predicted Distribution | The model's "internal thoughts" on what word comes next. |
| **Target Token** | Specific ID | The ground truth of what *actually* comes next. |
| **Weight** | Binary Multiplier | Decides if the "thought" at this position should be learned. |

### The "Exam Grading" Fundamental

Think of the entire training sequence as a test where some questions are "practice" and some are "for a grade":

1. **The Question:** The model provides a probability distribution (Logprobs) for every blank on the page.
2. **The Answer Key:** The teacher (Target Tokens) points to the correct word for every blank.
3. **The Selective Grade:** The teacher finds the student's confidence score for the **correct** word.
4. **The Weight (The "Practice" vs. "Final" flag):**
* **Weight 0 (Prompt):** The teacher says, "I see your score here, but this is a practice question. I’m not changing your grade based on this."
* **Weight 1 (Completion):** The teacher says, "This counts. If your score for the correct word is low, you are failing this part, and I am going to force you to study (update weights) until you get it right."


**Fundamental Truth:** During training, the model does **not** pick the token with the highest probability. It is simply told the correct answer, and it is "punished" (Loss) based on how far its own logprob for that specific answer was from 100%. The weight is the final filter that decides if that punishment actually happens.

## Final Loss Calculation

```py
loss = -np.dot(logprobs, weights) / weights.sum()
```

### The Core Logic

This line of code converts raw model predictions and the weight mask into a single "grade" (the loss). It is the final step in determining how well the model performed on a specific training example.

* **`np.dot(logprobs, weights)`**: This is a mathematical shortcut. It multiplies each logprob by its corresponding weight and sums the results. Since prompt weights are `0`, their logprobs are multiplied by zero and effectively "deleted" from the total. Only logprobs with a weight of `1` (the completion) remain.
* **The Negative Sign (`-`)**: Log-probabilities are negative numbers (e.g., `-0.5`). Training algorithms require a positive "loss" value to minimize. Negating the sum turns a negative confidence score into a positive penalty.
* **`/ weights.sum()`**: This divides the total loss by the number of completion tokens. This calculates the **average loss per token**, ensuring that a long sentence isn't penalized more than a short sentence simply because it has more words.

---

### Why this structure is used:

| Operation | Action | Fundamental Truth |
| --- | --- | --- |
| **Dot Product** | Masking & Summing | Automatically ignores the prompt and aggregates the completion. |
| **Negation** | Sign Flip | Converts "log-probability" into "loss." |
| **Division** | Normalization | Makes the loss comparable across sequences of different lengths. |

### The "Averaging the Grade" Fundamental

Think of a student taking a test with 10 questions, but only the last 5 count for their grade:

1. **The Scores (`logprobs`):** The student gets a score for all 10 questions.
2. **The Dot Product:** You multiply the first 5 scores by `0` (practice) and the last 5 by `1` (real test). You sum them up to get the total points earned on the real test.
3. **The Sum of Weights:** You count how many questions actually mattered (in this case, `5`).
4. **The Result:** You divide the total points by `5` to get the student's **average score**.

**Fundamental Truth:** This formula is the "Masked Cross-Entropy Loss." It ensures the model is only "punished" for the parts it was supposed to generate, not the parts it was given as context.


## Model Checkpointing and Inference Transition

```py
sampling_client = training_client.save_weights_and_get_sampling_client(name='pig-latin-model')
```

### The Core Logic

This line marks the transition from **Training** (learning) to **Inference** (using). The reason it doesn't use the weights mask is that the learning has already been "baked into" the model's parameters by the time this code executes.

* **`save_weights`**: Permanently stores the model's internal parameters (weights) that were modified during the `optim_step`.
* **`get_sampling_client`**: Creates an interface for you to talk to the newly trained model.
* **Weight Absence**: The `[0, 1]` weights mask is a tool for **Loss Computation**. Once the model has updated its internal state, the "grading rubric" (the mask) is no longer needed.

---

### Why this structure is used:

| Phase | Relevant Components | Fundamental Truth |
| --- | --- | --- |
| **Learning** | `forward_backward`, weights mask | The mask guides the gradient calculation. |
| **Adjustment** | `optim_step` | The model's brain changes based on gradients. |
| **Deployment** | `save_weights_and_get_sampling_client` | You are freezing the brain to use it. |

### The "Knowledge Transfer" Fundamental

Think of this as **Graduation Day**:

1. **The Weights Mask:** This was the red pen used by the teacher during the semester to mark the student's homework.
2. **The Updates:** These are the new neural pathways formed in the student's brain as they studied their mistakes.
3. **This Line of Code:** This is the student receiving their diploma and stepping into a job.

When the student starts their new job (Sampling), they don't carry the red pen (the weights mask) with them. They only carry the **knowledge** (the updated weights) they gained from the process.

**Fundamental Truth:** Training is a process of *changing* weights; Sampling is the process of *using* them. The weights mask is the catalyst for the change, but it is not part of the final result.
