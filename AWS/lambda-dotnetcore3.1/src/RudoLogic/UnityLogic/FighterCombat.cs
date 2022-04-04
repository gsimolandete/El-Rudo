using System.Collections;
using System.Collections.Generic;

public class FighterCombat
{
    public enum TeamNum { Team1, Team2 }
    public static string TeamColor(TeamNum team) {
        switch (team) { 
            case TeamNum.Team1:
                return "#13F8FF";
            case TeamNum.Team2:
                return "#FF4113";
            default:
                throw new System.Exception();
        }
    }

    TeamNum team;
    Fighter fighter;
    float turnMeter;
    float hp;

    public FighterCombat(Fighter fighter, TeamNum team)
    {
        this.fighter = fighter;
        this.TurnMeter = fighter.Initiative;
        this.Hp = GlobalVariables.MaxHP(fighter);
        this.team = team;
    }

    public Fighter Fighter { get => fighter; set => fighter = value; }
    public float TurnMeter { get => turnMeter; set => turnMeter = value; }
    public float Hp { get => hp; set => hp = value; }
    public TeamNum Team { get => team; set => team = value; }

    public void ModifyHP(float variation)
    {
        hp += variation;
    }
}
