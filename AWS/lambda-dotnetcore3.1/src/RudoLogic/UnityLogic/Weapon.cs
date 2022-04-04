using System.Collections;
using System.Collections.Generic;
using static GlobalVariables;

[System.Serializable]
public class Weapon
{

    readonly float strengthRatio, agilityRatio, velocityRatio;
    readonly float initiative = 0, multiHit = 0, counterattack = 0, evasion = 0, anticipate = 0, block = 0, armor = 0, disarm = 0, precision = 0, accuracy = 0;
    readonly AttackType weaponType;
    readonly float attackDistance;
    readonly float block_StrengthRatio;

    public float StrengthRatio { get => strengthRatio;}
    public float AgilityRatio { get => agilityRatio;}
    public float VelocityRatio { get => velocityRatio;}
    public AttackType WeaponType { get => weaponType; }
    public float Initiative => initiative;
    public float MultiHit => multiHit;
    public float Counterattack => counterattack;
    public float Evasion => evasion;
    public float Anticipate => anticipate;
    public float Block => block;
    public float Armor => armor;
    public float Disarm => disarm;
    public float Precision => precision;
    public float Accuracy => accuracy;
    public float AttackDistance => attackDistance;
    public float Block_StrengthRatio => block_StrengthRatio;
}
