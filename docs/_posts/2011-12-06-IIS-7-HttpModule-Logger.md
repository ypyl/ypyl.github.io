---
layout: post
title: "IIS 7 HttpModule Logger"
date: 2011-12-06

tags: iis
categories: administration
---
Простой модуль-лог для IIS 7 (Classic mode)

```cs
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;
using System.Threading.Tasks;
using System.IO;
using System.Threading;
using System.Net;
using ReutersKnowledge.Web.Services.Util;
using System.Collections.Specialized;
using System.Reflection;
 
namespace IISWsgLogger
{
    public class IISLoggerModule : IHttpModule
    {
        private static string fileName = "D:\\Log.txt";
 
        private static ConcurrentQueue<string> logRecords = new ConcurrentQueue<string>();
 
        private static object syncTask = new object();
        private static Task taskLog;
 
        public void Init(HttpApplication context)
        {
            if (taskLog == null)
            {
                lock (syncTask)
                {
                    if (taskLog == null)
                    {
                        taskLog = Task.Factory.StartNew(() => StartLog(), TaskCreationOptions.LongRunning);
                    }
                }
            }
            context.BeginRequest += new EventHandler(OnPreRequestHandlerExecute);
            context.EndRequest += new EventHandler(OnPostReleaseRequestState);
        }
 
        private void StartLog()
        {
            var t = File.AppendText(fileName);
            t.WriteLine("date time wsg-guid s-port cs-method reuters-uuid cs(Host) cs-uri-stem cs-uri-query sc-status sc-substatus cs(User-Agent) cs(Cookie) TimeTakenMS sc-bytes");
            int waitMil = 1000;
            string res = null;
            try
            {
                while (true)
                {
                    while (logRecords.TryDequeue(out res))
                    {
                        t.WriteLine(res);
                    }
                    Thread.Sleep(waitMil);
                }
            }
            finally
            {
                t.Close();
            }
 
        }
 
        public void Dispose()
        {
        }
 
        public void OnPreRequestHandlerExecute(Object source, EventArgs e)
        {
            HttpApplication app = (HttpApplication)source;
            var guid = Guid.NewGuid().ToString();
            app.Context.RewritePath(app.Context.Request.FilePath, app.Context.Request.PathInfo, "guid=" + guid);
        }
 
        public void OnPostReleaseRequestState(Object source, EventArgs e)
        {
            HttpApplication app = (HttpApplication)source;
            var time = DateTime.Now;
            var timeStamp = app.Context.Timestamp;
            var port = app.Context.Request.Url.Port;
            var typeOfRequest = app.Context.Request.RequestType;
            var Guid = app.Context.Request.QueryString[0];
            var host = app.Context.Request.Url.Host;
            var rawUrl = app.Context.Request.RawUrl;
            var contentRequestLength = app.Context.Request.ContentLength;
            var status = app.Context.Response.StatusCode;
            var agent = app.Context.Request.UserAgent;
            var cookies = string.Empty;
            foreach (var cookie in app.Context.Request.Cookies)
            {
                cookies += cookies;
            }
 
            logRecords.Enqueue(string.Format("{0} {1} {3} {4} {5} {6} {7} {8} {9} {10}", time, guid, port, typeOfRequest, host, rawUrl, status, agent, timeStamp, contentRequestLength));
        }
 
    }
}
```

web.config :

``xml
<?xml version="1.0"?>
<configuration>
 <configSections>
  ...
 </configSections>
 <system.web>
  ...
   <httpModules>
   ...
  </httpModules>
 </system.web>
 <system.webServer>
  <modules>
   <add name="IISLoggerModule" type="IISLogger.IISLoggerModule, IISLogger.IISLoggerNamespace.IISLogger"/>
```