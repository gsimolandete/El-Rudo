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

    public Random Random { get => random; set => random = value; }
}
