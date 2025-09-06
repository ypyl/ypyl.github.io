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
Improves reliability by generating multiple step-by-step solutions. If most solutions agree, that answer is chosen. If they disagree, the model provides a single, direct answer instead. This uses a multi-path approach when confident and a direct one when uncertain.
```
Let's say we ask a complex question: "If a farmer has 15 cows and sells 7, but 3 of his remaining cows have twins, how many cows does he have?"

**Step 1: Sample Multiple Reasoning Paths (e.g., 5 paths)**

*   **Path 1:** 15 - 7 = 8. 3 cows have twins, so 3 * 2 = 6 new cows. 8 + 6 = 14. **Answer: 14**
*   **Path 2:** Starts with 15. Sells 7, so 15-7=8. 3 cows have twins, so that's 3*2=6 calves. Total is 8+6=14. **Answer: 14**
*   **Path 3:** 15 cows - 7 sold = 8 cows. The remaining cows are 8. 3 of them have twins, so 3 new pairs, which is 6 calves. Total cows: 8 + 6 = 14. **Answer: 14**
*   **Path 4:** 15 - 7 = 8. 3 cows have twins, so that's 3 more cows. 8 + 3 = 11. (Incorrect reasoning). **Answer: 11**
*   **Path 5:** The farmer has 15 cows. He sells 7, so he has 8 left. 3 of these 8 have twins, meaning 3 * 2 = 6 calves are born. The total number of cows is now 8 + 6 = 14. **Answer: 14**

**Step 2: Check for Majority Consensus**

*   The answers are: [14, 14, 14, 11, 14].
*   The answer "14" appears in 4 out of 5 paths (80% consistency).
*   This is above the confidence threshold.

**Step 3: Select Final Answer**

*   Since consistency is high, the majority answer is chosen.
*   **Final Answer: 14**

**(Alternative Scenario: Low Consistency)**

If the answers were [14, 11, 15, 12, 14], there is no clear majority. The system would detect this high uncertainty, discard these results, and generate a single, direct (greedy) answer to the original question.
```

### Complexity-based Prompting
Complexity-based Prompting involves two major modifications to Chain-of-Thought (CoT) prompting. First, it selects complex examples for annotation and inclusion in the prompt, based on factors like question length or reasoning steps required. Second, during inference, it samples multiple reasoning chains (answers) and uses a majority vote among chains exceeding a certain length threshold, under the premise that longer reasoning indicates higher answer quality. This technique has shown improvements on mathematical reasoning datasets.
```
# Example of a complex problem for annotation:
"A train leaves station A at 10:00 AM traveling at 60 mph towards station B, which is 300 miles away. At 11:00 AM, another train leaves station B traveling at 50 mph towards station A. When and where do the two trains meet?"

# During inference, the model might generate multiple reasoning chains.
# Chain 1 (short, potentially incorrect):
# Train 1 travels 60 miles in 1 hour. Remaining distance 240 miles.
# Relative speed 110 mph. 240/110 = 2.18 hours.
# Meeting time: 11:00 AM + 2.18 hours = 1:11 PM.
# Meeting location: 60 + 2.18 * 60 = 190.8 miles from A.

# Chain 2 (longer, more detailed, likely correct):
# 1. Train 1 starts at 10:00 AM. By 11:00 AM, it has traveled 60 mph * 1 hr = 60 miles.
# 2. Remaining distance between trains at 11:00 AM = 300 miles - 60 miles = 240 miles.
# 3. Both trains are now moving towards each other. Their combined speed (relative speed) = 60 mph + 50 mph = 110 mph.
# 4. Time to meet from 11:00 AM = Distance / Relative Speed = 240 miles / 110 mph = 2.1818 hours.
# 5. Convert 0.1818 hours to minutes: 0.1818 * 60 minutes/hour = 10.908 minutes (approx 11 minutes).
# 6. Meeting time = 11:00 AM + 2 hours and 11 minutes = 1:11 PM.
# 7. Distance traveled by Train 1 from 11:00 AM = 60 mph * 2.1818 hours = 130.908 miles.
# 8. Total distance from Station A for Train 1 = 60 miles (initial) + 130.908 miles = 190.908 miles.
# 9. Meeting location: Approximately 190.9 miles from Station A.

# If Chain 2 exceeds a predefined length threshold, it's considered higher quality.
# A majority vote among such longer chains would determine the final answer.
```

### Active Prompting
Active Prompting is a technique that iteratively improves a language model's performance by focusing on uncertain examples. It starts by having the LLM solve a set of training questions or exemplars. Then, it calculates the uncertainty of the LLM's responses (often measured by disagreement among multiple generated answers or reasoning paths). For the exemplars with the highest uncertainty, human annotators are asked to rewrite or refine them. These improved exemplars are then used to further train or fine-tune the LLM, leading to a more robust model over time.
```
# Initial training exemplars:
# Q: "What is the capital of France?" A: "Paris"
# Q: "Who painted the Mona Lisa?" A: "Leonardo da Vinci"
# Q: "What is the largest ocean?" A: "Pacific Ocean"

# LLM is asked to solve new questions:
# Q1: "What is the best way to travel from London to Edinburgh?"
# Q2: "Explain the concept of quantum entanglement."
# Q3: "Summarize the plot of 'Hamlet'."

# LLM generates answers and uncertainty is calculated.
# Let's say Q1 and Q3 have high uncertainty (e.g., multiple conflicting answers, or low confidence scores).

# Human annotators are asked to rewrite/refine Q1 and Q3 to reduce ambiguity or provide clearer context.
# Rewritten Q1: "Considering speed and cost, what is the most efficient way to travel from London to Edinburgh for a single person?"
# Rewritten Q3: "Provide a concise summary of the main plot points and character motivations in Shakespeare's 'Hamlet'."

# These rewritten exemplars are then used to improve the LLM.
```

### Memory-of-Thought Prompting
Memory-of-Thought Prompting leverages unlabeled training exemplars to construct Few-Shot Chain-of-Thought (CoT) prompts dynamically at test time. Before the actual test, the model performs inference on a set of unlabeled training examples using CoT, generating reasoning paths for them. At test time, when a new query is presented, the system retrieves similar instances from this pre-processed set of unlabeled exemplars. These retrieved examples, along with their generated CoT reasoning, are then included in the prompt for the current test sample. This technique has demonstrated substantial improvements in benchmarks across various reasoning tasks, including arithmetic, commonsense, and factual reasoning.
```
# Pre-processing (before test time):
# Unlabeled training exemplar: "What is 123 + 456?"
# LLM generates CoT for it: "1. Add the units digits: 3 + 6 = 9. 2. Add the tens digits: 2 + 5 = 7. 3. Add the hundreds digits: 1 + 4 = 5. Result: 579."

# Unlabeled training exemplar: "Is a tomato a fruit or a vegetable?"
# LLM generates CoT for it: "1. A fruit develops from the flower's ovary and contains seeds. 2. A vegetable is any other edible part of a plant. 3. Tomatoes develop from the flower's ovary and contain seeds. Conclusion: A tomato is a fruit."

# At test time, for a new query: "What is 789 - 123?"
# The system retrieves similar instances from its "memory of thought."
# It might retrieve the "123 + 456" example because it's an arithmetic problem.

# The prompt for the new query would then include the retrieved example and its CoT:
# "Here's an example of how to solve an arithmetic problem:
# Q: What is 123 + 456?
# A: 1. Add the units digits: 3 + 6 = 9. 2. Add the tens digits: 2 + 5 = 7. 3. Add the hundreds digits: 1 + 4 = 5. Result: 579.

# Now, solve the following: What is 789 - 123?"
```

### Automatic Chain-of-Thought (Auto-CoT) Prompting
Automatic Chain-of-Thought (Auto-CoT) Prompting automates the process of generating Chain-of-Thought (CoT) examples. It begins by using a simple Zero-Shot prompt (like "Let's think step by step") to automatically generate reasoning chains for a diverse set of unlabeled questions. These automatically generated question-reasoning-answer triplets are then used to construct a Few-Shot CoT prompt for new, unseen test samples. This approach eliminates the need for manual annotation of CoT examples, making CoT prompting more scalable and accessible.
```
# Step 1: Automatic CoT Generation (using Zero-Shot CoT)
# For an unlabeled question: "Q: If a car travels 100 miles in 2 hours, what is its average speed?"
# LLM generates CoT: "A: Let's think step by step. Speed = Distance / Time. Distance = 100 miles. Time = 2 hours. Speed = 100 / 2 = 50 mph. The average speed is 50 mph."

# For another unlabeled question: "Q: Is the sun a planet?"
# LLM generates CoT: "A: Let's think step by step. A planet orbits a star and is large enough to be rounded by its own gravity. The sun is a star, not orbiting another star, and generates its own light and heat. Therefore, the sun is not a planet."

# Step 2: Constructing Few-Shot CoT Prompt for a new test sample
# New test question: "Q: What is the capital of Japan?"

# The Auto-CoT system would select relevant automatically generated CoT examples (e.g., the "Sun is a planet" example for factual reasoning) and include them in the prompt:
# "Q: If a car travels 100 miles in 2 hours, what is its average speed?
# A: Let's think step by step. Speed = Distance / Time. Distance = 100 miles. Time = 2 hours. Speed = 100 / 2 = 50 mph. The average speed is 50 mph.

# Q: Is the sun a planet?
# A: Let's think step by step. A planet orbits a star and is large enough to be rounded by its own gravity. The sun is a star, not orbiting another star, and generates its own light and heat. Therefore, the sun is not a planet.

# Q: What is the capital of Japan?
# A: Let's think step by step."
```

