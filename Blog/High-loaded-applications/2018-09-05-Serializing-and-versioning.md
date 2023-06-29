---
layout: post
title: Serializing and versioning
date: 2018-09-05

tags: high-load
categories: learning
---
Encoding format affects not only efficiency of the application, but also, what is more important, the architecture of the application which are using them.

In particular, many services require support for incremental deployment of a new version. So new version will be deployed only to a few nodes at one time, not to all nodes. Incremental deployment allows to release new versions of services without forced system inactivity (thereby encouraging frequent small releases rather than rarely performed large ones) and reduce deployment risk (through detection and rollback failed releases before they affect a large number of users). It is extremely beneficial for development — simplicity changes of the application.

In the case of incremental deployment, it is good to allow the possibility to have different versions of the application at the different nodes. Therefore, it is important to encode all DTO with backward compatibility support (new version of appication could read 'old' data objects) and direct compatibility (old could read new).

There are several encoding formats:

* Language-specific coding formats are limited to this language and often cannot provide the reverse and direct compatibility.
* Text formats like JSON, XML and CSV are common, and their compatibility depends on how they are used. They also have optional additional schemas. In these formats, data types are sometimes quite vague, so it's required to work carefully with entities like numbers and binary lines.
* Binary schema-based formats such as Thrift, Protocol Buffers and Avro, allow you to perform compressed and efficient coding with clearly certain semantics of backward and forward compatibility. Their schemes can be useful for documenting and generating code in programming languages with static typing. However, they have a drawback: the data needs to be decoded so that people can read it.

There are several modes of data flow movement, where a data encoding format plays an important role:

* databases where the writing process encodes the data and the reading process from it — decodes;
* RPC and REST APIs in which the client encodes a request, the server decodes the request and encodes a response, and finally, the client decodes the response;
* asynchronous messaging (using message brokers or actors), where nodes communicate by sending each other messages: encoded by sender and decoded by receiver.

Thanks.
