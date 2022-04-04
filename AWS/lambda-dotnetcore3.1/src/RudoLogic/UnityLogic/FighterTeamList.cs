using System.Collections;
using System.Collections.Generic;

public class FighterTeamList
{
    FighterCombat rudo;
    List<FighterCombat> pets;

    public List<FighterCombat> Pets { get => pets; set => pets = value; }
    public FighterCombat Rudo { get => rudo; set => rudo = value; }
    public int Count { get => pets.Count + 1; }
    public FighterCombat this[int index]
    {
        get
        {
            if (index == 0)
                return rudo;
            else 
                return pets[index - 1];
        }
    }
}
