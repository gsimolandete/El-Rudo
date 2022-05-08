using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static GlobalVariables;

[CreateAssetMenu(fileName = "RudoLog", menuName = "ScriptableObjects/Weapon", order = 1)]
[System.Serializable]
public class Weapon : WeaponStats
{
    [SerializeField]
    public Equipable Equipable;

    public Weapon(int nftId, int equipableId, int quality, string pathToAddressable, WeaponStats weaponStats) : base ( weaponStats.StrengthRatio,  weaponStats.DerivatedStats, weaponStats.WeaponType, weaponStats.AttackDistance, weaponStats.Block_DamagePercent)
    {
        Equipable = new Equipable(nftId,equipableId,quality, pathToAddressable);
    }
    public float Initiative => derivatedStats.initiative;
    public float MultiHit => derivatedStats.multiHit;
    public float Counterattack => derivatedStats.counterattack;
    public float Evasion => derivatedStats.evasion;
    public float Anticipate => derivatedStats.anticipate;
    public float Block => derivatedStats.block;
    public float Armor => derivatedStats.armor;
    public float Disarm => derivatedStats.disarm;
    public float Precision => derivatedStats.precision;
    public float Accuracy => derivatedStats.accuracy;
}
