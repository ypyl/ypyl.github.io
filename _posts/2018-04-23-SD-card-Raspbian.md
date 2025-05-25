---
layout: post
title: Prepare SD card for Raspbian
date: 2018-04-23

tags: raspberrypi
categories: iot
---
Steps:
1. Download lates version of OS from [here](https://www.raspberrypi.org/downloads/raspbian/);
2. Follow instructions from [here](https://www.raspberrypi.org/documentation/installation/installing-images/README.md).
3. To enable ssh need to create empty `/boot/ssh` file;
4. To turn on WiFi need to create `/boot/wpa_supplicant.conf`:

```bat
ctrl_interface=DIR=/var/run/wpa_supplicant GROUP=netdev
update_config=1
country=AU

network={
    ssid="ESSID"
    psk="Your_wifi_password"
}
```

5. Put SD card to Raspberry Pi.

Thanks.
