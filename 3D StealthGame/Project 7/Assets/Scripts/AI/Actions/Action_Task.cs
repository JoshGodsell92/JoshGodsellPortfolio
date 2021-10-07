//////////////////////////////////////////////////////////////////////////
///File name: Action_Task.cs
///Date Created: 09/11/2020
///Created by: JG
///Brief: AI Action for doing a task.
///Last Edited by: JG
///Last Edited on: 09/11/2020
//////////////////////////////////////////////////////////////////////////
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Action_Task : AI_Action
{

    AI_Blackboard m_BlackBoard;

    private GameObject m_goTaskObject;

    private bool m_bDoingTask;
    private bool m_bTaskComplete;

    public Action_Task()
    {
        AddPreCondition("AlarmRaised", true);
        AddPreCondition("TaskComplete", false);
        AddPreCondition("KnowsPlayer",false);


        AddEffect("TaskComplete", true);
    }

    public override void Reset(AI_Agent a_AI_Agent)
    {
        m_bTaskComplete = false;
        m_bDoingTask = false;

        SetIsInRange(false);

        if (m_goTaskObject != null)
        {
            m_goTaskObject.GetComponent<DroneTask>().SetIsOccupied(false);
        }

        StopAllCoroutines();
    }

    public override bool CheckPrecondition(GameObject a_AI_Agent)
    {
        if (m_BlackBoard == null)
        {
            m_BlackBoard = FindObjectOfType<AI_Blackboard>();
        }


        if(m_goTaskObject == null)
        {

            m_goTaskObject = a_AI_Agent.GetComponent<AI_Drone>().FindAvailableTask();

            m_goTaskObject.GetComponent<DroneTask>().SetIsOccupied(true);

            SetTarget(m_goTaskObject.transform.position);

            return true;
        }
        else
        {
            SetTarget(m_goTaskObject.transform.position);
            return true;
        }
    }

    public override bool RequiresInRange()
    {
        return true;
    }

    public override bool RangedAction()
    {
        return false;
    }

    public override bool IsComplete()
    {
        return m_bTaskComplete;
    }

    public override bool PerformAction(AI_Agent a_Agent)
    {

        //if the patrol point isnt null
        if (m_goTaskObject != null)
        {

            //temp distance
            float fDistance;

            //get distance to the patrol point
            fDistance = Vector3.Distance(GetTarget(), a_Agent.transform.position);

            //if the distance is less than 1.0f enact the effect and set has patrolled to true
            if (fDistance < 1.5f)
            {
                //if not already investigating
                if (!m_bDoingTask)
                {
                    m_bDoingTask = true;

                    StartCoroutine(TimeForTask(a_Agent, m_goTaskObject.GetComponent<DroneTask>().GetTaskTime()));

                }
                else
                {


                }


            }

            return true;
        }
        return false;

    }


    IEnumerator TimeForTask(AI_Agent a_Agent, float a_fSecondsToWait)
    {
        yield return new WaitForSeconds(a_fSecondsToWait);

        //m_bTaskComplete = true;

        m_goTaskObject.GetComponent<DroneTask>().StartCooldown();

        m_goTaskObject.GetComponent<DroneTask>().SetIsOccupied(false);

        m_bDoingTask = false;

        SetIsInRange(false);

        m_goTaskObject = ((AI_Drone)a_Agent).FindAvailableTask();
        

        //Debug.Log("Task Complete");

        yield return null;

    }



}
