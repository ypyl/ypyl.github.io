---
layout: post
title: "Cron schedule using F#"
date: 2017-06-01
tags: dotnet fsharp
categories: programming
---
I decided to start learning fsharp and hope I will be able to use F# in my future small projects.

Currently I have a small service worked in Docker container. It is written using C# on dotnet core.
This service is for grabbing data from external services via HTTP and putting aggregated data to InfluxDB database. Internally the service is using cron to plan and run this process as we want to grab data periodically.

#### I started with rewriting cron scheduling.

So there is implementation in F# below:

Let's start with two helper objects: `String.split` and `TooMuchArgumentsException` exception.

```fsharp
type System.String with 
    static member split c (value: string) =
        value.Split c

exception TooMuchArgumentsException of int
```

`String.split` is just wrapper for `String.Split` method. Exception is needed to show when cron expression contains too much parts.

My internal cron supports the next template: 'minute hour dayOfMonth month dayOfWeek'. And the next type of cron expressions: 
1. '*' - wildcard;
2. '*/5' - every 5th, e.g. every five minutes;
3. '10-20/5' - range value, e.g. every from 10 till 20, e.g. every minute from 10 till 20. '/5' is optional and it works the same as previous one, so '10-20/5' for minutes means run at 10, 15 and 20;
4. '5' - one value only, e.g. only at 5th minute
5. '5,10,15,45' - list value, e.g. run at 5th, 10th, 15th and 45th minutes

```fsharp
open System
open System.Text.RegularExpressions

module Schedule =
    // regexp for */5
    [<Literal>]
    let DividePattern = @"(\*/\d+)"
    // regexp for range
    [<Literal>]
    let RangePattern = @"(\d+\-\d+(/\d+)?)"
    // regexp for wild char
    [<Literal>]
    let WildPattern = @"(\*)"
    // regexp for one value
    [<Literal>]
    let OneValuePattern = @"(\d)"
    // regexp for list value
    [<Literal>]
    let ListPattern = @"((\d+,)+\d+)"

    // internal record to parsed cron expression, so it contains minutes,
    // hours, days of month, months and day of week when we should run our jobs
    type ISchedueSet = 
        { 
            Minutes: int list;
            Hours: int list;
            DayOfMonth: int list;
            Months: int list;
            DayOfWeek: int list
        }
    
    // method to generate ISchedueSet record from cron expression
    let generate expression =
        // internal method to parse */5 
        let dividedArray (m:string) start max =
            let divisor = m |>  String.split [|'/'|] |> Array.skip 1 |> Array.head |> Int32.Parse
            [start .. max] |> List.filter (fun x -> x % divisor = 0)
        // internal method to parse range
        let rangeArray (m:string) =
            let split = m |> String.split [|'-'; '/'|] |> Array.map Int32.Parse
            match Array.length split with
                | 2 -> [split.[0] .. split.[1]]
                | 3 -> [split.[0] .. split.[1]] |> List.filter (fun x -> x % split.[2] = 0)
                | _ -> []
        // internal method to parse wild char
        let wildArray (m:string) start max =
            [start .. max]
        // internal method to parse single value
        let oneValue (m:string) =
            [m |> Int32.Parse]
        // internal method to parse list value
        let listArray (m:string) =
            m |> String.split [|','|] |> Array.map Int32.Parse |> Array.toList
        
        // we need to set minimum and maximum value for every part of date time
        let getStartAndMax i =
            match i with
            // for minutes
            | 0 -> (0, 59)
            // for hours
            | 1 -> (0, 23)
            // for days of month
            | 2 -> (1, 31)
            // for months
            | 3 -> (1, 12)
            // for day of week
            | 4 -> (0, 6)
            // throw an exception if don't know for what part of date time we need values
            | _ -> raise (TooMuchArgumentsException i)
        
        // active pattern to match regexp
        let (|MatchRegex|_|) pattern input =
            let m = Regex.Match(input, pattern)
            if m.Success then Some (m.ToString()) else None
        
        // parsing cron expression and create a array of lists which contains all possibles values
        // for every part of daytime
        let parts =
            expression 
            |> String.split [|' '|]
            |> Array.mapi (fun i x ->
                let (start, max) = getStartAndMax i 
                match x with
                    | MatchRegex DividePattern x -> dividedArray x start max
                    | MatchRegex RangePattern x -> rangeArray x
                    | MatchRegex WildPattern x -> wildArray x start max
                    | MatchRegex ListPattern x -> listArray x
                    | MatchRegex OneValuePattern x -> oneValue x
                    | _ -> []
            )
        // convert list of array to ISchedueSet
        { 
            Minutes = parts.[0];
            Hours = parts.[1];
            DayOfMonth = parts.[2];
            Months = parts.[3];
            DayOfWeek = parts.[4]
        }
    // method to check date time via generated ISchedueSet
    let isTime schedueSet (dateTime : DateTime) =
        List.exists ((=) dateTime.Minute) schedueSet.Minutes && 
        List.exists ((=) dateTime.Hour) schedueSet.Hours &&
        List.exists ((=) dateTime.Day) schedueSet.DayOfMonth &&
        List.exists ((=) dateTime.Month) schedueSet.Months &&
        List.exists ((=) (int dateTime.DayOfWeek)) schedueSet.DayOfWeek
```

