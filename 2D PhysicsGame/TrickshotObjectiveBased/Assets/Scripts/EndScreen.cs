using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class EndScreen : MonoBehaviour {

    private GameObject goGameController;
    private GameControl gcGameControlScript;

    // Use this for initialization
    void Start ()
    {
        goGameController = GameObject.FindWithTag("GameControl");
        gcGameControlScript = goGameController.GetComponent<GameControl>();
        gcGameControlScript.EndScreen();
    }
	
	// Update is called once per frame
	void Update () {
		if (Input.anyKey)
        {
            goGameController = GameObject.FindWithTag("GameControl");
            gcGameControlScript = goGameController.GetComponent<GameControl>();
            gcGameControlScript.Reset();
            SceneManager.LoadScene("Menu");
        }
	}
}
