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
This technique improves the model's reliability. Here's how it works:
1.  **Generate Multiple Solutions:** The model creates several different step-by-step solutions to the same problem.
2.  **Check for Agreement:** It then checks if most of the solutions arrive at the same answer.
3.  **Decide on the Best Answer:**
    *   If most solutions agree, the model uses that answer.
    *   If they disagree, the model throws them out and generates a single, direct answer instead.

This way, the model uses a more reliable, multi-path approach when it's confident and a safe, direct approach when it's uncertain.
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
Program-of-Thoughts (Chen et al., 2023d) uses LLMs like Codex to generate programming code as reasoning steps. A code interpreter executes these steps to obtain the final answer. It excels in athematical and programming-related tasks but is less effective for semantic reasoning tasks.
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
