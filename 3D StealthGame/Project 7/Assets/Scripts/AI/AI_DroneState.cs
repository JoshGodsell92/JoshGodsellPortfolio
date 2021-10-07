//////////////////////////////////////////////////////////////////////////
///File name: AI_DroneState.cs
///Date Created: 09/11/2020
///Created by: JG
///Brief: World state for drone AI.
///Last Edited by: JG
///Last Edited on: 09/11/2020
//////////////////////////////////////////////////////////////////////////

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AI_DroneState : MonoBehaviour, WorldState
{

    //Stored states for returning
    private HashSet<KeyValuePair<string, bool>> m_WorldData;

    //Bools for AI states
    private bool m_bIsIdle = false;
    private bool m_bKnowsPlayer = false;
    private bool m_bPlayerDefeated = true;
    private bool m_bAlarmRaised = true;
    private bool m_bTaskCompleted = false;

    // Use this for initialization
    void Start()
    {

        Initialise();

    }

    public void Initialise()
    {
        //create a new world data
        m_WorldData = new HashSet<KeyValuePair<string, bool>>();

        //Add a possible data point into world state
        m_WorldData.Add(new KeyValuePair<string, bool>("IsIdle", m_bIsIdle));
        m_WorldData.Add(new KeyValuePair<string, bool>("PlayerDefeated", m_bPlayerDefeated));
        m_WorldData.Add(new KeyValuePair<string, bool>("AlarmRaised", m_bAlarmRaised));
        m_WorldData.Add(new KeyValuePair<string, bool>("KnowsPlayer", m_bKnowsPlayer));
        m_WorldData.Add(new KeyValuePair<string, bool>("TaskComplete", m_bTaskCompleted));
    }

    //Get and set for the world state
    public HashSet<KeyValuePair<string, bool>> GetWorldState()
    {
        return m_WorldData;
    }
    public void SetWorldState(HashSet<KeyValuePair<string, bool>> t_WorldData)
    {
        m_WorldData = t_WorldData;
    }

    //Create a new Goal state
    public HashSet<KeyValuePair<string, bool>> CreateGoalState(string a_GoalState, bool a_GoalBool)
    {
        HashSet<KeyValuePair<string, bool>> t_Goal = new HashSet<KeyValuePair<string, bool>>();

        t_Goal.Add(new KeyValuePair<string, bool>(a_GoalState, a_GoalBool));

        return t_Goal;
    }

    public HashSet<KeyValuePair<string, bool>> CreateGoalState(HashSet<KeyValuePair<string, bool>> a_GoalSet)
    {
        HashSet<KeyValuePair<string, bool>> t_Goal = a_GoalSet;

        return t_Goal;
    }


    //Plan failed
    public void PlanFailed(HashSet<KeyValuePair<string, bool>> a_FailedGoalState)
    {
        Debug.Log(this.gameObject.name + " <color=orange>Action failed</color>");

    }

    //Plan was made
    public void PlanMade(HashSet<KeyValuePair<string, bool>> a_Goal, Queue<AI_Action> a_PlanedActions)
    {
        //Debug.Log(this.gameObject.name + " <color=green>Plan found</color> " + AI_Agent.PrintPlan(a_PlanedActions));
    }

    //all Actions have been finished
    public void ActionsFinished()
    {
        Debug.Log(this.gameObject.name + " <color=blue>Actions finished</color>");
    }

    //an action has caused abort
    public void PlanAborted(AI_Action a_ActionFailed)
    {
        Debug.Log(this.gameObject.name + " <color=red>Plan Aborted</color> " + a_ActionFailed.ToString());
    }


    //function to change the world state for when an action has been completed
    public void EnactEffect(HashSet<KeyValuePair<string, bool>> a_Effects)
    {
        //new state to return
        HashSet<KeyValuePair<string, bool>> t_NewState = new HashSet<KeyValuePair<string, bool>>();

        //copy the keyValuePairs 
        foreach (KeyValuePair<string, bool> condition in GetWorldState())
        {
            t_NewState.Add(new KeyValuePair<string, bool>(condition.Key, condition.Value));
        }

        //if the key exists in the current state update the value
        foreach (KeyValuePair<string, bool> change in a_Effects)
        {
            bool t_Exists = false;

            foreach (KeyValuePair<string, bool> Condition in t_NewState)
            {
                if (Condition.Key.Equals(change.Key))
                {
                    t_Exists = true;
                    break;
                }
            }

            //remove the previous state and create a new state with the new values
            if (t_Exists)
            {
                t_NewState.RemoveWhere((KeyValuePair<string, bool> pair) => { return pair.Key.Equals(change.Key); });
                KeyValuePair<string, bool> t_UpdatedPair = new KeyValuePair<string, bool>(change.Key, change.Value);
                t_NewState.Add(t_UpdatedPair);
            }
            else
            {
                //if it doesn't exist add it
                t_NewState.Add(new KeyValuePair<string, bool>(change.Key, change.Value));
            }
        }

        //set the new state to the world state 
        SetWorldState(t_NewState);

    }

    //function to set change a specific condition
    public void SetCondition(string a_Condition, bool a_bool)
    {
        //new state to return
        HashSet<KeyValuePair<string, bool>> t_NewState = new HashSet<KeyValuePair<string, bool>>();

        //copy the keyValuePairs 
        foreach (KeyValuePair<string, bool> condition in GetWorldState())
        {
            t_NewState.Add(new KeyValuePair<string, bool>(condition.Key, condition.Value));
        }

        //if the condition exists change its bool value
        foreach (KeyValuePair<string, bool> condition in GetWorldState())
        {
            if (condition.Key == a_Condition)
            {
                t_NewState.RemoveWhere((KeyValuePair<string, bool> pair) => { return pair.Key.Equals(condition.Key); });
                KeyValuePair<string, bool> t_UpdatedPair = new KeyValuePair<string, bool>(condition.Key, a_bool);
                t_NewState.Add(t_UpdatedPair);
            }

        }

        //set the new state to the world state
        SetWorldState(t_NewState);
    }

}
