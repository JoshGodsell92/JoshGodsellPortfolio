//////////////////////////////////////////////////////////////////////////
///File name: AI_Drone.cs
///Date Created: 09/11/2020
///Created by: JG
///Brief: AI Drone Class
///Last Edited by: JG
///Last Edited on: 14/12/2020
//////////////////////////////////////////////////////////////////////////
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class AI_Drone : AI_Agent
{
    //Agents current path
    Vector3[] PathPositions;

    //empty constructor
    public AI_Drone()
    {

    }

    //Function to find the Data source for the Agents world view
    public override void FindDataSource()
    {
        //try and catch for intiialiseing variables
        try
        {
            //Ge the attached world data source
            m_WorldDataSource = GetComponent<AI_DroneState>();
        }
        catch (System.Exception)
        {

            throw;
        }
    }

    //function to set the drones goal state
    public override void SetGoal()
    {
        //create a new goal world state
         m_Goal = new HashSet<KeyValuePair<string, bool>>();

        //add a goal state to the world state
        m_Goal.Add(new KeyValuePair<string, bool>("TaskComplete", true));

    }

    //fixed update to be called at the start of each framerate frame
    public void FixedUpdate()
    {

    }

    public override void Update()
    {
        //if the player can been seen
        if (m_VisionCone.GetPlayerSeen())
        {
            //and not already sighted
            if (!m_bPlayerSighted)
            {
                //change the agents world view to reflect knows of the player
                GetWorldDataSource().SetCondition("KnowsPlayer", true);

                //if the alarm isn't raised reflect this in the world state
                if(!GetBlackboard().GetIsAlarmRaised())
                {
                    GetWorldDataSource().SetCondition("AlarmRaised", false);

                }

            }

            if (!m_bAlert)
            {
                //set this agent to alert
                SetAlert(true);

                StartCoroutine(AlertTime());
            }
            //set player sighted to true
            m_bPlayerSighted = true;

            //share data when it can see the player
            GetBlackboard().ShareData(this);

            //set its stimulus to be where the player was seen
            m_v3Stimulus = m_PlayerObject.transform.position;
        }
        else
        {
            //if the player was sighted but not currently
            if (m_bPlayerSighted)
            {

                //Debug.Log("Lost Player");

            }

            //set sighted player to false
            m_bPlayerSighted = false;

        }

        //call base class update
        base.Update();

    }

    //function for manipulating the current awareness as viewed/heard by the player
    public override void AwareProgression()
    {
        
    }

    //function for when the agent hearsd a sound
    public override void HeardSound(Vector3 a_v3Position,bool a_isDecoy)
    {
        //Log to console
        //Debug.Log("Drone Heard Sound");
    }

    //function to find the shortest path to an object from a list of objects
    public GameObject FindShortPath(List<GameObject> a_lGameObjects)
    {

        //set up a gameobject for the target
        GameObject TargetObject = null;

        //get a new navmesh path
        NavMeshPath t_path = new NavMeshPath();

        //temp variables ffor distance checks
        float Dist = 0.0f;
        float PrevDist = 0.0f;

        //for each object in the list
        foreach (GameObject target in a_lGameObjects)
        {

            //calculate a path to the object
            GetNavAgent().CalculatePath(target.transform.position, t_path);

            //if a complete path is not found
            if(t_path.status != NavMeshPathStatus.PathComplete)
            {
                //temp variable for navmesh position
                NavMeshHit hit;

                //sample a position near the target
                if(NavMesh.SamplePosition(target.transform.position,out hit,1.5f,NavMesh.AllAreas))
                {

                    //recalculate the path
                    GetNavAgent().CalculatePath(hit.position, t_path);

                }

            }
            

            //each point along the path
            Vector3[] pathPoints = t_path.corners;

            //for each point in the path
            for (int i = 0; i < pathPoints.Length - 1; i++)
            {

                //add the length between each point to get total distance
                Dist += Vector3.Distance(pathPoints[i], pathPoints[i + 1]);
            }


            //if this path is shorter than the prior path 
            if (Dist < PrevDist)
            {
                //store the new distance, target and path points
                PrevDist = Dist;
                TargetObject = target;
                PathPositions = pathPoints;

                //reset the temp distance
                Dist = 0.0f;
            }
            else if(PrevDist == 0.0f)
            {
                PrevDist = Dist;
                TargetObject = target;
                PathPositions = pathPoints;

                Dist = 0.0f;
            }

        }

        //return the object with the shortest path to it
        return TargetObject;
    }

    //function for finding a available task
    public GameObject FindAvailableTask()
    {
        //temp variable to hold picked task
        GameObject PickedTask = null;

        //list of task objects in scene
        DroneTask[] t_TaskObjs = FindObjectsOfType<DroneTask>();
        //list for available tasks in scene
        List<DroneTask> t_lAvailableTasks = new List<DroneTask>();

        //for each task in the scene
        foreach(DroneTask task in t_TaskObjs)
        {
            //if the task is not already occupied or on cooldown
            if(!task.GetIsOccupied() || !task.GetIsOnCooldown())
            {

                //add the task to available tasks
                t_lAvailableTasks.Add(task);

            }
        }


        NavMeshPath t_path = new NavMeshPath();

        float Dist = 0.0f;
        float PrevDist = 0.0f;

        foreach (DroneTask task in t_lAvailableTasks)
        {

            GetNavAgent().CalculatePath(task.GetPosition(), t_path);

            if (t_path.status != NavMeshPathStatus.PathComplete)
            {
                NavMeshHit hit;

                if (NavMesh.SamplePosition(task.GetPosition(), out hit, 1.5f, NavMesh.AllAreas))
                {

                    GetNavAgent().CalculatePath(hit.position, t_path);


                }

            }


            Vector3[] pathPoints = t_path.corners;


            for (int i = 0; i < pathPoints.Length - 1; i++)
            {

                Dist += Vector3.Distance(pathPoints[i], pathPoints[i + 1]);
            }


            if (Dist < PrevDist)
            {
                PrevDist = Dist;
                PickedTask = task.gameObject;
                PathPositions = pathPoints;

            }
            else if (PrevDist == 0.0f)
            {
                PrevDist = Dist;
                PickedTask = task.gameObject;
                PathPositions = pathPoints;

            }

            Dist = 0.0f;


        }

        return PickedTask;

    }

    public IEnumerator AlertTime()
    {

        yield return new WaitForSeconds(10.0f);

        m_bAlert = false;

        yield return null;

    }

    private void OnDrawGizmosSelected()
    {
        if(PathPositions != null)
        {
            foreach(Vector3 pos in PathPositions)
            {
                Gizmos.color = Color.magenta;
                Gizmos.DrawSphere(pos, 0.25f);
            }
        }
    }

}
