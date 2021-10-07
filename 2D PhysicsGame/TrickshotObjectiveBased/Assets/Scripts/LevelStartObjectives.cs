using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class LevelStartObjectives : MonoBehaviour {
    GameObject goTextGameObject;

    private bool bLevelStart = true;

    // Use this for initialization
    void Start () {
        GameObject goGameController = GameObject.FindGameObjectWithTag("GameControl");
        GameControl gcGameControlScript = goGameController.GetComponent<GameControl>();
        gcGameControlScript.Reset();
        int iSceneNumber = int.Parse(SceneManager.GetActiveScene().name);
        GameObject.Find("LevelObjectives").GetComponent<Text>().text = "Level " + iSceneNumber + " Objectives";
        //GCScript.InitGame(SceneNumber);
        bool[] CompletedLevels = new bool[4];
        CompletedLevels = gcGameControlScript.GetLevelStars(iSceneNumber);

        if (iSceneNumber > 4)
        {
            for (int i = 1; i < 4; i++)
            {
                Debug.Log("Objective" + i + ": " + CompletedLevels[i]);
                if (!CompletedLevels[i])
                {
                    GameObject.Find("Star" + i.ToString()).GetComponent<Image>().color = Color.grey;
                }
                //Debug.Log(CompletedLevels[i]);

            }
            gcGameControlScript.SetRequirement();
            goTextGameObject = GameObject.Find("Objective1");
            goTextGameObject.GetComponent<Text>().text = gcGameControlScript.GetLevelRequirement(1);
            goTextGameObject = GameObject.Find("Objective2");
            goTextGameObject.GetComponent<Text>().text = gcGameControlScript.GetLevelRequirement(2);
            goTextGameObject = GameObject.Find("Objective3");
            goTextGameObject.GetComponent<Text>().text = gcGameControlScript.GetLevelRequirement(3);
        }
        else if (iSceneNumber == 1)
        {

        }
        else
        {
            if (!CompletedLevels[1])
            {
                GameObject.Find("Star1").GetComponent<Image>().color = Color.grey;
            }
            gcGameControlScript.SetRequirement();
            goTextGameObject = GameObject.Find("Objective1");
            goTextGameObject.GetComponent<Text>().text = gcGameControlScript.GetLevelRequirement(1);
        }
        //Debug.Log(GCScript.GetLevelRequirement(1));
        
        goTextGameObject = GameObject.FindGameObjectWithTag("ComboObj");
        goTextGameObject.GetComponent<Text>().text = gcGameControlScript.GetCombo().ToString();
        goTextGameObject = GameObject.FindGameObjectWithTag("ShotObj");
        goTextGameObject.GetComponent<Text>().text = gcGameControlScript.getShotCount().ToString();
    }
	
	// Update is called once per frame
	void Update () {
        if (bLevelStart)
        {
            if (Input.anyKey)
            {
                Time.timeScale = 1.0f;
                this.gameObject.SetActive(false);
                bLevelStart = false;

                GameObject.FindGameObjectWithTag("Projectile").GetComponent<ProjectileDrag>().bStart = false;

                GameObject.FindGameObjectWithTag("GameControl").GetComponent<GameControl>().Reset();

            }
        }		
	}

    public void ReloadScreen()
    {
        Time.timeScale = 0.0f;
        GameObject GC = GameObject.FindGameObjectWithTag("GameControl");
        GameControl GCScript = GC.GetComponent<GameControl>();
        int SceneNumber = int.Parse(SceneManager.GetActiveScene().name);
        bool[] CompletedLevels = new bool[4];
        CompletedLevels = GCScript.GetLevelStars(SceneNumber);
        if (SceneNumber > 4)
        {
            for (int i = 1; i < 4; i++)
            {
                Debug.Log("Objective" + i + ": " + CompletedLevels[i]);
                if (!CompletedLevels[i])
                {
                    GameObject.Find("Star" + i.ToString()).GetComponent<Image>().color = Color.grey;
                }
                else if (CompletedLevels[i])
                {
                    GameObject.Find("Star" + i.ToString()).GetComponent<Image>().color = Color.yellow;
                }
            }
        }
        else if (SceneNumber == 1)
        {

        }
        else
        {
            Debug.Log(CompletedLevels[1]);
            if (!CompletedLevels[1])
            {
                GameObject.Find("Star1").GetComponent<Image>().color = Color.grey;
            }
            else
            {
                GameObject.Find("Star1").GetComponent<Image>().color = Color.yellow;
            }
        }
        
        goTextGameObject = GameObject.FindGameObjectWithTag("ComboObj");
        goTextGameObject.GetComponent<Text>().text = GCScript.GetCombo().ToString();
        goTextGameObject = GameObject.FindGameObjectWithTag("ShotObj");
        goTextGameObject.GetComponent<Text>().text = GCScript.getShotCount().ToString();
    }

    public void SetStart(bool a_bStart)
    {
        bLevelStart = a_bStart;
    }

    public bool GetStart()
    {
        return bLevelStart;
    }
}
