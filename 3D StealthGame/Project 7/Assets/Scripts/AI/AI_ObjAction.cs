﻿//////////////////////////////////////////////////////////////////////////
///File name: AI_ObjAction.cs
///Date Created: 02/03/2021
///Created by: JG
///Brief: AI Obj Action base class for creating AI actions for objects.
//////////////////////////////////////////////////////////////////////////
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AI_ObjAction : AI_Action
{
    //actions ID
    [SerializeField]
    private int m_iObjectID;

    //if the action is already reserved by another agent
    private bool m_bReserved;

    //if the action is currently available
    private bool m_bAvailable;

    public virtual void AddIndexPreCondition(int a_iIndex)
    {

        AddPreCondition("Object" + GetID().ToString(), false);

    }

    public override bool CheckPrecondition(GameObject a_AI_Agent)
    {
        return false;
    }

    public override bool RangedAction()
    {
        return false;
    }

    public override bool RequiresInRange()
    {
        return false;
    }

    public override bool IsComplete()
    {
        return false;
    }

    public override bool PerformAction(AI_Agent a_AI_Agent)
    {
        return false;
    }

    //get and set for Object action ID
    public int GetID()
    {
        return m_iObjectID;
    }

    public void SetID(int a_iIDnum)
    {
        m_iObjectID = a_iIDnum;
    }

    //get and set for if the Action is reserved
    public bool GetIsReserved()
    {
        return m_bReserved;
    }
    public void SetIsReserved(bool a_bool)
    {
        m_bReserved = a_bool;
    }

    //get and set for if action is in available state
    public bool GetAvailable()
    {
        return m_bAvailable;
    }
    public void SetAvailable(bool a_bool)
    {
        m_bAvailable = a_bool;
    }

}
