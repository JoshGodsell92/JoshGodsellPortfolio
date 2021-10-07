//////////////////////////////////////////////////////////////////////////
///File name: UnblockDoor.cs
///Date Created: 24/03/2021
///Created by: JG
///Brief: AI Obj Action class for AI to unblock blocked doors.
//////////////////////////////////////////////////////////////////////////
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UnblockDoor : AI_Action
{

    //bool for when complete
    private bool m_bIsUnblocked;

    public GameObject  DoorBlocker;

    public UnblockDoor()
    {

        //Adds Preconditions to the action
        AddPreCondition("KnowsPlayer", false);

        AddPreCondition("HasPatrolled", false);

        AddPreCondition("BlockedDoor", true);

        AddEffect("BlockedDoor", false);

    }

    public override bool CheckPrecondition(GameObject a_AI_Agent)
    {

        if(true)
        {

            return false;
        }
        else
        {


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
        return m_bIsUnblocked;
    }

    public override bool PerformAction(AI_Agent a_Agent)
    {

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
