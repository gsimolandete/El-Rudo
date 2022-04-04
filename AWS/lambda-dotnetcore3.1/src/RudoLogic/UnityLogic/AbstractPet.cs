using System.Collections;
using System.Collections.Generic;

public class AbstractPet : Fighter
{
    public override void CompleteAttack(FighterCombat target, float damage)
    {
        throw new System.NotImplementedException();
    }

    public override void GetDisarmed()
    {
        
    }

    public override void MoveCharacter(float attackDistance, FighterCombat fighter)
    {
        throw new System.NotImplementedException();
    }

    public override void NextMove(FighterCombat attacker, FighterCombat target)
    {
        throw new System.NotImplementedException();
    }

    protected override float CalculateDamage()
    {
        throw new System.NotImplementedException();
    }
}
