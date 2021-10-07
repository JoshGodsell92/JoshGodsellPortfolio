/////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
///Name: Collectables.cs 
///Created by: Charlie Bullock
///Description: This script will check if the player has collected collected tagged objects and can be tweaked to cause
///certain outcomes based on the amount of collectables which the player collects, additionally this value will be stored
///in the system preferences as a variable
///Last edited by:
///Date Edited:
/////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Collectables : MonoBehaviour {
    //Variables
    #region Variables
    private int iCollectables;
    private int iHighScore;
    private int iTempValue;
    [SerializeField]
    private Text currentScore;
    [SerializeField]
    private Text highScore;
    #endregion Variables;

    //Function sets iCollectables and the iTempValue both to 0
    private void Awake()
    {
        iCollectables = 0;
        iTempValue = 0;
    }

    //Update function checking score and highscore values, additionally a switch statement is there to trigger possible events based upon the amount of collectable collected by the player
    private void Update()
    {
        //If the current score is more than the highscore then it's value is set as the highscore
        if (PlayerPrefs.GetInt("GameCurrentScore") > PlayerPrefs.GetInt("GameHighScore"))
        {
            iHighScore = iCollectables;
            PlayerPrefs.SetInt("GameHighScore", iHighScore);
            iHighScore = PlayerPrefs.GetInt("GameHighScore");
        }
        if (currentScore != null && highScore != null)
        {
            currentScore.text = "Level score = " + PlayerPrefs.GetInt("GameCurrentScore").ToString();
            highScore.text = "High score = " + PlayerPrefs.GetInt("GameHighScore").ToString();
        }

        if (iCollectables != iTempValue)
        {
            //Switch statement to be used for triggering events when players reach a certain amount of collected objects
            switch (iCollectables)
            {
                case 0:
                    break;
                case 1:
                    break;
                case 2:
                    break;
                case 3:
                    break;
                case 4:
                    break;
                case 5:
                    break;
                case 6:
                    break;
                case 7:
                    break;
                case 8:
                    break;
                case 9:
                    break;
                case 10:
                    break;
            }
            PlayerPrefs.SetInt("GameCurrentScore", (PlayerPrefs.GetInt("GameCurrentScore") + 1));
            iTempValue = iCollectables;
        }
    }

    //Triggewr function for when a collectable is found by the player to add to the iCollectables value
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.tag == "Collectable")
        {
            iCollectables++;
            Destroy(collision.gameObject);
        }
    }
}
