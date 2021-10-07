//////////////////////////////////////////////////////////////////
// File Name: Main menu.cs                                
// Author: Josh Godsell                                    
// Date Created: 20/5/19                                   
// Date Last Edited: 20/5/19                               
// Brief: main menu return button script                    
//////////////////////////////////////////////////////////////////
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MainMenu : MonoBehaviour {

    //game manager
    private GameManager m_GM;


	// Use this for initialization
	void Start ()
    {
        try
        {
            m_GM = FindObjectOfType<GameManager>();
        }
        catch (System.Exception)
        {

            throw;
        }	
	}
	
	// Update is called once per frame
	void Update () {
		
	}

    //function to return to the main menu
    public void BackToMainMenu()
    {
        m_GM.ReturnToMenu();
    }
}
