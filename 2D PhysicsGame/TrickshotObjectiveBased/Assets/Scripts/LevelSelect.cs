using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class LevelSelect : MonoBehaviour {

    //A bool that I will use to check if you can go to this level. - KT
    bool bAllowLoad = true;

	// Use this for initialization
	void Start ()
    {
        //A bool that I will use to check if you can go to this level. - KT
        int LevelName = int.Parse(this.gameObject.name);
        GameObject GameControl = GameObject.FindGameObjectWithTag("GameControl");

        //A bool that is used to store the level stars. - KT
        bool[] LevelStars = new bool[4];
        LevelStars = GameControl.GetComponent<GameControl>().GetLevelStars(LevelName);

        //Check if the level is able to be loaded. If it can't be then it can set the colour of the object will be set to red. - KT
        if (!LevelStars[0] && this.gameObject.name != "1")
        {
            this.gameObject.GetComponent<Image>().color = Color.red;
            bAllowLoad = false;
        }

        //Check if the objectives have been completed on the objective. - KT
        if (!LevelStars[1])
        {
            try
            {
                this.gameObject.transform.GetChild(2).GetComponent<SpriteRenderer>().color = Color.grey;
            }
            catch
            {

            } 
        }
        if (!LevelStars[2])
        {
            try
            {
                this.gameObject.transform.GetChild(3).GetComponent<SpriteRenderer>().color = Color.grey;
            }
            catch
            {

            }
            
        }
        if (!LevelStars[3])
        {
            try
            {
                this.gameObject.transform.GetChild(4).GetComponent<SpriteRenderer>().color = Color.grey;
            }
            catch
            {

            } 
        }
    }
	
	// Update is called once per frame
	void Update () {
		
	}

    //If the level can be loaded and the button has been clicked then the level will loaded. - KT
    public void LoadLevel()
    {
        if (bAllowLoad)
        {
            int LevelName = int.Parse(this.gameObject.name);
            GameObject GC = GameObject.FindGameObjectWithTag("GameControl");
            GameControl GCScript = GC.GetComponent<GameControl>();
            GCScript.SetLevelToLoad(LevelName);
            SceneManager.LoadScene("ChangeBallType");
        }
    }

    //Restart the level. - KT
    public void Restart()
    {
        string thisLevel = SceneManager.GetActiveScene().name;
        SceneManager.LoadScene(thisLevel);
        Time.timeScale = 1.0f;
    }
}
