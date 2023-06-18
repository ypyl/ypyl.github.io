---
layout: post
title: "Partitioning"
date: 2018-09-07

tags: high-load
categories: learning
---
Partitioning is needed when there is too much data to save it at one physical machine.

The goal of partitioning is sharing load between servers and to avoid bottlenecks. It is required to select approproate partitioning scheme and section rebalance during adding or deleting nodes from the cluster.

There are two main approaches in doing partitioning:

* Partitioning by key value range, where keys are sorted and the partition contains all keys from the certain minimum to the certain maximum. The advantage of the sorting is the ability to perform efficient range queries, but there is to create hot spots if the application accesses to the keys located close frequently. This approach usually follows dynamic rebalancing partitioning using splitting the range in two sub-bands in the case when the section's getting too big.
* Hash partitioning, where the hash function is calculated for each key and each partition has a specific range of hashes. This method breaks the order of the keys, making the queries range inefficient, but allows to distribute the load more evenly.
* Hash partitioning often creates a fixed number of partitions in advance, several for each node. Entire partitions are often moved between nodes when any node is added or removed. In this case dynamic partitioning is also used sometimes.

Hybrid approaches are also possible, e.g. with a composite key: using one part of the key to identify the partition and the other part to determine the sort order.

There is also connection between partitioning and secondary indexes. Secondary indexes should be also partitioned and there are two methods to do that:

* Index partitioning by documents (local indexes). Secondary indexes are stored in the same partition with the primary key and value. This means that a write operation only needs to update one partition, but reading a secondary index requires fragmented reading across all partitions.
* Term-based index partitioning (global indexes), in which secondary indexes are partitioned separately, using indexed values. An element of this secondary index can include records from all primary key partitions. It is required to update multiple partitions of the secondary index during recording a document, but it is possible to read the result from a single partition.

There are also different methods for routing queries to the corresponding section, starting with a simple, tailored to the section of the load balancer to complex parallel mechanisms for querying.

All partitions intentionally operate almost independently - this allows to scale a partitioned database across multiple machines.

Thanks.