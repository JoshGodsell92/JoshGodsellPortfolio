//////////////////////////////////////////////////////////////////
// File Name: IcicleImpact.cs                                   //
// Author: Josh Godsell                                         //
// Date Created: 23/5/19                                        //
// Date Last Edited: 24/5/19                                    //
// Brief: Class containing control of the iclicle impact        //
//////////////////////////////////////////////////////////////////
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IcicleImpact : MonoBehaviour {

    //the start position
    private Vector3 m_v3StartPos;

    //the dropper script responcible for the icicle
    private IcicleDrop m_ParentDropper;

	// Use this for initialization
	void Start ()
    {
        //set the start position
        m_v3StartPos = this.transform.position;
	}
	
	// Update is called once per frame
	void Update ()
    {
		
	}

    public void OnCollisionEnter2D(Collision2D collision)
    {
        //if the icicle collides with the player then reset to the start pos and call the wait to drop coroutine from the parent dropper
        if (collision.gameObject.tag == "Player")
        {
            this.transform.position = m_v3StartPos;
            m_ParentDropper.StartCoroutine(m_ParentDropper.WaitToDrop(1.0f));
        }
    }

    //set the dropper parent
    public void SetDropp(IcicleDrop a_parentDropper)
    {
        m_ParentDropper = a_parentDropper;
    }
}
