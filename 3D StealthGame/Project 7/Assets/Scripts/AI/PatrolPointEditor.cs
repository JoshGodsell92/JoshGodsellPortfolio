#if UNITY_EDITOR
//////////////////////////////////////////////////////////////////////////
///File name: PatrolPointEditor.cs
///Date Created: 12/10/2020
///Created by: JG
///Brief: Editor interface for controlling patrol points.
///Last Edited by: JG
///Last Edited on: 12/10/2020
//////////////////////////////////////////////////////////////////////////

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(PatrolPoint))]
public class PatrolPointEditor : Editor
{

    SerializedProperty m_NeighbourArray;

    private void OnEnable()
    {
        m_NeighbourArray = serializedObject.FindProperty("m_aNeighbourPoints");
    }

    public override void OnInspectorGUI()
    {
        serializedObject.Update();

        EditorGUILayout.PropertyField(m_NeighbourArray);

        serializedObject.ApplyModifiedProperties();


    }

}

#endif
