//////////////////////////////////////////////////////////////////////////
///File name: AI_Blackboard.cs
///Date Created: 29/10/2020
///Created by: JG
///Brief: Blackboard for AI World shared data and polls.
///Last Edited by: JG
///Last Edited on: 08/12/2020
//////////////////////////////////////////////////////////////////////////

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AI_Blackboard : MonoBehaviour
{
    //Enum for the awareness state
    public enum AWARENESS_STATE
    {
        UNAWARE,
        ALERT,
        ENGAGED
    }



    //stored Highest current awareness level of the Agents
    [SerializeField]
    private /*static*/  AWARENESS_STATE s_eAwareness = AWARENESS_STATE.UNAWARE;

    //AI planner
    private static AI_Planner s_AIPlanner;

    //List of all AI Agents
    private static AI_Agent[] s_AIAgents;

    //Player Object
    private GameObject m_PlayerObject;

    //Player Ground Area
    [SerializeField]
    private GameObject m_PlayerGround;

    //List for the alarm objects in the scene
    public GameObject[] m_AlarmObjects;

    //Global Bool states
    private bool m_bAlarmActive;
    private bool m_bPlayerDefeated;

    //Alarm Range
    [SerializeField]
    private float m_fAlarmRange;

    // Start is called before the first frame update
    void Start()
    {

        //Initialise the random with a seed
        Random.InitState((int)System.Environment.TickCount);

        //Try catch for assigning variables
        try
        {
            //find all the AI_Agents in the scene and assign them to the static list
            s_AIAgents = FindObjectsOfType<AI_Agent>();

            //if the AI planner is null create a new one
            if (s_AIPlanner == null)
            {
                s_AIPlanner = new AI_Planner();

            }

            //if the player objhect is unassigned find and assign it
            if (m_PlayerObject == null)
            {
                m_PlayerObject = GameObject.FindGameObjectWithTag("Player");
            }

            //gather all the alarm objects in the scene
            m_AlarmObjects = GameObject.FindGameObjectsWithTag("Alarm");

        }
        catch (System.Exception)
        {

            throw new System.Exception("AI_Blackboard failed to assign all variables");
        }


    }

    //Fixedupdate to be called at the beginning of each fixed framerate frame
    public void FixedUpdate()
    {

        //function to find and set the player ground
        FindPlayerGround();

        //Function to get the current highest awareness level
        AwarenessLevel();

    }

    //function to gage the current awareness level for the Agents
    public void AwarenessLevel()
    {
        AWARENESS_STATE t_eAwareness = AWARENESS_STATE.UNAWARE;

        //For each agent in the agent list
        foreach (AI_Agent agent in s_AIAgents)
        {
            //if any of the agents has direct sight of the player
            if (agent.GetAwareness() == AI_Agent.AWARENESS_STATE.ENGAGED && t_eAwareness != AWARENESS_STATE.ENGAGED)
            {
                if (t_eAwareness < AWARENESS_STATE.ENGAGED)
                {
                    t_eAwareness = AWARENESS_STATE.ENGAGED;
                }
            }
            else if (agent.GetAwareness() == AI_Agent.AWARENESS_STATE.ALERT && t_eAwareness != AWARENESS_STATE.ALERT)
            {
                if (t_eAwareness < AWARENESS_STATE.ALERT)
                {
                    t_eAwareness = AWARENESS_STATE.ALERT;
                }
            }
            else
            {
                if (t_eAwareness == AWARENESS_STATE.UNAWARE)
                {
                    t_eAwareness = AWARENESS_STATE.UNAWARE;
                }
            }
        }

        s_eAwareness = t_eAwareness;

    }

    //funjction to share data between AI_Agents within sight of each other
    public void ShareData(AI_Agent a_Agent)
    {

        //for each AI_Agent in the list
        foreach (AI_Agent agent2 in s_AIAgents)
        {
            //if it is not the one assigned through the argument 
            if (a_Agent != agent2)
            {
                //create a line of sight ray (LOS) between the two agents
                Ray LOS = new Ray(a_Agent.transform.position, (agent2.transform.position - a_Agent.transform.position));
                //get the distance between the two agents
                float RayDist = Vector3.Distance(a_Agent.transform.position, agent2.transform.position);
                //Set an variable to hold hit data from raycast
                RaycastHit hitObj;
                //Set a layer mask for the raycast
                int layerMask = 1 << 9;
                layerMask = 1 << 9 | 1 << 11;

                //if the raycast collides with a collider
                if (Physics.Raycast(LOS, out hitObj, RayDist, layerMask))
                {
                    //if the collider belongs to the second agent
                    if (hitObj.collider.gameObject == agent2.gameObject)
                    {
                        //if they are of the same AI type
                        if (a_Agent.GetAIType() == agent2.GetAIType())
                        {
                            //Check if the Two AI_Agents are not in the same state
                            if (!s_AIPlanner.InState(a_Agent.GetWorldDataSource().GetWorldState(), agent2.GetWorldDataSource().GetWorldState()))
                            {
                                //set the test pairs for "Investigating","KnowsPlayer" and "PlayerDefeated" to check what states to change to change
                                KeyValuePair<string, bool> testdata = new KeyValuePair<string, bool>("Investigating", true);
                                KeyValuePair<string, bool> testdata2 = new KeyValuePair<string, bool>("KnowsPlayer", true);
                                KeyValuePair<string, bool> testdata3 = new KeyValuePair<string, bool>("PlayerDefeated", false);

                                //Use the share states function from the planner to check and change the data for the second agent to match the first
                                s_AIPlanner.ShareStates(a_Agent, agent2, testdata);
                                s_AIPlanner.ShareStates(a_Agent, agent2, testdata2);
                                s_AIPlanner.ShareStates(a_Agent, agent2, testdata3);

                                //if the two agents do not have the same stimulus assign the stimulus to the second agent from the first
                                if (agent2.GetStimulus() != a_Agent.GetStimulus())
                                {
                                    agent2.SetStimulus(m_PlayerObject.transform.position);
                                }

                                //Log to console
                                //Debug.Log("Data shared between " + a_Agent.gameObject.name + " and " + agent2.gameObject.name);
                            }
                        }
                        //if the first Agent is a drone type
                        else if (a_Agent.GetAIType() == AI_Agent.AGENT_TYPE.DRONE)
                        {
                            //assign the same stimulus
                            if (agent2.GetStimulus() != a_Agent.GetStimulus())
                            {
                                agent2.SetStimulus(m_PlayerObject.transform.position);
                            }

                            //set the appropriate conditions for the second agent
                            agent2.GetWorldDataSource().SetCondition("Investigating", true);
                            agent2.GetWorldDataSource().SetCondition("PlayerDefeated", false);

                            //Log to console
                            //Debug.Log("Drone warned " + agent2.gameObject.name);

                        }
                    }
                }

            }
        }
    }

    //function to find and assign the player's current ground object
    public void FindPlayerGround()
    {
        //Set a ray for the player down
        Ray PlayerDownRay = new Ray(m_PlayerObject.transform.position, Vector3.down);

        //set a variable to hold hit data
        RaycastHit HitObj;
        //assign the ground layermask
        LayerMask GroundMask = LayerMask.GetMask("GroundLayer");

        //Debug.DrawRay(m_PlayerObject.transform.position, Vector3.down * 5.0f, Color.magenta);

        //if a collider is hit by the raycast on this layer
        if (Physics.Raycast(PlayerDownRay, out HitObj, 5.0f, GroundMask,QueryTriggerInteraction.Collide))
        {
            //if the hit object has a ground script attached                                                                   `
            if (HitObj.collider.gameObject.GetComponent<Ground>() != null)
            {
                //assign this object as the player ground 
                m_PlayerGround = HitObj.collider.gameObject;
            }
        }


    }

    //Function to alert guards within range of an alarm that has been triggered
    public void AlarmGuards(Vector3 a_PlayerPos, Alarms a_AlarmRaised)
    {
        //for each agent in the static list                                                                                
        foreach (AI_Agent agent in s_AIAgents)
        {
            //if the agent is a guard
            if (agent.GetAIType() == AI_Agent.AGENT_TYPE.GUARD)
            {
                //get the distance between the alarm and the guard
                float DistanceToAlarm = Vector3.Distance(agent.gameObject.transform.position, a_AlarmRaised.gameObject.transform.position);

                //if the distance is less than the alarm range 
                if (DistanceToAlarm <= m_fAlarmRange)
                {
                    //set the agents stimulus position to the player position parsed in
                    agent.SetStimulus(a_PlayerPos);

                    //call the agents alarm heard function
                    //((AI_Guard_V2)agent).AlarmHeard();
                }
            }
        }
    }

    //function to return the static planner
    public AI_Planner GetAIPlanner()
    {
        return s_AIPlanner;
    }

    //function to return the player object
    public GameObject GetPlayerObject()
    {
        return m_PlayerObject;
    }

    //function to return the player ground object
    public GameObject GetPlayerGround()
    {
        return m_PlayerGround;
    }

    //function to return an array of alarm objects
    public GameObject[] GetAlarms()
    {
        return m_AlarmObjects;
    }

    //function to return if an alarm is active
    public bool GetIsAlarmRaised()
    {
        return m_bAlarmActive;
    }
    //function to set if an alarm is active
    public void SetIsAlarmRaised(bool a_bool)
    {
        m_bAlarmActive = a_bool;
    }

    public AWARENESS_STATE GetAwarenessEnum()
    {
        return s_eAwareness;
    }
    public int GetAwarenessInt()
    {
        return (int)s_eAwareness;
    }


}
