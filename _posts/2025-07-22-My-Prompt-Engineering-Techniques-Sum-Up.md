---
layout: post
title: Text based prompting
date: 2025-07-22

tags: slm prompting llm genai ai
categories: programming genai
---

# In-Context learning

## Few-Shot prompting

Few-shot prompting provides the model with a few examples (shots) of the task to guide its response. By including several input-output pairs in the prompt, the model can better understand the desired format, reasoning process, and expected output. This approach is especially useful when you have limited data or want to quickly adapt the model to a new task without fine-tuning.

### K-Nearest Neighbor (KNN)
Selects examples most similar to the current input.
```
Translate the following English words to French:
- sea -> mer
- sky -> ciel
- house -> maison
- car -> voiture
```

### Vote-K
A method to select similar exemplars to the test sample. In one stage, a model proposes useful unlabeled candidate exemplars for an annotator to label. In the second stage, the labeled pool is used for Few-Shot Prompting. Vote-K also ensures that newly added exemplars are sufficiently different than existing ones to increase diversity and representativeness.
```
# Stage 1: The model proposes useful unlabeled candidate exemplars.

I'm training a sentiment classifier. Here are some existing labeled examples:
- "I loved the movie." (Positive)
- "The film was a disaster." (Negative)

Propose 3 new, diverse, and unlabeled sentences about movies that would be most useful to add to my training set.

# After an annotator labels the proposed sentences, they are added to the few-shot prompt.
```

### Self-Generated In-Context Learning (SG-ICL)
Model generates its own examples for prompting.
```
Generate three examples of positive and negative movie reviews, and then classify the sentiment of the following review: 'The movie was a masterpiece.'
```

### Prompt Mining
Prompt Mining automatically finds the most effective prompts from a large dataset. It works in two stages: first, it generates a set of candidate prompts, and second, it selects the best one based on performance or frequency.
```
# Task: Find the best prompt to ask for a country's capital.

# Stage 1: Prompt Generation (Mining)
# Scan a large text corpus (like Wikipedia) for sentences containing known entity pairs (e.g., "France" and "Paris").
# Candidate prompts are generated from the text connecting these pairs.
"What is the capital of [Country]?"
"[Country]'s capital is?"
"Tell me the capital of [Country]."

# Stage 2: Prompt Selection (Evaluation)
# Each candidate prompt is used to query the language model for a set of known capitals.
# The prompt that results in the highest accuracy is selected.

# Let's say "What is the capital of [Country]?" has the highest accuracy.

# Selected Prompt for use:
"What is the capital of Japan?"
```

# Zero-Shot prompting

Zero-shot prompting involves giving a model a task without any prior examples, relying on its pre-trained knowledge to generate a response.

### Role Prompting
Assigns a specific persona or role to the model (e.g., "You are a historian").
```
You are a world-class chef. Provide a recipe for a classic French onion soup.
```

### Style Prompting
Instructs the model to generate text in a particular style (e.g., "Write in a formal tone").
```
Write a poem about the beauty of nature in the style of William Wordsworth.
```

### Emotion Prompting
Incorporates phrases of psychological relevance to humans into the prompt, which may lead to improved LLM performance.
```
Summarize the following text. This is very important for my career.
```

### System 2 Attention (S2A)
First asks an LLM to rewrite the prompt and remove any information unrelated to the question therein. Then, it passes this new prompt into an LLM to retrieve a final response.
```
# Original Prompt:
"I was thinking about the first man on the moon, Neil Armstrong. My grandpa, who was a carpenter, told me stories about watching the moon landing on his black-and-white TV. What was the date of the moon landing?"

# S2A - Step 1: Rewrite the prompt to remove unrelated information.
"What was the date of the moon landing?"

# S2A - Step 2: Pass the new prompt to the LLM.

# Final Response:
"July 20, 1969"
```

### SimToM
Deals with complicated questions which involve multiple people or objects. Given the question, it attempts to establish the set of facts one person knows, then answer the question based only on those facts. This is a two prompt process and can help eliminate the effect of irrelevant information in the prompt.
```
# Original Prompt:
"John knows that Mary is in the library. Mary knows that the library is closed on Sundays. Today is Sunday. Where does John think Mary is?"

# SimToM - Step 1: Establish the facts known by the person in question.
"What does John know about Mary's location?"

# SimToM - Step 2: Answer the question based only on those facts.
"Based on what John knows, where does he think Mary is?"

# Final Response:
"John thinks Mary is in the library."
```

### Rephrase and Respond (RaR)
Instructs the LLM to rephrase and expand the question before generating the final answer. This could all be done in a single pass or the new question could be passed to the LLM separately.
```
# Original question
What is the capital of Australia?

# Rephrase and Respond (RaR) prompt
Rephrase and expand the following question, and respond: "What is the capital of Australia?"

# Model's possible response:
Rephrased question: What is the official capital city of the country of Australia?
Response: The official capital of Australia is Canberra.
```

