using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static GlobalVariables;

public class SkillsPassiveStats : SkillsStats
{
    public readonly DerivatedStats DerivatedStats;
    public readonly BasicStats BasicStats;

    public SkillsPassiveStats(string name, string iconAddressable, DerivatedStats derivatedStats, BasicStats basicStats) : base(name, iconAddressable)
    {
        DerivatedStats = derivatedStats;
        BasicStats = basicStats;
    }
}
