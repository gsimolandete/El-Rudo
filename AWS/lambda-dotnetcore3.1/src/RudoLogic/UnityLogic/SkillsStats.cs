using System.Collections;
using System.Collections.Generic;
using static GlobalVariables;

public abstract class SkillsStats
{
    public readonly string name;
    public readonly string iconAddressable;

    public SkillsStats(string name, string iconAddressable)
    {
        this.name = name;
        this.iconAddressable = iconAddressable;
    }
}
