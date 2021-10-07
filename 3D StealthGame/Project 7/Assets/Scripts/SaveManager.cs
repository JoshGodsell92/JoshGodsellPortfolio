using System.IO;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using Newtonsoft.Json; //(parentElement LLC, 2017)

[System.Serializable]
public class SaveData
{
    public int iCurrentLevelID;
    public Vector3 v3PlayerPosition;
    public Vector3 v3PlayerRotEulers;
    public Vector3[] v3AIPositions;
    public Vector3[] v3AIRotEulers;
    //get player values (crouching, under an object, hiding, etc.)
    public bool bPlayerCrouching;
    //get AI values
    //get player inventory
    public InventoryData inventoryData;
    public int iKeyTotal;
    public int iBigKeyTotal;
    public bool bDoorOpen;
    public bool bHasFoundDoor;
}

public class SaveManager : Singleton<SaveManager>
{
    [SerializeField] LevelDetails levelDetails;
    [SerializeField] PlayerController player;
    public SaveData saveData;
    string strSaveDir;

    // Start is called before the first frame update
    void Start()
    {
        saveData = new SaveData();
    }

    void Awake()
    {
        strSaveDir = Application.persistentDataPath + "/savedata/";
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void LoadGame()
    {
        SceneManager.sceneLoaded += LoadSave;
        SceneManager.LoadScene(1);
    }

    public void LoadSave(Scene a_Scene, LoadSceneMode a_Mode)
    {
        if (levelDetails == null)
        {
            levelDetails = Component.FindObjectOfType<LevelDetails>();
        }
        if (player == null)
        {
            player = GameObject.FindObjectOfType<PlayerController>();
        }
        if (player.GetInventory() == null)
        {
            player.SetInventory(player.GetComponent<Inventory>());
        }

        Debug.Log("Loading");
        string dataAsJson = File.ReadAllText(Application.persistentDataPath + "/savedata/savedGame.json");

        SaveData loadedData = JsonConvert.DeserializeObject<SaveData>(dataAsJson, new JsonSerializerSettings() { ReferenceLoopHandling = ReferenceLoopHandling.Serialize, Formatting = Formatting.Indented });

        saveData = loadedData;
        Debug.Log("Loaded data");

        //set values based on imported data

        //player values
        levelDetails.GetPlayer().transform.position = saveData.v3PlayerPosition;
        levelDetails.GetPlayer().transform.rotation = Quaternion.Euler(saveData.v3PlayerRotEulers);
        player.GetInventory().ImportInventory(saveData.inventoryData);
        if (saveData.bPlayerCrouching)
        {
            player.Crouch();
        }
        GameObject[] AIs = levelDetails.GetAIs();
        for (int i = 0; i < AIs.Length; i++)
        {
            AIs[i].transform.position = saveData.v3AIPositions[i];
            AIs[i].transform.rotation = Quaternion.Euler(saveData.v3AIRotEulers[i]);
        }

        //Not saving if I got the key??
        levelDetails.SetKeys(saveData.iKeyTotal);
        levelDetails.SetDoorOpen(saveData.bDoorOpen);
        levelDetails.SetHasFoundDoor(saveData.bHasFoundDoor);

        Debug.Log("Data imported");
        SceneManager.sceneLoaded -= LoadSave;
    }

    public void SaveGame()
    {
        if (player.GetIsHiding())
        {
            Debug.Log("Player cannot save while hiding"); //look into this
        }
        else
        {
            Debug.Log("Updating save data");
            //store player
            saveData.iCurrentLevelID = SceneManager.GetActiveScene().buildIndex;
            saveData.v3PlayerPosition = levelDetails.GetPlayer().transform.position;
            saveData.v3PlayerRotEulers = levelDetails.GetPlayer().transform.rotation.eulerAngles;
            saveData.inventoryData = player.GetInventory().ExportInventory();
            saveData.bPlayerCrouching = player.GetIsCrouched();

            GameObject[] AIs = levelDetails.GetAIs();
            saveData.v3AIPositions = new Vector3[AIs.Length];
            saveData.v3AIRotEulers = new Vector3[AIs.Length];

            for (int i = 0; i < AIs.Length; i++)
            {
                saveData.v3AIPositions[i] = AIs[i].transform.position;
                saveData.v3AIRotEulers[i] = AIs[i].transform.rotation.eulerAngles;
            }

            saveData.iKeyTotal = levelDetails.GetKeys();
            saveData.iBigKeyTotal = levelDetails.GetBigKey();
            saveData.bDoorOpen = levelDetails.GetDoorOpen();
            saveData.bHasFoundDoor = levelDetails.GetHasFoundDoor();


            Debug.Log("Saving data");
            WriteFile();
            Debug.Log("Data saved");
        }
    }

    public void WriteFile()
    {
        string result = JsonConvert.SerializeObject(saveData, new JsonSerializerSettings() { ReferenceLoopHandling = ReferenceLoopHandling.Serialize, Formatting = Formatting.Indented });
        if (!Directory.Exists(strSaveDir))
        {
            Directory.CreateDirectory(strSaveDir);
        }
        Debug.Log("Saving: " + result);
        string fileName = "savedGame";
        string filePath = strSaveDir + fileName + ".json";
        File.WriteAllText(filePath, result);
        StreamWriter newTask = new StreamWriter(strSaveDir + fileName + ".txt", false);
        newTask.WriteLine(System.DateTime.Now.ToString("HH:mm dd MMMM, yyyy"));
        newTask.Close();
    }
}
