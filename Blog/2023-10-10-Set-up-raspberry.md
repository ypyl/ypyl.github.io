---
layout: post
title: Set up raspberry
date: 2023-10-10
categories: raspberry
tags: raspberry
---

# Setup SD

Install raspberry os - https://www.raspberrypi.com/software/

Create `wpa_supplicant.conf` with:

```
ctrl_interface=DIR=/var/run/wpa_supplicant GROUP=netdev
update_config=1
ap_scan=1
fast_reauth=1
country=PL
network={
    ssid="WIFI Name"
    psk="WIFI password"
    id_str="0"
    priority=100
}
```

Create empty `ssh` file

Copy both files to flash

# Mount HDD

```
#For everyone well versed in Linux and BASH, here's everything up front

#Find the drive (in our case /dev/sda1)
sudo fdisk -l
#install NTFS-3g
sudo apt-get install ntfs-3g
#Make the mount directory and manage it's owner
sudo mkdir /mnt/hdd
sudo chown pi:pi /mnt/hdd
#Mount the drive
sudo mount -t ntfs-3g -o uid-pi,gid-pi /dev/sda1 /mnt/hdd
#Now you're done but it's not permanent

#Making it permanent
#Edit file system table
sudo nano /etc/fstab

#Add this line of text after the SD card partitions
/dev/sda1	/mnt/hdd	ntfs-3g	uid=pi,gid=pi	0	0
#hit ctrl-o to save and ctrl-x to exit nano

#Now the mounting will be restored on reboot
#Reboot to test it
sudo shutdown -r now
```

# Samba

```
sudo apt install samba samba-common-bin

sudo cp /etc/samba/smb.conf /etc/samba/smb.conf_backup

sudo nano /etc/samba/smb.conf
```

Add to the end of the file

```
[share]
Comment = Raspberry Pi Shared Folder
Path = /mnt/hdd
Browseable = yes
Writeable = Yes
only guest = no
create mask = 0777
directory mask = 0777
Public = yes
Guest ok = yes
```

```
sudo service smbd restart

sudo shutdown -r now
```

# transmission

```
sudo apt install transmission-daemon

sudo systemctl stop transmission-daemon

sudo chown -R pi:pi /mnt/hdd/torrents/downloads
sudo chown -R pi:pi /mnt/hdd/torrents/incomplete

sudo nano /etc/transmission-daemon/settings.json
```

Modify in settings
```
"incomplete-dir": "/mnt/hdd/torrents/incomplete",
"download-dir": "/mnt/hdd/torrents/downloads",
"incomplete-dir-enabled": true,
"rpc-password": "Your_Password",
"rpc-username": "Your_Username",
"rpc-whitelist": "192.168.*.*",
```

```
sudo nano /etc/init.d/transmission-daemon
```

Edit file:
```
USER=pi
```

```
sudo nano /etc/systemd/system/multi-user.target.wants/transmission-daemon.service
```

Edit file:
```
user=pi
```

```
sudo systemctl daemon-reload
sudo chown -R pi:pi /etc/transmission-daemon
sudo mkdir -p /home/pi/.config/transmission-daemon/
sudo ln -s /etc/transmission-daemon/settings.json /home/pi/.config/transmission-daemon/
sudo chown -R pi:pi /home/pi/.config/transmission-daemon/
sudo systemctl start transmission-daemon
sudo shutdown -r now
```
