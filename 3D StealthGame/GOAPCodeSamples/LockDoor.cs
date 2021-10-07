//////////////////////////////////////////////////////////////////////////
///File name: LockDoor.cs
///Date Created: 23/03/2021
///Created by: JG
///Brief: AI Obj Action class for AI to lock certain doors.
//////////////////////////////////////////////////////////////////////////
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LockDoor : AI_ObjAction
{
    //bool for checking when action complete
    private bool m_bUsed;

    //bool for if light switch is switched on
    private bool m_bIsActive;

    public GameObject DoorObject;


    public LockDoor()
    {

        //Adds Preconditions to the action
        AddPreCondition("KnowsPlayer", false);

        AddPreCondition("HasPatrolled", false);

        AddPreCondition("ImprovedChances", false);

        AddSecondaryEffect("Object" + GetID().ToString(), true);

        AddEffect("ImprovedChances", true);

        if (DoorObject != null)
        {
            DoorObject.SetActive(false);
        }
    }

    public void Awake()
    {

        SetTarget(this.gameObject.transform.position);

    }

    public override bool CheckPrecondition(GameObject a_AI_Agent)
    {

        Door doorscript = DoorObject.GetComponent<Door>();

        if(!doorscript.GetLockedState())
        {
            m_bIsActive = false;
        }
        
        SetAvailable(!m_bIsActive);

        if (m_bIsActive)
        {

            a_AI_Agent.GetComponent<AI_Agent>().GetWorldDataSource().EnactEffect(GetSecondaryEffects());

            return false;

        }
        else
        {

            float distance = Vector3.Distance(a_AI_Agent.transform.position, this.gameObject.transform.position);

            SetCost(distance);

            SetTarget(this.gameObject.transform.position);

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
        return m_bUsed;
    }

    public override bool PerformAction(AI_Agent a_Agent)
    {

        if (m_bIsActive)
        {
            m_bUsed = true;

            return true;
        }

        if (GetTarget() != null)
        {
            //temp distance
            float fDistance;

            //get distance to the target point

            Vector3 LeveledPos = new Vector3(GetTarget().x, a_Agent.transform.position.y, GetTarget().z);

            fDistance = Vector3.Distance(LeveledPos, a_Agent.transform.position);

            //if the distance is less than 1.5f
            if (fDistance <= 0.5f)
            {

                //set is in range true
                SetIsInRange(true);

                m_bIsActive = true;

                Door doorScript = DoorObject.GetComponent<Door>();

                doorScript.SetLockedState(true);

                m_bUsed = true;

                a_Agent.GetWorldDataSource().EnactEffect(GetEffects());
                a_Agent.GetWorldDataSource().EnactEffect(GetSecondaryEffects());

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
