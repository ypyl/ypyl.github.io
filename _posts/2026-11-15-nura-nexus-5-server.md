---
layout: post
title: "The Flash Is the Cheap Part: Nura on a 2013 Nexus 5"
date: 2026-11-15
categories: programming
tags: [nura, postmarketos, nexus-5, linux, self-hosting, arm]
---

The flash took four minutes. Getting a shell that survived a reboot took the next two hours.

I had wanted to run Linux on an old phone for years and never started, for a boring reason: the gap between wanting it and knowing every nuance felt too wide. Which bootloader, which partition, which kernel argument, which of the dozen commands in a guide is the one that actually matters. That is exactly the kind of reading an agent can absorb, so I spent a morning supervising instead of a weekend learning: the commands and the research below came from the model, DeepSeek v4.1 Flash, and my part was reading each proposal, deciding whether it was safe to accept, and pressing the buttons on the phone.

The phone is an LG Nexus 5 from 2013 (`lg-hammerhead`): Snapdragon 800, 1.8 GB of usable RAM, 32-bit ARM, last security patch in 2016. It now runs **Nura**, which is postmarketOS under its new name. The times below are UTC from the session log, because the shape of the day is the point: the part I planned for took fifteen minutes, and the part I did not plan for took the rest.

## The night before, the tier decided the work

Nura grades devices by tier. `lg-hammerhead` is **testing**: WiFi, screen, USB networking and OTG work; audio is absent, battery reporting is partial, and there is no cpufreq. Prebuilt images only exist for main and community tier devices, so a testing-tier phone means running the build yourself.

The alternative is **Termux** on stock Android: half an hour, reversible, and not a server, because an app cannot bind port 53 or hold a `tun` device. One detail looked like taste and was not: the `console` UI. No compositor, no GPU bug, and no way to type on the phone either, which I would meet at 10:08.

## 09:12: Windows could not see the bootloader

`adb` worked. `fastboot devices` returned nothing, while the phone sat on `FASTBOOT MODE` with an open bootloader. Device Manager had the answer:

```text
USB\VID_18D1&PID_4EE0\0865F5C70245C4DA
Status: Error   Problem: CM_PROB_FAILED_INSTALL (28)
```

`adb` worked because that interface has a composite driver; the bootloader interface had none. Google's USB driver package covers that exact hardware ID:

```powershell
pnputil /add-driver android_winusb.inf /install
```

`fastboot getvar` then reported `hammerhead` on a `D821(H)`, with the boot chain already open, so no wipe and no bootloader update were needed. Storage was 6.9 GB free with an empty `/sdcard`, so nothing needed rescuing before a wipe.

## 09:27: prove the way out before you go in

The point of no return is one `fastboot flash userdata`, so the return path should be a fact and not a hope:

```text
https://dl.google.com/dl/android/aosp/hammerhead-m4b30z-factory-625c027b.zip
HTTP 200, 597,499,040 bytes
```

That is the last factory image for exactly the build the phone was running, and it restores Android with `flash-all`. Only `boot` and `userdata` get written; `aboot`, the modem and `persist` are never touched.

## 09:31: "unsupported" was one line

pmbootstrap's README says Linux only. Before fighting that, I tested what a build actually needs: loop devices, `mount` and `unshare` all worked in WSL2. ARM emulation was the only gap, and Ubuntu disables it with one systemd condition:

```text
qemu-arm-static.service: ConditionVirtualization=!wsl was not met
```

Registered by hand and proved with a real armv7 binary, exit code 0. Five minutes of testing replaced a detour through a VM.

## 09:43: the build needed a real terminal

`pmbootstrap install` allocates a pty and reads prompts from it, so piped stdout deadlocks it in `n_tty_read`. A build started from inside the WSL session also died with the session. Both are fixed by driving the tool from a detached process with a real terminal:

```python
child = pexpect.spawn('./pmbootstrap.py install', timeout=3600)
child.expect('New password for user')
child.sendline(PASSWORD)
child.expect('DONE!', timeout=3600)
```

That password prompt appears halfway through and nobody mentions it. After that it is downloads of prebuilt armv7 packages, nine minutes, no kernel compilation.

