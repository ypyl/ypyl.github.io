---
title: "Data models and query languages"
date: 2018-09-10
tags: ["high-load"]
categories: ["learning"]
---
Initially, data models can be represented as one big tree, but it will not be suitable for `many-to-many` relationships.
RDS can be used to cover such scenarios.

Moreover, there are NoSQL data models:

1. Document-based where data is represented by a set of documents and there aren't a lot of connections between them.
2. Graph-based where a lot of connections between data nodes.

NoSQL data models don't have a mandatory schema for saving data. But it means that such schema will be represented in the code of as application that is reading this data.

Each data model has it's own query language or framework, e.g. SQL, MapReduce, Cypher, SPARQL, Datalog, aggregation in MongoDB.
