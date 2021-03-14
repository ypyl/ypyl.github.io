---
layout: post
title: "Use LogParser"
date: 2011-05-05

tags: dotnet
categories: programming
---
1. Download LogParser 2.2.
2. C:\>tlbimp LogParser.dll /out:Interop.MSUtil.dll
3. Include Interop.MSUtil.dll in a project.
4.

```cs
using LogQuery = Interop.MSUtil.LogQueryClassClass;
using IISW3CInputFormat = Interop.MSUtil.COMIISW3CInputContextClassClass;
using CsvOutputFormat = Interop.MSUtil.COMCSVOutputContextClassClass;
using LogRecordSet = Interop.MSUtil.ILogRecordset;

LogQuery oLogQuery = new LogQuery();

// Instantiate the Event Log Input Format object
IISW3CInputFormat oW3CInputFormat = new IISW3CInputFormat();

string query = String.Format(_queryPostOverall, _pathToLog, "200", _date);
var oRecordSet = oLogQuery.Execute(query, oW3CInputFormat);
for (; !oRecordSet.atEnd(); oRecordSet.moveNext())
{
   double count = Convert.ToDouble(oRecordSet.getRecord().getValue("count1"));
   _postCountOverall.overall += count;
}
oRecordSet.close();
```