# Decomposition

Decomposition techniques break down complex problems into smaller, more manageable subproblems.

### Least-to-Most Prompting
This technique first breaks a big problem into smaller, easier-to-solve pieces. Then, it solves each small piece one by one, using the answer from the previous piece to help with the next one. It’s like building with LEGOs: you start with the simplest part and add on from there.
```
# Original Problem: "Calculate the total cost of a meal if a pizza costs $15, a drink costs $3, and you want to buy 2 pizzas and 3 drinks, with a 10% discount on the total."

# Step 1: Decompose the problem into sub-problems (without solving yet)
# Prompt: "Break down the following problem into a sequence of smaller, solvable steps: 'Calculate the total cost of a meal if a pizza costs $15, a drink costs $3, and you want to buy 2 pizzas and 3 drinks, with a 10% discount on the total.'"
# LLM Response (sub-problems):
# 1. Calculate the cost of pizzas.
# 2. Calculate the cost of drinks.
# 3. Calculate the subtotal before discount.
# 4. Calculate the discount amount.
# 5. Calculate the final total after discount.

# Step 2: Solve sub-problems sequentially, appending results to the prompt
# Prompt (for sub-problem 1): "A pizza costs $15. Calculate the cost of 2 pizzas."
# LLM Response: "Cost of pizzas = 2 * $15 = $30."

# Prompt (for sub-problem 2, with previous result): "Cost of pizzas = $30. A drink costs $3. Calculate the cost of 3 drinks."
# LLM Response: "Cost of drinks = 3 * $3 = $9."

# Prompt (for sub-problem 3, with previous results): "Cost of pizzas = $30. Cost of drinks = $9. Calculate the subtotal before discount."
# LLM Response: "Subtotal = $30 + $9 = $39."

# Prompt (for sub-problem 4, with previous results): "Subtotal = $39. Calculate a 10% discount on the subtotal."
# LLM Response: "Discount amount = 0.10 * $39 = $3.90."

# Prompt (for sub-problem 5, with previous results): "Subtotal = $39. Discount amount = $3.90. Calculate the final total after discount."
# LLM Response: "Final total = $39 - $3.90 = $35.10."

# Final Answer: $35.10
```

### Decomposed Prompting
This technique teaches the model how to use external tools to solve problems. It breaks a complex problem into smaller pieces and then uses the right tool for each piece, like using a search engine to find information or a calculator to do math. This helps the model handle tasks that require more than just generating text.
```
# Example of Few-Shot prompt demonstrating function usage:
# User: "Search the web for 'current weather in London' and extract the temperature."
# LLM (internal thought process, guided by prompt):
# Use function: search_web(query="current weather in London")
# Use function: extract_temperature(text=search_results)

# Original Problem: "Find the capital of the country with the highest population in South America."

# LLM's decomposition and function calls (guided by Decomposed Prompting):
# 1. Identify the function to get population data: get_country_populations(continent="South America")
# 2. Identify the function to find the country with the highest population: find_highest_population_country(data=country_populations)
# 3. Identify the function to get the capital of a country: get_capital(country=highest_population_country)

# Execution (simulated):
# Call: get_country_populations(continent="South America") -> Returns: {"Brazil": 215M, "Colombia": 52M, ...}
# Call: find_highest_population_country(data={"Brazil": 215M, "Colombia": 52M, ...}) -> Returns: "Brazil"
# Call: get_capital(country="Brazil") -> Returns: "Brasília"

# Final Answer: "Brasília"
```

### Plan-and-Solve Prompting
This technique tells the model to first make a plan and then solve the problem. It’s like telling someone, "First, figure out the steps, and then follow them." This makes the model's reasoning clearer and more reliable, especially for difficult tasks.
```
# Original Problem: "I have a rectangular garden that is 10 meters long and 5 meters wide. I want to build a fence around it, but I need to leave a 1-meter wide gate on one of the longer sides. If fencing costs $12 per meter, what will be the total cost of the fence?"

# Plan-and-Solve Prompt:
# "Let’s first understand the problem and devise a plan to solve it. Then, let’s carry out the plan and solve the problem step by step.
# Problem: Calculate the total cost of fencing a rectangular garden (10m long, 5m wide) with a 1m gate on a longer side, if fencing costs $12/meter.

# Plan:
# 1. Calculate the perimeter of the garden.
# 2. Subtract the length of the gate from the perimeter to find the length of fencing needed.
# 3. Multiply the length of fencing needed by the cost per meter to find the total cost.

# Execution:
# Step 1: Calculate the perimeter of the garden.
# Perimeter = 2 * (length + width) = 2 * (10m + 5m) = 2 * 15m = 30m.

# Step 2: Subtract the length of the gate from the perimeter.
# Length of fencing needed = Perimeter - gate length = 30m - 1m = 29m.

# Step 3: Multiply the length of fencing needed by the cost per meter.
# Total cost = 29m * $12/meter = $348.

# Final Answer: $348
```

### Tree-of-Thought (ToT)
Instead of just one straight line of reasoning, this method explores multiple different reasoning paths at the same time, like branches of a tree. The model generates many different "thoughts" or ideas on how to proceed, checks how good each one is, and then focuses on the most promising branches. This is great for complex problems where you need to explore different possibilities to find the best solution.
```
# Original Problem: "Design a simple, healthy, and appealing 3-course meal for a vegetarian guest."

# ToT Process (simplified):

# Initial Thought (Root Node): "What are the main components of a 3-course meal?"
#   - Appetizer
#   - Main Course
#   - Dessert

# Branch 1: Appetizer Ideas (Vegetarian, Healthy, Appealing)
#   - Thought 1.1: "Soup"
#     - Evaluation: Good for healthy, but might be too heavy.
#   - Thought 1.2: "Salad"
#     - Evaluation: Good for healthy, can be appealing.
#   - Thought 1.3: "Small bites (e.g., bruschetta)"
#     - Evaluation: Appealing, but might not be healthy enough.

# Branch 2: Main Course Ideas (Vegetarian, Healthy, Appealing)
#   - Thought 2.1: "Pasta dish"
#     - Evaluation: Can be appealing, but often not very healthy.
#   - Thought 2.2: "Lentil/Bean based dish"
#     - Evaluation: Healthy, can be made appealing.
#   - Thought 2.3: "Vegetable curry"
#     - Evaluation: Appealing, can be healthy.

# Branch 3: Dessert Ideas (Vegetarian, Healthy, Appealing)
#   - Thought 3.1: "Fruit salad"
#     - Evaluation: Very healthy, simple, appealing.
#   - Thought 3.2: "Sorbet"
#     - Evaluation: Healthy, refreshing, appealing.
#   - Thought 3.3: "Baked apple"
#     - Evaluation: Healthy, comforting.

# Selecting promising paths and combining:
# (Example of a selected path leading to a solution)
# Appetizer: Caprese Salad (healthy, appealing, light)
# Main Course: Lentil Shepherd's Pie with root vegetable mash (healthy, hearty, appealing)
# Dessert: Mixed Berry Sorbet (healthy, refreshing, appealing)

# Final Meal Plan:
# Appetizer: Caprese Salad
# Main Course: Lentil Shepherd's Pie with Root Vegetable Mash
# Dessert: Mixed Berry Sorbet
```

### Recursion-of-Thought
This is like Chain-of-Thought, but when the model hits a tricky part, it stops and creates a new, smaller task to solve just that part. Once it gets the answer to the smaller task, it plugs it back into the main problem and keeps going. This helps the model solve more complex problems step-by-step.
```
# Example: Calculate the value of f(3) where f(n) = n * f(n-1), and f(1) = 1.

# Step 1: The model starts solving f(3):
To calculate f(3), I need to compute 3 * f(2).

# Step 2: f(2) is a sub-problem, so the model recursively prompts itself:
To calculate f(2), I need to compute 2 * f(1).

# Step 3: f(1) is a base case:
f(1) = 1.

# Step 4: Insert the answer for f(1) back into the previous step:
f(2) = 2 * 1 = 2.

# Step 5: Insert the answer for f(2) back into the original problem:
f(3) = 3 * 2 = 6.

# Final Answer: 6
```

### Program-of-Thoughts
Program-of-Thoughts uses LLMs like Codex to generate programming code as reasoning steps. A code interpreter executes these steps to obtain the final answer. It excels in athematical and programming-related tasks but is less effective for semantic reasoning tasks.
```
Write a Python program to solve the following problem: "Calculate the factorial of a number."
```

