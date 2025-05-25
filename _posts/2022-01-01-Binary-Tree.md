---
layout: post
title: Binary tree
date: 2022-01-01
tags: algorithm binary tree structure
categories: programming
---

# [Binary tree](https://en.wikipedia.org/wiki/Binary_tree)

- at most 2 children per node
- exactly one root
- exactly one path between root and any node

Leaf node is a node without any child node.

## Node

There is node implementation:

```python
class Node:
  def __init__(self, val):
    self.value = val
    self.left = None
    self.right = None
```

## Depth First

### Stack based implementation

```python
def depthFirst(root):
  if root is None:
    return []

  results = []
  stack = [root]
  while len(stack) > 0:
    current = stack.pop()
    results.append(current.val)

    # need to start with right, so it will go to the bot of the stack
    if current.right is not None:
      stack.append(current.right)
    if current.left is not None:
      stack.append(current.left)

  return results
```

### Recursion based implementation

```python
def depthFirst(root):
  if root is None:
    return []
  leftValues = depthFirst(root.left)
  rightValues = depthFirst(root.right)
  return [root.val, *leftValues, *rightValues]
```

## Breadth First

```python
from queue import Queue

def breadthFirst(root):
  if root is None:
    return []

  results = []
  queue = Queue()
  while not queue.empty():
    current = queue.get()
    results.append(current.val)

    if current.left is not None:
      queue.put(current.left)
    if current.right is not None:
      queue.put(current.right)
  return results
```

[More information about binary tree and examples of coding challenges](https://www.youtube.com/watch?v=fAAZixBzIAI).
