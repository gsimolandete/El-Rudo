using System.Collections;
using System.Collections.Generic;
using static GlobalVariables;

public class AttackProperties
{
    public float damage;
    public FighterCombat attacker;
    public AttackPropertiesEnum[] attackPropertiesEnums;
    public static readonly AttackPropertiesEnum[] emptyEnum = new AttackPropertiesEnum[0] { };

    public AttackProperties(float damage, FighterCombat attacker, AttackPropertiesEnum[] attackPropertiesEnums = null)
    {
        this.damage = damage;
        this.attacker = attacker;
        this.attackPropertiesEnums = attackPropertiesEnums;

        if (this.attackPropertiesEnums == null)
            this.attackPropertiesEnums = new AttackPropertiesEnum[0] { };
    }
}
