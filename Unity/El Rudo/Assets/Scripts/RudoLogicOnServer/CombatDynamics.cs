using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using UnityEngine;
using System;
using static FighterCombat;
using static ServerDifferentFunctions;
using static GlobalVariables;
using UnityEngine.AddressableAssets;
using System.Threading.Tasks;

public class CombatDynamics : MonoBehaviour
{
    public bool randomSeed, showLogs;
    public int seed;
    public static Rudo rudo1, rudo2;

    protected bool team1ended = false, team2ended = false;
    protected float team1ExcessHealth = 0, team2ExcessHealth = 0;
    protected int team1Wins = 0, team2Wins = 0, combatTurns = 0;

    public bool Team1Ended { get => team1ended; set => team1ended = value; }
    public bool Team2Ended { get => team2ended; set => team2ended = value; }
    public int Team1Wins { get => team1Wins; set => team1Wins = value; }
    public int Team2Wins { get => team2Wins; set => team2Wins = value; }

    protected FighterTeamList[] teamFighterList;

    public DebugVariables dv;

    [System.Serializable]
    public class DebugVariables
    {
        public int
           team1NumberTurns, team2NumberTurns, team1NumberAnticipate, team2NumberAnticipate, team1NumberCounterAttack, team2NumberCounterAttack, team1NumberEvasion, team2NumberEvasion, team1NumberBlock,
           team2NumberBlock, team1NumberYieldWeapon, team2NumberYieldWeapon, team1NumberAttacks, team2NumberAttacks;
        public DebugVariables()
        {
            team1NumberTurns = 0; team2NumberTurns = 0;
            team1NumberTurns = 0; team2NumberTurns = 0; team1NumberAnticipate = 0; team2NumberAnticipate = 0; team1NumberCounterAttack = 0; team2NumberCounterAttack = 0; team1NumberEvasion = 0;
            team2NumberEvasion = 0; team1NumberBlock = 0; team2NumberBlock = 0; team1NumberYieldWeapon = 0; team2NumberYieldWeapon = 0; team1NumberAttacks = 0; team2NumberAttacks = 0;
        }
    }

    public CombatDynamics(bool randomSeed, Rudo abstractRudo1, Rudo abstractRudo2, int seed = 0)
    {
        this.randomSeed = randomSeed;
        this.seed = seed;
        CombatDynamics.rudo1 = abstractRudo1;
        CombatDynamics.rudo2 = abstractRudo2;
    }

    protected virtual void InitializeScript()
    {
        CombatDynamicsInstance = this;
        ShowLogs = showLogs;
        DeadKeepFighting = false;
    }

    protected async virtual Task PrepareFight()
    {
        if(randomSeed)
            seed = (int)(RandomSingleton.NextDouble() * int.MaxValue);

        RandomSingleton.Instance.Random = new System.Random(seed);

        teamFighterList = new FighterTeamList[2];

        GetTeams();
    }

    protected virtual void GetTeams()
    {
        teamFighterList[0] = rudo1.GetTeam(TeamNum.Team1);
        teamFighterList[1] = rudo2.GetTeam(TeamNum.Team2);
    }

    public virtual void StartCombat()
    {
        InitializeScript();
        StartCoroutine(Fight());
    }

    protected virtual IEnumerator Fight()
    {
        Task task = PrepareFight();
        yield return new WaitUntil(() => { return task.IsCompleted; }); 

        while (!CombatEnded())
        {
            FighterCombat attacker = FindNextAttacker();
            FighterCombat target = FindNextTarget(attacker);
            PrintWithColor("###### " + attacker.Fighter.FighterName + " attacks ######", "#46FF00");
            yield return StartCoroutine(attacker.NextMove(target));
        }

        yield return null;
    }

    virtual protected FighterCombat FindNextAttacker()
    {
        float closestTimeToAttack = 100000;
        List<FighterCombat> nextAttackers = new List<FighterCombat>();

        for (int i = 0; i < teamFighterList.Length; i++)
        {
            for (int ii = 0; ii < teamFighterList[i].Count; ii++)
            {
                float a = TimeToAttack(teamFighterList[i][ii]);
                if (closestTimeToAttack >= a)
                {
                    //SAME OPTIMIZE
                    if (Math.Abs(a - closestTimeToAttack) < float.Epsilon) //comprovamos que no sean el mismo valor, si lo son añadimos y no borramos
                    {
                        nextAttackers.Add(teamFighterList[i][ii]);
                    }
                    else //no son el mismo valor
                    {
                        nextAttackers.Clear();
                        nextAttackers.Add(teamFighterList[i][ii]);
                        closestTimeToAttack = a;
                    }
                }
            }
        }
        FighterCombat fighterCombat = nextAttackers[(int)(RandomSingleton.NextDouble() * nextAttackers.Count)];

        if (!(closestTimeToAttack < 0)) //puede ser negativo closesttime, por la iniciativa muy grande, asi que si es negativo no tenemos que tocar nada
        {
            //aumentar medidor
            for (int i = 0; i < teamFighterList.Length; i++)
            {
                for (int ii = 0; ii < teamFighterList[i].Count; ii++)
                {
                    if (fighterCombat == teamFighterList[i][ii])
                        teamFighterList[i][ii].TurnMeter = 0;
                    else
                        teamFighterList[i][ii].TurnMeter += MeterIncrement(teamFighterList[i][ii], closestTimeToAttack);
                }
            }
        }
        return fighterCombat;
    }

    FighterCombat FindNextTarget(FighterCombat attacker)
    {
        if (attacker.Team == TeamNum.Team2)
        {
            return teamFighterList[0][(int)(RandomSingleton.NextDouble() * (teamFighterList[0].Count - 1))];
        }
        else
        {
            return teamFighterList[1][(int)(RandomSingleton.NextDouble()*(teamFighterList[1].Count - 1))];
        }
    }

    protected virtual bool CombatEnded()
    {
        if (teamFighterList[0].Rudo.Hp <= 0)
        {
            PrintWithColor(teamFighterList[1].Rudo.Fighter.FighterName + " won", "#FFFFFF");
            OnCombatEnded();
            return true;
        }
        else if (teamFighterList[1].Rudo.Hp <= 0)
        {
            PrintWithColor(teamFighterList[0].Rudo.Fighter.FighterName + " won", "#FFFFFF");
            OnCombatEnded();
            return true;
        }
        return false;
    }

    protected virtual void OnCombatEnded()
    {

    }
}