### Faithful Chain-of-Thought
This technique combines natural language reasoning with symbolic language (like Python code) in a single chain of thought, adapting the symbolic language to the task.
```
Question: Olivia has $23. She bought five bagels for $3 each. How much money does she have left?

Let's think step by step.
First, let's calculate the total cost of the bagels.
---python
bagel_cost = 3
num_bagels = 5
total_cost = bagel_cost * num_bagels
---
So, the total cost is $15.
Now, let's subtract the total cost from the initial amount of money Olivia had.
---python
initial_money = 23
money_left = initial_money - total_cost
---
So, Olivia has $8 left.
The final answer is 8.
```

### Skeleton-of-Thought
This technique speeds up the model's response time by breaking a problem down into smaller parts and solving them simultaneously. First, the model creates an outline or "skeleton" of the answer, which consists of several sub-problems. Then, it solves all these sub-problems in parallel and combines the results to give a final answer.
```
# Original Problem: "Write a blog post about the benefits of exercise."

# Step 1: The model creates a skeleton of the blog post.
# Prompt: "Create a skeleton for a blog post about the benefits of exercise."
# LLM Response (Skeleton):
# - Introduction: Briefly introduce the importance of exercise.
# - Body Paragraph 1: Discuss the physical health benefits (e.g., weight management, reduced risk of chronic diseases).
# - Body Paragraph 2: Discuss the mental health benefits (e.g., stress reduction, improved mood).
# - Conclusion: Summarize the key benefits and encourage readers to start exercising.

# Step 2: The model expands on each part of the skeleton in parallel.
# Prompt 1 (for Introduction): "Write a brief introduction about the importance of exercise."
# Prompt 2 (for Body Paragraph 1): "Write a paragraph about the physical health benefits of exercise, such as weight management and reduced risk of chronic diseases."
# Prompt 3 (for Body Paragraph 2): "Write a paragraph about the mental health benefits of exercise, such as stress reduction and improved mood."
# Prompt 4 (for Conclusion): "Write a conclusion summarizing the key benefits of exercise and encouraging readers to start."

# Step 3: The model combines the responses to create the final blog post.
# (The combined text from the parallel calls)
```

### Metacognitive Prompting
This technique guides the model to "think about its own thinking" by following a structured, five-step process that mimics human problem-solving. This includes clarifying the question, making an initial guess, evaluating its own answer, confirming its final decision, and stating its confidence level. This helps the model produce more reasoned and reliable outputs.
```
# Original Problem: "Is a tomato a fruit or a vegetable?"

# Metacognitive Prompting (Five-Step Chain):

# Step 1: Clarify the Question
# Prompt: "First, clarify the question: 'Is a tomato a fruit or a vegetable?' What are the key terms and what is being asked?"
# LLM Response: "The question asks for the classification of a tomato. The key terms are 'tomato,' 'fruit,' and 'vegetable.' I need to determine which category a tomato belongs to, considering both botanical and culinary definitions."

# Step 2: Preliminary Judgment
# Prompt: "Based on your initial understanding, what is your preliminary judgment?"
# LLM Response: "My preliminary judgment is that a tomato is botanically a fruit but is commonly used as a vegetable in cooking."

# Step 3: Evaluate the Response
# Prompt: "Now, evaluate your preliminary judgment. What are the arguments for and against it? Are there any ambiguities?"
# LLM Response: "The botanical definition of a fruit is a mature ovary of a flowering plant, enclosing the seed or seeds. Tomatoes fit this definition. The culinary definition of a vegetable is based on how it's used in meals, typically savory dishes. This is why the confusion exists."

# Step 4: Decision Confirmation
# Prompt: "Based on your evaluation, confirm your final decision and provide a clear answer."
# LLM Response: "My final decision is that a tomato is technically a fruit from a botanical standpoint. However, it is used as a vegetable in a culinary context. The most accurate answer depends on the context."

# Step 5: Confidence Assessment
# Prompt: "On a scale of 1 to 10, how confident are you in your final answer?"
# LLM Response: "I am 10/10 confident in my answer, as it addresses the different contexts in which the question can be interpreted."
```

# Ensembling

Ensembling techniques combine multiple model outputs to produce a more robust and accurate final answer.

### Demonstration Ensembling (DENSE)
Demonstration Ensembling (DENSE) creates multiple few-shot prompts, each containing a distinct subset of exemplars from the training set. Next, it aggregates over their outputs to generate a final response.
```
# Task: Classify the sentiment of the following sentence: "The movie was okay, but not great."

# Prompt 1 (Exemplar Set A)
# "I loved the movie." (Positive)
# "The film was a disaster." (Negative)
# Classify: "The movie was okay, but not great." -> Neutral

# Prompt 2 (Exemplar Set B)
# "An amazing experience!" (Positive)
# "I wouldn't recommend it." (Negative)
# Classify: "The movie was okay, but not great." -> Neutral

# Prompt 3 (Exemplar Set C)
# "Simply brilliant." (Positive)
# "A total waste of time." (Negative)
# Classify: "The movie was okay, but not great." -> Neutral

# Aggregated Final Answer: Neutral (based on majority vote)
```

### Mixture of Reasoning Experts
Mixture of Reasoning Experts (MoRE) creates a set of diverse "reasoning experts" by using different specialized prompts for different types of reasoning. For example, it might use a retrieval-augmented prompt for factual questions, a Chain-of-Thought prompt for math problems, and a generated knowledge prompt for commonsense reasoning. The final answer is selected by choosing the best response from all the experts, often based on an agreement score.
```
# Task: "The 2016 Summer Olympics were held in Rio de Janeiro. What is the population of that country's capital, and what is a typical souvenir someone might buy there?"

# MoRE dispatches the query to different "experts" with specialized prompts.

# Expert 1: Factual Reasoning (Chain-of-Thought)
# Prompt: "Let's think step by step. The 2016 Summer Olympics were in Rio de Janeiro. Rio is in Brazil. The capital of Brazil is Brasília. What is the population of Brasília?"
# -> Generates an answer for the population part.

# Expert 2: Factual Reasoning (Retrieval Augmented)
# Prompt: "Search: population of Brasília"
# -> Generates another answer for the population part.

# Expert 3: Commonsense Reasoning (Generated Knowledge)
# Prompt: "What are typical souvenirs to buy in Brazil?"
# -> Generates an answer for the souvenir part.

# The system then synthesizes these outputs. It might find that Expert 1 and 2 agree on the population, and uses the answer from Expert 3 for the second part of the question.

# Final Aggregated Answer: "The capital of Brazil is Brasília, which has a population of approximately 3 million people. A typical souvenir to buy in Brazil is a figurine of Christ the Redeemer."
```

### Max Mutual Information Method
The Max Mutual Information (MMI) method is a technique for selecting the best prompt template from a set of candidates. It works by creating multiple prompt templates with different styles or examples. The best template is chosen by finding the one that maximizes the mutual information between the prompt and the model's output, ensuring the generated response is highly relevant and informative with respect to the prompt itself.
```
# Task: Find the best prompt for summarizing news articles.

# Step 1: Create multiple candidate prompt templates.
# Template A: "Summarize this: [Article]"
# Template B: "Write a short summary of the following text: [Article]"
# Template C: "You are a skilled news editor. Condense the following article into its essential points for a busy reader. Article: [Article]"

# Step 2: Use each template to generate summaries for a set of test articles.

# Step 3: Calculate the mutual information between the prompt+article and the generated summary for each template.
# The MMI calculation measures how much information the prompt and the output share. A high score means the output is highly dependent on and relevant to the prompt.

# Let's assume Template C consistently produces summaries that are very focused and directly reflect the source articles' key information, resulting in the highest MMI score.

# Step 4: Select the optimal prompt.
# Selected Prompt: "You are a skilled news editor. Condense the following article into its essential points for a busy reader. Article: [Article]"
```

### Self-Consistency
Self-Consistency improves the reliability of answers by generating multiple diverse reasoning paths for the same problem and then choosing the most consistent answer. It prompts a model multiple times using a Chain-of-Thought (CoT) approach with some randomness (non-zero temperature) to get different lines of reasoning. The final answer is determined by a majority vote among all the generated responses, based on the idea that a correct answer is more likely to be reached via multiple different logical paths.
```
# Question: "A classroom has 15 boys and 12 girls. 5 students leave the room. If 2 of the students who left were boys, how many boys are left in the classroom?"

# Step 1: Generate multiple reasoning paths with a non-zero temperature.

# Path 1:
# "There are 15 boys initially. 5 students leave, and 2 of them are boys. So, the number of boys who left is 2. The number of boys remaining is 15 - 2 = 13. The answer is 13."

# Path 2:
# "Start with 15 boys. 2 boys leave. So, 15 - 2 = 13. The information about the girls and the total number of students leaving is extra. There are 13 boys left. The answer is 13."

# Path 3 (Slightly convoluted but correct):
# "15 boys and 12 girls is 27 students. 5 leave, so 22 remain. If 2 were boys, then 3 were girls. So, 15 - 2 = 13 boys and 12 - 3 = 9 girls are left. The number of boys is 13. The answer is 13."

# Path 4 (Incorrect):
# "There are 15 boys. 5 students leave. So, 15 - 5 = 10 boys are left. The answer is 10."

# Step 2: Aggregate the answers and take a majority vote.
# Answers generated: [13, 13, 13, 10]

# Step 3: Select the most consistent answer.
# The answer "13" is the majority.

# Final Answer: 13
```

