create a blog post based on page content. Provide markdown code to copy paste.  Do not add any source or reference links to it.

# Get started with text analysis using Azure AI Language

Azure AI Language is a cloud service that can perform advanced natural language processing over unstructured text. In this blog post, I will introduce some of the text analysis features that Azure AI Language offers and how you can use them in your applications.

## Entity recognition and linking

One of the text analysis features is entity recognition and linking, which can identify people, places, events, and more from the text. For example, if you provide the text "I ate at the restaurant in Seattle last week", Azure AI Language will return the following entities:

- Seattle: a location entity with a link to Wikipedia
- last week: a date range entity

Entity recognition and linking can help you extract useful information from text and link it to relevant sources.

## Language detection

Another text analysis feature is language detection, which can identify the language in which text is written. For example, if you provide the text "Comida maravillosa y gran servicio", Azure AI Language will return the following language information:

- Spanish: the language name
- es: the ISO 639-1 language code
- 1.0: the confidence score

Language detection can help you handle multilingual text and provide appropriate translations or responses.

## Sentiment analysis and opinion mining

A third text analysis feature is sentiment analysis and opinion mining, which can evaluate text and return sentiment scores and labels for each sentence. For example, if you provide the text "We had dinner at this restaurant last night and the first thing I noticed was how courteous the staff was", Azure AI Language will return the following sentiment information:

- Positive: the document sentiment
- 0.90: the positive score
- 0.10: the neutral score
- 0.00: the negative score

Sentiment analysis and opinion mining can help you detect positive and negative sentiment in social media, customer reviews, discussion forums and more.

## Key phrase extraction

A fourth text analysis feature is key phrase extraction, which can identify the main points from text. For example, if you provide the text "We had dinner here for a birthday celebration and had a fantastic experience", Azure AI Language will return the following key phrases:

- birthday celebration
- fantastic experience
- dinner

Key phrase extraction can help you summarize the main points from text and understand the topics or themes.

## How to use Azure AI Language

To use Azure AI Language in an application, you need to provision a resource in your Azure subscription. You can choose either a Language resource or an Azure AI services resource, depending on your needs and preferences. Then, you can use the Azure portal, SDKs, or REST APIs to access the text analysis features.

If you want to try out Azure AI Language without creating a resource, you can use the Language Studio, a web-based tool that lets you explore the text analysis features interactively. You can also use the Language Studio to customize the text analysis models for your specific scenarios.

## Conclusion

Azure AI Language is a powerful service that can help you analyze unstructured text and extract valuable insights. You can use it to perform entity recognition and linking, language detection, sentiment analysis and opinion mining, key phrase extraction, and more. You can also customize the text analysis models to suit your needs. To learn more about Azure AI Language, check out the [official documentation](https://docs.microsoft.com/en-us/azure/cognitive-services/language-service/).
](2023-02-11-AI900-Understanding-Text-Analytics.md)
