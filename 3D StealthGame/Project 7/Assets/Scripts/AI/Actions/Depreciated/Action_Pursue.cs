//////////////////////////////////////////////////////////////////////////
///File name: Action_Pursue.cs
///Date Created: 19/10/2020
///Created by: JG
///Brief: AI Action for pursuit of an adversary.
///Last Edited by: JG
///Last Edited on: 09/11/2020
//////////////////////////////////////////////////////////////////////////

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Action_Pursue : AI_Action
{

    AI_Blackboard m_BlackBoard;


    //bool for if Target is sighted
    bool m_bTargetVisual;

    bool m_bHasPursued;

    //Game object to be detected
    private GameObject m_TargetObject;

    public Action_Pursue()
    {
        AddPreCondition("InPursuit", true);

        AddEffect("InPursuit", false);
    }

    public override void Reset(AI_Agent a_AI_Agent)
    {
        m_bTargetVisual = false;

        m_bHasPursued = false;

        m_TargetObject = null;

        SetIsInRange(true);


    }

    public override bool CheckPrecondition(GameObject a_Agent)
    {

        if(m_BlackBoard == null)
        {
            m_BlackBoard = FindObjectOfType<AI_Blackboard>();
        }

        if (m_TargetObject == null)
        {
            m_TargetObject = m_BlackBoard.GetPlayerObject();
        }

        //ensure target object is assigned
        if(m_TargetObject == null)
        {
            return false;
        }
        else
        {

            SetTarget(m_TargetObject.transform.position);
            SetIsInRange(true);

            return true;
        }

    }

    //if the action requires the Agent to be in range of an object to complete
    public override bool RequiresInRange()
    {
        return false;
    }

    public override bool RangedAction()
    {
        return false;
    }

    public override bool IsComplete()
    {
        return m_bHasPursued;
    }

    public override bool PerformAction(AI_Agent a_Agent)
    {

        m_bTargetVisual = a_Agent.GetPlayersighted();

        if (m_TargetObject != null)
        {
            if (m_bTargetVisual)
            {
                a_Agent.GetNavAgent().SetDestination(m_TargetObject.transform.position);

                if (a_Agent.GetNavAgent().remainingDistance <= 1.5f)
                {

                    a_Agent.GetNavAgent().SetDestination(a_Agent.transform.position);

                    a_Agent.GetWorldDataSource().EnactEffect(GetEffects());


                    m_bHasPursued = true;
                }
            }
            else
            {

                a_Agent.SetStimulus(a_Agent.GetNavAgent().pathEndPosition);

                if (a_Agent.GetNavAgent().remainingDistance <= 1.5f)
                {

                    a_Agent.GetWorldDataSource().SetCondition("Investigating", true);
                    a_Agent.GetWorldDataSource().SetCondition("KnowsPlayer", false);

                    a_Agent.GetWorldDataSource().EnactEffect(GetEffects());


                    m_bHasPursued = true;
                }
            }

            return true;

        }
        return false;

    }





}
