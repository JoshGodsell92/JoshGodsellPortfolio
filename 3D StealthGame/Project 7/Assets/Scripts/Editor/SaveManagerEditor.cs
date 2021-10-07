using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(SaveManager))]
public class SaveManagerEditor : Editor
{
    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();

        SaveManager myScript = (SaveManager)target;
        if (GUILayout.Button("Save Data"))
        {
            myScript.SaveGame();
        }
        if (GUILayout.Button("Load Data"))
        {
            myScript.LoadGame();
        }
    }
}
