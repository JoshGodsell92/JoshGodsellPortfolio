//////////////////////////////////////////////////////////////////
// File Name: RetractableSpike.cs                               //
// Author: Josh Godsell                                         //
// Date Created: 21/5/19                                        //
// Date Last Edited: 21/5/19                                    //
// Brief:Class to control the Retractable spike object          //
//////////////////////////////////////////////////////////////////
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RetractableSpike : MonoBehaviour {

    //enum for the spike state
    public enum SpikeState
    {
        RETRACTED,
        EXTENDED
    }

    //member variable for the state
    private SpikeState m_eSpikeState;

    //the game manager and player control script
    private GameManager m_GM;
    private PlayerControl m_PlayerControl;

    //the two sprites to swap between
    public Sprite m_RetractedSprite;
    public Sprite m_ExtendedSprite;

    //the audio source and sound clip to be played
    private AudioSource m_AudioSource;
    public AudioClip m_Sound;

    //the time delay between swapping
    public float m_fTimeDelay;
    //this objects sprite renderer
    private SpriteRenderer m_spriteRenderer;

    // Use this for initialization
    void Start ()
    {
        //try to assign the variables from the scene
        try
        {
            m_spriteRenderer = GetComponent<SpriteRenderer>();

            m_GM = FindObjectOfType<GameManager>();
            m_PlayerControl = FindObjectOfType<PlayerControl>();
        }
        catch (System.Exception)
        {

            throw;
        }


        //if there is no audio source component the add one
        if(GetComponent<AudioSource>() == null)
        {
            m_AudioSource = this.gameObject.AddComponent<AudioSource>();
        }
        else
        {
            m_AudioSource = GetComponent<AudioSource>();
        }

        //set the initial state to retracted
        m_eSpikeState = SpikeState.RETRACTED;

        //set the retracted tag to platform
        this.gameObject.tag = "Platform";

        //begin the looping coroutine
        StartCoroutine(TimeLoop(m_fTimeDelay));

	}

    //function to swap the sprite
    public void SwapSprite()
    {
        //if the spike is retracted
        if (m_eSpikeState == SpikeState.RETRACTED)
        {
            //swap to the extended sprite and set the state to match
            m_spriteRenderer.sprite = m_ExtendedSprite;
            m_eSpikeState = SpikeState.EXTENDED;
            //swap the tag to spike so the player can now be killed by this object
            this.gameObject.tag = "Spike";

            //reset the polygon collider attached
            Destroy(GetComponent<PolygonCollider2D>());
            gameObject.AddComponent<PolygonCollider2D>();


            //Debug.Log("Spike Extended");
        }
        else
        {
            //otherwise use the retrected sprite and set the state to retracted
            m_spriteRenderer.sprite = m_RetractedSprite;
            m_eSpikeState = SpikeState.RETRACTED;
            //assign the platform tag so the player will not be killed in this state
            this.gameObject.tag = "Platform";

            //reset the polygon collider
            Destroy(GetComponent<PolygonCollider2D>());
            gameObject.AddComponent<PolygonCollider2D>();

            //Debug.Log("Spike Extended");

        }
    }

    //function to check if the player is in the room
    public bool CheckInPlayerRoom()
    {

        //get the level rooms from the game manager
        Room[] t_Rooms = m_GM.GetLevelRooms();

        foreach (Room room in t_Rooms)
        {
            //if this objects room is the same as the players room return true
            if (room.m_bounds.Contains(this.transform.position))
            {
                if (room == m_PlayerControl.GetRoom())
                {
                    return true;
                }
            }
        }

        //return false if the player room and this objects room  dont match return false
        return false;
    }

    //time loop ienumerator
    IEnumerator TimeLoop(float a_TimeLoop)
    {
        while (true)
        {
            //wait based on the loop time
            yield return new WaitForSeconds(a_TimeLoop);

            //if the player is in the room then play the audio clip
            if(CheckInPlayerRoom())
            {
                m_AudioSource.PlayOneShot(m_Sound);
                m_AudioSource.volume = 0.2f;
            }

            //swap the sprite
            SwapSprite();
        }
    }

}
