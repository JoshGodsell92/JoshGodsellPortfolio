using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

////////////////////////////////////////////////////////////
// File: HidingPlace.cs
// Author: Cameron Lillie
// Brief: Script for the interactable hiding places
////////////////////////////////////////////////////////////

public class HidingPlace : Interactable
{
    [SerializeField] GameObject goHidingPosition; //position to put the player when they hide
    [SerializeField] GameObject goLeavePosition; //position to put the player when they get out
    [SerializeField] GameObject goPlayer; //player
    [SerializeField] GameObject goBoxHide; // Hiding Box
    private PlayerController playerController;
    [SerializeField] float fHorizontalCameraClamp;
    [SerializeField] float fVerticalCameraClamp;
    [SerializeField] float fHorizontalLeanClamp;
    [SerializeField] float fVerticalLeanClamp;
    public bool bInUse;
    public bool bForceCrouch;
    public bool bIsCloset; //can the player crouch?
    [Tooltip("Standing position of the player in the closet, not used in boxes")]
    [SerializeField] Vector3 v3StandingPosition = new Vector3(0, 0, -0.45f);
    [Tooltip("Crouching position of the player in the closet, not used in boxes")]
    [SerializeField] Vector3 v3CrouchingPosition = new Vector3(0, 0, -1.45f);

    [SerializeField]
    private GameObject goSearchPoint;

    [SerializeField]
    public GameObject goInteractCanvas;

    // Start is called before the first frame update
    void Start()
    {
        goPlayer = GameObject.FindGameObjectWithTag("Player");
        playerController = goPlayer.GetComponent<PlayerController>();
    }

    // Update is called once per frame
    void Update()
    {

    }

    public override void AIInteract(AI_Agent a_Agent)
    {
        if (bInUse)
        {

            if (CheckExit(goLeavePosition.transform.position))
            {
                //Enable world object
                if (gameObject.GetComponent<MeshRenderer>() != null && gameObject.GetComponent<BoxCollider>())
                {

                    goBoxHide.SetActive(false);

                    gameObject.GetComponent<MeshRenderer>().enabled = true;
                    gameObject.GetComponent<BoxCollider>().isTrigger = false;
                    goPlayer.GetComponent<Rigidbody>().isKinematic = false;

                    //gameObject.GetComponent<NavMeshObstacle>().enabled = true;

                }
                else
                {
                    Debug.LogError("Hiding Place doesn't have a mesh collider or box collider yet somehow passed a raycast check, breaking most laws of physics in the process.");
                }
                //deactivate box object on player
                if (playerController.GetBoxView() != null)
                {
                    playerController.GetBoxView().SetActive(false);
                }
                else
                {
                    Debug.LogWarning("Player Box Object isn't set");
                }
                //move player to leave position

                goPlayer.transform.position = goLeavePosition.transform.position;
                goPlayer.transform.rotation = goLeavePosition.transform.rotation;
                goPlayer.GetComponent<Rigidbody>().useGravity = true;
                goPlayer.GetComponent<NavMeshAgent>().enabled = true;

                //if the player wasn't crouched, uncrouch
                if (bForceCrouch)
                {
                    if (!playerController.GetIsCrouched())
                    {
                        playerController.Uncrouch();
                    }
                }

                //allow movement
                playerController.SetIsHiding(false);

                //goPlayer.GetComponent<NavMeshAgent>().enabled = true;
                bInUse = false;
            }
            else
            {

                Vector3 newPickExit = goLeavePosition.transform.position;

                Vector3 newExit = goLeavePosition.transform.position + (goLeavePosition.transform.forward * 1.2f);
                Vector3 newExit1 = goLeavePosition.transform.position + (goLeavePosition.transform.right * 1.2f);
                Vector3 newExit2 = goLeavePosition.transform.position + (goLeavePosition.transform.forward * -1.2f);

                if (CheckExit(newExit))
                {
                    newPickExit = newExit;
                }
                else if (CheckExit(newExit1))
                {
                    newPickExit = newExit1;

                }
                else if (CheckExit(newExit2))
                {
                    newPickExit = newExit2;

                }




                //Enable world object
                if (gameObject.GetComponent<MeshRenderer>() != null && gameObject.GetComponent<BoxCollider>())
                {

                    goBoxHide.SetActive(false);

                    gameObject.GetComponent<MeshRenderer>().enabled = true;
                    gameObject.GetComponent<BoxCollider>().isTrigger = false;
                    goPlayer.GetComponent<Rigidbody>().isKinematic = false;

                    //gameObject.GetComponent<NavMeshObstacle>().enabled = true;

                }
                else
                {
                    Debug.LogError("Hiding Place doesn't have a mesh collider or box collider yet somehow passed a raycast check, breaking most laws of physics in the process.");
                }
                //deactivate box object on player
                if (playerController.GetBoxView() != null)
                {
                    playerController.GetBoxView().SetActive(false);
                }
                else
                {
                    Debug.LogWarning("Player Box Object isn't set");
                }
                //move player to leave position
                //goPlayer.GetComponent<NavMeshAgent>().enabled = true;
                goPlayer.transform.position = newPickExit;
                goPlayer.transform.rotation = goLeavePosition.transform.rotation;
                goPlayer.GetComponent<Rigidbody>().useGravity = true;

                //if the player wasn't crouched, uncrouch
                if (bForceCrouch)
                {
                    if (!playerController.GetIsCrouched())
                    {
                        playerController.Uncrouch();
                    }
                }
                //allow movement
                playerController.SetIsHiding(false);

                //goPlayer.GetComponent<NavMeshAgent>().enabled = true;

                bInUse = false;
            }
        }
    }

