//////////////////////////////////////////////////////////////////////////
///File name: Action_Flee.cs
///Date Created: 09/11/2020
///Created by: JG
///Brief: AI Flee action
///Last Edited by: JG
///Last Edited on: 10/11/2020
//////////////////////////////////////////////////////////////////////////
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class Action_Flee : AI_Action
{
    //AI blackboard
    private AI_Blackboard m_BlackBoard;

    //bools for completion and test states
    private bool m_bHasEscaped;
    private bool m_bPlayerVisual;
    private bool m_bHiding;

    //position for drone to flee to
    private Vector3 m_v3FleePosition;

    //value to osify distance if no ground object is secure
    [SerializeField]
    private float m_fFleeMod;

    //angle to test flee object against
    [SerializeField]
    private float m_fAngleFlee = 30.0f;

    //default constructor
    public Action_Flee()
    {
        //set preconditions
        AddPreCondition("KnowsPlayer", true);

        //set efffects
        AddEffect("KnowsPlayer", false);
    }

    //reset action
    public override void Reset(AI_Agent a_Agent)
    {
        //returnn  all bools to default states
        m_bHasEscaped = false;
        m_bPlayerVisual = false;
        m_bHiding = false;

        //set in range to true
        SetIsInRange(true);

        //StopAllCoroutines all coroutines
        StopAllCoroutines();

        //zero the flee position
        m_v3FleePosition = Vector3.zero;
    }

    //check pre-conditions
    public override bool CheckPrecondition(GameObject a_Agent)
    {

        //if black board is null then find it
        if (m_BlackBoard == null)
        {
            m_BlackBoard = FindObjectOfType<AI_Blackboard>();
        }

        //if the flee position is zero
        if (m_v3FleePosition == Vector3.zero)
        {

            m_v3FleePosition = a_Agent.transform.position - m_BlackBoard.GetPlayerObject().transform.position;
            SetTarget(m_v3FleePosition);
            return true;
        }
        else
        {
            SetTarget(m_v3FleePosition);
            return true;
        }

    }

    public override bool IsComplete()
    {
        return m_bHasEscaped;
    }

    public override bool RequiresInRange()
    {
        return false;
    }

    public override bool RangedAction()
    {
        return false;
    }

    public override bool PerformAction(AI_Agent a_Agent)
    {
        m_bPlayerVisual = a_Agent.GetPlayersighted();


        //if the flee position isnt zero
        if (m_v3FleePosition != Vector3.zero)
        {
            //if the player isnt visable
            if (!m_bPlayerVisual)
            {
                //if the remaining distance is less than 1.5
                if (a_Agent.GetNavAgent().remainingDistance <= 1.5f)
                {

                    //if not hiding
                    if (!m_bHiding)
                    {
                        //start the hide coroutine
                        StartCoroutine(Hide(a_Agent, 5.0f));

                        //set hiding to true
                        m_bHiding = true;
                    }
                }
            }
            //if player is visible
            else if (m_bPlayerVisual)
            {
                //set new flee position
                SetFleePos(a_Agent.gameObject);

                //set the nav destination
                a_Agent.GetNavAgent().SetDestination(m_v3FleePosition);

            }
            //if not rached destination
            else
            {
                //set destination
                a_Agent.GetNavAgent().SetDestination(m_v3FleePosition);

                
                if (a_Agent.GetNavAgent().remainingDistance <= 1.5f)
                {

                    SetFleePos(a_Agent.gameObject);

                }
            }
            


            return true;
        }
        return false;

    }

    //function to set a flee position that is away from the player by calculating angle between safe areas and the player
    public void SetFleePos(GameObject a_AIAgent)
    {

        //get all the gorund objects
        Ground[] t_aGroundScripts = FindObjectsOfType<Ground>();

        //two lists for different escape angles
        List<GameObject> t_lFirstGroundObjects = new List<GameObject>();
        List<GameObject> t_lSecondGroundObjects = new List<GameObject>();

        //for each ground object
        foreach (Ground script in t_aGroundScripts)
        {
            //if it is not the ground object the player is currently on
            if (script.gameObject != m_BlackBoard.GetPlayerGround())
            {

                //get the direction to the ground object
                Vector3 ObjectDir = script.gameObject.transform.position - a_AIAgent.transform.position;

                //get the direction of the player from this drone
                Vector3 PlayerDir = m_BlackBoard.GetPlayerObject().transform.position - a_AIAgent.transform.position;

                //get the angle between the ground direction and the player direction
                float fAngleBetween = Vector3.Angle(PlayerDir.normalized, ObjectDir.normalized);

                //if the angle is greater than half the flee angle 
                if (fAngleBetween >= m_fAngleFlee * 0.5f)
                {

                    //add the object to the first list
                    t_lFirstGroundObjects.Add(script.gameObject);


                    Debug.DrawRay(a_AIAgent.transform.position, PlayerDir, Color.red);
                    Debug.DrawRay(a_AIAgent.transform.position, ObjectDir, Color.blue);

                }
            }
        }

        //for each object in the first list
        foreach (GameObject target in t_lFirstGroundObjects)
        {

            //get the ground direction and player direction and the angle between
            Vector3 ObjectDir = target.gameObject.transform.position - a_AIAgent.transform.position;
            Vector3 PlayerDir = m_BlackBoard.GetPlayerObject().transform.position - a_AIAgent.transform.position;

            float fAngleBetween = Vector3.Angle(PlayerDir.normalized, ObjectDir.normalized);

            //if the angle is greater than the flee angle
            if (fAngleBetween >= m_fAngleFlee)
            {

                //add to the second list
                t_lSecondGroundObjects.Add(target.gameObject);


                Debug.DrawRay(a_AIAgent.transform.position, ObjectDir, Color.green);

            }
        }

        //temporary object to store picked object
        GameObject t_TargetGround;

        //if there are objects in the second list
        if (t_lSecondGroundObjects.Count > 0)
        { 
            //find the object with the shortest path from drone
             t_TargetGround = a_AIAgent.GetComponent<AI_Drone>().FindShortPath(t_lSecondGroundObjects);
        }
        else
        {
            //if there are no objects in the second list use the first list to find a shortest path
             t_TargetGround = a_AIAgent.GetComponent<AI_Drone>().FindShortPath(t_lFirstGroundObjects);

        }
        
        //if an object has been picked
        if (t_TargetGround != null)
        {
            //get a sampled position upon the object
            NavMeshHit hit;

            if (NavMesh.SamplePosition(t_TargetGround.transform.position, out hit, 2.5f, NavMesh.AllAreas))
            {
                //assignt the flee position
                m_v3FleePosition = hit.position;
            }
        }
        else
        {
            //if not object was found pick a position away from the player
            m_v3FleePosition = a_AIAgent.transform.position + ((a_AIAgent.transform.position - m_BlackBoard.GetPlayerObject().transform.position).normalized * m_fFleeMod);
        }
    
        //set the action target
        SetTarget(m_v3FleePosition);
    }

    IEnumerator Hide(AI_Agent a_Agent, float a_fTimeToHide)
    {
        //wait
        yield return new WaitForSeconds(a_fTimeToHide);

        //apply effects to world data source
        a_Agent.GetWorldDataSource().EnactEffect(GetEffects());

        //set the completion bool
        m_bHasEscaped = true;

        //set in range to true
        SetIsInRange(true);

        //end coroutine
        yield return null;
    }

}
