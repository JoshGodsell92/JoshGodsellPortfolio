//////////////////////////////////////////////////////////////////////////
///File name: AI_Agents.cs
///Created by: JG
///Brief: AI base class for AI agents.
//////////////////////////////////////////////////////////////////////////

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public abstract class AI_Agent : MonoBehaviour
{
    //enum for state machine
    public enum AGENT_STATE
    {
        IDLE,
        MOVETO,
        ACTION,
        STUNNED
    }

    //Enum for the awareness state
    public enum AWARENESS_STATE
    {
        UNAWARE,
        ALERT,
        ENGAGED
    }

    //enum for agent type
    public enum AGENT_TYPE
    {
        DRONE,
        GUARD
    }

    //AI Blackboard for sharing data
    private AI_Blackboard m_AIBlackboard;

    //bool for if Improve chances objects present
    protected bool m_bHasImprovementObjects;

    //current state of statemachine
    private AGENT_STATE m_eCurrentState = AGENT_STATE.IDLE;

    //This agents personal awareness
    protected AWARENESS_STATE m_eAwarenessState = AWARENESS_STATE.UNAWARE;

    //AI Type
    protected AGENT_TYPE m_eType = AGENT_TYPE.DRONE;

    //Actions capable of
    protected HashSet<AI_Action> m_AvailableActions;
    //current plan of actions
    protected Queue<AI_Action> m_CurrentActions;

    //Ideal Goal
    protected HashSet<KeyValuePair<string, bool>> m_Goal;

    //Primary Goal
    protected HashSet<KeyValuePair<string, bool>> m_PrimaryGoal;

    //source of world data
    protected WorldState m_WorldDataSource;

    //NavMeshAgent
    protected NavMeshAgent m_NavMeshAgent;

    //Agents current position
    protected Vector3 m_v3AgentPosition;

    //bool for has arrived
    protected bool m_bHasArrived;

    //Vector for target position
    protected Vector3 m_v3TargetPos;

    //Stimulus object to investigate
    protected Vector3 m_v3Stimulus;

    //Object representing player
    protected GameObject m_PlayerObject;

    //Vision script for agent
    protected VisionCone m_VisionCone;

    //bool for if the player is currently sseen by this agent
    protected bool m_bPlayerSighted;

    //bool for if the agent is alert but not engaged with the player 
    [SerializeField]
    protected bool m_bAlert;

    //bool for if the AI agent has velocity
    public bool m_bAgentMoving;

    //bool for if AI is Stunned
    protected bool m_bStunned = false;

    //index for path point
    private int m_iPathIndex = 0;

    //stored path
    protected NavMeshPath m_Path;

    //AI Speed
    [SerializeField]
    protected float m_fSpeed;

    //AI Turn Speed
    [SerializeField]
    private float m_fAngularSpeed;

    //Current movement direction
    protected Vector3 m_v3CurrentMoveDir;

    [SerializeField] protected float fStunTime;
    float fStunTimer;

    [SerializeField]
    string CurrentAction;

    //base constructor
    public AI_Agent()
    {

    }

    // Use this for initialization
    public virtual void Start()
    {
        //set up the available actions and the current actions variables
        m_AvailableActions = new HashSet<AI_Action>();
        m_CurrentActions = new Queue<AI_Action>();

        //Find the agents Data source
        FindDataSource();
        AddActions();


        //null checks for blackboard and player object
        if (m_AIBlackboard == null)
        {
            m_AIBlackboard = GameObject.FindObjectOfType<AI_Blackboard>();
        }

        if (m_PlayerObject == null)
        {
            m_PlayerObject = GameObject.Find("PlayerV2");
        }

        //try and catch for Vision objecta nd navmesh
        try
        {
            m_NavMeshAgent = GetComponent<NavMeshAgent>();
            m_VisionCone = transform.Find("VisionStart").gameObject.GetComponent<VisionCone>();
        }
        catch (System.Exception)
        {

            throw new System.Exception("No NavMeshAgent component on gameobject");
        }

        //set the agent goals
        SetGoal();

        fStunTimer = 0.0f;
    }

    //function to try and find a SpyData script attached to this object 
    public virtual void FindDataSource()
    {
        try
        {
            m_WorldDataSource = GetComponent<WorldState>();
        }
        catch (System.Exception)
        {

            throw;
        }
    }

    //add actions function
    public void AddActions()
    {
        //for each GOAP_Action attached to the gameobject put it in the available actions hashset
        foreach (AI_Action action in transform.GetComponents<AI_Action>())
        {
            m_AvailableActions.Add(action);
        }

        //Find all object actions in scene and include them in the list of available actions set improvement action to true
        if (FindObjectsOfType<AI_ObjAction>().Length > 0)
        {
            m_bHasImprovementObjects = true;

            foreach (AI_ObjAction action in FindObjectsOfType<AI_ObjAction>())
            {
                m_AvailableActions.Add(action);
            }
        }
        else
        {
            m_bHasImprovementObjects = false;
        }
    }

    // Update is called once per frame
    public virtual void Update()
    {
        //main switch for agent state
        switch (GetState())
        {
            case AGENT_STATE.IDLE:

                Idle();

                break;
            case AGENT_STATE.MOVETO:

                MoveTo();

                break;
            case AGENT_STATE.ACTION:

                Action();

                break;
            case AGENT_STATE.STUNNED:
                Stunned();
                break;
            default:
                break;
        }
    }


    public virtual void Idle()
    {
        //get the current world state and set a goal to escape
        HashSet<KeyValuePair<string, bool>> t_WorldState = m_WorldDataSource.GetWorldState();

        //get a plan from the GOAP planner
        Queue<AI_Action> t_Plan = GetBlackboard().GetAIPlanner().ActionPlan(gameObject, m_AvailableActions, t_WorldState, m_Goal, m_PrimaryGoal);

        //if the plan isn't null
        if (t_Plan != null)
        {
            //the current actions queue is now the plan
            m_CurrentActions = t_Plan;

            //inform the world data that a plan has been made and proceed to the move to state and return
            m_WorldDataSource.PlanMade(m_Goal, m_CurrentActions);
            SetState(AGENT_STATE.MOVETO);
            return;
        }
        else
        {

            //otherwise there was no plan created so inform the world data and return
            m_WorldDataSource.PlanFailed(m_Goal);
            return;
        }
    }

    public virtual void MoveTo()
    {
        //take the first action from the queue
        AI_Action t_ActionMove = m_CurrentActions.Peek();

        CurrentAction = t_ActionMove.GetType().Name;

        //if the preconditions for the current action are not still the same
        if (!m_AIBlackboard.GetAIPlanner().InState(t_ActionMove.GetPreConditions(), m_WorldDataSource.GetWorldState()))
        {
            //clear the action queue and path
            m_CurrentActions = new Queue<AI_Action>();

            m_eCurrentState = AGENT_STATE.IDLE;
            return;
        }

        //if the agent has arrived at the destination
        if (GetHasArrived())
        {
            //set the actions in range bool to true
            t_ActionMove.SetIsInRange(true);
            //reset has arrived and move to the action state
            SetHasArrived(false);
            m_eCurrentState = AGENT_STATE.ACTION;
            return;
        }
        //if the action doesnt require the agent to be in range
        else if (!t_ActionMove.RequiresInRange())
        {
            //move to the action state
            m_eCurrentState = AGENT_STATE.ACTION;
            return;
        }
        else if (!GetHasArrived() && t_ActionMove.RequiresInRange())
        {
            //if the agent isnt in range and needs to be check the preconditions for the action are still true and set movement
            Vector3 v3TargetPos = Vector3.zero;

            if (t_ActionMove.CheckPrecondition(this.gameObject))
            {
                v3TargetPos = t_ActionMove.GetTarget();
                m_v3TargetPos = v3TargetPos;
            }
            else
            {
                //return to idle and inform the world state that the plan was aborted
                m_eCurrentState = AGENT_STATE.IDLE;
                m_WorldDataSource.PlanAborted(t_ActionMove);
                return;
            }

            Vector3 ThisLeveledPos = new Vector3(this.transform.position.x, v3TargetPos.y, this.transform.position.z);

            //if the target action is in range
            if (Vector3.Distance(v3TargetPos, ThisLeveledPos) <= t_ActionMove.GetRange())
            {
                m_bHasArrived = true;

                m_bAgentMoving = false;

                m_iPathIndex = 0;

                m_Path = null;

                m_NavMeshAgent.enabled = true;

            }
            else
            {
                //if there is no current path
                if (m_Path == null)
                {

                    RecalculatePath(v3TargetPos);

                }

                m_bAgentMoving = true;


                //traversePath
                TraversePath();

            }
        }
    }

    public virtual void Action()
    {
        //if the action queue count is less than or equal to 0
        if (m_CurrentActions.Count <= 0)
        {
            //return to idle and inform the world state that the plan was finished
            m_eCurrentState = AGENT_STATE.IDLE;

            m_WorldDataSource.ActionsFinished();
            return;
        }
        else
        {
            //otherwise take the first action from the queue
            AI_Action t_ActionPerform = m_CurrentActions.Peek();

            CurrentAction = t_ActionPerform.GetType().Name;

            //if the action is completed
            if (t_ActionPerform.IsComplete())
            {
                //remove the action from the queue
                m_CurrentActions.Dequeue();

                //if the queue count is above 0
                if (m_CurrentActions.Count > 0)
                {
                    //get the next action
                    t_ActionPerform = m_CurrentActions.Peek();
                }
                else
                {
                    //otherwise return to idle and tell the world state that the plan was finished
                    m_eCurrentState = AGENT_STATE.IDLE;
                    m_WorldDataSource.ActionsFinished();
                    return;
                }
            }

            //if the actions preconditions are no longer true reset the plan and path and return to idle
            if (!m_AIBlackboard.GetAIPlanner().InState(t_ActionPerform.GetPreConditions(), m_WorldDataSource.GetWorldState()))
            {
                m_CurrentActions = new Queue<AI_Action>();

                m_Path = null;
                m_iPathIndex = 0;

                t_ActionPerform.Reset(this);

                m_eCurrentState = AGENT_STATE.IDLE;
                return;
            }

            //if the queue count is above 0
            if (m_CurrentActions.Count > 0)
            {
                //is the action in range
                if (t_ActionPerform.GetIsInRange())
                {
                    //test to see if the action is performed
                    bool t_Success = t_ActionPerform.PerformAction(this);

                    //if the action  can't be performed
                    if (!t_Success)
                    {
                        //return to idle and inform the world state that the plan was aborted
                        m_eCurrentState = AGENT_STATE.IDLE;
                        m_WorldDataSource.PlanAborted(t_ActionPerform);
                        return;

                    }
                }
                else
                {
                    //other wise the action isnt in range so go to move to state
                    m_eCurrentState = AGENT_STATE.MOVETO;
                    return;
                }
            }
        }
    }

    //function to calculate path using navmesh
    public void RecalculatePath(Vector3 a_v3TargetPos)
    {
        //initialise new path
        m_Path = new NavMeshPath();

        //enable the navmesh agent
        m_NavMeshAgent.enabled = true;

        //use the built in pathing function to calculate path points
        m_NavMeshAgent.CalculatePath(a_v3TargetPos, m_Path);

        //if the path is invalid or incomplete
        if (m_Path.status == NavMeshPathStatus.PathInvalid)
        {
            //set up a navmesh hit for return data
            NavMeshHit hit;

            //get the AI walkable layer
            int AILayermask = 1 << NavMesh.GetAreaFromName("AI Walkable");

            //Sample a position within range on the navmesh layer if returns true then a position was found
            if (NavMesh.SamplePosition(a_v3TargetPos, out hit, 2f, AILayermask))
            {

                //calculate the new path
                m_NavMeshAgent.CalculatePath(hit.position, m_Path);

            }
        }

        //disable the navmesh agent - not needed allows Agent to stick to ground level when active.
        //m_NavMeshAgent.enabled = false;

        //return the path index to 0
        m_iPathIndex = 0;
    }

    public void TraversePath()
    {

        if (m_Path.status == NavMeshPathStatus.PathInvalid)
        {
            RecalculatePath(m_v3TargetPos);
        }

        Vector3[] pathpoints = m_Path.corners;

        float groundOffset = this.transform.position.y;

        if (m_iPathIndex > pathpoints.Length)
        {
            Debug.Log("Can't find pathpoint");
        }

        //Sphere case and ray cast against other AI to add avoidance

        Vector3 Avoidance = AvoidAI();

        if(m_iPathIndex > pathpoints.Length)
        {
            m_iPathIndex = 0;
        }

        Vector3 newPos = new Vector3(pathpoints[m_iPathIndex].x, groundOffset, pathpoints[m_iPathIndex].z);

        Vector3 MoveDir = newPos - this.transform.position;

        Vector3 MoveDirNorm = MoveDir.normalized;

        Vector3 Forward = this.transform.forward.normalized;

        float Diff = Vector3.Distance(Forward, MoveDirNorm);

        if (MoveDir == m_v3CurrentMoveDir && Diff <= 0.01f)
        {

            this.transform.position += ((MoveDir.normalized + Avoidance ) * m_fSpeed) * Time.deltaTime;

            if (Vector3.Distance(this.transform.position, newPos) <= 0.25f)
            {
                if (m_iPathIndex < pathpoints.Length - 1)
                {
                    m_iPathIndex++;
                }
                else
                {
                    m_bHasArrived = true;

                    m_iPathIndex = 0;

                    m_Path = null;
                }
            }
        }
        else
        {

            if (m_iPathIndex == 0 && MoveDir == Vector3.zero)
            {
                m_iPathIndex++;
            }

            float RadMove = m_fAngularSpeed * Mathf.Deg2Rad;

            Vector3 newRot = Vector3.RotateTowards(this.transform.forward, MoveDir, RadMove, 0.0f);

            m_v3CurrentMoveDir = MoveDir;

            this.transform.rotation = Quaternion.LookRotation(newRot);

        }


    }

    public Vector3 AvoidAI()
    {

        Vector3 AvoidanceDir = Vector3.zero;

        RaycastHit hitObj;
        int layermask = LayerMask.GetMask("AI");

        if (Physics.SphereCast(this.transform.position, 1.0f, this.transform.forward, out hitObj,0.5f, layermask))
        {

            AI_Agent agentAI = hitObj.transform.GetComponent<AI_Agent>();

            if(agentAI != null)
            {

                Vector3 ObjDir = this.transform.position - hitObj.transform.position;

                Vector3 Cross = Vector3.Cross(this.transform.forward, ObjDir);

                float Direction = Vector3.Dot(Cross, this.transform.up);

                if (Direction > 0f)
                {

                    AvoidanceDir = this.transform.right;

                }
                else if (Direction < 0f)
                {
                    AvoidanceDir = -this.transform.right;

                }
                else
                {
                    AvoidanceDir = new Vector3(0f, 0f, 0f);
                }
            }
        }
        else
        {
            AvoidanceDir = new Vector3(0f, 0f, 0f);
        }

        return AvoidanceDir;
    }

    public abstract void AwareProgression();

    public virtual void Stunned()
    {

        fStunTimer += Time.deltaTime;

        GetNavAgent().isStopped = true;

        if (fStunTimer >= fStunTime)
        {
            m_eCurrentState = AGENT_STATE.ACTION;

            GetNavAgent().isStopped = false;

            m_bStunned = false;

            fStunTimer = 0f;
        }
    }

    public abstract void SetGoal();

    public abstract void HeardSound(Vector3 a_v3Position, bool a_bIsDecoy);

    #region Get&Set

    //get and set agent has arrived
    public bool GetHasArrived()
    {
        return m_bHasArrived;
    }
    public void SetHasArrived(bool a_bool)
    {
        m_bHasArrived = a_bool;
    }

    //Get and set for is alert
    public bool GetAlert()
    {
        return m_bAlert;
    }
    public void SetAlert(bool a_bool)
    {
        m_bAlert = a_bool;
    }

    //get for the world data source
    public virtual WorldState GetWorldDataSource()
    {
        return m_WorldDataSource;
    }

    public void SetWorldData(AI_WorldState a_DataSource)
    {
        m_WorldDataSource = a_DataSource;
    }

    public AI_Blackboard GetBlackboard()
    {
        return m_AIBlackboard;
    }

    public AGENT_STATE GetState()
    {
        return m_eCurrentState;
    }
    public void SetState(AGENT_STATE a_eState)
    {
        m_eCurrentState = a_eState;
    }

    public Vector3 GetStimulus()
    {
        return m_v3Stimulus;
    }
    public void SetStimulus(Vector3 a_v3Position)
    {
        m_v3Stimulus = a_v3Position;
    }

    public GameObject GetPlayerObject()
    {
        return m_PlayerObject;
    }
    public bool GetPlayersighted()
    {
        return m_bPlayerSighted;
    }

    public NavMeshAgent GetNavAgent()
    {
        return m_NavMeshAgent;
    }

    public AGENT_TYPE GetAIType()
    {
        return m_eType;
    }

    public bool GetIsMoving()
    {
        return m_bAgentMoving;
    }

    public void SetAwareness(AWARENESS_STATE a_eAwareState)
    {
        m_eAwarenessState = a_eAwareState;
    }
    public AWARENESS_STATE GetAwareness()
    {
        return m_eAwarenessState;
    }


    public void SetSpeed(float a_fSpeed)
    {
        m_fSpeed = a_fSpeed;
    }
    public float GetSpeed()
    {
        return m_fSpeed;
    }

    public void SetAngularSpeed(float a_fAngularSpeed)
    {
        m_fAngularSpeed = a_fAngularSpeed;
    }
    public float GetAngularSpeed()
    {
        return m_fAngularSpeed;
    }
    #endregion //Get&Set

    #region Utility

    //function to print the plan to the console
    public static string PrintPlan(Queue<AI_Action> a_Plan)
    {
        string t_String = "";
        foreach (AI_Action action in a_Plan)
        {
            t_String += action.GetType().Name;
            t_String += " -> ";
        }
        t_String += "GOAL";
        return t_String;
    }

    IEnumerator LoopOnTime(float a_fTimePerLoop, System.Action a_FunctionToCall)
    {
        do
        {
            yield return new WaitForSeconds(a_fTimePerLoop);

            a_FunctionToCall();

        } while (true);

    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;

        Gizmos.DrawRay(this.transform.position, m_v3TargetPos);

        if (m_Path != null)
        {
            foreach (Vector3 point in m_Path.corners)
            {
                Gizmos.color = Color.magenta;
                Gizmos.DrawSphere(point, 0.5f);
            }
        }
    }

    #endregion //Utility

    

}