using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class ObjectiveScript : MonoBehaviour {

    //enumerations for the different types of balls objectives and combo types - JG
    public enum objectiveType
    {
        Combo = 0,
        MaxShots = 1,
        StarObject = 2,
        WallTarget = 3,
        BallCompleted = 4,
    };

    public enum comboOptions
    {
        LessThan,
        GreaterThan,
        EqualTo
    };

    public enum BallType
    {
        BasketBall,
        BouncyBall,
        BeanBag,
        FloatyBall
    };

    //variable for storing of this instances objective id - JG
    [HideInInspector]
    [SerializeField]
    private int objectiveIndex;

    //variables for storing the current instances objective type and if relevent its combo type and ball type -JG
    #region Stored_types

    [HideInInspector]
    [SerializeField]
    private objectiveType objType;

    [HideInInspector]
    [SerializeField]
    private comboOptions comboOption;

    [HideInInspector]
    [SerializeField]
    private BallType BallTypePick;
    #endregion

    //values used for checking with players score - JG
    [HideInInspector]
    [SerializeField]
    public int comboValue;

    [HideInInspector]
    [SerializeField]
    private int shotValue;


    //game object and bool for the star - JG
    [HideInInspector]
    [SerializeField]
    private GameObject starObject;
    private bool bIsStarCollected = false;

    //target wall game object and bools - JG
    [HideInInspector]
    [SerializeField]
    private GameObject targetWall;
    private bool bTargetWallHit;

    //bool for if this objective was completed - JG
    public bool objComplete;

    //getters and setters for all variables - JG
    #region Getter_Setter_functions
    public int GetIndex()
    {
        return objectiveIndex;
    }

    public void SetIndex(int a_index)
    {
        objectiveIndex = a_index;
    }

    public objectiveType GetObjectiveType()
    {
        return objType;
    }

    public void SetType(objectiveType a_Type)
    {
        objType = a_Type;
    }

    public comboOptions GetComboOption()
    {
        return comboOption;
    }

    public void SetComboOption(comboOptions a_option)
    {
        comboOption = a_option;
    }

    public int GetComboValue()
    {
        return comboValue;
    }

    public void SetComboValue(int a_ComboValue)
    {
        comboValue = a_ComboValue;
    }

    public BallType GetBallType()
    {
        return BallTypePick;
    }

    public void SetBallType(BallType a_BallPick)
    {
        BallTypePick = a_BallPick;
    }

    public int GetShotValue()
    {
        return shotValue;
    }

    public void SetShotValue(int a_ShotValue)
    {
        shotValue = a_ShotValue;
    }

    public GameObject GetStarObject()
    {
        return starObject;
    }

    public void SetStarObject()
    {
        starObject = GameObject.FindGameObjectWithTag("Star");
    }

    public GameObject GetTargetWall()
    {
        return targetWall;
    }

    public void SetTargetWall(GameObject a_targetWall)
    {
        targetWall = a_targetWall;
    }

    public bool getCompleted()
    {
        return objComplete;
    }

    public void setCompleted(bool a_bool)
    {
        objComplete = a_bool;
    }

    public void setStarCollected(bool a_bool)
    {
        bIsStarCollected = a_bool;
    }

    public void setWallHit(bool a_bool)
    {
        bTargetWallHit = a_bool;
    }

    #endregion

    //function for testing the objective to see if it has been completed - JG
    public void testObjective()
    {
        //gets the game control script in the scene and gets the current active scene - JG
        GameControl controlScript = FindObjectOfType<GameControl>();
        int Level = int.Parse(SceneManager.GetActiveScene().name);


        //if the objective is of type combo which type is it - JG
        if (objType == objectiveType.Combo)
        {
            //if the combo type is of type equal to and the combo the player achieved
            //is equal to the iComboValue then the objective was complete and update 
            //the control script - JG
            if (comboOption == comboOptions.EqualTo)
            {
                if (controlScript.iCombo == comboValue)
                {
                    objComplete = true;
                    controlScript.UpdateObjective(Level, objectiveIndex, true);
                }
            }
            //if the combo type is of type greater than to and the combo the player 
            //achieved is greater than to the iComboValue then the objective was complete 
            //and update the control script - JG
            else if (comboOption == comboOptions.GreaterThan)
            {
                if (controlScript.iCombo > comboValue)
                {
                    objComplete = true;
                    controlScript.UpdateObjective(Level, objectiveIndex, true);

                }
            }
            //if the combo type is of type less than to and the combo the player achieved 
            //is less than to the iComboValue then the objective was complete and update
            //the control script - JG
            else if (comboOption == comboOptions.LessThan)
            {
                if (controlScript.iCombo < comboValue)
                {
                    objComplete = true;
                    controlScript.UpdateObjective(Level, objectiveIndex, true);
                }
            }
        }
        else if (objType == objectiveType.MaxShots)
        {
            //if the objective is of type MaxShots - JG
            if (controlScript.getShotCount() < shotValue)
            {
                // and player has taken less shots than iShotValue then set as complete and update the control script - JG
                objComplete = true;
                controlScript.UpdateObjective(Level, objectiveIndex, true);
            }
        }
        else if (objType == objectiveType.StarObject)
        {
            //if the type of objective is star object and it has been collected then set to completed and update 
            //the conrol script - JG
            if (bIsStarCollected)
            {
                objComplete = true;
                controlScript.UpdateObjective(Level, objectiveIndex, true);
            }
        }
        //if the objective is of type ball or wall when the player completes the level with the right balltype 
        //or has hit the wall in the same shot as the player has completed the level then set the objective to 
        //completed and then save the objective in the control script - JG
        else if (objType == objectiveType.WallTarget)
        {
            if (bTargetWallHit)
            {
                objComplete = true;
                controlScript.UpdateObjective(Level, objectiveIndex, true);
                controlScript.Save();
            }
        }
        else if (objType == objectiveType.BallCompleted)
        {
            if(controlScript.GetBallTypePicked() == BallTypePick)
            {
                objComplete = true;
                controlScript.UpdateObjective(Level, objectiveIndex, true);
                controlScript.Save();
            }
        }
    }

    public void Start()
    {
        GameControl controlScript = FindObjectOfType<GameControl>();
        int Level = int.Parse(SceneManager.GetActiveScene().name);

        objComplete = controlScript.baLevels[Level, objectiveIndex];

        //controlScript.SetRequirement(objectiveIndex);

    }
}

