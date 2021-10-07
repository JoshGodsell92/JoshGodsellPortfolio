//////////////////////////////////////////////////////////////////////////
///File name: Action_CheckHide.cs
///Date Created: 16/02/2021
///Created by: JG
///Brief: AI Action for Investigating a Hidingspot.
//////////////////////////////////////////////////////////////////////////
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Action_CheckHide : AI_Action
{

    private bool m_bHasSearched = false;

    private Vector3 m_v3HidingSpot;  

    public Action_CheckHide()
    {
        //Adds Preconditions to the action
        AddPreCondition("ActiveSearch", true);

        //Adds effect of Action
        AddEffect("ActiveSearch", false);

    }

    public override void Reset(AI_Agent a_Agent)
    {
        StopAllCoroutines();

        m_bHasSearched = false;

        SetIsInRange(false);
    }

    //Checks for additional preconditions and sets action variables
    public override bool CheckPrecondition(GameObject a_Agent)
    {
        m_v3HidingSpot = a_Agent.GetComponent<AI_Agent>().GetStimulus();

        if(m_v3HidingSpot == Vector3.zero)
        {



            return false;
        }
        else
        {

            Vector3 LevelVec = new Vector3(m_v3HidingSpot.x, this.transform.position.y, m_v3HidingSpot.z);

            Vector3 HidingVec = LevelVec - this.transform.position;


            HidingVec = HidingVec.normalized;

            HidingVec = LevelVec - HidingVec;

            SetTarget(HidingVec);

            return true;
        }


    }

    public override bool IsComplete()
    {
        return m_bHasSearched;
    }

    public override bool RequiresInRange()
    {
        return true;
    }

    public override bool RangedAction()
    {
        return false;
    }

    public override bool PerformAction(AI_Agent a_Agent)
    {
        AI_Guard_V2 Guard = (AI_Guard_V2)a_Agent;

        //if the player object isnt null
        if (m_v3HidingSpot != null)
        {
                //temp distance
                float fDistance;

                //get distance to the target point
                fDistance = Vector3.Distance(GetTarget(), a_Agent.transform.position);

                //if the distance is less than 1.5f
                if (fDistance <= GetRange())
                {

                    //set is in range true
                    SetIsInRange(true);

                    Guard.ForceUnhide();

                    m_bHasSearched = true;

                    a_Agent.GetWorldDataSource().EnactEffect(GetEffects());

                    

                }
                else
                {

                    //set is in range to false
                    SetIsInRange(false);
                }

            return true;
        }
        return false;
    }






}
