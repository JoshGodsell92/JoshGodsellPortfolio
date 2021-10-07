//////////////////////////////////////////////////////////////////////////
///File name: Action_CheckClosestHide.cs
///Date Created: 22/03/2021
///Created by: JG
///Brief: AI Action for Investigating a Hidingspot nearest the last known location.
//////////////////////////////////////////////////////////////////////////
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Action_CheckClosestHide : AI_Action
{

    private bool m_bHasSearched = false;

    private Vector3 m_v3LastKnownLocation;

    private HidingPlace m_NearbyHidingSpot;

    private float MaxmimumRange = 5.0f;

    public Action_CheckClosestHide()
    {
        //Adds Preconditions to the action
        AddPreCondition("SearchNearbyHide", true);

        //Adds effect of Action
        AddEffect("SearchNearbyHide", false);
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

        SetRange(1.5f);

        //Get the closest Hide to the last known player pos within range
        m_v3LastKnownLocation = a_Agent.GetComponent<AI_Agent>().GetStimulus();

        m_NearbyHidingSpot = FindNearbyHide(m_v3LastKnownLocation);

        if (m_NearbyHidingSpot == null)
        {

            SetTarget(a_Agent.transform.position);

            return true;
        }
        else
        {

            SetTarget(m_NearbyHidingSpot.GetSearchPoint().transform.position);

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
        if (m_NearbyHidingSpot != null)
        {
            //temp distance
            float fDistance;

            Vector3 LeveledPos = new Vector3(GetTarget().x, a_Agent.transform.position.y, GetTarget().z);

            //get distance to the target point
            fDistance = Vector3.Distance(LeveledPos, a_Agent.transform.position);

            //if the distance is less than 1.5f
            if (fDistance <= GetRange())
            {

                //set is in range true
                SetIsInRange(true);

                Guard.ForceUnhide(m_NearbyHidingSpot);

                m_bHasSearched = true;

                a_Agent.GetWorldDataSource().EnactEffect(GetEffects());

                Guard.StartActiveSearch();

            }
            else
            {

                //set is in range to false
                SetIsInRange(false);
            }

            return true;
        }
        else
        {
            //set is in range true
            SetIsInRange(true);

            m_bHasSearched = true;

            a_Agent.GetWorldDataSource().EnactEffect(GetEffects());

            return true;
        }

        return false;
    }

    public HidingPlace FindNearbyHide(Vector3 a_LastPosition)
    {
        HidingPlace HidingPlacePick = null;

        HidingPlace[] AllHidingSpots = FindObjectsOfType<HidingPlace>();

        float distance = 0.0f;

        distance = 5.0f;

        foreach (HidingPlace hide in AllHidingSpots)
        {
            float tempDist = Vector3.Distance(hide.transform.position, a_LastPosition);

            if (distance == 0.0f)
            {
                HidingPlacePick = hide;
                distance = tempDist;
            }
            else if (tempDist <= distance)
            {
                HidingPlacePick = hide;
                distance = tempDist;
            }
        }

        return HidingPlacePick;

    }

}
