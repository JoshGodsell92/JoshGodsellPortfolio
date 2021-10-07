//////////////////////////////////////////////////////////////////
// File Name: Spear.cs                                          //
// Author: Josh Godsell                                         //
// Date Created: 29/5/19                                        //
// Date Last Edited: 29/5/19                                    //
// Brief:Class to control the spear object                      //
//////////////////////////////////////////////////////////////////
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Spear : MonoBehaviour {

    //the target and start positions
    public Vector3 m_v3TargetPos;
    public Vector3 m_v3StartPos;

    // the game manager
    private GameManager m_GameManager;

    //speed for the spear
    public float m_fSpeed;

    // Use this for initialization
    void Start ()
    {
        //try and assign the gamemanager                       
        try
        {
            m_GameManager = GameObject.FindObjectOfType<GameManager>();

        }
        catch (System.Exception)
        {

            throw new System.Exception("Game Manager not found");

        }
    }
	
	// Update is called once per frame
	void Update ()
    {
        //calculate the travel direction
        Vector3 m_v3Direction = Vector3.Normalize(m_v3TargetPos - m_v3StartPos);

        //set the facing direction
        this.transform.right = -m_v3Direction;

        //move the spear by time.deltaTime
        this.transform.position += (m_v3Direction * m_fSpeed * Time.deltaTime);

        //check if the spear is in the bossRoom
        CheckInBossRoom();
    }

    public void CheckInBossRoom()
    {
        //get the level rooms from the game manager
        Room[] t_Rooms = m_GameManager.GetLevelRooms();

        //setup a temporary room
        Room t_BossRoom = null;

        //for each room in the level rooms find the one named boss room
        foreach (Room room in t_Rooms)
        {
            if (room.gameObject.name == "Boss Room")
            {
                t_BossRoom = room;
            }
        }

        //if the temporary boss room has been assigned
        if (t_BossRoom != null)
        {
            //if the spear is not within the bounds of the boss room destroy the spear
            if (!t_BossRoom.m_bounds.Contains(this.transform.position))
            {
                Destroy(this.gameObject);
            }
        }
    }

    //reset function just destroys the object
    public void Reset()
    {
        Destroy(this.gameObject);
    }
}
