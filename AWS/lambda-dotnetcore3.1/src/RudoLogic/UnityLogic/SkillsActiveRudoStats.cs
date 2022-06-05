using System.Collections;
using System.Collections.Generic;
using static GlobalVariables;

public class SkillsActiveRudoStats<T> : SkillsActiveStats<T>  where T : ASkillTriggerTime
{
    public readonly float activateChance;
    public SkillsActiveRudoStats(string name, string iconAddressable, string animationAddressable, SkillInteractions[] skillInteractions, T skillTriggerTime, float activateChance) : base(name, iconAddressable, animationAddressable, skillInteractions, skillTriggerTime)
    {
        this.activateChance = activateChance;
    }
}

