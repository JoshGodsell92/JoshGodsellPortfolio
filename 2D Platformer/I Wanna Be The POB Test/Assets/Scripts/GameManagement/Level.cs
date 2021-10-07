///////////////////////////////////////////////////////////////////////////////////////////
// File Name: Level.cs                                
// Author: Josh Godsell                                    
// Date Created: 13/3/19                                   
// Date Last Edited: 15/3/19                               
// Brief:serializable class for holding level information for the game  and user run information                    
///////////////////////////////////////////////////////////////////////////////////////////
using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;
using UnityEngine.SceneManagement;

[System.Serializable]
public class Level {

    //the name of the level for the data stored
    private string m_LevelName;

    //the bools for if the player has unlocked the level or has completed the level
    private bool m_bIsUnlocked = false;
    private bool m_bHasCompleted = false;
    //an array to hold which collectables have been collected as bools
    private bool[] m_aCollectableBools;

    // best timed run and death run of the level
    private LevelRun m_BestTimeRun;
    private LevelRun m_BestDeathRun;

    //default constructor
    public Level()
    {
        //srt unlocked to false
        m_bIsUnlocked = false;

        //initialise anew array for the collectables
        m_aCollectableBools = new bool[10];

        for(int i = 0; i < m_aCollectableBools.Length; ++i)
        {
            m_aCollectableBools[i] = false;
        }

        //set new LevelRuns
        m_BestTimeRun = new LevelRun();
        m_BestDeathRun = new LevelRun();
    }

    //function for when a level is completed
    public void LevelCompleted(int a_iTimesDied, TimeSpan a_TimeCompleted)
    {
        //if this is the first time the level is completed by the user
        if (m_bHasCompleted == false)
        {
            //set has completed to true
            m_bHasCompleted = true;

            //set the best times and death runs to the currrent run as it is the first completion
            m_BestTimeRun.SetTimeTaken(a_TimeCompleted);
            m_BestTimeRun.SetDeaths(a_iTimesDied);

            m_BestDeathRun.SetTimeTaken(a_TimeCompleted);
            m_BestDeathRun.SetDeaths(a_iTimesDied);

        }
        else
        {

            //if the user has completed the level before then check the current run time and deaths against the old best time and deaths

            if(CheckBestTime(a_TimeCompleted))
            {
                //if the new time is better assign the values
                m_BestTimeRun.SetTimeTaken(a_TimeCompleted);
                m_BestTimeRun.SetDeaths(a_iTimesDied);
            }

            if(CheckBestDeaths(a_iTimesDied))
            {
                //if the new deaths is better assign the values
                m_BestDeathRun.SetTimeTaken(a_TimeCompleted);
                m_BestDeathRun.SetDeaths(a_iTimesDied);
            }
        }       
    }

    //function to check the best time
    public bool CheckBestTime(TimeSpan a_TimeTaken)
    {
        TimeSpan t_BestTime = m_BestTimeRun.GetTimeTaken();

        if(t_BestTime > a_TimeTaken)
        {
            return true;
        }


        return false;
    }

    //function to check the best death
    public bool CheckBestDeaths(int a_iDeaths)
    {
        int t_BestDeaths = m_BestDeathRun.GetDeaths();

        if (t_BestDeaths > a_iDeaths)
        {
            return true;
        }


        return false;
    }

    #region Get&Set Functions
    public void SetIsLevelUnlocked(bool a_bool)
    {
        m_bIsUnlocked = a_bool;
    }
    public bool GetIsLevelUnlocked()
    {
        return m_bIsUnlocked;
    }

    public void SetLevelName(string a_levelName)
    {
        m_LevelName = a_levelName;
    }
    public string GetLevelName()
    {
        return m_LevelName;
    }

    public bool GetCollectableInfo(int a_index)
    {
        return m_aCollectableBools[a_index];
    }
    public bool[] GetCollectables()
    {
        return m_aCollectableBools;
    }

    public void SetCollectablesData(Collectable[] a_LevelCollectables)
    { 
        for(int i = 0; i < a_LevelCollectables.Length; ++i)
        {
            m_aCollectableBools[i] = a_LevelCollectables[i].GetCollected();
        }
    }

    public LevelRun GetBestTime()
    {
        return m_BestTimeRun;
    }
    public LevelRun GetBestDeath()
    {
        return m_BestDeathRun;
    }

    #endregion

}


//serializable class for run data
[System.Serializable]
public class LevelRun
{
    //user thatdid the run
    private string m_sUsername;
    //the time taken to finish the level
    private TimeSpan m_TimeTaken;
    //the deaths taken during the level
    private int m_iDeaths;

    //constructor for new levelrun
    public LevelRun()
    {
        m_sUsername = null;
        m_iDeaths = 0;
        m_TimeTaken = new TimeSpan();
    }

    //getter and setters for values stored
    public void SetTimeTaken(TimeSpan a_TimeTaken)
    {
        m_TimeTaken = a_TimeTaken;
    }
    public TimeSpan GetTimeTaken()
    {
        return m_TimeTaken;
    }

    public void SetDeaths(int a_iDeaths)
    {
        m_iDeaths = a_iDeaths;
    }
    public int GetDeaths()
    {
        return m_iDeaths;
    }

    public void SetUserName(string a_sUserName)
    {
        m_sUsername = a_sUserName;
    }
    public string GetUserName()
    {
        return m_sUsername;
    }
}
