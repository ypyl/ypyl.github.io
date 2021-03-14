---
layout: post
title: "0x80004005 Failed to Execute URL"
date: 2011-09-12
tags: linux dotnet
categories: programming
---

The error was connected with GET requests. ApplicationPool was set in Classic mode.
There is HttpModule, which throws this type of exceptions:

{% highlight csharp %}
System.Web.HttpException (0x80004005): Failed to Execute URL.
at System.Web.Hosting.ISAPIWorkerRequestInProcForIIS6.BeginExecuteUrl(String url, String method, String childHeaders, Boolean sendHeaders, Boolean addUserIndo, IntPtr token, String name, String authType, Byte[] entity, AsyncCallback cb, Object state)
at System.Web.HttpResponse.BeginExecuteUrlForEntireResponse(String pathOverride, NameValueCollection requestHeaders, AsyncCallback cb, Object state)
at System.Web.DefaultHttpHandler.BeginProcessRequest(HttpContext context, AsyncCallback callback, Object state)
at System.Web.HttpApplication.CallHandlerExecutionStep.System.Web.HttpApplication.IExecutionStep.Execute()
at System.Web.HttpApplication.ExecuteStep(IExecutionStep step, Boolean& completedSynchronously)
{% endhighlight %}

contained the next source code in event context.BeginRequest:

{% highlight csharp %}
app.Context.RewritePath(app.Context.Request.Path);
{% endhighlight %}

The solve of the problem is:

{% highlight csharp %}
app.Context.RewritePath(app.Context.Request.FilePath, app.Context.Request.PathInfo, string.Empty);
{% endhighlight %}