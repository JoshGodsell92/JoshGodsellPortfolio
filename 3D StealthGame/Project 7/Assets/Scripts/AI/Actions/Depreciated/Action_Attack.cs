//////////////////////////////////////////////////////////////////////////
///File name: Action_Investigate.cs
///Date Created: 02/11/2020
///Created by: JG
///Brief: AI Action for Attacking the player.
///Last Edited by: JG
///Last Edited on: 16/11/2020
//////////////////////////////////////////////////////////////////////////
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Action_Attack : AI_Action
{
    //AI Blackboard for global data
    AI_Blackboard m_AIBlackboard;

    //Player game object
    private GameObject m_PlayerObject;

    //bool for completed and end game check
    private bool m_bHasAttacked;
    private bool m_bPlayerDefeated = false;

    [SerializeField]
    private AudioSource Source;

    [SerializeField]
    private AudioClip AttackSound;

    //Default constructor                                           
    public Action_Attack()
    {
        //set action preconditions
        AddPreCondition("PlayerDefeated", false);
        AddPreCondition("InPursuit", false);
        AddPreCondition("KnowsPlayer", true);

        //set action effects
        AddEffect("PlayerDefeated", true);
    }

    //Action reset
    public override void Reset(AI_Agent a_AI_Agent)
    {
        //reset the bools
        m_bHasAttacked = false;
        m_bPlayerDefeated = false;

        //stop all current coroutines
        StopAllCoroutines();

        //set is in range to false
        SetIsInRange(false);


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

    //check the actions pre-conditions
    public override bool CheckPrecondition(GameObject a_Agent)
    {
        //if the blackboard has yet to be assigned find it
        if(m_AIBlackboard == null)
        {
            m_AIBlackboard = FindObjectOfType<AI_Blackboard>();
        }

        //if the player object is not set
        if(m_PlayerObject == null)
        {
            //get it from the blackboard
            m_PlayerObject = m_AIBlackboard.GetPlayerObject();
        }

        //if the player object still isnt set return false else set the target and return true
        if(m_PlayerObject == null)
        {

            return false;
        }
        else
        {
            SetTarget(m_PlayerObject.transform.position);

            return true;
        }

    }

    //return if action complete
    public override bool IsComplete()
    {
        return m_bPlayerDefeated;
    }

    //perform action function
    public override bool PerformAction(AI_Agent a_Agent)
    {

        //if the player object isnt null
        if (m_PlayerObject != null)
        {
            if (a_Agent.GetPlayersighted())
            {

                //temp distance
                float fDistance;

                //get distance to the target point
                fDistance = Vector3.Distance(m_PlayerObject.transform.position, a_Agent.transform.position);

                //if the distance is less than 1.5f
                if (fDistance <= 1.5f)
                {

                    //set is in range true
                    SetIsInRange(true);


                    //if not already attacking
                    if (!m_bHasAttacked)
                    {

                        //Attack and start timer till next attack
                        StartCoroutine(Attack(a_Agent, 2.5f));

                        //ensure only one instance of the coroutine is called by setting bool
                        m_bHasAttacked = true;

                    }
                    else
                    {

                    }
                }
                else
                {

                    //if the player is out of range or becomes out of range return to pursue
                    a_Agent.GetWorldDataSource().SetCondition("InPusuit", true);

                    //set is in range to false
                    SetIsInRange(false);
                }

            }
            else
            {
                //if the player is out of range or becomes out of range return to pursue
                a_Agent.GetWorldDataSource().SetCondition("Investigating", true);
                a_Agent.GetWorldDataSource().SetCondition("KnowsPlayer", false);

                //set is in range to false
                SetIsInRange(false);
            }

            return true;
        }
        return false;

    }


    IEnumerator Attack(AI_Agent a_Agent, float a_fTimeSpent)
    {

        Debug.Log("Attacked Player");

        //Do attack stuff Animation/Damage

        Source.clip = AttackSound;

        Source.Play();

        ((AI_Guard)a_Agent).AttackAnim();

        m_PlayerObject.GetComponent<PlayerController>().TakeDamage();

        yield return new WaitForSeconds(a_fTimeSpent);

        m_bHasAttacked = false;

        yield return null;

    }
}
