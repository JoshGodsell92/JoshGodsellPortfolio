using System.Collections;
using System.Collections.Generic;
using UnityEngine;

////////////////////////////////////////////////////////////
// File: Alarms.cs
// Author: Cameron Lillie
// Brief: Script for the interactable alarms
////////////////////////////////////////////////////////////

public class Alarms : Interactable
{
    [SerializeField] bool bAlarmsDisabled;


    private Vector3 AlarmedPosition;


    // Update is called once per frame
    void Update()
    {
        
    }

    public override void Interact()
    {
        //make sure player can interact
        Debug.Log("Alarms Interacted");
        if (!bAlarmsDisabled)
        {
            bAlarmsDisabled = true;
            bInteractable = false;
        }
    }


    //Function added for AI interaction - JG
    public override void AIInteract(AI_Agent a_agent)
    {
        if(a_agent.GetAIType() == AI_Agent.AGENT_TYPE.DRONE)
        {

            if (!bAlarmsDisabled)
            {
                GetBlackBoard().SetIsAlarmRaised(true);
                AlarmedPosition = a_agent.GetStimulus();

                GetBlackBoard().AlarmGuards(AlarmedPosition, this);

                if(bInteractable)
                {
                    //bInteractable = false;

                    StartCoroutine(AlarmTimer(5.0f));
                }
            }
            else
            {

            }
        }
    }

    public void SetAlarmStatus(bool disabled)
    {
        bAlarmsDisabled = disabled;
    }

    public bool GetAlarmDisabled()
    {
        return bAlarmsDisabled;
    }


    public IEnumerator AlarmTimer(float a_TimeToReuse)
    {

        yield return new WaitForSeconds(a_TimeToReuse);

        bInteractable = true;

        yield return null;

    }
}
