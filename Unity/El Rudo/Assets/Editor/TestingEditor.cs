using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(Testing))]
public class TestingEditor : Editor
{
    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();

        Testing myTarget = (Testing)target;

        if (GUILayout.Button("Test 1"))
        {
            myTarget.test1();
        }

        if (GUILayout.Button("Test 2"))
        {
            myTarget.test2();
        }
    }
}
