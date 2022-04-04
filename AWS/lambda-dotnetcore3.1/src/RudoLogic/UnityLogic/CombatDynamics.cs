using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System;
using static FighterCombat;
using static ServerDifferentFunctions;

public class CombatDynamics
{
    public bool randomSeed;
    public int seed;
    public AbstractRudo abstractRudo1;
    public AbstractRudo abstractRudo2;

    FighterTeamList[] teamFighterList;

    public CombatDynamics(int seed, AbstractRudo abstractRudo1, AbstractRudo abstractRudo2)
    {
        randomSeed = false;
        this.seed = seed;
        this.abstractRudo1 = abstractRudo1;
        this.abstractRudo2 = abstractRudo2;
    }

    public void Initialize()
    {
        if(randomSeed)
            seed = (int)(RandomSingleton.NextDouble() * int.MaxValue);

        RandomSingleton.Instance.Random = new System.Random(seed);

        teamFighterList = new FighterTeamList[2];

        teamFighterList[0] = abstractRudo1.GetTeam(TeamNum.Team1);
        teamFighterList[1] = abstractRudo2.GetTeam(TeamNum.Team2);
    }

    public void StartCombat()
    {
        Stopwatch  stopwatch = Stopwatch.StartNew();
        stopwatch.Start();

        print2("seed:" + seed);

        Initialize();
        while (!RudoDefeated())
        {
            FighterCombat attacker = FindNextAttacker();
            FighterCombat target = FindNextTarget(attacker);
            GlobalVariables.PrintWithColor("#46FF00", "###### " + attacker.Fighter.FighterName + " attacks ######");
            attacker.Fighter.NextMove(attacker, target);
        }

        stopwatch.Stop();
        print2(stopwatch.ElapsedMilliseconds);
    }

    FighterCombat FindNextAttacker()
    {
        float closestTimeToAttack = 100000;
        List<FighterCombat> nextAttackers = new List<FighterCombat>();

        for (int i = 0; i < teamFighterList.Length; i++)
        {
            for (int ii = 0; ii < teamFighterList[i].Count; ii++)
            {
                float a = GlobalVariables.TimeToAttack(teamFighterList[i][ii]);
                if (closestTimeToAttack >= a)
                {
                    //SAME OPTIMIZE
                    if (Math.Abs(a - closestTimeToAttack) < float.Epsilon) //comprovamos que no sean el mismo valor, si lo son a�adimos y no borramos
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
                        teamFighterList[i][ii].TurnMeter += GlobalVariables.MeterIncrement(teamFighterList[i][ii], closestTimeToAttack);
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

    bool RudoDefeated()
    {
        if (teamFighterList[0].Rudo.Hp <= 0)
        {
            GlobalVariables.PrintWithColor("#FFFFFF", teamFighterList[1].Rudo.Fighter.FighterName + " won");
            return true;
        }
        else if (teamFighterList[1].Rudo.Hp <= 0)
        {
            GlobalVariables.PrintWithColor("#FFFFFF", teamFighterList[0].Rudo.Fighter.FighterName + " won");
            return true;
        }
        return false;
    }
}
