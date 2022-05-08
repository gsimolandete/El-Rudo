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

    readonly static WeaponStats[] legendaryWeapons =
    {
        new WeaponStats(1.2f, new DerivatedStats(0,0,0,0,0,0,0,0,0,0), AttackType.Melee, .3f, 0, "Assets/TestAssets/Weapons/katana.prefab"),
        new WeaponStats(0.6f, new DerivatedStats(0,0.2f,0,0,0,0,0,0,0,0), AttackType.Melee, 0, 0, "Assets/TestAssets/Weapons/cuchillo.prefab"),
        new WeaponStats(1, new DerivatedStats(0,0,0,0,0,0,0,0,0,0), AttackType.Melee, .3f, 0, "Assets/TestAssets/Weapons/sword_01.prefab"),
        new WeaponStats(1, new DerivatedStats(0,0,0,0,0,0,0,0,0,0), AttackType.Melee, .3f, 0, "Assets/TestAssets/Weapons/nose.prefab"),
        new WeaponStats(1.6f, new DerivatedStats(0,0,0,0,0,0,0,0,0,0), AttackType.Melee, .3f, 0, "Assets/TestAssets/Weapons/maza.prefab")
    };
    readonly static WeaponStats[] epicWeapons =
    {
        new WeaponStats(1.2f, new DerivatedStats(0,0,0,0,0,0,0,0,0,0), AttackType.Melee, .3f, 0, "Assets/TestAssets/Weapons/katana.prefab"),
        new WeaponStats(0.6f, new DerivatedStats(0,0.2f,0,0,0,0,0,0,0,0), AttackType.Melee, 0, 0, "Assets/TestAssets/Weapons/cuchillo.prefab"),
        new WeaponStats(1, new DerivatedStats(0,0,0,0,0,0,0,0,0,0), AttackType.Melee, .3f, 0, "Assets/TestAssets/Weapons/sword_01.prefab"),
        new WeaponStats(1, new DerivatedStats(0,0,0,0,0,0,0,0,0,0), AttackType.Melee, .3f, 0, "Assets/TestAssets/Weapons/nose.prefab"),
        new WeaponStats(1.6f, new DerivatedStats(0,0,0,0,0,0,0,0,0,0), AttackType.Melee, .3f, 0, "Assets/TestAssets/Weapons/maza.prefab")
    };
    readonly static WeaponStats[] rareWeapons =
    {
        new WeaponStats(1.2f, new DerivatedStats(0,0,0,0,0,0,0,0,0,0), AttackType.Melee, .3f, 0, "Assets/TestAssets/Weapons/katana.prefab"),
        new WeaponStats(0.6f, new DerivatedStats(0,0.2f,0,0,0,0,0,0,0,0), AttackType.Melee, 0, 0, "Assets/TestAssets/Weapons/cuchillo.prefab"),
        new WeaponStats(1, new DerivatedStats(0,0,0,0,0,0,0,0,0,0), AttackType.Melee, .3f, 0, "Assets/TestAssets/Weapons/sword_01.prefab"),
        new WeaponStats(1, new DerivatedStats(0,0,0,0,0,0,0,0,0,0), AttackType.Melee, .3f, 0, "Assets/TestAssets/Weapons/nose.prefab"),
        new WeaponStats(1.6f, new DerivatedStats(0,0,0,0,0,0,0,0,0,0), AttackType.Melee, .3f, 0, "Assets/TestAssets/Weapons/maza.prefab")
    };
    readonly static WeaponStats[] commonWeapons = 
    {
        new WeaponStats(1.2f, new DerivatedStats(0,0,0,0,0,0,0,0,0,0), AttackType.Melee, .3f, 0, "Assets/TestAssets/Weapons/katana.prefab"),
        new WeaponStats(0.6f, new DerivatedStats(0,0.2f,0,0,0,0,0,0,0,0), AttackType.Melee, 0, 0, "Assets/TestAssets/Weapons/cuchillo.prefab"),
        new WeaponStats(1, new DerivatedStats(0,0,0,0,0,0,0,0,0,0), AttackType.Melee, .3f, 0, "Assets/TestAssets/Weapons/sword_01.prefab"),
        new WeaponStats(1, new DerivatedStats(0,0,0,0,0,0,0,0,0,0), AttackType.Melee, .3f, 0, "Assets/TestAssets/Weapons/nose.prefab"),
        new WeaponStats(1.6f, new DerivatedStats(0,0,0,0,0,0,0,0,0,0), AttackType.Melee, .3f, 0, "Assets/TestAssets/Weapons/maza.prefab")
    };
}
