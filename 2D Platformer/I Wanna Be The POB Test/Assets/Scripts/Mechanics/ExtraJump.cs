//////////////////////////////////////////////////////////////////
// File Name: ExtraJump.cs                                      //
// Author: Josh Godsell                                         //
// Date Created: 31/1/19                                        //
// Date Last Edited: 31/1/19                                    //
// Brief: Class containing ExtraJump Pickup functions           //
//////////////////////////////////////////////////////////////////

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ExtraJump : MonoBehaviour {

    //bool for is active or resetting
    private bool m_bRespawnStarted = false;
    private bool m_bIsActive = true;

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}

    //when the jumpboost is collected 
    public void Collected()
    {
        //hide the sprite
        GetComponent<Renderer>().enabled = false;

        //start a timer
        StartRespawnTimer(5.0f);
    }

    //reset function
    public void Reset()
    {
        m_bIsActive = true;
        m_bRespawnStarted = false;

        GetComponent<Renderer>().enabled = true;
    }

    //function to start a respawn timer
    public void StartRespawnTimer(float a_fTimeToRespawn)
    {
        m_bRespawnStarted = true;

        StartCoroutine(JumpRespawnAfterTime(a_fTimeToRespawn));
    }

    //set and get for respawn started
    public void SetRespawnStarted(bool a_bool)
    {
        m_bRespawnStarted = a_bool;
    }
    public bool GetRespawnStarted()
    {
        return m_bRespawnStarted;
    }

    //get and set for is active
    public void SetIsActive(bool a_bool)
    {
        m_bIsActive = a_bool;
    }
    public bool GetIsActive()
    {
        return m_bIsActive;
    }

    //coroutine for reseting after a time        
    IEnumerator JumpRespawnAfterTime(float a_fTimeToRespawn)
    {
        //Debug.Log("Respawn Start");

        yield return new WaitForSeconds(a_fTimeToRespawn);

        Reset();

        yield return null;
    }
}
