#if (UNITY_EDITOR)

using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;

//custom editor for the objective script - JG
[CustomEditor(typeof(ObjectiveScript))]
public class ObjectiveScriptEditor : Editor {

    //variables for the instance of the objective script this editor is responcible for - JG
    static int objectiveIndex;
    private ObjectiveScript oS;


    //variables for the enumerations of objective script - JG
    private ObjectiveScript.comboOptions comType;
    private ObjectiveScript.objectiveType objectiveType;
    private ObjectiveScript.BallType ballType;

    //variables for setting the targets for the objective - JG
    static Object targetWall;
    static int targetValue;
    static int shotTargetValue;

    public void OnEnable()
    {

        //find this objective script and check its index - JG
        oS = (ObjectiveScript)target;
        checkIndex();

    }

    public override void OnInspectorGUI()
    {
        //get and show and set the index of this script in the editor - JG
        objectiveIndex = oS.GetIndex();
        objectiveIndex = EditorGUILayout.IntField("Objective Index:", objectiveIndex);
        oS.SetIndex(objectiveIndex);

        //get the saved objective type setup a drop down option for the objective type and
        //when it is changed save it in the objective script - JG
        objectiveType = oS.GetObjectiveType();
        objectiveType = (ObjectiveScript.objectiveType)EditorGUILayout.EnumPopup("Objective Option:", objectiveType);
        oS.SetType(objectiveType);
      


        if (objectiveType == ObjectiveScript.objectiveType.Combo)
        {
            // if the combo type is selected in the drop down create a drop down for type of combo and a int field value to be tested for later - JG
            comType = oS.GetComboOption();
            comType = (ObjectiveScript.comboOptions)EditorGUILayout.EnumPopup("Combo Option:", comType);
            if (GUI.changed)
            {
                oS.SetComboOption(comType);
                //EditorSceneManager.SaveScene(EditorSceneManager.GetActiveScene(), EditorSceneManager.GetActiveScene().path);
            }

            if (comType== ObjectiveScript.comboOptions.LessThan)
            {
                int tempCombo = oS.GetComboValue();
                tempCombo = EditorGUILayout.IntField("Combo less than:", tempCombo);
                if(GUI.changed)
                {
                    oS.SetComboValue(tempCombo);
                    EditorSceneManager.SaveScene(EditorSceneManager.GetActiveScene(), EditorSceneManager.GetActiveScene().path); 
                }
            }
            else if (comType == ObjectiveScript.comboOptions.GreaterThan)
            {
                int tempCombo = oS.GetComboValue();
                tempCombo = EditorGUILayout.IntField("Combo greater than:", tempCombo);
                if (GUI.changed)
                {
                    oS.SetComboValue(tempCombo);
                    EditorSceneManager.SaveScene(EditorSceneManager.GetActiveScene(), EditorSceneManager.GetActiveScene().path);
                }
            }
            else if (comType == ObjectiveScript.comboOptions.EqualTo)
            {
                int tempCombo = oS.GetComboValue();
                tempCombo = EditorGUILayout.IntField("Combo equal to:", tempCombo);
                if (GUI.changed)
                {
                    oS.SetComboValue(tempCombo);
                    EditorSceneManager.SaveScene(EditorSceneManager.GetActiveScene(), EditorSceneManager.GetActiveScene().path);
                }
            }
        }
        // if the objective type is of type max shots create a int field to set the max shots that can be taken - JG
        else if (objectiveType == ObjectiveScript.objectiveType.MaxShots)
        {
            int tempShot = oS.GetShotValue();
            tempShot = EditorGUILayout.IntField("Shot Target:", tempShot);
            if (GUI.changed)
            {
                oS.SetShotValue(tempShot);
                EditorSceneManager.SaveScene(EditorSceneManager.GetActiveScene(), EditorSceneManager.GetActiveScene().path);
            }
        }
        else if (objectiveType == ObjectiveScript.objectiveType.StarObject)
        {
            oS.SetStarObject();
        }

        // if the objective type is of target wall tpe create a object field to set the target object that can be tested for in the collision - JG
        else if (objectiveType == ObjectiveScript.objectiveType.WallTarget)
        {
            targetWall = oS.GetTargetWall();
            targetWall = EditorGUILayout.ObjectField("Target Wall:", targetWall, typeof(GameObject),true,null);
            targetWall.name = "TargetWall";
            oS.SetTargetWall((GameObject)targetWall);

        }

        //if the objective is of type ballcompleted the create a enum drop down options for the type of ball to be used to complete the level - JG
        else if (objectiveType == ObjectiveScript.objectiveType.BallCompleted)
        {
            ballType = oS.GetBallType();
            ballType = (ObjectiveScript.BallType)EditorGUILayout.EnumPopup("Ball To Be Used:", ballType);
            if (GUI.changed)
            {
                oS.SetBallType(ballType);
            }
        }

        base.OnInspectorGUI();
    }

    public void checkIndex()
    {
        //assign an index for a new objective script thats been added - JG
        ObjectiveScript[] objectives;

        objectives = GameObject.FindObjectsOfType<ObjectiveScript>();


        for(int i=0; i < objectives.Length;i++)
        {
            objectives[i].SetIndex(i + 1);
        }

    }
}
#endif