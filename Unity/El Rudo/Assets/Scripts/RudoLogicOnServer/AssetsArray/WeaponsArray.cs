using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static GlobalVariables;
public static class WeaponsArray
{
    public const int MAXLEGENDARY = 1000, MAXEPIC = 2000, MAXRARE = 3000, MAXCOMMON = 4000;

    public static WeaponStats GetInstance(int id)
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

    readonly static WeaponStats[] legendaryWeapons =
    {
        new WeaponStats("katana",0.5f, new DerivatedStats(0,0,0,0,0,0,0,0,0,0), AttackType.Melee, .3f, 0, "Assets/TestAssets/Weapons/EquipableSpawnProperties.asset","Assets/TestAssets/Rudos/AttackBackwards.anim",
            new SkillsActiveWeaponStats("","","Assets/TestAssets/Rudos/SampleWeaponSkill.anim",new SkillInteractions[0],new AttackSkillTrigger(1),15f)),
        new WeaponStats("sword",0.5f, new DerivatedStats(0,0,0,0,0,0,0,0,0,0), AttackType.Melee, .3f, 0, "Assets/TestAssets/Weapons/SampleSword.asset","Assets/AssetStore/Wizard - 2D Character/Animations/Attack.anim",
            new SkillsActiveWeaponStats("","","Assets/TestAssets/Rudos/SampleWeaponSkill2.anim",new SkillInteractions[0],new AttackSkillTrigger(1),15f))
    };
    readonly static WeaponStats[] epicWeapons =
    {
    };
    readonly static WeaponStats[] rareWeapons =
    {
    };
    readonly static WeaponStats[] commonWeapons = 
    {
    };
}
