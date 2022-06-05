using System;
using System.Collections;
using System.Collections.Generic;
using static FighterCombat;
using static GlobalVariables;

[System.Serializable]
public class Rudo : Fighter
{
    int nftId, experience;
    int level;
    protected Pet pet;
    public List<int> skills;

    public readonly ActiveSkills ActiveSkills;


    public Rudo(int nftId, int experience, string name, int lvl, float vitality, float strength, float velocity, float agility, List<Weapon> weapons, Pet pet, Shield shield, List<int> skills) : base(name, vitality, strength, velocity, agility, weapons, shield)
    {
        this.experience = experience;
        this.nftId = nftId;
        this.level = lvl;
        this.pet = pet;
        this.ActiveSkills = new ActiveSkills();
        this.skills = skills == null ? new List<int>() : skills;

        derivatedStats.counterattack = defaultCounterattack + basicStats.agility * agility_CounterAttack;
        derivatedStats.evasion = defaultEvasion + basicStats.agility * agility_Evasion;
        derivatedStats.multiHit = defaultMultiHit + basicStats.velocity * agility_Multihit;
        derivatedStats.initiative = defaultInitiative + basicStats.velocity * velocity_Initiative;
        derivatedStats.anticipate = defaultAnticipate + basicStats.velocity * velocity_Anticipation;
        derivatedStats.block = defaultBlock;
        derivatedStats.armor = defaultArmor;
        derivatedStats.disarm = defaultDisarm;
        derivatedStats.precision = defaultPrecision;
        derivatedStats.accuracy = defaultAccuracy;

        for (int i = 0; i < this.skills.Count; i++)
        {
            switch (SkillsArray.GetInstance(skills[i]))
            {
                case SkillsPassiveStats sps:
                    derivatedStats.counterattack += sps.DerivatedStats.counterattack;
                    derivatedStats.evasion += sps.DerivatedStats.evasion;
                    derivatedStats.multiHit += sps.DerivatedStats.multiHit;
                    derivatedStats.initiative += sps.DerivatedStats.initiative;
                    derivatedStats.anticipate += sps.DerivatedStats.anticipate;
                    derivatedStats.block += sps.DerivatedStats.block;
                    derivatedStats.armor += sps.DerivatedStats.armor;
                    derivatedStats.disarm += sps.DerivatedStats.disarm;
                    derivatedStats.precision += sps.DerivatedStats.precision;
                    derivatedStats.accuracy += sps.DerivatedStats.accuracy;
                    break;
                case SkillsActiveRudoStats<DefensiveBlockSkillTrigger> sas:
                    ActiveSkills.BlockSkillTriggers.Add(this.skills[i], sas);
                    break;
                default:
                    throw new Exception("not handled skill trigger moment");
            }
        }
    }


    public int NftId { get => nftId; }
    public int Level { get => level; }
    public Pet Pet { get => pet; set => pet = value; }
    public int Experience { get => experience; }

    public FighterTeamList GetTeam(TeamNum team)
    {

        FighterCombat rudo = new FighterCombat(new Rudo(nftId, experience, fighterName, level, Vitality, Strength, Velocity, Agility, new List<Weapon>(weapons), Pet, shield, skills), team);
        FighterCombat pet = Pet != null ? new FighterCombat(new Pet(Pet.Equipable.nftId, Pet.Equipable.equipableId, Pet.Equipable.quality), team) : null;

        FighterTeamList teamList = new FighterTeamList(rudo,pet);

        return teamList;
    }
    public FighterTeamList GetTeamVisual(TeamNum team)
    {

        FighterCombat rudo = new FighterCombat(new Rudo(nftId, experience, fighterName, level, Vitality, Strength, Velocity, Agility, new List<Weapon>(weapons), Pet, shield, skills), team);
        FighterCombat pet = Pet != null ? new FighterCombat(new Pet(Pet.Equipable.nftId, Pet.Equipable.equipableId, Pet.Equipable.quality), team) : null;

        FighterTeamList teamList = new FighterTeamList(rudo, pet);

        return teamList;
    }
}
