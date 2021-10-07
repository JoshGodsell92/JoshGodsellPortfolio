//////////////////////////////////////////////////////////////////
// File Name: FallingSpike.cs                                   //
// Author: Josh Godsell                                         //
// Date Created: 29/5/19                                        //
// Date Last Edited: 29/5/19                                    //
// Brief: Class to control the falling  spike platform          //
//////////////////////////////////////////////////////////////////
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FallingSpike : MonoBehaviour
{
    //the spike speed and cleanup point
    public float m_fSpeed;
    public GameObject m_DestroyPoint;

    // Update is called once per frame
    void Update()
    {

        //move the spike down by the speed
        Vector3 newPos = this.transform.position;
        newPos.y -= m_fSpeed * Time.deltaTime;


        this.transform.position = newPos;

        //if the spike is below the cleanup point then destroy this object
        if (this.transform.position.y <= m_DestroyPoint.transform.position.y)
        {
            Destroy(this.gameObject);
        }
    }
}
