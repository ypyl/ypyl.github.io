---
layout: post
title: "Storage systems"
date: 2018-09-15

tags: high-load
categories: learning
---
Storage systems are divided into two broad categories:

* optimized for transaction processing (OLTP)
* optimized for analytics (OLAP)

There are big differences between access approaches in these scenarios.
* OLTP systems are usually aimed to working with end users. It means that there is huge potential number of requests to this system from an application. Typically only a small amount of rows in each request is queried to cope with such a load. Applications request records using a specific key, so the storage system is able to use the index to search for specific data. The bottleneck is usually the time which is required to find data at the disk.
* OLAP systems are less well known, because they are primarily used by business analysts rather than end users. OLAP handles a much smaller number of requests, than OLTP-systems, but all requests are usually very resource-intensive and require checking millions of records in a short time. The bottleneck here is usually disk throughput (not the time to find required data). Columnar oriented databases are quite popular choice for this kind of tasks.

There are two ways of saving data in OLTP:
* Journaled approach that allows only appending data to files and deleting outdated files rather than updating recorded files. E.g. Bitcask, SS tables, LSM trees, LevelDB, HBase, Lucene and others.
* In-place upgrade approach in which the disk is treated as a set pages of a specified size that can be overwritten. Largest representative of this philosophy is B-trees which are used in almost all popular relational and non-relational databases.

Journaling storage systems is a relatively recent invention. Their main idea is in systematic transforming of inconsistent writing to disk into sequential writing, which provides higher throughput ability to record due to performance characteristics of hard drives.

Also there are some storage systems which store all data in RAM.

As for OLAP systems, indexes are not playing big role for them as it is needed to look through the big amount of data. The most important is saving data in compact way at the disk, so the volume of data, which will be read from the disk, will be small. Column-oriented databases help to solve this kind of problems.

Thanks.