### Re-reading (RE2)
Adds the phrase "Read the question again:" to the prompt in addition to repeating the question. Although this is such a simple technique, it has shown improvement in reasoning benchmarks, especially with complex questions.
```
# Original question:
If a train leaves the station at 3:00 PM traveling at 60 mph, and a second train leaves the same station at 4:00 PM traveling at 80 mph in the same direction, at what time will the second train catch up to the first train?

# Re-reading (RE2) prompt:
Read the question again: If a train leaves the station at 3:00 PM traveling at 60 mph, and a second train leaves the same station at 4:00 PM traveling at 80 mph in the same direction, at what time will the second train catch up to the first train?
```

### Self-Ask
Prompts LLMs to first decide if they need to ask follow up questions for a given prompt. If so, the LLM generates these questions, then answers them and finally answers the original question.
```
# Original Question:
Who was the first person to walk on the moon?

# Self-Ask Prompt:
Are there any follow-up questions needed to answer this question? If so, what are they?

# Model's possible response:
Follow-up questions:
1. Who was the first person to step out of the lunar module?
2. What was the name of the mission?

Answers to follow-up questions:
1. Neil Armstrong was the first person to step out of the lunar module.
2. The mission was Apollo 11.

Final Answer: The first person to walk on the moon was Neil Armstrong, during the Apollo 11 mission.
```

# Thought Generation

Thought generation techniques guide the model to produce intermediate reasoning steps, leading to more accurate and transparent outputs.

## Zero-Shot-CoT

### Zero-Shot-CoT
It involves appending a thought-inducing phrase to the prompt, such as "Let's think step by step." This encourages the model to generate a chain of thought before providing the final answer. Other phrases can also be used, like "First, let’s think about this logically" or "Let’s work this out in a step by step way to be sure we have the right answer." This approach is attractive because it doesn't require examples and is generally task-agnostic.
```
Q: A man has 5 apples and he gives 2 to his friend. How many apples does he have left? Let's think step by step.
A: 1. The man starts with 5 apples.
2. He gives 2 apples to his friend.
3. To find out how many apples he has left, I need to subtract the number of apples he gave away from the number of apples he started with.
4. 5 - 2 = 3.
The man has 3 apples left.
```

### Step-Back Prompting
The model is prompted to take a step back and consider the broader context of the question.
```
Before you answer the question, take a step back and consider the underlying principles of physics that are relevant to this problem.
```

### Analogical Prompting
Automatically generates exemplars (examples) with included chains of thought to improve reasoning for a given task. It is particularly useful for mathematical reasoning and code generation.
```
# User's problem:
# "A cyclist bikes at 15 mph for 2 hours. How far do they go?"

# Internally, we are asking the model to generate relevant examples with step-by-step solutions:
# Exemplar 1:
# Q: If a car travels at 60 km/h, how far will it travel in 3 hours?
# A: Let's think step by step. Speed is 60 km/h, time is 3 hours. Distance = 60 * 3 = 180 km.

# The model then uses this analogical reasoning to solve the user's problem:
# A: Let's think step by step. Speed is 15 mph, time is 2 hours. Distance = 15 * 2 = 30 miles.
```

### Thread-of-Thought (ThoT) Prompting
An improved thought inducer for CoT reasoning that uses the phrase "Walk me through this context in manageable parts step by step, summarizing and analyzing as we go". This is particularly effective for question-answering and retrieval tasks with large, complex contexts.
```
# Context: [A long and complex article about the history of artificial intelligence]

# Prompt:
Walk me through this context in manageable parts step by step, summarizing and analyzing as we go. What were the key milestones in the development of AI?

# Model's possible response:
Step 1: Early Beginnings (1940s-1950s)
Summary: The concept of AI emerged with early pioneers like Alan Turing. The Dartmouth Workshop in 1956 is considered the birthplace of AI as a field.
Analysis: This period was characterized by theoretical work and foundational ideas, with limited practical application due to computational constraints.

Step 2: The First AI Boom (1960s-1970s)
Summary: This era saw the development of early expert systems and natural language processing.
Analysis: ...
```

