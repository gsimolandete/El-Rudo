using System.Collections;
using System.Collections.Generic;
using static GlobalVariables;

public class PetStats : Fighter
{
    public readonly DerivatedStats InitialDerivatedStats;
    public readonly string pathToAddressable;

    public PetStats(string _name, float _vitality, float _strength, float _velocity, float _agility, List<Weapon> _weapons, Shield _shield, DerivatedStats initialDerivatedStats, string pathToAddressable) : base(_name, _vitality, _strength, _velocity, _agility, _weapons, _shield)
    {
        InitialDerivatedStats = initialDerivatedStats;
        this.pathToAddressable = pathToAddressable;
    }
}
