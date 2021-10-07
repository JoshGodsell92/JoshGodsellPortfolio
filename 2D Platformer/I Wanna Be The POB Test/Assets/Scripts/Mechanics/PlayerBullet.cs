//////////////////////////////////////////////////////////////////
// File Name: PlayerBullet.cs                                   //
// Author: Josh Godsell                                         //
// Date Created: 25/1/19                                        //
// Date Last Edited: 25/1/19                                    //
// Brief: Class containing player bullet functionality          //
//////////////////////////////////////////////////////////////////

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerBullet {

    //bools for checks
    private bool m_bIsActive = false;
    private bool m_bCheckVisible = false;
    private bool m_bDelayStarted = false;

    //the bullet speed
    private float m_fBulletSpeed = 5.0f;

    //the travel direction
    private Vector2 m_v2TravelDirection;

    //the bullet prefab
    private GameObject m_Prefab;

    //the instance of the bullet atached to the script
    private GameObject m_Instance;

    //the bullets parent object
    private GameObject m_Parent;

    //the bullets current position
    private Vector3 m_v3Position;

    //default constructor
    public PlayerBullet(GameObject a_ParentObject)
    {
        //assign the parent object
        m_Parent = a_ParentObject;

        //try and find the prefab in the resources folder
        try
        {
            m_Prefab = (GameObject)Resources.Load("PlayerBullet");

        }
        catch (System.Exception)
        {

            throw new System.Exception("Player Bullet Prefab not found in 'Resources' folder");
        }
    }

    // Update is called once per frame
    public void Update()
    {
        //if the bullet is active then activate the game object
        if (m_bIsActive)
        {
            //call the travel function and check the bullets rendered state
            m_Instance.SetActive(true);
            BulletTravel();

            BulletRenderedCheck();
        }
    }

    //function for bullet travel
    private void BulletTravel()
    {
        //get the position as a vector2
        Vector2 t_v2Position = m_v3Position;

        //set the forward direction
        m_Instance.transform.right = m_v2TravelDirection;

        //calculate a new position
        Vector2 t_v2NewPosition = t_v2Position + (m_v2TravelDirection * m_fBulletSpeed * Time.deltaTime);

        //store the new position and move the instanced object
        m_v3Position = t_v2NewPosition;

        m_Instance.transform.position = m_v3Position;
    }

    //assign the bullet instance and parents position
    public void InstancedBullet(GameObject a_Instance)
    {
        m_Instance = a_Instance;
        m_v3Position = m_Parent.transform.position;
    }

    //function to check if the bullet is rendered
    public void BulletRenderedCheck()
    {
        //if the instacne is set
        if(m_Instance != null)
        {

            //and check is visible is true
            if (m_bCheckVisible)
            {
                //get the renderer component for the bullet instance
                SpriteRenderer t_Renderer = m_Instance.GetComponent<SpriteRenderer>();

                //if not visible
                if (!t_Renderer.isVisible)
                {
                    //set active to false and the instance to false reset the position to the parent 
                    m_bIsActive = false;
                    m_Instance.SetActive(false);
                    m_v3Position = m_Parent.transform.position;
                                             
                    //reset the bools for checks
                    m_bCheckVisible = false;
                    m_bDelayStarted = false;

                }
            }
            else if(!m_Instance.activeSelf)
            {
                //if the instance is not active the reset the bools and position
                m_bIsActive = false;
                m_v3Position = m_Parent.transform.position;

                m_bCheckVisible = false;
                m_bDelayStarted = false;
            }
        }
    }

    //function to reset the bullet remotly
    public void Reset()
    {
        //set all the variables back to a default state
        m_bIsActive = false;
        m_Instance.SetActive(false);
        m_Instance.transform.position = m_Parent.transform.position;
        m_v3Position = m_Parent.transform.position;

        m_bCheckVisible = false;
        m_bDelayStarted = false;
    }

    #region Get&Set

    //Get and Set for IsActive 
    public void SetIsActive(bool a_bool)
    {
        m_bIsActive = a_bool;
    }
    public bool GetIsActive()
    {
        return m_bIsActive;
    }

    //Get and Set for DelayStarted 
    public void SetDelayStarted(bool a_bool)
    {
        m_bDelayStarted = a_bool;
    }
    public bool GetDelayStarted()
    {
        return m_bDelayStarted;
    }


    //Get and Set For CheckVisible
    public void SetCheckVisible(bool a_bool)
    {
        m_bCheckVisible = a_bool;
    }
    public bool GetCheckVisible()
    {
        return m_bCheckVisible;
    }

    //Get and Set for Travel Direction
    public void SetTravelDirection(Vector2 a_v2TravelDiriection)
    {
        m_v2TravelDirection = a_v2TravelDiriection;

    }
    public Vector2 GetTravelDirection()
    {
        return m_v2TravelDirection;
    }

    //Get and Set Position
    public void SetPosition(Vector3 a_v3Position)
    {
        m_v3Position = a_v3Position;
    }
    public Vector3 GetPosition()
    {
        return m_v3Position;
    }

    //Get and Set Prefab
    public void SetPrefab(GameObject a_Prefab)
    {
        m_Prefab = a_Prefab;
    }
    public GameObject GetPrefab()
    {
        return m_Prefab;
    }

    //Get and Set Instance
    public void SetInstance(GameObject a_Instance)
    {
        m_Instance = a_Instance;
    }
    public GameObject GetInstance()
    {
        return m_Instance;
    }

    #endregion
}
