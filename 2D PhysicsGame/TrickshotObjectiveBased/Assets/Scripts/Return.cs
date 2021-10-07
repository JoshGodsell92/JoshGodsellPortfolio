using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Return : MonoBehaviour {

    //The level to load - KT
    public string LevelName;

	// Use this for initialization
	void Start () {
        DestroyObject(GameObject.FindWithTag("GameControl"));
	}
	
	// Update is called once per frame
	void Update () {
		
	}

    public void LoadLevel()
    {
        SceneManager.LoadScene(0);
    }

    public void LoadAnyLevel()
    {
        SceneManager.LoadScene(LevelName);
    }
}
