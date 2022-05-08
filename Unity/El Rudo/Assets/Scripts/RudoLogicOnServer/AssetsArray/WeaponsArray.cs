using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static GlobalVariables;
public static class WeaponsArray
{
    public const int MAXLEGENDARY = 1000, MAXEPIC = 2000, MAXRARE = 3000, MAXCOMMON = 4000;

    public static WeaponStats GetInstance(int id)
    {
        if (id < MAXLEGENDARY)
        {
            return legendaryWeapons[id % legendaryWeapons.Length];
        }
        else if (id < MAXLEGENDARY + MAXEPIC)
        {
            return epicWeapons[id % epicWeapons.Length + MAXLEGENDARY];
        }
        else if (id < MAXLEGENDARY + MAXEPIC + MAXRARE)
        {
            return rareWeapons[id % rareWeapons.Length + MAXLEGENDARY + MAXEPIC];
        }
        else
        {
            return commonWeapons[id % commonWeapons.Length + MAXLEGENDARY + MAXEPIC + MAXCOMMON];
        }
    }

    readonly static WeaponStats[] legendaryWeapons =
    {
        new WeaponStats(1, new DerivatedStats(0,0,0,0,0,0,0,0,0,0), AttackType.Melee, 1.5f, 0),
        new WeaponStats(1, new DerivatedStats(0,0,0,0,0,0,0,0,0,0), AttackType.Melee, 1.5f, 0),
        new WeaponStats(1, new DerivatedStats(0,0,0,0,0,0,0,0,0,0), AttackType.Melee, 1.5f, 0),
        new WeaponStats(1, new DerivatedStats(0,0,0,0,0,0,0,0,0,0), AttackType.Melee, 1.5f, 0),
        new WeaponStats(1, new DerivatedStats(0,0,0,0,0,0,0,0,0,0), AttackType.Melee, 1.5f, 0)
    };
    readonly static WeaponStats[] epicWeapons =
    {
        new WeaponStats(1, new DerivatedStats(0,0,0,0,0,0,0,0,0,0), AttackType.Melee, 1.5f, 0),
        new WeaponStats(1, new DerivatedStats(0,0,0,0,0,0,0,0,0,0), AttackType.Melee, 1.5f, 0),
        new WeaponStats(1, new DerivatedStats(0,0,0,0,0,0,0,0,0,0), AttackType.Melee, 1.5f, 0),
        new WeaponStats(1, new DerivatedStats(0,0,0,0,0,0,0,0,0,0), AttackType.Melee, 1.5f, 0),
        new WeaponStats(1, new DerivatedStats(0,0,0,0,0,0,0,0,0,0), AttackType.Melee, 1.5f, 0)
    };
    readonly static WeaponStats[] rareWeapons =
    {
        new WeaponStats(1, new DerivatedStats(0,0,0,0,0,0,0,0,0,0), AttackType.Melee, 1.5f, 0),
        new WeaponStats(1, new DerivatedStats(0,0,0,0,0,0,0,0,0,0), AttackType.Melee, 1.5f, 0),
        new WeaponStats(1, new DerivatedStats(0,0,0,0,0,0,0,0,0,0), AttackType.Melee, 1.5f, 0),
        new WeaponStats(1, new DerivatedStats(0,0,0,0,0,0,0,0,0,0), AttackType.Melee, 1.5f, 0),
        new WeaponStats(1, new DerivatedStats(0,0,0,0,0,0,0,0,0,0), AttackType.Melee, 1.5f, 0)
    };
    readonly static WeaponStats[] commonWeapons = 
    { 
        new WeaponStats(1, new DerivatedStats(0,0,0,0,0,0,0,0,0,0), AttackType.Melee, 1.5f, 0),
        new WeaponStats(1, new DerivatedStats(0,0,0,0,0,0,0,0,0,0), AttackType.Melee, 1.5f, 0),
        new WeaponStats(1, new DerivatedStats(0,0,0,0,0,0,0,0,0,0), AttackType.Melee, 1.5f, 0),
        new WeaponStats(1, new DerivatedStats(0,0,0,0,0,0,0,0,0,0), AttackType.Melee, 1.5f, 0),
        new WeaponStats(1, new DerivatedStats(0,0,0,0,0,0,0,0,0,0), AttackType.Melee, 1.5f, 0)
    };
}
