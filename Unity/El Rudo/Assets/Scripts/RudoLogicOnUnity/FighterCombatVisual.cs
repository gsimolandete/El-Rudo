using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.AddressableAssets;
using static GlobalVariables;
using CharacterCreator2D;

public abstract class FighterCombatVisual : FighterCombat
{
    protected FighterController fighterController;

    public FighterController FighterController { get => fighterController; set => fighterController = value; }

    public FighterCombatVisual(Fighter fighter, TeamNum team) : base(fighter,team)
    {
    }

    public override void ModifyHP(float variation)
    {
        base.ModifyHP(variation);
    }

    public override void ModifyShieldHp(float variation)
    {
        base.ModifyShieldHp(variation);

        if (ShieldHp <= 0)
        {
            fighterController.SelfDisarmShield();
        }
    }

    protected override IEnumerator WaitUntilActionsEnded(FighterCombat target)
    {
        yield return new WaitUntil(()=>
        {
            return FighterController.FinishedAction && (target as FighterCombatVisual).FighterController.FinishedAction;
        });
    }

    public void InitializeRudoCombatVisual(GameObject rudoController, Vector3 thisRudoSpawn, Vector3 enemyRudoSpawn)
    {
        FighterController = rudoController.GetComponent<FighterController>();
        FighterController.Initialize(thisRudoSpawn, enemyRudoSpawn, this is RudoCombatVisual ? this as RudoCombatVisual : null);

        if (activeShield != null)
            FighterController.YieldShield(activeShield);
    }

    protected override void BasicAttack(FighterCombat target, AttackPropertiesEnum[] attackPropertiesEnums = null) 
    {
        base.BasicAttack(target, attackPropertiesEnums);
        FighterController.Attack((target as FighterCombatVisual).fighterController);
    }
    protected override void WeaponSkillAttack(FighterCombat target)
    {
        base.WeaponSkillAttack(target);
        FighterController.WeaponSkillAttack((target as FighterCombatVisual).fighterController);
    }
    public override void GetHurt(float damage)
    {
        base.GetHurt(damage);
        if (Hp > 0)
            FighterController.BeingHurt();
        else
            FighterController.BeingDefeated();
    }
    public override DefenseInteraction Block(AttackProperties attacker)
    {
        DefenseInteraction ai = base.Block(attacker);

        if (Hp > 0)
        {
            if(ai!= DefenseInteraction.None)
                FighterController.Blocking();
        }
        else
            FighterController.BeingDefeated();

        return ai;
    }
    public override void Dodge(AttackProperties attacker)
    {
        base.Dodge(attacker);

        FighterController.Dodging();
    }

    public override void CompleteGetDisarmed(DisarmInteraction disarmInteraction)
    {
        base.CompleteGetDisarmed(disarmInteraction);
        switch (disarmInteraction)
        {
            case DisarmInteraction.Forced:
                fighterController.Disarming();
                break;
            case DisarmInteraction.Intentional:
                fighterController.SelfDisarm();
                break;
            default:
                break;
        }

    }
    public override void MoveCharacterToSpawn()
    {
        FighterController.SetTargetPosition(FighterController.OwnSpawn);
    }
    public override void MoveCharacterToAttack(float attackDistance, FighterCombat target)
    {
        GameObject rcvt = (target as FighterCombatVisual).FighterController.gameObject;
        FighterController.SetTargetPosition(rcvt.transform.position + ((rcvt.transform.position - FighterController.gameObject.transform.position).x > 0 ? 
            new Vector3(-attackDistance, 0, 0) : 
            new Vector3(attackDistance, 0, 0)));

    }
    protected override void CompleteYieldWeapon()
    {
        base.CompleteYieldWeapon();

        fighterController.YieldWeapon(activeWeapon);
        
    }
    protected override void CompleteUseActiveSkill<T>(SkillsActiveStats<T> skillsActiveStats)
    {
        base.CompleteUseActiveSkill(skillsActiveStats);

        fighterController.Invoke(skillsActiveStats.animationAddressable,0f);
    }
    //void parry()
    //{
    //    fighterController.ParryNextAttack();
    //}

}