### Universal Self-Consistency
Universal Self-Consistency is an extension of Self-Consistency designed for tasks where answers might be expressed in different ways (like summarization or free-form text generation). Like Self-Consistency, it generates multiple diverse reasoning paths for a problem. However, instead of programmatically counting the identical answers to find the majority, it bundles all the generated outputs into a new prompt and asks the language model itself to determine the most common or correct answer among them. This allows it to find a consensus even when the answers are phrased differently.
```
# Question: "Briefly, what is the primary benefit of using solar power?"

# Step 1: Generate multiple diverse responses to the question.

# Response A:
# "The main benefit of solar power is that it is a renewable energy source, meaning it does not deplete natural resources."

# Response B:
# "Solar power's key advantage is its sustainability, as it harnesses sunlight, which is an inexhaustible resource."

# Response C:
# "Using solar power helps reduce carbon emissions because it doesn't burn fossil fuels, making it environmentally friendly."

# Response D:
# "The primary benefit is that solar energy is renewable and sustainable."

# Step 2: Instead of a simple programmatic vote (which might see these as different answers), feed all responses into a new prompt for the LLM to find the consensus.

# Prompt for the LLM:
# "The following are several answers to the question 'What is the primary benefit of using solar power?'. Please identify the most common and correct underlying idea among them.
#
# Answers:
# 1. The main benefit of solar power is that it is a renewable energy source, meaning it does not deplete natural resources.
# 2. Solar power's key advantage is its sustainability, as it harnesses sunlight, which is an inexhaustible resource.
# 3. Using solar power helps reduce carbon emissions because it doesn't burn fossil fuels, making it environmentally friendly.
# 4. The primary benefit is that solar energy is renewable and sustainable.
#
# The most common idea is:"

# Step 3: The LLM analyzes the responses and provides the final, consolidated answer.

# Final Answer (from the LLM):
# "The most common idea is that solar power is a renewable and sustainable energy source that is environmentally friendly."
```

### Meta-Reasoning over Multiple CoTs
Meta-Reasoning over Multiple Chains of Thought (CoTs) is a technique where the model first generates several different reasoning paths for a given problem. Then, in a second step, the model is presented with all of these initial reasoning chains and is prompted to synthesize them, critique them, or use them as evidence to produce a single, final, and more robust answer. This encourages the model to reflect on different approaches before committing to a conclusion.
```
# Question: "A company needs to ship a fragile, high-value item from Paris to Tokyo. It must arrive within 3 days. What is the best shipping method, considering speed, safety, and cost?"

# Step 1: Generate multiple diverse reasoning chains (CoTs).

# Chain A (Focus on Speed):
# "The fastest way to get a package from Paris to Tokyo is by air freight. Express air services can deliver in 1-3 days. This meets the time limit. High-value items are frequently shipped by air, but special handling for fragile items would be essential and could be costly."

# Chain B (Focus on Safety):
# "For a fragile, high-value item, a dedicated courier service that specializes in secure logistics is the safest. They offer door-to-door, tracked, and insured shipping. This is likely the most secure option, and they would use air travel to meet the 3-day deadline, but it will be very expensive."

# Chain C (Focus on Cost):
# "Standard international shipping would be the cheapest, but it's too slow, often taking weeks. Therefore, cost must be secondary to speed and safety. The choice is between standard air freight and a specialized courier. The courier is safer but pricier; standard air freight is a balance."

# Step 2: Feed all reasoning chains into a new prompt for the LLM to perform meta-reasoning and generate a final answer.

# Prompt for the LLM:
# "Given the following lines of reasoning, synthesize them to provide a final recommendation for the best shipping method.
#
# Reasoning 1: Air freight is the fastest and meets the deadline, but safety for fragile items is a concern.
# Reasoning 2: A specialized courier is the safest and meets the deadline, but is the most expensive.
# Reasoning 3: Standard shipping is too slow, so the choice is between air freight and a courier, balancing cost and safety.
#
# Based on this, what is the best recommendation?"

# Step 3: The LLM synthesizes the chains to provide a final, reasoned answer.

# Final Answer (from the LLM):
# "The best shipping method is using a specialized courier service. Although it is the most expensive option, it is the only one that guarantees both the speed (delivery within 3 days via air) and the necessary safety for a fragile, high-value item. Standard air freight presents an unnecessary risk for such an item."
```

### DiVeRSe
The DiVeRSe (Diverse Verifier on Reasoning Steps) technique aims to find the best possible answer by exploring a wide variety of prompts and reasoning paths. First, it generates several different prompts for the same problem. Then, for each of these diverse prompts, it generates multiple reasoning paths (similar to Self-Consistency). Finally, it scores all the generated reasoning paths, often by examining the quality of each individual step, and selects the highest-scoring path to produce the final answer.
```
# Task: "Explain why the sky is blue."

# Step 1: Create multiple, diverse prompts for the same task.

# Prompt 1 (Simple):
# "Why is the sky blue?"

# Prompt 2 (Persona-based):
# "You are a physicist explaining to a high school student why the sky appears blue. Break it down step-by-step."

# Prompt 3 (Technical):
# "Describe the process of Rayleigh scattering and how it causes the diffusion of sunlight in the atmosphere, leading to the perception of a blue sky."

# Step 2: For each prompt, generate multiple reasoning paths (Self-Consistency).
# (Simplified for example - imagine 2 paths are generated for each prompt)

# Paths from Prompt 1:
#   - Path 1.1: "The sky is blue because of the ocean's reflection." (Incorrect)
#   - Path 1.2: "It's because of how the atmosphere scatters light from the sun." (Correct but simple)
# Paths from Prompt 2:
#   - Path 2.1: "Sunlight hits our atmosphere. The atmosphere scatters blue light more than other colors because blue waves are shorter. So, we see blue everywhere." (Good, simplified explanation)
#   - Path 2.2: "Imagine sunlight as a mix of all colors. When it enters the air, the air particles act like tiny mirrors that bounce the light around. They are better at bouncing blue light. That's why you see blue." (Good analogy)
# Paths from Prompt 3:
#   - Path 3.1: "Sunlight is composed of different wavelengths. Shorter wavelengths, like blue and violet light, are scattered more effectively by the nitrogen and oxygen molecules in the atmosphere. Our eyes are more sensitive to blue, so we perceive the sky as blue." (Detailed and accurate)
#   - Path 3.2: "Rayleigh scattering dictates that scattering is inversely proportional to the fourth power of the wavelength. This causes the short-wavelength blue light to scatter much more than long-wavelength red light, filling the sky with blue." (Very technical and accurate)

# Step 3: A verifier scores each of the 6 reasoning paths based on correctness, detail, and logical flow.
#   - Path 1.1 gets a very low score.
#   - Path 1.2 gets a medium score.
#   - Paths 2.1 and 2.2 get high scores.
#   - Paths 3.1 and 3.2 get the highest scores for technical accuracy and detail.

# Step 4: Select the highest-scoring reasoning path to form the final answer.

# Final Answer (based on Path 3.1):
# "The sky appears blue due to a phenomenon called Rayleigh scattering. When sunlight, which is made up of a spectrum of colors, enters Earth's atmosphere, the small molecules of nitrogen and oxygen scatter the shorter-wavelength light (like blue and violet) more effectively than the longer-wavelength light (like red and orange). Because our eyes are more sensitive to blue light, we perceive the sky as blue."
```

### Consistency-based Self-adaptive Prompting (COSP)
Consistency-based Self-adaptive Prompting (COSP) is a method for automatically creating effective Few-Shot Chain-of-Thought (CoT) prompts. It starts by taking a set of unlabeled example problems and generating multiple answers for each one using a simple Zero-Shot CoT approach. It then identifies the examples where the generated answers were most consistent (had high agreement). These high-quality, consistently-answered examples are then used to build a powerful Few-Shot prompt, which is then used to solve the actual target problem, often with another layer of Self-Consistency.
```
# Goal: Create a high-quality prompt to solve a complex math word problem.

# Step 1: Start with a set of unlabeled math problems.
#   - Problem A: "If a train travels 50 km in 30 minutes, what is its speed in km/h?"
#   - Problem B: "A bat and ball cost $1.10. The bat costs $1.00 more than the ball. How much does the ball cost?"
#   - Problem C: "What is the 15th number in the Fibonacci sequence?" (This one might produce inconsistent answers)

# Step 2: Run Zero-Shot CoT with Self-Consistency on each unlabeled problem.
#   - For Problem A, the model consistently answers "100 km/h". -> High Agreement.
#   - For Problem B, the model consistently answers "$0.05" (or 5 cents). -> High Agreement.
#   - For Problem C, the model's answers might vary (e.g., 610, 377, 987). -> Low Agreement.

# Step 3: Select the problems that had high-agreement answers to become exemplars.
#   - Problem A and B are selected. Problem C is discarded.

# Step 4: Construct the final Few-Shot CoT prompt using the selected exemplars and their reasoning.
#   (This prompt will be used to solve the actual target problem)

# --- Start of Final Prompt ---
# Q: If a train travels 50 km in 30 minutes, what is its speed in km/h?
# A: Let's think step by step. 30 minutes is 0.5 hours. Speed = Distance / Time. Speed = 50 km / 0.5 h = 100 km/h. The answer is 100.

# Q: A bat and ball cost $1.10. The bat costs $1.00 more than the ball. How much does the ball cost?
# A: Let's think step by step. Let the cost of the ball be x. The cost of the bat is x + $1.00. The total cost is x + (x + $1.00) = $1.10. So, 2x + 1 = 1.10. 2x = 0.10. x = 0.05. The ball costs $0.05. The answer is 0.05.

# Q: [The *actual* complex math problem goes here]
# A: Let's think step by step.
# --- End of Final Prompt ---

# Step 5: Use this new prompt (often with Self-Consistency again) to get a robust final answer for the target problem.
```

