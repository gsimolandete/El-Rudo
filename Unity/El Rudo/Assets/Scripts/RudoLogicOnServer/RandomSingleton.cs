using System;
using System.Collections;
using System.Collections.Generic;
using static ServerDifferentFunctions;

public sealed class RandomSingleton
{
    static readonly RandomSingleton instance = new RandomSingleton();
    Random random;

    static RandomSingleton()
    {
    }

    private RandomSingleton()
    {
        random = new Random();
    }

    public static RandomSingleton Instance
    {
        get
        {
            return instance;
        }
    }

    public static double NextDouble()
    {
        return instance.random.NextDouble();
    }
    public static void ShuffleSortedList<TValue>(SortedList<int, TValue> list)
    {
        int n = list.Count;
        while (n > 1)
        {
            n--;
            int k = (int)(NextDouble() * (n + 1));
            TValue value = list[k];
            list[k] = list[n];
            list[n] = value;
        }
    }

    public Random Random { get => random; set => random = value; }
}
