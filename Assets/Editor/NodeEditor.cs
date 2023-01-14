using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;


[CustomEditor(typeof(Node)), CanEditMultipleObjects]


public class NodeEditor : Editor
{
    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();

        GUILayout.Space(10f);
        Node script = (Node)target;

        if (GUILayout.Button("Redraw"))
        {
            script.DrawEdges();
        }

    }


}
