//////////////////////////////////////////////////////////////////////////
///File name: LightSwitch.cs
///Date Created: 02/03/2021
///Created by: JG
///Brief: AI Obj Action class for AI switching on a light.
//////////////////////////////////////////////////////////////////////////
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LightSwitch : AI_ObjAction
{

    //bool for checking when action complete
    private bool m_bUsed;

    //bool for if light switch is switched on
    private bool m_bIsActive;

    public GameObject LightObject;

    public LightSwitch()
    {

        //Adds Preconditions to the action
        AddPreCondition("KnowsPlayer", false);

        AddPreCondition("HasPatrolled", false);

        AddPreCondition("ImprovedChances", false);

        AddSecondaryEffect("Object" + GetID().ToString(), true);

        AddEffect("ImprovedChances", true);

        if (LightObject != null)
        {
            LightObject.SetActive(false);
        }
    }

    public void Awake()
    {

        SetTarget(this.gameObject.transform.position);

        m_bIsActive = LightObject.activeSelf;

        SetRange(1.5f);

    }

    public void LightReset()
    {
        m_bIsActive = LightObject.activeSelf;

        m_bUsed = m_bIsActive;
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

            a_AI_Agent.GetComponent<AI_Agent>().GetWorldDataSource().SetCondition("Object" + GetID().ToString(), false);

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

            //get distance to the target point

            Vector3 LeveledPos = new Vector3(GetTarget().x, a_Agent.transform.position.y, GetTarget().z);

            fDistance = Vector3.Distance(LeveledPos, a_Agent.transform.position);

            //if the distance is less than 1.5f
            if (fDistance <= GetRange())
            {

                //set is in range true
                SetIsInRange(true);

                m_bIsActive = true;

                if (LightObject != null)
                {
                    LightObject.SetActive(true);
                }

                m_bUsed = true;

                SetAvailable(false);

                AI_Guard_V2 Guard = (AI_Guard_V2)a_Agent;

                Guard.SetObjectSelected(false);

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

    public void ToggleActive()
    {
        m_bIsActive = !m_bIsActive;

        m_bUsed = m_bIsActive;

        LightObject.SetActive(!m_bIsActive);

        SetAvailable(!m_bIsActive);

    }

}