### Tabular Chain-of-Thought (Tab-CoT)
A Zero-Shot CoT prompt that makes the LLM output reasoning as a markdown table. This tabular design enables the LLM to improve the structure and thus the reasoning of its output.
```
Please solve the following logic puzzle and present your reasoning in a table with columns for 'Step', 'Action', and 'Justification'.

| Step | Action | Justification |
|---|---|---|
| 1 | Assume A is the knight. | Start with an assumption. |
| 2 | If A is the knight, then A's statement is true. | Knights always tell the truth. |
| 3 | A says "B is the knave." | This is A's statement. |
| 4 | Therefore, B must be the knave. | Based on steps 2 and 3. |
| 5 | B says "A and I are of opposite types." | This is B's statement. |
| 6 | If B is the knave, their statement is false. | Knaves always lie. |
| 7 | The statement "A and I are of opposite types" is false. | Based on step 6. |
| 8 | This means A and B are of the same type. | Negation of B's statement. |
| 9 | This contradicts our finding that A is a knight and B is a knave. | Contradiction between step 4 and 8. |
| 10 | Therefore, our initial assumption is wrong. A is not the knight. | The assumption led to a contradiction. |
| ... | ... | ... |
```

## Few-Shot CoT

### Contrastive CoT Prompting
The model is provided with both correct and incorrect reasoning examples to help it learn to distinguish between them.
```
Here is an example of a good reasoning process and a bad reasoning process. ... Now, solve the following problem using the good reasoning process.
```

### Uncertainty-Routed CoT Prompting
The model is prompted to identify and address uncertainty in its reasoning process.
```
If you are unsure about any part of your answer, please state your uncertainty and explain why you are unsure.
```

### Complexity-based Prompting
The model is prompted to adjust its reasoning process based on the complexity of the problem.
```
This is a complex problem, so take your time and break it down into smaller steps. Explain your reasoning for each step.
```

### Active Prompting
The model is prompted to ask clarifying questions to resolve ambiguity.
```
If you need more information to answer the question, please ask me.
```

### Memory-of-Thought Prompting
The model is prompted to use a memory of its previous thoughts to inform its current reasoning.
```
Remember that you previously determined that the suspect was at the scene of the crime. How does this new piece of evidence affect your conclusion?
```

### Automatic Chain-of-Thought (Auto-CoT) Prompting
The model automatically generates its own chain of thought without explicit prompting.
```
Solve the following problem and show your work.
```

# Decomposition

Decomposition techniques break down complex problems into smaller, more manageable subproblems.

### Least-to-Most Prompting
The model solves a problem by starting with the simplest subproblem and gradually moving to more complex ones.
```
First, solve for x in the equation 2x = 4. Then, solve for y in the equation 3y = 9. Finally, solve for z in the equation 4z = 16.
```

### Decomposed Prompting
The model is prompted to break down a problem into its constituent parts.
```
Break down the task of writing a research paper into a series of smaller steps.
```

### Plan-and-Solve Prompting
The model first creates a plan for solving a problem and then executes the plan.
```
Create a plan to solve the following problem, and then execute the plan. ...
```

### Tree-of-Thought (ToT)
The model explores multiple reasoning paths in a tree-like structure.
```
Generate a tree of thoughts to solve the following problem. Each node in the tree should represent a possible step in the solution.
```

### Recursion-of-Thought
The model uses recursion to solve problems that have a recursive structure.
```
Define a recursive function to calculate the factorial of a number.
```

### Program-of-Thoughts
The model generates a program to solve a problem.
```
Write a Python program to solve the following problem. ...
```

### Faithful Chain-of-Thought
The model is prompted to generate a chain of thought that is faithful to the original problem.
```
Make sure that your reasoning process is sound and that you are not making any logical leaps.
```

### Skeleton-of-Thought
The model is given a skeleton of a solution and is prompted to fill in the details.
```
Here is a skeleton of a proof. Fill in the missing steps.
```

### Metacognitive Prompting
The model is prompted to reflect on its own reasoning process.
```
After you have solved the problem, reflect on your reasoning process and identify any areas where you could have improved.
```

# Ensembling

Ensembling techniques combine multiple model outputs to produce a more robust and accurate final answer.

### Demonstration Ensembling
The model is prompted with multiple demonstrations of the same task, and the final answer is an aggregation of the outputs.
```
Here are three different ways to solve the same problem. ... Now, solve the following problem using the method that you think is best.
```

### Mixture of Reasoning Experts
The model is prompted to use different reasoning strategies for different parts of the problem.
```
Use a logical reasoning approach to solve the first part of the problem, and then use a creative reasoning approach to solve the second part of the problem.
```

### Max Mutual Information Method
The model is prompted to generate a response that has the maximum mutual information with the prompt.
```
Generate a response that is both informative and relevant to the prompt.
```

### Self-Consistency
The model generates multiple responses to the same prompt and the most consistent response is chosen.
```
Generate five different responses to the following question, and then choose the one that you think is the most accurate.
```

### Universal Self-Consistency
A variation of self-consistency that uses a universal set of prompts.
```
Generate five different responses to the following question using five different prompts, and then choose the one that you think is the most accurate.
```

### Meta-Reasoning over Multiple CoTs
The model reasons about the quality of multiple chains of thought before selecting the best one.
```
Generate three different chains of thought to solve the following problem, and then explain which one is the most logical and coherent.
```