So Schedule module contains two methods and one type: 
* ISchedueSet is a container for parsed cron expression;
* generate is to generate ISchedueSet record from cron expression. This record contains all possible values of minutes, hours, months, days of month, days of week for particular cron expression;
* isTime is to check if we need to run a job in passed date time 

Small sets of unit tests written using mstest:

```fsharp
namespace FsharpTest

open System
open Microsoft.VisualStudio.TestTools.UnitTesting
open Cron
open Schedule

// minute hour dayOfMonth month dayOfWeek
[<TestClass>]
type ScheduleTests () =

    [<TestMethod>]
    member this.All () =
        let schedule = Schedule.generate "* * * * *"
        Assert.IsTrue(List.forall2 ( = ) schedule.DayOfMonth [1..31]);
        Assert.IsTrue(List.forall2 ( = ) schedule.DayOfWeek [0..6]);
        Assert.IsTrue(List.forall2 ( = ) schedule.Hours [0..23]);
        Assert.IsTrue(List.forall2 ( = ) schedule.Minutes [0..59]);
        Assert.IsTrue(List.forall2 ( = ) schedule.Months [1..12]);

    [<TestMethod>]
    member this.Range () =
        let schedule = Schedule.generate "0-5 0-10/2 10-20/3 3-5 2-6/2"
        Assert.IsTrue(List.forall2 ( = ) schedule.DayOfMonth [12; 15; 18]);
        Assert.IsTrue(List.forall2 ( = ) schedule.DayOfWeek [2; 4; 6]);
        Assert.IsTrue(List.forall2 ( = ) schedule.Hours [0; 2; 4; 6; 8; 10]);
        Assert.IsTrue(List.forall2 ( = ) schedule.Minutes [0; 1; 2; 3; 4; 5]);
        Assert.IsTrue(List.forall2 ( = ) schedule.Months [3; 4; 5]);

    [<TestMethod>]
    member this.OneTime () =
        let schedule = Schedule.generate "0 0 1 1 0"
        Assert.IsTrue(List.forall2 ( = ) schedule.DayOfMonth [1]);
        Assert.IsTrue(List.forall2 ( = ) schedule.DayOfWeek [0]);
        Assert.IsTrue(List.forall2 ( = ) schedule.Hours [0]);
        Assert.IsTrue(List.forall2 ( = ) schedule.Minutes [0]);
        Assert.IsTrue(List.forall2 ( = ) schedule.Months [1]);

    [<TestMethod>]
    member this.Every () =
        let schedule = Schedule.generate "*/15 */3 */2 */5 *"
        Assert.IsTrue(List.forall2 ( = ) schedule.DayOfMonth [for i in [1 .. 31] do
                                                                if i%2 = 0 then
                                                                    yield i]
        );
        Assert.IsTrue(List.forall2 ( = ) schedule.DayOfWeek [0 .. 6]);
        Assert.IsTrue(List.forall2 ( = ) schedule.Hours [0; 3; 6; 9; 12; 15; 18; 21]);
        Assert.IsTrue(List.forall2 ( = ) schedule.Minutes [0; 15; 30; 45]);
        Assert.IsTrue(List.forall2 ( = ) schedule.Months [5; 10]);

    [<TestMethod>]
    member this.OnlySome () =
        let schedule = Schedule.generate "1,2,3 5,6,7 28,29 1,2 5,6"
        Assert.IsTrue(List.forall2 ( = ) schedule.DayOfMonth [28;29]);
        Assert.IsTrue(List.forall2 ( = ) schedule.DayOfWeek [5;6]);
        Assert.IsTrue(List.forall2 ( = ) schedule.Hours [5;6;7]);
        Assert.IsTrue(List.forall2 ( = ) schedule.Minutes [1;2;3]);
        Assert.IsTrue(List.forall2 ( = ) schedule.Months [1;2]);

    [<TestMethod>]
    member this.IsTime () =
        let dateTime = DateTime(2000, 1, 1, 0, 0, 0)
        let schedule = { Minutes = [0]; Hours = [0]; DayOfWeek = [0 .. 6]; DayOfMonth = [1]; Months = [1]}
        Assert.IsTrue(Schedule.isTime schedule dateTime)
```

Quite simple implementation of cron schedule part of mentioned service.

Thanks.