using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Runtime.Serialization.Formatters.Binary;
using System.IO;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GameControl : MonoBehaviour {

    //Some generic variables that are used for the main aspects of the game - KT
    private int iScore = 0;
    public int iShotCounter = 0;
    public int iCombo = 0;
    private int iHighScore = 0;
    private int iMultiplier = 1;

    public bool bX2combo = false;
    public bool bPaused = false;

    public string sLevelName;

    //Some references to the UI - KT
    private Text tLevelTextName;
    private Text tScoreText;
    private Text tShotTextName;
    private Text tShotText;
    private Text tComboText;
    private Text tHighScoreText;
    private Text tComboTextObj;
    private Text tShotTextObj;

    //An object for the text - KT
    public GameObject goTextGameObject;

    //Some structures for the objectives - KT
    public Objective Obj1 = new Objective();
    public Objective Obj2 = new Objective();
    public Objective Obj3 = new Objective();

    public GameObject goPauseCanvas;

    //An array for the objectives - KT
    public Objective[] Objectives;

    //An array that stores which objectives have been completed on each level - KT
    public bool[,] baLevels;

    //The customisable colours - KT
    public Color cPlayerColor = Color.blue;
    public Color cProjectileColor = Color.red;
    public Color cLineOneColor;
    public Color cLineTwoColor;
    public Color cAimLineColor;
    public Color cAimLineColor2;

    private int iLevelToLoad = 0;

    //The different types of physics materials - KT
    public PhysicsMaterial2D BallType;

    public PhysicsMaterial2D pmDefault;
    public PhysicsMaterial2D pmBouncyBall;
    public PhysicsMaterial2D pmBeanBag;
    public PhysicsMaterial2D pmFloatyBall;

    //The sprites that go with each physics materials - KT
    public Sprite sDefaultSprite;
    public Sprite sBouncyBallSprite;
    public Sprite sBeanBagSprite;
    public Sprite sFloatyBallSprite;

    [SerializeField]
    private string sBallTypeName;

    private ObjectiveScript.BallType BallTypePick;

    [SerializeField]
    private bool bClearSave;

    // Use this for initialization
    void Start()
    {
        DontDestroyOnLoad(this.gameObject);
     
    }

    private void FixedUpdate()
    {
      
    }

    // Update is called once per frame
    void Update() {

        if (bX2combo == true)
        {
            iMultiplier = 2;
        }
        else
        {
            iMultiplier = 1;
        }

    }

    //When the game starts this will initialise all the arrays - KT
    void OnEnable()
    {
        if (bClearSave)
        {
            ClearFile();
        }
        Objectives = new Objective[3];
        for (int i = 0; i < 3; i++)
        {
            Objectives[i] = new Objective();
        }
        baLevels = new bool[22, 4];
        for (int a = 0; a < 22; a++)
        {
            for (int b = 0; b < 4; b++)
            {
                baLevels[a, b] = false;
            }
        }
        Load();

    }

    public void Save()
    {
        //Create a binary formatter to store the game save into - KT
        BinaryFormatter bf = new BinaryFormatter();

        //Set the path that the data will be saved into - KT
        FileStream file = File.Create(Application.persistentDataPath + "/playerInfo.dat");

        //Move everything from the game control to the save file - KT
        PlayerData data = new PlayerData();
        data.iHighScore = iHighScore;
        data.Levels = baLevels;
        data.BallTypeName = sBallTypeName;

        //Turn the data into binary - KT
        bf.Serialize(file, data);
        file.Close();
    }

    public void Load()
    {
        //Check if the save file exists - KT
        if (File.Exists(Application.persistentDataPath + "/playerInfo.dat"))
        {
            //Create a new binary formatter and get the file location - KT
            BinaryFormatter bf = new BinaryFormatter();
            FileStream file = File.Open(Application.persistentDataPath + "/playerInfo.dat", FileMode.Open);

            //Turn the data from binary to normal data - KT
            PlayerData data = (PlayerData)bf.Deserialize(file);
            file.Close();

            //Move everything from the save file into the game control - KT
            iHighScore = data.iHighScore;
            baLevels = data.Levels;
            sBallTypeName = data.BallTypeName;
        }
        else
        {
            //If a file doesn't exist then it will create a new one - KT
            Save();
            Load();
        }
    }

    public void ClearFile()
    {
        //If the file exists then delete - KT
        if (File.Exists(Application.persistentDataPath + "/playerInfo.dat"))
        {
            File.Delete(Application.persistentDataPath + "/playerInfo.dat");
        }
    }

    //Load the level depending on the level name - KT
    public void LoadLevel()
    {
        SceneManager.LoadScene(sLevelName);
    }

    //Increase the shot counter - KT
    public void AddShotCounter(int a_iShotCounter)
    {
        iShotCounter += a_iShotCounter;
        tShotText.text = iShotCounter.ToString();
        if (iShotCounter <= 0)
        {
            SceneManager.LoadScene("End");
            goTextGameObject = GameObject.FindWithTag("ScoreText");
            tScoreText = goTextGameObject.GetComponent<Text>(); //Set the object to the text - KT
            tScoreText.text = iScore.ToString();
        }
    }

    public void AddScore(int a_iScore)
    {
        iScore += a_iScore;
        tScoreText.text = iScore.ToString();
    }

    public void SetShotCounter(int a_iShotCounter)
    {
        iShotCounter = a_iShotCounter;
        tShotText.text = iShotCounter.ToString();
    }

    public void SetScore(int a_iScore)
    {
        iScore = a_iScore;
        tScoreText.text = iScore.ToString();
    }

    public void IncreaseCombo(int a_iCombo)
    {
        iCombo += (a_iCombo * iMultiplier);
        tComboText.text = iCombo.ToString();
    }

    public void SetCombo(int a_iCombo)
    {
        iCombo = a_iCombo;
        tComboText.text = iCombo.ToString();
    }

    //Set the shot and counter text
    public void Scored()
    {
        tShotText.text = iShotCounter.ToString();
        tComboText.text = iCombo.ToString();
    }

    //This function gets called when the game ends and the end screen loads. This mainly just sets the text - KT
    public void EndScreen()
    {
        goTextGameObject = GameObject.FindWithTag("ScoreText");
        tScoreText = goTextGameObject.GetComponent<Text>(); //Set the object to the text - KT
        tScoreText.text = iScore.ToString();
        goTextGameObject = GameObject.FindWithTag("HighScore");
        tHighScoreText = goTextGameObject.GetComponent<Text>(); //Set the object to the text - KT
        tHighScoreText.text = iHighScore.ToString();
        CheckHighScore();
    }

    //This function is used when the game starts to set up the UI - KT
    public void StartUI()
    {
        goTextGameObject = GameObject.FindWithTag("ShotCounter");
        tShotText = goTextGameObject.GetComponent<Text>(); //Set the object to the text - KT
        tShotText.text = iShotCounter.ToString();

        goTextGameObject = GameObject.FindWithTag("Combo");
        tComboText = goTextGameObject.GetComponent<Text>(); //Set the object to the text - KT
        tComboText.text = iCombo.ToString();

        goPauseCanvas = GameObject.FindGameObjectWithTag("Pause");

    }

    public void CheckHighScore()
    {
        if (iScore > iHighScore)
        {
            iHighScore = iScore;
            Save();
        }
    }

    public void Reset()
    {
        iScore = 0;
        iShotCounter = 0;
        iCombo = 0;
    }

    //Set the objectives and the requirements - KT
    private void SetObjective(int a_ID, int a_Level, string a_Requirement)
    {
        if (Obj1.ID == 0)
        {
            Obj1.ID = a_ID;
            Obj1.Level = a_Level;
            Obj1.Requirement = a_Requirement;
        }
        else if (Obj2.ID == 0)
        {
            Obj2.ID = a_ID;
            Obj2.Level = a_Level;
            Obj2.Requirement = a_Requirement;
        }
        else if (Obj3.ID == 0)
        {
            Obj3.ID = a_ID;
            Obj3.Level = a_Level;
            Obj3.Requirement = a_Requirement;
        }

    }

    //Set the requirement for a specific objective - KT
    private void SetSpeficRequirement(string a_Requirement, int a_Objective)
    {
        switch (a_Objective)
        {
            case 1:
                Obj1.Requirement = a_Requirement;
                break;
            case 2:
                Obj2.Requirement = a_Requirement;
                break;
            case 3:
                Obj3.Requirement = a_Requirement;
                break;
            default:
                break;
        }

    }

    //This function is used to set the objective requirements - KT
    public void SetRequirement()
    {
        //For each objective script in objectives it will set the objective - KT
        GameObject ObjectiveManager = GameObject.Find("ObjectiveManager");
        ObjectiveScript[] Objectives = ObjectiveManager.GetComponents<ObjectiveScript>();

        foreach (ObjectiveScript os in Objectives)
        {
            string ObjectiveType = os.GetObjectiveType().ToString();
            if (ObjectiveType == "Combo")
            {
                string ComboType = os.GetComboOption().ToString();
                if (ComboType == "LessThan")
                {
                    SetSpeficRequirement("Score with a combo less than: " + os.GetComboValue().ToString(), os.GetIndex());

                }
                else if (ComboType == "GreaterThan")
                {
                    SetSpeficRequirement("Score with a combo greater than: " + os.GetComboValue().ToString(), os.GetIndex());

                }
                else if (ComboType == "EqualTo")
                {
                    SetSpeficRequirement("Score with an exact combo of: " + os.GetComboValue().ToString(), os.GetIndex());

                }
            }
            else if (ObjectiveType == "MaxShots")
            {
                SetSpeficRequirement("Score with less than " + os.GetShotValue().ToString() + " shots", os.GetIndex());
            }
            else if (ObjectiveType == "StarObject")
            {
                SetSpeficRequirement("Hit the star", os.GetIndex());
            }
            else if (ObjectiveType == "WallTarget")
            {
                SetSpeficRequirement("Hit wall A", os.GetIndex());
            }
            else if (ObjectiveType == "BallCompleted")
            {
                SetSpeficRequirement("Score with " + os.GetBallType().ToString(), os.GetIndex());
            }
        }
    }

    //Get the level requirement - KT
    public string GetLevelRequirement(int a_Objective)
    {
        switch (a_Objective)
        {
            case 1:
                return Obj1.Requirement;
            case 2:
                return Obj2.Requirement;
            case 3:
                return Obj3.Requirement;
            default:
                return null;
        }
    }

    //Return the spefic levels stars - KT
    public bool[] GetLevelStars(int a_Level)
    {
        bool[] Objectives = { false, false, false, false };
        Objectives = new bool[4];
        for (int a = 0; a < 4; a++)
        {
            Objectives[a] = baLevels[a_Level, a];
        }
        return Objectives;
    }

    public bool CheckObjective(int a_level, int a_Objective)
    {
        return baLevels[a_level, a_Objective];
    }

    public void UpdateObjective(int a_Level, int a_Objective, bool a_Completed)
    {
        string LevelName = "Level" + a_Level;

        baLevels[a_Level, a_Objective] = a_Completed;
        Save();
    }

    //put in the game controller
    //Set the color from the customise script - SM
    public void SetPlayerColor(Color a_Color)
    {
        cPlayerColor = a_Color;
    }

    public void SetProjectileColor(Color a_Color)
    {
        cProjectileColor = a_Color;
    }

    public void SetLineOneColor(Color a_Color)
    {
        cLineOneColor = a_Color;
    }

    public void SetLineTwoColor(Color a_Color)
    {
        cLineTwoColor = a_Color;
    }

    public void SetAimColor(Color a_Color)
    {
        cAimLineColor = a_Color;
    }

    public void SetAimColor2(Color a_Color)
    {
        cAimLineColor2 = a_Color;

    }

    public int getShotCount()
    {
        return iShotCounter;
    }

    public void setShotCount(int a_iShotCount)
    {
        iShotCounter = a_iShotCount;
    }

    //If the game has been paused then set the canvas as false if not then set it true - KT
    public void Pause()
    {
        if (bPaused == false)
        {
            goPauseCanvas.SetActive(false);
            bPaused = true;
        }
        
        else if (bPaused == true)
        {
            goPauseCanvas.GetComponent<Canvas>().enabled = false;
            bPaused = false;
        }
    }

    public void SetPhysicsMaterial(PhysicsMaterial2D a_PM)
    {
        BallType = a_PM;
    }

    public PhysicsMaterial2D GetPhysicsMaterial()
    {
        return BallType;
    }

    public void SetBallTypeName(string a_BallTypeName)
    {
        sBallTypeName = a_BallTypeName;
        Save();
    }

    public string GetBallTypeName()
    {
        return sBallTypeName;
    }

    public ObjectiveScript.BallType GetBallTypePicked()
    {
        return BallTypePick;
    }

    public void SetBallPick(ObjectiveScript.BallType a_BallType)
    {
        BallTypePick = a_BallType;
    }

    public void SetLevelToLoad(int a_LevelNumber)
    {
        iLevelToLoad = a_LevelNumber;
    }

    public int GetLevelToLoad()
    {
        return iLevelToLoad;
    }

    //Return the correct physics material - KT
    public PhysicsMaterial2D GetCorrectBall()
    {
        switch (sBallTypeName)
        {
            case "Default":
                return pmDefault;
            case "BouncyBall":
                return pmBouncyBall;
            case "BeanBag":
                return pmBeanBag;
            case "FloatyBall":
                return pmFloatyBall;

            default:
                return null;
        }

    }

    //Return the correct ball sprite - KT
    public Sprite GetCorrectBallSprite()
    {
        switch (sBallTypeName)
        {
            case "Default":
                return sDefaultSprite;
            case "BouncyBall":
                return sBouncyBallSprite;
            case "BeanBag":
                return sBeanBagSprite;
            case "FloatyBall":
                return sFloatyBallSprite;

            default:
                return null;
        }

    }

    public int GetCombo()
    {
        return iCombo;
    }
}




//A class for each objective - KT
[Serializable]
public class Objective
{
    public int ID;
    public int Level;
    public string Requirement;

    public Objective()
    {
        ID = 0;
        Level = 0;
        Requirement = null;
    }
}

//A class to hold a whole levels objectives - KT
[Serializable]
class LevelObjectives
{
    public Objective Obj1;
    public Objective Obj2;
    public Objective Obj3;
}

//This is the game data class - KT
[Serializable]
class PlayerData
{
    public int iHighScore;

    public bool[,] Levels;

    public PlayerData()
    {
        Levels = new bool[22, 4];

        for (int a = 0; a < 22; a++)
        {
            for (int b = 0; b < 4; b++)
            {
                Levels[a, b] = false;
            }
        }

        Levels[0, 0] = true;
    }

    public string BallTypeName;
}

