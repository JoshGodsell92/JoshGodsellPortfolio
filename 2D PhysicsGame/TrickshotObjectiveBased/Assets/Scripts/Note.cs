#if (UNITY_EDITOR)

using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;


//custom editor class for helpbox scrpit to add comments to any object this scrip0t is attached to  - JG
[CustomEditor(typeof(HelpBox))]
public class Note : Editor
{
    private string sHelpMessage;
    private HelpBox hpThisHelpbox;
    private bool bEditable;

    public void OnEnable()
    {
        hpThisHelpbox = (HelpBox)target;
        bEditable = false;
    }

    public override void OnInspectorGUI()
    {
        // creates a help box and gets the message stored in the helpbox script  - JG
        if (!bEditable)
        {
            EditorGUILayout.HelpBox(hpThisHelpbox.getMessage(), MessageType.Info);
        }

        EditorGUILayout.BeginHorizontal();

        // adds a edit button to allow the helpbox message to be edited  - JG
        if (GUILayout.Button("Edit",GUILayout.Width(Screen.width / 2)))
        {
            bEditable = true;
        }
        // adds a save button to allow the helpbox message to be saved it also breaks the prefab of objective manager 
        //to save the individual objective of each scene - JG
        if (GUILayout.Button("Save"))
        {
            if (bEditable)
            {
                hpThisHelpbox.setMessage(sHelpMessage);
                bEditable = false;
                sHelpMessage = hpThisHelpbox.getMessage();
            }

            if (hpThisHelpbox.gameObject.tag == "ObjectiveManager")
            {
                PrefabUtility.DisconnectPrefabInstance(hpThisHelpbox.gameObject);
            }

            EditorSceneManager.SaveScene(EditorSceneManager.GetActiveScene(), EditorSceneManager.GetActiveScene().path);
        }

        EditorGUILayout.EndHorizontal();

        if(bEditable)
        {
          sHelpMessage = EditorGUILayout.TextArea(sHelpMessage);

        }

    }
        



}

#endif