using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using System;
using UnityEngine.Profiling;

// grabbed from https://stackoverflow.com/a/13744322/4490823
public static class MiscExctensions
{
    public static IEnumerable<IEnumerable<T>> Split<T>(this ICollection<T> items,
                                                       int numberOfChunks)
    {
        if (numberOfChunks <= 0 || numberOfChunks > items.Count)
            throw new ArgumentOutOfRangeException("numberOfChunks");

        int sizePerPacket = items.Count / numberOfChunks;
        int extra = items.Count % numberOfChunks;

        for (int i = 0; i < numberOfChunks - extra; i++)
            yield return items.Skip(i * sizePerPacket).Take(sizePerPacket);

        int alreadyReturnedCount = (numberOfChunks - extra) * sizePerPacket;
        int toReturnCount = extra == 0 ? 0 : (items.Count - numberOfChunks) / extra + 1;
        for (int i = 0; i < extra; i++)
            yield return items.Skip(alreadyReturnedCount + i * toReturnCount).Take(toReturnCount);
    }

    public static T profileF<T>(this string label, Func<T> func)
    {
        Profiler.BeginSample(label);
        T result = func();
        Profiler.EndSample();
        return result;
    }

    public static void profile(this string label, Action func)
    {
        Profiler.BeginSample(label);
        func();
        Profiler.EndSample();
    }

}

