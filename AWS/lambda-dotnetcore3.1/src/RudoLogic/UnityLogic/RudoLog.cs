using System.Collections;
using System.Collections.Generic;

public class RudoLog : AbstractRudo
{
    public RudoLog(string name, int lvl, float vitality, float strength, float velocity, float agility, List<Weapon> weapons, List<AbstractPet> abstractPets) : base(name, lvl, vitality, strength, velocity, agility, weapons, abstractPets)
    {
    }

    public override void CompleteGetDisarmed()
    {
        GlobalVariables.PrintWithColor("#B80000", this.FighterName + " got disarmed");
    }

    public override void CompleteAttack(FighterCombat target, float damage)
    {
        GlobalVariables.PrintWithColor(FighterCombat.TeamColor(target.Team), this.FighterName + " dealt " + damage + " to " + target.Fighter.FighterName);
    }

    public override void CompleteYieldWeapon()
    {
        GlobalVariables.PrintWithColor("#DAFF1E", this.FighterName + " yield a weapon");
    }

    public override void MoveCharacter(float attackDistance, FighterCombat fighter)
    {
        
    }
}
