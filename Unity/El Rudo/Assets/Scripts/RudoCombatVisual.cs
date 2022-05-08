using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class RudoCombatVisual : FighterCombatVisual
{
    Slider hpSlider;
    float maxHp;
    public RudoCombatVisual(Fighter fighter, TeamNum teamnum, Slider hpSlider) : base(fighter, teamnum)
    {
        this.hpSlider = hpSlider;
        maxHp = Hp;
    }

    public override void ModifyHP(float variation)
    {
        base.ModifyHP(variation);

        hpSlider.value = Hp / maxHp;
    }
}
