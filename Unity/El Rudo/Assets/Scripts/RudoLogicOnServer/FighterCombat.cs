using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static GlobalVariables;

public class FighterCombat
{
    public enum TeamNum { Team1, Team2 }
    public static string TeamColor(TeamNum team) {
        switch (team) { 
            case TeamNum.Team1:
                return "#13F8FF";
            case TeamNum.Team2:
                return "#FF4113";
            default:
                throw new System.Exception();
        }
    }
    [SerializeField]
    TeamNum team;
    [SerializeField]
    Fighter fighter;
    [SerializeField]
    float turnMeter;
    [SerializeField]
    float hp, shieldHp;
    [SerializeField]
    protected Weapon activeWeapon;
    [SerializeField]
    protected Shield activeShield;
    [SerializeField]
    protected AttackType attackType;
    [SerializeField]
    protected float attackDistance;

    public EventHandler hpModifiedHandler;

    public FighterCombat(Fighter fighter, TeamNum team)
    {
        activeWeapon = null;

        this.fighter = fighter;
        turnMeter = fighter.Initiative;
        hp = MaxHP(fighter);
        if(fighter.Shield!=null)
            shieldHp = fighter.Shield.ShieldHealth;
        this.team = team;
        attackDistance = defaultAttackDistance;
    }

    public Fighter Fighter { get => fighter; set => fighter = value; }
    public float TurnMeter { get => turnMeter; set => turnMeter = value; }
    public float Hp { get => hp; }
    public TeamNum Team { get => team; set => team = value; }
    public float ShieldHp { get => shieldHp; }

    //DERIVATEDS
    public Weapon ActiveWeapon { get => activeWeapon; set => activeWeapon = value; }
    public float Initiative { get { return fighter.Initiative + (activeWeapon != null ? activeWeapon.Initiative : 0); } }
    public float MultiHit { get { return fighter.MultiHit + (activeWeapon != null ? activeWeapon.MultiHit : 0); } }
    public float Counterattack { get { return fighter.Counterattack + (activeWeapon != null ? activeWeapon.Counterattack : 0); } }
    public float Evasion { get { return fighter.Evasion + (activeWeapon != null ? activeWeapon.Evasion : 0); } }
    public float Anticipate { get { return fighter.Anticipate + (activeWeapon != null ? activeWeapon.Anticipate : 0); } }
    //if shield get shield block rate, if not shield but weapon get weapon block rate
    public float Armor { get { return fighter.Armor + (activeWeapon != null ? activeWeapon.Armor : 0); } }
    public float Disarm { get { return fighter.Disarm + (activeWeapon != null ? activeWeapon.Disarm : 0); } }
    public float Precision { get { return fighter.Precision + (activeWeapon != null ? activeWeapon.Precision : 0); } }
    public float Accuracy { get { return fighter.Accuracy + (activeWeapon != null ? activeWeapon.Accuracy : 0); } }
    public float Block { get { return fighter.Block + (activeShield != null ? activeShield.BlockRate : (activeWeapon != null ? activeWeapon.Block : 0)); } }
    public float DamageMitigationPercent { get { return 1 - (activeShield != null ? activeShield.BlockPercent : (activeWeapon != null ? activeWeapon.Block_DamagePercent + emptyHandedBlockPercent : emptyHandedBlockPercent)); } }

    //ATTACK INFO
    public float AttackDistance { get => attackDistance; set => attackDistance = value; }
    public AttackType AttackType { get => attackType; set => attackType = value; }

