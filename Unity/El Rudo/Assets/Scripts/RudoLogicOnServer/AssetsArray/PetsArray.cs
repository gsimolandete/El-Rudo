using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static GlobalVariables;

public class PetsArray
{
    public const int MAXLEGENDARY = 1000, MAXEPIC = 2000, MAXRARE = 3000, MAXCOMMON = 4000;

    public static PetStats GetInstance(int id)
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

    readonly static PetStats[] legendaryWeapons =
    {
        new PetStats("mage",5,5,5,5,new List<Weapon>(), null, new DerivatedStats(0,0,0,0,0,0,0,0,0,0), "Assets/TestAssets/Pets/Wizard/MonD_01.prefab")
    };
    readonly static PetStats[] epicWeapons =
    {
    };
    readonly static PetStats[] rareWeapons =
    {
    };
    readonly static PetStats[] commonWeapons =
    {
    };
}
