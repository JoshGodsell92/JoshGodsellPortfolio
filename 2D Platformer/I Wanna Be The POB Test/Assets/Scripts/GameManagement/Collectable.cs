/////////////////////////////////////////////////
// File Name: Collectable.cs                     
// Author: Josh Godsell                          
// Date Created: 10/2/19                         
// Date Last Edited: 30/5/19                     
// Brief:Class for collectable                   
/////////////////////////////////////////////////
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

public class Collectable : MonoBehaviour {

    //the index for the collectable
    public int m_iIndex;
    //if the collectable has been collected
    private bool m_bCollected = false;

    //the audio clip for collection
    public AudioClip m_Collection;

    // Use this for initialization
    void Start ()
    {
        //if the collectable is collected then disable the gameobject
		if(m_bCollected)
        {
            this.gameObject.SetActive(false);
        }
	}

    //get the collectable index
    public int GetIndex()
    {
        return m_iIndex;
    }

    //get and set for is collected
    public bool GetCollected()
    {
        return m_bCollected;
    }
    public void SetCollected(bool a_bool)
    {
        m_bCollected = a_bool;
    }

    //collision with player sets the collected bool
    private void OnTriggerEnter2D(Collider2D collider)
    {
        if(collider.gameObject.tag == "Player")
        {
            collider.gameObject.GetComponent<PlayerControl>().PlayAudio(m_Collection);

            m_bCollected = true;

            this.gameObject.SetActive(false);

        }
    }


}
