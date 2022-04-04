using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(CombatDynamics))]
public class CombatDynamicsEditor : Editor
{
    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();

        CombatDynamics myTarget = (CombatDynamics) target;

        if(GUILayout.Button("Start Battle"))
        {
            myTarget.StartCombat();
        }
    }
}
