using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class BallSelect : MonoBehaviour {

    //Some private instances of the game control object, the game control script and the project game object. - KT
    private GameObject goGameController;
    private GameControl gcGameControlScript;

    private GameObject goProjectile;

    //The ball type text - KT
    public Text tBallTypeText;

    //The ball type name string - KT
    public string sBallTypeName;

    public int iBallTypeNumber = 0; //0 = Default, 1 = Bouncy and 2 = Bean

    public Sprite sDefaultSprite;
    public Sprite sBouncyBallSprite;
    public Sprite sBeanBagSprite;
    public Sprite sFloatyBallSprite;

    public Image imBallImage;

	// Use this for initialization
	void Start () {
        //Get the current ball name - KT
        goGameController = GameObject.FindGameObjectWithTag("GameControl");
        gcGameControlScript = goGameController.GetComponent<GameControl>();
        sBallTypeName = gcGameControlScript.GetBallTypeName();
        goProjectile = GameObject.Find("Projectile");


        //A switch statement to check the name of the ball and change the sprite and number depending on it. - KT
        switch (sBallTypeName)
        {
            case "Default":
                tBallTypeText.text = "Basketball";
                imBallImage.sprite = sDefaultSprite;
                iBallTypeNumber = 0;
                break;
            case "BouncyBall":
                tBallTypeText.text = "Bouncy Ball";
                imBallImage.sprite = sBouncyBallSprite;
                iBallTypeNumber = 1;
                break;
            case "BeanBag":
                tBallTypeText.text = "Bean Bag";
                imBallImage.sprite = sBeanBagSprite;
                iBallTypeNumber = 2;
                break;
            case "FloatyBall":
                tBallTypeText.text = "Floaty Ball";
                imBallImage.sprite = sFloatyBallSprite;
                iBallTypeNumber = 3;
                break;
            default:
                tBallTypeText.text = "Default";
                imBallImage.sprite = sDefaultSprite;
                iBallTypeNumber = 0;
                break;
        }

        //Check the ball type number
        CheckBallTypeNumber();

		
	}
	
	// Update is called once per frame
	void Update () {
		
	}

    //Decrease the ball type number - KT
    public void CycleLeft()
    {
        iBallTypeNumber--;
        if (iBallTypeNumber < 0)
        {
            iBallTypeNumber = 3;
        }
        CheckBallTypeNumber();
    }

    //Increase the ball type number - KT
    public void CycleRight()
    {
        iBallTypeNumber++;
        if (iBallTypeNumber > 3)
        {
            iBallTypeNumber = 0;
        }
        CheckBallTypeNumber();
    }

    //Load the scene that you clicked in the level select - KT
    public void Continue()
    {
        goGameController = GameObject.FindGameObjectWithTag("GameControl");
        gcGameControlScript = goGameController.GetComponent<GameControl>();
        gcGameControlScript.SetBallTypeName(sBallTypeName);
        gcGameControlScript.SetBallPick((ObjectiveScript.BallType)iBallTypeNumber);
        gcGameControlScript.Save();
        string LevelName = gcGameControlScript.GetLevelToLoad().ToString();
        SceneManager.LoadScene(LevelName);
    }

    //Change the sprite depending on the ball - KT
    public void CheckBallTypeNumber()
    {
        switch (iBallTypeNumber)
        {
            case 0:
                tBallTypeText.text = "Basketball";
                sBallTypeName = "Default";
                imBallImage.sprite = sDefaultSprite;
                break;
            case 1:
                tBallTypeText.text = "Bouncy Ball";
                sBallTypeName = "BouncyBall";
                imBallImage.sprite = sBouncyBallSprite;
                break;
            case 2:
                tBallTypeText.text = "Bean Bag";
                sBallTypeName = "BeanBag";
                imBallImage.sprite = sBeanBagSprite;
                break;
            case 3:
                tBallTypeText.text = "Floaty Ball";
                sBallTypeName = "FloatyBall";
                imBallImage.sprite = sFloatyBallSprite;
                break;
            default:
                break;
        }

    }
}
