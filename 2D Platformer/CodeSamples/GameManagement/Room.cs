//////////////////////////////////////////////////////////////////
// File Name: Room.cs                                           //
// Author: Josh Godsell                                         //
// Date Created: 3/3/19                                         //
// Date Last Edited: 3/2/19                                     //
// Brief: Class containing room data and conections             //
//////////////////////////////////////////////////////////////////
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Room : MonoBehaviour {

    //this rooms position and dimensions
    private Vector3 m_RoomPosition;
    private Vector3 m_RoomDimensions;

    //room bounds
    public Bounds m_bounds;

    //the gravity for the room
    private Gravity m_Gravity;

    private void OnEnable()
    {
        //set the room position and dimensions
        m_RoomPosition = this.transform.position;
        m_RoomDimensions = new Vector3(16.0f, 10.0f, 1.0f);

        //set a new bounds for the room
        m_bounds = new Bounds(m_RoomPosition, m_RoomDimensions);

        //if the room has no gravity then add a gravity component
        if(GetComponent<Gravity>() == null)
        {
            m_Gravity = this.gameObject.AddComponent<Gravity>();
        }
        else
        {
            m_Gravity = GetComponent<Gravity>();
        }

    }

    // Update is called once per frame
    void Update () {
		
	}

    //retrieve the room gravity
    public Gravity GetRoomGravity()
    {
        return m_Gravity;
    }

    //draw a gizmo to represent the room bounds
    private void OnDrawGizmosSelected()
    {
#if (UNITY_EDITOR)

        m_RoomPosition = this.transform.position;
        m_RoomDimensions = new Vector3(16.0f, 10.0f, 1.0f);

        Gizmos.color = Color.red;

        Gizmos.DrawWireCube(m_RoomPosition, m_RoomDimensions);
#endif
    }
}
