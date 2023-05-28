---
layout: post
title: "Copy remote MSSQL DB to local server"
date: 2017-12-20

tags: dotnet mssql
categories: programming
---
I faced with one issue how to automatically download and restore databases from remote server to my local MSSQL server. It sounds like very generic task so I tried to find a solution in Internet without success. So there is a small console application which does the next things:
1. Backup selected databases on remote server;
2. Copy backups to local folder;
3. Restore copied databases to local MSSQL;
4. Change the owner of databases to current user;

A small snippet with implementation is below.
```cs
/*
You need to add next NuGet packages:
"Microsoft.Extensions.Configuration": "2.0.0",
"Microsoft.Extensions.Configuration.FileExtensions": "2.0.0",
"Microsoft.Extensions.Configuration.Json": "2.0.0",
"System.Data.Common": "4.3.0",
"System.Data.SqlClient": "4.4.2",
"System.Runtime": "4.3.0"
*/
class Program
{
    private static IConfigurationRoot Configuration = new ConfigurationBuilder()
            .SetBasePath(Directory.GetCurrentDirectory())
            .AddJsonFile("appsettings.json").Build();

    private static string[] dbs { get; } = Configuration.GetSection("dbs")
        .GetChildren().Select(x=>x.Value).ToArray();

    private static string remoteServer { get; } = Configuration["connectionStrings:remoteServer"];
    private static string localServer { get; } = Configuration["connectionStrings:localServer"];
    private static string currentUser { get; } = Configuration["currentUser"];
    private static string remoteServerPathWithBackups { get; } = Configuration["remoteServerPathWithBackups"];
    private static string serverPathWithBackups { get; } = Configuration["serverPathWithBackups"];
    private static string userNameToLogInToServer { get; } = Configuration["userNameToLogInToServer"];
    private static string passwordToLogInToServer { get; } = Configuration["passwordToLogInToServer"];
    private static string localPathToCopyBackups { get; } = Configuration["localPathToCopyBackups"];
    private static string backupFileName = Configuration["backupFileName"];

    private const string changeOwnerOfDb = @"
USE {0}
EXEC sp_changedbowner '{1}'";

    private const string changeToSingleUser = @"
ALTER DATABASE[{0}] SET Single_User WITH Rollback Immediate";

    private const string changeToMultiUser = @"
ALTER DATABASE [{0}] SET Multi_User";

    private const string AddNetPath = "NET USE {0} {1} /USER:{2}";
    private const string DeleteNetPath = "NET USE {0} /DELETE";
    private const string CopyReport = "robocopy {0} {1} {2}";

    private const string restoreDb = @"
RESTORE DATABASE {0} FROM DISK = '{1}' WITH REPLACE, RECOVERY;";
    private const string backupDb = @"
BACKUP DATABASE {0} TO DISK='{1}' WITH INIT, COPY_ONLY, FORMAT, COMPRESSION;";

    static void Main(string[] args)
    {
        foreach (var db in dbs)
        {
            Console.WriteLine($"Backup {db}");
            BackupTargetDatabase(remoteServer, db);

            Console.WriteLine($"Copy created backup to local - {db}");
            CopyBackupToLocal(db);

            Console.WriteLine($"Change to single user mode - {db}");
            ChangeToSingleUser(localServer, db);

            Console.WriteLine($"Restore db locally - {db}");
            RestoreTargetDatabase(localServer, db);

            Console.WriteLine($"Change the owner - {db}");
            ChangeOwnerOfDb(localServer, db);

            Console.WriteLine($"Reset to multi user mode - {db}");
            ChangeToMultiUser(localServer, db);
        }
    }

    private static void ChangeToMultiUser(string localServer, string db) =>
        ExecuteSqlCommand(localServer, string.Format(changeToMultiUser, db));

    private static void ChangeToSingleUser(string localServer, string db) =>
        ExecuteSqlCommand(localServer, string.Format(changeToSingleUser, db));

    private static void ChangeOwnerOfDb(string localServer, string db) =>
        ExecuteSqlCommand(localServer, string.Format(changeOwnerOfDb, db, currentUser));

    private static void CopyBackupToLocal(string database)
    {
        ExecuteCommand(string.Format(AddNetPath, remoteServerPathWithBackups, passwordToLogInToServer, userNameToLogInToServer));
        ExecuteCommand(string.Format(CopyReport, remoteServerPathWithBackups, localPathToCopyBackups, string.Format(backupFileName, database)));
        ExecuteCommand(string.Format(DeleteNetPath, remoteServerPathWithBackups));
    }

    private static void ExecuteCommand(string command)
    {
        var processInfo = new ProcessStartInfo("cmd.exe", "/c " + command);
        processInfo.CreateNoWindow = true;
        processInfo.UseShellExecute = false;
        processInfo.RedirectStandardError = true;
        processInfo.RedirectStandardOutput = true;

        Console.WriteLine(command);

        var process = Process.Start(processInfo);

        process.OutputDataReceived += (object sender, DataReceivedEventArgs e) =>
            Console.WriteLine($"{e.Data}");
        process.BeginOutputReadLine();

        process.ErrorDataReceived += (object sender, DataReceivedEventArgs e) =>
            Console.WriteLine($"{e.Data}");
        process.BeginErrorReadLine();

        process.WaitForExit();
        if (process.ExitCode > 0)
            Console.WriteLine($"Error {nameof(process.ExitCode)}: {process.ExitCode}");
        process.Close();
    }

    private static void RestoreTargetDatabase(string connectionString, string database) =>
        ExecuteSqlCommand(connectionString, string.Format(restoreDb, database, Path.Combine(localPathToCopyBackups, string.Format(backupFileName, database))));

    private static void BackupTargetDatabase(string connectionString, string database) =>
        ExecuteSqlCommand(connectionString, string.Format(backupDb, database, Path.Combine(serverPathWithBackups, string.Format(backupFileName, database))));

    private static void ExecuteSqlCommand(string connectionString, string query)
    {
        using (var connection = new SqlConnection(connectionString))
        {
            using (var command = new SqlCommand(query, connection))
            {
                connection.Open();
                command.CommandTimeout = 300;
                command.ExecuteNonQuery();
            }
        }
    }
}
```

Thanks.