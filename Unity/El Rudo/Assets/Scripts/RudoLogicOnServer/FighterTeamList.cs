using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FighterTeamList
{
    FighterCombat rudo;
    public FighterCombat pet;

    public FighterTeamList(FighterCombat rudo, FighterCombat pet)
    {
        this.rudo = rudo;
        this.pet = pet;
    }

    public FighterCombat Rudo { get => rudo; set => rudo = value; }
    public int Count { get => pet != null ? 2 : 1; }
    public FighterCombat this[int index]
    {
        get
        {
            if (index == 0)
                return rudo;
            else 
                return pet;
        }
    }
}
