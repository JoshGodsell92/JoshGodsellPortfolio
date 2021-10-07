/////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
///Name: MovingObjectTrigger.cs 
///Created by: Charlie Bullock
///Description: This script allows a selected gameobject to be moved at a set speed, between set positions and if to
///repeat as well as way moving object is activated
/////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MovingObjectTrigger : MonoBehaviour {
    //Variables
    #region Variables
    [SerializeField]
    private GameObject MovingObject;
    [SerializeField]
    private float fStartingXposition;
    [SerializeField]
    private float fStartingYposition;
    [SerializeField]
    private float fEndingXposition;
    [SerializeField]
    private float fEndingYposition;
    [SerializeField]
    private float fSpeed;
    [SerializeField]
    private bool bReturn;
    [SerializeField]
    private bool bActivateByJumpingInput;
    private bool bReturning;
    private bool bActivated;
    private bool bJumpedPlayer;
    #endregion Variables

    //Start function simply setting various booleans to false
    void Start () {
        bJumpedPlayer = false;
        bActivated = false;
        bReturning = false;
    }

    //Fixed update function checking for player jumping
    private void FixedUpdate()
    {
        if (bActivateByJumpingInput && Input.GetKeyDown(KeyCode.W) && bJumpedPlayer == false)
        {
            bJumpedPlayer = true;
        }

        if (bActivateByJumpingInput == false)
        {
            MoveObject();
        }
        else if (bJumpedPlayer == true || bActivated)
        {
            MoveObject();
        }
    }

    //Trigger collision function for checking if player is above moving object and triggering boolean for this if true
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.tag == "Player")
        {
            bActivated = true;
        }
    }

    //Function for moving the object, firstly this will check that the bReturn variable is true as if it is not the moving obect will only go to set destination and would not return
    void MoveObject()
    {
        if (bReturn)
        {
            if (bReturning)
            {
                if (this.gameObject.transform.position == new Vector3(fStartingXposition, fStartingYposition, transform.position.z))
                {
                    bJumpedPlayer = false;
                    bReturning = false;
                }
                transform.position = Vector3.MoveTowards(transform.position, new Vector3(fStartingXposition, fStartingYposition, transform.position.z), Time.deltaTime * fSpeed);
            }
            else
            {
                if (this.gameObject.transform.position == new Vector3(fEndingXposition, fEndingYposition, transform.position.z))
                {
                    bJumpedPlayer = false;
                    bReturning = true;
                }
                transform.position = Vector3.MoveTowards(transform.position, new Vector3(fEndingXposition, fEndingYposition, transform.position.z), Time.deltaTime * fSpeed);
            }
        }
        else
        {
            if (bActivated)
            {
                if (this.gameObject.transform.position == new Vector3(fEndingXposition, fEndingYposition, transform.position.z))
                {
                    bJumpedPlayer = false;
                }
                transform.position = Vector3.MoveTowards(transform.position, new Vector3(fEndingXposition, fEndingYposition, transform.position.z), Time.deltaTime * fSpeed);
            }
        }

    }

}
