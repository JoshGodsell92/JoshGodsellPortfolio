//////////////////////////////////////////////////////////////////////////
///File name: TrapSwitch.cs
///Date Created: 02/03/2021
///Created by: JG
///Brief: AI Obj Action class for AI switching on a trap.
//////////////////////////////////////////////////////////////////////////

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TrapSwitch : AI_ObjAction
{
    //bool for checking when action complete
    private bool m_bUsed;

    //bool for if trap is switched on
    private bool m_bIsActive;

    public GameObject TrapObject;

    public TrapSwitch()
    {
        //Adds Preconditions to the action
        AddPreCondition("KnowsPlayer", false);

        AddPreCondition("HasPatrolled", false);

        AddPreCondition("ImprovedChances", false);

        AddSecondaryEffect("Object" + GetID().ToString(), true);

        AddEffect("ImprovedChances", true);

        if (TrapObject != null)
        {
            //TrapObject.SetActive(false);
        }
    }

    public void Awake()
    {

        SetTarget(this.gameObject.transform.position);
        SetRange(1.5f);
    }

    public override bool CheckPrecondition(GameObject a_AI_Agent)
    {

        SetAvailable(!m_bIsActive);

        if (m_bIsActive)
        {

            a_AI_Agent.GetComponent<AI_Agent>().GetWorldDataSource().EnactEffect(GetSecondaryEffects());

            return false;

        }
        else
        {

            Vector3 LeveledPos = new Vector3(this.transform.position.x, a_AI_Agent.transform.position.y, this.transform.position.z);

            float distance = Vector3.Distance(a_AI_Agent.transform.position, this.gameObject.transform.position);

            SetCost(distance);

            SetTarget(LeveledPos);

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

            AI_Guard_V2 Guard = (AI_Guard_V2)a_Agent;

            //get distance to the target point
            Vector3 LeveledPos = new Vector3(GetTarget().x, a_Agent.transform.position.y, GetTarget().z);

            fDistance = Vector3.Distance(LeveledPos, a_Agent.transform.position);

            //if the distance is less than 1.5f
            if (fDistance <= GetRange())
            {

                //set is in range true
                SetIsInRange(true);

                m_bIsActive = true;

                Interactable trapScript = TrapObject.GetComponent<Interactable>();

                trapScript.AIInteract(a_Agent);

                SetAvailable(false);

                m_bUsed = true;

                a_Agent.GetWorldDataSource().EnactEffect(GetEffects());
                a_Agent.GetWorldDataSource().EnactEffect(GetSecondaryEffects());

                Guard.SetObjectSelected(false);
               
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
