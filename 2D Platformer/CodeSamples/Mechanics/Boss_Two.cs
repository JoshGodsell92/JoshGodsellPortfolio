//////////////////////////////////////////////////////////////////
// File Name: Boss_Two.cs                                       //
// Author: Josh Godsell                                         //
// Date Created: 25/5/19                                        //
// Date Last Edited: 25/5/19                                    //
// Brief: Boss Controller for the second boss                   //
//////////////////////////////////////////////////////////////////
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Boss_Two : BossController {

    //the bosses states
    public enum ATTACK_STATE
    {
        IDLE,
        LASERGRID,
        TRACKINGLASER,
        GOLDENLASER,
        CHARGE
    }

    //the current attack state and the last attack state
    public ATTACK_STATE m_eAttackState = ATTACK_STATE.IDLE;
    public ATTACK_STATE m_eLastAttackState = ATTACK_STATE.IDLE;

    //the potential boss positions and the current position
    private Vector3 m_v3UpPos;
    private Vector3 m_v3SecondUpPos;
    private Vector3 m_v3DropPos;
    private Vector3 m_v3SecondDropPos;
    private Vector3 m_v3CurrentPos;

    //the sprite rendere and the gun sprites
    SpriteRenderer m_SpriteRenderer;
    public GameObject m_Gun1;
    public GameObject m_Gun2;
    //the current room gravity
    Gravity m_RoomGrav;

    //the tracking poin and golden laser object
    private GameObject m_TrackingPoint;
    private GameObject m_GoldenLaser;

    //the laser parent and an array of laser objects
    private GameObject m_LaserParent;
    private GameObject[] m_LaserObjects; 

    //the golden laser scale maximum and scale speed
    public float m_fGoldenLaserScaleMax;
    public float m_fScaleSpeed;
    //the bosses charging speed
    public float m_fChargeSpeed;

    //the bosses audio source and clips
    public AudioSource m_SecondSource;
    public AudioClip m_Ambient;
    public AudioClip m_BossDeath;
    public AudioClip m_TrackSound;
    public AudioClip m_TrackExpand;
    public AudioClip m_ChargeSound;
    public AudioClip m_LaserStart;
    public AudioClip m_Laser;

    //the boss start function
    public override void Start()
    {
        //the boss controller base start
        base.Start();

        //set the health to 30
        SetHealth(30);

        //seed the random
        Random.InitState(System.DateTime.Now.Millisecond);

        //assign the variables from the scene throwing an exception if any are missing
        try
        {
            m_SpriteRenderer = GetComponent<SpriteRenderer>();

            //flip the sprite at the begining
            if (this.transform.localScale.x > 0)
            {
                Vector3 scaleFlip = this.transform.localScale;
                scaleFlip.x *= -1;

                this.transform.localScale = scaleFlip;
            }

            //assign the positions
            m_v3SecondDropPos = GameObject.Find("Boss_SecondPos").transform.position;
            m_v3SecondUpPos = GameObject.Find("Boss_SecondUpPos").transform.position;
            m_v3DropPos = GameObject.Find("Boss_DropPos").transform.position;
            m_v3UpPos = GameObject.Find("Boss_UpPos").transform.position;

            //add a second audio source for the boss ambient sound
            m_SecondSource = this.gameObject.AddComponent<AudioSource>();
            m_SecondSource.volume = 0.5f;
            m_SecondSource.loop = true;
            m_SecondSource.clip = m_Ambient;

            //set the current position
            m_v3CurrentPos = m_v3UpPos;

            ///get the boss room gravity
            m_RoomGrav = GameObject.Find("Boss Room").GetComponent<Gravity>();

            //find the laser parent holder
            m_LaserParent = GameObject.Find("LaserHolder");

            //initialise the laser object array from the laser parent child count
            m_LaserObjects = new GameObject[m_LaserParent.transform.childCount];

            //for loop getting each child laser 
            for(int i = 0; i < m_LaserParent.transform.childCount; ++i)
            {
                m_LaserObjects[i] = m_LaserParent.transform.GetChild(i).gameObject;
                m_LaserObjects[i].SetActive(false);
            }

            //assign the golden laser and the tracking point 
            m_GoldenLaser = GameObject.Find("GoldenLaser");

            m_TrackingPoint = GameObject.Find("TrackingPoint");

            //disable the tracking object and the golden laser
            m_TrackingPoint.SetActive(false);
            m_GoldenLaser.SetActive(false);

        }
        catch (System.Exception)
        {
            throw new System.Exception("Boss Start fail");
        }

        //set the death sound
        SetDeathClip(m_BossDeath);
    }

    //overridden update function
    public override void Update()
    {
        base.Update();

        //if triggered then update based on gravity
        if(GetIsTriggered())
        {
            GravHandle();
        }

        //behaviour switch statement
        switch (m_eAttackState)
        {
            case ATTACK_STATE.IDLE:


                //if not already triggered and the player is in the boss room
                if (!GetIsTriggered())
                {
                    if (GetPlayer() != null)
                    {
                        if (GetPlayer().GetComponent<PlayerControl>().GetRoom() != null)
                        {
                            if (GetPlayer().GetComponent<PlayerControl>().GetRoom().name == "Boss Room")
                            {
                                //set the boss to is tiggered and lock the boss room

                                SetIsTriggered(true);
                                LockRoom();

                                //start playing the boss ambience
                                m_SecondSource.Play();
                            
                                //call the lasergrid state function after 2 seconds
                                StartCoroutine(DelayedCall(2.0f, LaserGridState));

                            }
                        }
                    }
                }



                break;
            case ATTACK_STATE.LASERGRID:

                LaserGrid();

                break;
            case ATTACK_STATE.TRACKINGLASER:


                TrackingLaser();


                break;
            case ATTACK_STATE.GOLDENLASER:

                GoldenLaser();

                break;
            case ATTACK_STATE.CHARGE:

                Charge();


                break;
            default:
                break;
        }
    }
    
    //gravity handling function
    public void GravHandle()
    {
        //if grayity y is greater than 0
        if(m_RoomGrav.GetGravityDirection().y > 0)
        {
            //if the boss position is greater or equal to the up position y
            if (!(this.transform.position.y >= m_v3UpPos.y))
            {
                //if the sprite scale is above zero multiply y element by -1 to flip on the y axis
                if (this.transform.localScale.y > 0)
                {
                    Vector3 scaleFlip = this.transform.localScale;
                    scaleFlip.y *= -1;

                    this.transform.localScale = scaleFlip;
                }

                //fall towards the new position
                Vector3 newPos = m_RoomGrav.GetGravityDirection() * Gravity.Gravity_Constant * Time.deltaTime;

                this.transform.position = this.transform.position + newPos;
            }

        }

        //same again but for the opposite direction
        else if(m_RoomGrav.GetGravityDirection().y < 0)
        {

            if (!(this.transform.position.y <= m_v3DropPos.y))
            {
                if (this.transform.localScale.y < 0)
                {
                    Vector3 scaleFlip = this.transform.localScale;
                    scaleFlip.y *= -1;

                    this.transform.localScale = scaleFlip;
                }

                Vector3 newPos = m_RoomGrav.GetGravityDirection() * Gravity.Gravity_Constant * Time.deltaTime;

                this.transform.position = this.transform.position + newPos;
            }
        }

    }

    //laser grid update
    public void LaserGrid()
    {
        //play the initial laser grid sound
        PlayAudio(m_LaserStart);

        //start ten lasers apced out by a tuime delay based on the index
        for(int i = 0; i < 10; ++i)
        {
            StartCoroutine(DelayedCall((float)i + m_LaserStart.length, FireRandomLaser));

        }

        //delayed call of change to tracking laser function
        StartCoroutine(DelayedCall(12.0f, TrackingLaserState));

        //switch to idle
        m_eAttackState = ATTACK_STATE.IDLE;

    }

    //function to fire a random laser
    public void FireRandomLaser()
    {
        //get a random index
        int index = Random.Range(0, 4);

        //get the laser from the laser array
        GameObject LaserToFire = m_LaserObjects[index];

        //get the script component  if it is already active then pick a new laser
        if(LaserToFire.GetComponent<Boss_Two_Laser>() != null)
        {
            if(LaserToFire.GetComponent<Boss_Two_Laser>().GetIsActive())
            {
                FireRandomLaser();

                return;
            }
        }

        //start the laser fire routine with this laser
        StartCoroutine(PreLaser(LaserToFire, 0.01f, 1.5f));
    }

    //tracking laser function
    public void TrackingLaser()
    {
        //start the tracking audio
        StartCoroutine(TrackingAudio());
        //start the tracking laser routine
        StartCoroutine(TrackingPointFlash(m_TrackingPoint,4.0f,2.0f));

        //change the staet to golden laser
        m_eAttackState = ATTACK_STATE.GOLDENLASER;
    }

    //golden laser function
    public void GoldenLaser()
    {
        //scale the golden laser object for the explosion while rotating to appear as a spiral
        m_GoldenLaser.transform.localScale += Vector3.one * m_fScaleSpeed * Time.deltaTime;

        Vector3 t_NewRotation = m_GoldenLaser.transform.localEulerAngles;
        t_NewRotation.z += (90.0f * m_fScaleSpeed * Time.deltaTime);

        m_GoldenLaser.transform.localEulerAngles = t_NewRotation;

    }

    public void Charge()
    {

        if (m_v3CurrentPos.x < 0)
        {
            Vector3 newDir = m_v3SecondDropPos - m_v3DropPos;

            newDir = Vector3.Normalize(newDir);

            this.transform.position += newDir * m_fChargeSpeed * Time.deltaTime;

            if (this.transform.position.x >= m_v3SecondDropPos.x)
            {

                if (this.transform.localScale.x < 0)
                {
                    Vector3 scaleFlip = this.transform.localScale;
                    scaleFlip.x *= -1;

                    this.transform.localScale = scaleFlip;
                }

                m_eAttackState = ATTACK_STATE.IDLE;
                StartCoroutine(DelayedCall(1.0f, LaserGridState));

                m_v3CurrentPos = m_v3SecondDropPos;
            }
        }
        else if (m_v3CurrentPos.x > 0)
        {
            Vector3 newDir = m_v3DropPos - m_v3SecondDropPos;

            newDir = Vector3.Normalize(newDir);

            this.transform.position += newDir * m_fChargeSpeed * Time.deltaTime;

            if (this.transform.position.x <= m_v3DropPos.x)
            {

                if (this.transform.localScale.x > 0)
                {
                    Vector3 scaleFlip = this.transform.localScale;
                    scaleFlip.x *= -1;

                    this.transform.localScale = scaleFlip;
                }

                m_eAttackState = ATTACK_STATE.IDLE;
                StartCoroutine(DelayedCall(1.0f, LaserGridState));

                m_v3CurrentPos = m_v3DropPos;

            }
        }

    }

    //state changing functions
    #region StateChangers

    public void IdleState()
    {
        m_eAttackState = ATTACK_STATE.IDLE;
    }

    public void ChargeState()
    {
        m_eAttackState = ATTACK_STATE.CHARGE;
    }

    public void LaserGridState()
    {
        m_eAttackState = ATTACK_STATE.LASERGRID;
    }

    public void TrackingLaserState()
    {
        m_eAttackState = ATTACK_STATE.TRACKINGLASER;
    }

    #endregion

    //get the state 
    public virtual ATTACK_STATE GetState()
    {
        return m_eAttackState;
    }

    //overridden reset function stops all coroutines and sets initial values for variables
    public override void Reset()
    {
        base.Reset();

        SetHealth(30);

        this.transform.position = m_v3UpPos;

        if (this.transform.localScale.x > 0)
        {
            Vector3 scaleFlip = this.transform.localScale;
            scaleFlip.x *= -1;

            this.transform.localScale = scaleFlip;
        }

        m_v3CurrentPos = m_v3UpPos;

        foreach (GameObject laser in m_LaserObjects)
        {
            laser.GetComponent<Boss_Two_Laser>().Reset();
        }

        m_GoldenLaser.GetComponent<Boss_Two_Laser>().Reset();

        StopAllCoroutines();

        if (GetIsTriggered())
        {
            StartCoroutine(DelayedCall(2.0f, LaserGridState));
        }
    }

    //use the base trigger function
    public override void OnTriggerEnter2D(Collider2D collision)
    {
        base.OnTriggerEnter2D(collision);
    }

    //laser grid routine
    public IEnumerator PreLaser(GameObject a_LaserToFire, float a_FlashRate,float a_TimeToFlash)
    {
        //set the laser object to active
        a_LaserToFire.SetActive(true);

        //get the initial colour 
        Color t_startCol = a_LaserToFire.GetComponent<SpriteRenderer>().color;
        //make the object translucent
        t_startCol.a = 0;
        //increment value for alpha channel
        float t_fAlphaChange = 0.1f;
        //set bool to fade in
        bool t_bFadeIn = true;
        //assign the adjusted colour
        a_LaserToFire.GetComponent<SpriteRenderer>().color = t_startCol;
        float t_Timer = 0;

        //start the laser audio
        PlayAudio(m_Laser,.5f);

        //do while loop for alternating between translucent and opaque
        do
        {
            t_Timer += Time.deltaTime;

            if (t_bFadeIn)
            {
                t_startCol.a += t_fAlphaChange;

                if(t_startCol.a >= 0.7f)
                {
                    t_bFadeIn = false;
                }
            }
            else
            {
                t_startCol.a -= t_fAlphaChange;

                if (t_startCol.a <= 0.1f)
                {
                    t_bFadeIn = true;
                }
            }


            a_LaserToFire.GetComponent<SpriteRenderer>().color = t_startCol;
            yield return new WaitForSeconds(a_FlashRate);

        } while (t_Timer < a_TimeToFlash);

        //once the laser is ready to fire set the alpha to opaque
        if(t_Timer >= a_TimeToFlash)
        {
            t_startCol.a = 1;

            a_LaserToFire.GetComponent<SpriteRenderer>().color = t_startCol;

            //set the script to active
            a_LaserToFire.GetComponent<Boss_Two_Laser>().SetIsActive(true);

            //reset the timer
            t_Timer = 0;


            //after 1 second of being active
            yield return new WaitForSeconds(1.0f);

            //deactivate the script
            a_LaserToFire.GetComponent<Boss_Two_Laser>().SetIsActive(false);


        }

        //loop to fade out the laser slowly
        do
        {
            t_startCol.a -= t_fAlphaChange;

            a_LaserToFire.GetComponent<SpriteRenderer>().color = t_startCol;

            yield return new WaitForSeconds(a_FlashRate);

        } while (t_startCol.a > 0.0f);

        //deactivate the laser object
        a_LaserToFire.SetActive(false);

        //end the routine
        yield return null;


    }

    //tracking point routine
    public IEnumerator TrackingPointFlash(GameObject a_TrackingPoint,float a_TimeToTrack, float a_fTrackingSpeed)
    {
        //similarly to the laser routine get initial position and colour values
        m_TrackingPoint.transform.position = GetPlayer().transform.position;

        m_TrackingPoint.SetActive(true);

        Color t_startCol = m_TrackingPoint.transform.GetChild(0).GetComponent<SpriteRenderer>().color;
        t_startCol.a = 0;
        float t_fAlphaChange = 0.1f;
        bool t_bFadeIn = true;
        m_TrackingPoint.transform.GetChild(0).GetComponent<SpriteRenderer>().color = t_startCol;
        float t_Timer = 0;

        //do while loop for following the player and fading in and out on the alpha channel
        do
        {
            t_Timer += Time.deltaTime;

            if (t_bFadeIn)
            {
                t_startCol.a += t_fAlphaChange;

                if (t_startCol.a >= 0.7f)
                {
                    t_bFadeIn = false;
                }
            }
            else
            {
                t_startCol.a -= t_fAlphaChange;

                if (t_startCol.a <= 0.1f)
                {
                    t_bFadeIn = true;
                }
            }

            Vector3 moveDir = Vector3.Normalize(GetPlayer().transform.position - a_TrackingPoint.transform.position);

            a_TrackingPoint.transform.position = a_TrackingPoint.transform.position + moveDir * (a_fTrackingSpeed * Time.deltaTime);

            m_TrackingPoint.transform.GetChild(0).GetComponent<SpriteRenderer>().color = t_startCol;

            yield return new WaitForSeconds(0.01f);

        } while (t_Timer < a_TimeToTrack);


        //once the tracker timer has expires set the alpha to 1 and 

        t_startCol.a = 1;
        m_TrackingPoint.transform.GetChild(0).GetComponent<SpriteRenderer>().color = t_startCol;

        //deactivate the tracking object and launch the Golden laser routine
        m_TrackingPoint.SetActive(false);

        StartCoroutine(GoldenLaser(100.0f, m_TrackingPoint.transform.position));

        yield return null;
    }

    //tracking audio routine
    public IEnumerator TrackingAudio()
    {


        PlayAudio(m_TrackSound);


        yield return null;
    }

    //golden laser routine
    public IEnumerator GoldenLaser(float a_fSpinSpeed, Vector3 a_v3Position)
    {
        //set the golden laser to active
        m_GoldenLaser.SetActive(true);

        //play the explosion audio
        PlayAudio(m_TrackExpand);

        //set its position and scale
        m_GoldenLaser.transform.position = a_v3Position;
        m_GoldenLaser.transform.localScale = Vector3.zero;

        m_GoldenLaser.GetComponent<Boss_Two_Laser>().SetIsActive(true);

        //wait for the scale to increas to maximum scale
        do
        {


            yield return new WaitForSeconds(0.1f);

        } while (m_GoldenLaser.transform.localScale.x < m_fGoldenLaserScaleMax);

        //deactivate the laser object and script
        m_GoldenLaser.GetComponent<Boss_Two_Laser>().SetIsActive(false);
        m_GoldenLaser.SetActive(false);

        //stop the audio
        StopAudio();

        //return the state to idle
        m_eAttackState = ATTACK_STATE.IDLE;

        //start the charge sound
        PlayAudio(m_ChargeSound);

        //initiate the charge state
        StartCoroutine(DelayedCall(1.0f, ChargeState));

        yield return null;
    }
}
