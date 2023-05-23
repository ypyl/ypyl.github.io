---
layout: post
title: 'Backtracking problem'
date: 2021-10-08
categories: backtracking programming
---

# Backtracking

It is an algorithmic technique that is often used to solve complicated coding problems. It considers searching in every possible combination for solving a computational problem.

There is a template that can used to understand and solve problems using backtracking.

```python
def is_valid_state(state):
    # check if it is a valid solution
    return True

def get_candidates(state):
    return []

def search(state, solutions):
    if is_valid_state(state):
        solutions.append(state.copy())
        # return

    for candidate in get_candidates(state):
        state.add(candidate)
        search(state, solutions)
        state.remove(candidate)

def solve():
    solutions = []
    state = set()
    search(state, solutions)
    return solutions
```

- `is_valid_state` checks if the current state is valid and should be remembered as valid case.
- `get_candidates` provides a list of all possible candidates satisfying the problem constraints based on our current state.
- `search` contains recursion and aggregate valid states.
- `solve` is the entry point.

# Reference

[Learn How to Solve Coding Interview Backtracking Problems](https://www.freecodecamp.org/news/solve-coding-interview-backtracking-problem/)