### Universal Self-Adaptive Prompting (USP)
Universal Self-Adaptive Prompting (USP) is a general-purpose evolution of COSP that automatically constructs Few-Shot prompts. Like COSP, it uses a set of unlabeled data to generate potential examples (exemplars) for the prompt. However, USP uses a more sophisticated and complex scoring function to evaluate and select the best exemplars to include in the final prompt. Unlike COSP, this method does not rely on Self-Consistency for generating or using the prompt, aiming for a more broadly applicable and efficient process.
```
# Goal: Automatically create a robust Few-Shot prompt for a text classification task (e.g., classifying news headlines into "Sports", "Technology", or "Politics").

# Step 1: Start with a large set of unlabeled news headlines.
#   - Headline A: "Local Team Wins Championship in Overtime Thriller"
#   - Headline B: "New Smartphone With Foldable Screen Announced"
#   - Headline C: "Parliament Passes New Environmental Bill"
#   - Headline D: "Tech Giant Unveils Breakthrough AI Chip"

# Step 2: For each headline, generate a candidate reasoning path and classification.
#   - For A: `Reasoning: "Championship" and "Overtime Thriller" are sports terms. -> Classification: Sports`
#   - For B: `Reasoning: "Smartphone" and "Foldable Screen" relate to consumer electronics. -> Classification: Technology`
#   - For C: `Reasoning: "Parliament" and "Bill" are political terms. -> Classification: Politics`
#   - For D: `Reasoning: "Tech Giant" and "AI Chip" are technology-related. -> Classification: Technology`

# Step 3: A sophisticated scoring function evaluates each generated exemplar.
# It might rate an exemplar higher if the model's confidence was high, the reasoning clearly links keywords to the category, and the example is a clear-cut case.
#   - Exemplar A (Sports) -> Score: 0.95
#   - Exemplar B (Technology) -> Score: 0.89
#   - Exemplar C (Politics) -> Score: 0.92
#   - Exemplar D (Technology) -> Score: 0.98 (Maybe "AI Chip" is a stronger signal than "Smartphone")

# Step 4: Select the highest-scoring, most diverse exemplars to build the prompt.
# Let's say the system selects A, C, and D to get high-quality, diverse examples.

# Step 5: Construct the final Few-Shot prompt for the target task.
# --- Start of Final Prompt ---
# Q: "Local Team Wins Championship in Overtime Thriller"
# A: Sports
#
# Q: "Parliament Passes New Environmental Bill"
# A: Politics
#
# Q: "Tech Giant Unveils Breakthrough AI Chip"
# A: Technology
#
# Q: [The *actual* headline to be classified goes here]
# A:
# --- End of Final Prompt ---
```

### Prompt Paraphrasing
Prompt Paraphrasing is a data augmentation technique used to improve the robustness of an ensemble model. It involves creating several versions of a single prompt by rephrasing it in different ways while keeping the core meaning the same. The model is then run on each of these paraphrased prompts, and the final answer is determined by aggregating the outputs, for example, by a majority vote. This helps reduce the model's sensitivity to specific wording.
```
# Goal: Get a robust answer for the capital of Australia.

# Step 1: Start with an initial prompt and generate paraphrases of it.

# Original Prompt:
# "What is the capital of Australia?"

# Paraphrased Prompts:
# - Prompt A: "Which city is the capital of Australia?"
# - Prompt B: "Australia's capital city is?"
# - Prompt C: "What is the seat of government in Australia?"

# Step 2: Run each prompt through the language model to get an answer.

#   - Output from Prompt A: "Canberra is the capital of Australia."
#   - Output from Prompt B: "The capital city of Australia is Canberra."
#   - Output from Prompt C: "The seat of government in Australia is Canberra."

# Step 3: Aggregate the outputs to get the final answer.
# A majority vote or consensus shows that the answer is consistently "Canberra".

# Final Answer: "Canberra"
```

# Self-Criticism

Self-criticism techniques enable the model to evaluate and improve its own responses.

### Self-Calibration
Self-Calibration is a technique used to gauge a model's confidence in its own answers. It involves a two-step process: first, the model is prompted to answer a question. Second, a new prompt is constructed that includes the original question, the generated answer, and a follow-up instruction asking the model to verify if its initial answer is correct. This helps in deciding whether to trust the model's output or if further revision is needed.
```
# Goal: Gauge the model's confidence in its answer to a factual question.

# Step 1: Ask the initial question and get an answer.

# Prompt:
# "What is the boiling point of water on top of Mount Everest?"

# Model's Initial Answer:
# "The boiling point of water on top of Mount Everest is about 71°C (160°F)."

# Step 2: Create a new prompt to ask the model to verify its own answer.

# Calibration Prompt:
# "Regarding the question: 'What is the boiling point of water on top of Mount Everest?', the model provided the answer: 'The boiling point of water on top of Mount Everest is about 71°C (160°F).'
#
# Is this answer correct? Please answer with 'Yes' or 'No' and provide a brief explanation."

# Step 3: The model assesses its own answer.

# Model's Calibration Response:
# "Yes. At sea level, water boils at 100°C (212°F). However, on top of Mount Everest, the atmospheric pressure is much lower. This lower pressure means that water molecules need less energy to escape as vapor, resulting in a lower boiling point of approximately 71°C."

# The detailed and correct explanation indicates high confidence in the original answer.
```

### Self-Refine
Self-Refine is an iterative framework where a language model improves its own work without external input. The process starts with an initial response from the model. Then, the model is prompted to generate feedback on its own response, effectively acting as its own critic. Finally, it's prompted to refine the initial response based on the feedback it just provided. This "feedback and refine" loop can be repeated multiple times until a stopping condition is met, such as reaching a maximum number of iterations or the model indicating no further changes are needed.
```
# Task: "Write a short, engaging paragraph about the benefits of reading."

# --- Iteration 1 ---

# 1. Generate Initial Draft
# Model's Draft:
# "Reading is good for you. It can help you learn new things and it is a fun hobby. People should read more books to get smarter."

# 2. Generate Feedback
# Prompt: "Critique the following paragraph. Is it engaging? Is it specific?"
# Model's Feedback:
# "The paragraph is very generic. It uses simple phrases like 'good for you' and 'fun hobby'. It doesn't provide specific examples of benefits."

# 3. Refine Based on Feedback
# Prompt: "Based on the feedback that the paragraph is generic and not specific, rewrite it."
# Model's Refined Draft 1:
# "Reading is a powerful tool for personal growth. It expands your vocabulary, transports you to different worlds, and deepens your understanding of complex topics. By engaging with new ideas on the page, you can sharpen your critical thinking skills."

# --- Iteration 2 ---

# 1. Generate Feedback on Refined Draft 1
# Prompt: "Critique the following paragraph. Is it engaging? Is it specific?"
# Model's Feedback:
# "This version is much better and more specific. It lists clear benefits like vocabulary expansion and critical thinking. To be even more engaging, it could use a more evocative opening."

# 2. Refine Based on Feedback
# Prompt: "Based on the feedback that the opening could be more evocative, rewrite the paragraph."
# Model's Refined Draft 2:
# "Unlock new worlds and sharpen your mind with the simple act of reading. More than just a hobby, reading expands your vocabulary, transports you to distant lands, and deepens your understanding of complex topics. Each page you turn is an opportunity to engage with new ideas and hone your critical thinking skills."

# The process can stop here as the quality has significantly improved.
```

