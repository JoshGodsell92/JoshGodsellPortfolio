using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Action_Attack_V2 : AI_Action
{
    //AI Blackboard for global data
    AI_Blackboard m_AIBlackboard;

    //Player game object
    private GameObject m_PlayerObject;

    //bool for completed and end game check
    private bool m_bHasAttacked;
    private bool m_bPlayerDefeated = false;

    [SerializeField]
    private AudioSource Source;

    [SerializeField]
    private AudioClip AttackSound;

    [SerializeField]
    private float m_fAttackRange;

    //Default constructor                                           
    public Action_Attack_V2()
    {
        //set action preconditions
        AddPreCondition("Investigating", false);
        AddPreCondition("KnowsPlayer", true);
        AddPreCondition("CanGrab", false);
        AddPreCondition("PlayerGrabbed", true);

        //set action effects
        //AddEffect("PlayerDefeated", true);
        AddEffect("KnowsPlayer", false);
    }

    //Action reset
    public override void Reset(AI_Agent a_AI_Agent)
    {
        //reset the bools
        m_bHasAttacked = false;
        m_bPlayerDefeated = false;

        //stop all current coroutines
        StopAllCoroutines();

        //set is in range to false
        SetIsInRange(false);
    }

    //if the action requires the Agent to be in range of an object to complete
    public override bool RequiresInRange()
    {
        return true;
    }

    public override bool RangedAction()
    {
        return true;
    }

    //check the actions pre-conditions
    public override bool CheckPrecondition(GameObject a_Agent)
    {
        //if the blackboard has yet to be assigned find it
        if (m_AIBlackboard == null)
        {
            m_AIBlackboard = FindObjectOfType<AI_Blackboard>();
        }

        //if the player object is not set
        if (m_PlayerObject == null)
        {
            //get it from the blackboard
            m_PlayerObject = m_AIBlackboard.GetPlayerObject();
        }

        SetRange(m_fAttackRange);

        //if the player object still isnt set return false else set the target and return true
        if (m_PlayerObject == null)
        {

            return false;
        }
        else
        {
            Vector3 AttackVec = m_PlayerObject.transform.position;// - this.transform.position;

            //AttackVec = AttackVec.normalized;

            //AttackVec = m_PlayerObject.transform.position - AttackVec;

            SetTarget(AttackVec);

            return true;
        }

    }
    
    //return if action complete
    public override bool IsComplete()
    {
        return m_bPlayerDefeated;
    }

    //perform action function
    public override bool PerformAction(AI_Agent a_Agent)
    {

        AI_Guard_V2 Guard = (AI_Guard_V2)a_Agent;

        //if the player object isnt null
        if (m_PlayerObject != null)
        {
            if (a_Agent.GetPlayersighted())
            {

                //temp distance
                float fDistance;

                Vector3 LeveledPlayer = new Vector3(m_PlayerObject.transform.position.x, 0.0f, m_PlayerObject.transform.position.z);
                Vector3 LeveledAgent = new Vector3(a_Agent.transform.position.x, 0.0f, a_Agent.transform.position.z);

                Vector3 PlayerLookPos = new Vector3(m_PlayerObject.transform.position.x, this.transform.position.y, m_PlayerObject.transform.position.z);

                //get distance to the target point
                fDistance = Vector3.Distance(LeveledPlayer,LeveledAgent);

                //if the distance is less than 1.5f
                if (fDistance <= m_fAttackRange)
                {

                    Vector3 RotationTarget = PlayerLookPos - a_Agent.transform.position;

                    //rotates the agent toward a target rotation
                    a_Agent.transform.forward = Vector3.RotateTowards(a_Agent.transform.forward, RotationTarget.normalized, 1.5f * Time.deltaTime, 1.0f);

                    //set is in range true
                    SetIsInRange(true);

                    //if not already attacking
                    if (!m_bHasAttacked && Guard.GetActionAvailable())
                    {

                        //Attack and start timer till next attack
                        StartCoroutine(Attack(a_Agent, 1.0f));

                        Guard.StartActionTimer(1.5f);

                        //ensure only one instance of the coroutine is called by setting bool
                        m_bHasAttacked = true;

                    }
                    else
                    {
                        
                    }
                }
                else
                {

                    //set is in range to false
                    SetIsInRange(false);
                }
            }
            else
            {

                a_Agent.RecalculatePath(GetTarget());

                SetIsInRange(false);

            }

            return true;
        }
        return false;

    }


    IEnumerator Attack(AI_Agent a_Agent, float a_fTimeSpent)
    {

        Debug.Log("Attacked Player");

        //Do attack stuff Animation/Damage
        Source.clip = AttackSound;

        Source.Play();

        ((AI_Guard_V2)a_Agent).AttackAnim();

        //m_PlayerObject.GetComponent<PlayerController>().TakeDamage();
        m_PlayerObject.GetComponent<PlayerController>().KillPlayer();

        m_bHasAttacked = false;

        yield return null;

    }

}
