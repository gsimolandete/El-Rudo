using System.Collections;
using System.Collections.Generic;
using static GlobalVariables;

public class SkillsActiveWeaponStats : SkillsActiveStats<AttackSkillTrigger>
{
    public readonly float pointsToActivate;
    public SkillsActiveWeaponStats(string name, string iconAddressable, string animationAddressable, SkillInteractions[] skillInteractions, AttackSkillTrigger skillTriggerTime, float pointsToActivate) : base(name,iconAddressable,animationAddressable,skillInteractions,skillTriggerTime)
    {
        this.pointsToActivate = pointsToActivate;
    }
}
