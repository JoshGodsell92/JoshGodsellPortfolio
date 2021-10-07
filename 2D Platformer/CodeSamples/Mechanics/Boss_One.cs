//////////////////////////////////////////////////////////////////
// File Name: Boss_One.cs                                       //
// Author: Josh Godsell                                         //
// Date Created: 15/3/19                                        //
// Date Last Edited: 20/5/19                                    //
// Brief: Boss Controller for the first boss                   //
//////////////////////////////////////////////////////////////////
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Boss_One : BossController {
    
    //boss attack state
    public enum ATTACK_STATE
    {
        IDLE,
        ANGER,
        BOUNCE,
        FIRE,
    }

    //boss current state
    public ATTACK_STATE m_eAttackState = ATTACK_STATE.IDLE;

    //the boss audio clips
    public AudioClip m_Jump;
    public AudioClip m_Shoot;
    public AudioClip m_Death;


    //the left and right positions the boss will stop at
    //private Vector3 m_v3StartPos;
    private Vector3 m_v3SecondPos;

    //the bounce height length and the parabola time frame
    public float m_fBounceHeight;
    public float m_fBounceLength;
    public float m_fSpeed = 1.0f;
    private float m_fParabolaTime = 0;

    //array of the bounce positions and the index for picking from them
    private Vector3[] m_aBouncePositions;
    private int m_iBounceIndex = 0;

    //projectile array
    private BossProjectile[] m_aProjectiles;
    //spread minimum
    private float m_fSpreadMin = 1.5f;
    //has fired bool
    private bool m_bHasFired = false;
    private int m_iAngerCount = 0;

    public override void Start()
    {
        //base start
        base.Start();

        //flip the sprite x
        if (this.transform.localScale.x > 0)
        {
            Vector3 scaleFlip = this.transform.localScale;
            scaleFlip.x *= -1;

            this.transform.localScale = scaleFlip;
        }

        //assign the second position for the boss to move to
        try
        {
            m_v3SecondPos = GameObject.Find("BossLeftPos").transform.position;

        }
        catch (System.Exception)
        {

            throw;
        }

        //initialise the projectiles
        m_aProjectiles = new BossProjectile[5];

        for (int i = 0; i < m_aProjectiles.Length; ++i)
         {
            m_aProjectiles[i] = new BossProjectile();
         }

        //set the death sund
        SetDeathClip(m_Death);

        //calculate the bounce positions
        CalculateBouncePositions();

    }

    //basic fixed update
    public override void FixedUpdate()
    {
        base.FixedUpdate();
    }

    //update
    public override void Update()
    {
        //base update
        base.Update();

        //behaviour switch statement
        switch (m_eAttackState)
        {
            case ATTACK_STATE.IDLE:

                //if the player enters the boss room set is triggered and lock the boss room
                if (!GetIsTriggered())
                {
                    if (GetPlayer() != null)
                    {
                        if (GetPlayer().GetComponent<PlayerControl>().GetRoom() != null)
                        {
                            if (GetPlayer().GetComponent<PlayerControl>().GetRoom().name == "Boss Room")
                            {
                                LockRoom();

                                StartCoroutine(DelayedCall(2.0f, AngerState));
                                SetIsTriggered(true);
                            }
                        }
                    }
                }

                break;
            case ATTACK_STATE.ANGER:

                Anger();

                break;
            case ATTACK_STATE.BOUNCE:

                Bounce();

                break;
            case ATTACK_STATE.FIRE:

                //if has fired is flase then call fire function
                if (m_bHasFired == false)
                {
                    Fire();
                }

                //for each active projectile update it if there are no active projectile then change state
                int t_ActiveCount = m_aProjectiles.Length;

                foreach (BossProjectile projectile in m_aProjectiles)
                {
                    if (projectile.GetIsActive())
                    {
                        projectile.Update();
                    }
                    else
                    {
                        --t_ActiveCount;
                    }

                    if (t_ActiveCount == 0)
                    {
                        StartCoroutine(DelayedCall(1.0f, AngerState));
                        m_eAttackState = ATTACK_STATE.IDLE;
                    }
                }

                break;
            default:
                break;

        }
    }

    
    //bounce function
    public void Bounce()
    {                
        //reset the has fired bool
        if (m_bHasFired == true)
        {
            m_bHasFired = false;
        }
             
        //if the parabola time is 0 the play the jump audio to start
        if(m_fParabolaTime == 0)
        {
            PlayAudio(m_Jump);

        }

        //add selta time to the parabola time
        m_fParabolaTime += Time.deltaTime;

        //get the current element for the bounce position
        Vector3 t_v3CurrentPos = m_aBouncePositions[m_iBounceIndex];
        //get the next element as the next position
        Vector3 t_v3NextPos = m_aBouncePositions[m_iBounceIndex + 1];

        //calculate the where the boss needs to be along a parabola curve for this frame
        Vector3 t_v3NewPos = Parabola(t_v3CurrentPos, t_v3NextPos, m_fBounceHeight, m_fParabolaTime);

        //set the bosses positon to the new position
        this.transform.position = t_v3NewPos;

        //if the this transforms psotion on x axis matches the x component of the next position
        if (this.transform.position.x == t_v3NextPos.x)
        {
            //play the audio each time the boss reaches the target point
            PlayAudio(m_Jump);

            //increment the bounce index
            m_iBounceIndex++;
            //reset the parabola time
            m_fParabolaTime = 0;
            //set this transforms position to the next position
            this.transform.position = t_v3NextPos;

            //if the index has reached the end of the array of positions
            if (m_iBounceIndex >= m_aBouncePositions.Length - 1)
            {
                //set the attack state to fire
                m_eAttackState = ATTACK_STATE.IDLE;

                //reset the bounce index
                m_iBounceIndex = 0;
                //reverse the array of positions
                System.Array.Reverse(m_aBouncePositions);


                //flip the x if the bounce sequence has ended
                Vector3 scaleFlip = this.transform.localScale;
                scaleFlip.x *= -1;

                this.transform.localScale = scaleFlip;


                StartCoroutine(DelayedCall(2.0f, FireState));
            }
        }
    }

    //fire function
    public void Fire()
    {
        //get some target positions for the projectiles
        Vector3[] t_TargetPositions = GetTargetPosition(m_aProjectiles.Length);

        //for each inactive projectile if it is inactive and has no instance instatiate one and set it to active
        //otherwise if it has an instance move it to the boss and set it to active
        int i = 0;
        foreach (BossProjectile projectile in m_aProjectiles)
        {
            if (!projectile.GetIsActive())
            {
                if (projectile.GetInstance() == null)
                {
                    GameObject t_ProjObj = Instantiate(projectile.GetPrefab(), this.transform);

                    projectile.SetInstance(t_ProjObj);
                    projectile.SetPosition(this.transform.position);

                }
                else
                {
                    projectile.GetInstance().SetActive(true);
                    projectile.SetPosition(this.transform.position);
                }


                projectile.SetFallPosition(t_TargetPositions[i]);
                ++i;

                projectile.SetIsActive(true);
            }
        }

        //play the firing sound set has fired to true
        PlayAudio(m_Shoot);

        m_bHasFired = true;
    }

    //anger sequence
    public void Anger()
    {
        if (m_fParabolaTime == 0)
        {
            PlayAudio(m_Jump);

        }

        //add selta time to the parabola time
        m_fParabolaTime += Time.deltaTime;

        //get the current element for the bounce position
        Vector3 t_v3CurrentPos = m_aBouncePositions[m_iBounceIndex];

        Vector3 t_v3NewPos = Parabola(t_v3CurrentPos, t_v3CurrentPos, m_fBounceHeight * 0.5f, m_fParabolaTime * 2.0f);

        //set the bosses positon to the new position
        this.transform.position = t_v3NewPos;

        //for each of the anger jumps play the audio and increment the count
        if (this.transform.position.y <= m_aBouncePositions[m_iBounceIndex].y)
        {

            this.transform.position = m_aBouncePositions[m_iBounceIndex];

            m_fParabolaTime = 0;

            ++m_iAngerCount;

            PlayAudio(m_Jump);

            //once the count reaches the required amount change the boss state
            if (m_iAngerCount >= 3)
            {
                m_eAttackState = ATTACK_STATE.IDLE;
                StartCoroutine(DelayedCall(1.0f, BounceState));
                m_iAngerCount = 0;
            }

        }
    }

    //state changer functions
    #region stateChangers
    public void BounceState()
    {
        m_eAttackState = ATTACK_STATE.BOUNCE;
    }

    public void FireState()
    {
        m_eAttackState = ATTACK_STATE.FIRE;
    }

    public void IdleState()
    {
        m_eAttackState = ATTACK_STATE.IDLE;
    }

    public void AngerState()
    {
        m_eAttackState = ATTACK_STATE.ANGER;
    }
    #endregion

    //function to get random x positions along the top of the screen for each projectile in the array
    public Vector3[] GetTargetPosition(int a_fArraySize)
    {
        Vector3[] t_targetPositions = new Vector3[a_fArraySize];

        float ScreenHeight = Camera.main.orthographicSize * 2.0f;
        float ScreenWidth = ScreenHeight * Screen.width / Screen.height;

        float maxRange = ScreenWidth -= 2.5f;
        float minRange = 1.5f;

        for (int i = 0; i < a_fArraySize; ++i)
        {
            float t_fDiff = 0;
            float t_fRandomX = 0;

            do
            {
                t_fRandomX = Random.Range(minRange, maxRange);

                if (this.transform.position == GetStartPos())
                {
                    t_fRandomX = this.transform.position.x - t_fRandomX;
                }
                else
                {
                    t_fRandomX = this.transform.position.x + t_fRandomX;

                }

                for (int j = 0; j < a_fArraySize; ++j)
                {
                    if (j != i)
                    {
                        t_fDiff = t_fRandomX - t_targetPositions[j].x;

                        if (t_fDiff < 0)
                        {
                            t_fDiff = -t_fDiff;
                        }

                        if (t_fDiff < m_fSpreadMin)
                        {
                            break;
                        }
                    }
                }
            } while (t_fDiff < m_fSpreadMin);


            t_targetPositions[i] = this.transform.position;
            t_targetPositions[i].y += ScreenHeight;
            t_targetPositions[i].x = t_fRandomX;

        }

        return t_targetPositions;
    }

    //function to calculate each bounce position
    public void CalculateBouncePositions()
    {
        //calculates the number of bounces between the boss start and end positions for the bounce length
        int t_iPositions = (int)((GetStartPos().x - m_v3SecondPos.x) / m_fBounceLength);

        //adds one for the start position
        t_iPositions += 1;

        //initialises the array
        m_aBouncePositions = new Vector3[t_iPositions];

        //for each of the values in the array calculate the position at that step
        for (int i = 0; i < t_iPositions; ++i)
        {
            if (i == 0)
            {
                //add the first element as the start position
                m_aBouncePositions[i] = this.transform.position;
            }
            else if (i == t_iPositions)
            {
                //if the current position is the start pos then make the last element the end position
                if (this.transform.position == GetStartPos())
                {
                    m_aBouncePositions[i] = m_v3SecondPos;

                }
            }
            else
            {
                //for each element that isnt the first or last starting from the current position
                //add the bounce length multiplied by the element index to get the position at that stage of the bounce sequence 
                Vector3 t_v3NextPos = GetStartPos();
                t_v3NextPos.x -= (m_fBounceLength * i);

                m_aBouncePositions[i] = t_v3NextPos;
            }
        }
    }

    public override void OnTriggerEnter2D(Collider2D collision)
    {
        base.OnTriggerEnter2D(collision);
    }

    //reset function
    public override void Reset()
    {
        base.Reset();



        if (this.transform.localScale.x > 0)
        {
            Vector3 scaleFlip = this.transform.localScale;
            scaleFlip.x *= -1;

            this.transform.localScale = scaleFlip;
        }

        CalculateBouncePositions();

        if (GetIsTriggered())
        {
            m_eAttackState = ATTACK_STATE.ANGER;
        }

        m_iBounceIndex = 0;
        m_fParabolaTime = 0;
    }

    //get projectiles
    public BossProjectile[] GetProjectiles()
    {
        return m_aProjectiles;
    }

    //getthe state
    public virtual ATTACK_STATE GetState()
    {
        return m_eAttackState;
    }
}
