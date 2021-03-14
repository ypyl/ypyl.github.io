---
layout: post
title: "InfluxDB can't be started as a service"
date: 2016-12-21

tags: indluxdb
categories: deployment
---
I was not able to run influxdb as a service after [installing](https://docs.influxdata.com/influxdb/v1.1/introduction/installation/) influxdb on my local ubuntu:

Command `sudo service influxd status` showed:

```none
● influxdb.service - InfluxDB is an open-source, distributed, time series database
   Loaded: loaded (/lib/systemd/system/influxdb.service; enabled; vendor preset: enabled)
   Active: inactive (dead) (Result: exit-code) since Fri 2016-12-23 15:05:34 CET; 3min 47s ago
     Docs: https://docs.influxdata.com/influxdb/
  Process: 13938 ExecStart=/usr/bin/influxd -config /etc/influxdb/influxdb.conf ${INFLUXD_OPTS} (code=exited, status=1/FAILURE)
 Main PID: 13938 (code=exited, status=1/FAILURE)

Dec 23 15:05:34 fes-U36SG systemd[1]: influxdb.service: Main process exited, code=exited, status=1/FAILURE
Dec 23 15:05:34 fes-U36SG systemd[1]: influxdb.service: Unit entered failed state.
Dec 23 15:05:34 fes-U36SG systemd[1]: influxdb.service: Failed with result 'exit-code'.
Dec 23 15:05:34 fes-U36SG systemd[1]: influxdb.service: Service hold-off time over, scheduling restart.
Dec 23 15:05:34 fes-U36SG systemd[1]: Stopped InfluxDB is an open-source, distributed, time series database.
Dec 23 15:05:34 fes-U36SG systemd[1]: influxdb.service: Start request repeated too quickly.
Dec 23 15:05:34 fes-U36SG systemd[1]: Failed to start InfluxDB is an open-source, distributed, time series database.
```

But the simple command `influxd` worked well.

I found that there is the next code in `/lib/systemd/system/influxdb.service`:

```none
# If you modify this, please also make sure to edit init.sh

[Unit]
Description=InfluxDB is an open-source, distributed, time series database
Documentation=https://docs.influxdata.com/influxdb/
After=network-online.target

[Service]
User=influxdb
Group=influxdb
LimitNOFILE=65536
EnvironmentFile=-/etc/default/influxdb
ExecStart=/usr/bin/influxd -config /etc/influxdb/influxdb.conf ${INFLUXD_OPTS}
KillMode=control-group
Restart=on-failure

[Install]
WantedBy=multi-user.target
Alias=influxd.service
```

All is working well after I commended the next two lines:

```none
[Service]
# User=influxdb
# Group=influxdb
```

Thanks!