using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(Pathfinding))]
public class CustomInspecter : Editor {

    public override void OnInspectorGUI()
    {
        Pathfinding myScript = (Pathfinding)target;
        base.OnInspectorGUI();
        if (GUILayout.Button("Run Test Ops")) {
            myScript.motherOfAllMethods();
        }
    }

}
