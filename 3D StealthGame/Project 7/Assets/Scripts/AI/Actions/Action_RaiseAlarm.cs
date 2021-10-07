//////////////////////////////////////////////////////////////////////////
///File name: Action_RaiseAlarm.cs
///Date Created: 10/11/2020
///Created by: JG
///Brief: AI Action for Raising the Alarm.
///Last Edited by: JG
///Last Edited on: 10/11/2020
//////////////////////////////////////////////////////////////////////////
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Action_RaiseAlarm : AI_Action
{

    AI_Blackboard m_BlackBoard;

    private GameObject m_goAlarmObject;

    private bool m_bAlarmRaised;
    private bool m_bUsingAlarm;


    public Action_RaiseAlarm ()
    {
        AddPreCondition("KnowsPlayer", false);
        AddPreCondition("AlarmRaised", false);

        AddEffect("AlarmRaised",true);
    }

    public override void Reset(AI_Agent a_AI_Agent)
    {
        m_bAlarmRaised = false;
        m_bUsingAlarm = false;

        m_goAlarmObject = null;

        SetIsInRange(false);

        StopAllCoroutines();
    }

    public override bool CheckPrecondition(GameObject a_AI_Agent)
    {

        if (m_BlackBoard == null)
        {
            m_BlackBoard = FindObjectOfType<AI_Blackboard>();
        }

        List<GameObject> t_lAlarms = new List<GameObject>();

        foreach (GameObject go in m_BlackBoard.GetAlarms())
        {
            if (go.GetComponent<Alarms>().GetInteractable())
            {
                t_lAlarms.Add(go);
            }
        }

        if (t_lAlarms.Count > 0)
        {
            if (m_goAlarmObject == null)
            {
                m_goAlarmObject = a_AI_Agent.GetComponent<AI_Drone>().FindShortPath(t_lAlarms);

                SetTarget(m_goAlarmObject.transform.position);

                return true;
            }
            else
            {
                SetTarget(m_goAlarmObject.transform.position);

                return true;
            }
        }
        else
        {
            Debug.Log("Alarm already raised");

            m_bAlarmRaised = true;
            a_AI_Agent.GetComponent<AI_Agent>().GetWorldDataSource().EnactEffect(GetEffects());

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
        return m_bAlarmRaised;
    }

    public override bool PerformAction(AI_Agent a_Agent)
    {

        //if the patrol point isnt null
        if (m_goAlarmObject != null)
        {

            //temp distance
            float fDistance;

            //get distance to the patrol point
            fDistance = Vector3.Distance(GetTarget(), a_Agent.transform.position);

            //if the distance is less than 1.0f enact the effect and set has patrolled to true
            if (fDistance < 1.5f)
            {
                //if not already investigating
                if (!m_bAlarmRaised)
                {
                    
                    if(!m_bUsingAlarm)
                    {
                        m_bUsingAlarm = true;

                        StartCoroutine(RaiseAlarm(a_Agent));
                    }
                    

                }
                else
                {


                }


            }

            return true;
        }
        return false;


    }


    IEnumerator RaiseAlarm(AI_Agent a_Agent)
    {

        Alarms AlarmScript = m_goAlarmObject.GetComponent<Alarms>();

        yield return new WaitForSeconds(AlarmScript.GetInteractTime());

        m_bAlarmRaised = true;

        m_BlackBoard.SetIsAlarmRaised(true);

        m_goAlarmObject.GetComponent<Alarms>().AIInteract(a_Agent);

        a_Agent.SetAlert(false);

        a_Agent.GetWorldDataSource().EnactEffect(GetEffects());

        yield return null;

    }

}
