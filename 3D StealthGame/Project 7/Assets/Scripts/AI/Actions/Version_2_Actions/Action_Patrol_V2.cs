//////////////////////////////////////////////////////////////////////////
///File name: Action_Patrol_V2.cs
///Date Created: 04/02/2021
///Created by: JG
///Brief: AI Action for Patrolling.
//////////////////////////////////////////////////////////////////////////
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Action_Patrol_V2 : AI_Action
{

    //Array of patrol points inh order
    public PatrolPoint[] m_aPatrolPoints;

    //array indexer
    private int m_iPatrolIndex;

    //Time To wait at each point
    public float m_fWaitTime;   

    //bool for if patrol path is looped
    public bool m_bLoopedPath;

    //Bool for inverting path direction
    private bool m_bInvert = false;

    //Bool to check for action complete
    private bool m_bHasPatrolled;

    //Bool for waiting at point
    private bool m_bIsWaiting = false;


    //Constructor
    public Action_Patrol_V2()
    {


        //Adds Preconditions to the action
        AddPreCondition("HasPatrolled", false);
        AddPreCondition("ActiveSearch", false);
        AddPreCondition("ImprovedChances", true);
        AddPreCondition("Investigating", false);
        AddPreCondition("KnowsPlayer", false);

        //Adds effect of Action
        AddEffect("HasPatrolled", true);
        //AddEffect("KnowsPlayer", true);
    }

    //Resets the action
    public override void Reset(AI_Agent a_Agent)
    {
        m_bHasPatrolled = false;

        StopAllCoroutines();

        m_bIsWaiting = false;

        SetIsInRange(false);
    }

    //Checks for additional preconditions and sets action variables
    public override bool CheckPrecondition(GameObject a_Agent)
    {

        if (m_aPatrolPoints == null)
        {
  
            return false;
        }
        else
        {

            SetTarget(m_aPatrolPoints[m_iPatrolIndex].GetPosition());

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
        return false;
    }

    //condition check
    public override bool IsComplete()
    {
        return m_bHasPatrolled;
    }

    //holds the performance of the action
    public override bool PerformAction(AI_Agent a_Agent)
    {
        //if the patrol point isnt null
        if (m_aPatrolPoints != null)
        {

            //temp distance
            float fDistance;

            //get distance to the patrol point
            fDistance = Vector3.Distance(GetTarget(), a_Agent.transform.position);

            //if the distance is less than 1.0f enact the effect and set has patrolled to true
            if (fDistance <= 1.5f)
            {
                if (!m_bIsWaiting)
                {
                    StartCoroutine(PatrolWait(m_fWaitTime));
                }

            }

            return true;
        }

        return false;
    }

    public IEnumerator PatrolWait(float a_fSecondsToWait)
    {
        m_bIsWaiting = true;


        yield return new WaitForSeconds(a_fSecondsToWait);

        if (!m_bLoopedPath)
        {
            if (!m_bInvert)
            {
                m_iPatrolIndex++;

                if (m_iPatrolIndex == m_aPatrolPoints.Length - 1)
                {
                    m_bInvert = true;
                }
            }
            else
            {
                m_iPatrolIndex--;

                if (m_iPatrolIndex == 0)
                {
                    m_bInvert = false;
                }
            }
        }
        else
        {

            m_iPatrolIndex++;

            if(m_iPatrolIndex == m_aPatrolPoints.Length)
            {
                m_iPatrolIndex = 0;
            }

        }

        SetTarget(m_aPatrolPoints[m_iPatrolIndex].GetPosition());

        SetIsInRange(false);

        m_bIsWaiting = false;

        yield return null;
    }

    private void OnDrawGizmosSelected()
    {

        if (m_aPatrolPoints.Length > 0)
        {
            for (int i = 1; i < m_aPatrolPoints.Length; i++)
            {
                Gizmos.color = Color.red;

                Gizmos.DrawLine(m_aPatrolPoints[i - 1].GetPosition(), m_aPatrolPoints[i].GetPosition());
            }
        }
    }
}
