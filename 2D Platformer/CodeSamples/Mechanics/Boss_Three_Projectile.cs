//////////////////////////////////////////////////////////////////
// File Name: Boss_Three_Projectile.cs                          //
// Author: Josh Godsell                                         //
// Date Created: 29/5/19                                        //
// Date Last Edited: 29/5/19                                    //
// Brief: class for the third boss projectile                   //
//////////////////////////////////////////////////////////////////
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Boss_Three_Projectile {

    //game manager
    private GameManager m_GameManager;

    //the current position
    private Vector3 m_v3Position;

    //player position
    private Vector3 m_v3PlayerPosition;

    //direction
    private Vector3 m_v3Direction;

    //prefab, instance and bool for is active
    private GameObject m_Prefab;
    private GameObject m_Instance;
    private bool m_bIsActive = false;

    //projectile speed
    private float m_fSpeed = 2.0f;

    // Use this for initialization
    public Boss_Three_Projectile()
    {
        //load the prefab from the resource folder
        try
        {
            m_Prefab = (GameObject)Resources.Load("ErrorProjectile");
        }
        catch (System.Exception)
        {

            throw new System.Exception("ErrorProjectile Prefab not found in 'Resources' folder");
        }

        //get the game manager
        try
        {
            m_GameManager = GameObject.FindObjectOfType<GameManager>();

        }
        catch (System.Exception)
        {

            throw new System.Exception("Game Manager not found");

        }
    }

    // Update is called once per frame
    public void Update()
    {
        //if is active fire and check if it is in the boss room
        if (m_bIsActive)
        {

            Fire();

            CheckInBossRoom();

        }

    }

    //fire function
    public void Fire()
    {
        //deparent the instanced object
        m_Instance.transform.parent = null;

        //get the direction
        m_v3Direction = Vector3.Normalize(m_v3Direction);

        //make the object face the direction
        m_Instance.transform.right = -m_v3Direction;

        //assign the new position
        m_Instance.transform.position += (m_v3Direction * m_fSpeed * Time.deltaTime);

    }

    //function to check if the projectile is inside the boss room if it isnt deactivate the object
    public void CheckInBossRoom()
    {
        Room[] t_Rooms = m_GameManager.GetLevelRooms();

        Room t_BossRoom = null;

        foreach (Room room in t_Rooms)
        {
            if (room.gameObject.name == "Boss Room")
            {
                t_BossRoom = room;
            }
        }

        if (t_BossRoom != null)
        {
            if (!t_BossRoom.m_bounds.Contains(m_Instance.transform.position))
            {
                m_bIsActive = false;
                m_Instance.SetActive(false);
            }
        }
    }

    //reset function
    public void Reset()
    {
        if (m_Instance != null)
        {
            m_Instance.transform.position = Vector3.zero;
            m_Instance.SetActive(false);

        }
        m_bIsActive = false;
    }

    //collision detection function
    public void CollisionDetected(Collision2D a_Collision)
    {


        if (a_Collision.gameObject.tag == "Player")
        {

            m_bIsActive = false;
            m_Instance.SetActive(false);

        }

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
    //Get and Set Position
    public void SetPosition(Vector3 a_v3Position)
    {
        m_v3Position = a_v3Position;
        m_Instance.transform.position = a_v3Position;
    }
    public Vector3 GetPosition()
    {
        return m_v3Position;
    }
    //Get and Set Player Position
    public void SetPlayerPosition(Vector3 a_v3Position)
    {
        m_v3Position = a_v3Position;
    }
    public void SetDirection(Vector3 a_v3Direction)
    {
        m_v3Direction = a_v3Direction;
    }
    public Vector3 GetDirection()
    {
        return m_v3Direction;
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

    IEnumerator DelayedCall(float a_fSecondsToWait, System.Action a_FunctionToUse)
    {
        yield return new WaitForSeconds(a_fSecondsToWait);

        a_FunctionToUse();

        yield return null;

    }
}