```bash
./pmbootstrap.py init     # lg / hammerhead, console UI, edge channel
./pmbootstrap.py install  # 9 minutes
./pmbootstrap.py export   # lk2nd.img 311 KB, lg-hammerhead.img 1.25 GB
```

## 10:02: four minutes of flashing

```bash
fastboot flash boot     lk2nd.img            # 311 KB, second-stage bootloader
fastboot flash userdata lg-hammerhead.img    # 1.25 GB, the whole system
fastboot reboot
```

The output looks alarming and is not:

```text
Sending sparse 'userdata' 1/1 (547476 KB)   OKAY [ 28.249s]
Writing 'userdata'                    OKAY [183.375s]
```

547 MB of a 1.25 GB image. I read that as a truncated write and started planning a reflash. It is sparse encoding: the ext4 image is mostly empty space, and 547 MB was the real non-zero payload. The proof that the flash was complete is not in the output, it is a `login:` prompt on the phone.

{::nomarkdown}
<figure>
<svg xmlns="http://www.w3.org/2000/svg" viewBox="0 0 960 320" role="img" style="width:100%;height:auto" font-family="-apple-system,BlinkMacSystemFont,'Segoe UI',Roboto,Helvetica,Arial,sans-serif">
  <title>Where the kernel lives on a Nexus 5 running Nura</title>
  <desc>aboot loads lk2nd from the boot partition. lk2nd reads extlinux.conf and the kernel from inside the ext4 image that occupies the userdata partition, then boots the rootfs from the same filesystem.</desc>
  <defs>
    <pattern id="dots" width="22" height="22" patternUnits="userSpaceOnUse">
      <circle cx="1" cy="1" r="0.9" fill="#E3E2DC"/>
    </pattern>
  </defs>
  <rect width="960" height="320" fill="#f5f4ed"/>
  <rect width="960" height="320" fill="url(#dots)" opacity="0.55"/>

  <text x="80" y="40" fill="#1B365D" font-size="14" font-weight="600" font-family="'JetBrains Mono','SF Mono','Fira Code',Consolas,Monaco,monospace" letter-spacing="3">FIGURE  1</text>
  <text x="200" y="40" fill="#504e49" font-size="14" font-family="'JetBrains Mono','SF Mono','Fira Code',Consolas,Monaco,monospace" letter-spacing="3">WHERE THE KERNEL ACTUALLY LIVES</text>
  <line x1="80" y1="52" x2="880" y2="52" stroke="#1B365D" stroke-width="0.8"/>

  <rect x="80" y="104" width="160" height="64" rx="6" fill="#faf9f5" stroke="#141413" stroke-width="1.5"/>
  <text x="160" y="134" text-anchor="middle" font-size="18" font-weight="600" fill="#141413">aboot</text>
  <text x="160" y="154" text-anchor="middle" font-size="14" fill="#6b6a64">open, HHZ20h</text>

  <rect x="80" y="208" width="160" height="64" rx="6" fill="#faf9f5" stroke="#141413" stroke-width="1.5"/>
  <text x="160" y="238" text-anchor="middle" font-size="18" font-weight="600" fill="#141413">boot</text>
  <text x="160" y="258" text-anchor="middle" font-size="14" fill="#6b6a64">lk2nd only</text>

  <rect x="320" y="88" width="528" height="160" rx="6" fill="#EEF2F7" stroke="#1B365D" stroke-width="1.5"/>
  <text x="344" y="120" font-size="14" font-weight="600" fill="#1B365D" font-family="'JetBrains Mono','SF Mono','Fira Code',Consolas,Monaco,monospace" letter-spacing="2">USERDATA · EXT4 IMAGE</text>

  <rect x="344" y="136" width="144" height="64" rx="6" fill="#faf9f5" stroke="#141413" stroke-width="1.5"/>
  <text x="416" y="166" text-anchor="middle" font-size="18" font-weight="600" fill="#141413">extlinux</text>
  <text x="416" y="186" text-anchor="middle" font-size="14" fill="#6b6a64">kernel cmdline</text>

  <rect x="512" y="136" width="144" height="64" rx="6" fill="#faf9f5" stroke="#141413" stroke-width="1.5"/>
  <text x="584" y="166" text-anchor="middle" font-size="18" font-weight="600" fill="#141413">kernel</text>
  <text x="584" y="186" text-anchor="middle" font-size="14" fill="#6b6a64">+ initramfs</text>

  <rect x="680" y="136" width="144" height="64" rx="6" fill="#faf9f5" stroke="#141413" stroke-width="1.5"/>
  <text x="752" y="166" text-anchor="middle" font-size="18" font-weight="600" fill="#141413">rootfs</text>
  <text x="752" y="186" text-anchor="middle" font-size="14" fill="#6b6a64">systemd, musl</text>

  <path d="M 160 168 V 208" fill="none" stroke="#1B365D" stroke-width="1.5"/>
  <path d="M 156 202 L 160 208 L 164 202" fill="none" stroke="#1B365D" stroke-width="1.5" stroke-linecap="round" stroke-linejoin="round"/>

  <path d="M 240 240 H 320" fill="none" stroke="#1B365D" stroke-width="1.5"/>
  <path d="M 314 236 L 320 240 L 314 244" fill="none" stroke="#1B365D" stroke-width="1.5" stroke-linecap="round" stroke-linejoin="round"/>

  <path d="M 488 168 H 512" fill="none" stroke="#504e49" stroke-width="1.5"/>
  <path d="M 506 164 L 512 168 L 506 172" fill="none" stroke="#504e49" stroke-width="1.5" stroke-linecap="round" stroke-linejoin="round"/>

  <path d="M 656 168 H 680" fill="none" stroke="#504e49" stroke-width="1.5"/>
  <path d="M 674 164 L 680 168 L 674 172" fill="none" stroke="#504e49" stroke-width="1.5" stroke-linecap="round" stroke-linejoin="round"/>
