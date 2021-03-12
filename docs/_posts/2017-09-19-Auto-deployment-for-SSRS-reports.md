---
layout: post
title: "Auto deployment for SSRS reports"
date: 2017-09-19
categories: ssrs reports programming
---
If you work with SSRS it is not so easy to create CI build for SSRS project. It looks like the best approach is to use shared datasources, shared datasets and embedded datasets in reports which use shared datasets. Shared datasets should use stored procedures to receive data so you will be able to cover your SQL logic by unit tests.

So you will have the next types of files in your project:
1. *.rdl - reports
2. *.rsd - shared datasets
3. *.rds - shared datasources

To deploy them to clean SSRS server we can use [RS tool](https://docs.microsoft.com/en-us/sql/reporting-services/tools/rs-exe-utility-ssrs). And that is a bat file to call it:
{% highlight bat %}
REM SSRS server, you can provide also user and password to RS tool if you can't use Windows credentials (default behaviour)
set varServerPath=http://desktop-h33a7aa/reportserver
REM name of folder at SSRS server for reports
set varReportFolder=My_Reports
REM name of folder at SSRS server for datasets
Set varDatasetFolder=Datasets
REM name of folder at SSRS server for data sources
set varDataSourceFolder=Data Sources
REM path to a folder with reports, datasets and datasources
set varReportFilePath=.\Project\Reports\My_Reports

"C:\Program Files\Microsoft SQL Server Reporting Services\Shared Tools\rs.exe" -i Commonscript.rss -s %varServerPath% -v ReportFolder="%varReportFolder%" -v DataSetFolder="%varDatasetFolder%" -v DataSourceFolder="%varDataSourceFolder%" -v filePath="%varReportFilePath%" -e Mgmt2010
{% endhighlight %}

All logic is described in Commonscript.rss. It creates folders in SSRS server for reports, datasets and datasources (using CreateFolders method), later it reads all *.rds files in varReportFilePath and create datasources for each file if it doesn't exist. After it reads all *.rsd files in varReportFilePath, create datasets and connects a dataset with a datasource. In the end it read all *.rdl files in varReportFilePath, create reports and connects a report with a dataset.

{% highlight vb %}
Dim definition As [Byte]() = Nothing
Dim bytedefinition as [Byte]() = nothing
Dim warnings As Warning() = Nothing

'Main Entry point of utility
Public Sub Main()
    Console.WriteLine()
    Console.WriteLine("Initiating Deployment")
    rs.Credentials = System.Net.CredentialCache.DefaultCredentials
    Try
        'Create the shared data source
        CreateFolders(DataSourceFolder,"/","Data Sources","Visible")
        'Create the folder that will contain the shared data sets
        CreateFolders(DataSetFolder, "/", "Data Set Folder", "Visible")
        'Create the folder that will contain the deployed reports
        CreateFolders(ReportFolder, "/", "Report Folder","Visible")
    Catch goof As Exception
        Console.WriteLine(goof.Message)
    End Try
    ReadFiles(filepath, "*.rds")
    ReadFiles(filepath, "*.rsd")
    ReadFiles(filepath, "*.rdl")
End Sub
 
'Utility for creation of folders
Public Sub CreateFolders(ByVal folderName as string, ByVal parentPath as string, ByVal description as String, ByVal visible as string)
    Console.WriteLine()
    Console.WriteLine("Checking for Target Folders")
    'CatalogItem properties
    Dim descriptionProp as new [Property]
    descriptionProp.Name = "Description"
    descriptionProp.Value= description
    Dim visibleProp as new [Property]
    visibleProp.Name = "Visible"
    visibleProp.value= visible
    Dim props(1) as [Property]
    props(0) = descriptionProp
    props(1) = visibleProp
    Try
        rs.CreateFolder(folderName,parentPath,props)
        Console.WriteLine("Folder {0} successfully created", foldername)
    Catch goof as SoapException
        If goof.Message.Indexof("AlreadyExists")>0 Then
            Console.WriteLine("Folder {0} already exists",foldername)
        End If
    End Try
End Sub
 
'Utility for reading files from the Report Sevices Project
Public sub ReadFiles(filepath as string, fileextension as string)
    Console.WriteLine()
    Console.WriteLine("Reading Files from Report Services Project")
    Dim rptdirinfo As System.IO.DirectoryInfo
    rptdirinfo = New System.IO.DirectoryInfo(filepath)
    Dim filedoc As FileInfo()
    filedoc = rptdirinfo.GetFiles(fileextension)
    Try
        For rptcount As Integer = 0 To filedoc.Length-1
            If Not filedoc(rptcount).Name.ToString.Trim.ToUpper.Contains("BACKUP") Then
                SELECT Case fileextension
                Case "*.rds"
                    CreateDataSource(filedoc(rptcount).tostring.trim)
                Case "*.rsd"
                    CreateDataSet(filedoc(rptcount).tostring.trim)
                Case "*.rdl"
                    PublishReport(filedoc(rptcount).tostring.trim)
                End Select
            End If
        Next
    Catch goof as Exception
        Console.WriteLine("In ReadFiles " + goof.message)
    End Try
End Sub
 
'Utility for Creating Shared Data Sets contained in the project
Public Sub CreateDataSet(ByVal filename as string)
    Dim valstart as integer
    Dim valend as integer
    Dim DSDefinitionStr as string
    Dim DataSourceName as string
    Dim QueryString as string
    Try
        Dim stream As FileStream = File.OpenRead(filePath + "\" + filename )
        definition = New [Byte](stream.Length-1) {}
        stream.Read(definition, 0, CInt(stream.Length))
        stream.Close()
        For i As Integer = 0 To definition.Length - 1
            DSDefinitionStr = DSDefinitionStr + Convert.ToString(Convert.ToChar(Convert.ToInt16(definition(i).ToString)))
        Next
        valstart=DSDefinitionStr.ToString.Indexof("<DataSourceReference>")
        If valstart > 0 Then
            valstart = DSDefinitionStr.ToString.IndexOf("<DataSourceReference>") + 21
            valend = DSDefinitionStr.ToString.IndexOf("</DataSourceReference>")
            DataSourceName=DSDefinitionStr.ToString.Substring(valstart, valend - valstart)
            Console.WriteLine(DataSourceName)
        End If
    Catch e As IOException
        Console.WriteLine(e.Message)
    End Try
    filename=filename.tostring.replace(".rsd","")
    Console.WriteLine("Attempting to Deploy DataSet {0}", filename)
    Try
        Dim item as CatalogItem
        item=rs.CreateCatalogItem("DataSet",filename, "/" + DataSetFolder, false, definition, nothing, warnings)
        If Not (warnings Is Nothing) Then
            Dim warning As Warning
            For Each warning In warnings
                if warning.message.tostring.tolower.contains("refers to the shared data source") then
                    Console.WriteLine("Connecting DataSet {0} to Data Source {1}",filename, DataSourceName)
                    Dim referenceData() as ItemReferenceData = rs.GetItemReferences("/" + DataSetFolder + "/" + filename,"DataSet")
                    Dim references(0) as ItemReference
                    Dim reference as New ItemReference()
                    Dim datasourceURL = "/" + DataSourceFolder + "/" + DataSourceName
                    reference.name=referenceData(0).Name
                    Console.WriteLine("Reference name = " + reference.name)
                    reference.Reference=datasourceURL
                    references(0)=reference
                    rs.SetItemReferences("/" + DataSetFolder + "/" + filename, references)
                else
                    Console.WriteLine(warning.Message)
                end if
            Next warning
        Else
            Console.WriteLine("DataSet: {0} published successfully with no warnings", filename)
        End If
    Catch goof as SoapException
        If goof.Message.Indexof("AlreadyExists")>0 Then
            Console.WriteLine("The DataSet {0} already exists",fileName.ToString)
        Else
            If goof.Message.IndexOf("published")=-1 Then
                Console.Writeline(goof.Message)
            End If
        End If
    End Try
End Sub
 
'Utility for creating Data Sources on the Server
Public Sub CreateDataSource(filename as string)
    'Define the data source definition.
    Dim dsDefinition As New DataSourceDefinition()
    Dim DataSourceName as string
    Dim valstart As Integer
    Dim valend As Integer
    Dim ConnectionString As String
    Dim Extension As String
    Dim IntegratedSec As String
    Dim DataSourceID As String
    Dim PromptStr As String
    PromptStr=""
    Dim DSDefinitionStr As String
    DSDefinitionStr = ""
    DataSourceName=filename.tostring.trim.substring(0,filename.tostring.trim.length-4)
    Console.WriteLine("Attempting to Deploy Data Source {0}", DataSourceName)
    Try
        Dim stream As FileStream = File.OpenRead(filepath + "\" + filename)
        bytedefinition = New [Byte](stream.Length-1) {}
        stream.Read(bytedefinition, 0, CInt(stream.Length))
        stream.Close()
        For i As Integer = 0 To bytedefinition.Length - 1
            DSDefinitionStr = DSDefinitionStr + Convert.ToString(Convert.ToChar(Convert.ToInt16(bytedefinition(i).ToString)))
        Next
    Catch goof As IOException
        Console.WriteLine(goof.Message)
    End Try
    If DSDefinitionStr.ToString.Contains("<ConnectString>") And DSDefinitionStr.ToString.Contains("</ConnectString>") Then
        valstart = DSDefinitionStr.ToString.IndexOf("<ConnectString>") + 15
        valend = DSDefinitionStr.ToString.IndexOf("</ConnectString>")
        ConnectionString = DSDefinitionStr.ToString.Substring(valstart, valend - valstart)
    End If
    If DSDefinitionStr.ToString.Contains("<Extension>") And DSDefinitionStr.ToString.Contains("</Extension>") Then
        valstart = DSDefinitionStr.ToString.IndexOf("<Extension>") + 11
        valend = DSDefinitionStr.ToString.IndexOf("</Extension>")
        Extension = DSDefinitionStr.ToString.Substring(valstart, valend - valstart)
    End If
    If DSDefinitionStr.ToString.Contains("<IntegratedSecurity>") And DSDefinitionStr.ToString.Contains("</IntegratedSecurity>") Then
        valstart = DSDefinitionStr.ToString.IndexOf("<IntegratedSecurity>") + 20
        valend = DSDefinitionStr.ToString.IndexOf("</IntegratedSecurity>")
        IntegratedSec = DSDefinitionStr.ToString.Substring(valstart, valend - valstart)
    End If
    If DSDefinitionStr.ToString.Contains("<DataSourceID>") And DSDefinitionStr.ToString.Contains("</DataSourceID>") Then
        valstart = DSDefinitionStr.ToString.IndexOf("<DataSourceID>") + 14
        valend = DSDefinitionStr.ToString.IndexOf("</DataSourceID>")
        DataSourceID = DSDefinitionStr.ToString.Substring(valstart, valend - valstart)
    End If
    If DSDefinitionStr.ToString.Contains("<Prompt>") And DSDefinitionStr.ToString.Contains("</Prompt>") Then
        valstart = DSDefinitionStr.ToString.IndexOf("<Prompt>") + 8
        valend = DSDefinitionStr.ToString.IndexOf("</Prompt>")
        PromptStr = DSDefinitionStr.ToString.Substring(valstart, valend - valstart)
    End If
    dsdefinition.CredentialRetrieval = CredentialRetrievalEnum.Integrated
    dsdefinition.ConnectString = ConnectionString
    dsdefinition.Enabled = True
    dsdefinition.EnabledSpecified = True
    dsdefinition.Extension = extension
    dsdefinition.ImpersonateUser = False
    dsdefinition.ImpersonateUserSpecified = True
    'Use the default prompt string.
    If PromptStr.ToString.Length=0 Then
        dsdefinition.Prompt = Nothing
    Else
        dsdefinition.Prompt = PromptStr
    End if
    dsdefinition.WindowsCredentials = False
    Try
        rs.CreateDataSource(DataSourceName, "/" + DataSourceFolder, False, dsdefinition, Nothing)
        Console.WriteLine("Data source {0} created successfully", DataSourceName.ToString)
    Catch goof as SoapException
        If goof.Message.Indexof("AlreadyExists")>0 Then
            Console.WriteLine("The Data Source name {0} already exists",DataSourceName.ToString)
        End If
    End Try
End Sub
 
'Utility to Publish the Reports
Public Sub PublishReport(ByVal reportName As String)
    Console.WriteLine("Publishing " + reportName)
    Dim DSDefinitionStr As String
    DSDefinitionStr = ""
    Try
        Dim stream As FileStream = File.OpenRead(filePath + "\" + reportName )
        definition = New [Byte](stream.Length-1) {}
        stream.Read(definition, 0, CInt(stream.Length))
        stream.Close()
        For i As Integer = 0 To definition.Length - 1
            DSDefinitionStr = DSDefinitionStr + Convert.ToString(Convert.ToChar(Convert.ToInt16(definition(i).ToString)))
        Next
    Catch e As IOException
        Console.WriteLine(e.Message)
    End Try
    reportname=reportname.tostring.replace(".rdl","")
    Console.WriteLine("Attempting to Deploy Report Name {0}", reportname.tostring)
    Try
        Dim item as CatalogItem
        item=rs.CreateCatalogItem("Report",reportname, "/" + ReportFolder, false, definition,nothing, warnings)
        'warnings = rs.CreateCatalogItem(reportName, "/" + ReportFolder, False, definition, Nothing)
        If Not (warnings Is Nothing) Then
            If item.Name <> "" then 
                Console.WriteLine("Report: {0} published successfully with warnings", reportName)
                UpdateDataSources_report(reportName)
                UpdateDataSet_report(reportName, DSDefinitionStr)
            else
                Dim warning As Warning
                For Each warning In warnings
                    Console.WriteLine(warning.Message)
                Next warning
            End If
        Else
            Console.WriteLine("Report: {0} published successfully with no warnings", reportName)
            UpdateDataSources_report(reportName)
            UpdateDataSet_report(reportName, DSDefinitionStr)
        End If
    Catch goof as SoapException
        If goof.Message.Indexof("AlreadyExists")>0 Then
            Console.WriteLine("The Report Name {0} already exists",reportName.ToString)
        Else
            If goof.Message.IndexOf("published")=-1 Then
            Console.WriteLine(goof.Message)
            End If
        End If
    End Try
End Sub

'Utility to Update The Data Sources on the Server
Public Sub UpdateDataSources_report(ReportName as string)
    rs.Credentials = System.Net.CredentialCache.DefaultCredentials
    Dim item as CatalogItem
    Dim items as CatalogItem()
    Try
        Dim dataSources() as DataSource = rs.GetItemDataSources( "/" +  ReportFolder + "/" + ReportName)
        For Each ds as DataSource in dataSources
            Dim sharedDs(0) as DataSource
            sharedDs(0)=GetDataSource(DataSourceFolder, ds.Name)
            rs.SetItemDataSources("/" +  ReportFolder + "/" + ReportName, sharedDs)
            Console.WriteLine("Set " & ds.Name & " datasource for " & "/" +  ReportFolder + "/" + ReportName & " report")
        Next
        Console.WriteLine("All the shared data source reference set for report {0} ", "/" + ReportFolder + "/" + ReportName)
    Catch goof As SoapException
    Console.WriteLine(goof.Detail.InnerXml.ToString())
    End Try
End Sub

'Utility to link The Dataset with the Report
Public Sub UpdateDataSet_report(ReportName as string, DSDefinitionStr as string)
    Dim valstart As Integer
    Dim valend As Integer
    Dim sharedDataSetReference As string
    rs.Credentials = System.Net.CredentialCache.DefaultCredentials
    Try
        Dim dataSets As ItemReferenceData() = rs.GetItemReferences("/" +  ReportFolder + "/" + ReportName, "DataSet")
        If dataSets IsNot Nothing AndAlso dataSets.Length > 0 AndAlso Not String.IsNullOrEmpty(dataSets(0).Name) Then
            For i as integer = 0 to dataSets.Length -1
                If DSDefinitionStr.ToString.Contains("<DataSet Name="""& dataSets(i).Name &""">") Then
                    valstart = DSDefinitionStr.ToString.IndexOf("<SharedDataSetReference>", DSDefinitionStr.ToString.IndexOf("<DataSet Name="""& dataSets(i).Name &""">")) + 24
                    valend = DSDefinitionStr.ToString.IndexOf("</SharedDataSetReference>", DSDefinitionStr.ToString.IndexOf("<DataSet Name="""& dataSets(i).Name &""">"))
                    sharedDataSetReference = DSDefinitionStr.ToString.Substring(valstart, valend - valstart)
                End If
                Dim sharedDataSetName as String = dataSets(i).Name
                Dim references(0) as ItemReference
                Dim sharedDataSet = New ItemReference() 
                sharedDataSet.Name = sharedDataSetName
                Console.WriteLine("Attempting to Link Dataset {0}", sharedDataSetName)
                sharedDataSet.Reference = "/" + DataSetFolder + "/" + sharedDataSetReference 
                references(0)=sharedDataSet
                rs.SetItemReferences("/" + ReportFolder + "/" + ReportName, references)
                Console.WriteLine("Report " + ReportName + " Linked to data set " + "/" + DataSetFolder + "/" + Convert.ToString(sharedDataSet.Name))
            Next
        End If
    Catch goof As SoapException
        Console.WriteLine(goof.Detail.InnerXml.ToString())
    End Try
End Sub
 
'Function to Reference Data Sources
Private Function GetDataSource(sharedDataSourcePath as string, dataSourceName as String) as DataSource
    Dim reference As New DataSourceReference()
    Dim ds As New DataSource
    reference.Reference = "/" & sharedDataSourcePath & "/" & dataSourceName
    ds.Item = CType(reference, DataSourceDefinitionOrReference)
    ds.Name = dataSourceName
    Console.WriteLine("Attempting to Publish Data Source {0}", ds.Name)
    GetDataSource=ds
End Function
{% endhighlight %}

This script is based on [this article](https://blogs.msdn.microsoft.com/johndesch/2012/12/17/using-the-rs-exe-utility-to-deploy-a-report-server-project-and-shared-dataset/).

Thanks!