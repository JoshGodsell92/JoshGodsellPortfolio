using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class StartGame : MonoBehaviour {

    //Some instances of the buttons, canvas' and game objects - KT
    private GameObject SoundOffButton;
    private GameObject SoundOnButton;
    public Canvas LevelSelect;
    public Button StartButton;
    public GameObject GameControlObj;
    public Button Customise;

    //Some instances of the different chapters - KT
    public GameObject Tutorial;
    public GameObject Chapter1;
    public GameObject Chapter2;

    //Some variables for the level select - KT
    private int LevelSelectZone = 0; //1 = Tutorial 2 = Chapter1

    private bool bLevelSelect = false;

    //A bool to mute the sound - KT
    private bool SoundMuted = false;

    //Twitter Adresses and message
    private string TwitterAddress = "https://twitter.com/intent/tweet";
    private string TwitterMessage = "I've downloaded @TrickshotUoG !";
    private string TwitterLink = "https://twitter.com/TrickShotUoG";

    // Use this for initialization
    void Start () {
        //Check the sound button - KT
        SoundOffButton = GameObject.Find("SoundOff");
        SoundOnButton = GameObject.Find("SoundOn");
        SoundOffButton.SetActive(false);
        LevelSelect.gameObject.SetActive(false);

        //Check if there is a game control and if there is then destroy it - KT
        if (GameObject.Find("Game Control"))
        {
            Destroy(GameObject.Find("GameControl"));
        }
	}
	
	// Update is called once per frame
	void Update () {
        //Once any button has been pressed then it will change the name of the game control to make sure another one can not be created - KT
		if (Input.anyKey)
        {
            try
            {
                GameObject.Find("GameControl").name = "Game Control";
            }
            catch
            {

            }
        }
	}

    public void LoadLevel(string name)
    {
        SceneManager.LoadScene(name);
    }

    public void TurnSoundOff()
    {
        SoundOnButton.SetActive(false);
        SoundOffButton.SetActive(true);
        AudioListener.pause = true;
    }

    public void TurnSoundOn()
    {
        SoundOnButton.SetActive(true);
        SoundOffButton.SetActive(false);
        AudioListener.pause = false;
    }

    public void Twitter()
    {
        Application.OpenURL(TwitterAddress + "?text=" + WWW.EscapeURL(TwitterMessage));
    }

    public void Facebook()
    {
        Application.OpenURL("https://www.facebook.com/TrickShot-129042007956471");
    }

    //Load the level select - KT
    public void LoadLevelSelect()
    {
        LevelSelect.gameObject.SetActive(true);
        StartButton.gameObject.SetActive(false);
        Customise.gameObject.SetActive(false);
        bLevelSelect = true;
        LevelSelectZone = 1;
    }

    public void LoadCustomise()
    {
        SceneManager.LoadScene("Customise ball");
    }
}
