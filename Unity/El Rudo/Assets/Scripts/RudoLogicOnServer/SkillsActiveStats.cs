using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static GlobalVariables;

public abstract class SkillsActiveStats<T> : SkillsStats where T : ASkillTriggerTime
{
    public readonly string animationAddressable;
    public readonly SkillInteractions[] skillInteractions;
    public readonly T skillTriggerTime;

    protected SkillsActiveStats(string name, string iconAddressable,string animationAddressable, SkillInteractions[] skillInteractions, T skillTriggerTime) : base(name, iconAddressable)
    {
        this.animationAddressable = animationAddressable;
        this.skillInteractions = skillInteractions;
        this.skillTriggerTime = skillTriggerTime;
    }

    public void DoInteractions(FighterCombat attacker, FighterCombat attacked)
    {
        for (int i = 0; i < skillInteractions.Length; i++)
        {
            switch (skillInteractions[i])
            {
                case SkillInteractions.DisarmAttacker:
                    attacker.Disarm(DisarmInteraction.Forced);
                    break;
                case SkillInteractions.DisarmTarget:
                    attacked.Disarm(DisarmInteraction.Forced);
                    break;
                default:
                    break;
            }
        }
    }
}
public abstract class ASkillTriggerTime { }
public class DefensiveBlockSkillTrigger : ASkillTriggerTime
{
    public readonly float BlockPercent;

    public DefensiveBlockSkillTrigger(float blockPercent)
    {
        BlockPercent = blockPercent;
    }
}
public class AttackSkillTrigger : ASkillTriggerTime
{
    public readonly float StrengthRatio;

    public AttackSkillTrigger(float StrengthRatio)
    {
        this.StrengthRatio = StrengthRatio;
    }
}

