using System.Collections;
using System.Collections.Generic;
using static GlobalVariables;

public abstract class Fighter 
{
    protected float strength, agility, velocity, vitality;
    private float initiative, multiHit, counterattack, evasion, anticipate, block, armor, disarm, precision, accuracy;
    protected string fighterName;
    private float attackDistance;
    private AttackType attackType = AttackType.Melee;

    protected Fighter()
    {
        Initialize();
    }

    public void Initialize(float initiative = 0, float multiHit = 15, float counterattack = 15,float evasion = 10,float anticipate = 5,float block = 20, float armor=0.5f,float disarm = 0,float precision = 0,float accuracy = 0)
    {
        this.initiative = initiative; this.multiHit = multiHit; this.counterattack = counterattack; this.evasion = evasion; this.anticipate = anticipate;
        this.block = block; this.armor = armor; this.disarm = disarm; this.precision = precision; this.accuracy = accuracy;
    }

    //PRINCIPALS
    public virtual float Strength { get => strength; }
    public virtual float Agility { get => agility; }
    public virtual float Velocity { get => velocity; }
    public virtual float Vitality { get => vitality; }

    //DERIVATEDS
    public virtual float Initiative { get => initiative + velocity * velocity_Initiative;}
    public virtual float MultiHit { get => multiHit + agility * agility_Multihit;}
    public virtual float Counterattack { get => counterattack + agility * agility_CounterAttack; }
    public virtual float Evasion { get => evasion + agility * agility_Evasion;}
    public virtual float Anticipate { get => anticipate + velocity * velocity_Anticipation;}
    public virtual float Block { get => block;}
    public virtual float Armor { get => armor;}
    public virtual float Disarm { get => disarm;}
    public virtual float Precision { get => precision + agility * agility_Evasion; }
    public virtual float Accuracy { get => accuracy;}

    //ATTACK INFO
    protected float AttackDistance { get => attackDistance; set => attackDistance = value; }
    protected AttackType AttackType { get => attackType; set => attackType = value; }
    public string FighterName { get => fighterName; set => fighterName = value; }

    public void ConsiderAnticipateAttack(FighterCombat target)
    {
        if (Anticipate <= RandomSingleton.NextDouble()*100f)
            return;

        PrintWithColor("#B80000", " ANTICIPATE ");
        Attack(target);
    }
    public void ConsiderCounterAttack(FighterCombat target)
    {
        if (Counterattack <= RandomSingleton.NextDouble()*100f)
            return;

        PrintWithColor("#B80000", " COUNTER ");
        Attack(target);
    }
    public bool Evaded(FighterCombat attacker, FighterCombat target)
    {
        if (Evasion - attacker.Fighter.Precision <= RandomSingleton.NextDouble() * 100f)
            return false;

        PrintWithColor("#B80000", " EVASION ");
        return true;
    }
    public bool Blocked(FighterCombat attacker, FighterCombat target)
    {
        if (Block - attacker.Fighter.Accuracy <= RandomSingleton.NextDouble() * 100f)
            return false;

        PrintWithColor("#B80000", " BLOCKED ");
        return true;
    }
    protected void Attack(FighterCombat target)
    {
        float damage = CalculateDamage();

        target.ModifyHP(-damage);

        CompleteAttack(target, damage);
    }
    protected virtual void BlockedAttack(FighterCombat attacker, FighterCombat target)
    {
        float damage = CalculateDamage();
        damage -= emptyHandedBlockedDamage + strength_Block * Strength;

        target.ModifyHP(-damage);

        attacker.Fighter.CompleteAttack(target, damage);
    }
    protected void EvadedAttack(FighterCombat attacker, FighterCombat target)
    {
        attacker.Fighter.CompleteAttack(target, 0);
    }
    protected abstract float CalculateDamage();
    public abstract void CompleteAttack(FighterCombat target, float damage);
    public abstract void MoveCharacter(float attackDistance, FighterCombat fighter);
    public abstract void GetDisarmed();
    public virtual void NextMove(FighterCombat attacker, FighterCombat target)
    {
        if (attackType == AttackType.Melee)
        {
            MoveCharacter(attackDistance, target);
            target.Fighter.ConsiderAnticipateAttack(attacker);
        }

        do //consecutive attacks
        {
            if (target.Fighter.Evaded(attacker, target))
            {
                EvadedAttack(attacker, target);
            }
            else if (target.Fighter.Blocked(attacker, target))
            {
                BlockedAttack(attacker, target);
            }
            else
            {
                Attack(target);
            }

            if(Disarm > RandomSingleton.NextDouble() * 100f)
                GetDisarmed();

            if (attackType==AttackType.Melee && target.Hp > 0)
                target.Fighter.ConsiderCounterAttack(attacker);

        } while (attacker.Hp > 0 && MultiHit > RandomSingleton.NextDouble() * 100f);
    }
}
