using System;
using System.Collections;
using System.Collections.Generic;
using static GlobalVariables;
using System.Linq;

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
    TeamNum team;
    Fighter fighter;
    float turnMeter;
    float hp, shieldHp, weaponSkillFillPoints;
    protected Weapon activeWeapon;
    protected Shield activeShield;
    protected AttackType attackType;
    protected float attackDistance;

    public EventHandler hpModifiedHandler;

    public FighterCombat(Fighter fighter, TeamNum team)
    {
        activeWeapon = null;

        this.fighter = fighter;
        turnMeter = fighter.Initiative;
        hp = MaxHP(fighter);
        weaponSkillFillPoints = 0;
        if (fighter.Shield != null)
        {
            activeShield = fighter.Shield;
            //shieldHp = fighter.Shield.shieldHealth;
            shieldHp = 1;
        }
        this.team = team;
        attackDistance = defaultAttackDistance;
    }

    public Fighter Fighter { get => fighter; set => fighter = value; }
    public float TurnMeter { get => turnMeter; set => turnMeter = value; }
    public float Hp { get => hp; }
    public float WeaponSkillFillPoints { get => weaponSkillFillPoints; }
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
    public float DisarmChance { get { return fighter.Disarm + (activeWeapon != null ? activeWeapon.Disarm : 0); } }
    public float Precision { get { return fighter.Precision + (activeWeapon != null ? activeWeapon.Precision : 0); } }
    public float Accuracy { get { return fighter.Accuracy + (activeWeapon != null ? activeWeapon.Accuracy : 0); } }
    public float BlockChance { get { return fighter.Block + (activeShield != null ? activeShield.blockRate : (activeWeapon != null ? activeWeapon.Block : 0)); } }
    public float DamageMitigationPercent { get { return activeShield != null ? activeShield.blockPercent : (activeWeapon != null ? activeWeapon.block_DamagePercent + emptyHandedBlockPercent : emptyHandedBlockPercent); } }

    //ATTACK INFO
    public float AttackDistance { get => attackDistance; set => attackDistance = value; }
    public AttackType AttackType { get => attackType; set => attackType = value; }

    public virtual void ModifyHP(float variation)
    {
        hp += variation;
    }
    public virtual void ModifyWeaponSkillPoints(float variation)
    {
        weaponSkillFillPoints += variation;
    }
    public virtual void ModifyShieldHp(float variation)
    {
        if (activeShield == null)
            return;

        shieldHp += variation; 
        
        if (ShieldHp <= 0)
        {
            activeShield = null;
        }
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
    public bool ConsiderAnticipateAttack(FighterCombat attacker)
    {
        if (Anticipate < RandomSingleton.NextDouble())
            return false;

        if (team == TeamNum.Team1)
            CombatDynamicsInstance.dv.team1NumberAnticipate++;
        else
            CombatDynamicsInstance.dv.team2NumberAnticipate++;

        PrintWithColor(" ANTICIPATE ", "#B80000");
        return true;
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
        BasicAttack(attacker, new AttackPropertiesEnum[1] { AttackPropertiesEnum.NoCounter});
    }
    public bool Evaded(FighterCombat attacker)
    {
        if (Evasion - attacker.Precision < RandomSingleton.NextDouble())
            return false;

        if (team == TeamNum.Team1)
            CombatDynamicsInstance.dv.team1NumberEvasion++;
        else
            CombatDynamicsInstance.dv.team2NumberEvasion++;

        return true;
    }
    public bool Blocked(FighterCombat attacker)
    {
        if (BlockChance - attacker.Accuracy < RandomSingleton.NextDouble())
            return false;

        if (team == TeamNum.Team1)
            CombatDynamicsInstance.dv.team1NumberBlock++;
        else
            CombatDynamicsInstance.dv.team2NumberBlock++;

        return true;
    }
    protected virtual void BasicAttack(FighterCombat target, AttackPropertiesEnum[] attackPropertiesEnums = null)
    {
        PrintWithColor(this.Fighter.FighterName + " attacks", TeamColor(target.Team));
        AttackProperties ap = new AttackProperties(CalculateBasicAttackDamage(target), this, attackPropertiesEnums);

        if(target.CanKeepFighting())
            target.ReceiveAttack(this, ap);
    }
    protected virtual void WeaponSkillAttack(FighterCombat target)
    {
        PrintWithColor(this.Fighter.FighterName + " uses weapon skill", TeamColor(target.Team));
        AttackProperties ap = new AttackProperties(CalculateDamage(target,ActiveWeapon.weaponSkill.skillTriggerTime.StrengthRatio), this, new AttackPropertiesEnum[2] { AttackPropertiesEnum.NoCounter, AttackPropertiesEnum.NoDodge});

        if (target.CanKeepFighting())
            target.ReceiveAttack(this, ap);
    }
    public virtual void GetHurt(float damage)
    {
        ModifyHP(-damage);
        PrintWithColor(this.Fighter.FighterName + " received " + damage + " damage " , TeamColor(Team));
    }
    public virtual DefenseInteraction Block(AttackProperties attackProperties)
    {
        PrintWithColor(this.Fighter.FighterName + " BLOCKED ", "#B80000");

        DefenseInteraction ai = DefenseInteraction.Blocked;
        float damagePercentMitigationByShield = DamageMitigationPercent;
        float naturalDamageMitigation = 0;
        if (fighter.GetType() == typeof(Rudo))
        {
            SkillsActiveStats<DefensiveBlockSkillTrigger> bst = (fighter as Rudo).ActiveSkills.GetBlockSkill();
            if (bst != null)
            {
                CompleteUseActiveSkill(bst);
                ai = DefenseInteraction.None;
                bst.DoInteractions(attackProperties.attacker, this);
                naturalDamageMitigation += bst.skillTriggerTime.BlockPercent;
            }
        }

        attackProperties.damage -= BlockFlatDamage;
        attackProperties.damage -= attackProperties.damage * naturalDamageMitigation;
        if (activeShield != null)
        {
            float damageMitigatedByShield = attackProperties.damage * damagePercentMitigationByShield;
            damageMitigatedByShield = Math.Max(damageMitigatedByShield, 0);

            if (ShieldHp - damageMitigatedByShield < 0)
            {
                damageMitigatedByShield = shieldHp;
            }
            attackProperties.damage -= damageMitigatedByShield;
            ModifyShieldHp(-damageMitigatedByShield);
        }

        return ai;
    }
    public virtual void Dodge(AttackProperties attackProperties)
    {
        attackProperties.damage = 0;
        PrintWithColor(this.Fighter.FighterName + " DODGED ", "#B80000");
    }
    protected float CalculateDamage(FighterCombat damagedFighter, float strengthRatio)
    {
        float damage = defaultDamage;

        damage += Fighter.Strength * strengthRatio;
        damage *= (1 - damagedFighter.Armor);
        return damage;
    }
    protected float CalculateBasicAttackDamage(FighterCombat damagedFighter)
    {
        if (ActiveWeapon != null)
        {
            return CalculateDamage(damagedFighter, ActiveWeapon.strengthRatio);
        }
        else
        {
            return CalculateDamage(damagedFighter, strength_noWeaponDamage);
        }
    }

    public virtual void MoveCharacterToSpawn(){}
    public virtual void MoveCharacterToAttack(float attackDistance, FighterCombat target){}
    public void ConsiderGetDisarmed(FighterCombat attacker, DisarmInteraction disarmInteraction)
    {
        if (ActiveWeapon == null)
            return;

        if (attacker.DisarmChance > RandomSingleton.NextDouble())
        {
            Disarm(disarmInteraction);
        }
    }
    public void Disarm(DisarmInteraction disarmInteraction)
    {
        if (activeWeapon == null)
            return;

        ActiveWeapon = null;
        AttackType = AttackType.Melee;
        AttackDistance = defaultAttackDistance;

        CompleteGetDisarmed(disarmInteraction);
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
            AttackType = activeWeapon.attackType;
            AttackDistance = activeWeapon.attackDistance + defaultAttackDistance;
            CompleteYieldWeapon();
        }
    }
    protected virtual void CompleteUseActiveSkill<T>(SkillsActiveStats<T> skillsActiveStats) where T : ASkillTriggerTime
    {
        PrintWithColor(this.Fighter.FighterName + " used skill: "+skillsActiveStats.name, "#31FF11");
    }
    protected virtual void CompleteYieldWeapon()
    {
        PrintWithColor(Fighter.FighterName + " yield weapon: "+ activeWeapon.name, "#DAFF1E");
    }
    IEnumerator ReceiveAttack(FighterCombat attacker, AttackProperties attackProperties)
    {
        if (!attackProperties.attackPropertiesEnums.Contains(AttackPropertiesEnum.NoDodge) && Evaded(attacker))
        {
            Dodge(attackProperties);
            yield break;
        }
        else if (Blocked(attacker))
        {
            Block(attackProperties);
        }

        if (attackProperties.damage <= 0)
            yield break;

        ConsiderGetDisarmed(attacker, DisarmInteraction.Forced);
        GetHurt(attackProperties.damage);

        if (!attackProperties.attackPropertiesEnums.Contains(AttackPropertiesEnum.NoCounter) && CanKeepFighting() && attackType == AttackType.Melee)
            ConsiderCounterAttack(attacker);
    }
    public virtual IEnumerator NextMove(FighterCombat target)
    {
        ModifyWeaponSkillPoints(weaponFillAmmountPerTurn);

        if (Fighter.Weapons != null)
        {
            YieldWeapon();
        }

        if (activeWeapon!= null && activeWeapon.weaponSkill.pointsToActivate <= WeaponSkillFillPoints)
        {
            SkillsActiveWeaponStats saws = activeWeapon.weaponSkill;
            ModifyWeaponSkillPoints(-saws.pointsToActivate);
            MoveCharacterToSpawn();
            WeaponSkillAttack(target);
            MoveCharacterToSpawn();
            yield break;
        }

        if (attackType == AttackType.Melee)
        {
            MoveCharacterToAttack(attackDistance, target);
            if (target.ConsiderAnticipateAttack(this))
            {
                target.BasicAttack(this, new AttackPropertiesEnum[1] { AttackPropertiesEnum.NoCounter});
            }
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
            }

            BasicAttack(target);


        } while (CanKeepFighting() && MultiHit > RandomSingleton.NextDouble());

        if(CanKeepFighting())
            MoveCharacterToSpawn();

    }

    protected virtual IEnumerator WaitUntilActionsEnded(FighterCombat target)
    {
        yield return null;
    }
}
