using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ActiveSkills
{
    public SortedList<int, SkillsActiveRudoStats<DefensiveBlockSkillTrigger>> BlockSkillTriggers;

    public ActiveSkills()
    {
        BlockSkillTriggers = new SortedList<int, SkillsActiveRudoStats<DefensiveBlockSkillTrigger>>();
    }

    public SkillsActiveRudoStats<DefensiveBlockSkillTrigger> GetBlockSkill()
    {
        RandomSingleton.ShuffleSortedList(BlockSkillTriggers);
        for (int i = 0; i < BlockSkillTriggers.Count; i++)
        {
            if (RandomSingleton.NextDouble() <= BlockSkillTriggers.Values[i].activateChance)
            {
                return BlockSkillTriggers.Values[i];
            }
        }

        return null;
    }
}
