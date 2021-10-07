using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VisionCone : MonoBehaviour
{
    //The vision range of the long range sight of the guards
    //This sight will not be able to see in the dark
    [SerializeField]
    private float fLongRangeSight;

    //The vision range for the shorter range vision over a wider cone
    //This vision can see in the dark
    [SerializeField]
    private float fShortRangeSight;

    //The vision range for the small vision cone that is placed behind the guard to simulate the guard having awareness
    [SerializeField]
    private float fBackRangeSight;

    //The FOV of the long range vision
    [SerializeField]
    private float fLongFOV;
    
    //The FOV of the short range vision
    [SerializeField]
    private float fShortFOV;

    //The FOV of the back range vision
    [SerializeField]
    private float fBackFOV;

    //Player object
    [SerializeField]
    GameObject goPlayer;

    //A layer mask that will be used to ensure the drone only detects objects that they are meant to
    LayerMask ViewableLayer;
    LayerMask IgnoreLayer;

    public bool m_bPlayerSeen = false;

    [SerializeField]
    private bool m_bLongVision = false;
    [SerializeField]
    private bool m_bShortVision = false;
    [SerializeField]
    private bool m_bBackVision = false;

    private bool m_bIsInSmoke = false;


    // Start is called before the first frame update
    void Start()
    {
        //Sets the layer mask used in the physics in this script to the Viewable layer
        ViewableLayer = LayerMask.GetMask("Player");
        IgnoreLayer = LayerMask.GetMask("AI") | LayerMask.GetMask("UI");
    }

    //Physics Update
    private void FixedUpdate()
    {
        if(m_bIsInSmoke)
        {
            m_bPlayerSeen = false;
            return;
        }

        if(goPlayer.GetComponent<PlayerController>().GetIsHiding())
        {
            m_bPlayerSeen = false;
            return;
        }

        float DistanceToPlayer = Vector3.Distance(goPlayer.transform.position, this.transform.position);

        //The drone will create 3 overlap spheres of different ranges and fill lists with the overlaped objects in the Viewable layer

        //The forward facing vector of the drone
        Vector3 v3Forward = gameObject.transform.forward;

        //Loop through the overlaping objects to see if they are in the drone's FOV. I hope to replace angles with fustrum in the future
        foreach (Collider col in Physics.OverlapSphere(transform.position, fLongRangeSight, ViewableLayer))
        {
            try
            {
                if (!col.gameObject.GetComponent<FalsePlayer>().GetInDarkness())
                {
                    //Otherwise find the directional vector between the object and the drone and calculate the angle between that and the drone's forward vector
                    //Vector3 v3DirectionVector = col.gameObject.transform.position - transform.position;
                    Vector3 v3DirectionVector = col.gameObject.transform.position - transform.position;

                    //v3DirectionVector = new Vector3(v3DirectionVector.x, 0.0f, v3DirectionVector.z);

                    float fAngleBetween = Vector3.Angle(v3Forward, v3DirectionVector);

                    //If the object is within the drone's vision angle then fire a raycast towards the object
                    if (fAngleBetween <= fLongFOV)
                    {


                        //Debug.DrawRay(transform.position, v3DirectionVector, Color.blue);

                        //If the raycast hits the object then the drone notices it and will switch behaviour
                        RaycastHit hitLong;
                        if (Physics.Raycast(transform.position, v3DirectionVector, out hitLong, fLongRangeSight, ~IgnoreLayer))
                        {
                            if (hitLong.collider == col)
                            {
                                //Debug.Log(hitLong.collider.gameObject.name + " was seen from long range");
                                m_bLongVision = true;
                            }
                            else
                            {
                                m_bLongVision = false;

                            }
                        }
                        else
                        {
                            m_bLongVision = false;

                        }
                    }
                    else
                    {
                        m_bLongVision = false;

                    }

                }
                else
                {
                    m_bLongVision = false;

                }
            }
            catch
            {

            }
            //Null check to ensure no object creates an exception error with the darkness variable
        } 

        foreach(Collider col in Physics.OverlapSphere(transform.position, fShortRangeSight, ViewableLayer))
        {
            try
            {
                //Short range detection can see regardless of lighting conditions, otherwise the function is the same as long range vision
                Vector3 v3DirectionVector = col.gameObject.transform.position - transform.position;
                float fAngleBetween = Vector3.Angle(v3Forward, v3DirectionVector);

                if (fAngleBetween <= fShortFOV)
                {
                    RaycastHit hitShort;
                    //Debug.Draw+Ray(transform.position, col.gameObject.transform.position, Color.red, 1.0f);
                    if (Physics.Raycast(transform.position, v3DirectionVector, out hitShort, fShortRangeSight, ~IgnoreLayer))
                    {
                        if (hitShort.collider == col)
                        {
                            //Debug.Log(hitShort.collider.gameObject.name + " was seen from short range");
                            m_bShortVision = true;

                        }
                        else
                        {
                            m_bShortVision = false;

                        }
                    }
                    else
                    {
                        m_bShortVision = false;

                    }
                }
                else
                {
                    m_bShortVision = false;

                }
            }
            catch
            {

            }
            
        }

        foreach(Collider col in Physics.OverlapSphere(transform.position, fBackRangeSight, ViewableLayer))
        {
            try
            {
                //Back vision has tiny range but can see behind the drone to simulate spatial awareness
                Vector3 v3DirectionVector = col.gameObject.transform.position - transform.position;
                float fAngleBetween = Vector3.Angle(v3Forward, v3DirectionVector);

                RaycastHit hitBack;
                if (Physics.Raycast(transform.position, v3DirectionVector, out hitBack, fBackRangeSight, ~IgnoreLayer))
                {
                    if (hitBack.collider == col)
                    {
                        //Debug.Log(hitBack.collider.gameObject.name + " was noticed from behind");
                        m_bBackVision = true;

                    }
                    else
                    {
                        m_bBackVision = false;

                    }
                }
                else
                {
                    m_bBackVision = false;

                }
            }

            catch
            {

            }
        }

        if(DistanceToPlayer > fLongRangeSight)
        {
            m_bLongVision = false;
        }
        if(DistanceToPlayer > fShortRangeSight)
        {
            m_bShortVision = false;
        }
        if(DistanceToPlayer > fBackRangeSight)
        {
            m_bBackVision = false;
        }

        if(m_bLongVision || m_bShortVision || m_bBackVision)
        {
            m_bPlayerSeen = true;
        }
        else
        {
            m_bPlayerSeen = false;
        }
    }

    public bool GetInSmoke()
    {
        return m_bIsInSmoke;
    }

    public void SetInSmoke(bool abInSmoke)
    {
        m_bIsInSmoke = abInSmoke;
    }


    public bool GetPlayerSeen()
    {
        return m_bPlayerSeen;
    }

    public float GetAISightTime()
    {
        float t_fTimeTillSeen = 0;

        if (m_bLongVision || m_bShortVision || m_bBackVision)
        {
           
            if(m_bShortVision)
            {
                if(m_bBackVision)
                {
                    t_fTimeTillSeen = 0;
                }
                else
                {
                    t_fTimeTillSeen = 1.5f;
                }
            }
            else
            {
                if (m_bBackVision)
                {
                    t_fTimeTillSeen = 0;
                }
                else
                {
                    t_fTimeTillSeen = 2.5f;
                }
            }

        }
        else
        {

            t_fTimeTillSeen = 100;
        }

        return t_fTimeTillSeen;


    }
}