    public override void Interact()
    {
        bInUse = !bInUse;

        if (bInUse)
        {
            if (playerController != null)
            {
                //Set Player's Min and Max X+Y rotation (for looking around)
                Vector2 minClamp = new Vector2(goHidingPosition.transform.rotation.eulerAngles.y - fHorizontalCameraClamp, fVerticalCameraClamp * -1f);
                playerController.SetHidingCameraMinClamp(minClamp);
                Vector2 maxClamp = new Vector2(goHidingPosition.transform.rotation.eulerAngles.y + fHorizontalCameraClamp, fVerticalCameraClamp);
                playerController.SetHidingCameraMaxClamp(maxClamp);
                Vector2 leanClamp = new Vector2(Mathf.Abs(fHorizontalLeanClamp), Mathf.Abs(fVerticalLeanClamp));
                playerController.SetHidingCameraLeanClamp(leanClamp);
                //stop movement
                playerController.SetIsHiding(true);
                //Force crouching
                if(bForceCrouch)
                {
                    if (!playerController.GetIsCrouched())
                    {
                        playerController.Crouch();
                    }
                }

                //set if the player can crouch
                playerController.SetCanCrouchWhenHiding(bIsCloset);

                //Move player to box position
                //goPlayer.GetComponent<NavMeshAgent>().enabled = false;
                goPlayer.transform.position = goHidingPosition.transform.position;
                goPlayer.transform.rotation = goHidingPosition.transform.rotation;
                goPlayer.GetComponent<Rigidbody>().useGravity = false;
                goPlayer.GetComponent<Rigidbody>().velocity = Vector3.zero;
                goPlayer.GetComponent<Rigidbody>().isKinematic = true;


                //Activate box object on player
                if (playerController.GetBoxView() != null)
                {
                    playerController.GetBoxView().SetActive(true);
                }
                else
                {
                    Debug.LogWarning("Player Box Object isn't set");
                }
                //Disable world object
                if (gameObject.GetComponent<MeshRenderer>() != null && gameObject.GetComponent<BoxCollider>())
                {

                    goBoxHide.SetActive(true);

                    gameObject.GetComponent<MeshRenderer>().enabled = false;
                    gameObject.GetComponent<BoxCollider>().isTrigger = true;

                    if(goInteractCanvas != null)
                    {
                        goInteractCanvas.SetActive(false);
                    }
                }
                else
                {
                    Debug.LogError("Hiding Place doesn't have a mesh collider or box collider yet somehow passed a raycast check, breaking most laws of physics in the process.");
                }
                //Set interactable
                playerController.SetInteractHoldTime(fInteractTime);
            }
        }
        else
        {
            if (playerController != null)
            {

                //Enable world object
                if (gameObject.GetComponent<MeshRenderer>() != null && gameObject.GetComponent<BoxCollider>())
                {

                    goBoxHide.SetActive(false);

                    gameObject.GetComponent<MeshRenderer>().enabled = true;
                    gameObject.GetComponent<BoxCollider>().isTrigger = false;
                    gameObject.GetComponent<NavMeshObstacle>().enabled = true;

                }
                else
                {
                    Debug.LogError("Hiding Place doesn't have a mesh collider or box collider yet somehow passed a raycast check, breaking most laws of physics in the process.");
                }
                //deactivate box object on player
                if (playerController.GetBoxView() != null)
                {
                    playerController.GetBoxView().SetActive(false);
                }
                else
                {
                    Debug.LogWarning("Player Box Object isn't set");
                }

                if(goInteractCanvas != null)
                {
                    goInteractCanvas.SetActive(true);
                }

                //move player to leave position
                goPlayer.transform.position = goLeavePosition.transform.position;
                goPlayer.transform.rotation = goLeavePosition.transform.rotation;

                goPlayer.GetComponent<Rigidbody>().useGravity = true;
                goPlayer.GetComponent<Rigidbody>().isKinematic = false;

                NavMeshHit hit;
                int mask = NavMesh.GetAreaFromName("AI Walkable");

                NavMeshQueryFilter filter = new NavMeshQueryFilter();
                filter.areaMask = 1 << 3;
                filter.agentTypeID = 0;

                if(NavMesh.SamplePosition(goLeavePosition.transform.position + Vector3.up,out hit,2.0f,filter))
                {
                    goPlayer.transform.position = hit.position;
                }

                //goPlayer.GetComponent<NavMeshAgent>().enabled = true;

                //if the player wasn't crouched, uncrouch
                if (bForceCrouch)
                {
                    if (!playerController.GetIsCrouched())
                    {
                        playerController.Uncrouch();
                    }
                }

                //allow movement
                playerController.SetIsHiding(false);


            }
        }
    }

    public bool CheckExit(Vector3 a_PosToCheck)
    {
        LayerMask layerMask = LayerMask.GetMask("AI","Level Geometry");

        foreach (Collider col in Physics.OverlapSphere(a_PosToCheck, 1.0f, layerMask))
        {

            if (col.name != "Floor" && col.gameObject.tag != "GrenadeNet")
            {
                Debug.Log(col.name + " has Exit Blocked");
                return false;
            }
        }

        return true;
    }

    public GameObject GetSearchPoint()
    {
        return goSearchPoint;
    }

}
