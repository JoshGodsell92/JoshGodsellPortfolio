//////////////////////////////////////////////////////////////////
// File Name: FloppyFall.cs                                     //
// Author: Josh Godsell                                         //
// Date Created: 29/5/19                                        //
// Date Last Edited: 29/5/19                                    //
// Brief: Class to control the floppy disk fall                 //
//////////////////////////////////////////////////////////////////
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FloppyFall : MonoBehaviour {

    //the fall speed
    public float m_fFallSpeed;

    // the point to check against for clean up
    public GameObject m_DestroyPoint;

    //the player control and game manager
    private GameManager m_GM;
    private PlayerControl m_PlayerControl;

    //the audio source and the clip to be played
    private AudioSource m_AudioSource;
    public AudioClip m_FloppySound;

    //bool for if is in the players room
    private bool m_bPlayerInRoom;


    private void Start()
    {
        //assign the player control and game manager
        try
        {
            m_GM = FindObjectOfType<GameManager>();
            m_PlayerControl = FindObjectOfType<PlayerControl>();
        }
        catch (System.Exception)
        {

            throw;
        }

        //if there is not an audio source then add one
        if (GetComponent<AudioSource>() == null)
        {
            m_AudioSource = this.gameObject.AddComponent<AudioSource>();
        }
        else
        {
            m_AudioSource = GetComponent<AudioSource>();
        }
    }

    // Update is called once per frame
    void Update ()
    {

        //update the position of thr falling floppy disk
        Vector3 newPos = this.transform.position;
        newPos.y -=  m_fFallSpeed * Time.deltaTime;


        this.transform.position = newPos;

        if(this.transform.position.y <= m_DestroyPoint.transform.position.y)
        {
            Destroy(this.gameObject);

        }

        //check if in the players room
        CheckInPlayerRoom();
	}

    //function for checking if the player is in the same room as the falling disk
    public void CheckInPlayerRoom()
    {

        Room[] t_Rooms = m_GM.GetLevelRooms();

        foreach (Room room in t_Rooms)
        {
            if (room.m_bounds.Contains(this.transform.position))
            {
                if (room == m_PlayerControl.GetRoom())
                {

                    if (m_bPlayerInRoom == false)
                    {
                        //if the player is in the same room as the falling  floppy disk then start the audio coroutine
                        m_bPlayerInRoom = true;

                        StartCoroutine(PlayAudio());

                    }

                }
                else
                {
                    //otherwise stop all coroutines and stop the audio from playing
                    StopAllCoroutines();

                    m_AudioSource.Stop();

                    m_bPlayerInRoom = false;
                }
            }
        }

    }

    public void Reset()
    {
        Destroy(this.gameObject);
    }

    //ienumerator to play the audio on repeating when it has finished
    public IEnumerator PlayAudio()
    {

        while (true)
        {
            if (m_bPlayerInRoom)
            {

                if (m_FloppySound != null)
                {
                    m_AudioSource.PlayOneShot(m_FloppySound);
                    m_AudioSource.volume = 0.5f;

                    yield return new WaitForSeconds(m_FloppySound.length);
                }
                else
                {
                    yield return null;
                }
            }
        }

    }
}