    public virtual void ModifyHP(float variation)
    {
        hp += variation;
    }
    public void ModifyShieldHp(float variation)
    {
        hp += variation;
    }
    public bool CanKeepFighting()
    {
        if (DeadKeepFighting && Hp <= 0)
        {
            if (team == TeamNum.Team1)
            {
                CombatDynamicsInstance.Team2Ended = true;
                if (!CombatDynamicsInstance.Team1Ended)
                {
                    CombatDynamicsInstance.Team2Wins++;
                }
            }
            else
            {
                CombatDynamicsInstance.Team1Ended = true;
                if (!CombatDynamicsInstance.Team2Ended)
                {
                    CombatDynamicsInstance.Team1Wins++;
                }
            }

            return true;
        }

        return Hp > 0;
    }
    public void ConsiderAnticipateAttack(FighterCombat attacker)
    {
        if (Anticipate < RandomSingleton.NextDouble())
            return;

        if (team == TeamNum.Team1)
            CombatDynamicsInstance.dv.team1NumberAnticipate++;
        else
            CombatDynamicsInstance.dv.team2NumberAnticipate++;


        ConsiderDisarmOponent(attacker, DisarmInteraction.Forced);
        PrintWithColor(" ANTICIPATE ", "#B80000");
        Attack(attacker);
    }
    public void ConsiderCounterAttack(FighterCombat attacker)
    {
        if (Counterattack < RandomSingleton.NextDouble())
            return;

        if (team == TeamNum.Team1)
            CombatDynamicsInstance.dv.team1NumberCounterAttack++;
        else
            CombatDynamicsInstance.dv.team2NumberCounterAttack++;

        PrintWithColor(" COUNTER ", "#B80000");
        Attack(attacker);
    }
    public bool Evaded(FighterCombat attacker)
    {
        if (Evasion - attacker.Precision < RandomSingleton.NextDouble())
            return false;

        if (team == TeamNum.Team1)
            CombatDynamicsInstance.dv.team1NumberEvasion++;
        else
            CombatDynamicsInstance.dv.team2NumberEvasion++;

        PrintWithColor(" EVASION ", "#B80000");
        return true;
    }
    public bool Blocked(FighterCombat attacker)
    {
        if (Block - attacker.Accuracy < RandomSingleton.NextDouble())
            return false;

        if (team == TeamNum.Team1)
            CombatDynamicsInstance.dv.team1NumberBlock++;
        else
            CombatDynamicsInstance.dv.team2NumberBlock++;

        PrintWithColor(" BLOCKED ", "#B80000");
        return true;
    }
    protected void Attack(FighterCombat damagedFighter)
    {
        float damage = CalculateDamage(damagedFighter);

        damagedFighter.ModifyHP(-damage);

        CombatDynamicsInstance.StartCoroutine(CompleteAttack(damagedFighter, damage, AttackInteraction.Clean, 0.3f));
    }
    protected virtual void BlockedAttack(FighterCombat damagedTarget)
    {
        float damage = CalculateDamage(damagedTarget);

        damage -= BlockFlatDamage;
        float damageMitigated = damage * damagedTarget.DamageMitigationPercent;

        if(ShieldHp - damageMitigated < 0)
        {
            damageMitigated = shieldHp;
        }

        damage -= damageMitigated;

        damagedTarget.ModifyHP(-damage);
        damagedTarget.ModifyShieldHp(-damageMitigated);

        CombatDynamicsInstance.StartCoroutine(CompleteAttack(damagedTarget, damage, AttackInteraction.Blocked, 0.3f));
    }
    protected void EvadedAttack(FighterCombat target)
    {
        CombatDynamicsInstance.StartCoroutine(CompleteAttack(target, 0, AttackInteraction.Dodged, 0.3f));
    }
    protected float CalculateDamage(FighterCombat damagedFighter)
    {
        float damage = 10;

        if (ActiveWeapon != null)
        {
            damage += Fighter.Strength * ActiveWeapon.StrengthRatio;
            damage *= damagedFighter.Armor;
            return damage;
        }
        else
        {
            damage += Fighter.Strength * strength_noWeaponDamage;
            damage *= damagedFighter.Armor;
            return damage;
        }
    }
    public virtual IEnumerator CompleteAttack(FighterCombat target, float damage, AttackInteraction attackInteraction, float targetInteractionDelay)
    {
        PrintWithColor(this.Fighter.FighterName + " dealt " + damage + " to " + target.Fighter.FighterName, TeamColor(target.Team));
        yield return null;
    }
    public virtual void MoveCharacterToSpawn()
    {

    }
    public virtual void MoveCharacterToAttack(float attackDistance, FighterCombat target)
    {

    }
    public void ConsiderDisarmOponent(FighterCombat target, DisarmInteraction disarmInteraction)
    {
        if (target.ActiveWeapon == null)
            return;

        if (Disarm > RandomSingleton.NextDouble())
        {
            target.ActiveWeapon = null;
            target.AttackType = AttackType.Melee;
            target.AttackDistance = defaultAttackDistance;

            target.CompleteGetDisarmed(disarmInteraction);
        }
    }

