---
layout: post
title: "Notes from 'Introduction to Linux'"
date: 2015-05-21

tags: linux
categories: administration
---
[course link](https://www.edx.org/course/introduction-linux-linuxfoundationx-lfs101x-0)

Linux [Filesystem](http://www.tldp.org/LDP/sag/html/filesystems.html)

* Conventional disk filesystems: ext2, ext3, ext4, XFS, Btrfs, JFS,NTFS, etc.
* Flash storage filesystems: ubifs, JFFS2, YAFFS, etc.
* Database filesystems
* Special purpose filesystems: procfs, sysfs, tmpfs, debugfs, etc.

[Partitions](https://en.wikipedia.org/wiki/Disk_partitioning)

1. Windows
    * Partition: Disk1 
    * Filesystem type: NTFS/FAT32
    * Mounting Parameters: DriveLetter 
    * Base Folder of OS: C drive
2. Linux
    * Partition: /dev/sda1
    * Filesystem type: EXT3/EXT4/XFS
    * Mounting Parameters: MountPoint
    * Base Folder of OS: /

[The Filesystem Hierarchy Standard](https://en.wikipedia.org/wiki/Filesystem_Hierarchy_Standard)

![File System](/images/linux_foundation_filesystem.jpg)

The Boot Process

![Boot Process](/images/linux_foundation_boot_process.jpg)

Choosing a [Linux Distribution](https://en.wikipedia.org/wiki/Linux_distribution)

![Choose](/images/linux_foundation_choose.jpg)