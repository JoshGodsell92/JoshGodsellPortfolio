//////////////////////////////////////////////////////////////////
// File Name: PassGate.cs                             //
// Author: Josh Godsell                                         //
// Date Created: 18/4/19                                        //
// Date Last Edited: 18/4/19                                    //
// Brief: Class containing PassGate functionality               //
//////////////////////////////////////////////////////////////////
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PassGate : MonoBehaviour {

    //the game manager
    private GameManager m_GameManager;

    //the current level
    private Level m_CurrentLevel;

	// Use this for initialization
	void Start ()
    {
        //assign the current level and game manager
        try
        {
            m_GameManager = GameObject.Find("GameManager").GetComponent<GameManager>();
            m_CurrentLevel = m_GameManager.GetCurrentLevel();
        }
        catch (System.Exception)
        {

            throw;
        }	
	}
	
	// Update is called once per frame
	void Update ()
    {
        //int to count collected collectables 
        int collectedCount = 0;

        //for each collectable in the current level
        foreach(bool collectable in m_CurrentLevel.GetCollectables())
        {
            //increment the count if it has been collected
            if(collectable)
            {
                ++collectedCount;
            }
        }

        //if the collected count is four or more then destroy the pass gate object 
        if(collectedCount >= 4 )
        {
            this.gameObject.SetActive(false);
        }
	}
}
