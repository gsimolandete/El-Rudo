using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static GlobalVariables;

public abstract class FighterCombat
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

    TeamNum team;
    Fighter fighter;
    float turnMeter;
    float hp, shieldHp;
    protected Weapon activeWeapon;
    protected Shield activeShield;
    protected AttackType attackType;
    protected float attackDistance;

    public FighterCombat(Fighter fighter, TeamNum team)
    {
        activeWeapon = null;

        this.fighter = fighter;
        turnMeter = fighter.Initiative;
        hp = GlobalVariables.MaxHP(fighter);
        if(fighter.Shield!=null)
            shieldHp = fighter.Shield.ShieldHealth;
        this.team = team;
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

    public void ModifyHP(float variation)
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
                    CombatDynamicsInstance.Team2Wins++;
            }
            else
            {
                CombatDynamicsInstance.Team1Ended = true;
                if (!CombatDynamicsInstance.Team2Ended)
                    CombatDynamicsInstance.Team1Wins++;
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

        CompleteAttack(damagedFighter, damage);
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

        CompleteAttack(damagedTarget, damage);
    }
    protected void EvadedAttack(FighterCombat target)
    {
        CompleteAttack(target, 0);
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
    public abstract void CompleteAttack(FighterCombat target, float damage);
    public abstract void MoveCharacter(float attackDistance, FighterCombat fighter);
    public void GetDisarmed()
    {
        if (ActiveWeapon == null)
            return;

        ActiveWeapon = null;
        AttackType = AttackType.Melee;
        AttackDistance = defaultAttackDistance;

        CompleteGetDisarmed();
    }

    public abstract void CompleteGetDisarmed();
    protected void YieldWeapon()
    {
        float chances = ActiveWeapon == null ? 0.2f : 0.9f;

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
    protected abstract void CompleteYieldWeapon();
    public virtual void NextMove(FighterCombat target)
    {
        if (Fighter.Weapons != null)
        {
            YieldWeapon();
        }

        if (attackType == AttackType.Melee)
        {
            MoveCharacter(attackDistance, target);
            target.ConsiderAnticipateAttack(this);
        }


        do //consecutive attacks
        {
            if (team == TeamNum.Team1)
                CombatDynamicsInstance.dv.team1NumberAttacks++;
            else
                CombatDynamicsInstance.dv.team2NumberAttacks++;

            if (target.CanKeepFighting() && target.Evaded(this))
            {
                EvadedAttack(target);
            }
            else if (target.CanKeepFighting() && target.Blocked(this))
            {
                BlockedAttack(target);
            }
            else
            {
                Attack(target);
            }

            //if(Disarm > RandomSingleton.NextDouble())
            //    GetDisarmed();

            if (target.CanKeepFighting() && attackType == AttackType.Melee)
                target.ConsiderCounterAttack(this);

        } while (CanKeepFighting() && MultiHit > RandomSingleton.NextDouble());
    }
}
