using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static GlobalVariables;

public class CombatDynamics_Balance : CombatDynamics
{
    public int combatIterations;

    public CombatDynamics_Balance(bool randomSeed, int seed, Rudo abstractRudo1, Rudo abstractRudo2) : base(randomSeed, abstractRudo1, abstractRudo2,seed)
    { }

    protected override void InitializeScript()
    {
        dv = new DebugVariables();
        team1ExcessHealth = 0; team2ExcessHealth = 0;
        team1Wins = 0; team2Wins = 0;
        base.InitializeScript();
        DeadKeepFighting = true;
    }
    protected override void PrepareFight()
    {
        team1ended = false;
        team2ended = false;
        combatTurns = 0;
        base.PrepareFight();
    }

    public override void StartCombat()
    {
        InitializeScript();
        for (int i = 0; i < combatIterations; i++)
        {
            base.Fight();
        }
        print("team1ExcessHealth: " + team1ExcessHealth + " team2ExcessHealth: " + team2ExcessHealth);
        print("team1Wins: " + team1Wins + " team2Wins: " + team2Wins);
        print("team1WinPercent: " + team1Wins*100f/(team1Wins + team2Wins)+"%" + "team1HealthDifference: " + team1ExcessHealth*100f/(team1ExcessHealth+team2ExcessHealth) + "%");
    }
    protected override bool CombatEnded()
    {
        combatTurns++;

        if (team1ended && team2ended) {
            team1ExcessHealth += teamFighterList[0].Rudo.Hp;
            team2ExcessHealth += teamFighterList[1].Rudo.Hp;
            return true; 
        }

        return false;
    }
}

