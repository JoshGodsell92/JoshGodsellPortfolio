using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

////////////////////////////////////////////////////////////
// File: Door.cs
// Author: Cameron Lillie
// Brief: Script for the interactable doors
////////////////////////////////////////////////////////////

public class Door : Interactable
{

    [SerializeField] bool bOpen;
    [SerializeField] bool bLocked;
    [SerializeField] bool bTriggerStay;

    NavMeshObstacle ObstacleComponent;

    // Start is called before the first frame update
    void Start()
    {
        try
        {
            if (ObstacleComponent == null)
            {
                ObstacleComponent = GetComponent<NavMeshObstacle>();

                if (bLocked)
                {
                    ObstacleComponent.enabled = true;
                    ObstacleComponent.carving = true;
                }
                else
                {
                    ObstacleComponent.enabled = false;
                    ObstacleComponent.carving = false;

                }
            }
        }
        catch (System.Exception)
        {

            throw;
        }





    }

    // Update is called once per frame
    void FixedUpdate()
    {
        if (bLocked)
        {
            ObstacleComponent.enabled = true;
            ObstacleComponent.carving = true;
        }
        else
        {
            ObstacleComponent.enabled = false;
            ObstacleComponent.carving = false;

        }
    }

    public override void Interact()
    {
        //open/close door (through animations)
        //Debug.Log("Interacted with door");

        if (!bOpen)
        {
            GetComponent<Animator>().SetTrigger("OpenDoor");
            bOpen = true;
        }
        else
        {
            GetComponent<Animator>().SetTrigger("CloseDoor");
            bOpen = false;

        }
    }

    private void OnTriggerEnter(Collider collider)
    {
        if (!bLocked)
        {
            if (collider.tag == "AIAgent")
            {
                //Open door
                //Debug.Log("OpenDoor");
                GetComponent<Animator>().SetTrigger("OpenDoor");

            }
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (!bLocked)
        {
            if (other.tag == "AIAgent")
            {
                if(!bTriggerStay)
                {

                    GetComponent<Animator>().SetTrigger("CloseDoor");
                }
            }
        }
    }


    public void SetLockedState(bool a_bool)
    {
        bLocked = a_bool;
    }

    public bool GetLockedState()
    {
        return bLocked;
    }

}
