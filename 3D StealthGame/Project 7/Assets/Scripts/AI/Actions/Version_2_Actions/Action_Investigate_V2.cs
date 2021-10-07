//////////////////////////////////////////////////////////////////////////
///File name: Action_Investigate_V2.cs
///Date Created: 04/02/2021
///Created by: JG
///Brief: AI Action for Investigating a sensory input.
//////////////////////////////////////////////////////////////////////////
using System.Collections;
using UnityEngine;

public class Action_Investigate_V2 : AI_Action
{

    //enum for different investigation stages
    private enum Investigate_Stage
    {
        Stage_1,
        Stage_2,
        Stage_3
    }

    //Ai blackboard
    private AI_Blackboard m_BlackBoard;

    //current investigation stage
    private Investigate_Stage m_eCurrentStage = Investigate_Stage.Stage_1;

    //Bool for complete test
    private bool m_bHasInvestigated = false;

    //bool for inProgress
    private bool m_bInvestigating = false;

    //Target Location
    private Vector3 m_v3InvestigatePos;

    //investigation time
    public float m_fTimeToInvestigate = 2.5f;

    //Step time
    public float m_fStepTime;

    //Step counter
    public int m_iStageUpCount = 3;
    private int m_iStageCount = 0;

    //Rotation target
    Vector3 RotationTarget;

    public Action_Investigate_V2()
    {
        //Adds Preconditions to the action
        AddPreCondition("SearchNearbyHide", false);
        AddPreCondition("Investigating", true);

        //Adds effect of Action
        AddEffect("Investigating", false);

    }

    public override void Reset(AI_Agent a_Agent)
    {
        StopAllCoroutines();

        m_bHasInvestigated = false;

        m_bInvestigating = false;

        SetIsInRange(false);

    }

