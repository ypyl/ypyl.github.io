---
layout: post
title: "Windrunner Links a WalkingPad Treadmill to World of Warcraft"
date: 2026-11-09
categories: news
---

A developer has published **Windrunner + WalkingPad Control** on GitHub, a pair of tools that make a treadmill and World of Warcraft drive each other. The Windows app controls Xiaomi and KingSmith walking pads over Bluetooth (A1, A1 Pro, and other WiLink models), and the Windrunner addon tracks walking time, distance, and speed in-game, with a **Hardcore mode** that keeps movement keys locked until a training session starts.

The link runs in both directions: the belt can set the character's pace, or the game can set the belt's, where W walks, W+Shift sprints, and the pad follows at proportional speed. WoW addons cannot talk to other programs, so Windrunner draws three small colored squares in the screen corner for the app to read, and the app sends belt speed back by emulating F11/F12 and numpad `/` keystrokes once a second. A third mode ignores games and only tracks the walk, and the addon runs on Retail, Forever, and Classic clients. The code is MIT licensed.

[Windrunner + WalkingPad Control](https://github.com/adunis/windrunner-walkingpad-)
