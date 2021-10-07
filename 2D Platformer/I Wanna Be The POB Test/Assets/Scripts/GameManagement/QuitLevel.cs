//////////////////////////////////////////////////////////////////
// File Name: QuitLevel.cs                                
// Author: Josh Godsell                                    
// Date Created: 14/5/19                                   
// Date Last Edited: 14/5/19                               
// Brief: level select return button script                      
//////////////////////////////////////////////////////////////////
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class QuitLevel : MonoBehaviour {

    //game manager
    private GameManager m_GMscript;


    // Use this for initialization
    void Awake ()
    {      
        //assign the game manager script
        try
        {
            m_GMscript = FindObjectOfType<GameManager>();
        }
        catch (System.Exception)
        {

            throw new System.Exception("Game manager not found");
        }
        //add the game manager quit level function to the button component of this script
        try
        {
            this.GetComponent<Button>().onClick.AddListener(m_GMscript.QuitLevel);
        }
        catch (System.Exception)
        {

            throw;
        }
    }

}