    //Checks for additional preconditions and sets action variables
    public override bool CheckPrecondition(GameObject a_Agent)
    {
        if (m_BlackBoard == null)
        {
            m_BlackBoard = FindObjectOfType<AI_Blackboard>();
        }

        m_v3InvestigatePos = a_Agent.GetComponent<AI_Agent>().GetStimulus();

        AI_Guard_V2 Guard = (AI_Guard_V2)a_Agent.GetComponent<AI_Agent>();

        if(Guard.GetDecoyed())
        {
            SetTarget(m_v3InvestigatePos);

            return true;
        }

        

        switch (m_eCurrentStage)
        {
            case Investigate_Stage.Stage_1:

                if (m_v3InvestigatePos == Vector3.zero)
                {

                    return false;
                }
                else
                {

                    SetTarget(this.transform.position);

                    return true;
                }

            case Investigate_Stage.Stage_2:

                if (m_v3InvestigatePos == Vector3.zero)
                {

                    return false;
                }
                else
                {
                    SetTarget(m_v3InvestigatePos);

                    return true;
                }

            case Investigate_Stage.Stage_3:

                if (m_v3InvestigatePos == Vector3.zero)
                {

                    return false;
                }
                else
                {
                    SetTarget(m_v3InvestigatePos);

                    return true;
                }

            default:

                if (GetTarget() == Vector3.zero)
                {

                    return false;
                }
                else
                {
                    SetTarget(m_v3InvestigatePos);

                    return true;
                }
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
        if (m_v3InvestigatePos != Vector3.zero)
        {

            AI_Guard_V2 GuardScript = (AI_Guard_V2)a_Agent;

            if (GuardScript.GetDecoyed())
            {
                if (!m_bInvestigating)
                {
                    StartCoroutine(Decoyed_Wait(m_fStepTime, a_Agent));
                }
                else
                {

                    //rotates the agent toward a target rotation
                    a_Agent.transform.forward = Vector3.RotateTowards(a_Agent.transform.forward, RotationTarget.normalized, 1.5f * Time.deltaTime, 1.0f);
                }

            }
            else
            {
                switch (m_eCurrentStage)
                {
                    case Investigate_Stage.Stage_1:


                        if (!m_bInvestigating)
                        {
                            StartCoroutine(Stage_1_Wait(m_fStepTime, a_Agent));
                        }
                        else
                        {
                            Vector3 LookPos = new Vector3(m_v3InvestigatePos.x, a_Agent.transform.position.y, m_v3InvestigatePos.z);

                            RotationTarget = LookPos - a_Agent.transform.position;

                            //rotates the agent toward a target rotation
                            a_Agent.transform.forward = Vector3.RotateTowards(a_Agent.transform.forward, RotationTarget.normalized, 1.5f * Time.deltaTime, 1.0f);

                        }

                        break;
                    case Investigate_Stage.Stage_2:

                        if (!m_bInvestigating)
                        {
                            StartCoroutine(Stage_2_Wait(m_fStepTime, a_Agent));
                        }
                        else
                        {

                            //rotates the agent toward a target rotation
                            a_Agent.transform.forward = Vector3.RotateTowards(a_Agent.transform.forward, RotationTarget.normalized, 1.5f * Time.deltaTime, 1.0f);

                        }

                        break;
                    case Investigate_Stage.Stage_3:

                        if (!m_bInvestigating)
                        {
                            StartCoroutine(Stage_2_Wait(m_fStepTime, a_Agent));
                        }
                        else
                        {
                            if (!a_Agent.GetPlayersighted())
                            {//rotates the agent toward a target rotation
                                a_Agent.transform.forward = Vector3.RotateTowards(a_Agent.transform.forward, RotationTarget.normalized, 1.5f * Time.deltaTime, 1.0f);
                            }
                        }

                        break;
                    default:

                        break;
                }
            }

            return true;
        }
        return false;
    }

    IEnumerator Stage_1_Wait(float a_fTimeToWait, AI_Agent a_Agent)
    {

        m_bInvestigating = true;

        AI_Guard_V2 Guard = (AI_Guard_V2)a_Agent;

        //Guard.StartSightTimer(m_fTimeToInvestigate);

        //Debug.Log("Investigating");

        float Timer = 0;

        do
        {
            if (Guard.GetSliderVal() <= 0)
            {

                //Debug.Log("Investigation ended");

                a_Agent.GetWorldDataSource().EnactEffect(GetEffects());

                m_bInvestigating = false;

                m_bHasInvestigated = true;

                m_iStageCount++;

                if (m_iStageCount >= m_iStageUpCount)
                {
                    //Debug.Log("Investigate stage up -> 2");

                    m_iStageCount = 0;

                    m_eCurrentStage = Investigate_Stage.Stage_2;

                }

                a_Agent.SetAwareness(AI_Agent.AWARENESS_STATE.UNAWARE);

                Guard.StopAwareFade();

                yield break;
            }
            else if (Timer >= a_fTimeToWait)
            {

                a_Agent.GetWorldDataSource().EnactEffect(GetEffects());

                m_bInvestigating = false;

                m_bHasInvestigated = true;

                m_iStageCount++;

                if (m_iStageCount >= m_iStageUpCount)
                {
                    //Debug.Log("Investigate stage up -> 2");

                    m_iStageCount = 0;

                    m_eCurrentStage = Investigate_Stage.Stage_2;

                }

                a_Agent.SetAwareness(AI_Agent.AWARENESS_STATE.UNAWARE);

                Guard.StopAwareFade();

                yield break;

            }

                Timer += Time.deltaTime;

            yield return new WaitForFixedUpdate();

        } while (!Guard.GetPlayerLocated());


        //add effect of player known location
        AddEffect("KnowsPlayer", true);

        a_Agent.GetWorldDataSource().EnactEffect(GetEffects());

        RemoveEffect("KnowsPlayer");

        m_bInvestigating = false;

        m_iStageCount++;

        if (m_iStageCount >= m_iStageUpCount)
        {
            //Debug.Log("Investigate stage up -> 2");

            m_iStageCount = 0;

            m_eCurrentStage = Investigate_Stage.Stage_2;
        }

        Guard.StopAwareFade();

        yield break;
    }

    IEnumerator Stage_2_Wait(float a_fTimeToWait, AI_Agent a_Agent)
    {

        m_bInvestigating = true;

        AI_Guard_V2 Guard = (AI_Guard_V2)a_Agent;

        //Debug.Log("Investigating - stage 2");

        Vector3 Forward = a_Agent.transform.forward;

        Vector3 Right = a_Agent.transform.right;
        Vector3 Left = -Right;

        RotationTarget = Right;

        int DirChangeCount = 3;
        int ChangeOfDirCount = 0;


        do
        {
            if(a_Agent.transform.forward == Right)
            {
                RotationTarget = Left;
                ChangeOfDirCount++;

            }
            else if(a_Agent.transform.forward == Left)
            {
                RotationTarget = Right;
                ChangeOfDirCount++;

            }

            if(ChangeOfDirCount >= DirChangeCount)
            {
                //Debug.Log("Investigation ended");

                a_Agent.GetWorldDataSource().EnactEffect(GetEffects());

                a_Agent.SetAwareness(AI_Agent.AWARENESS_STATE.UNAWARE);


                m_bInvestigating = false;


                m_bHasInvestigated = true;

                m_iStageCount++;

                if (m_iStageCount >= m_iStageUpCount)
                {
                   Debug.Log("Investigate stage up -> 3");

                    Guard.StartActiveSearch();

                    m_iStageCount = 0;

                    m_eCurrentStage = Investigate_Stage.Stage_2;

                }

                Guard.StopAwareFade();

                yield break;
            }


            yield return new WaitForFixedUpdate();

        } while (!Guard.GetPlayerLocated());

        //add effect of player known location
        AddEffect("KnowsPlayer", true);

        a_Agent.GetWorldDataSource().EnactEffect(GetEffects());

        RemoveEffect("KnowsPlayer");

        m_bInvestigating = false;

        m_iStageCount++;

        if (m_iStageCount >= m_iStageUpCount)
        {
            //Debug.Log("Investigate stage up -> 3");

            Guard.StartActiveSearch();

            m_iStageCount = 0;

            m_eCurrentStage = Investigate_Stage.Stage_3;
        }

        Guard.StopAwareFade();

        yield break;
    }

    IEnumerator Decoyed_Wait(float a_fTimeToWait, AI_Agent a_Agent)
    {
        m_bInvestigating = true;

        AI_Guard_V2 Guard = (AI_Guard_V2)a_Agent;

        Vector3 Right = a_Agent.transform.right;
        Vector3 Left = -Right;

        RotationTarget = Right;

        int DirChangeCount = 3;
        int ChangeOfDirCount = 0;

        do
        {
            if (a_Agent.transform.forward == Right)
            {
                RotationTarget = Left;
                ChangeOfDirCount++;

            }
            else if (a_Agent.transform.forward == Left)
            {
                RotationTarget = Right;
                ChangeOfDirCount++;

            }

            if (ChangeOfDirCount >= DirChangeCount)
            {
                //Debug.Log("Investigation ended");

                a_Agent.GetWorldDataSource().EnactEffect(GetEffects());

                a_Agent.SetAwareness(AI_Agent.AWARENESS_STATE.UNAWARE);


                m_bInvestigating = false;

                m_bHasInvestigated = true;

                Guard.StopAwareFade();

                Guard.SetDecoyed(false);

                yield break;
            }

            yield return new WaitForFixedUpdate();

        } while (!Guard.GetPlayerLocated());


        Guard.StopAwareFade();

        Guard.SetDecoyed(false);

        a_Agent.GetWorldDataSource().EnactEffect(GetEffects());

        m_bInvestigating = false;



        yield return null;
    }

}
