using System.Collections;
using System.Collections.Generic;
using static GlobalVariables;

public class SkillsArray
{
    public const int MAXLEGENDARY = 100, MAXEPIC = 150, MAXRARE = 200, MAXCOMMON = 250;

    public static SkillsStats GetInstance(int id)
    {
        return legendaryWeapons[id % legendaryWeapons.Length];

        if (id < MAXLEGENDARY)
        {
            return legendaryWeapons[id % legendaryWeapons.Length];
        }
        else if (id < MAXLEGENDARY + MAXEPIC)
        {
            return epicWeapons[(id + MAXLEGENDARY) % epicWeapons.Length];
        }
        else if (id < MAXLEGENDARY + MAXEPIC + MAXRARE)
        {
            return rareWeapons[(id + MAXLEGENDARY + MAXEPIC) % rareWeapons.Length];
        }
        else
        {
            return commonWeapons[(id + MAXLEGENDARY + MAXEPIC + MAXCOMMON) % commonWeapons.Length];
        }
    }

    readonly static SkillsStats[] legendaryWeapons =
    {
        new SkillsPassiveStats("fast attacks", "", new DerivatedStats(0,0.1f,0,0,0,0,0,0,0,0), new BasicStats()),
        new SkillsActiveRudoStats<DefensiveBlockSkillTrigger>("parry","","ParryNextAttack",new SkillInteractions[1]{ SkillInteractions.DisarmAttacker}, new DefensiveBlockSkillTrigger(1), 0.4f)
    };
    readonly static SkillsStats[] epicWeapons =
    {
    };
    readonly static SkillsStats[] rareWeapons =
    {
    };
    readonly static SkillsStats[] commonWeapons =
    {
    };
}
