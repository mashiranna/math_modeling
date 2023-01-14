using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;


[CustomEditor(typeof(Edge)), CanEditMultipleObjects]
public class EdgeEditor : Editor
{

    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();
        GUILayout.Space(10f);


        Edge script = (Edge)target;

        if (GUILayout.Button("Reboot"))
        {
            script.Reboot();
        }

        if (GUILayout.Button("Redraw"))
        {
            script.DrawEdge();
        }

    }
}
