---
layout: post
title: "Daemon cron using F#"
date: 2017-05-19
categories: dotnet fsharp programming
---
As continuation for [my previous post]({% post 2017-06-01-Cron-schedule-using-FSharp %}) I want to create a daemon which runs jobs using created cron code.

{% highlight fsharp %}
module Daemon =
    [<Literal>]
    let INTERVAL = 30000

    let internalRun interval (now: unit->DateTime) (jobs: seq<Job>) =
        // to dispose System.Threading.Timer properly
        let createDisposable f =
            {
                new IDisposable with
                    member x.Dispose() = f()
            }
        
        // event fot Timer
        let timerElapsed obj =
            let checkJob = () |> now |> Schedule.isTime
            jobs 
            |> Seq.map (fun x ->
                let schedule = Schedule.generate x.cron
                (schedule, x.action)
            ) 
            |> Seq.filter (fun (x, y) -> checkJob x)
            |> Seq.map (fun (x, y) -> y) 
            |> Async.Parallel 
            |> Async.RunSynchronously
            |> ignore
        
        // timer
        let localTimer = new Timer(timerElapsed, null, Timeout.Infinite, interval)
        // start timer
        localTimer.Change(0, interval) |> ignore
        // return timer as IDisposable 
        createDisposable (fun() -> localTimer.Dispose())

    // get DateTime
    let now = fun () -> DateTime.UtcNow
    // public method to call
    let run jobs = internalRun INTERVAL now jobs
{% endhighlight %}

How to use the daemon above:

{% highlight fsharp %}
type Job = { action: Async<unit>; cron: string }

let act id =
    async {
        printfn "Execution %A" id
    }
let jobs = [|1; 2|] |> Array.map (fun id ->
    {
        action = act id;
        cron = "* * * * *"
    })
let daemon = run jobs
{% endhighlight %}

Unfortunately I don't see the good way to wtite unit test for this code as there is hardcoded dependency to System.Threading.Timer.

Feel free to comment if you see the solution. Thanks in advance.