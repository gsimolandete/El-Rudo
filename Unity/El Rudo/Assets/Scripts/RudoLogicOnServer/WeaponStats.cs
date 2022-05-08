using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static GlobalVariables;

public class WeaponStats
{
    [SerializeField]
    protected float strengthRatio;
    [SerializeField]
    protected DerivatedStats derivatedStats;
    [SerializeField]
    protected AttackType weaponType;
    [SerializeField]
    protected float attackDistance, block_DamagePercent;

    public WeaponStats(float strengthRatio, DerivatedStats derivatedStats, AttackType weaponType, float attackDistance, float block_DamagePercent)
    {
        this.strengthRatio = strengthRatio;
        this.derivatedStats = derivatedStats;
        this.weaponType = weaponType;
        this.attackDistance = attackDistance;
        this.block_DamagePercent = block_DamagePercent;
    }
    public DerivatedStats DerivatedStats { get => derivatedStats; }
    public float AttackDistance => defaultAttackDistance + attackDistance;
    public float Block_DamagePercent => block_DamagePercent + emptyHandedBlockPercent;
    public float StrengthRatio { get => strengthRatio; }
    public AttackType WeaponType { get => weaponType; }
}
