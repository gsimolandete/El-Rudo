using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using static FighterCombat;
using static GlobalVariables;

[System.Serializable]
[CreateAssetMenu(fileName = "RudoLog", menuName = "ScriptableObjects/Rudo", order = 2)]
public class Rudo : Fighter
{
    int nftId, experience;
    [SerializeField]
    int level;
    [SerializeField]
    protected Pet pet;


    public Rudo(int nftId, int experience, string name, int lvl, float vitality, float strength, float velocity, float agility, List<Weapon> weapons, Pet pet, Shield shield) : base(name, vitality, strength, velocity, agility, weapons, shield)
    {
        this.experience = experience;
        this.nftId = nftId;
        this.level = lvl;
        this.pet = pet;
    }


    public int NftId { get => nftId; }
    public int Level { get => level; }
    public Pet Pet { get => pet; }
    public int Experience { get => experience; }

    public FighterTeamList GetTeam(TeamNum team)
    {
        FighterTeamList teamList = new FighterTeamList();
        List<FighterCombat> fighterCombats = new List<FighterCombat>();

        teamList.Rudo = new FighterCombat(new Rudo(nftId, experience, fighterName, level, Vitality, Strength, Velocity, Agility, new List<Weapon>(weapons), pet, shield), team);

        teamList.Pets = fighterCombats;

        return teamList;
    }
    public FighterTeamList GetTeamVisual(TeamNum team, Slider hpSlider)
    {
        FighterTeamList teamList = new FighterTeamList();
        List<FighterCombat> fighterCombats = new List<FighterCombat>();

        teamList.Rudo = new RudoCombatVisual(new Rudo(nftId, experience, fighterName, level, Vitality, Strength, Velocity, Agility, new List<Weapon>(weapons), pet, shield), team, hpSlider);

        teamList.Pets = fighterCombats;

        return teamList;
    }
}