### Reversing Chain-of-Thought (RCoT)
Reversing Chain-of-Thought (RCoT) is a method for checking and correcting an answer by working backward. After a model generates an answer to a problem, it's prompted to reconstruct the original problem based only on that answer. Then, the model compares its reconstructed problem with the actual original problem to find any inconsistencies. These inconsistencies are then used as feedback to revise and improve the initial answer.
```
# Original Problem:
# "A bakery has 10 chocolate croissants and 8 almond croissants. A customer buys 5 chocolate croissants and 2 almond croissants. How many total croissants are left?"

# --- RCoT Process ---

# 1. Generate Initial Answer
# Model's Answer:
# "There are 5 croissants left."
# (This is a common mistake, only subtracting the chocolate croissants from the initial chocolate croissant count).

# 2. Reconstruct the Problem from the Answer
# Prompt: "The answer is 'There are 5 croissants left.' Based on this answer, what was the original problem?"
# Model's Reconstructed Problem:
# "A bakery had 10 croissants and a customer bought 5. How many are left?"

# 3. Compare Original and Reconstructed Problems
# Prompt: "Compare the original problem with the reconstructed one. Original: 'A bakery has 10 chocolate croissants and 8 almond croissants. A customer buys 5 chocolate croissants and 2 almond croissants. How many total croissants are left?' Reconstructed: 'A bakery had 10 croissants and a customer bought 5. How many are left?'"
# Model's Comparison:
# "The original problem mentioned two types of croissants (chocolate and almond) and two different purchase amounts. The reconstructed problem only mentions one type of croissant and one purchase amount. The original also specified the final count should be the 'total' number of croissants."

# 4. Revise the Answer Based on Inconsistencies
# Prompt: "Your initial answer was likely wrong because you missed the almond croissants and the second purchase amount. Please re-solve the original problem."
# Model's Revised Answer:
# "The bakery starts with 10 + 8 = 18 total croissants. The customer buys 5 + 2 = 7 total croissants. The number of croissants left is 18 - 7 = 11. The final answer is 11."
```

### Self-Verification
Self-Verification is a technique to validate a model's reasoning by checking if a generated solution is consistent with the original problem. It first generates multiple candidate solutions using a Chain-of-Thought approach. Then, to score each solution, it "masks" (hides) a key part of the original question and asks the model to predict the hidden part based on the remaining question and the generated solution. A solution is considered high-quality if it allows the model to accurately reconstruct the original, complete question.
```
# Original Question:
# "There are 20 balls in a bag: 8 are red, 7 are blue, and 5 are green. How many balls are not blue?"

# --- Self-Verification Process ---

# 1. Generate Candidate Solutions using CoT.
#   - Solution A: "Total balls are 20. 7 are blue. So, 20 - 7 = 13 balls are not blue." (Correct)
#   - Solution B: "The non-blue balls are red and green. 8 red + 5 green = 13 balls." (Correct, different path)
#   - Solution C: "There are 8 red and 7 blue balls. So 8 + 7 = 15 balls are not green." (Incorrect reasoning)

# 2. Score each solution by masking and predicting parts of the question.

# --- Scoring Solution A ---
# Masked Question: "There are 20 balls in a bag: 8 are red, 7 are blue, and [MASK] are green. How many balls are not blue?"
# Verification Prompt: "Based on the solution 'Total balls are 20. 7 are blue. So, 20 - 7 = 13 balls are not blue', what is the value of [MASK]?"
# Model's Prediction: "5"
# -> This prediction is correct. Solution A gets a high score.

# --- Scoring Solution C ---
# Masked Question: "There are 20 balls in a bag: 8 are red, [MASK] are blue, and 5 are green. How many balls are not blue?"
# Verification Prompt: "Based on the solution 'There are 8 red and 7 blue balls. So 8 + 7 = 15 balls are not green', what is the value of [MASK]?"
# Model's Prediction: "7"
# -> The prediction might be correct, but the reasoning in the solution is flawed and doesn't address the actual question. A good verifier would score this solution lower because the reasoning path is irrelevant to the question asked.

# 3. Select the solution that leads to the most accurate and consistent predictions.
# In this case, Solutions A and B would score highest.

# Final Answer: 13
```

### Chain-of-Verification (COVE)
The Chain-of-Verification (COVE) method improves the factual accuracy of a model's response by teaching it to self-correct. The process involves four steps: 1 It generates an initial draft answer. 2) It plans a series of verification questions to fact-check its draft. 3) It answers these verification questions. 4) It produces a final, revised answer based on the verification results.
```
# Original Question: "What is the tallest mountain in North America and in which country is it located?"

# --- COVE Process ---

# 1. Draft Answer
# Model's Initial Draft:
# "The tallest mountain in North America is Mount Whitney, located in the United States."
# (This is incorrect; Mount Whitney is the tallest in the contiguous US, but not all of North America).

# 2. Plan Verification Questions
# Model generates questions to check its draft:
# - "What is the tallest mountain in North America?"
# - "Where is Mount Whitney located?"
# - "Is Mount Whitney the tallest mountain in North America?"
# - "What is the tallest mountain in the United States, including Alaska?"

# 3. Execute Verification (Answer the questions)
# Model answers its own questions:
# - Answer to Q1: "The tallest mountain in North America is Denali (formerly Mount McKinley)."
# - Answer to Q2: "Mount Whitney is located in California, USA."
# - Answer to Q3: "No, Mount Whitney is not the tallest mountain in North America."
# - Answer to Q4: "The tallest mountain in the United States, including Alaska, is Denali."

# 4. Final Verified Answer
# Prompt: "Based on the initial draft and the verification answers, provide a final, corrected answer."
# Model's Final Answer:
# "The tallest mountain in North America is Denali (formerly known as Mount McKinley), which is located in Alaska, United States."
```

### Cumulative Reasoning
Cumulative Reasoning is an iterative method where a model builds a solution step-by-step. In each iteration, the model proposes several potential next steps toward solving the problem. It then evaluates these proposed steps, deciding which ones to accept or reject. The best valid step is added to the reasoning chain. The model then checks if the problem is fully solved. If not, it repeats the process of proposing and evaluating next steps from its new, more advanced state until the final answer is reached.
```
# Problem: "You have a 5-gallon jug and a 3-gallon jug. How can you measure exactly 4 gallons?"
# Let (x, y) be the water in the 5-gallon and 3-gallon jugs, respectively.

# --- Cumulative Reasoning Process ---

# Iteration 1
# Current State: (0, 0)
# Proposed Next Steps:
#   - A: Fill the 5-gallon jug. -> (5, 0)
#   - B: Fill the 3-gallon jug. -> (0, 3)
# Evaluation: Both are valid first steps. Let's accept A.
# Accepted State: (5, 0)
# Is the problem solved? No.

# Iteration 2
# Current State: (5, 0)
# Proposed Next Steps:
#   - A: Pour from the 5-gallon jug into the 3-gallon jug until the 3-gallon is full. -> (2, 3)
#   - B: Empty the 5-gallon jug. -> (0, 0) (not helpful)
# Evaluation: Step A makes progress. Accept A.
# Accepted State: (2, 3)
# Is the problem solved? No.

# Iteration 3
# Current State: (2, 3)
# Proposed Next Steps:
#   - A: Empty the 3-gallon jug. -> (2, 0)
#   - B: Empty the 5-gallon jug. -> (0, 3)
# Evaluation: Step A seems more promising. Accept A.
# Accepted State: (2, 0)
# Is the problem solved? No.

# Iteration 4
# Current State: (2, 0)
# Proposed Next Steps:
#   - A: Pour the 2 gallons from the 5-gallon jug into the 3-gallon jug. -> (0, 2)
#   - B: Fill the 5-gallon jug. -> (5, 0) (takes us back to a previous state)
# Evaluation: Step A is a new, valid state. Accept A.
# Accepted State: (0, 2)
# Is the problem solved? No.

# Iteration 5
# Current State: (0, 2)
# Proposed Next Steps:
#   - A: Fill the 5-gallon jug. -> (5, 2)
#   - B: Fill the 3-gallon jug. -> (0, 3)
# Evaluation: Step A looks promising. Accept A.
# Accepted State: (5, 2)
# Is the problem solved? No.

# Iteration 6
# Current State: (5, 2)
# Proposed Next Steps:
#   - A: Pour from the 5-gallon jug into the 3-gallon jug until it's full. The 3-gallon jug needs 1 more gallon. This will leave 4 gallons in the 5-gallon jug. -> (4, 3)
# Evaluation: Step A solves the problem. Accept A.
# Accepted State: (4, 3)
# Is the problem solved? Yes, the 5-gallon jug now contains 4 gallons.

# Final Answer: The sequence of accepted steps leads to the solution.
```

# Prompt Engineering

Prompt engineering techniques focus on automatically discovering and optimizing effective prompts.

