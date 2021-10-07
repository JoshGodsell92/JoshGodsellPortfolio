//////////////////////////////////////////////////////////////////////////
///File name: AI_Planner.cs
///Date Created: 08/10/2020
///Created by: JG
///Brief: AI planner for creating an action plan for an AI AI_Agent to follow.
//////////////////////////////////////////////////////////////////////////

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AI_Planner
{

    public Queue<AI_Action> ActionPlan(GameObject a_AI_Agent, HashSet<AI_Action> a_ActionsAvailable, HashSet<KeyValuePair<string, bool>> a_WorldState, HashSet<KeyValuePair<string, bool>> a_Goal, HashSet<KeyValuePair<string,bool>> a_PrimaryGoal)
    {

        //Reset each action so they can be used
        foreach (AI_Action action in a_ActionsAvailable)
        {
            action.Reset(a_AI_Agent.GetComponent<AI_Agent>());
        }

        //temporary hash set to store currently available actions where preconditions are met
        HashSet<AI_Action> t_UsableActions = new HashSet<AI_Action>();
        //fill the current usable action set after checking each precondition
        foreach (AI_Action action in a_ActionsAvailable)
        {
            if (action.CheckPrecondition(a_AI_Agent))
            {
                t_UsableActions.Add(action);
            }
        }

        //Build a tree with leaf nodes for solution to goal
        List<GOAP_Node> t_Leaves = new List<GOAP_Node>();

        //build graph starting with this node to find ideal plan
        GOAP_Node t_StartNode = new GOAP_Node(null, 0, a_WorldState, null);
        bool t_bSuccess = BuildGraph(t_StartNode, t_Leaves, t_UsableActions, a_Goal);

        //if not successful find a plan for primnary goal state
        if (!t_bSuccess)
        {

            Debug.Log("<color=orange>Ideal Plan Failed</color> ");
            //build graph from start node for primary goal                                                               
            t_bSuccess = BuildGraph(t_StartNode, t_Leaves, t_UsableActions, a_PrimaryGoal);

            if(!t_bSuccess)
            {
                //return null and print to log
                Debug.Log("No Plan found");
                return null;
            }

        }

        //Find cheapest solution
        GOAP_Node t_CheapestLeaf = null;

        foreach (GOAP_Node leaf in t_Leaves)
        {
            if (t_CheapestLeaf == null)
            {
                t_CheapestLeaf = leaf;
            }
            else
            {
                if (leaf.GetCost() < t_CheapestLeaf.GetCost())
                {
                    t_CheapestLeaf = leaf;
                }
            }

        }

        //Get the node and work back through parents
        List<AI_Action> t_Result = new List<AI_Action>();
        GOAP_Node t_ResultNode = t_CheapestLeaf;


        //get actions into correct order
        while (t_ResultNode != null)
        {
            if (t_ResultNode.GetAction() != null)
            {
                //insert the action in the front end
                t_Result.Insert(0, t_ResultNode.GetAction());
            }

            t_ResultNode = t_ResultNode.GetParent();
        }


        //move the action list into a queue
        Queue<AI_Action> t_Queue = new Queue<AI_Action>();
        foreach (AI_Action action in t_Result)
        {
            t_Queue.Enqueue(action);
        }


        return t_Queue;

    }

    //returns true if a solution can be found stores a cost for each sequence
    private bool BuildGraph(GOAP_Node a_Parent, List<GOAP_Node> a_Leaves, HashSet<AI_Action> a_UsableActions, HashSet<KeyValuePair<string, bool>> a_Goal)
    {
        bool t_SolutionFound = false;

        //for each action check to see if it can be used at this stage
        foreach (AI_Action action in a_UsableActions)
        {
            //if the parent state conditions match the actions we can do the action
            if (InState(action.GetPreConditions(), a_Parent.GetState()))
            {
                //apply actions effects to parent state
                HashSet<KeyValuePair<string, bool>> t_CurrentState = GetResultState(a_Parent.GetState(), action.GetEffects());

                GOAP_Node t_Node = new GOAP_Node(a_Parent, a_Parent.GetCost() + action.GetCost(), t_CurrentState, action);

                if (InState(a_Goal, t_CurrentState))
                {
                    //solution was found
                    a_Leaves.Add(t_Node);
                    t_SolutionFound = true;
                }
                else
                {
                    //no solution yet, so test all remaining actions and branch out
                    HashSet<AI_Action> t_ActionSubset = ActionSubset(a_UsableActions, action);
                    bool t_NewSolutionFound = BuildGraph(t_Node, a_Leaves, t_ActionSubset, a_Goal);

                    if (t_NewSolutionFound)
                    {
                        t_SolutionFound = true;
                    }
                }
            }
        }

        return t_SolutionFound;
    }

    //creates a subset of the actions removing an action, creating a new set
    private HashSet<AI_Action> ActionSubset(HashSet<AI_Action> a_Actions, AI_Action a_RemoveAction)
    {

        HashSet<AI_Action> t_Subset = new HashSet<AI_Action>();

        //for each action in the actions set if it does not equal the remove action then add it to the subset
        foreach (AI_Action action in a_Actions)
        {
            if (!action.Equals(a_RemoveAction))
            {
                t_Subset.Add(action);
            }
        }

        return t_Subset;
    }

    //checks if a test state is true for the world state
    public bool InState(HashSet<KeyValuePair<string, bool>> a_Test, HashSet<KeyValuePair<string, bool>> a_State)
    {
        bool t_StateTrue = true;

        //for each condition in the test state
        foreach (KeyValuePair<string, bool> testPair in a_Test)
        {
            bool t_PairMatch = false;

            //for each world state condition
            foreach (KeyValuePair<string, bool> statePair in a_State)
            {
                //if there is a matching pair then set match to true
                if (statePair.Equals(testPair))
                {
                    t_PairMatch = true;
                    break;
                }
            }

            //if match is false for any condition then state match is false
            if (!t_PairMatch)
            {
                t_StateTrue = false;
            }
        }

        return t_StateTrue;
    }

    private HashSet<KeyValuePair<string, bool>> GetResultState(HashSet<KeyValuePair<string, bool>> a_CurrentState, HashSet<KeyValuePair<string, bool>> a_ChangeInState)
    {
        //new state to return
        HashSet<KeyValuePair<string, bool>> t_NewState = new HashSet<KeyValuePair<string, bool>>();

        //copy the keyValuePairs 
        foreach (KeyValuePair<string, bool> condition in a_CurrentState)
        {
            t_NewState.Add(new KeyValuePair<string, bool>(condition.Key, condition.Value));
        }

        //for each key in the change state
        foreach (KeyValuePair<string, bool> change in a_ChangeInState)
        {
            bool t_Exists = false;

            foreach (KeyValuePair<string, bool> Condition in t_NewState)
            {
                //if it exists 
                if (Condition.Key.Equals(change.Key))
                {
                    t_Exists = true;
                    break;
                }
            }

            //if it exists then change the value
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

        return t_NewState;
    }

    //function for checking if the state has changed since last fail 
    private bool StateChangedSinceFail(HashSet<KeyValuePair<string, bool>> a_WorldState, HashSet<KeyValuePair<string, bool>> a_FailedGoalWorldState)
    {
        bool t_StateChanged = false;

        foreach (KeyValuePair<string, bool> condition in a_WorldState)
        {
            bool t_Matches = false;

            foreach (KeyValuePair<string, bool> FailCondition in a_FailedGoalWorldState)
            {
                if (FailCondition.Equals(condition))
                {
                    t_Matches = true;
                }
            }
            if (!t_Matches)
            {
                t_StateChanged = true;
            }

        }

        return t_StateChanged;
    }

    //function to share states between agents
    public void ShareStates(AI_Agent a_FirstAgent,AI_Agent a_SecondAgent,KeyValuePair<string,bool> a_DataSet)
    {
        //each agents current data source
        HashSet<KeyValuePair<string, bool>> FirstDataSet = a_FirstAgent.GetWorldDataSource().GetWorldState();
        HashSet<KeyValuePair<string, bool>> SecondDataSet = a_SecondAgent.GetWorldDataSource().GetWorldState();

        //compare each corresponding data set and change one set to match the input data set
        foreach(KeyValuePair<string,bool> DataSet in FirstDataSet)
        {
            foreach (KeyValuePair<string, bool> DataSet2 in SecondDataSet)
            {
                if(DataSet.Key == DataSet2.Key)
                {
                    if (DataSet.Key == a_DataSet.Key)
                    {
                        if (DataSet.Value != a_DataSet.Value)
                        {
                            a_FirstAgent.GetWorldDataSource().SetCondition(DataSet.Key, a_DataSet.Value);

                        }
                        else if (DataSet2.Value != a_DataSet.Value)
                        {
                            a_SecondAgent.GetWorldDataSource().SetCondition(DataSet.Key, a_DataSet.Value);
                        }
                    }
                }
            }

        }
    }

}

#region GOAP_Node

//for use in graph and holding costs
public class GOAP_Node
{
    //Node parent
    private GOAP_Node m_Parent;
    //node run cost
    private float m_fCost;

    //state at this node in plan graph
    private HashSet<KeyValuePair<string, bool>> m_State;

    //the action for this node
    private AI_Action m_Action;

    //constructor
    public GOAP_Node(GOAP_Node a_Parent, float a_fCost, HashSet<KeyValuePair<string, bool>> a_State, AI_Action a_Action)
    {
        this.m_Parent = a_Parent;
        this.m_fCost = a_fCost;
        this.m_State = a_State;
        this.m_Action = a_Action;
    }

    #region GET&SET_GOAP_Node

    //get and set for parent
    public GOAP_Node GetParent()
    {
        return m_Parent;
    }
    public void SetParent(GOAP_Node a_Parent)
    {
        m_Parent = a_Parent;
    }

    //get and set for cost
    public float GetCost()
    {
        return m_fCost;
    }
    public void SetCost(float a_fCost)
    {
        m_fCost = a_fCost;
    }

    //get and set for state
    public HashSet<KeyValuePair<string, bool>> GetState()
    {
        return m_State;
    }
    public void SetState(HashSet<KeyValuePair<string, bool>> a_State)
    {
        m_State = a_State;
    }

    //get and set bfor action
    public AI_Action GetAction()
    {
        return m_Action;
    }
    public void SetAction(AI_Action a_Action)
    {
        m_Action = a_Action;
    }

    #endregion GET&SET_GOAP_Node
}

#endregion GOAP_Node
