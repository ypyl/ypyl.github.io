---
title: "Replication"
date: 2018-09-20

tags: ["high-load"]
categories: ["learning"]
---

Replication is used for the following purposes:

- High availability, so the system will keep working even some of the servers will break down.

- Offline work, so the application is working without an internet connection.

- Latency, so the data of the application is located near consumers.

- Scaling, e.g. allowing to process the huge number of reading operations via sharing the load via replicas.

It is required to take care of concurrent access and failovers while using replication. Moreover, need to plan the process of recovering the system after failovers (e.g. breaking network connections, crashed system nodes, damaged data).

There are three types of replication:

- Replication with one master node. Clients send all write operations to one master node, which shares changes with other slaves nodes. Read operations can be done via slave nodes, but the data on them can be outdated.

- Replication with more than one master node. Clients send write operations to one of the master nodes. Master nodes can send data to other master nodes and slaves nodes.

- Replication without master node. Clients send write operations to one of the nodes and read data from more than one node to discover nodes with outdated data and update it.

Replication with one master is more widespread due to its simplicity and lack of the need for conflict resolution. Other methods are more resistant to node failures, sudden issues in latency, but they are more complex and provide weak guarantees of consistency. 

Replication can be synchronous and asynchronous, and it affects the behavior of the system in case of failure. Even the synchronous replication can be executed more quickly in general, but it will bring additional issues in case of nodes crashes. E.g., there is a probability of losing recently captured data if the master node will fail.

There are a couple of consistency models useful for solving issues with replication delaying:

- Read-after-write consistency. Users should always see the data that they sent to the database. 

- Monotonous reading. After the user has seen the data at any point in time, he should not later see the same data as at an earlier point in time. 

- Consistent prefix read. Users should see the data in a state that does not violate causation, for example, see the question and the answer in the correct order.

Some algorithms allow to check whether one operation happened before another, or they happened competitively (which can be used in databases).

Also, there are methods of resolving conflicts by merging competitive updates. 

