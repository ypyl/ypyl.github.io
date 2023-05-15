---
layout: post
title: "Linq and Group"
date: 2010-10-16

tags: dotnet
categories: programming
---
Description. There are modules that have the input and output parameters (type and number of parameters may be different). The goal is to select some first modules that have no input parameters, then the modules whose inputs are the output parameters of the previously selected modules and so on. Until the last module will not output parameters.

The database stores all displayed modules and displays of all parameters where the parameter mapping to the mapping of modules is many-to-one.
Initially, we choose to do all the settings using a simple SQL query.

```cs
private static SqlCeDataReader SelectAllParamterAndModuleName()
{
    return ExecuteDataReader(String.Format(QSelectAllParamterANdModuleName));
}
```

where QSelectAllParamterANdModuleName is text of the query.

```csharp
private static IEnumerable<ParameterType> SelectAllParameterToArray()
{
    var query =
        from row in SelectAllParamterAndModuleName().Cast()
        select new ParameterType
        {
            Id = (int)row["FID"],
            Name = (string)row["FNAME"],
            TypeName = (string)row["FTYPE"],
            ModuleId = (int)row["FMODULEID"],
                        ModuleName = (string)row["FMODULE"], Number = (int)row["FNUMBER"], TypeOfPar = (bool)row["FTYPEOFPARAMETER"]};
    return query.ToArray();
}
```

create from selected SqlReader array of elements of ParameterType. The result can be cached.

```csharp
public static List<ModuleType> SelectModuleByInputParameter(List<ParameterType> inputParameter)
{
    if (inputParameter == null)
    {
        throw new ArgumentNullException();
    }
    var res = new List();
    var para = SelectAllParameterToArray();
    if (inputParameter.Count == 0)
    {
        var par = from parameter in para
                    where !(from parameter2 in para
                            where parameter2.TypeOfPar
                            select parameter2.ModuleId).Contains(parameter.ModuleId)
                    select parameter;
        return par.Select(x =&gt; new ModuleType {Id = x.ModuleId, Name = x.ModuleName}).ToList();
    }

    var parGr = from parameter in para
                where parameter.TypeOfPar
                group parameter by parameter.ModuleId
                into modPara
                where modPara.Count() == inputParameter.Count
                select modPara;

    foreach (var paramGr in parGr)
    {
        bool AddInRes = true;
        var listParaGr = paramGr.ToList();
        for (int i = 0; i &lt; listParaGr.Count; i++)
        {
            if (listParaGr[i].Number != inputParameter[i].Number ||
            listParaGr[i].TypeName != inputParameter[i].TypeName)
                AddInRes = false;
        }
        if (AddInRes)
        {
            res.Add(new ModuleType(){Id =  listParaGr[0].ModuleId, Name = listParaGr[0].ModuleName});
        }
    }
    return res;
}
```

This method returns a collection of our modules, which contains defined input parameters. Thus, this method should be called justbefore the loop until the result will contain at least one value. It should be noted that calls to this method will generally be bifurcate (the first layer selected modules, and for everyone in this layer, called again, this method and so on). But this is not Linq.