</svg>
</figure>
{:/nomarkdown}

The consequence is the two instructions that otherwise look arbitrary: never flash a kernel to `boot`, because the kernel is a file inside the filesystem; and edit kernel arguments in `/boot/extlinux/extlinux.conf`, because there is no boot image carrying a cmdline to patch.

## 10:08: it booted, and there was no way in

The phone came up on the first attempt and offered no way to reach it. The `console` UI is a framebuffer `getty` with no graphical session, so there is no on-screen keyboard, and the USB gadget came up adb-only with no network adapter, even though `sshd` and `unudhcpd@usb0` were enabled inside.

The tempting move is to fix the gadget. Instead I mounted the image in WSL and put the credentials in it:

```bash
# inside the mounted rootfs, after the build and before flashing
install -m 600 /dev/stdin \
  ./etc/NetworkManager/system-connections/home.nmconnection <<'EOF'
[connection]
id=home
type=wifi
[wifi]
ssid=<ssid>
[wifi-security]
key-mgmt=wpa-psk
psk=<psk>
[ipv4]
method=auto
EOF

install -d -m 700 -o 10000 -g 10000 ./home/user/.ssh
install -m 600 -o 10000 -g 10000 id_ed25519.pub ./home/user/.ssh/authorized_keys
```

Then reflash `userdata` only, eleven minutes. At 10:25 the phone was on the LAN and SSH worked on the first attempt, and the root filesystem read 110 MB free. One reboot later it showed 10.8 GB free: the first-boot resize had not run yet.

The gadget is the worse debugging target, which decided the order. Iterating on configfs offline means writing a script blind, flashing 1.25 GB and watching a screen you cannot read; iterating from a shell takes seconds. So get *any* shell first, even a mediocre one, and bake in both unknowns at once.

The cost: the WiFi key is now plaintext in `/etc/NetworkManager/system-connections/`, which is where every Linux laptop keeps it. Use a guest network, or rotate the key afterwards.

## 10:51: a Windows driver was eating the network gadget

With a shell in hand, the gadget turned out to be healthy and Windows wrong, and the evidence was one line in Google's driver package:

```text
android_winusb.inf:54:  %CompositeAdbInterface% = USB_Install, USB\VID_18D1&PID_D001
```

Nura reuses Android's vendor and product ID for its NCM network gadget, and Google's driver claims that ID outright, binding WinUSB to the whole composite device so the network function never appears. That driver is also what `fastboot` needs, so the fix belongs on the phone:

