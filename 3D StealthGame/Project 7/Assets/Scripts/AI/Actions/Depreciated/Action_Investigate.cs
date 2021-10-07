//////////////////////////////////////////////////////////////////////////
///File name: Action_Investigate.cs
///Date Created: 15/10/2020
///Created by: JG
///Brief: AI Action for Investigating a sensory input.
///Last Edited by: JG
///Last Edited on: 03/12/2020
//////////////////////////////////////////////////////////////////////////

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Action_Investigate : AI_Action
{

    AI_Blackboard m_BlackBoard;

    //Radius of the objects to check
    [SerializeField]
    private float m_fCheckRadius;

    //Bool for complete test
    private bool m_bHasInvestigated = false;

    //Bool for when the Agent is investigating
    private bool m_bInvestigating = false;

    //Target Location
    private Vector3 m_v3StimulusPos;

    public float m_fTimeToInvestigate;

    //Target Rotation
    private Vector3 m_v3RotateTarget;

    public Action_Investigate()
    {
        //Adds Preconditions to the action
        AddPreCondition("InPursuit", false);
        AddPreCondition("Investigating", true);

        //Adds effect of Action
        AddEffect("Investigating", false);
        AddEffect("KnowsPlayer", true);
    }

    public override void Reset(AI_Agent a_Agent)
    {
        StopAllCoroutines();

        m_bHasInvestigated = false;

        m_bInvestigating = false;

        m_v3RotateTarget = Vector3.zero;

        SetIsInRange(false);

    }

    //Checks for additional preconditions and sets action variables
    public override bool CheckPrecondition(GameObject a_Agent)
    {
        if(m_BlackBoard == null)
        {
            m_BlackBoard = FindObjectOfType<AI_Blackboard>();
        }

        Vector3 t_v3StimulusPos = a_Agent.GetComponent<AI_Agent>().GetStimulus();


        if (t_v3StimulusPos != Vector3.zero)
        {
            m_v3StimulusPos = t_v3StimulusPos;
        }

        if ( m_v3StimulusPos == Vector3.zero)
        {

            return false;
        }
        else
        {

            SetTarget(m_v3StimulusPos);

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
        return m_bHasInvestigated;
    }


    //holds the performance of the action
    public override bool PerformAction(AI_Agent a_Agent)
    {   
        //if the patrol point isnt null
        if (m_v3StimulusPos != Vector3.zero)
        {
            if(!a_Agent.GetAlert() && !m_bHasInvestigated)
            {
                a_Agent.SetAlert(true);
            }

            if (a_Agent.GetStimulus() != m_v3StimulusPos)
            {
                Debug.Log("Stimulus change");

                StopAllCoroutines();

                m_bInvestigating = false;

                m_v3StimulusPos = a_Agent.GetStimulus();

                SetIsInRange(false);
            }
            else
            {
                //temp distance
                float fDistance;

                //get distance to the patrol point
                fDistance = Vector3.Distance(GetTarget(), a_Agent.transform.position);

                //if the distance is less than 1.0f enact the effect and set has patrolled to true
                if (fDistance < 1.5f)
                {
                    //if not already investigating
                    if (!m_bInvestigating)
                    {

                        //start the investigation timer
                        StartCoroutine(LookAround(a_Agent, 6.0f));

                        //set investigation to true
                        m_bInvestigating = true;

                        //start the roatation coroutine
                        StartCoroutine(Rotate(a_Agent));

                    }
                    else
                    {

                        //rotates the agent toward a target rotation
                        a_Agent.transform.forward = Vector3.RotateTowards(a_Agent.transform.forward, m_v3RotateTarget, 1.5f * Time.deltaTime, 1.0f);


                    }


                }
            }

            return true;
        }
        return false;
    }


    IEnumerator LookAround(AI_Agent a_Agent, float a_fTimeSpent)
    {
        yield return new WaitForSeconds(a_fTimeSpent);

        /*
        m_bHasInvestigated = true;

        m_bInvestigating = false;

        RemoveEffect("KnowsPlayer");

        a_Agent.SetAlert(false);

        a_Agent.GetWorldDataSource().EnactEffect(GetEffects());

        AddEffect("KnowsPlayer", true);
          */

        StartCoroutine(Investigation(a_Agent));

        yield return null;
    }

    //coroutine for how long the Agent spends investigating
    IEnumerator Investigation(AI_Agent a_Agent)
    {

        Collider[] colliders = Physics.OverlapSphere(transform.position, m_fCheckRadius);

        List<HidingPlace> hidingSpots = new List<HidingPlace>();

        for (int i = 0; i < colliders.Length; i++)
        {
            HidingPlace interact = colliders[i].transform.GetComponentInParent<HidingPlace>();
            if (interact != null)
            {

                Vector3 TargetDir = colliders[i].gameObject.transform.position - a_Agent.gameObject.transform.position;

                if (Physics.Raycast(a_Agent.gameObject.transform.position, TargetDir, 20.0f))
                {
                    hidingSpots.Add(interact);
                }

            }
        }

        if (hidingSpots.Count > 0)
        {

            int Pick = Random.Range(0, hidingSpots.Count);

            SetTarget(hidingSpots[Pick].GetSearchPoint().transform.position);

            //SetIsInRange(false);

            a_Agent.GetNavAgent().SetDestination(GetTarget());

            float fDistance;

            do
            {
                //get distance to the point
                fDistance = Vector3.Distance(hidingSpots[Pick].GetSearchPoint().transform.position, a_Agent.gameObject.transform.position);

                yield return new WaitForFixedUpdate();


            } while (fDistance > 1.5f);

            yield return new WaitForSeconds(0.5f);

            hidingSpots[Pick].AIInteract(a_Agent);

        }

        m_bHasInvestigated = true;

        m_bInvestigating = false;

        RemoveEffect("KnowsPlayer");

        a_Agent.SetAlert(false);

        a_Agent.GetWorldDataSource().EnactEffect(GetEffects());

        AddEffect("KnowsPlayer", true);

        yield return null;
    }

    //coroutine for simple look left and right
    IEnumerator Rotate(AI_Agent a_Agent)
    {
        Vector3 t_v3Forward = a_Agent.gameObject.transform.forward;
        Vector3 t_v3Right = a_Agent.gameObject.transform.right;
        Vector3 t_v3Left = -a_Agent.gameObject.transform.right;

        m_v3RotateTarget = t_v3Right;

        do
        {

            Debug.DrawRay(a_Agent.transform.position,t_v3Forward);
            Debug.DrawRay(a_Agent.transform.position, t_v3Right);
            Debug.DrawRay(a_Agent.transform.position, t_v3Left);

            if (a_Agent.transform.forward == t_v3Right)
            {
                m_v3RotateTarget = t_v3Left;

            }
            else if(a_Agent.transform.forward == t_v3Left)
            {
                m_v3RotateTarget = t_v3Right;

            }

            yield return new WaitForFixedUpdate();

        } while (m_bInvestigating);

        if(!m_bInvestigating)
        {
            yield return null;
        }

    }

}
