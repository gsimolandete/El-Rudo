using System.Collections;
using System.Collections.Generic;
using static GlobalVariables;

public class WeaponStats
{
    public readonly string name;
    public readonly float strengthRatio;
    public readonly DerivatedStats derivatedStats;
    public readonly AttackType attackType;
    public readonly float attackDistance, block_DamagePercent;
    public readonly string pathToPrefab, pathToAnimation;
    public readonly SkillsActiveWeaponStats weaponSkill;

    public WeaponStats(string name, float strengthRatio, DerivatedStats derivatedStats, AttackType attackType, float attackDistance, float block_DamagePercent, string pathToPrefab, string pathToAnimation, SkillsActiveWeaponStats weaponSkill)
    {
        this.name = name;
        this.strengthRatio = strengthRatio;
        this.derivatedStats = derivatedStats;
        this.attackType = attackType;
        this.attackDistance = attackDistance;
        this.block_DamagePercent = block_DamagePercent;
        this.pathToPrefab = pathToPrefab;
        this.pathToAnimation = pathToAnimation;
        this.weaponSkill = weaponSkill;
    }
}
