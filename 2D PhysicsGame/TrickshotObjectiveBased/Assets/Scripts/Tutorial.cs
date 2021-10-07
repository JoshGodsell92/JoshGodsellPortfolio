using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Tutorial : MonoBehaviour {

    private int iKeyPressed = 0;

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {

	}

    public void CloseTutorial()
    {
        this.gameObject.SetActive(false);
        GameObject.Find("TutorialButton").SetActive(false);
    }
}
