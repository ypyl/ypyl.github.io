---
layout: post
title: "Simplest syslog server for rfc5424 (TCP)"
date: 2017-09-12

tags: dotnet docker
categories: deployment
---
I am working on onside project where we have more than 5 docker containers. Previously we used just serilog to do our logging in all our containers. So each project had his own logging mechanism. Finally we decided to centralize our logging system ans start using docker logs. There are a lot of solutions using clouds (e.g. [loggly](https://www.loggly.com/)) or quite complex systems to grab, analyze and show your logs (like [kafka](https://kafka.apache.org), [kibana](https://www.elastic.co/products/kibana)). But we wanted to save our logs to simple txt files without any additional complex stuff.

There are nice project which allows automatically read docker container logs and push them to one syslog server - [logspout](https://github.com/gliderlabs/logspout).

We tried to find simple syslog server without success. So there is a [small syslog server](https://github.com/eapyl/syslog-collector) written in dotnet core. I used serilog to write logs to files ([Rolling File](https://github.com/serilog/serilog-sinks-rollingfile)).

So docker-compose configuration looks like:
```yaml
  #another containers
  logspout:
    image: gliderlabs/logspout
    command: syslog+tcp://logcollector:5000
    volumes:
      - /var/run/docker.sock:/var/run/docker.sock
    depends_on:
      - logcollector
  logcollector:
    image: eapyl/syslog-collector
    volumes: 
      - $PWD/../logs:/log/logs
```

After that logs from all our containers are in logs folder! Very simple and easy!

Please find docker image [here](https://hub.docker.com/r/eapyl/syslog-collector/).

P.S. We started to write log message to stdout or stderr using [serilog](https://github.com/serilog/serilog-sinks-literate). So it was just small configuration change in all our projects to use Serilog.Literate instead of Serilog.RollingFile .

Thanks!