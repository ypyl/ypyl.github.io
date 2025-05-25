---
layout: post
title: Automating Article Summarization and Posting to Telegram using Python
date: 2024-06-08

tags: ai groq llm python automation web-scraping beautifulsoup telegram-bot api-integration tech-content-curation
categories: programming
---

# Automating Article Summarization and Posting to Telegram using Python

In this blog post, we will walk through a Python script that automates the process of extracting text from a web page, summarizing it into key points using the Groq API, and posting the summary to a Telegram channel. This can be particularly useful for curating and sharing tech content efficiently.

## Overview of the Script

The script is divided into several key functions:

1. **Extracting text from a URL using BeautifulSoup**
2. **Summarizing the text with the Groq API**
3. **Posting the summary to a Telegram channel**

Let's dive into each part of the script.

### Importing Necessary Libraries

```python
import requests
from groq import Groq
from bs4 import BeautifulSoup
```

We start by importing the necessary libraries: `requests` for making HTTP requests, `BeautifulSoup` for parsing HTML content, and `Groq` for interacting with the Groq API.

### Setting Up Variables

```python
channel_id = "@ArticleDigestTech"
token = "<<TOKEN>>"
url = "https://techcommunity.microsoft.com/t5/educator-developer-blog/local-development-using-azure-cosmos-db-emulator-at-no-cost/ba-p/4153822"
title = "Local Development using Azure Cosmos DB Emulator at no Cost"
client = Groq(api_key="<<API_KEY>>")
```

Here, we define variables for the Telegram channel ID, bot token, article URL, and title. We also initialize the Groq API client with an API key.

### Defining the Question Template

```python
question_template = (
    "Please extract the main points of the article as bullet list. Article is an extraction from html page as a text. "
    "Provide only valuable information for senior dotnet developer. Do not add any non-valuable information from software development perspective. "
    "Return ONLY bullet points WITHOUT any explanation or comments. Don't be verbose. "
    "Expected result: "
    "- point 1 "
    "- point 2 "
    "- point 3 "
    "- point N "
    "INPUT:\n{}"
)
```

The `question_template` is used to instruct the Groq API on how to summarize the article. It specifies that only valuable information for senior .NET developers should be included.

### Extracting Text from the URL

```python
def extract_text_from_url():
    response = requests.get(url)
    if response.status_code == 200:
        soup = BeautifulSoup(response.content, 'html.parser')
        text = soup.get_text()
        lines = (line.strip() for line in text.splitlines())
        chunks = (phrase.strip() for line in lines for phrase in line.split("  "))
        text = '\n'.join(chunk for chunk in chunks if chunk)
        return text
    else:
        return f"Failed to retrieve the webpage. Status code: {response.status_code}"
```

This function sends a GET request to the specified URL, parses the HTML content with BeautifulSoup, and extracts the text. If the request fails, it returns an error message.

### Summarizing the Text with Groq API

```python
def get_answer_from_groqcloud(article_text):
    question = question_template.format(article_text)
    chat_completion = client.chat.completions.create(
        messages=[
            {
                "role": "user",
                "content": question,
            }
        ],
        model="llama3-8b-8192"
    )
    return chat_completion.choices[0].message.content
```

This function formats the extracted article text with the `question_template` and sends it to the Groq API. The response, which contains the summarized points, is returned.

### Posting the Summary to Telegram

```python
def send_telegram_message(message):
    telegram_url = f"https://api.telegram.org/bot{token}/sendMessage"
    payload = {
        "chat_id": channel_id,
        "text": f"[{title}]({url})\n{message}",
        "parse_mode": "Markdown"
    }
    response = requests.post(telegram_url, data=payload)
    return response.json()
```

This function sends the summarized points to the specified Telegram channel using the Telegram bot API. The message is formatted in Markdown.

### Main Function

```python
if __name__ == "__main__":
    try:
        article = extract_text_from_url()
        if "Failed to retrieve the webpage" in article:
            raise ValueError(article)
        print("Extracted Article Text:")
        print(article)
    except (requests.exceptions.RequestException, ValueError) as e:
        print("Error extracting text from URL:", e)
        exit(1)

    try:
        answer = get_answer_from_groqcloud(article)
        print("Extracted Main Points:")
        print(answer)
    except requests.exceptions.RequestException as e:
        print("Error calling GroqCloud API:", e)
        exit(1)

    try:
        response = send_telegram_message(answer)
        print("Response from Telegram API:")
        print(response)
    except requests.exceptions.RequestException as e:
        print("Error posting message to Telegram:", e)
        exit(1)
```

The main function orchestrates the entire process: extracting the article text, summarizing it, and posting the summary to Telegram. It includes error handling for each step to ensure robustness.

## Conclusion

This script automates the extraction, summarization, and sharing of technical articles. By leveraging the Groq API and BeautifulSoup, it ensures that only the most relevant information is shared with your audience, saving time and effort. Whether you're a content curator or a tech enthusiast, this script can be a valuable tool in your workflow.
