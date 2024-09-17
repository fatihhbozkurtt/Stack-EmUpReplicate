using System;
using System.Collections.Generic;

public static class ListExtensions
{
    private static Random _rng = new Random();

    public static void Shuffle<T>(this List<T> list)
    {
        var n = list.Count;
        while (n > 1)
        {
            n--;
            var k = _rng.Next(n + 1);
            (list[k], list[n]) = (list[n], list[k]);
        }
    }
}