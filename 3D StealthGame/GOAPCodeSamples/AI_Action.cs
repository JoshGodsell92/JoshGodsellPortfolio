//////////////////////////////////////////////////////////////////////////
///File name: AI_Action.cs
///Created by: JG
///Brief: AI Action base class for creating AI actions from.
//////////////////////////////////////////////////////////////////////////
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class AI_Action : MonoBehaviour
{
    //PreConditions
    private HashSet<KeyValuePair<string, bool>> m_PreConditions;

    //Effects which the planner assumes is the outcome of the action
    private HashSet<KeyValuePair<string, bool>> m_Effects;

    //Secondary effects which the planner doesnt know/see when planning used for if one action alone doesnt achive the effect
    private HashSet<KeyValuePair<string, bool>> m_SecondaryEffects;


    //action cost 
    private float m_fCost;

    //Is in range of Action
    private bool m_bIsInRange = false;

    //Can action be performed at Range
    private bool m_bRangedAction = false;

    //range at which action can be performed
    private float m_fActionRange = 0.5f;

    //Position for action
    private Vector3 m_ActionPosition;

    //base constructor
    public AI_Action()
    {
        m_PreConditions = new HashSet<KeyValuePair<string, bool>>();
        m_Effects = new HashSet<KeyValuePair<string, bool>>();
        m_SecondaryEffects = new HashSet<KeyValuePair<string, bool>>();
    }

    //base reset function
    public virtual void Reset(AI_Agent a_AI_Agent)
    {
        m_bIsInRange = false;
        m_ActionPosition = Vector3.zero;
    }

    //checks if action can be run
    public abstract bool CheckPrecondition(GameObject a_AI_Agent);

    //is the action completed
    public abstract bool IsComplete();

    //Run the action
    public abstract bool PerformAction(AI_Agent a_AI_Agent);

    //does the AI_Agent need to be in range of the action object
    public abstract bool RequiresInRange();

    //does the action have a range from which it can be performed
    public abstract bool RangedAction();

    //Get and set for the actions maximum range
    public float GetRange()
    {
        return m_fActionRange;
    }
    public void SetRange(float a_fRange)
    {
        m_fActionRange = a_fRange;
    }

    //Get and set for is in range of action object
    public bool GetIsInRange()
    {
        return m_bIsInRange;
    }
    public void SetIsInRange(bool a_bool)
    {
        m_bIsInRange = a_bool;
    }

    //get and set for cost
    public void SetCost(float a_fCost)
    {
        m_fCost = a_fCost;
    }
    public float GetCost()
    {
        return m_fCost;
    }

    //get and set for target vector
    public Vector3 GetTarget()
    {
        return m_ActionPosition;
    }
    public void SetTarget(Vector3 a_v3Position)
    {
        m_ActionPosition = a_v3Position;
    }

    //add and remove for preconditions
    public void AddPreCondition(string a_sPreCondition, bool a_bBoolState)
    {
        m_PreConditions.Add(new KeyValuePair<string, bool>(a_sPreCondition, a_bBoolState));
    }
    public void RemovePrecondition(string a_sPrecondition)
    {
        KeyValuePair<string, bool> remove = default(KeyValuePair<string, bool>);
        //for each precondition if this precondition exists remove it
        foreach (KeyValuePair<string, bool> PreCondition in m_PreConditions)
        {
            if (PreCondition.Key.Equals(a_sPrecondition))
            {
                remove = PreCondition;

            }
        }
        if (!default(KeyValuePair<string, object>).Equals(remove))
        {
            m_PreConditions.Remove(remove);
        }
    }

    //add and remove for Effects
    public void AddEffect(string a_sEffect, bool a_bBoolState)
    {
        m_Effects.Add(new KeyValuePair<string, bool>(a_sEffect, a_bBoolState));
    }
    public void RemoveEffect(string a_sEffect)
    {
        KeyValuePair<string, bool> remove = default(KeyValuePair<string, bool>);
        foreach (KeyValuePair<string, bool> effect in m_Effects)
        {
            if (effect.Key.Equals(a_sEffect))
            {
                remove = effect;

            }
        }
        if (!default(KeyValuePair<string, object>).Equals(remove))
        {
            m_Effects.Remove(remove);
        }
    }

    //add and remove for secondary Effects
    public void AddSecondaryEffect(string a_sEffect, bool a_bBoolState)
    {
        m_SecondaryEffects.Add(new KeyValuePair<string, bool>(a_sEffect, a_bBoolState));
    }

    public void RemoveSecondaryEffect(string a_sEffect)
    {
        KeyValuePair<string, bool> remove = default(KeyValuePair<string, bool>);
        foreach (KeyValuePair<string, bool> effect in m_Effects)
        {
            if (effect.Key.Equals(a_sEffect))
            {
                remove = effect;

            }
        }
        if (!default(KeyValuePair<string, object>).Equals(remove))
        {
            m_SecondaryEffects.Remove(remove);
        }
    }

    //getters for preconditions and effects
    public HashSet<KeyValuePair<string, bool>> GetPreConditions()
    {
        return m_PreConditions;
    }
    public HashSet<KeyValuePair<string, bool>> GetEffects()
    {
        return m_Effects;
    }

    public HashSet<KeyValuePair<string, bool>> GetSecondaryEffects()
    {
        return m_SecondaryEffects;
    }
}
