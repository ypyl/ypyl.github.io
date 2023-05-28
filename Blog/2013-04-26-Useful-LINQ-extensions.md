---
layout: post
title: "Useful LINQ extensions"
date: 2013-04-26

tags: dotnet linq
categories: programming
---
# Useful LINQ extensions

After reading a lot of articles about this theme, I start to use the next [extensions](http://pastebin.com/As5as2pE)

```cs
public static class Linq
{
    public static IEnumerable<T> Except<T>(this IEnumerable<T> source, IEnumerable<T> target, Func<T, T, bool> func)
    {
        return source.Except(target, new LambdaComparer<T>(func));
    }

    public static TResult With<TInput, TResult>(this TInput o, Func<TInput, TResult> evaluator)
        where TInput : class
    {
        if (o == null) return default(TResult);
        return evaluator(o);
    }

    public static TResult Return<TInput, TResult>(this TInput o, Func<TInput, TResult> evaluator, TResult failure_value) where TInput : class
    {
        if (o == null) return failure_value;
        return evaluator(o);
    }

    public static bool Check<TInput>(this TInput o, Func<TInput, bool> evaluator) where TInput : class
    {
        if (o == null) return false;
        return evaluator(o);
    }

    public static TInput If<TInput>(this TInput o, Func<TInput, bool> evaluator) where TInput : class
    {
        if (o == null) return null;
        return evaluator(o) ? o : null;
    }

    public static TInput Unless<TInput>(this TInput o, Func<TInput, bool> evaluator) where TInput : class
    {
        if (o == null) return null;
        return evaluator(o) ? null : o;
    }

    public static TInput Do<TInput>(this TInput o, Action<TInput> action) where TInput : class
    {
        if (o == null) return null;
        action(o);
        return o;
    }

    public static List<TInput> Delete<TInput>(this List<TInput> o, Func<TInput, bool> evaluator) where TInput : class
    {
        var listToDelete = o.Where(evaluator).ToList();
        foreach (var input in listToDelete)
        {
            o.Remove(input);
        }
        return o;
    }
}


public class LambdaComparer<T> : IEqualityComparer<T>
{
    private readonly Func<T, T, bool> _lambdaComparer;
    private readonly Func<T, int> _lambdaHash;

    public LambdaComparer(Func<T, T, bool> lambdaComparer) :
        this(lambdaComparer, o => 0)
    {
    }

    public LambdaComparer(Func<T, T, bool> lambdaComparer, Func<T, int> lambdaHash)
    {
        if (lambdaComparer == null)
            throw new ArgumentNullException("lambdaComparer");
        if (lambdaHash == null)
            throw new ArgumentNullException("lambdaHash");

        _lambdaComparer = lambdaComparer;
        _lambdaHash = lambdaHash;
    }

    public bool Equals(T x, T y)
    {
        return _lambdaComparer(x, y);
    }

    public int GetHashCode(T obj)
    {
        return _lambdaHash(obj);
    }
}
```