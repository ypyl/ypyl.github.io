---
layout: post
title: "Response without root element"
date: 2011-06-21

tags: wcf
categories: programming
---
Write WCF web-service that accepts SOAP 1.2 and SOAP 1.1 requests. Service should expose 1 operation (let it be "GetResponse_1") with the following structure (just an example, please combine schema for the service):

```xml
<GetResponse_Request_1>
    <element1>text1</element1><!-- at least 1 "element1" -->
    <element1>text2</element1>
    <element1>text3</element1>
    <element2>textn</element2><!-- at least 1 "element2" -->
    <element2>textn+1</element2>
    <element2>textn+2</element2>     
</GetResponse_Request_1>
```

Response should contain "planed" element1 and element2 sets, just comma separated values:

```xml
<GetResponse_Response_1>
    <element1_plane>text1, text2, text3</element1>
    <element2_plane>textn, textn+1, textn+2</element2>
</GetResponse_Response_1>
```