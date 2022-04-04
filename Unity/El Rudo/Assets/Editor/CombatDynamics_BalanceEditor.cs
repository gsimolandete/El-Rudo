using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(CombatDynamics_Balance))]
public class CombatDynamics_BalanceEditor : Editor
{
    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();

        CombatDynamics_Balance myTarget = (CombatDynamics_Balance) target;

        if(GUILayout.Button("Start Battle"))
        {
            myTarget.StartCombat();
        }
    }
}
