using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static GlobalVariables;

public abstract class FighterCombatVisual : FighterCombat
{
    protected FighterController fighterController;

    public FighterController FighterController { get => fighterController; set => fighterController = value; }

    public FighterCombatVisual(Fighter fighter, TeamNum team) : base(fighter,team)
    {
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
        FighterController.Initialize(thisRudoSpawn, enemyRudoSpawn);
    }

    public override IEnumerator CompleteAttack(FighterCombat target, float damage, AttackInteraction attackInteraction, float targetInteractionDelay) 
    {
        CombatDynamicsInstance.StartCoroutine(base.CompleteAttack( target,  damage,  attackInteraction,  targetInteractionDelay));
        FighterController.Attack();
        switch (attackInteraction)
        {
            case AttackInteraction.Clean:
                yield return new WaitForSeconds(targetInteractionDelay);
                if (target.Hp > 0)
                    (target as FighterCombatVisual).FighterController.GetHurt();
                else
                {
                    (CombatDynamicsInstance as CombatDynamicsVisual).LoadMainMenu.SetActive(true);
                    (target as FighterCombatVisual).FighterController.GetDefeated();
                }
                break;
            case AttackInteraction.Blocked:
                if (target.Hp > 0)
                    (target as FighterCombatVisual).FighterController.Block();
                else
                    (target as FighterCombatVisual).FighterController.GetDefeated();
                break;
            case AttackInteraction.Dodged:
                (target as FighterCombatVisual).FighterController.Dodge();
                break;
            default:
                break;
        }
    }

    public override void CompleteGetDisarmed(DisarmInteraction disarmInteraction)
    {
        base.CompleteGetDisarmed(disarmInteraction);
        switch (disarmInteraction)
        {
            case DisarmInteraction.Forced:
                fighterController.ForcedDisarm();
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
        GameObject rcvt = (target as RudoCombatVisual).FighterController.gameObject;
        FighterController.SetTargetPosition(rcvt.transform.position + ((rcvt.transform.position - FighterController.gameObject.transform.position).x > 0 ? 
            new Vector3(-attackDistance, 0, 0) : 
            new Vector3(attackDistance, 0, 0)));

    }

    protected override void CompleteYieldWeapon()
    {
        base.CompleteYieldWeapon();

        fighterController.YieldWeapon(activeWeapon);
    }
}
