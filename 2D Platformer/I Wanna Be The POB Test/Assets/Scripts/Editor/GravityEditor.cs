#if (UNITY_EDITOR)
//////////////////////////////////////////////////////////////////
// File Name: GravityEditor.cs                                  //
// Author: Josh Godsell                                         //
// Date Created: 1/5/19                                         //
// Date Last Edited: 1/5/19                                     //
// Brief:Custom editor class for Gravity class                  //
//////////////////////////////////////////////////////////////////
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEditor.SceneManagement;


[CustomEditor(typeof(Gravity))]
public class GravityEditor : Editor {

    //the propertiey variables for the instance of the script
    private SerializedProperty m_fMultiplier;
    private SerializedProperty m_eDirection;

    public void OnEnable()
    {

        //get the serialized variables from the edited class
        m_eDirection = serializedObject.FindProperty("m_eGravityDirection");
        m_fMultiplier = serializedObject.FindProperty("m_fGravityMultiplier");



    }

    public override void OnInspectorGUI()
    {

        //create a property fields on the editor to change the Variables
        EditorGUILayout.PropertyField(m_eDirection, new GUIContent("Gravity Direction"));
     

        EditorGUILayout.PropertyField(m_fMultiplier, new GUIContent("Gravity Multiplier"));
        
        //if a change is made save the scene
        if (GUI.changed)
        {
            EditorSceneManager.SaveScene(EditorSceneManager.GetActiveScene(), EditorSceneManager.GetActiveScene().path);
        }


        //apply the changes to the script
        serializedObject.ApplyModifiedProperties();
    }

}
#endif