### DiVeRSe
The model is prompted to generate diverse responses to the same prompt.
```
Generate five different responses to the following question, each with a different perspective.
```

### Consistency-based Self-adaptive Prompting (COSP)
The model adapts its prompting strategy based on the consistency of its previous responses.
```
If the model is consistently making the same mistake, try a different prompting strategy.
```

### Universal Self-Adaptive Prompting (USP)
A variation of COSP that uses a universal set of prompts.
```
If the model is consistently making the same mistake, try a different prompt from a universal set of prompts.
```

### Prompt Paraphrasing
The model is prompted with multiple paraphrases of the same question, and the final answer is an aggregation of the outputs.
```
User: 'What is the capital of France?'
Model: 'What is the main city of France?'
Model: 'What is the most populous city in France?'
```

# Self-Criticism

Self-criticism techniques enable the model to evaluate and improve its own responses.

### Self-Calibration
The model is prompted to assess the confidence of its own predictions.
```
On a scale of 1 to 5, how confident are you in your answer?
```

### Self-Refine
The model is prompted to refine its own responses based on feedback.
```
Your previous answer was incorrect. Please try again and make sure to address the feedback that I have provided.
```

### Reversing Chain-of-Thought (RCoT)
The model is prompted to reverse its chain of thought to identify errors.
```
Start from your conclusion and work backwards to see if you can find any flaws in your reasoning.
```

### Self-Verification
The model is prompted to verify the correctness of its own responses.
```
Please double-check your answer to make sure that it is correct.
```

### Chain-of-Verification (COVE)
The model generates a verification chain to check the correctness of its reasoning steps.
```
For each step in your reasoning process, provide a justification for why it is correct.
```

### Cumulative Reasoning
The model accumulates evidence for its conclusions and uses it to refine its reasoning.
```
As you gather more information, update your conclusion to reflect the new evidence.
```

# Prompt Engineering

Prompt engineering techniques focus on automatically discovering and optimizing effective prompts.

### Meta Prompting
Using a model to generate prompts for another model.
```
Generate a prompt that will encourage a language model to be more creative.
```

### AutoPrompt
An algorithm for automatically generating prompts.
```
Use the AutoPrompt algorithm to generate a prompt for the task of summarizing news articles.
```

### Automatic Prompt Engineer (APE)
A framework for automatically generating and selecting prompts.
```
Use the APE framework to generate and select the best prompt for the task of translating between English and French.
```

### Gradientfree Instructional Prompt Search (GrIPS)
A gradient-free method for searching for effective prompts.
```
Use the GrIPS algorithm to search for the best prompt for the task of answering questions about a given text.
```

### Prompt Optimization with Textual Gradients (ProTeGi)
A method for optimizing prompts using textual gradients.
```
Use the ProTeGi algorithm to optimize the prompt for the task of generating creative text formats.
```

### RLPrompt
A reinforcement learning-based approach to prompt optimization.
```
Use the RLPrompt algorithm to optimize the prompt for the task of playing a text-based adventure game.
```

### Dialogue-comprised Policy-gradient-based Discrete Prompt Optimization (DP2O)
A policy-gradient-based method for optimizing prompts in a dialogue setting.
```
Use the DP2O algorithm to optimize the prompts for a chatbot that is designed to help users with their mental health.
```

# Answer Engineering

Answer engineering techniques focus on constraining the model's output to a specific format or structure.

### Answer Shape
The model is prompted to generate a response that has a specific shape or structure.
```
Please provide your answer in the form of a JSON object with the following keys: 'name', 'age', and 'city'.
```

### Answer Space
The model is prompted to generate a response from a predefined set of possible answers.
```
Please choose one of the following options: A, B, C, or D.
```

### Answer Extractor
A component that extracts the final answer from the model's output.

#### Verbalizer
A component that maps the model's output to a specific set of labels.
```
If the model outputs 'positive', then the label is '1'. If the model outputs 'negative', then the label is '0'.
```

#### Regex
A regular expression that is used to extract the answer from the model's output.
```
Use the following regular expression to extract the phone number from the model's output: ...
```

#### Separate LLM
A separate language model that is used to extract the answer from the model's output.
```
Use a separate language model to extract the key information from the model's output.
```

# Sources

- [The Prompt Report: A Systematic Survey of Prompt Engineering Techniques](https://arxiv.org/pdf/2406.06608)
- [K-Nearest Neighbor (KNN) Prompting](https://learnprompting.org/docs/advanced/few_shot/k_nearest_neighbor_knn)
- [Prompt Engineering Guide](https://www.promptingguide.ai/)
- [Prompt Mining](https://learnprompting.org/docs/advanced/few_shot/prompt_mining)
- [Learning Prompting](https://learnprompting.org/)
