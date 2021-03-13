---
layout: post
title: "Backup InfluxDB in Docker"
date: 2017-04-15
categories: programming docker
---
If you want to backup your InfluxDB in Docker you can probably can start with this wonderful [article](https://devblog.digimondo.io/how-to-backup-influxdb-running-in-a-docker-container-615938dbab90?gi=78ccabcf77cf).

Later I will just describe how I am doing backup to local folder instead of S3 storage.

It is my docker-compose:

{% highlight docker %}
    db:
        image: "influxdb:1.2"
        restart: always
        ports: 
            - "8086:8086"
        volumes:
            - /etc/influxdb:/var/lib/ #folder with db data
            - ./influxdb.conf:/etc/influxdb/influxdb.conf:ro #configuration for InfluxDB
    influxdb-backup:
        image: influxdb-backup:1.2
        container_name: influxdb-backup
        restart: always
        volumes:
            - /backup:/backup #folder with backups
        links:
            - db
        environment:
            INFLUX_HOST: db
            DATABASES: entity_db
{% endhighlight %}

So there are two containers: influxdb is the standard [InfluxDB](https://docs.influxdata.com/influxdb/v1.2/introduction/getting_started/) and influxdb-backup:1.2 (see dockerfile for this image below).

{% highlight docker %}
FROM influxdb:1.2-alpine

# Backup the following databases, separator ":"
ENV DATABASES=entity_db
ENV INFLUX_HOST=influxdb

# Some tiny helpers 
RUN apk update && apk add ca-certificates && update-ca-certificates && apk add openssl
RUN apk add --no-cache bash py2-pip py-setuptools ca-certificates
RUN pip install python-magic

# Backup script
COPY backup.sh /bin/backup.sh
RUN chmod +x /bin/backup.sh

# Setup crontab
COPY cron.conf /var/spool/cron/crontabs/root

# Run Cron in foreground
CMD crond -l 0 -f
{% endhighlight %}

So we are running cron job to run our `backup.sh`. The configuration (cron.conf):
{% highlight bat %}
# do daily/weekly/monthly maintenance
# min	hour	day	month	weekday	command
0 0 * * * /bin/backup.sh
# run every day at 00:00:00
{% endhighlight %}

There is backup.sh with the next content:
{% highlight bat %}
#!/bin/bash
set -e

: ${INFLUX_HOST:?"INFLUX_HOST env variable is required"}

dir=/backup
min_dirs=16
#we are saving only last 14 backups (2 weeks)

if [ $(find "$dir" -maxdepth 1 -type d | wc -l) -ge $min_dirs ]
  then find "$dir" -maxdepth 1 | sort | head -n 2 | sort -r | head -n 1 | xargs rm -rf
fi

#all backups are in /backup folder
#every backup is in a folder with name which is date when a backup has been created
DATE=`date +%Y-%m-%d-%H-%M-%S`

echo 'Backup Influx metadata'
influxd backup -host $INFLUX_HOST:8088 /backup/$DATE

# Replace colons with spaces to create list.
for db in ${DATABASES//:/ }; do
  echo "Creating backup for $db"
  influxd backup -database $db -host $INFLUX_HOST:8088 /backup/$DATE
done
{% endhighlight %}

Please find an image [here](/assets/inluxDbBackupDocker.zip).

Thank you!