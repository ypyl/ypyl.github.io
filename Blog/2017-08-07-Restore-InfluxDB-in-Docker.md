---
layout: post
title: "Restore InfluxDB in Docker"
date: 2017-08-07

tags: docker influxdb
categories: administration
---
It is not easy to restore influxdb in an official container. Unfortunately it is not possible directly. You need to restore db out of a container and mount restored db to the container.

```bat
# Restoring a backup requires that influxd is stopped (note that stopping the process kills the container).
docker stop "$CONTAINER_ID"

# Run the restore command in an ephemeral container.
# This affects the previously mounted volume mapped to /var/lib/influxdb.
docker run --rm \
--entrypoint /bin/bash \
-v "$INFLUXDIR":/var/lib/influxdb \
-v "$BACKUPDIR":/backups \
influxdb:1.3 \
-c "influxd restore -metadir /var/lib/influxdb/meta -datadir /var/lib/influxdb/data -database foo /backups/foo.backup"

# Start the container just like before, and get the new container ID.
CONTAINER_ID=$(docker run --rm \
--detach \
-v "$INFLUXDIR":/var/lib/influxdb \
-v "$BACKUPDIR":/backups \
-p 8086 \
influxdb:1.3
)
```

More info [here](https://gist.github.com/mark-rushakoff/36b4491f97b8781198da36752ecd949b).

Thanks.