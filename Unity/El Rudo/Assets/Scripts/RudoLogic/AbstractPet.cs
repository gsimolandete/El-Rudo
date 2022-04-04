using System.Collections;
using System.Collections.Generic;

[System.Serializable]
public class AbstractPet : Fighter {
    public AbstractPet(string name, float vitality, float strength, float velocity, float agility, List<Weapon> weapons) : base(name, vitality, strength, velocity, agility, weapons,null)
    { }
}
