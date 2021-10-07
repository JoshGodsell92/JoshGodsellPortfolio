//////////////////////////////////////////////////////////////////
// File Name: Boss_Two_Laser.cs                                 //
// Author: Josh Godsell                                         //
// Date Created: 28/5/19                                        //
// Date Last Edited: 28/5/19                                    //
// Brief: base Class controlling a boss                         //
//////////////////////////////////////////////////////////////////
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Boss_Two_Laser : MonoBehaviour {

    //is active
    private bool m_bIsActive = false;

    //get and set for is active
    public void SetIsActive(bool a_bool)
    {
        m_bIsActive = a_bool;                               
    }
    public bool GetIsActive()
    {
        return m_bIsActive;
    }

    //reset for the laser 
    public void Reset()
    {
        this.gameObject.SetActive(false);
        m_bIsActive = false;
    }
}
