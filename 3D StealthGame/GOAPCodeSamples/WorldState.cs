//////////////////////////////////////////////////////////////////////////
///File name: WorldState.cs
///Date Created: 08/10/2020
///Created by: JG
///Brief: World State Interface.
///Last Edited by: JG
///Last Edited on: 09/11/2020
//////////////////////////////////////////////////////////////////////////

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface WorldState
{

    //Starting State
    HashSet<KeyValuePair<string, bool>> GetWorldState();

    //Create a Goal State
    HashSet<KeyValuePair<string, bool>> CreateGoalState(string a_GoalState, bool a_GoalBool);

    //No Plan Found for goal
    void PlanFailed(HashSet<KeyValuePair<string, bool>> a_FailedGoal);

    //Plan was found
    void PlanMade(HashSet<KeyValuePair<string, bool>> a_GoalPlaned, Queue<AI_Action> a_ActionSequence);

    //all Actions have been finished
    void ActionsFinished();

    //an action has caused abort
    void PlanAborted(AI_Action a_ActionFailed);

    void SetCondition(string a_Condition, bool a_bool);

    void EnactEffect(HashSet<KeyValuePair<string, bool>> a_Effects);

}
