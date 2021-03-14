---
layout: post
title: "SMO Scripter. Create script of DB"
date: 2013-06-25

tags: dotnet
categories: programming
---
It is possible to create script of MSSQL using SMO: ([link](http://pastebin.com/AQkprTS7#))

```cs
private static void Main(string[] args)
{
    var arguments = args.Select(x => x.ToLower()).ToList();
    if (arguments.Count == 0 || arguments.Contains("-help"))
    {
        Console.WriteLine("-d - Database Name");
        Console.WriteLine("-i - Output Sql File Name");
        Console.WriteLine("-s - Server Instance");
        Console.WriteLine("-u - User Name");
        Console.WriteLine("-p - Password");
        Console.WriteLine("It should be 'tables.txt' file in folder with names of tables to script; if it does not exist the application scripts all tables with prefix 'GMP_'");
        Console.ReadKey();
    }
    else if (arguments.Count > 1 && arguments.Contains("-d") && args.Contains("-i"))
    {
        if (arguments.Count <= arguments.IndexOf("-d") + 1)
        {
            throw new ArgumentException("Database Name");
        }
        if (arguments.Count <= argumSMO Scripter. Create script of DBents.IndexOf("-i") + 1)
        {
            throw new ArgumentException("Output Sql File Name");
        }
        var dbName = arguments[arguments.IndexOf("-d") + 1];
        var outputFileName = arguments[arguments.IndexOf("-i") + 1];
        var srv = new Server();
        if (arguments.Contains("-s") && args.Contains("-u") && arguments.Contains("-p"))
        {
            if (arguments.Count <= arguments.IndexOf("-s") + 1)
            {
                throw new ArgumentException("Server Instance");
            }
            if (arguments.Count <= arguments.IndexOf("-u") + 1)
            {
                throw new ArgumentException("User Name");
            }
            if (arguments.Count <= arguments.IndexOf("-p") + 1)
            {
                throw new ArgumentException("Password");
            }
            var connection = new ServerConnection(arguments[arguments.IndexOf("-s") + 1], arguments[arguments.IndexOf("-u") + 1], arguments[arguments.IndexOf("-p") + 1]);
            srv = new Server(connection);
        }
        // read names of tables
        var tablesFromFile = new List<string>();
        if (File.Exists("tables.txt"))
        {
            using (var file = File.OpenText("tables.txt"))
            {
                while (file.Peek() > 0)
                {
                    tablesFromFile.Add(file.ReadLine());
                }
            }
        }
        Database db = srv.Databases[dbName];
        var dropKeys = new Scripter(srv) {Options = {ScriptDrops = true, IncludeIfNotExists = true, DriForeignKeys = true}};
        var listOfScripts = new List<Scripter>
                            {
                                new Scripter(srv) {Options = {ScriptDrops = true, IncludeIfNotExists =true, DriAllKeys = false}},
                                new Scripter(srv) {Options = {ScriptDrops = false, ScriptSchema = true, WithDependencies = false, DriIndexes = true, DriClustered = true, IncludeIfNotExists = true, DriAllKeys = false}},
                                new Scripter(srv) {Options = {ScriptDrops = false, ScriptSchema = true, DriDefaults = false, DriIndexes = false, DriPrimaryKey = false, DriClustered = false, Default = false, DriAll =false, DriForeignKeys = true, IncludeIfNotExists = true, DriAllKeys = false}},
                                new Scripter(srv) {Options = {DriIndexes = true, Default = true, DriDefaults = true, DriClustered = false, IncludeIfNotExists = true, DriAll = true, DriAllConstraints = true, DriAllKeys = true, SchemaQualify = true, SchemaQualifyForeignKeysReferences = true, NoCollation = true}}
                            };
        using (var file = File.CreateText(outputFileName))
        {
            foreach (Table tb in db.Tables)
            {
                if ((tablesFromFile.Count > 0 && tablesFromFile.Contains(tb.Name) || (tablesFromFile.Count== 0 && tb.Name.StartsWith("GMP_"))))
                {
                    if (tb.IsSystemObject == false)
                    {
                        foreach (ForeignKey foreignKey in tb.ForeignKeys)
                        {
                            System.Collections.Specialized.StringCollection scd = dropKeys.Script(new[] {foreignKey.Urn });
                            foreach (string st in scd)
                            {
                                file.WriteLine(st);
                                file.WriteLine("GO");
                            }
                        }
                        file.WriteLine();
                    }
                }
            }
            foreach (var script in listOfScripts)
            {
                foreach (Table tb in db.Tables)
                {
                    if ((tablesFromFile.Count > 0 && tablesFromFile.Contains(tb.Name) ||(tablesFromFile.Count == 0 && tb.Name.StartsWith("GMP_"))))
                    {
                        if (tb.IsSystemObject == false)
                        {
                            System.Collections.Specialized.StringCollection scd = script.Script(new[] {tb.Urn });
                            foreach (string st in scd)
                            {
                                file.WriteLine(st);
                                file.WriteLine("GO");
                            }
                            file.WriteLine();
                        }
                    }
                }
            }
        }
    }
}
```