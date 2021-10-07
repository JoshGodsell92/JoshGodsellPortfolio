//////////////////////////////////////////////////////////////////////////
///File name: Action_PAtrol.cs
///Date Created: 08/10/2020
///Created by: JG
///Brief: AI Action for patrolling around a set of patrol points.
///Last Edited by: JG
///Last Edited on: 15/10/2020
//////////////////////////////////////////////////////////////////////////

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class Action_Patrol : AI_Action
{

    //The number of patrol points to use
    public int m_iPatrolCount;

    //Array of patrol points
    public PatrolPoint[] m_aPatrolPoints;

    //Bool for path set at initial run
    private bool m_bPathSet;

    //Bool for Loop or Invert
    public bool m_bLoop;

    //Time to wait at each point
    public float m_fWaitFor;


    //Target Index
    private int m_iIndex = 0;

    //Bool to check for action complete
    private bool m_bHasPatrolled;

    //Bool for waiting at point
    private bool m_bIsWaiting = false;

    //Bool for inverting path direction
    private bool m_bInvert = false;

    //Constructor
    public Action_Patrol()
    {
        //Adds Preconditions to the action
        AddPreCondition("InPursuit", false);
        AddPreCondition("HasPatrolled", false);
        AddPreCondition("Investigating", false);

        //Adds effect of Action
        AddEffect("HasPatrolled", true);
        AddEffect("KnowsPlayer", true);
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
        if (!m_bPathSet)
        {
            GeneratePatrolPath(a_Agent);
            m_bPathSet = true;
        }

        if (m_aPatrolPoints == null)
        {
            return false;
        }
        else
        {

            SetTarget(m_aPatrolPoints[m_iIndex].GetPosition());

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
                if(!m_bIsWaiting)
                {
                    StartCoroutine(PatrolWait(m_fWaitFor));
                }

            }

            return true;
        }

        return false;
    }

    public void GeneratePatrolPath(GameObject a_Agent)
    {
        PatrolPoint[] t_aPatrolPoints = GameObject.FindObjectsOfType<PatrolPoint>();

        PatrolPoint[] t_PatrolPath = new PatrolPoint[m_iPatrolCount];

        //temp distances
        float fDistance;
        float fPrevDistance = 0.0f;

        PatrolPoint t_StartPoint = null;

        NavMeshPath t_Path = new NavMeshPath();

        //Find the closest point to the agent
        foreach (PatrolPoint point in t_aPatrolPoints)
        {
            fDistance = Vector3.Distance(point.GetPosition(), a_Agent.transform.position);

            if (fPrevDistance == 0.0f)
            {
                fPrevDistance = fDistance;
                t_StartPoint = point;

            }
            else if (fPrevDistance > fDistance)
            {
                fPrevDistance = fDistance;
                t_StartPoint = point;
            }
        }

        //assign the closest point as the start point
        t_PatrolPath[0] = t_StartPoint;

        //temp array for neighbours of a point
        GameObject[] t_tempNeighbours;
        int t_iRandIndex;

        //get a random neighbour to assign from each point to create a array aas a path
        for(int i = 1; i < t_PatrolPath.Length; i++)
        {
            t_tempNeighbours = t_PatrolPath[i-1].GetComponent<PatrolPoint>().GetNeighbours();

            if (t_tempNeighbours.Length > 1)
            {
                int loopCount = 0;

                do
                {
                    t_iRandIndex = Random.Range(0, t_tempNeighbours.Length);

                    
                    if(loopCount > 10)
                    {
                        break;
                    }

                    loopCount++;

                } while (CheckPath(t_tempNeighbours[t_iRandIndex], t_PatrolPath));

                if(!CheckPath(t_tempNeighbours[t_iRandIndex], t_PatrolPath))
                {
                    
                }


            }
            else
            {
                t_iRandIndex = 0;
            }

            t_PatrolPath[i] = t_tempNeighbours[t_iRandIndex].GetComponent<PatrolPoint>();

        }

        m_aPatrolPoints = t_PatrolPath;

    }


    //function to check if game object is already in the path
    public bool CheckPath(GameObject a_GameObject, PatrolPoint[] a_aGameObjectArray)
    {

        for(int i = 0; i < a_aGameObjectArray.Length; i++)
        {

            if (a_aGameObjectArray[i] != null)
            {
                if (a_GameObject == a_aGameObjectArray[i].gameObject)
                {
                    return true;
                }
            }

        }

        return false;
    }

    public bool CheckAllNeighbours(PatrolPoint[] a_aPatrolPoints, GameObject[] a_aNeighbours)
    {
        int t_iCount = 0;

        for (int i = 0; i < a_aNeighbours.Length; i++)
        {
            for (int j = 0; j < a_aPatrolPoints.Length; j++)
            {

                if (a_aPatrolPoints[j] != null)
                {
                    if (a_aNeighbours[i] == a_aPatrolPoints[j].gameObject)
                    {
                        t_iCount++;
                    }
                }
            }
        }

        if(t_iCount == a_aNeighbours.Length-1)
        {
            return false;
        }

        return true;
    }

    //Coroutine for waiting at each patrol point and then moving to the next
    public IEnumerator PatrolWait(float a_fSecondsToWait)
    {
        m_bIsWaiting = true;


        yield return new WaitForSeconds(a_fSecondsToWait);

        if(!m_bInvert)
        {
            m_iIndex++;

            if(m_iIndex == m_aPatrolPoints.Length -1)
            {
                m_bInvert = true;
            }
        }
        else
        {
            m_iIndex--;

            if (m_iIndex == 0)
            {
                m_bInvert = false;
            }
        }

        SetTarget(m_aPatrolPoints[m_iIndex].GetPosition());

        SetIsInRange(false);

        m_bIsWaiting = false;

        yield return null;
    }

    private void OnDrawGizmosSelected()
    {

        if(m_aPatrolPoints.Length > 0)
        {
            for (int i = 1; i < m_aPatrolPoints.Length; i++)
            {
                Gizmos.color = Color.red;

                Gizmos.DrawLine(m_aPatrolPoints[i - 1].GetPosition(), m_aPatrolPoints[i].GetPosition());
            }

            //Gizmos.DrawLine(m_aPatrolPoints[4].GetPosition(), m_aPatrolPoints[0].GetPosition());
        }
    }
}
