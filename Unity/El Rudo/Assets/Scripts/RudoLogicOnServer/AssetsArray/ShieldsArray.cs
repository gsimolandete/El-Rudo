using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static GlobalVariables;
public static class ShieldArray
{
    public const int MAXLEGENDARY = 1000, MAXEPIC = 2000, MAXRARE = 3000, MAXCOMMON = 4000;

    public static ShieldStats GetInstance(int id)
    {
        return legendaryWeapons[id % legendaryWeapons.Length];

        if (id < MAXLEGENDARY)
        {
            return legendaryWeapons[id % legendaryWeapons.Length];
        }
        else if (id < MAXLEGENDARY + MAXEPIC)
        {
            return epicWeapons[(id + MAXLEGENDARY) % epicWeapons.Length];
        }
        else if (id < MAXLEGENDARY + MAXEPIC + MAXRARE)
        {
            return rareWeapons[(id + MAXLEGENDARY + MAXEPIC) % rareWeapons.Length];
        }
        else
        {
            return commonWeapons[(id + MAXLEGENDARY + MAXEPIC + MAXCOMMON) % commonWeapons.Length];
        }
    }

    readonly static ShieldStats[] legendaryWeapons =
    {
        new ShieldStats(10, 0, 0.4f, "Assets/TestAssets/Shield/Shield1.asset")
    };
    readonly static ShieldStats[] epicWeapons =
    {
    };
    readonly static ShieldStats[] rareWeapons =
    {
    };
    readonly static ShieldStats[] commonWeapons =
    {
    };
}
