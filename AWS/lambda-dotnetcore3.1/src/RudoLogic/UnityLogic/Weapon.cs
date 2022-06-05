using System.Collections;
using System.Collections.Generic;
using static GlobalVariables;

[System.Serializable]
public class Weapon : WeaponStats
{
    public Equipable Equipable;

    public Weapon(int nftId, int equipableId, int quality) : base (WeaponsArray.GetInstance(equipableId).name, WeaponsArray.GetInstance(equipableId).strengthRatio, WeaponsArray.GetInstance(equipableId).derivatedStats, WeaponsArray.GetInstance(equipableId).attackType, WeaponsArray.GetInstance(equipableId).attackDistance, WeaponsArray.GetInstance(equipableId).block_DamagePercent, WeaponsArray.GetInstance(equipableId).pathToPrefab, WeaponsArray.GetInstance(equipableId).pathToAnimation, WeaponsArray.GetInstance(equipableId).weaponSkill)
    {
        Equipable = new Equipable(nftId,equipableId,quality);
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
