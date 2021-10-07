//////////////////////////////////////////////////////////////////
// File Name: Gravity.cs                                        //
// Author: Josh Godsell                                         //
// Date Created: 10/4/19                                        //
// Date Last Edited: 10/4/19                                    //
// Brief: Class to control the room gravity                     //
//////////////////////////////////////////////////////////////////
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Gravity : MonoBehaviour {

    //enum for gravity directions
    public enum GravDirection
    {
        DOWN,
        UP,
        LEFT,
        RIGHT
    }

    //the gravitational constant
    public const float Gravity_Constant = 9.81f;

    //the gravity multipler 
    [SerializeField]
    private float m_fGravityMultiplier = 1;
    //the current gravitational direction and the initial set in the editor
    [SerializeField]
    private GravDirection m_eGravityDirection;

    private GravDirection m_eInitialDirection;

    //a gravitational vector
    private Vector2 m_v2GravityVector;

    // Use this for initialization
    void Start ()
    {           
        //set the normal gravity vector and the initial direction
        m_v2GravityVector = new Vector2(0.0f, -1.0f);

        m_eInitialDirection = m_eGravityDirection;
    }

    // Update is called once per frame
    void Update()
    {        
        //swap the gravitational vector based on the current direction
        switch(m_eGravityDirection)
        {
            case GravDirection.DOWN:
                m_v2GravityVector = new Vector2(0.0f, -1.0f);
                break;
            case GravDirection.UP:
                m_v2GravityVector = new Vector2(0.0f, 1.0f);
                break;
            case GravDirection.LEFT:
                m_v2GravityVector = new Vector2(-1.0f, 0.0f);
                break;
            case GravDirection.RIGHT:
                m_v2GravityVector = new Vector2(1.0f, 0.0f);
                break;
            default:
                m_v2GravityVector = new Vector2(0.0f, -1.0f);
                break;

        }
    }

    //get and set for gravity direction
    public Vector2 GetGravityDirection()
    {
        return m_v2GravityVector;
    }
    public void SetGravityDirection(GravDirection a_eDirection)
    {
        m_eGravityDirection = a_eDirection;
    }

    //get and set for gravity multiplier
    public float GetGravityMultiplier()
    {
        return m_fGravityMultiplier;
    }
    public void SetGravityMultiplier(float a_fMultiplier)
    {
        m_fGravityMultiplier = a_fMultiplier;
    }

    //function to reverse the gravity by swapping the direction
    public void ReverseGrav()
    {
        switch (m_eGravityDirection)
        {
            case GravDirection.DOWN:
                m_eGravityDirection = GravDirection.UP;
                break;
            case GravDirection.UP:
                m_eGravityDirection = GravDirection.DOWN;

                break;
            case GravDirection.LEFT:
                m_eGravityDirection = GravDirection.RIGHT;

                break;
            case GravDirection.RIGHT:
                m_eGravityDirection = GravDirection.LEFT;

                break;
            default:
                break;
        }
    }

    //reset the gravity to its initial direction to be called when the player dies
    public void Reset()
    {
        m_eGravityDirection = m_eInitialDirection;
    }
}