```bash
# before: 18d1:d001, claimed by android_winusb.inf
# after:  1d6b:0104, the standard Linux composite gadget
echo 0x1d6b > /sys/kernel/config/usb_gadget/g1/idVendor
echo 0x0104 > /sys/kernel/config/usb_gadget/g1/idProduct
```

Windows then got `172.16.42.2` from the phone's own DHCP server, and `ssh user@172.16.42.1` worked over the cable. Two access paths, and Google's driver stays installed for the next reflash.

## 10:56: the device was rebooting itself, and taking my writes with it

Five boots in ten minutes. The first came seconds after I offlined `cpu2` and `cpu3`, with `rcu_preempt detected expedited stalls` right behind it. The port has no cpufreq, so offlining two cores had looked like a heat lever for a phone that lives on a charger. It was the cause, and it was not even reversible: writing `1` back returned `EIO`, so undoing it needed a reboot. If heat becomes a problem on this board, reduce what runs on it.

The reboots left a quieter bug. A systemd unit was written, reported 292 bytes, and read back as 0. Abrupt reboots lost page cache with the writes still in it, and `systemctl` then called the empty units masked, which reads as a config error and is not one. Write, then `sync`, then read the size back before trusting the exit code.

Two smaller traps in the same family: there is no bash, only busybox `sh`, and `systemctl restart sshd` kills every process in the service cgroup, including the SSH session that issued it. Use `reload`.

## 11:26: the keyboard that never rendered

`buffyboard`, the on-screen keyboard packaged for exactly this UI, ran and held the right devices. Your taps landed on the right keys. What reached the panel was a black rectangle, and neither a light theme nor `fbdev_force_refresh` changed it. The panel is `msmdrmfb` with a stride of 4352 (1088 pixels, not 1080), and the device package ships its own force-refresh hack, which tells you nobody has solved this corner.

So I stopped it, added `consoleblank=60` to `/boot/extlinux/extlinux.conf` so the backlight does not burn for a screen nobody reads, and treated the phone as headless. At 11:46 it came back from a clean reboot with everything in place, unattended.

## 14:37: read sensors by type, never by index

A thermal read failed and the sensor was innocent. Zone numbers follow device tree order, so they move between kernels: here `thermal_zone4` is `wlan` and the CPU zones are `thermal_zone5` through `8`. Anything hardcoded to an index from an older note reads the wrong sensor or a dead one:

```bash
for z in /sys/class/thermal/thermal_zone*; do
  printf '%-22s %s C\n' "$(cat $z/type)" "$(awk '{print int($1/1000)}' $z/temp 2>/dev/null || echo unreadable)"
done
```

Which is how I found `thermal_zone11`, type `battery`, at a read error with the kernel's own explanation: `Unable to get temperature, disabling!`. Battery temperature is not available on this port by any route: the zone is disabled, the `hwmon` node returns no data, the charger exposes no input. Capacity and voltage work. That matters because charging is capped at 4.0 V and 512 mA to keep a 2013 cell alive on permanent power, and the device cannot report the cell's temperature.

## What the port breaks, and what a server needs

| Broken | Does a headless server care |
|---|---|
| No audio at all | no |
| No camera | no |
| Partial 3D acceleration | no, nothing starts a compositor |
| Unreliable calls | no |
| Partial battery reporting | partly: capacity and voltage work, temperature does not |
| No cpufreq | yes, heat is managed by doing less |
| 32-bit armv7 only | yes, no arm64 images |

Most of a phone port's defects are phone defects. What survives into server use is the CPU and memory side, and that side is fine: 1.8 GB of RAM with 2.8 GB of zram, an eMMC instead of an SD card, and a battery that behaves like a small UPS. Four settings did the real work, and all four survive a reboot: key-only SSH, a pinned WiFi MAC, the 4.0 V charge cap and the gadget rebadge.

The price is a device with no thermal visibility into its own battery, no cpufreq, and a port that its own project files under `testing`. Fair trade for a box that costs nothing and reflashes in twenty minutes. The one number I watch is `uptime`, because if it reboots with four cores and nothing hotplugged, the instability belongs to the port and not to me.

[postmarketOS / Nura device page for the Nexus 5](https://wiki.postmarketos.org/wiki/Google_Nexus_5_(lg-hammerhead)) · [Nura project state](https://nura.eco/state) · [Nexus 5 factory images](https://developers.google.com/android/images)
