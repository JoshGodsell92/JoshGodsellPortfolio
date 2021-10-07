//////////////////////////////////////////////////////////////////
// File Name: BossController.cs                                 //
// Author: Josh Godsell                                         //
// Date Created: 21/3/19                                        //
// Date Last Edited: 28/5/19                                    //
// Brief: base Class controlling a boss                         //
//////////////////////////////////////////////////////////////////
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BossController : MonoBehaviour {

    //boss health state
    public enum Boss_State
    {
        FULLHEALTH,
        HALFHEALTH,
    }



    private GameManager m_GameManager;
    private GameObject m_BossBlocker;

    //member enums for state machine
    private Boss_State m_eHealthState = Boss_State.FULLHEALTH;

    //audios source
    private AudioSource m_AudioSource;

    //death sound clip
    private AudioClip m_DeathSound;

    //base health
    private int m_iHealth = 30;

    //the left and right positions the boss will stop at
    private Vector3 m_v3StartPos;

    //the player object
    private GameObject m_Player;
    //if the boss has been triggered
    private bool m_bTriggered = false;

    //the boss health bar
    private Slider m_HealthBar;
    //can the boss be hit
    private bool m_bCanBeHit = false;
    //has the boss been hit
    private bool m_bIsHit = false;

    // Use this for initialization
    public virtual void Start ()
    {
        //set the starting position
        m_v3StartPos = this.transform.position;

        //assign the variables and disable the room blocking object
        try
        {
            m_GameManager = FindObjectOfType<GameManager>();
            m_Player = GameObject.FindGameObjectWithTag("Player");
            m_HealthBar = GameObject.Find("BossHealthBar").GetComponent<Slider>();
            m_AudioSource = GetComponent<AudioSource>();
            m_BossBlocker = GameObject.Find("BossBlocker");
            m_BossBlocker.SetActive(false);
        }
        catch (System.Exception)
        {

            throw;
        }

        //deactivate the health bar for now
        m_HealthBar.gameObject.SetActive(false);
    }

    //base fixed update
    public virtual void FixedUpdate()
    {
        //set has been hit to false
        m_bIsHit = false;

        //if the health drops below 0 then play the death sound and trigger the end screen
        if (m_iHealth <= 0)
        {
            PlayAudio(m_DeathSound);

            m_GameManager.LevelCompleteOverlay();

        }

        //assign the health value to the slider
        m_HealthBar.value = m_iHealth;
    }

    // Update is called once per frame
    public virtual void Update ()
    {

    }

    //base trigger collision
    virtual public void OnTriggerEnter2D(Collider2D collision)
    {
        //if hit by a player bullet the deincrement health and set has been hit to true deactivate the bullet
        if (collision.gameObject.tag == "PlayerBullet")
        {
            if (m_bCanBeHit)
            {
                if (!m_bIsHit)
                {
                    m_bIsHit = true;
                    collision.gameObject.SetActive(false);
                    m_iHealth--;

                    //Debug.Log("Boss Hit");
                }
            }
        }
    }

    //function to play an audio  clip
    public void PlayAudio(AudioClip a_AudioClip)
    {

        m_AudioSource.PlayOneShot(a_AudioClip);
    }

    //function to play an audio  clip taking a volume to reduce as well
    public void PlayAudio(AudioClip a_AudioClip,float a_volume)
    {

        if (a_volume > 0 && a_volume < 1.0)
        {
            m_AudioSource.volume = a_volume;
        }
        
        m_AudioSource.PlayOneShot(a_AudioClip);
    }

    //function to stop audio
    public void StopAudio()
    {
        m_AudioSource.Stop();
    }

    //base reset
    virtual public void Reset()
    {
        m_iHealth = 30;
        this.gameObject.transform.position = m_v3StartPos;

    }

    //function to activate the room blocker to seal the player in with the boss
    public void LockRoom()
    {
        m_BossBlocker.SetActive(true);
    }

    //function to calculate the parabola curve for the bounce 
    public Vector2 Parabola(Vector2 a_v2Start, Vector2 a_v2End, float a_fHeight, float a_fTime)
    {

        //parabola calculation
        System.Func<float, float> f = x => -4 * a_fHeight * x * x + 4 * a_fHeight * x;

        //move the vector along the curve for the time
        Vector2 mid = Vector2.Lerp(a_v2Start, a_v2End, a_fTime);

        //return the position along the curve at this time step
        return new Vector2(mid.x, f(a_fTime) + Mathf.Lerp(a_v2Start.y, a_v2End.y, a_fTime));
    }

    //coroutine to call a function after a time
    public IEnumerator DelayedCall(float a_fSecondsToWait, System.Action a_FunctionToUse)
    {
        yield return new WaitForSeconds(a_fSecondsToWait);

        a_FunctionToUse();

        if (m_HealthBar.gameObject.activeSelf == false)
        {
            m_bCanBeHit = true;
            m_HealthBar.gameObject.SetActive(true);
        }

        yield return null;

    }

    public void BossDeath()
    {
        StopAllCoroutines();      
    }

    #region Get&Set

    public void SetIsTriggered(bool a_bool)
    {
        m_bTriggered = a_bool;
    }
    public bool GetIsTriggered()
    {
        return m_bTriggered;
    }

    public GameObject GetPlayer()
    {
        return m_Player;
    }

    public void SetCanBeHit(bool a_bool)
    {
        m_bCanBeHit = a_bool;
    }
    public bool GetCanBeHit()
    {
        return m_bCanBeHit;
    }

    public void SetIsHit(bool a_bool)
    {
        m_bIsHit = a_bool;
    }
    public bool GetIsHit()
    {
        return m_bIsHit;
    }

    public Vector3 GetStartPos()
    {
        return m_v3StartPos;
    }

    public void SetHealth(int a_Health)
    {
        m_HealthBar.maxValue = a_Health;
        m_iHealth = a_Health;
    }
    public int GetHealth()
    {
        return m_iHealth;
    }
    public void DamageHealth()
    {
        m_iHealth--;
    }

    public void SetDeathClip(AudioClip a_AudioClip)
    {
        m_DeathSound = a_AudioClip;
    }

    #endregion

}
