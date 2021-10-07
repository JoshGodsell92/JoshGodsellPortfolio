//////////////////////////////////////////////////////////////////
// File Name: Boss_Three.cs                                       //
// Author: Josh Godsell                                         //
// Date Created: 25/5/19                                        //
// Date Last Edited: 25/5/19                                    //
// Brief: Boss Controller for the third boss                   //
//////////////////////////////////////////////////////////////////
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Boss_Three : BossController
{
     //the states for the boss
    public enum ATTACK_STATE
    {
        IDLE,
        FLOAT
    }

    //the current boss state
    public ATTACK_STATE m_eAttackState;

    //projectile array
    private Boss_Three_Projectile[] m_aErrorProjectiles;

    //has the boss fired its projectiles
    private bool m_bHasFired = false;

    //floppy disk prefab
    public GameObject m_aFloppyDiskPrefab;

    //objects for floppy disk manipulation
    private GameObject m_FallingFloppySpawnLevel;
    private GameObject m_FallingFloppyLowerLevel;

    //the max range of the floppy spawn from the spawn pos
    private float m_fMaxX = 4.0f;
    private float m_fMinX = -4.0f;

    //the audio clips for the boss
    public AudioClip m_Shoot;
    public AudioClip m_Death;


    //overridden start function
    public override void Start()
    {
        base.Start();

        //seed the random
        Random.InitState(System.DateTime.Now.Millisecond);

        //set the health to 50
        SetHealth(50);

        //set the initial state to idle
        m_eAttackState = ATTACK_STATE.IDLE;

        //initiate the projectile array
        m_aErrorProjectiles = new Boss_Three_Projectile[3];

        //assign new projectiles
        for (int i = 0; i < m_aErrorProjectiles.Length; ++i)
        {
            m_aErrorProjectiles[i] = new Boss_Three_Projectile();
        }
        
        //assign the floppy disk manipulation objects
        try
        {
            m_FallingFloppySpawnLevel = GameObject.Find("Falling-disk Spawn");
            m_FallingFloppyLowerLevel = GameObject.Find("Falling-disk DropPoint");
        }
        catch (System.Exception)
        {

            throw;
        }

        //set the death noise clip
        SetDeathClip(m_Death);
    }

    //overridden update
    public override void Update()
    {
        base.Update();

        //swicth statement for boss behaviour
        switch (m_eAttackState)
        {
            case ATTACK_STATE.IDLE:

                if (!GetIsTriggered())
                {
                    if (GetPlayer() != null)
                    {
                        if (GetPlayer().GetComponent<PlayerControl>().GetRoom() != null)
                        {
                            if (GetPlayer().GetComponent<PlayerControl>().GetRoom().name == "Boss Room")
                            {
                                //if the player is in the boss room lok the room and start the coroutines for the flooppy disk and floating state
                                LockRoom();

                                StartCoroutine(DelayedCall(2.0f, FloatState));
                                StartCoroutine(LaunchFloppy());
                                SetIsTriggered(true);
                            }
                        }
                    }
                }
                break;

            case ATTACK_STATE.FLOAT:


                Floating();


                break;

            default:
                break;

        }

        UpdateProjectiles();

    }

    //floating function
    public void Floating()
    {

        //if the boss hasnt fired
        if (!m_bHasFired)
        {
            //start the fire error coroutine
            StartCoroutine(FireError());
            m_bHasFired = true;

        }

    }

    //function to update the projectiles
    public void UpdateProjectiles()
    {
        foreach (Boss_Three_Projectile proj in m_aErrorProjectiles)
        {
            if (proj.GetIsActive())
            {
                proj.Update();
            }
        }
    }

    //floppy disk falling function
    public void FallingDrive()
    {
        //get a random x position from the spawn 
        float t_XPos = Random.Range(m_fMinX, m_fMaxX); 

        Vector3 SpawnPos = new Vector3(t_XPos, 0.0f, 0.0f);

        //create a new instance of the floppy disk and assign its variables
        GameObject a_Instance = Instantiate(m_aFloppyDiskPrefab, m_FallingFloppySpawnLevel.transform);

        SpawnPos = a_Instance.transform.position;
        SpawnPos.x += t_XPos;

        a_Instance.transform.position = SpawnPos;

        a_Instance.GetComponent<FloppyFall>().m_DestroyPoint = m_FallingFloppyLowerLevel;

    }

    //state changer functions
    #region stateChangers
    public void IdleState()
    {
        m_eAttackState = ATTACK_STATE.IDLE;
    }
    public void FloatState()
    {
        m_eAttackState = ATTACK_STATE.FLOAT;
    }
    #endregion

    //get for the projectile array
    public Boss_Three_Projectile[] GetProjectiles()
    {
        return m_aErrorProjectiles;
    }

    //overridden collision trigger
    public override void OnTriggerEnter2D(Collider2D collision)
    {

            if (collision.gameObject.tag == "PlayerBullet")
            {
                if (GetCanBeHit())
                {
                    if (!GetIsHit())
                    {
                        SetIsHit(true);
                        collision.gameObject.SetActive(false);
                        DamageHealth();

                        //Debug.Log("Boss Hit");
                    }
                }
            }
    }

    //overridden reset function
    public override void Reset()
    {


        base.Reset();

       
            SetHealth(50);

        StopAllCoroutines();

        //if it is triggered reset all the variables and set the state to floating
        if (GetIsTriggered())
        {
            m_bHasFired = false;

        
            StartCoroutine(LaunchFloppy());
            StartCoroutine(DelayedCall(1.0f, FloatState));

            FloppyFall[] ActiveFloppyDisks = FindObjectsOfType<FloppyFall>();

            //reset the floppy disks and projectiles
            for (int i = 0; i < ActiveFloppyDisks.Length; ++i)
            {
                ActiveFloppyDisks[i].Reset();
            }

            foreach (Boss_Three_Projectile proj in m_aErrorProjectiles)
            {
                proj.Reset();
            }
        }
    }


    //fire error routine
    public IEnumerator FireError()
    {
        //for each projectile in the array
        for (int i = 0; i < m_aErrorProjectiles.Length; ++i)
        {
            //if the indexed element is not active
            if (!m_aErrorProjectiles[i].GetIsActive())
            {
                //if the instance is not null
                if (m_aErrorProjectiles[i].GetInstance() == null)
                {
                    //instantiate a new object and assign it to the array object
                    GameObject t_ProjObj = Instantiate(m_aErrorProjectiles[i].GetPrefab(), this.transform);

                    m_aErrorProjectiles[i].SetInstance(t_ProjObj);
                    m_aErrorProjectiles[i].SetPosition(this.transform.position);

                }
                else
                {
                    //if the instance is not null the set the position to the boss and is active to true
                    m_aErrorProjectiles[i].GetInstance().SetActive(true);
                    m_aErrorProjectiles[i].SetPosition(this.transform.position);
                }


                //set the travel direction and activate the script
                m_aErrorProjectiles[i].SetDirection(GetPlayer().transform.position - this.transform.position);
                m_aErrorProjectiles[i].SetIsActive(true);

                //play the shoot audio
                PlayAudio(m_Shoot);
            }

            yield return new WaitForSeconds(2.0f);

        }

        //set has fired to true
        m_bHasFired = true;

        //after 2 seconds
        yield return new WaitForSeconds(2.0f);

        //do while loop while has fired is true 
        do
        {
            //inactive count
            int InActive = 0;

            //for each array element increment the inactive count if is inactive 
            for (int i = 0; i < m_aErrorProjectiles.Length; ++i)
            {

                if (!m_aErrorProjectiles[i].GetIsActive())
                {
                    InActive++;
                }
            }

            //if all elements are inactive the b has fired is false allowing the fire function to be called again
            if (InActive == m_aErrorProjectiles.Length)
            {
                m_bHasFired = false;
            }
            else
            {
                yield return new WaitForSeconds(0.5f);
            }
        } while (m_bHasFired == true);


        yield return null;
    }

    //function to launch a floppy disk
    public IEnumerator LaunchFloppy()
    {

        while (true)
        {

            FallingDrive();

            yield return new WaitForSeconds(5.0f);
        }

        yield return null;
    }
}
