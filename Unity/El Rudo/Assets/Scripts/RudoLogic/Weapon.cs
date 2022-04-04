using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static GlobalVariables;

[CreateAssetMenu(fileName = "RudoLog", menuName = "ScriptableObjects/Weapon", order = 1)]
[System.Serializable]
public class Weapon : ScriptableObject
{
    [SerializeField]
    Rarities rarity;
    [SerializeField]
    float strengthRatio;
    [SerializeField]
    float initiative = 0, multiHit = 0, counterattack = 0, evasion = 0, anticipate = 0, block = 0, armor = 0, disarm = 0, precision = 0, accuracy = 0;
    [SerializeField]
    AttackType weaponType;
    [SerializeField]
    float attackDistance, block_DamagePercent;

    public float StrengthRatio { get => strengthRatio;}
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
    public float AttackDistance => defaultAttackDistance + attackDistance;
    public float Block_DamagePercent => block_DamagePercent + emptyHandedBlockPercent;
}
