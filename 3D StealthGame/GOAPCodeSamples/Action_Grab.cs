using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Action_Grab : AI_Action
{
    //AI Blackboard for global data
    AI_Blackboard m_AIBlackboard;

    //Player game object
    private GameObject m_PlayerObject;

    private bool m_bGrabbedPlayer;

    [SerializeField]
    private float m_fGrabRange;

    public Action_Grab()
    {
        AddPreCondition("CanGrab", true);
        AddPreCondition("Investigating", false);
        AddPreCondition("KnowsPlayer", true);

        AddEffect("CanGrab", false);
        AddEffect("PlayerGrabbed", true);
    }

    //Action reset
    public override void Reset(AI_Agent a_AI_Agent)
    {
        //reset the bools
        m_bGrabbedPlayer = false;

        //stop all current coroutines
        StopAllCoroutines();

        //set is in range to false
        SetIsInRange(false);
    }

    public override bool CheckPrecondition(GameObject a_AI_Agent)
    {
        //if the blackboard has yet to be assigned find it
        if (m_AIBlackboard == null)
        {
            m_AIBlackboard = FindObjectOfType<AI_Blackboard>();
        }

        SetRange(m_fGrabRange);

        //if the player object is not set
        if (m_PlayerObject == null)
        {
            //get it from the blackboard
            m_PlayerObject = m_AIBlackboard.GetPlayerObject();
        }

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

    //if the action requires the Agent to be in range of an object to complete
    public override bool RequiresInRange()
    {
        return true;
    }

    public override bool RangedAction()
    {
        return true;
    }

    //return if action complete
    public override bool IsComplete()
    {
        return m_bGrabbedPlayer;
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

                Vector3 PlayerLookPos = new Vector3(m_PlayerObject.transform.position.x, this.transform.position.y, m_PlayerObject.transform.position.z);

                //temp distance
                float fDistance;

                Vector3 LeveledPos = new Vector3(m_PlayerObject.transform.position.x, a_Agent.transform.position.y, m_PlayerObject.transform.position.z);

                //get distance to the target point
                fDistance = Vector3.Distance(LeveledPos, a_Agent.transform.position);

                //if the distance is less than 1.5f
                if (fDistance <= m_fGrabRange + 0.2f)
                {


                    Vector3 RotationTarget = PlayerLookPos - a_Agent.transform.position;

                    //rotates the agent toward a target rotation
                    a_Agent.transform.forward = Vector3.RotateTowards(a_Agent.transform.forward, RotationTarget.normalized, 1.5f * Time.deltaTime, 1.0f);


                    //set is in range true
                    SetIsInRange(true);

                    bool isGrabbed = m_PlayerObject.GetComponent<PlayerController>().GetGrabbed();

                    //if not already attacking
                    if (!m_bGrabbedPlayer && Guard.GetActionAvailable() && !isGrabbed)
                    {

                        Guard.GrabPlayer();

                        Guard.StartActionTimer(1.5f);

                        a_Agent.GetWorldDataSource().EnactEffect(GetEffects());


                        //ensure only one instance of the coroutine is called by setting bool
                        m_bGrabbedPlayer = true;

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


}
