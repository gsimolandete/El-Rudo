using System.Collections;
using System.Collections.Generic;
using static FighterCombat;
using static GlobalVariables;

public abstract class AbstractRudo : Fighter
{
    List<Weapon> weapons;
    Weapon activeWeapon;
    List<AbstractPet> abstractPets;
    int level;


    public AbstractRudo(string name, int lvl, float vitality, float strength, float velocity, float agility, List<Weapon> weapons, List<AbstractPet> abstractPets) : base()
    {
        Initialize(name,lvl,vitality,strength,velocity,agility,weapons,abstractPets);
    }
    //necesitamos que para iniciar el scriptable object se llame esto, no se puede llamar el constructor
    public void Initialize(string name, int lvl, float vitality, float strength, float velocity, float agility, List<Weapon> weapons, List<AbstractPet> abstractPets)
    {
        this.fighterName = name;
        this.level = lvl;
        this.vitality = vitality;
        this.strength = strength;
        this.velocity = velocity;
        this.agility = agility;
        this.activeWeapon = null;
        this.weapons = weapons;
        this.abstractPets = abstractPets;
        base.Initialize();
    }

    public List<Weapon> Weapons { get => weapons; set => weapons = value; }
    public Weapon ActiveWeapon { get => activeWeapon; set => activeWeapon = value; }
    public override float Initiative { get { return base.Initiative + (activeWeapon != null ? activeWeapon.Initiative : 0); } }
    public override float MultiHit { get { return base.MultiHit + (activeWeapon != null ? activeWeapon.MultiHit : 0); } }
    public override float Counterattack { get { return base.Counterattack + (activeWeapon != null ? activeWeapon.Counterattack : 0); } }
    public override float Evasion { get { return base.Evasion + (activeWeapon != null ? activeWeapon.Evasion : 0); } }
    public override float Anticipate { get { return base.Anticipate + (activeWeapon != null ? activeWeapon.Anticipate : 0); } }
    public override float Block { get { return base.Block + (activeWeapon != null ? activeWeapon.Block : 0); } }
    public override float Armor { get { return base.Armor + (activeWeapon != null ? activeWeapon.Armor : 0); } }
    public override float Disarm { get { return base.Disarm + (activeWeapon != null ? activeWeapon.Disarm : 0); } }
    public override float Precision { get { return base.Precision + (activeWeapon != null ? activeWeapon.Precision : 0); } }
    public override float Accuracy { get { return base.Accuracy + (activeWeapon != null ? activeWeapon.Accuracy : 0); } }
    public int Level { get => level;}

    public FighterTeamList GetTeam(TeamNum team)
    {
        FighterTeamList teamList = new FighterTeamList();
        List<FighterCombat> fighterCombats = new List<FighterCombat>();

        teamList.Rudo = new FighterCombat(this, team);

        foreach (var item in abstractPets)
        {
            fighterCombats.Add(new FighterCombat(item, team));
        }

        teamList.Pets = fighterCombats;

        return teamList;
    }

    public override void NextMove(FighterCombat attacker, FighterCombat target)
    {
        YieldWeapon();

        base.NextMove(attacker, target);
    }

    public override void GetDisarmed()
    {
        activeWeapon = null;
        AttackType = defaultAttackType;
        AttackDistance = defaultAttackRange;

        if (ActiveWeapon != null)
            CompleteGetDisarmed();
    }

    public abstract void CompleteGetDisarmed();

    void YieldWeapon()
    {
        float chances = activeWeapon==null ? 0.5f : 0.9f;

        if (weapons.Count > 0 && chances < RandomSingleton.NextDouble())
        {
            int index = (int)(RandomSingleton.NextDouble()*(weapons.Count - 1));
            activeWeapon = weapons[index];
            weapons.RemoveAt(index);
            AttackType = activeWeapon.WeaponType;
            AttackDistance = activeWeapon.AttackDistance;
            CompleteYieldWeapon();
        }
    }

    public abstract void CompleteYieldWeapon();
    protected override float CalculateDamage()
    {
        float damage = 10;

        if (activeWeapon != null)
        {
            damage += Strength * activeWeapon.StrengthRatio;
            damage = damage * Armor;
            return damage;
        }
        else
        {
            damage += Strength * strength_Damage;
            damage = damage * Armor;
            return damage;
        }
    }
    protected override void BlockedAttack(FighterCombat attacker, FighterCombat target)
    {
        float damage = CalculateDamage();
        damage -= emptyHandedBlockedDamage + (activeWeapon != null ? activeWeapon.Block_StrengthRatio * Strength : strength_Block * Strength);

        target.ModifyHP(-damage);

        attacker.Fighter.CompleteAttack(target, damage);
    }
}