    public virtual void CompleteGetDisarmed(DisarmInteraction disarmInteraction)
    {
        PrintWithColor(this.Fighter.FighterName + " got disarmed", "#B80000");
    }
    protected void YieldWeapon()
    {
        float chances = ActiveWeapon == null ? 0.2f : 0.66f;

        if (Fighter.Weapons.Count > 0 && chances < RandomSingleton.NextDouble())
        {
            if(team==TeamNum.Team1)
                CombatDynamicsInstance.dv.team1NumberYieldWeapon++;
            else
                CombatDynamicsInstance.dv.team2NumberYieldWeapon++;

            int index = (int)(RandomSingleton.NextDouble() * (Fighter.Weapons.Count - 1));
            activeWeapon = Fighter.Weapons[index];
            Fighter.Weapons.RemoveAt(index);
            AttackType = activeWeapon.WeaponType;
            AttackDistance = activeWeapon.AttackDistance;
            CompleteYieldWeapon();
        }
    }
    protected virtual void CompleteYieldWeapon()
    {
        PrintWithColor(Fighter.FighterName + " yield a weapon", "#DAFF1E");
    }
    public virtual IEnumerator NextMove(FighterCombat target)
    {
        if (Fighter.Weapons != null)
        {
            YieldWeapon();
            yield return CombatDynamicsInstance.StartCoroutine(WaitUntilActionsEnded(target));
        }

        if (attackType == AttackType.Melee)
        {
            MoveCharacterToAttack(attackDistance, target);
            yield return CombatDynamicsInstance.StartCoroutine(WaitUntilActionsEnded(target));
            target.ConsiderAnticipateAttack(this);
            yield return CombatDynamicsInstance.StartCoroutine(WaitUntilActionsEnded(target));
        }


        do //consecutive attacks
        {
            if (team == TeamNum.Team1)
                CombatDynamicsInstance.dv.team1NumberAttacks++;
            else
                CombatDynamicsInstance.dv.team2NumberAttacks++;

            if (attackType == AttackType.Melee)
            {
                MoveCharacterToAttack(attackDistance, target);
                yield return CombatDynamicsInstance.StartCoroutine(WaitUntilActionsEnded(target));
            }

            if (target.CanKeepFighting() && target.Evaded(this))
            {
                EvadedAttack(target);
                yield return CombatDynamicsInstance.StartCoroutine(WaitUntilActionsEnded(target));
            }
            else if (target.CanKeepFighting() && target.Blocked(this))
            {
                ConsiderDisarmOponent(target, DisarmInteraction.Forced);
                BlockedAttack(target);
                yield return CombatDynamicsInstance.StartCoroutine(WaitUntilActionsEnded(target));
            }
            else
            {
                ConsiderDisarmOponent(target, DisarmInteraction.Forced);
                Attack(target);
                yield return CombatDynamicsInstance.StartCoroutine(WaitUntilActionsEnded(target));
            }

            if (target.CanKeepFighting() && attackType == AttackType.Melee)
                target.ConsiderCounterAttack(this);

            yield return CombatDynamicsInstance.StartCoroutine(WaitUntilActionsEnded(target));

        } while (CanKeepFighting() && MultiHit > RandomSingleton.NextDouble());

        MoveCharacterToSpawn();
        yield return CombatDynamicsInstance.StartCoroutine(WaitUntilActionsEnded(target));
    }

    protected virtual IEnumerator WaitUntilActionsEnded(FighterCombat target)
    {
        yield return null;
    }
}
