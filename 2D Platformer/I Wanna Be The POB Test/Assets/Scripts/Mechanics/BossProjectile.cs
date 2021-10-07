//////////////////////////////////////////////////////////////////
// File Name: BossProjectile.cs                                 //
// Author: Josh Godsell                                         //
// Date Created: 22/3/19                                        //
// Date Last Edited: 22/3/19                                    //
// Brief: Class for first boss projectile                       //
//////////////////////////////////////////////////////////////////
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BossProjectile {
     
    //potential projectile states
    enum PROJECTILE_STATE
    {
        FLUNG,
        FALL,
    }

    //the game manager and boss controller
    private GameManager m_GameManager;
    private BossController m_BossController;
                    
    //the current state
    private PROJECTILE_STATE m_eProjState = PROJECTILE_STATE.FLUNG;

    //the current position
    private Vector3 m_v3Position;

    //the starting fall pos
    private Vector3 m_v3FallStartPos;

    //the prefab,instance and active bool
    private GameObject m_Prefab;
    private GameObject m_Instance;
    private bool m_bIsActive = false;

    //the fling height and fall speed
    public float m_fFlingHeight = 1.0f;
    public float m_fFallSpeed = 2.0f;

    //the time along a parabolic curve
    private float m_fParabolaTime;

    //an audio clip
    private AudioClip m_Audio;

	//constructor
	public BossProjectile()
    {
        //get the prefab and audio clip from the resources folder
        try
        {
            m_Prefab = (GameObject)Resources.Load("BossProjectile");
            m_Audio = (AudioClip)Resources.Load("GlassSmash");

        }
        catch (System.Exception)
        {

            throw new System.Exception("Player Bullet Prefab not found in 'Resources' folder");
        }

        //get the game manager and player control
        try
        {
            m_GameManager = GameObject.FindObjectOfType<GameManager>();
            m_BossController = GameObject.FindObjectOfType<BossController>();

        }
        catch (System.Exception)
        {

            throw new System.Exception("Game Manager not found");
        }
    }

    // Update is called once per frame
    public void Update()
    {

        //if is active then update based on the current state
        if (m_bIsActive)
        {
            switch (m_eProjState)
            {
                case PROJECTILE_STATE.FLUNG:
                    Fling();
                    break;
                case PROJECTILE_STATE.FALL:
                    Fall();
                    break;
                default:
                    break;
            }
        }
    }

    //fall fuction
    public void Fall()
    {

        //calculate and assign the new position of the instance object
        Vector3 t_v3NewPos = m_Instance.transform.position;

        t_v3NewPos.y -= m_fFallSpeed * Time.deltaTime;

        m_Instance.transform.position = t_v3NewPos;


        //rotate the object slightly
        Vector3 t_v3Rotation = m_Instance.transform.localEulerAngles;

        t_v3Rotation.z -= 15.0f;

        m_Instance.transform.localEulerAngles = t_v3Rotation;

    }

    //fling function
    public void Fling()
    {
        //increment the parabola time by the frame time
        m_fParabolaTime += Time.deltaTime;

        //get the new instance position from the parabola function
        m_Instance.transform.position = Parabola(m_v3Position, m_v3FallStartPos, m_fFlingHeight, m_fParabolaTime);

        //rotate the object slightly
        Vector3 t_v3Rotation = m_Instance.transform.localEulerAngles;

        t_v3Rotation.z -= 15.0f;

        m_Instance.transform.localEulerAngles = t_v3Rotation;

        //if the instance height exceeds the dropping height
        if(m_Instance.transform.position.y >= m_v3FallStartPos.y)
        {
            //then set the position to the dropping position and switch the state to the fall state
            m_Instance.transform.position = m_v3FallStartPos;
            m_eProjState = PROJECTILE_STATE.FALL;
        }
    }

    //collision
    public void CollisionDetected(Collision2D a_Collision)
    {

        //if the projectile collides with a player or platform then deactivate the projectile and reset the parabola time
        //also trigger the audio clip
        if(a_Collision.gameObject.tag == "Platform")
        {

            m_bIsActive = false;

            m_BossController.PlayAudio(m_Audio,0.1f);

            m_fParabolaTime = 0;
            m_Instance.SetActive(false);
        }
        else if (a_Collision.gameObject.tag == "Player")
        {

            m_bIsActive = false;

            m_BossController.PlayAudio(m_Audio,0.1f);

            m_fParabolaTime = 0;
            m_Instance.SetActive(false);

        }

    }

    //function to calculate the parabola curve for the bounce 
    public Vector2 Parabola(Vector2 a_v2Start, Vector2 a_v2End, float a_fHeight, float a_fTime)
    {

        //parabola calculation
        System.Func<float, float> f = x => -4 * a_fHeight * x * x + 4 * a_fHeight * x;

        //move the vector along the curve for the time
        Vector2 mid = Vector2.Lerp(a_v2Start, a_v2End, a_fTime);

        //return the position along the curve at this time step
        return new Vector2(mid.x, f(a_fTime) + Mathf.Lerp(a_v2Start.y, a_v2End.y, a_fTime));
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
    }
    public Vector3 GetPosition()
    {
        return m_v3Position;
    }

    public void SetFallPosition(Vector3 a_v3Position)
    {
        m_v3FallStartPos = a_v3Position;
    }
    public Vector3 GetFallPosition()
    {
        return m_v3FallStartPos;
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

    #region StateChangers

    public void FallState()
    {
        m_eProjState = PROJECTILE_STATE.FALL;
    }

    public void FlungState()
    {
        m_eProjState = PROJECTILE_STATE.FLUNG;
    }

    #endregion

    IEnumerator DelayedCall(float a_fSecondsToWait, System.Action a_FunctionToUse)
    {
        yield return new WaitForSeconds(a_fSecondsToWait);

        a_FunctionToUse();

        yield return null;

    }
}
