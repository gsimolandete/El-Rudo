using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static FighterCombat;
using static GlobalVariables;

[System.Serializable]
public class Rudo : Fighter
{
    [SerializeField]
    int level;


    public Rudo(string name, int lvl, float vitality, float strength, float velocity, float agility, List<Weapon> weapons, List<AbstractPet> abstractPets) : base(name, vitality, strength, velocity, agility, weapons, abstractPets)
    {
        this.level = lvl;
    }


    public int Level { get => level;}

    public FighterTeamList GetTeam(TeamNum team)
    {
        FighterTeamList teamList = new FighterTeamList();
        List<FighterCombat> fighterCombats = new List<FighterCombat>();

        teamList.Rudo = new RudoCombatLog(new Rudo(fighterName,level,vitality,strength,velocity,agility,new List<Weapon>(weapons),abstractPets), team);

        foreach (var item in abstractPets)
        {
            throw new System.Exception("Not implemented");
        }

        teamList.Pets = fighterCombats;

        return teamList;
    }
}