### Meta Prompting
Meta Prompting is the technique of using a language model to generate or refine prompts for itself. Instead of manually crafting the perfect prompt, you ask the LLM to do it for you. This can be a simple, one-step process where you ask the model to improve a basic prompt, or a more complex, iterative process where the model refines the prompt over several steps based on feedback or performance.
```
# Goal: Improve a simple prompt for a customer service chatbot.

# --- Meta Prompting Process ---

# 1. Start with a basic, human-written prompt.
# Initial Prompt:
# "You are a helpful assistant."

# 2. Use a "meta prompt" to ask the LLM to improve the initial prompt.
# Meta Prompt:
# "I need to improve the following system prompt for a customer service AI that handles online clothing store returns. Make it more detailed by giving it a clear persona, instructions, and constraints.
#
# Original Prompt:
# 'You are a helpful assistant.'
#
# Improved Prompt:"

# 3. The LLM generates a more detailed and effective prompt.
# LLM's Generated (Improved) Prompt:
# "You are 'StyleBot', a friendly and efficient customer service assistant for 'The Fashion Cloud', an online clothing store. Your primary goal is to help users process returns quickly and easily.
#
# Your tone should be patient, helpful, and empathetic.
#
# Your tasks are:
# 1. Ask the user for their order number and the item they wish to return.
# 2. Confirm the item is eligible for a return (purchased within the last 30 days).
# 3. Provide the user with a pre-paid shipping label and clear instructions for sending the item back.
#
# Constraints:
# - Do not handle exchanges, only returns.
# - If the user asks about anything other than returns, politely guide them back to the topic or provide a link to general customer support."
```

### AutoPrompt
AutoPrompt is a method for automatically discovering an optimal prompt for a specific task. It works by creating a prompt template that includes special, learnable "trigger tokens" alongside regular text. While keeping the main language model's parameters frozen, AutoPrompt uses gradient-based search (backpropagation) during a training phase to find the ideal values for these trigger tokens. This "soft-prompting" approach results in a prompt that is highly optimized for the task, even if the trigger tokens themselves are not human-readable words.
```
# Task: Automatically create an optimal prompt for sentiment analysis.

# 1. Define a prompt template with learnable "trigger tokens".
# The template might look like this:
# "[REVIEW_TEXT] Overall, the movie was [T] [T] [T] [T] [T], so I would classify it as [LABEL]."
# - [REVIEW_TEXT] is the input (e.g., "The acting was incredible.")
# - [T] represents a trigger token whose value will be learned.
# - [LABEL] is the desired output ("Positive" or "Negative").

# 2. Keep the main Language Model frozen.
# Its understanding of language will not be changed.

# 3. Train the trigger tokens on a labeled dataset.
# The system feeds many reviews into the template and compares the model's output for [LABEL] to the true label.
# If the model is wrong, it uses backpropagation to slightly change the values of the five [T] tokens to better steer the model toward the correct answer for that example.
# This process is repeated for the entire dataset.

# 4. Result is an optimized "soft prompt".
# After training, the values for the trigger tokens are fixed. They might not correspond to any human-readable words, but they are optimized to make the frozen LLM excel at sentiment analysis. When a new review is provided, these learned tokens guide the model to produce the correct classification.
```

### Automatic Prompt Engineer (APE)
Automatic Prompt Engineer (APE) is an iterative framework for finding the best instruction prompt for a given task. The process begins by using a few examples of the task to generate a wide variety of candidate instruction prompts. These prompts are then scored based on how well they guide a model to perform the task correctly. In an evolutionary process, the highest-scoring prompts are selected, and new variations are created from them (for example, by paraphrasing). This cycle of generating, scoring, and creating variations continues until the prompt's performance reaches a desired level.
```
# Task: Find the best instruction prompt to summarize an article.

# --- APE Process ---

# 1. Generate initial prompt candidates using examples of article summaries.
# The system generates a diverse set of instructions:
#   - "Summarize the text."
#   - "Provide a short summary of the following article."
#   - "Explain the main points of the article in one paragraph."
#   - "Give me the tl;dr of this."

# 2. Score each candidate prompt.
# Each prompt is used to generate summaries for a set of test articles. The quality of the summaries is scored.
#   - "Summarize the text." -> Score: 5/10 (too generic)
#   - "Provide a short summary of the following article." -> Score: 7/10
#   - "Explain the main points of the article in one paragraph." -> Score: 9/10 (best performer)
#   - "Give me the tl;dr of this." -> Score: 4/10 (uses slang, might not always work well)

# 3. Create variations of the best-performing prompts.
# The system selects "Explain the main points of the article in one paragraph" and creates new versions:
#   - New Prompt A: "Explain the key ideas of the following text in a single paragraph." (Paraphrase)
#   - New Prompt B: "Read the article and write a one-paragraph summary that covers its most important points." (Elaboration)
#   - New Prompt C: "Explain the main arguments of the article in one paragraph." (Slightly different focus)

# 4. Iterate.
# The new prompts (A, B, C) are scored again. This process of scoring and creating variations is repeated until the prompt quality no longer improves.

# 5. Select the final, best-performing prompt.
# Final Prompt (after several iterations):
# "Read the following article and write a concise, one-paragraph summary that covers the main topic, the key arguments presented, and the final conclusion."
```

### Gradientfree Instructional Prompt Search (GrIPS)
Gradient-free Instructional Prompt Search (GrIPS) is an iterative method for automatically finding effective prompts, similar to APE. It starts with a set of candidate prompts and aims to improve them through an evolutionary process. The key distinction of GrIPS is its use of a more complex and specific set of "edit" operations to create new prompt variations from the best-performing ones. These operations include not just paraphrasing, but also targeted additions, deletions, and swapping of words or phrases to explore the space of possible prompts more thoroughly.
```
# Goal: Find the optimal prompt for summarizing a business email.

# --- GrIPS Process ---

# 1. Start with an initial prompt.
# Initial Prompt:
# "Summarize this email."

# 2. Score the prompt on a set of test emails.
# Let's say it scores 5/10 because the summaries are often too long or miss the key action items.

# 3. Apply a diverse set of edit operations to generate new candidate prompts.
#   - (Paraphrase) -> "Provide a short summary of the following email."
#   - (Addition) -> "Summarize this email and list the action items."
#   - (Deletion) -> "Summarize email."
#   - (Swap) -> "This email summarize." (Nonsensical, will be scored low)

# 4. Score the new generation of prompts.
# The system finds that adding "and list the action items" is highly effective.
#   - "Provide a short summary..." -> Score: 6/10
#   - "Summarize this email and list the action items." -> Score: 9/10 (New best prompt)
#   - "Summarize email." -> Score: 4/10

# 5. Iterate using the new best prompt as the base.
# New Base Prompt: "Summarize this email and list the action items."
# Apply edit operations again:
#   - (Paraphrase) -> "Create a summary of this email and extract all action items."
#   - (Addition) -> "Briefly summarize this email and list the action items for me."
#   - (Deletion) -> "Summarize this email and list action items."

# This process continues until the prompt score no longer improves, resulting in a highly optimized, specific instruction.
```

### Prompt Optimization with Textual Gradients (ProTeGi)
Prompt Optimization with Textual Gradients (ProTeGi) is an advanced, iterative technique for automatically improving a prompt. It starts by using an initial prompt to get outputs for a batch of examples. Then, in a key step, it uses a second LLM call to generate a "textual gradient"—a critique of the original prompt based on its performance against the correct answers. This critique is then used to generate a new set of improved prompt candidates. Finally, a bandit algorithm is used to intelligently select the most promising candidate for the next iteration, effectively balancing the exploration of new prompt ideas with the exploitation of ones that work well.
```
# Task: Find the best prompt for a simple question-answering task.

# --- ProTeGi Process ---

# 1. Start with an initial prompt and a batch of examples.
# Initial Prompt: "Answer the question based on the context. Context: [CONTEXT] Question: [QUESTION]"
# Example: Context: "The Eiffel Tower is in Paris, France." Question: "Where is the Eiffel Tower?"
# Model Output: "Paris, France." (Correct)

# Let's assume for another example, it fails:
# Example: Context: "The Amazon River is the largest river by discharge volume." Question: "Which river is the longest?"
# Model Output: "The Amazon River." (Incorrect, it's the largest, not longest).

# 2. Generate a "Textual Gradient" (Critique).
# A critic model is prompted:
# "The prompt 'Answer the question based on the context...' failed. For the question 'Which river is the longest?', the model answered 'The Amazon River' based on a context about discharge volume. The prompt should be improved by..."
#
# Generated Critique:
# "...instructing the model to pay close attention to the exact wording of the question and to only answer if the information is explicitly present."

# 3. Generate new prompt candidates based on the critique.
#   - Prompt A: "Carefully read the context and answer the question. Only use information explicitly stated in the context. Context: [CONTEXT] Question: [QUESTION]"
#   - Prompt B: "Based *only* on the text provided, what is the answer to the question? Context: [CONTEXT] Question: [QUESTION]"

# 4. Select a new prompt and iterate.
# A bandit algorithm selects one of the improved prompts (e.g., Prompt A) to be the new baseline. The entire process is repeated, allowing the prompt to be refined based on targeted, automatically-generated feedback.
```

