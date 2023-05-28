---
layout: post
title: "How to use wait-for-it with docker compose"
date: 2018-09-15

tags: docker
categories: deployment
---
There is a possibility to control startup order in Compose using great script - [wait-for-it](https://github.com/vishnubob/wait-for-it).
It allows you to wait some docker containers which have long-running initialization process like databases.

And there is step-by-step explanation how to use it:
1. Copy [wait-for-it.sh](https://github.com/vishnubob/wait-for-it/blob/master/wait-for-it.sh) to your project

![image](/images/service-project.png)

2. Copy it to your docker image
```toml
FROM ...
...
COPY wait-for-it.sh .
...
CMD ...
```

3. Modify docker-compose.yml to call wait-for-it script:
```toml
service:
    image: service:latest
    expose:
        - "80"
    depends_on:
        - database
    command: ["./wait-for-it.sh", "database:3306", "--timeout=360", "--", "command"]
```
where
    - `command` is a command which will be called by wait-for-it script after timeout or successful ping result to database:3306

Thanks.