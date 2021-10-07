//////////////////////////////////////////////////////////////////
// File Name: SpaceSawAudio.cs                                  //
// Author: Josh Godsell                                         //
// Date Created: 30/5/19                                        //
// Date Last Edited: 30/5/19                                    //
// Brief:Class to control the Space saw objects audio           //
//////////////////////////////////////////////////////////////////
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpaceSawAudio : MonoBehaviour
{

    //the game manager and the player controller
    private GameManager m_GM;
    private PlayerControl m_PlayerControl;

    //the audio source attached to the saw and the sound to be played
    private AudioSource m_AudioSource;
    public AudioClip m_BuzzSawSound;

    //bool for id the player is in the same room as the saw
    private bool m_bPlayerInRoom;

    void Start()
    {
        //try to assign the game manager and player control
        try
        {
            m_GM = FindObjectOfType<GameManager>();
            m_PlayerControl = FindObjectOfType<PlayerControl>();
        }
        catch (System.Exception)
        {

            throw;
        }

        //if no audio source component attached to the object then add one and assign it
        if (GetComponent<AudioSource>() == null)
        {
            m_AudioSource = this.gameObject.AddComponent<AudioSource>();
        }
        else
        {
            m_AudioSource = GetComponent<AudioSource>();
        }
    }



    public void Update()
    {
        //check if the playr is in the room
        CheckInPlayerRoom();


    }

    public void CheckInPlayerRoom()
    {
        //get the rooms from the game manager
        Room[] t_Rooms = m_GM.GetLevelRooms();

        foreach (Room room in t_Rooms)
        {
            //if the room contains this saw
            if (room.m_bounds.Contains(this.transform.position))
            {

                // if the room is the same as the players current room
                if (room == m_PlayerControl.GetRoom())
                {

                    //if the bool is false
                    if (m_bPlayerInRoom == false)
                    {

                        //set the player in room bool to true
                        m_bPlayerInRoom = true;

                        //start the audio coroutine
                        StartCoroutine(PlayAudio());

                    }

                }
                else
                {
                    //if the player is not in the same room
                    //stop all the play audio coroutines
                    StopAllCoroutines();

                    //stop the audio
                    m_AudioSource.Stop();

                    //set the bool to false
                    m_bPlayerInRoom = false;
                }
            }
        }

    }

    //ienumerator to play the saw audio
    public IEnumerator PlayAudio()
    {

        while (true)
        {
            //if the player is in the room
            if (m_bPlayerInRoom)
            {
                //and the sound clip has been set
                if (m_BuzzSawSound != null)
                {
                    //one shot play the clip and reduce the volume to .5
                    m_AudioSource.PlayOneShot(m_BuzzSawSound);
                    m_AudioSource.volume = 0.05f;

                    //wait for the clip to end before repeating
                    yield return new WaitForSeconds(m_BuzzSawSound.length);
                }
                else
                {
                    yield return null;
                }
            }
        }

    }
}
