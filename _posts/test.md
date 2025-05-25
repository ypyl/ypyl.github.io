•	The important part of RAG is how we can search for the correct document. The search accuracy will directly impact the result of the response accuracy. *
There are typical search methods we can consider which are BM25 and deep retrieval.
o	BM25: It is a ranking function used by search engines to rank documents according to their relevance to a query. Not only does it demonstrate high performance in general information search tasks but also keep accuracy for domain specific area. Highly practical in terms of cost and maintainability.

o	Deep Retrieval: Vectorize the documents and queries and search by quantifying the distance between vectors using cosine similarity. The key consideration is whether the performance exceeds that of BM25.

•	there are 2 major ways to improve the search. modifying the document and modifying the search model. For the document modification below are the methods they've tried.
o	Chunking: there may be a lack of continuity between texts or missing information when chunked automatically. Therefore, manual checking and text formatting to divide the texts in a meaningful chunk is important. Currently, it is difficult to automatically chunk in this way, and it requires a trial-and-error based on domain knowledge. This may not be necessary if keyword-based search can cover a certain level of use cases, but it is unavoidable if semantic search for complex questions is to be performed.

o	Summary Generation: Documents may be multiple lines and include figures and tables, but most questions are only one or two lines long.  Instead of directly comparing the vectors of documents with the questions, it may improve accuracy by generating a summary of the document and then compare with the questions. If you have FAQs to search, it is better to search the related questions from the FAQ and pick up the related document from the answer.

o	Expanding the Question: the opposite of summary generation. Idea to make the question closer to the document text. It may improve accuracy by having LLM generate a provisional document from the question, and then searching document that is close to it. Since it requires the LLM to move for each search, they did not use this method for production.

o	Other methods that can be consider: Knowledge Graph

For modifying search model
o	Fine-tune:  fine tune the existing embedding models like Sentence Transformers, can add domain knowledge if there is enough training data and it can increase the search accuracy. They had enough data and took this approach. It contributed most to improve the accuracy in their case.

o	Re-ranking model: it is used to find the most relevant sentence/paragraph after semantic search has narrowed down the document list to a certain degree.  Using Cross-Encoders as reranker in multistage vector search | Weaviate - vector database
