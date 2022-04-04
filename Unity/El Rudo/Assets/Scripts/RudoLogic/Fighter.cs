using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static GlobalVariables;

[System.Serializable]
public abstract class Fighter
{
    [SerializeField]
    protected string fighterName;
    [SerializeField]
    protected float vitality, strength, agility, velocity;
    protected float initiative, multiHit, counterattack, evasion,
                    anticipate, block, armor, disarm, precision, accuracy;
    [SerializeField]
    protected Shield shield;
    [SerializeField]
    protected List<Weapon> weapons;
    [SerializeField]
    protected List<AbstractPet> abstractPets;

    protected Fighter(string _name, float _vitality, float _strength, float _velocity, float _agility, List<Weapon> _weapons, List<AbstractPet> _abstractPets)
    {
        fighterName = _name;
        vitality = _vitality;
        strength = _strength;
        agility = _agility;
        velocity = _velocity;
        weapons = _weapons;
        abstractPets = _abstractPets;
        counterattack = defaultCounterattack + agility * agility_CounterAttack;
        evasion = defaultEvasion + agility * agility_Evasion;
        multiHit = defaultMultiHit + velocity * velocity_Multihit;
        initiative = defaultInitiative + velocity * velocity_Initiative;
        anticipate = defaultAnticipate + velocity * velocity_Anticipation;
        block = defaultBlock;
        armor = defaultArmor;
        disarm = defaultDisarm;
        precision = defaultPrecision;
        accuracy = defaultAccuracy;
    }

    //PRINCIPALS
    public virtual float Strength { get => strength; }
    public virtual float Agility { get => agility; }
    public virtual float Velocity { get => velocity; }
    public virtual float Vitality { get => vitality; }

    //DERIVATEDS
    public List<Weapon> Weapons { get => weapons; }
    public float Initiative { get { return initiative; } }
    public float MultiHit { get { return multiHit; } }
    public float Counterattack { get { return counterattack; } }
    public float Evasion { get { return evasion; } }
    public float Anticipate { get { return anticipate; } }
    //if shield get shield block rate, if not shield but weapon get weapon block rate
    public float Block { get { return block; } } 
    public float Armor { get { return armor; } }
    public float Disarm { get { return disarm; } }
    public float Precision { get { return precision; } }
    public float Accuracy { get { return accuracy; } }
    public string FighterName { get => fighterName; set => fighterName = value; }
    public Shield Shield { get => shield; }
    public List<AbstractPet> AbstractPets { get => abstractPets; }

    
}
