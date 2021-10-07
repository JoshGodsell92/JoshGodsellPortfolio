using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Pause : MonoBehaviour {

    private bool CanvasDisplayed = false;

    public GameObject PauseCanvas;

	// Use this for initialization
	void Start () {
        PauseCanvas.SetActive(false);
	}
	
	// Update is called once per frame
	void Update () {
		
	}

    public void DisplayPauseCanvas()
    {
        //if the function has been called and the canvas isnt currently displayed then stop teh game time and display it - SM
        if (!CanvasDisplayed)
        {
            PauseCanvas.SetActive(true);
            Time.timeScale = 0f;
            CanvasDisplayed = true;
        }
        else
        {
            //else set the time to resume and hide the canvas - SM
            PauseCanvas.SetActive(false);
            Time.timeScale = 1.0f;
            CanvasDisplayed = false;
        }

    }
}
