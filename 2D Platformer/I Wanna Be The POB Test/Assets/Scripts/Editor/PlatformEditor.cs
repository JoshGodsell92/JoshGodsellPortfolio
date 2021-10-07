#if (UNITY_EDITOR)
//////////////////////////////////////////////////////////////////
// File Name: PlatformEditor.cs                                 //
// Author: Josh Godsell                                         //
// Date Created: 24/2/19                                        //
// Date Last Edited: 24/2/19                                    //
// Brief:Custom editor class for platformController class       //
//////////////////////////////////////////////////////////////////
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEditor.SceneManagement;


[CustomEditor(typeof(PlatformController))]
public class PlatformEditor : Editor
{
    private PlatformController m_instance;

    //the editable variables from the platform controller script
    private SerializedProperty m_eInstanceDirection;
    private SerializedProperty m_fAngle;
    private SerializedProperty m_fTravelDistance;
    private SerializedProperty m_fTravelSpeed;
    private SerializedProperty m_fWaitDelay;
    private SerializedProperty m_PointCount;
    private SerializedProperty m_Path;

    public void OnEnable()
    {

        //get the serialized variables from the edited class
        m_eInstanceDirection = serializedObject.FindProperty("m_eTravelDirection");

        //Find all the properties from the instance of the script
        m_fAngle = serializedObject.FindProperty("m_fAngle");
        m_fTravelDistance = serializedObject.FindProperty("m_fDistance");
        m_fTravelSpeed = serializedObject.FindProperty("m_fSpeed");
        m_fWaitDelay = serializedObject.FindProperty("m_fWaitPathDelay");
        m_PointCount = serializedObject.FindProperty("m_iPathPointCount");
        m_Path = serializedObject.FindProperty("m_PathPoints");

    }

    public override void OnInspectorGUI()
    {

        //create a property fields on the editor to change the Variables
        EditorGUILayout.PropertyField(m_eInstanceDirection, new GUIContent("Travel Direction"));

        //if the direction of travel is set to Diagonal
        if (m_eInstanceDirection.enumValueIndex == (int)PlatformController.DIRECTION.DIAGONAL)
        {
            //create property fields for each relevent property
            EditorGUILayout.PropertyField(m_fAngle, new GUIContent("Angle"));
            EditorGUILayout.PropertyField(m_fTravelDistance, new GUIContent("Distance"));
            EditorGUILayout.LabelField(new GUIContent("Distance is from origin in each direction"));

            EditorGUILayout.PropertyField(m_fTravelSpeed, new GUIContent("Speed"));
        }

        //if the direction is set to path
        else if(m_eInstanceDirection.enumValueIndex == (int)PlatformController.DIRECTION.PATH)
        {

            //create property fields for each property
            EditorGUILayout.PropertyField(m_Path, new GUIContent("Path Objects"),true);

            EditorGUILayout.PropertyField(m_fTravelSpeed, new GUIContent("Speed"));
            EditorGUILayout.PropertyField(m_fWaitDelay, new GUIContent("Wait at Point"));


        }
        else
        {

            //else if any other direction type only create the relevent property fields
            EditorGUILayout.PropertyField(m_fTravelDistance, new GUIContent("Distance"));
            EditorGUILayout.LabelField(new GUIContent("Distance is from origin in each direction"));


            EditorGUILayout.PropertyField(m_fTravelSpeed, new GUIContent("Speed"));
        }

                      
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