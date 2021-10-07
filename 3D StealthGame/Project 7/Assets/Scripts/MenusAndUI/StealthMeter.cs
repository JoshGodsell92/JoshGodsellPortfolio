using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StealthMeter : MonoBehaviour
{
    private const float UNAWAREROT = 75.0f;
    private const float ALERTROT = 0.0f;
    private const float ENGAGEDROT = -75.0f;


    private AI_Blackboard.AWARENESS_STATE m_currentState;

    [SerializeField]
    private GameObject m_goGamemanager;

    private AI_Blackboard blackboard;

    private void Start()
    {
        blackboard = m_goGamemanager.GetComponent<AI_Blackboard>();
    }

    // Update is called once per frame
    void Update()
    {
        m_currentState = blackboard.GetAwarenessEnum();

        switch(m_currentState)
        {
            case AI_Blackboard.AWARENESS_STATE.UNAWARE:
                this.transform.localEulerAngles = new Vector3(0, 0, UNAWAREROT);
                break;

            case AI_Blackboard.AWARENESS_STATE.ALERT:
                this.transform.localEulerAngles = new Vector3(0, 0, ALERTROT);
                break;

            case AI_Blackboard.AWARENESS_STATE.ENGAGED:
                this.transform.localEulerAngles = new Vector3(0, 0, ENGAGEDROT);
                break;

            default:
                break;

        }
    }
}
