using System.Collections;
using System.Collections.Generic;
using static GlobalVariables;

[System.Serializable]
public abstract class Fighter
{
    protected string fighterName;
    protected BasicStats basicStats;
    protected DerivatedStats derivatedStats;

    protected Shield shield;
    protected List<Weapon> weapons;

    protected Fighter(string _name, float _vitality, float _strength, float _velocity, float _agility, List<Weapon> _weapons, Shield _shield)
    {
        fighterName = _name;
        basicStats = new BasicStats
        {
            vitality = _vitality,
            strength = _strength,
            agility = _agility,
            velocity = _velocity
        };
        weapons = _weapons;
        shield = _shield;
    }

    //PRINCIPALS
    public virtual float Strength { get => basicStats.strength; }
    public virtual float Agility { get => basicStats.agility; }
    public virtual float Velocity { get => basicStats.velocity; }
    public virtual float Vitality { get => basicStats.vitality; }

    //DERIVATEDS
    public List<Weapon> Weapons { get => weapons; }
    public float Initiative { get { return derivatedStats.initiative; } }
    public float MultiHit { get { return derivatedStats.multiHit; } }
    public float Counterattack { get { return derivatedStats.counterattack; } }
    public float Evasion { get { return derivatedStats.evasion; } }
    public float Anticipate { get { return derivatedStats.anticipate; } }
    //if shield get shield block rate, if not shield but weapon get weapon block rate
    public float Block { get { return derivatedStats.block; } } 
    public float Armor { get { return derivatedStats.armor; } }
    public float Disarm { get { return derivatedStats.disarm; } }
    public float Precision { get { return derivatedStats.precision; } }
    public float Accuracy { get { return derivatedStats.accuracy; } }
    public string FighterName { get => fighterName; set => fighterName = value; }
    public Shield Shield { get => shield; set => shield = value; }

    
}
