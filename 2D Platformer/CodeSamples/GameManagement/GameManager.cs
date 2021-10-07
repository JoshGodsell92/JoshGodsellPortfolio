//////////////////////////////////////////////////////////////////
// File Name: GameManager.cs                                    //
// Author: Josh Godsell                                         //
// Date Created: 30/1/19                                        //
// Date Last Edited: 30/5/19                                    //
// Brief: Class containing Main Game Functions                  //
//////////////////////////////////////////////////////////////////
using System.Collections;
using System.Collections.Generic;
using System.Runtime.Serialization.Formatters.Binary;
using System.IO;
using System;

using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour {

    //game states enumeration
    private enum Game_State
    {
        MENU,
        LEVEL,
        LEVELCOMPLETE
    }

    //current game state
    Game_State m_GameState = Game_State.MENU;

    //User Selection Elements
    Canvas m_UserSelectionCanvas;
    Button m_AddUserButton;
    Dropdown m_UserDropDown;
    InputField m_UserInput;
    String[] m_UserList;
    Button m_ContinueButton;
    Text m_FeedbackText;

    //Main Canvas Elements
    Canvas m_MainMenuCanvas;
    Button m_StatsButton;
    Button m_LevelSelectButton;
    Button m_UserSelectButton;
    Text m_tUsername;

    //level selection elements
    Canvas m_LevelSelectCanvas;
    Button m_LevelOneButton;
    Button m_LevelTwoButton;
    Button m_LevelTwoLockButton;
    Button m_LevelThreeButton;
    Button m_LevelThreeLockButton;
    Button m_LevelFourButton;
    Button m_LevelFourLockButton;

    //LeaderBoard elements
    Canvas m_LeaderboardCanvas;
    GameObject m_LevelOneHolder;
    GameObject m_LevelOneEntries;
    Text[] m_LevelOneTimeTextElements;
    GameObject m_LevelTwoHolder;
    GameObject m_LevelTwoEntries;
    Text[] m_LevelTwoTimeTextElements;
    GameObject m_LevelThreeHolder;
    GameObject m_LevelThreeEntries;
    Text[] m_LevelThreeTimeTextElements;
    GameObject m_LevelFourHolder;
    GameObject m_LevelFourEntries;
    Text[] m_LevelFourTimeTextElements;

    GameObject m_LevelOneDeathHolder;
    GameObject m_LevelTwoDeathHolder;
    GameObject m_LevelThreeDeathHolder;
    GameObject m_LevelFourDeathHolder;

    GameObject m_LevelOneDeathEntries;
    GameObject m_LevelTwoDeathEntries;
    GameObject m_LevelThreeDeathEntries;
    GameObject m_LevelFourDeathEntries;

    Text[] m_LevelOneDeathTextEntries;
    Text[] m_LevelTwoDeathTextEntries;
    Text[] m_LevelThreeDeathTextEntries;
    Text[] m_LevelFourDeathTextEntries;

    //Player Data 
    private string m_sUsername;
    private Level[] m_aLevels;

    //Level Data
    private Level m_CurrentLevel;
    private Collectable[] m_LevelCollectables;

    //camera variables
    private GameObject m_MainCam;
    private int  m_iScreenWidth;
    private int  m_iScreenHeight;

    //player object
    private GameObject m_PlayerObject;

    //enemies and jump boosts
    private GameObject[] m_aExtraJumps;
    private StateBasedAi[] m_aEnemiesAI;

    //game overlay canvases and text components
    private Canvas m_GameOverlay;
    private Canvas m_PauseScreen;
    private Button m_QuitLevel;
    private Canvas m_LevelComplete;
    private Text m_FinalTimeText;
    private Text m_FinalDeathText;
    private Text m_TimeText;
    private Text m_DeathCount;

    //level run details
    private int m_iDeathCount = 0;
    private TimeSpan m_LevelTime;

    //the start time
    private DateTime m_StartTime;

    //pause time elements for calculation
    private DateTime m_PauseStart;
    private TimeSpan m_PauseTime;

    //if the game is paused
    private bool m_bIsPaused = false;

    //time for jumps to respawn
    public float m_fJumpRespawnTime;
   
    //arrays of level rooms and level bounds
    private Room[] m_aLevelRooms;
    private Bounds[] m_LevelBounds;

    //array of leaderboard data
    private LeaderboardData[] m_LevelBoards;

    //this game manager instance
    static GameManager m_thisInstance;

    //if the game unlocked for testing
    //unlocks all levels for play in order to test
    public bool m_bUnlockedForTest;

	// Use this for initialization
	void Awake ()
    {
         //get this game manager script otherwise if an instance already exists destroy this object
        if(m_thisInstance == null)
        {
            m_thisInstance = this.gameObject.GetComponent<GameManager>();
        }
        else
        {
            Destroy(this.gameObject);
            return;
        }

        //add this to dont destroy on load
        DontDestroyOnLoad(this);

        //initialise a new array for the leaderboard data
        m_LevelBoards = new LeaderboardData[4];

        //if the state is menu
        if (m_GameState == Game_State.MENU)
        {
            //initialise the menu elements
            InitialiseMenu();

            //deactivate the main canvases
            m_MainMenuCanvas.gameObject.SetActive(false);
            m_LevelSelectCanvas.gameObject.SetActive(false);

            //load the user list data and the leaderboard data
            LoadUserList();
            LoadLevelLeaderboards();
        }




    }

    //function to initialise the menus
    public void InitialiseMenu()
    {
        //assign the leaderboard canvas and elements
        try
        {
            //find and assign the leaderboard canvas
            m_LeaderboardCanvas = GameObject.Find("StatsCanvas").GetComponent<Canvas>();

            FindLeaderboardObjects();

        }
        catch (Exception)
        {

            throw;
        }

        //assign all elements of user selection screen
        try
        {
            m_UserSelectionCanvas = GameObject.Find("UserSelectionCanvas").GetComponent<Canvas>();
            m_UserDropDown = GameObject.Find("Dropdown").GetComponent<Dropdown>();
            m_AddUserButton = GameObject.Find("AddUser").GetComponent<Button>();
            m_ContinueButton = GameObject.Find("Continue").GetComponent<Button>();
            m_UserInput = GameObject.Find("UserInput").GetComponent<InputField>();
            m_AddUserButton.onClick.AddListener(AddUserToList);
            m_ContinueButton.onClick.AddListener(UserSelectContinue);
            m_FeedbackText = GameObject.Find("Feedback").GetComponent<Text>();
            m_FeedbackText.gameObject.SetActive(false);
        }
        catch (Exception)
        {

            throw;
        }

        //assign all elements of Main menu screen
        try
        {
            m_MainMenuCanvas = GameObject.Find("MainMenuCanvas").GetComponent<Canvas>();
            m_StatsButton = GameObject.Find("Stats").GetComponent<Button>();
            m_LevelSelectButton = GameObject.Find("LevelSelect").GetComponent<Button>();
            m_UserSelectButton = GameObject.Find("UserSelect").GetComponent<Button>();
            m_tUsername = GameObject.Find("Username").GetComponent<Text>();
            m_StatsButton.onClick.AddListener(StatsScreen);
            m_UserSelectButton.onClick.AddListener(UserSelectScreen);
            m_LevelSelectButton.onClick.AddListener(LevelSelectScreen);
            
        }
        catch (Exception)
        {

            throw;
        }

        //assign all elements of level select screen
        try
        {
            m_LevelSelectCanvas = GameObject.Find("LevelSelectCanvas").GetComponent<Canvas>();
            m_LevelOneButton = GameObject.Find("Level 1").GetComponent<Button>();
            m_LevelTwoButton = GameObject.Find("Level 2").GetComponent<Button>();
            m_LevelTwoButton.gameObject.SetActive(false);
            m_LevelTwoLockButton = GameObject.Find("Level 2 Locked").GetComponent<Button>();
            m_LevelThreeButton = GameObject.Find("Level 3").GetComponent<Button>();
            m_LevelThreeButton.gameObject.SetActive(false);
            m_LevelThreeLockButton = GameObject.Find("Level 3 Locked").GetComponent<Button>();
            m_LevelFourButton = GameObject.Find("Level 4").GetComponent<Button>();
            m_LevelFourButton.gameObject.SetActive(false);
            m_LevelFourLockButton = GameObject.Find("Level 4 Locked").GetComponent<Button>();


        }
        catch (Exception)
        {

            throw;
        }
    }

    //fixed update
    private void FixedUpdate()
    {

        //if the game state is level
        if (m_GameState == Game_State.LEVEL)
        {
            //check if the player is out of bounds
            PlayerDeath();
            //snap the camera when changing rooms

            CameraSnap();
        }
    }
    
    // Update is called once per frame
    void Update ()
    {

        //Main switch statement for game state
        switch(m_GameState)
        {
            case Game_State.MENU:

                break;
            case Game_State.LEVEL:

                //set the current level collectable data
                m_CurrentLevel.SetCollectablesData(m_LevelCollectables);

                //if esc is pressed when not paused
                if (Input.GetKeyDown(KeyCode.Escape) && !m_bIsPaused)
                {
                    //pause the game
                    m_bIsPaused = true;
                    //disable the main overlay and enable the pause screen
                    m_GameOverlay.gameObject.SetActive(false);
                    m_PauseScreen.gameObject.SetActive(true);
                    //set the pause time and set time scale to 0
                    m_PauseStart = DateTime.Now;
                    Time.timeScale = 0;
                }
                else if (Input.GetKeyDown(KeyCode.Escape) && m_bIsPaused)
                {
                    //eslse if pause is pressed while paused then set is paused to false
                    m_bIsPaused = false;
                    //diasable the pause screen and enable the game overlay
                    m_PauseScreen.gameObject.SetActive(false);
                    m_GameOverlay.gameObject.SetActive(true);

                    //calculate the start time from the level run and pause time
                    m_StartTime += m_PauseTime;
                    m_PauseTime = TimeSpan.Zero;
                    //Set time scale to 1
                    Time.timeScale = 1;
                }

                if (!m_bIsPaused)
                {
                    TimerCalc();
                }
                else
                {
                    PauseTimeCalc();
                }

                break;
            case Game_State.LEVELCOMPLETE:

                //if any key is pressed while the level complete state then unnlock the next level and go to the level select screen
                if(Input.anyKeyDown)
                {
                    UnlockNextLevel(GetCurrentLevel());

                    LevelSelectScreen();

                    Time.timeScale = 0;
                }

                break;
            default:
                break;
        }
    }

    private void OnApplicationQuit()
    {
        //m_CurrentLevel.SetCollectablesData(m_LevelCollectables);
        //SavePlayerData(m_sUsername);
    }

    //function to load player level and assign level variables
    public void LoadLevel()
    {
        //load the user data
        LoadPlayerData(m_sUsername);

        //assign the player object
        try
        {
            m_PlayerObject = GameObject.FindGameObjectWithTag("Player");
        }
        catch (Exception)
        {

            throw;
        }
        //assign the jump boost to the array
        try
        {
            m_aExtraJumps = GameObject.FindGameObjectsWithTag("ExtraJump");
        }
        catch (System.Exception)
        {

            throw new System.Exception("no Extra Jumps found");
        }

        //get and set the collectable data for the level
        try
        {
            Collectable[] t_LevelCollectables = FindObjectsOfType<Collectable>();
            m_LevelCollectables = new Collectable[t_LevelCollectables.Length];

           foreach(Collectable collectable in t_LevelCollectables)
           {
                m_LevelCollectables[collectable.GetIndex()] = collectable;

           }

           for(int i = 0; i < m_LevelCollectables.Length;++i)
           {
                m_LevelCollectables[i].SetCollected(m_CurrentLevel.GetCollectables()[i]);
           }

        }
        catch (Exception)
        {

            throw new System.Exception("Can't find collectables");
        }

        //find all the ai enemies in the level
        try
        {
            m_aEnemiesAI = FindObjectsOfType<StateBasedAi>();

        }
        catch (Exception)
        {

            throw new System.Exception("Can't find AI scripts");
        }

        //the differnent screen canvases for the level
        try
        {
            m_GameOverlay = GameObject.Find("MainOverlay").GetComponent<Canvas>();
            m_PauseScreen = GameObject.Find("PauseScreen").GetComponent<Canvas>();
            m_LevelComplete = GameObject.Find("EndScreen").GetComponent<Canvas>();

            m_FinalTimeText = GameObject.Find("FinalTime").GetComponent<Text>();
            m_FinalDeathText = GameObject.Find("FinalDeath").GetComponent<Text>();

            m_QuitLevel = GameObject.Find("QuitLevel").GetComponent<Button>();

            m_LevelComplete.gameObject.SetActive(false);
            m_PauseScreen.gameObject.SetActive(false);
        }
        catch (Exception)
        {

            throw new System.Exception("Can't find main overlay");
        }

        foreach (Transform child in m_GameOverlay.transform)
        {
            if (child.name == "Timer")
            {
                m_TimeText = child.GetComponent<Text>();
            }
            if (child.name == "DeathCount")
            {
                m_DeathCount = child.GetComponent<Text>();
            }
        }

        m_StartTime = DateTime.Now;

        foreach (GameObject ExtraJump in m_aExtraJumps)
        {
            ExtraJump.AddComponent<ExtraJump>();
        }


        //get the main camera of the level
        m_MainCam = Camera.main.gameObject;
        m_iScreenWidth = Screen.width;
        m_iScreenHeight = Screen.height;

        try
        {
            m_aLevelRooms = FindObjectsOfType<Room>();
        }
        catch (Exception)
        {

            throw new System.Exception("Cant find rooms");

        }
    }

    //timer calculation while not paused
    public void TimerCalc()
    {

        //get the run time from the start of the players run
        TimeSpan t_TimeChange = DateTime.Now - m_StartTime;

        m_LevelTime = t_TimeChange;

        m_TimeText.text = (t_TimeChange.Minutes.ToString() + ":" + t_TimeChange.Seconds.ToString("D2"));

    }

    //timer calculation while paused
    public void PauseTimeCalc()
    {
        m_PauseTime = DateTime.Now - m_PauseStart;
    }

    //functions to snap the camera to the room the player is in
    public void CameraSnap()
    {
        //get the player screenpos
        Vector3 t_v3PlayerScreenPos = Camera.main.WorldToScreenPoint(m_PlayerObject.transform.position);

        //if the screen pos is greater than the screen width 
        if(t_v3PlayerScreenPos.x > m_iScreenWidth)
        {
            //move the camera along to the next camera position
            Vector3 t_CamPosInPixels = Camera.main.WorldToScreenPoint(m_MainCam.transform.position);
            Vector3 t_NewCamPos = t_CamPosInPixels;
            t_NewCamPos.x += m_iScreenWidth;

            t_NewCamPos = Camera.main.ScreenToWorldPoint(t_NewCamPos);

            m_MainCam.transform.position = t_NewCamPos;
        }
        //same but for the opposite direction
        else if(t_v3PlayerScreenPos.x < 0)
        {
            Vector3 t_CamPosInPixels = Camera.main.WorldToScreenPoint(m_MainCam.transform.position);
            Vector3 t_NewCamPos = t_CamPosInPixels;
            t_NewCamPos.x -= m_iScreenWidth;

            t_NewCamPos = Camera.main.ScreenToWorldPoint(t_NewCamPos);

            m_MainCam.transform.position = t_NewCamPos;
        }

        //same again but for in the y axis
        if(t_v3PlayerScreenPos.y < 0)
        {
            Vector3 t_CamPosInPixels = Camera.main.WorldToScreenPoint(m_MainCam.transform.position);
            Vector3 t_NewCamPos = t_CamPosInPixels;
            t_NewCamPos.y -= m_iScreenHeight;

            t_NewCamPos = Camera.main.ScreenToWorldPoint(t_NewCamPos);

            m_MainCam.transform.position = t_NewCamPos;
        }
        else if (t_v3PlayerScreenPos.y > m_iScreenHeight)
        {
            Vector3 t_CamPosInPixels = Camera.main.WorldToScreenPoint(m_MainCam.transform.position);
            Vector3 t_NewCamPos = t_CamPosInPixels;


                t_NewCamPos.y += m_iScreenHeight;

                t_NewCamPos = Camera.main.ScreenToWorldPoint(t_NewCamPos);

                m_MainCam.transform.position = t_NewCamPos;
        }
    }

    //function for when the player is out of bounds
    public void PlayerDeath()
    {
        //if the player is not within any of the level room bounds then the player is killed
        if (!CheckPlayerinBounds())
        {
            //find the boss controller script and call reset
            if (FindObjectOfType<BossController>() != null)
            {
                FindObjectOfType<BossController>().Reset();
            }
                m_PlayerObject.GetComponent<PlayerControl>().Reset();
                m_iDeathCount++;
                Reset();

        }

        m_DeathCount.text = m_iDeathCount.ToString();
    }

    //when the player hits a spike kill the player
    public void SpikeDeath()
    {
        if (FindObjectOfType<BossController>() != null)
        {
            FindObjectOfType<BossController>().Reset();
        }

        m_iDeathCount++;
        Reset();


        m_DeathCount.text = m_iDeathCount.ToString();

    }

    //function to kill the player and reset the boss
    public void KillPlayer()
    {
        if (FindObjectOfType<BossController>() != null)
        {
            FindObjectOfType<BossController>().Reset();
        }

        m_PlayerObject.GetComponent<PlayerControl>().Reset();
        m_iDeathCount++;
        Reset();

        m_DeathCount.text = m_iDeathCount.ToString();

    }

    //function to return true if the player is within the level bounds
    public bool CheckPlayerinBounds()
    {
        foreach(Room room in m_aLevelRooms)
        {
            if(room.m_bounds.Contains(m_PlayerObject.transform.position))
            {
                try
                {
                    m_PlayerObject.GetComponent<PlayerControl>().SetRoom(room);
                }
                catch (Exception)
                {

                    throw;
                }
                return true;
            }

        }

        return false;
    }

    //Game reset function
    public void Reset()
    {

        //if the array of jump boosts is not null
        if (m_aExtraJumps != null)
        {
            //reset each of the jumps
            foreach (GameObject ExtraJump in m_aExtraJumps)
            {
                ExtraJump t_ExtraJumpScript = ExtraJump.GetComponent<ExtraJump>();

                if (t_ExtraJumpScript != null)
                {
                    if (!t_ExtraJumpScript.GetIsActive())
                    {
                        t_ExtraJumpScript.Reset();
                    }
                }
            }
        }

        //reset each of the enemies
        if(m_aEnemiesAI != null)
        {
            foreach(StateBasedAi AIScript in m_aEnemiesAI)
            {
                AIScript.Reset();
            }
        }
    }

    //User management functions
    public void LoadUserList()
    {
        //if the userlist exists
        if (File.Exists(Application.persistentDataPath + "/UserList.dat"))
        {
            BinaryFormatter formatter = new BinaryFormatter();
            FileStream file = File.Open(Application.persistentDataPath + "/UserList.dat", FileMode.Open);

            UserData UserData = (UserData)formatter.Deserialize(file);
            file.Close();

            //if the count of users is not 0
            if (UserData.GetCount() > 0)
            {
                //get the user list from the stored data
                m_UserList = UserData.GetList();
                //Set continue button and drop down list to active
                m_ContinueButton.gameObject.SetActive(true);
                m_UserDropDown.gameObject.SetActive(true);

                //initialise the drop down list
                if (m_UserList.Length > 0)
                {
                    UserDropDownInit();
                }

            }
            else
            {
                //if there are no users then set the continue button to inactive and the drop down list to inactive
                m_ContinueButton.gameObject.SetActive(false);
                m_UserDropDown.gameObject.SetActive(false);
            }
        }
        else
        {
            //if the file doesnt exist the save one and reload
            SaveUserList();
            LoadUserList();
        }
    }
    //functions to create a user list data file
    public void SaveUserList()
    {
        BinaryFormatter formatter = new BinaryFormatter();


        FileStream file = File.Create(Application.persistentDataPath + "/UserList.dat");

        UserData userData = new UserData();    


        formatter.Serialize(file, userData);
        file.Close();
    }
    void SaveUserList(UserData a_DataSet)
    {
        BinaryFormatter formatter = new BinaryFormatter();


        FileStream file = File.Create(Application.persistentDataPath + "/UserList.dat");

        UserData userData = a_DataSet;


        formatter.Serialize(file, userData);
        file.Close();
    }
    //function to add a new user to the userlist dataset
    public void AddUserToList()
    {
        //checks for invalid characters in the username
        bool validUsername = true;
        string invalidChars = "Characters: ";

        foreach(char character in m_UserInput.text)
        {
            if(!Char.IsLetterOrDigit(character))
            {
                validUsername = false;


                invalidChars = invalidChars + " " + character;

            }
        }

        //if there are no invalid characaters it loads the userlist data
        if (validUsername)
        {
            if (File.Exists(Application.persistentDataPath + "/UserList.dat"))
            {
                BinaryFormatter formatter = new BinaryFormatter();
                FileStream file = File.Open(Application.persistentDataPath + "/UserList.dat", FileMode.Open);

                UserData userData = (UserData)formatter.Deserialize(file);

                string[] users = userData.GetList();

                //if the list exists check for the enetered username to see if it already exists
                if (users != null)
                {
                    foreach (string user in users)
                    {
                        if (user == m_UserInput.text)
                        {
                            validUsername = false;
                        }
                    }
                }

                //if the name is valid add it to the dataset
                if (validUsername)
                {
                    userData.AddUser(m_UserInput.text);
                }
                else
                {
                    //otherwise give feedback to the user
                    m_FeedbackText.gameObject.SetActive(true);

                    m_FeedbackText.text = "Username already in use";

                }

                //cloase the file
                formatter.Serialize(file, userData);
                file.Close();

                //save the new data and reload the list                                 
                SaveUserList(userData);
                LoadUserList();

                Debug.Log("Added User");

            }
        }
        else
        {

            //other wise give the list of invalid characters the user entered
            m_FeedbackText.gameObject.SetActive(true);

            m_FeedbackText.text = invalidChars + " are invalid";
        }
    }

    //function to add each user to the drop down list
    public void UserDropDownInit()
    {
        m_UserDropDown.options.Clear();
        foreach(String userName in m_UserList)
        {
            Dropdown.OptionData t_UserOption = new Dropdown.OptionData(userName);
            m_UserDropDown.options.Add(t_UserOption);
        }
    }


    //transition to main menu and load user data
    public void UserSelectContinue()
    {
        LoadPlayerData(m_UserDropDown.captionText.text);
        Debug.Log("Loading user:" + m_UserDropDown.captionText.text);

        m_MainMenuCanvas.gameObject.SetActive(true);
        m_UserSelectionCanvas.gameObject.SetActive(false);

        m_tUsername.text = m_UserDropDown.captionText.text;

        m_sUsername = m_tUsername.text;
    }

    //return to the main menu function
    public void ReturnToMenu()
    {
        m_LeaderboardCanvas.gameObject.SetActive(false);

        m_LevelSelectCanvas.gameObject.SetActive(false);

        m_MainMenuCanvas.gameObject.SetActive(true);
    }

    //Transition to the leaderboards
    public void StatsScreen()
    {
        //set the canvases
        m_MainMenuCanvas.gameObject.SetActive(false);
        m_LeaderboardCanvas.gameObject.SetActive(true);

        //Set the leader board data
        SetLeaderBoards(0,m_LevelOneTimeTextElements,m_LevelOneDeathTextEntries);
        SetLeaderBoards(1,m_LevelTwoTimeTextElements,m_LevelTwoDeathTextEntries);
        SetLeaderBoards(2,m_LevelThreeTimeTextElements,m_LevelThreeDeathTextEntries);
        SetLeaderBoards(3,m_LevelFourTimeTextElements,m_LevelFourDeathTextEntries);
    }

    //function to go to the level select screen 
    public void LevelSelectScreen()
    {
        //if the scene is not the mainmenu 
        if (SceneManager.GetActiveScene().name != "MainMenu")
        {
            //change the game state to menu
            if(m_GameState != Game_State.MENU)
            {
                m_GameState = Game_State.MENU;
            }

            //load the main menu scene
            SceneManager.LoadScene("MainMenu");

            //add the after load function to the sceneloade event
            SceneManager.sceneLoaded += AfterLoadMenu;

        }
        else
        {
            //otherwise enable thecanvas 
            m_MainMenuCanvas.gameObject.SetActive(false);
            m_LevelSelectCanvas.gameObject.SetActive(true);

            //update the lock states
            UpdateLockStates();
        }
    }

    //function to quit from the pause screen
    public void QuitLevel()
    {
        //if the scene is not the menu
        if (SceneManager.GetActiveScene().name != "MainMenu")
        {
            //reset the death count and is paused to false
            m_iDeathCount = 0;

            m_bIsPaused = false;
            Time.timeScale = 1;

            //load the main menu with the after quit function added to the scene loaded event
            SceneManager.LoadScene("MainMenu");

            SceneManager.sceneLoaded += AfterQuit;
        }
    }

    //function for after quit to be added to the scene loaded event
    void AfterQuit(Scene a_Scene, LoadSceneMode a_mode)
    {
        //reinitialise the main menu elements
        InitialiseMenu();

        //set the game state to the main menu
        if (m_GameState != Game_State.MENU)
        {
            m_GameState = Game_State.MENU;
        }

        //update the lock states
        UpdateLockStates();

        //set the canvases
        m_UserSelectionCanvas.gameObject.SetActive(false);
        m_MainMenuCanvas.gameObject.SetActive(false);
        m_LevelSelectCanvas.gameObject.SetActive(true);

        //remove the function from the event
        SceneManager.sceneLoaded -= AfterQuit;

    }

    //function for load menu to be added to scene loaded event
    void AfterLoadMenu(Scene a_Scene, LoadSceneMode a_mode)
    {
        //initialise the menu
        InitialiseMenu();

        //update the lock states
        UpdateLockStates();

        //setup the canvases
        m_UserSelectionCanvas.gameObject.SetActive(false);
        m_MainMenuCanvas.gameObject.SetActive(false);
        m_LevelSelectCanvas.gameObject.SetActive(true);

        //save th eplayer data
        SavePlayerData(m_sUsername);

        //remove the function from the event
        SceneManager.sceneLoaded -= AfterLoadMenu;
    }

    //function to update the lock states of the levels
    public void UpdateLockStates()
    {
        
        //for each of the levels in the array of level data
        if (m_aLevels != null)
        {
            foreach (Level level in m_aLevels)
            {

                //if the level is unlocked then find the relevent buttons and disable the lock objects
                if (level.GetIsLevelUnlocked())
                {
                    switch (level.GetLevelName())
                    {
                        case "Level 1":

                            m_LevelOneButton.gameObject.SetActive(true);

                            break;
                        case "Level 2":

                            if (m_LevelTwoLockButton.gameObject.activeSelf)
                            {
                                m_LevelTwoLockButton.gameObject.SetActive(false);
                                m_LevelTwoButton.gameObject.SetActive(true);
                            }
                            break;
                        case "Level 3":

                            if (m_LevelThreeLockButton.gameObject.activeSelf)
                            {
                                m_LevelThreeLockButton.gameObject.SetActive(false);
                                m_LevelThreeButton.gameObject.SetActive(true);
                            }
                            break;
                        case "Level 4":
                            if (m_LevelFourLockButton.gameObject.activeSelf)
                            {
                                m_LevelFourLockButton.gameObject.SetActive(false);
                                m_LevelFourButton.gameObject.SetActive(true);
                            }
                            break;
                        default:
                            break;
                    }

                }
                //if unlocked for testing then all levels are unlocked
                else if (m_bUnlockedForTest)
                {
                    m_LevelOneButton.gameObject.SetActive(true);
                    m_LevelTwoLockButton.gameObject.SetActive(false);
                    m_LevelTwoButton.gameObject.SetActive(true);
                    m_LevelThreeLockButton.gameObject.SetActive(false);
                    m_LevelThreeButton.gameObject.SetActive(true);
                    m_LevelFourLockButton.gameObject.SetActive(false);
                    m_LevelFourButton.gameObject.SetActive(true);
                }
            }
        }
    }

    //user select screen transition
    public void UserSelectScreen()
    {
        m_MainMenuCanvas.gameObject.SetActive(false);
        m_UserSelectionCanvas.gameObject.SetActive(true);
    }

    //change state to level function
    public void ChangeState()
    {
        m_GameState = Game_State.LEVEL;
    }

    //function to load a users data from their .dat file
    public void LoadPlayerData(string a_sPlayerName)
    {
        if(File.Exists(Application.persistentDataPath + "/" + a_sPlayerName + ".dat"))
        {
            BinaryFormatter formatter = new BinaryFormatter();
            FileStream file = File.Open(Application.persistentDataPath + "/" + a_sPlayerName + ".dat", FileMode.Open);

            //if the file exists then deserialize the data and assign it to a temporary playerData
            PlayerData PlayData = (PlayerData)formatter.Deserialize(file);
            file.Close();

            //if the player name doesnt match the data players name save a new player.dat file and then try loading it again
            if(PlayData.GetPlayerName() != a_sPlayerName)
            {
                SavePlayerData(a_sPlayerName);
                LoadPlayerData(a_sPlayerName);
            }
            else
            {
                //else get the level data from the player file
                m_aLevels = PlayData.GetLevelData();

                //if the level name matches the current loaded level then set current level to that data set
                foreach(Level level in m_aLevels)
                {
                    if(level.GetLevelName() == SceneManager.GetActiveScene().name)
                    {
                        m_CurrentLevel = level;
                    }
                }
            }
        }
        else
        {
            //if the file doesnt exist create and load it
            SavePlayerData(a_sPlayerName);
            LoadPlayerData(a_sPlayerName);
        }
    }
    
    //save function for player data
    public void SavePlayerData(string a_sPlayerName)
    {
        
        BinaryFormatter formatter = new BinaryFormatter();

        
        FileStream file = File.Create(Application.persistentDataPath + "/" + a_sPlayerName + ".dat");

        //create a new data set for the player
        PlayerData PlayData = new PlayerData(a_sPlayerName,4);

        //in there is level data 
        if (m_aLevels != null)
        {
            //set the data name
            PlayData.SetPlayerName(a_sPlayerName);

            //get each level and set the current levels collectable data
            foreach(Level level in m_aLevels)
            {
                if (m_CurrentLevel != null)
                {
                    if (level.GetLevelName() == m_CurrentLevel.GetLevelName())
                    {
                        level.SetCollectablesData(m_LevelCollectables);
                    }
                }
            }

            //set the player data levels data to the current levels data of the game
            PlayData.SetLevelData(m_aLevels);
        }

        formatter.Serialize(file, PlayData);
        file.Close();
    }

    //function for loading leader boards
    public void LoadLevelLeaderboards()
    {
        //for each leader board
        for (int i = 1; i < 5; ++i)
        {

            //open that leaderboards file if it exists
            if (File.Exists(Application.persistentDataPath + "/Level" + i + "Boards.dat"))
            {
                BinaryFormatter formatter = new BinaryFormatter();
                FileStream file = File.Open(Application.persistentDataPath + "/Level" + i + "Boards.dat", FileMode.Open);


                LeaderboardData BoardData = (LeaderboardData)formatter.Deserialize(file);
                file.Close();

                //set the array object to the board data
                for (int j = 0; j < 4; ++j)
                {
                    if (j == i - 1)
                    {
                        if (BoardData.GetBoardLevel() == "Level " + i)
                        {
                            m_LevelBoards[j] = BoardData;
                        }
                    }
                }
            }
            else
            {
                //if the file doesnt exist create is and load again
                SaveLevelLeaderboards();
                LoadLevelLeaderboards();
                
            }   
        }

        //set up the leader boards
        SetLeaderBoards(0, m_LevelOneTimeTextElements,m_LevelOneDeathTextEntries);
        SetLeaderBoards(1, m_LevelTwoTimeTextElements, m_LevelTwoDeathTextEntries);
        SetLeaderBoards(2, m_LevelThreeTimeTextElements, m_LevelThreeDeathTextEntries);
        SetLeaderBoards(3, m_LevelFourTimeTextElements, m_LevelFourDeathTextEntries);
    }

    //function to save leaderboard data
    public void SaveLevelLeaderboards()
    {
        for (int i = 1; i < 5; ++i)
        {

            BinaryFormatter formatter = new BinaryFormatter();


            FileStream file = File.Create(Application.persistentDataPath + "/Level" + i + "Boards.dat");

            //get the leaderboard data from file
            LeaderboardData BoardData = new LeaderboardData("Level " + i);


            if(m_LevelBoards != null)
            {
                foreach(LeaderboardData data in m_LevelBoards)
                {
                    if(data.GetBoardLevel() == BoardData.GetBoardLevel())
                    {
                        BoardData = data;
                    }
                }
            }

            formatter.Serialize(file, BoardData);
            file.Close();
        }
    }

    //find and assign the leaderboard elements in the main menu scene
    public void FindLeaderboardObjects()
    {
        //get the best times pages
        m_LevelOneHolder = GameObject.Find("Level_1_BestTimes");
        m_LevelTwoHolder = GameObject.Find("Level_2_BestTimes");
        m_LevelThreeHolder = GameObject.Find("Level_3_BestTimes");
        m_LevelFourHolder = GameObject.Find("Level_4_BestTimes");

        m_LevelOneDeathHolder = GameObject.Find("Level_1_BestDeaths");
        m_LevelTwoDeathHolder = GameObject.Find("Level_2_BestDeaths"); ;
        m_LevelThreeDeathHolder = GameObject.Find("Level_3_BestDeaths"); ;
        m_LevelFourDeathHolder = GameObject.Find("Level_4_BestDeaths"); ;

        //find the child objects for each holder page
        foreach (Transform child in m_LevelOneHolder.transform)
        {
            if (child.name == "Entries")
            {
                m_LevelOneEntries = child.gameObject;
            }
        }
        foreach (Transform child in m_LevelTwoHolder.transform)
        {
            if (child.name == "Entries")
            {
                m_LevelTwoEntries = child.gameObject;
            }
        }
        foreach (Transform child in m_LevelThreeHolder.transform)
        {
            if (child.name == "Entries")
            {
                m_LevelThreeEntries = child.gameObject;
            }
        }
        foreach (Transform child in m_LevelFourHolder.transform)
        {
            if (child.name == "Entries")
            {
                m_LevelFourEntries = child.gameObject;
            }
        }

        foreach (Transform child in m_LevelOneDeathHolder.transform)
        {
            if (child.name == "Entries")
            {
                m_LevelOneDeathEntries = child.gameObject;
            }
        }
        foreach (Transform child in m_LevelTwoDeathHolder.transform)
        {
            if (child.name == "Entries")
            {
                m_LevelTwoDeathEntries = child.gameObject;
            }
        }
        foreach (Transform child in m_LevelThreeDeathHolder.transform)
        {
            if (child.name == "Entries")
            {
                m_LevelThreeDeathEntries = child.gameObject;
            }
        }
        foreach (Transform child in m_LevelFourDeathHolder.transform)
        {
            if (child.name == "Entries")
            {
                m_LevelFourDeathEntries = child.gameObject;
            }
        }


        //initialise the arrays for the text objects
        m_LevelOneTimeTextElements = new Text[10];
        m_LevelTwoTimeTextElements = new Text[10];
        m_LevelThreeTimeTextElements = new Text[10];
        m_LevelFourTimeTextElements = new Text[10];

        m_LevelOneDeathTextEntries = new Text[10];
        m_LevelTwoDeathTextEntries = new Text[10];
        m_LevelThreeDeathTextEntries = new Text[10];
        m_LevelFourDeathTextEntries = new Text[10];

        //assign the text elements to the array
        InitLeaderBoard(m_LevelOneEntries, m_LevelOneTimeTextElements);
        InitLeaderBoard(m_LevelTwoEntries, m_LevelTwoTimeTextElements);
        InitLeaderBoard(m_LevelThreeEntries, m_LevelThreeTimeTextElements);
        InitLeaderBoard(m_LevelFourEntries, m_LevelFourTimeTextElements);

        InitLeaderBoard(m_LevelOneDeathEntries,m_LevelOneDeathTextEntries);
        InitLeaderBoard(m_LevelTwoDeathEntries,m_LevelTwoDeathTextEntries);
        InitLeaderBoard(m_LevelThreeDeathEntries,m_LevelThreeDeathTextEntries);
        InitLeaderBoard(m_LevelFourDeathEntries,m_LevelFourDeathTextEntries);

        m_LeaderboardCanvas.gameObject.SetActive(false);
    }

    //function to enter each text element from each entry in a leaderboard
    public void InitLeaderBoard(GameObject a_EntriesParent, Text[] a_TextElements)
    {
        int i = 0;

        foreach (Transform child in a_EntriesParent.transform)
        {
            a_TextElements[i] = child.GetComponent<Text>();
            i++;

            if (i > 9)
            {
                break;
            }
        }
    }

    //function to get the set the text objects of the leader board
    public void SetLeaderBoards(int index, Text[] a_TimeEntries, Text[] a_DeathEntries)
    {
        //get the array of level runs from using the index to get each leaderboard
        LevelRun[] t_TimeLeaders = m_LevelBoards[index].GetBestTimeRuns();
        LevelRun[] t_DeathLeaders = m_LevelBoards[index].GetBestTimeRuns();

        //get the dats for each of the best times and deaths of this level
        for (int j = 0; j < a_TimeEntries.Length; ++j)
        {
            string t_username = t_TimeLeaders[j].GetUserName();
            TimeSpan t_timeTaken = t_TimeLeaders[j].GetTimeTaken();
            int t_deaths = t_TimeLeaders[j].GetDeaths();

            a_TimeEntries[j].text = (j + 1) + ". " + t_username + " - " + t_timeTaken.Minutes + ":" + t_timeTaken.Seconds.ToString("D2") + " - " + t_deaths;

             t_username = t_DeathLeaders[j].GetUserName();
             t_timeTaken = t_DeathLeaders[j].GetTimeTaken();
             t_deaths = t_DeathLeaders[j].GetDeaths();

            a_DeathEntries[j].text = (j + 1) + ". " + t_username + " - " + t_deaths + " - " + t_timeTaken.Minutes + ":" + t_timeTaken.Seconds.ToString("D2");
        }



    }

    //function to check the newest run against previous runs of a level
    public void CheckNewRun(LevelRun a_NewRun, Level a_Level)
    {
        //if the level name matches the leaderboard level name
        foreach(LeaderboardData data in m_LevelBoards)
        {
            if(data.GetBoardLevel() == a_Level.GetLevelName())
            {
                //call the check times and check deaths functions for the latest run
                data.CheckBestTimeRuns(a_NewRun);
                data.CheckBestDeathRuns(a_NewRun);
            }
        }

        SaveLevelLeaderboards();
    }

    //function for jump respawns
    public void JumpRespawn()
    {
        //for each of the jump boosts if it is inactive start the respawn timer
        foreach (GameObject Jump in m_aExtraJumps)
        {
            ExtraJump t_ExtraJumpScript = Jump.GetComponent<ExtraJump>();

            if (t_ExtraJumpScript != null)
            {
                if (!t_ExtraJumpScript.GetRespawnStarted() && !t_ExtraJumpScript.GetIsActive())
                {
                    t_ExtraJumpScript.StartRespawnTimer(m_fJumpRespawnTime);
                }
            }
        }
    }

    //get the currentgame level
    public Level GetCurrentLevel()
    {
        return m_CurrentLevel;
    }
    //get the rooms of the current level
    public Room[] GetLevelRooms()
    {
        return m_aLevelRooms;
    }

    //for when the level boss is defeated pause and set up the end screen showing the time and deaths during that run
    public void LevelCompleteOverlay()
    {
        Time.timeScale = 0;

        m_GameOverlay.gameObject.SetActive(false);
        m_LevelComplete.gameObject.SetActive(true);

        m_FinalTimeText.text = "Final time: " + m_LevelTime.Minutes + ":" + m_LevelTime.Seconds.ToString("D2");
        m_FinalDeathText.text = "Final Deaths: " + m_iDeathCount;


        //enter LevelComplete state to return to menu
        m_GameState = Game_State.LEVELCOMPLETE;
    }

    //function to unlock the next level
    public void UnlockNextLevel(Level a_currentLevel)
    {
        //call the levels level complete function
        a_currentLevel.LevelCompleted(m_iDeathCount, m_LevelTime);

        //setup a most recent run
        LevelRun t_MostRecentRun = new LevelRun();

        //set the values from the manager
        t_MostRecentRun.SetTimeTaken(m_LevelTime);
        t_MostRecentRun.SetDeaths(m_iDeathCount);
        t_MostRecentRun.SetUserName(m_sUsername);

        //check the most recent run against previous
        CheckNewRun(t_MostRecentRun,a_currentLevel);

        //reset the death counter
        m_iDeathCount = 0;

        //find the next level in the level array
        for (int i = 1; i <= m_aLevels.Length; ++i)
        {
            if(a_currentLevel.GetLevelName() == "Level " + i)
            {

                foreach (Level level in m_aLevels)
                { 

                    if(level.GetLevelName() == "Level " + (i+1))
                    {
                        //set the level to unlocked
                        level.SetIsLevelUnlocked(true);

                        //save the players data progression
                        SavePlayerData(m_sUsername);
                    }
                }
            }
        }

    }

    //serializable struct for player data
    [Serializable]
    struct PlayerData
    {
        //username
        private String m_sPlayerName;
        //the level data for this user
        private Level[] m_aLevels;

        //constructor taking a name and level count
        public PlayerData(string a_sPlayerName, int a_iLevelCount)
        {
            //assign the name and initialise the level array
            m_sPlayerName = a_sPlayerName;
            m_aLevels = new Level[a_iLevelCount];

            //for each of the array elements create a new level data set with the name of the level
            for(int i = 0; i < m_aLevels.Length;++i)
            {
                m_aLevels[i] = new Level();
                m_aLevels[i].SetLevelName("Level " + (i + 1));
            }

        }

        //set and get player name
        public void SetPlayerName(string a_sPlayerName)
        {
            m_sPlayerName = a_sPlayerName;
        }
        public string GetPlayerName()
        {
            return m_sPlayerName;
        }

        //get and set level array
        public void SetLevelData(Level[] a_aLevels)
        {
            m_aLevels = a_aLevels;
        }
        public Level[] GetLevelData()
        {
            return m_aLevels;
        }
        
    }

    //serializable struct for the users list
    [Serializable]
    struct UserData
    {
        //list of usersnames
        private String[] m_UserList;
        //username count
        private int m_UserCount;

        //function to add a user
        public void AddUser(String a_sUserName)
        {    
            //if the list is not uninitialised
            if (m_UserList != null)
            {

                //check for the username
                int MatchedUsers = 0;

                foreach (string userName in m_UserList)
                {
                    if (userName == a_sUserName)
                    {
                        MatchedUsers++;
                        break;
                    }
                }

                //if no matching user name is found
                if (MatchedUsers == 0)
                {       
                    //create a new username array of the previous length plus 1
                    String[] t_NewUserList = new String[m_UserList.Length + 1];

                    for (int i = 0; i < t_NewUserList.Length; ++i)
                    {

                        //add the new username to the new list element
                        if (i == t_NewUserList.Length - 1)
                        {
                            t_NewUserList[i] = a_sUserName;
                            m_UserCount = i + 1;
                        }
                        else
                        {
                            t_NewUserList[i] = m_UserList[i];
                        }
                    }

                    m_UserList = t_NewUserList;
                } 
            }
            else
            {
                //else create and initialise a new list
                m_UserList = new String[1];

                m_UserList[0] = a_sUserName;
                m_UserCount = 1;

            }
        }

        //getters for the count and the array of usernames
        public int GetCount()
        {
            return m_UserCount;
        }
        public String[] GetList()
        {
            return m_UserList;
        }
    }

    //serializable struct for leaderboard data
    [Serializable]
    struct LeaderboardData
    {
        //this boards associated level name
        private string m_sLevelName;

        //arrays for the best timed runs and the best death runs
        private LevelRun[] m_aBestRunsTime;
        private LevelRun[] m_aBestRunsDeath;


        //constructor for a level leaderboard
        public LeaderboardData(string a_sLevelName)
        {
            m_sLevelName = a_sLevelName;

            //initialise the arrays for a top ten
            m_aBestRunsTime = new LevelRun[10];
            m_aBestRunsDeath = new LevelRun[10];

            //create empty run data to fill the arrays
            for(int i = 0; i < m_aBestRunsTime.Length; ++i)
            {
                m_aBestRunsTime[i] = new LevelRun();
                m_aBestRunsDeath[i] = new LevelRun();
            }
        }
         
        //function to test for the best timed run
        public void CheckBestTimeRuns(LevelRun a_NewRun)
        {
            //index too replace at
            int ReplaceIndex = 10;

            //go through the array if there is no username then the slot is empty so set the replace index there 
            for(int i = 0; i < m_aBestRunsTime.Length;++i)
            {
                if (m_aBestRunsTime[i].GetUserName() != null)
                {
                    //if the list is full check the time to see if it beats the current indexed element
                    //if it does set the replace index to the current loop index
                    if (m_aBestRunsTime[i].GetTimeTaken() > a_NewRun.GetTimeTaken())
                    {
                        ReplaceIndex = i;
                        break;
                    }
                }
                else
                {
                    ReplaceIndex = i;
                    break;
                }
            }

            //if the replace index is still ten then the run didnt make the leaderboard
            if (ReplaceIndex != 10)
            {
                //starting from the end of the array moving up to the replace index
                for (int i = m_aBestRunsTime.Length - 1; i >= ReplaceIndex; --i)
                {

                    if (i != ReplaceIndex)
                    {
                        //if i isnt the replace index then the current element is set to the element above 
                        m_aBestRunsTime[i] = m_aBestRunsTime[i - 1];
                    }
                    else
                    {
                        //when i does equal the replace index then insert the new run here
                        m_aBestRunsTime[i] = a_NewRun;
                    }

                }
            }
        }
        //this does the same but checking the death board not times
        public void CheckBestDeathRuns(LevelRun a_NewRun)
        {
            int ReplaceIndex = 10;

            for (int i = 0; i < m_aBestRunsDeath.Length; ++i)
            {
                if (m_aBestRunsDeath[i].GetUserName() != null)
                {
                    if (m_aBestRunsDeath[i].GetDeaths() > a_NewRun.GetDeaths())
                    {
                        ReplaceIndex = i;
                        break;
                    }
                }
                else
                {
                    ReplaceIndex = i;
                    break;
                }
            }

            if (ReplaceIndex != 10)
            {
                for (int i = m_aBestRunsDeath.Length - 1; i >= ReplaceIndex; --i)
                {

                    if (i != ReplaceIndex)
                    {
                        m_aBestRunsDeath[i] = m_aBestRunsDeath[i - 1];
                    }
                    else
                    {
                        m_aBestRunsDeath[i] = a_NewRun;
                    }

                }
            }
        }

        //get the run arrays
        public LevelRun[] GetBestTimeRuns()
        {
            return m_aBestRunsTime;
        }
        public LevelRun[] GetBestDeathRuns()
        {
            return m_aBestRunsDeath;
        }

        //set and get the boards level name
        public void SetBoardLevel(string a_LevelName)
        {
            m_sLevelName = a_LevelName;
        }
        public string GetBoardLevel()
        {
            return m_sLevelName;
        }
    }



}
