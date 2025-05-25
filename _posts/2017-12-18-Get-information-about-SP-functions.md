---
layout: post
title: Get information about all stored procedures, views and functions
date: 2017-12-18

tags: dotnet sql
categories: administration
---
When your primary work is adding functionality to MSSQL database (like implementing business logic in stored procedures, functions and view) it is very useful to have the list of all SP, functions and views with their description, name of authors, created dates and input/output parameters.

In this small article I will describer how to create a small console application which will get information from DB and create a list with this information in memory. Later I am creating HTML report using this information. But creating HTML report is not described in this article.

One note: each SP, function and view should have the next comment in their code:

```sql
-- =============================================
-- Author:      FirstName LastName
-- Create date = "yyyyMMdd
-- Description: Short description
-- =============================================
```

SQL query which shows information about SP, functions and views is:
```cs
const string GetAllObjects = @"
DECLARE @StopString nvarchar(MAX) = '-- =============================================';

SELECT DISTINCT
o.name,
o.type_desc as Type,
s.name as [Schema],
m.definition,
o.create_date as Created,
o.modify_date as Modified,
CASE WHEN CHARINDEX(@StopString, m.definition, CHARINDEX(@StopString, m.definition, 0)+1)-CHARINDEX(@StopString, m.definition, 0)-LEN(@StopString) > CHARINDEX(@StopString, m.definition, 0) + LEN(@StopString)
    THEN
        SUBSTRING(m.definition, CHARINDEX(@StopString, m.definition, 0) + LEN(@StopString), CHARINDEX(@StopString, m.definition, CHARINDEX(@StopString, m.definition, 0)+1)-CHARINDEX(@StopString, m.definition, 0)-LEN(@StopString))
    ELSE
        'No description'
END
AS Description
FROM sys.sql_modules m
    INNER JOIN sys.objects o ON m.object_id = o.object_id
    INNER JOIN sys.schemas s ON o.schema_id = s.schema_id
WHERE
    s.name = 'dbo' and
    (o.type_desc = 'VIEW' or o.type_desc = 'SQL_STORED_PROCEDURE' or o.type_desc = 'SQL_INLINE_TABLE_VALUED_FUNCTION' or o.type_desc = 'SQL_SCALAR_FUNCTION')
";
```

As I usully use [Dapper](https://github.com/StackExchange/Dapper) to work with DB there is a list of DTOs:
```cs
internal class DBObject
{
    public string Name { get; set; }
    public string Schema { get; set; }
    public string Type { get; set; }
    public string Definition { get; set; }
    public DateTime Created { get; set; }
    public DateTime Modified { get; set; }
    public string Description { get; set; }
}

internal class HelpInfo
{
    public string Name { get; set; }
    public string Owner { get; set; }
    public string Type { get; set; }
    public DateTime Created_datetime { get; set; }
}

internal class Parameter
{
    public string Parameter_name { get; set; }
    public string Type { get; set; }
    public int Length { get; set; }
    public string Prec { get; set; }
    public int Param_oder { get; set; }
    public string Collation { get; set; }
}

internal class Table
{
    public string Column_name { get; set; }
    public string Type { get; set; }
    public string Computed { get; set; }
    public int Length { get; set; }
    public string Prec { get; set; }
    public string Scale { get; set; }
    public string Nullable { get; set; }
    public string TrimTrailingBlanks { get; set; }
    public string FixedLenNullInSource { get; set; }
    public string Collation { get; set; }
}
```

And to get the list of SP, functions and views we can use the next method:
```cs
public IEnumerable<(DBObject Object, HelpInfo Info, IEnumerable<Parameter> Parameters, IEnumerable<Table> OutputTables)> GetAllDbObjects()
{
    var objs = _sqlExecutor.Query<DBObject>(GetAllObjects);
    foreach (var data in objs.OrderBy(x => x.Created))
    {
        switch (data.Type)
        {
            case "SQL_STORED_PROCEDURE":
            case "SQL_SCALAR_FUNCTION":
                var infoAboutSPOrScalarFunction = _sqlExecutor.QueryMultiple<HelpInfo, Parameter>("sp_help",
                    new { @objname = $"{data.Schema}.{data.Name}" }, CommandType.StoredProcedure);
                yield return (data, infoAboutSPOrScalarFunction.Item1, infoAboutSPOrScalarFunction.Item2, null);
                break;
            case "SQL_INLINE_TABLE_VALUED_FUNCTION":
            case "VIEW":
                var inforAboutViewOrInlineFunction = _sqlExecutor.QueryMultiple<HelpInfo, Table, Parameter>("sp_help",
                    new { @objname = $"{data.Schema}.{data.Name}" }, CommandType.StoredProcedure);
                yield return (data, inforAboutViewOrInlineFunction.Item1, inforAboutViewOrInlineFunction.Item3, inforAboutViewOrInlineFunction.Item2);
                break;
        }
    }
}

/// where Query and QueryMultiple methods look like
public IEnumerable<T> Query<T>(string query)
{
    using (var connection = new SqlConnection(connectionString))
    {
        return connection.Query<T>(query);
    }
}

public (T, IEnumerable<K>) QueryMultiple<T, K>(string query, object param, CommandType commandType)
{
    using (var connection = new SqlConnection(connectionString))
    {
        using (var multi = connection.QueryMultiple(query, param, commandType: CommandType.StoredProcedure))
        {
            return (
                multi.Read<T>().Single(),
                multi.IsConsumed ? null : multi.Read<K>()
            );
        }
    }
}

public (T, IEnumerable<K>, IEnumerable<M>) QueryMultiple<T, K, M>(string query, object param, CommandType commandType)
{
    using (var connection = new SqlConnection(connectionString))
    {
        using (var multi = connection.QueryMultiple(query, param, commandType: CommandType.StoredProcedure))
        {
            return (
                multi.Read<T>().Single(),
                multi.Read<K>(),
                multi.IsConsumed ? null: multi.Read<M>()
            );
        }
    }
}
```

So mentioned GetAllDbObjects returns a list of tuples, where DBObject contains general information about SP, function or view; HelpInfo contains additional information about DBObject; IEnumerable<Parameter> contains information about input/output parameters for SP and functions; IEnumerable<Table> contains information about a view design (as view returns data in table format).

We can use this list to create different kinds of report like HTML report or delta report which shows only modified db objects.

Thanks!
