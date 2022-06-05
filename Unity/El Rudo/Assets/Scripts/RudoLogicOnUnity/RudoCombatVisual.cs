using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class RudoCombatVisual : FighterCombatVisual
{
    Slider hpSlider, weaponSkillSlider;
    float maxHp;
    public RudoCombatVisual(Fighter fighter, TeamNum teamnum, Slider hpSlider, Slider weaponSkillSlider) : base(fighter, teamnum)
    {
        this.hpSlider = hpSlider;
        this.weaponSkillSlider = weaponSkillSlider;
        maxHp = Hp;
    }

    public void UpdateHpSLider()
    {
        hpSlider.value = Hp / maxHp;
    }
    public override void ModifyWeaponSkillPoints(float variation)
    {
        base.ModifyWeaponSkillPoints(variation);

        weaponSkillSlider.value = WeaponSkillFillPoints / 50f;
    }
}
