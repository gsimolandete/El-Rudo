using System.Collections;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class Testing : MonoBehaviour
{
    public CombatDynamics combatDynamics;
    public TMP_InputField rudo1name, rudo1vitality, rudo1strength, rudo1agility, rudo1velocity;
    public TMP_InputField rudo2name, rudo2vitality, rudo2strength, rudo2agility, rudo2velocity;
    public TMP_InputField seed;
    public Toggle randomSeed;

    public void test1()
    {
#if !UNITY_EDITOR
        DebugStuff.DebugLogsTest.myLog = "";
#endif

        combatDynamics.randomSeed = randomSeed.isOn;

        if (!randomSeed.isOn)
            combatDynamics.seed = int.Parse(seed.text);

        combatDynamics.abstractRudo1 = new Rudo(rudo1name.text, 1, float.Parse(rudo1vitality.text), float.Parse(rudo1strength.text), float.Parse(rudo1agility.text), float.Parse(rudo1velocity.text), new List<Weapon>(), new List<AbstractPet>());
        combatDynamics.abstractRudo2 = new Rudo(rudo2name.text, 1, float.Parse(rudo2vitality.text), float.Parse(rudo2strength.text), float.Parse(rudo2agility.text), float.Parse(rudo2velocity.text), new List<Weapon>(), new List<AbstractPet>());

        combatDynamics.StartCombat();
    }

    public void test2()
    {

    }
}
