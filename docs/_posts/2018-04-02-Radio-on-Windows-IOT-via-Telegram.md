---
layout: post
title: "Radio on Windows IOT via Telegram Bot"
date: 2018-04-02

tags: dotnet
categories: programming
---
# Radio on Windows IOT via Telegram Bot

This article describes and share a source code of application to play radio using Windows IOT. It is possible to control application using Telegram Bot.

Resources:  
1. Rasperry PI 3 with installed Windows IOT;
2. Portable [JBL Charge 3](https://www.amazon.com/JBL-JBLCHARGE3BLKAM-Waterproof-Portable-Bluetooth/dp/B01F24RHF4?psc=1&SubscriptionId=AKIAILSHYYTFIVPWUY6Q&tag=duckduckgo-d-20&linkCode=xm2&camp=2025&creative=165953&creativeASIN=B01F24RHF4);

Developed radio application is [background application](https://docs.microsoft.com/en-us/windows/iot-core/develop-your-app/backgroundapplications). It consists from 2 parts:
1. [Telegram Bot](https://github.com/TelegramBots/telegram.bot) to control radio. Supported commands:
  * `/start` is to start play;
  * `/stop` is to stop application;
  * `/pause` is to pause playing;
  * `/stations` is to show list of radio stations;
  * `/up` is to increase volume;
  * `/down` is to decrease volume;
2. Radio player is to play radio. It based on [Windows-IoT-Core-Ignite](https://github.com/gloveboxes/Windows-IoT-Core-Ignite).

Example of the bot in Telegram:
![image](/images/radio_bot.png)

The source code is on [GitHub](https://github.com/eapyl/WinIOTRadio).

Thanks.
