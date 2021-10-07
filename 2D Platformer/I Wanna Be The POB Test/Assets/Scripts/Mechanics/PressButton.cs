//////////////////////////////////////////////////////////////////
// File Name: PressButton.cs                                    //
// Author: Josh Godsell                                         //
// Date Created: 25/5/19                                        //
// Date Last Edited: 25/5/19                                    //
// Brief:Class to control the button object                     //
//////////////////////////////////////////////////////////////////
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PressButton : MonoBehaviour
{
    //the game manager 
    private GameManager m_GM;
    //is the button pushable
    private bool m_bPushable = true;

    // Use this for initialization
    void Start()
    {
        //try and assign the game manager
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
    void Update()
    {

    }

    //on collision enter function
    private void OnCollisionEnter2D(Collision2D collision)
    {
        //if the player collides with the button object and the button is pushable
        if (collision.gameObject.tag == "Player")
        {
            if (m_bPushable)
            {

                //get the rooms from the game manager 
                Room[] t_Rooms = m_GM.GetLevelRooms();

                //find the room which contains this button 
                foreach (Room room in t_Rooms)
                {
                    if (room.m_bounds.Contains(this.transform.position))
                    {
                        //set  pushable to false to prevent multiple pushes
                        m_bPushable = false;

                        //call the reverse gravity function on the gravity script
                        room.GetRoomGravity().ReverseGrav();

                        //call the delay push coroutine
                        StartCoroutine(PushDelay(0.5f));
                    }
                }
            }
        }
    }

    //coroutine to delay pushes
    public IEnumerator PushDelay(float a_secondsToWait)
    {
         //reset the pushable bool after parsed seconds
        yield return new WaitForSeconds(a_secondsToWait);

        m_bPushable = true;

        yield return null;
    }

    //retrieve is the button pushable
    public bool GetPushable()
    {
        return m_bPushable;
    }
}
