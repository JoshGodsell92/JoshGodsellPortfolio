using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using UnityEngine.Analytics;

public class SinglePlayerLevelEnd : MonoBehaviour {

    //Some variables for the game control, player and objectives - KT
    public string NextLevel;

    private GameObject Player;
    private GameObject GC;
    private GameControl GCScript;
    private ObjectiveScript[] ObjectiveScripts; 
    public GameObject ObjectivesCanvas;

    public AudioSource Win;


    private bool LevelEnded = false;

    int SceneNumber = 0;

    // Use this for initialization
    void Start () {
        //Get Objective scripts and the game control scripts - KT
        GC = GameObject.FindGameObjectWithTag("GameControl");
        GCScript = GC.GetComponent<GameControl>();
        ObjectiveScripts = FindObjectsOfType<ObjectiveScript>();
    }
	
	// Update is called once per frame
	void Update () {
		if (LevelEnded)
        {
            int currentScene = SceneNumber;
            if (Input.anyKey)
            {
                Time.timeScale = 1.0f;

                //Load the correct scene depending on chapter - KT
                currentScene--;
                if (currentScene == 4)
                {
                    SceneManager.LoadScene("CompleteTutorial");
                }
                else if (currentScene == 12)
                {
                    
                    SceneManager.LoadScene("CompleteChapter1");
                    
                }
                else if (currentScene == 20)
                {
                    SceneManager.LoadScene("CompleteChapter2");
                }
                else
                {
                    SceneManager.LoadScene((SceneNumber.ToString()));
                }
                
            }
        }
	}

    //If the projectile collides with this the level will end - KT
    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.gameObject.CompareTag("Projectile"))
        {
            //Play the audio for getting the ball in goal - KT
            AudioSource Win = GetComponent<AudioSource>();
            Win.Play();

            //Test all the objectives - KT
            for (int i = 0; i < ObjectiveScripts.Length; i++)
            {
                ObjectiveScripts[i].testObjective();
            }

            //Refresh the objectives canvas - KT
            ObjectivesCanvas.SetActive(true);
            ObjectivesCanvas.GetComponent<LevelStartObjectives>().ReloadScreen();
            
            //Reset everything - KT
            GCScript = GC.GetComponent<GameControl>();
            GCScript.Scored();

            //Get the scene number - KT
            Scene scene = SceneManager.GetActiveScene();
            SceneNumber = int.Parse(scene.name);
            GameObject.Find("LevelObjectives").GetComponent<Text>().text = "Level " + SceneNumber + " Completed!";

            //A custom event that will display on the Unity Dashboard that shows how many shots someone has taken on a specfic level - KT
            Analytics.CustomEvent("Shots taken on level" + SceneNumber + ": " + GCScript.getShotCount());

            SceneNumber++;
            GCScript.UpdateObjective(SceneNumber, 0, true);

            Handheld.Vibrate();

            GCScript.bX2combo = false;

            LevelEnded = true;
        }
    }

    //Some functions to load the completed tutorial scenes - KT
    public void CompleteTutorial()
    {
        SceneManager.LoadScene("5");
    }

    public void CompleteChapterOne()
    {
        SceneManager.LoadScene("13");
    }

    public void CompleteChapterTwo()
    {
        SceneManager.LoadScene("Menu");
    }
}