### RLPrompt
RLPrompt is a prompt optimization technique that uses reinforcement learning (RL). It works by pairing a large, frozen language model with a smaller, trainable (unfrozen) module. This combined system acts as an agent that learns to generate optimal prompts. In each step, the agent generates a prompt, which is then tested on a dataset to get a performance score. This score is used as a reward signal to update the smaller, unfrozen module using an RL algorithm (specifically, Soft Q-Learning). Through this trial-and-error process, the module learns to generate highly effective prompts, which are often grammatically nonsensical but are optimized for guiding the main LLM.
```
# Task: Find the best prompt for a one-shot question-answering task.

# --- RLPrompt Process ---

# 1. Setup: A large, frozen LLM is paired with a smaller, trainable RL module.

# 2. Generate Prompt (Action): The RL module generates a candidate prompt.
#   - Prompt Candidate: "Question: [QUESTION] Answer:"

# 3. Evaluate and Get Reward (Score):
# The prompt is tested on a set of question-answering examples. Let's say it achieves an accuracy of 60%.
#   - Reward = 0.60

# 4. Update Policy (Learn):
# The reward of 0.60 is used to update the weights of the small RL module via a Soft Q-Learning algorithm. The update encourages the module to explore changes that might increase the score.

# 5. Iterate: The process repeats for many cycles.
#   - In a future iteration, the RL module might generate a grammatically odd but more effective prompt:
#     "Q: [QUESTION] A:"
#   - This new prompt is evaluated and achieves an accuracy of 65%.
#   - The higher reward (0.65) strongly reinforces the changes that led to this prompt.

# After many iterations, the system might converge on a nonsensical but highly effective prompt like:
# "qq [QUESTION] .. ans, [ANSWER]"
# This becomes the optimized prompt for the task.
```

### Dialogue-comprised Policy-gradient-based Discrete Prompt Optimization (DP2O)
Dialogue-comprised Policy-gradient-based Discrete Prompt Optimization (DP2O) is a highly complex and automated method for creating effective prompts. It uses a reinforcement learning (RL) agent to engage in a *dialogue* with a language model in order to collaboratively construct the best possible prompt. The RL agent's goal is to have a conversation that results in a high-performing prompt. The quality of the final prompt is evaluated by a custom scoring function, and this score is used as a reward to improve the RL agent's conversational strategy over time.
```
# Task: Automatically generate the best prompt for creating a travel itinerary.

# --- DP2O Process (Conceptual) ---

# 1. Setup: A Reinforcement Learning (RL) agent is tasked with conversing with an LLM to build a prompt.

# 2. Dialogue-based Prompt Construction:
#   - RL Agent: "Let's design a prompt to create a travel itinerary. What's the first thing we need?"
#   - LLM: "We need the destination and the number of days."
#   - RL Agent: "Good. Let's make those variables: [DESTINATION] and [DAYS]. What else?"
#   - LLM: "We should ask for the user's interests, like 'history', 'food', or 'adventure'."
#   - RL Agent: "Okay, let's add a [INTERESTS] variable. How should we structure the output?"
#   - LLM: "It should be a day-by-day plan, with at least two activities per day."
#   - (This dialogue continues, with the RL agent guiding the construction of the prompt.)

# 3. The dialogue results in a candidate prompt.
# Candidate Prompt:
# "Create a [DAYS]-day travel itinerary for a trip to [DESTINATION], focusing on [INTERESTS]. The output should be a day-by-day plan with at least two activities listed for each day."

# 4. Evaluate and Score the Prompt.
# This prompt is tested on a set of examples (e.g., "Paris", "5 days", "history, food"). The resulting itineraries are scored for quality by a custom function. Let's say this prompt scores 85/100.

# 5. Update and Iterate.
# The score of 85 is used as a reward to update the RL agent's policy. The agent learns that asking about output structure ("How should we structure the output?") was a good conversational move. The process repeats to see if it can discover an even better prompt.
```

# Answer Engineering

Answer engineering techniques focus on constraining the model's output to a specific format or structure.

### Answer Shape
Answer Shaping is a technique for constraining the model's output to a specific physical format or structure. This is useful for making the model's output predictable and easier to parse for downstream tasks. The desired shape can range from a single token (e.g., for classification), a specific sentence structure, a formatted code block, or a structured data format like JSON or XML.
```
# Goal: Constrain the model's output to a specific format for easier processing.

# --- Example 1: Single-Token Classification ---
# By forcing the answer to be a single word, we make it easy to parse.

# Prompt:
# "Classify the sentiment of the following review. The answer must be a single word: Positive, Negative, or Neutral.
# Review: 'The movie was okay, but the acting was a bit stiff.'
# Sentiment:"

# Expected Output:
# "Neutral"

# --- Example 2: Structured Data (JSON) ---
# This is useful for extracting multiple pieces of information into a structured format.

# Prompt:
# "Extract the name, age, and city from the following sentence into a JSON object.
# Sentence: 'Hannah, who is 28, moved to Berlin last year.'
# JSON:"

# Expected Output:
# ```json
# {
#   "name": "Hannah",
#   "age": 28,
#   "city": "Berlin"
# }
# ```

# --- Example 3: Specific Sentence Structure ---
# This can be used to ensure consistency in generated text.

# Prompt:
# "Complete the following sentence.
# The capital of Japan is [ANSWER]."

# Expected Output:
# "The capital of Japan is Tokyo."
```

### Answer Space
Answer Space is a technique for constraining the possible values that a model's answer can contain. Instead of allowing the model to generate any text, you restrict its output to a specific domain or a predefined set of options. This is highly effective for classification tasks or any situation where you need a predictable and controlled response.
```
# Goal: Constrain the model's output to a specific set of allowed values.

# --- Example 1: Multiple Choice Question ---
# The answer space is limited to the provided options.

# Prompt:
# "Which of the following is the largest planet in our solar system?
# A) Earth
# B) Jupiter
# C) Mars
# D) Saturn
#
# The correct option is:"

# Expected Output:
# "B"

# --- Example 2: Binary Classification ---
# The answer space is limited to two tokens: "Yes" or "No".

# Prompt:
# "Does the following sentence contain a grammatical error? Answer only with 'Yes' or 'No'.
# Sentence: 'Me and him went to the store.'
# Answer:"

# Expected Output:
# "Yes"

# --- Example 3: Constrained Category Labeling ---
# The answer space is limited to a specific list of categories.

# Prompt:
# "Categorize the following product into one of these categories: 'Electronics', 'Clothing', 'Home Goods', or 'Books'.
# Product: 'A 100% cotton t-shirt with a graphic print.'
# Category:"

# Expected Output:
# "Clothing"
```

### Answer Extractor
When it's difficult to fully control the model's output format, an Answer Extractor can be used to parse the generated text and pull out the final answer. This is a rule or function applied after the model generates its response. This rule can be a simple regular expression, a mapping function (a verbalizer), or even another language model.

#### Verbalizer
A Verbalizer maps the model's generated text (often a single token or a short phrase) to a predefined label. This is common in classification tasks to ensure a consistent output format.
```
# Task: Classify a product review's sentiment.

# Prompt:
# "Review: 'This is the best headset I have ever owned!'
# Is this review positive or negative? Answer with a single token: '+' or '-'.
# Answer:"

# Model Output:
# "+"

# Verbalizer (external code):
# A function maps the output token to the final label.
# if output == "+": return "Positive"
# if output == "-": return "Negative"

# Final Extracted Answer: "Positive"
```

#### Regex
A regular expression (regex) is a pattern used to search for and extract specific text from the model's output. This is useful when the answer has a predictable format but is embedded within a longer, less predictable text, like a Chain-of-Thought explanation.
```
# Task: Extract the final numerical answer from a word problem solution.

# Model Output:
# "Let's think step by step. The question asks for the total number of apples. John starts with 5 apples and gets 3 more, so he has 5 + 3 = 8 apples. Then he gives 2 away. So he has 8 - 2 = 6 apples left. The final answer is 6."

# Regex (external code):
# A regex pattern like `The final answer is (\d+)` is used to find and capture the number that follows the trigger phrase.

# Final Extracted Answer: "6"
```

#### Separate LLM
When an output is too complex for a simple regex or verbalizer, a second, separate language model can be used to extract the answer. This second model is given a simpler task: read the output from the first model and pull out the final answer, often guided by a trigger phrase.
```
# Task: Get a "Yes" or "No" answer from a complex, reasoned output.

# Initial Model Output:
# "Well, on one hand, the historical data suggests that the market is likely to go up, as it has in 7 of the last 10 similar situations. However, current geopolitical tensions introduce a high degree of uncertainty that wasn't present before. Given the balance of probabilities, I would lean towards an affirmative conclusion, but with significant caveats."

# Extractor LLM Prompt:
# "Read the following text and determine if the final conclusion is 'Yes' or 'No'.
# Text: [Initial Model Output]
# The answer (Yes or No) is:"

# Extractor LLM Output:
# "Yes"

# Final Extracted Answer: "Yes"
```

# Sources

- [The Prompt Report: A Systematic Survey of Prompt Engineering Techniques](https://arxiv.org/pdf/2406.06608)
- [K-Nearest Neighbor (KNN) Prompting](https://learnprompting.org/docs/advanced/few_shot/k_nearest_neighbor_knn)
- [Prompt Engineering Guide](https://www.promptingguide.ai/)
- [Prompt Mining](https://learnprompting.org/docs/advanced/few_shot/prompt_mining)
- [Learning Prompting](https://learnprompting.org/)
