using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Customise : MonoBehaviour {

    public int iBallCounter = 0;
    public int iLinePrimaryCounter = 1;
    public int iLineSecondaryCounter = 1;
    private int iAimLinePrimaryCounter = 0;
    private int iAimLineSecondaryCounter = 0;

    public GameObject goProjectileDisplay;
    public GameObject goLineOneDisplay;
    public GameObject goLineTwoDisplay;

    public Color[] cLineFirstColors;
    public Color[] cLineSecondColors;
    public Color[] cAimLineColors;

    // Use this for initialization
    void Start () {
        
    }
	
	// Update is called once per frame
	void Update () {
		


        //Loops around the counter - SM
        if (iBallCounter < 0) { iBallCounter = 9; }
        if (iBallCounter > 9) { iBallCounter = 0; }

        //Loops around the counter - SM
        if (iLinePrimaryCounter < 0) { iLinePrimaryCounter = 9; }
        if (iLinePrimaryCounter > 9) { iLinePrimaryCounter = 0; }

        //Loops around the counter - SM
        if (iLineSecondaryCounter < 0) { iLineSecondaryCounter = 9; }
        if (iLineSecondaryCounter > 9) { iLineSecondaryCounter = 0; }

        //Loops around the counter - SM
        if (iAimLinePrimaryCounter < 0) { iAimLinePrimaryCounter = 9; }
        if (iAimLinePrimaryCounter > 9) { iAimLinePrimaryCounter = 0; }

        //Loops around the counter - SM
        if (iAimLineSecondaryCounter < 0) { iAimLineSecondaryCounter = 9; }
        if (iAimLineSecondaryCounter > 9) { iAimLineSecondaryCounter = 0; }

        //set colour based on the counters number - SM
        goLineOneDisplay.GetComponent<SpriteRenderer>().material.color = cLineFirstColors[iLinePrimaryCounter];
        goLineTwoDisplay.GetComponent<SpriteRenderer>().material.color = cLineSecondColors[iLineSecondaryCounter];

        GameObject.FindGameObjectWithTag("GameControl").GetComponent<GameControl>().SetLineOneColor(cLineFirstColors[iLinePrimaryCounter]);
        GameObject.FindGameObjectWithTag("GameControl").GetComponent<GameControl>().SetLineTwoColor(cLineSecondColors[iLineSecondaryCounter]);
        GameObject.FindGameObjectWithTag("GameControl").GetComponent<GameControl>().SetAimColor(cLineFirstColors[iAimLinePrimaryCounter]);
        GameObject.FindGameObjectWithTag("GameControl").GetComponent<GameControl>().SetAimColor2(cLineSecondColors[iAimLineSecondaryCounter]);

    }

    //Functions for ball color buttons - SM
    public void BallLeftColour ()
    {
        iBallCounter--;
    }

    public void BallRightColour()
    {
        iBallCounter++;
    }

    //Functions for first line color buttons - SM
    public void LinePrimaryLeftColour()
    {
        iLinePrimaryCounter--;
        iAimLinePrimaryCounter--;
    }

    public void LinePrimaryRightColour()
    {
        iLinePrimaryCounter++;
        iAimLinePrimaryCounter++;
    }

    //Functions for player color buttons - SM
    public void LineSecondaryLeftColour()
    {
        iLineSecondaryCounter--;
        iAimLineSecondaryCounter--;
    }

    public void LineSecondaryRightColour()
    {
        iLineSecondaryCounter++;
        iAimLineSecondaryCounter++;
    }

    public void Back()
    {
        Time.timeScale = 1.0f;
        SceneManager.LoadScene("Menu");
        Destroy(GameObject.Find("GameControl"));
    }
}
