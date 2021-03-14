---
layout: post
title: "How to set up SQL code coverage"
date: 2017-12-15

tags: dotnet sql
categories: programming
---
If you are creating a lot of SQL code, it is a good idea to do unit testing for it and see [code coverage](https://en.wikipedia.org/wiki/Code_coverage). Luckly there is two nice tools/libs to do it: [tSQLt](http://tsqlt.org/) and [SQLCover](https://github.com/GoEddie/SQLCover).

In this article I will show how to create/set up SQLCover and run it for each CI build.
The basic idea is we are creating command line application which runs our tSQLt unit tests via SQLCover and generates HTML report with code coverage information.

Unfortunately there is no SQLCover package in NuGet, but we can download it from [here](http://the.agilesql.club/SQLCover/download.php).

Create a new command line application, add references to SQLCover.dll and Microsoft.SqlServer.TransactSql.ScriptDom.dll.
After that there is an example of Program.cs:
{% highlight cs %}
    private const string RunAllSqlTestsCommand = "exec tSQLt.RunAll;";
    private const string DefaultDatabaseName = "MyDB";

    private const string ReportName = "SQLCoverage.html";

    static void Main(string[] args)
    {
        var connectionString = ConfigurationManager.ConnectionStrings[DefaultDatabaseName].ConnectionString;
        var coverage = new CodeCoverage(connectionString, DefaultDatabaseName);
        var result = coverage.Cover(RunAllSqlTestsCommand);
        var updatedResult =  result.Html();
        File.WriteAllText(ReportName, updatedResult);
    }
{% endhighlight %}

After execution you will see SQLCoverage.html with code coverage statistics in the same folder.

One note from SQLCover owner: "When we target local sql instances we delete the trace files but when targetting remote instances we are unable to delete the files as we do not (or potentially) do not have access. If this is the case keep an eye on the log directory and remove old "SQLCover-Trace-.xel" and "SQLCover-Trace-.xem" files."

Thanks.