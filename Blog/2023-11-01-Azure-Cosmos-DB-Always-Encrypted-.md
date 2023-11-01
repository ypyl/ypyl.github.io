---
layout: post
title: Azure Cosmos DB Always Encrypted and GetItemLinqQueryable issue
date: 2023-11-01
categories: programming
tags: dotnet core azure cosmosDB encryption
---

# Reading ecnrypted data issue

When following [Microsoft's article](https://learn.microsoft.com/en-us/azure/cosmos-db/how-to-always-encrypted?tabs=dotnet) on client-side encryption in Azure Cosmos DB and using the {GetItemLinqQueryable}(https://learn.microsoft.com/en-us/dotnet/api/microsoft.azure.cosmos.container.getitemlinqqueryable?view=azure-dotnet) method, you may encounter decryption issues. To resolve this, swap out ToFeedIterator with ToEncryptionFeedIterator from [Microsoft.Azure.Cosmos.Encryption.EncryptionContainerExtensions](https://github.com/Azure/azure-cosmos-dotnet-v3/blob/master/Microsoft.Azure.Cosmos.Encryption/src/EncryptionContainerExtensions.cs). Here's an example:

```
var queryable = _container.GetItemLinqQueryable<TBaseEntity>().AsQueryable();

// Convert to feed iterator
using FeedIterator<TResult> linqFeed = _container.ToEncryptionFeedIterator(query);
```
