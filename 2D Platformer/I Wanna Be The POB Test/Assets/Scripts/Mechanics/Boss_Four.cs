//////////////////////////////////////////////////////////////////
// File Name: Boss_Four.cs                                       //
// Author: Josh Godsell                                         //
// Date Created: 30/5/19                                        //
// Date Last Edited: 30/5/19                                    //
// Brief: Boss Controller for the fourth boss                   //
//////////////////////////////////////////////////////////////////
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Boss_Four : BossController {

    //boss states
    public enum ATTACK_STATE
    {
        IDLE,
        THROW,
        MOVE
    }

    //current state
    private ATTACK_STATE m_eAttackState;

    //the two boss positions
    private GameObject m_BossPositionOne;
    private GameObject m_BossPositionTwo;

    //the current position object
    private GameObject m_CurrentPlatform;

    //projectile speed
    private float m_fSpeed = 4.0f;

    //spear prefab
    public GameObject m_SpearPrefab;

    //bool has moved
    private bool m_bMoved = false;

    //bool for thrown spears
    private bool m_bThrownSpears = false;

    //overridden start
    public override void Start()
    {
        base.Start();

        //set base health to 60
        SetHealth(60);

        //set initial state to idle
        m_eAttackState = ATTACK_STATE.IDLE;

        //assign the two  boss positions
        try
        {
            m_BossPositionOne = GameObject.Find("BossPlatform");
            m_BossPositionTwo = GameObject.Find("BossPlatform2");
        }
        catch (System.Exception)
        {

            throw;
        }

        //set the current position
        m_CurrentPlatform = m_BossPositionTwo;
    }

    //update
    public override void Update()
    {
        base.Update();

       

        switch(m_eAttackState)
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

                                LockRoom();

                                StartCoroutine(DelayedCall(2.0f, ThrowState));

                                SetIsTriggered(true);
                            }
                        }
                    }
                }
                break;
            case ATTACK_STATE.THROW:

                //if the bosses health drops below 30 move to the other platform
                if (!m_bMoved && GetHealth() <= 30)
                {
                    MoveState();
                    m_bMoved = true;
                }

                Throw();

                break;
            case ATTACK_STATE.MOVE:

                MovePlatform();

                break;
            default:
                break;
        }
    }

    //function fo if not thrown spears start the throw coroutine
    public void Throw()
    {
         if(!m_bThrownSpears)
        {
            StartCoroutine(ThrowSpears(3));
        }
    }

    //function to move bewteen the two platforms
    public void MovePlatform()
    {
        if(m_CurrentPlatform == m_BossPositionTwo)
        {
            Vector3 moveDir = Vector3.Normalize(m_BossPositionOne.transform.position - this.transform.position);

            this.transform.position += moveDir * m_fSpeed * Time.deltaTime;

            if(this.transform.position.y >= m_BossPositionOne.transform.position.y)
            {
                m_CurrentPlatform = m_BossPositionOne;
                IdleState();
                StartCoroutine(DelayedCall(1.0f, ThrowState));
            }
        }
        else  if(m_CurrentPlatform == m_BossPositionOne)
        {
            Vector3 moveDir = Vector3.Normalize(m_BossPositionTwo.transform.position - this.transform.position);

            this.transform.position += moveDir * m_fSpeed * Time.deltaTime;

            if (this.transform.position.y <= m_BossPositionTwo.transform.position.y)
            {
                m_CurrentPlatform = m_BossPositionTwo;
                IdleState();
                StartCoroutine(DelayedCall(1.0f, ThrowState));
            }
        }
    }

    //state change functions
    #region stateChangers
    public void ThrowState()
    {
        m_eAttackState = ATTACK_STATE.THROW;
    }
    public void IdleState()
    {
        m_eAttackState = ATTACK_STATE.IDLE;
    }
    public void MoveState()
    {
        m_eAttackState = ATTACK_STATE.MOVE;
    }
    #endregion

    //reset function
    public override void Reset()
    {
        base.Reset();

        m_bMoved = false;

        StopAllCoroutines();

        if (GetIsTriggered())
        {
            m_CurrentPlatform = m_BossPositionTwo;
            this.transform.position = m_BossPositionTwo.transform.position;

            IdleState();
            StartCoroutine(DelayedCall(1.0f, ThrowState));

            Spear[] t_Spears = FindObjectsOfType<Spear>();

            m_bThrownSpears = false;


            for (int i = 0; i < t_Spears.Length; ++i)
            {
                t_Spears[i].Reset();
            }
        }

        SetHealth(60);

    }

    public override void OnTriggerEnter2D(Collider2D collision)
    {
        base.OnTriggerEnter2D(collision);
    }

    //throw spear sequence
    public IEnumerator ThrowSpears(int m_iSpearsToThrow)
    {
        //set the has thrown spears to tru
        m_bThrownSpears = true;

        //for loop to throught the parsed number of spears
        for (int i = 0; i < m_iSpearsToThrow; ++i)
        {

            //instantiate the prefab and set the spear script variables
            GameObject t_Instance = Instantiate(m_SpearPrefab, this.transform);

            t_Instance.GetComponent<Spear>().m_v3StartPos = this.transform.position;

            t_Instance.GetComponent<Spear>().m_v3TargetPos = this.GetPlayer().transform.position;

            t_Instance.transform.parent = null;


            //wait between throws
            yield return new WaitForSeconds(2.0f);
        }

        //after three seconds reset the bool to throw spears again
        yield return new WaitForSeconds(3.0f);

        m_bThrownSpears = false;

        yield return null;
    }
}
