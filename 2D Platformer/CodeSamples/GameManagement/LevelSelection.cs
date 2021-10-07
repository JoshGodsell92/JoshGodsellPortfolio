//////////////////////////////////////////////////////////////////
// File Name: LevelSelection.cs                                
// Author: Josh Godsell                                    
// Date Created: 4/5/19                                   
// Date Last Edited: 4/5/19                               
// Brief: Level loading class                    
//////////////////////////////////////////////////////////////////
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class LevelSelection : MonoBehaviour {

    //game manager
    private GameManager m_GMscript;

    //name of the scene to load
    private string m_LevelToLoad;

    // Use this for initialization
    void Awake()
    {
        //assign the game manager
        try
        {
            m_GMscript = FindObjectOfType<GameManager>();
        }
        catch (System.Exception)
        {

            throw new System.Exception("Game manager not found");
        }

        //get the level to load from the name of the button
        try
        {
            m_LevelToLoad = this.gameObject.name;
        }
        catch(System.Exception)
        {
            throw new System.Exception(this.gameObject.name + " Scene not found");
        }

        //add load level to the button on click event
        try
        {
            this.GetComponent<Button>().onClick.AddListener(LoadLevel);
        }
        catch (System.Exception)
        {

            throw;
        }

    }

    //function to load the level
    public void LoadLevel()
    {
        SceneManager.LoadScene(m_LevelToLoad);

        //add the onSceneLoaded function to be run as soon as the level is loaded
        SceneManager.sceneLoaded += OnSceneLoaded;

    }

    void OnSceneLoaded(Scene a_Scene,LoadSceneMode a_mode)
    {
        //if the scene loaded isnt the main menu 
        if (a_Scene.name != "MainMenu")
        {
            //change the gmae state and run the loadlevel function to initialise the level
            Debug.Log("Scene Loaded");
            m_GMscript.ChangeState();
            m_GMscript.LoadLevel();

            //remove  the function from scene loaded
            SceneManager.sceneLoaded -= OnSceneLoaded;
        }
    }


}
