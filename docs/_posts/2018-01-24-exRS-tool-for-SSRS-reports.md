---
layout: post
title: "exRS tool to deploy SSRS reports and subscriptions"
date: 2018-01-24
tags: dotnet ssrs mssql
categories: programming
---
There is a nice tool to deploy reports and/or subscription to SSRS - [RS tool](https://docs.microsoft.com/en-us/sql/reporting-services/tools/rs-exe-utility-ssrs). And I described a way to use this tool [here]({% post_url 2017-09-19-Auto-deployment-for-SSRS-reports %}). 

But it is not so easy to support VB scripts, so there is a new tool [exRS](https://github.com/eapyl/exRS). This tool helps to deploy reports and create subscriptions for reports.

Currently there is a small console application with name exRSConsole.
> To deploy sample Report1 report and it's subscription from Report1.xml:
> exRSConsole.exe -n Report1 -p SampleReport -r -s

Also this tool allows to delete all folders at SSRS server:
> exRSConsole.exe -d

And backup all existing reports to local folder:
> exRSConsole.exe -b

To configure exRS you need to change configuration file:
{% highlight bat %}
<applicationSettings>
    <exRS.Properties.Settings>
        <!-- URI of SSRS server -->
        <setting name="exRS_SSRSService_ReportingService2010" serializeAs="String">
            <value>http://desktop-name:80/ReportServer/ReportService2010.asmx</value>
        </setting>
    </exRS.Properties.Settings>
</applicationSettings>
<userSettings>
    <exRS.Properties.Settings>
        <!-- a user to connect to SSRS server -->
        <setting name="SSRSUser" serializeAs="String">
            <value>desktop-name\user</value>
        </setting>
        <!-- a password to connect to SSRS server -->
        <setting name="SSRSPassword" serializeAs="String">
            <value></value>
        </setting>
        <!-- a path to folder where put reports and subscriptions during backup process -->
        <setting name="backupPath" serializeAs="String">
            <value>C:\Temp</value>
        </setting>
        <!-- a name of folder with subscription configuration for reports (near exRSConsole.exe) -->
        <setting name="subscriptionSettingsFolder" serializeAs="String">
            <value>Subscriptions</value>
        </setting>
        <!-- a user name to connect to DB for data sources -->
        <setting name="dbUserForDataSource" serializeAs="String">
            <value>test</value>
        </setting>
        <!-- a password name to connect to DB for data sources -->
        <setting name="dbPasswordForDataSource" serializeAs="String">
            <value>test</value>
        </setting>
        <!-- a path at SSRS server where put reports created by subscription -->
        <setting name="subscriptionFileSharePath" serializeAs="String">
            <value>\\DESKTOP-NAME\Temp</value>
        </setting>
        <!-- a name of folder with reports (*.rdl, *rds and *.rsd files) (near exRSConsole.exe) -->
        <setting name="sourceFolderPath" serializeAs="String">
            <value>.\Reports</value>
        </setting>
        <!-- a name of folder at SSRS server with data sources -->
        <setting name="datasourcesServerFolderName" serializeAs="String">
            <value>Data Sources</value>
        </setting>
        <!-- a name of folder at SSRS server with datasets-->
        <setting name="datasetsServerFolderName" serializeAs="String">
            <value>Datasets</value>
        </setting>
    </exRS.Properties.Settings>
</userSettings>
{% endhighlight %}

After deployment you should see 3 folders in SSRS: "Data Sources", "Datasets" and "SampleReport".
Please find the source code [here](https://github.com/eapyl/exRS).

Thanks.