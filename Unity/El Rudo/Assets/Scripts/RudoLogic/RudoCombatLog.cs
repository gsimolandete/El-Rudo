using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class RudoCombatLog : FighterCombat
{
    public RudoCombatLog(Fighter fighter, TeamNum teamnum) : base(fighter, teamnum)
    {
    }

    public override void CompleteGetDisarmed()
    {
        GlobalVariables.PrintWithColor(this.Fighter.FighterName + " got disarmed", "#B80000");
    }

    public override void CompleteAttack(FighterCombat target, float damage)
    {
        GlobalVariables.PrintWithColor(this.Fighter.FighterName + " dealt " + damage + " to " + target.Fighter.FighterName, FighterCombat.TeamColor(target.Team));
    }

    protected override void CompleteYieldWeapon()
    {
        GlobalVariables.PrintWithColor(this.Fighter.FighterName + " yield a weapon", "#DAFF1E");
    }

    public override void MoveCharacter(float attackDistance, FighterCombat fighter)
    {
        
    }
}
