---
layout: post
title: Data-oriented programming
date: 2023-08-30
categories: programming
tags: programming data
---

# Notes about data-oriented programming

## Code vs data

In data-oriented programming (DOP), its principles are language-agnostic and promote the separation of code from data as a foundational concept (Principle #1). This separation simplifies DOP systems compared to traditional Object-Oriented Programming (OOP) systems. DOP focuses on data entities, which hold information, and rejects strict data encapsulation, emphasizing flexibility to adapt to evolving requirements. By isolating code and data, DOP allows for independent design and representation of these elements. DOP systems are organized into high-level data entities and code modules composed of stateless functions. They are highly adaptable and often accommodate changing requirements without altering the core system design. Unlike traditional OOP, where object state is implicit, DOP employs explicit arguments for stateless functions. In DOP, code modules primarily relate through usage, while data entities have associations and compositions as their key relationships. This approach enhances system clarity and flexibility.

## Data

Principle #2 emphasizes representing data entities using generic data structures. These structures, often referred to as string maps, allow data to be represented as records, which are collections of fields, possibly of different data types. Data entity diagrams consist of these records, interconnected through composition or association relationships. DOP values flexibility and genericity, even at the cost of some data safety. It allows dynamic field addition, removal, and renaming at runtime. Data manipulation is achieved through generic functions, making it easier to adapt to changing requirements. While DOP might lose some compile-time type safety, it liberates data representation from classes and objects, promoting a clear, adaptable system with information accessible through information paths and generic functions, particularly suitable for data-heavy tasks.

## State

Principle #3 highlights that data should be immutable. Mutations, which are operations that change the system's state, are managed through a multi-version approach split into calculation and commit phases. All data manipulation occurs via immutable functions, and native hash map setters are forbidden. Structural sharing enables the efficient creation of new data versions by sharing common elements, reducing memory and computational costs. The calculation phase, which is stateless, employs immutable functions with structural sharing, while the commit phase handles system state updates and is stateful. Data validation is separated from data manipulation, ensuring system-wide consistency. DOP's structure allows efficient memory management and straightforward state restoration, thanks to the clear separation between calculation and commit phases.

## Concurrency

In data-oriented programming, optimistic concurrency control is a lock-free approach that allows mutations to proceed optimistically without waiting for permission. This method efficiently manages concurrent mutations of the system state, enabling high throughput of read and write operations, particularly when combined with immutable data. Conflicts between concurrent mutations are reconciled during the commit phase, often following a similar approach to Git's branch merging, with options for fast-forward or three-way merges. The reconciliation algorithm is universal and can be applied to systems using immutable hash maps. It leverages structural sharing to create subsequent versions of the system state efficiently. In user-facing systems, conflicting concurrent mutations are infrequent, and when reconciliation isn't possible, mutations are aborted, prompting the user to retry. Comparing structural differences between versions is efficient due to the inherent sharing in immutable data structures, and the algorithm handles replacements and additions effectively. However, it does not support deletions.

## Validation

Principle #4 emphasizes the separation of data schema and data representation. This principle recognizes the importance of clearly defining the boundaries of a system where data is exchanged, often involving data validation. Data validation in DOP entails ensuring that data adheres to a specific schema, and when validation fails, error information is provided in a human-readable format. Validating data at the system's boundaries reduces the need for redundant validation within the system. JSON Schema is a key tool in this process, allowing the separation of data validation from its representation. While JSON Schema syntax can be verbose, it offers high expressiveness, and schemas can be manipulated like any other maps in the program, enabling flexible and robust data validation practices for both incoming and outgoing data.

## Polymorphism

Polymorphism is primarily valued for its extensibility. This extensibility is enabled by multimethods, which facilitate polymorphism in scenarios where data is represented with generic maps. A multimethod comprises a dispatch function and multiple methods, with the dispatch function emitting a dispatch value used to determine the appropriate method implementation. Multimethods can emulate object-oriented class inheritance through single dispatch, where a map with a "type" field is used for dispatching. They also support more advanced polymorphisms, such as multiple dispatch and dynamic dispatch, allowing behavior to depend on multiple or runtime arguments. Multimethods provide extensibility by decoupling method initialization from method implementations and support default implementations when no specific method matches the dispatch value. Careful attention is needed to ensure consistency between the order of elements in the dispatch array and the method wiring in multimethods featuring multiple dispatch.

## Resources

- [Data-Oriented Programming](https://blog.klipse.tech/data-oriented-programming-book.html)
