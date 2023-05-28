---
layout: post
title: "How to create Discord bot"
date: 2017-02-18

tags: javascript discord
categories: programming
---
That is a manual how to create simple ping-pong discord bot using javascript (nodejs).

* Install nodejs and npm
* Create a folder for your discord bot
* Run `npm init` in the folder
* Install [Discrod.js](https://discord.js.org/#/): `npm install --save discordjs`
* Create index.js in this folder:

```js
/*
  A ping pong bot, whenever you send "ping", it replies "pong".
*/

// import the discord.js module
const Discord = require('discord.js');

// create an instance of a Discord Client, and call it bot
const bot = new Discord.Client();

// the token of your bot - https://discordapp.com/developers/applications/me
const token = 'your bot token here';

// the ready event is vital, it means that your bot will only start reacting to information
// from Discord _after_ ready is emitted.
bot.on('ready', () => {
  console.log('I am ready!');
});

// create an event listener for messages
bot.on('message', message => {
  // if the message is "ping",
  if (message.content === 'ping') {
    // send "pong" to the same channel.
    message.channel.sendMessage('pong');
  }
});

// log our bot in
bot.login(token);
```

* Got to https://discordapp.com/developers/applications/me and create your bot
* Put all needed values, click "Create Application". On the next page scroll down until you see "Create a bot user", click that.
* After that you will be able to copy a token of your bot. Copy it and post it in created index.js
* Go to https://discordapp.com/oauth2/authorize?&client_id=YOUR_CLIENT_ID_HERE&scope=bot&permissions=0. You should replace YOUR_CLIENT_ID_HERE with Client ID (it should be in App Details on the web page where you got your token).
* Add your bot to the server.
* Run created bot by command `node index.js`
* You should be able to see a bot in Discord at your server. Write 'ping' to the bot. The answer should be 'pong'.

Well done! You created your Discord bot! Thank you.

Here is a simple rss bot for discord:

```js
const Discord = require('discord.js');
var Store = require("jfs"); // using jfs to save already posted rss news
var db = new Store("rssfeeds");
const client = new Discord.Client();

var currentNews = [];
var postedNews = [];
var interval;

function log(message) {
    console.log(new Date() + ": " + message);
}

/// load all posted rss news
db.all(function (err, objs) {
    if (err) log(err);
    for (var id in objs) {
        postedNews.push({ id: id, value: objs[id] });
        log("Restored posted news " + id);
    }
    loadFeeds();
});

client.on('ready', () => {
    log('I am ready!');
    var generalChannel = client.channels.get("25466045464298784169786");
    if (!interval) {
        interval = setInterval(() => {
            if (currentNews.length > 0) {
                var newsToPost = currentNews.shift();
                generalChannel.sendMessage(newsToPost.title + " - " + newsToPost.link);
                db.save(newsToPost, function (err, id) {
                    if (err) log(err);
                    log("Saved posted news " + id);
                    postedNews.push({ id: id, value: newsToPost });
                });

                log("Post " + newsToPost.link);
                log("Left in array - " + currentNews.length);
            }
        }, 60000 * 30);
    }
});

setInterval(() => {
    log("Updating news");
    loadFeeds();
}, 60000 * 60 * 24);

setInterval(() => {
    while (postedNews.length > 1000) {
        var oldNewsToDelete = postedNews.shift();
        log("deleting " + oldNewsToDelete.id)
        db.delete(oldNewsToDelete.id, function (err) {
            if (err) log(err);
        });
    }
}, 60000 * 60 * 24);

client.login('MjgyNDg5MjQ0MDEyNzczMzc2.C4nUkw.Na6H7ZVrXbMZbXv4Wt9p8cZaj2Q');

// rss.json should contain information about rss, like:
//{
//    "bbc": {
//        "description" : "bbc news",
//        "url" : "https://bbs.com/rss"
//    }
//}
var rssFeeds = require("./rss.json");

/// load feeds from Resources
function loadFeeds() {
    for (var feedName in rssFeeds) {
        rssfeed(rssFeeds[feedName].url);
    }
}

function rssfeed(url) {
    var FeedParser = require('feedparser');
    var feedparser = new FeedParser();
    var request = require('request');
    request(url).pipe(feedparser);
    feedparser.on('error', function (error) {
        log(error);
    });
    feedparser.on('readable', function () {
        var stream = this;
        var meta = this.meta; // **NOTE** the "meta" is always available in the context of the feedparser instance
        var item;

        while (item = stream.read()) {
            if (postedNews && postedNews.some(x => x.value.title == item.title)) continue;
            if (currentNews.some(x => x.title == item.title)) continue;
            log('Add news to current news array ' + item.link);
            currentNews.push({
                title = "item.title,
                link: item.link,
                date = "item.date
            });
            if (currentNews.length > 100) currentNews.shift();
        }
    });
}
```

Links:
* [Creating a discord bot & getting a token](https://github.com/reactiflux/discord-irc/wiki/Creating-a-discord-bot-&-getting-a-token)
* [Discord.js](https://discord.js.org/#/)
* [Ping-Pong example](https://discord.js.org/#/docs/main/stable/examples/ping)
* [Discord Developers](https://discordapp.com/developers/applications/me)
* [Discord](https://discordapp.com/)