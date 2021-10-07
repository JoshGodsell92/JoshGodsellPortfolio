//////////////////////////////////////////////////////////////////
// File Name: ScrollSnap.cs                                
// Author: Josh Godsell                                    
// Date Created: 20/5/19                                   
// Date Last Edited: 20/5/19                               
// Brief: class to control the scoreboard screen level displays                    
//////////////////////////////////////////////////////////////////
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class ScrollSnap : MonoBehaviour
{
    //the leaderboard state
    private enum LeaderBoard
    {
        TIMES,
        DEATHS,
    }

    //current board state
    private LeaderBoard m_eState = LeaderBoard.TIMES;

    //the button  to swap states
    private Button m_SwitchBoard;

    //the buttons to change which level is displayed
    private Button m_PrevButton;
    private Button m_NextButton;

    //the times leaderboards
    private GameObject m_LeaderBoardOne;
    private GameObject m_LeaderBoardTwo;
    private GameObject m_LeaderBoardThree;
    private GameObject m_LeaderBoardFour;

    //the deaths leaderboards
    private GameObject m_LeaderBoardOneDeaths;
    private GameObject m_LeaderBoardTwoDeaths;
    private GameObject m_LeaderBoardThreeDeaths;
    private GameObject m_LeaderBoardFourDeaths;

   

    // Use this for initialization
    void Start ()
    {

        //find and assign all the game objects
        try
        {
            m_SwitchBoard = GameObject.Find("SwitchBoard").GetComponent<Button>();

            m_PrevButton = GameObject.Find("PrevButton").GetComponent<Button>();
            m_NextButton = GameObject.Find("NextButton").GetComponent<Button>();

            m_LeaderBoardOne = GameObject.Find("Level_1_BestTimes");
            m_LeaderBoardTwo = GameObject.Find("Level_2_BestTimes");
            m_LeaderBoardThree = GameObject.Find("Level_3_BestTimes");
            m_LeaderBoardFour = GameObject.Find("Level_4_BestTimes");

            m_LeaderBoardOneDeaths = GameObject.Find("Level_1_BestDeaths");
            m_LeaderBoardTwoDeaths = GameObject.Find("Level_2_BestDeaths");
            m_LeaderBoardThreeDeaths = GameObject.Find("Level_3_BestDeaths");
            m_LeaderBoardFourDeaths = GameObject.Find("Level_4_BestDeaths");

            //initialise the leaderboard to level one on the times board
            m_LeaderBoardTwo.SetActive(false);
            m_LeaderBoardThree.SetActive(false);
            m_LeaderBoardFour.SetActive(false);

            m_LeaderBoardOneDeaths.SetActive(false);
            m_LeaderBoardTwoDeaths.SetActive(false);
            m_LeaderBoardThreeDeaths.SetActive(false);
            m_LeaderBoardFourDeaths.SetActive(false);
        }
        catch (System.Exception)
        {

            throw;
        }

    }

    public void Update()
    {

        //if the state is times 
        if (m_eState == LeaderBoard.TIMES)
        {
            //if the level one leader board is active diable the Prev board button
            if (m_LeaderBoardOne.activeSelf)
            {
                m_PrevButton.gameObject.SetActive(false);
            }
            else
            {
                if (!m_PrevButton.gameObject.activeSelf)
                {
                    m_PrevButton.gameObject.SetActive(true);
                }
            }

            //if the level four leader board is active diable the next board button
            if (m_LeaderBoardFour.activeSelf)
            {
                m_NextButton.gameObject.SetActive(false);
            }
            else
            {
                if (!m_NextButton.gameObject.activeSelf)
                {
                    m_NextButton.gameObject.SetActive(true);


                }
            }
        }
        //if the state is deaths do the same but checking the level one death board and level four death board
        else if (m_eState == LeaderBoard.DEATHS)
        {
            if (m_LeaderBoardOneDeaths.activeSelf)
            {
                m_PrevButton.gameObject.SetActive(false);
            }
            else
            {
                if (!m_PrevButton.gameObject.activeSelf)
                {
                    m_PrevButton.gameObject.SetActive(true);
                }
            }

            if (m_LeaderBoardFourDeaths.activeSelf)
            {
                m_NextButton.gameObject.SetActive(false);
            }
            else
            {
                if (!m_NextButton.gameObject.activeSelf)
                {
                    m_NextButton.gameObject.SetActive(true);


                }
            }
        }

    }

    //function to swap the boards state
    public void SwitchBoard()
    {
        switch(m_eState)
        {
            case LeaderBoard.TIMES:

                m_LeaderBoardOne.SetActive(false);
                m_LeaderBoardTwo.SetActive(false);
                m_LeaderBoardThree.SetActive(false);
                m_LeaderBoardFour.SetActive(false);

                m_LeaderBoardOneDeaths.SetActive(true);


                m_eState = LeaderBoard.DEATHS;
                break;
            case LeaderBoard.DEATHS:

                m_LeaderBoardOneDeaths.SetActive(false);
                m_LeaderBoardTwoDeaths.SetActive(false);
                m_LeaderBoardThreeDeaths.SetActive(false);
                m_LeaderBoardFourDeaths.SetActive(false);

                m_LeaderBoardOne.SetActive(true);


                m_eState = LeaderBoard.TIMES;

                break;
            default:
                break;
        }
    }

    //previous button function
    public void PrevBoard()
    {

        if (m_eState == LeaderBoard.TIMES)
        {
            if (m_LeaderBoardTwo.activeSelf)
            {
                m_LeaderBoardTwo.SetActive(false);
                m_LeaderBoardOne.SetActive(true);
            }
            else if (m_LeaderBoardThree.activeSelf)
            {
                m_LeaderBoardThree.SetActive(false);
                m_LeaderBoardTwo.SetActive(true);

            }
            else if (m_LeaderBoardFour.activeSelf)
            {
                m_LeaderBoardFour.SetActive(false);
                m_LeaderBoardThree.SetActive(true);

            }
            else
            {

            }
        }
        else if (m_eState == LeaderBoard.DEATHS)
        {
            if (m_LeaderBoardTwoDeaths.activeSelf)
            {
                m_LeaderBoardTwoDeaths.SetActive(false);
                m_LeaderBoardOneDeaths.SetActive(true);
            }
            else if (m_LeaderBoardThreeDeaths.activeSelf)
            {
                m_LeaderBoardThreeDeaths.SetActive(false);
                m_LeaderBoardTwoDeaths.SetActive(true);

            }
            else if (m_LeaderBoardFourDeaths.activeSelf)
            {
                m_LeaderBoardFourDeaths.SetActive(false);
                m_LeaderBoardThreeDeaths.SetActive(true);

            }
            else
            {

            }
        }
    }

    //next button function
    public void NextBoard()
    {
        if (m_eState == LeaderBoard.TIMES)
        {
            if (m_LeaderBoardOne.activeSelf)
            {
                m_LeaderBoardOne.SetActive(false);
                m_LeaderBoardTwo.SetActive(true);
            }
            else if (m_LeaderBoardTwo.activeSelf)
            {
                m_LeaderBoardTwo.SetActive(false);
                m_LeaderBoardThree.SetActive(true);

            }
            else if (m_LeaderBoardThree.activeSelf)
            {
                m_LeaderBoardThree.SetActive(false);
                m_LeaderBoardFour.SetActive(true);

            }
            else
            {

            }
        }
        else if(m_eState == LeaderBoard.DEATHS)
        {
            if (m_LeaderBoardOneDeaths.activeSelf)
            {
                m_LeaderBoardOneDeaths.SetActive(false);
                m_LeaderBoardTwoDeaths.SetActive(true);
            }
            else if (m_LeaderBoardTwoDeaths.activeSelf)
            {
                m_LeaderBoardTwoDeaths.SetActive(false);
                m_LeaderBoardThreeDeaths.SetActive(true);

            }
            else if (m_LeaderBoardThreeDeaths.activeSelf)
            {
                m_LeaderBoardThreeDeaths.SetActive(false);
                m_LeaderBoardFourDeaths.SetActive(true);

            }
            else
            {

            }
        }
    }
}
