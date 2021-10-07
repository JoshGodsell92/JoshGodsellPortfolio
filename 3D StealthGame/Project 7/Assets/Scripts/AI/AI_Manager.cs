//////////////////////////////////////////////////////////////////////////
///File name: AI_Manager.cs
///Date Created: 08/10/2020
///Created by: JG
///Brief: Basic class to hold AI_planner for access (Prototype Only Will be in GameManager).
///Last Edited by: JG
///Last Edited on: 08/10/2020
//////////////////////////////////////////////////////////////////////////

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AI_Manager : MonoBehaviour
{
    private static AI_Planner s_AIPlanner;

    // Start is called before the first frame update
    void Start()
    {

        Random.InitState((int)System.Environment.TickCount);

        if(s_AIPlanner == null)
        {
            s_AIPlanner = new AI_Planner();
        }
        
    }

    public AI_Planner GetAIPlanner()
    {
        return s_AIPlanner;
    }

}
