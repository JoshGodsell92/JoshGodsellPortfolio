//////////////////////////////////////////////////////////////////
// File Name: PlatformController.cs                             //
// Author: Josh Godsell                                         //
// Date Created: 23/2/19                                        //
// Date Last Edited: 24/2/19                                    //
// Brief: Class containing Platform functions and controls      //
//////////////////////////////////////////////////////////////////
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlatformController : MonoBehaviour {

    //enumeration of directions
    public enum DIRECTION
    {
        HORIZONTAL,
        VERTICAL,
        DIAGONAL,
        PATH,
    }



    //this instances travel direction
    [HideInInspector]
    [SerializeField]
    private DIRECTION m_eTravelDirection = DIRECTION.HORIZONTAL;

    //angle for diagonal movement
    [SerializeField]
    private float m_fAngle = 45.0f;

    //the distance to travel each way from origin
    [SerializeField]
    private float m_fDistance = 1.0f;
    //the speed of travel
    [SerializeField]
    private float m_fSpeed = 1.0f;

    [SerializeField]
    private float m_fWaitPathDelay = 0.0f;

    //nodes for path 
    [SerializeField]
    private GameObject[] m_PathPoints;

    //Variables for calculations
    private Vector3 m_v3StartPos;

    //index for path
    private int m_iTargetIndex = 0;

    //vector3 array for path positions
    private Vector3[] m_aTargetPositions;

    private bool m_bWaitStart = false;

    private void OnEnable()
    {
        //assign the start position
        m_v3StartPos = this.gameObject.transform.position;

        //calculate the target positions
        CalculateTargetPositions();

    }

    private void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

        //switch statement for which function to use for movement
        switch (m_eTravelDirection)
        {
            case DIRECTION.HORIZONTAL:

                HorizontalMovement();

                break;

            case DIRECTION.VERTICAL:

                VerticalMovement();

                break;
            case DIRECTION.DIAGONAL:

                DiagonalMovement();

                break;
            case DIRECTION.PATH:

                PathMovement();
                break;
            default:

                break;
        }

    }

    //function for horizontal movement
    public void HorizontalMovement()
    {
        //get the current x and the target x
        float t_fCurrentX = this.transform.position.x;
        float t_fTargetX = m_aTargetPositions[m_iTargetIndex].x;

        if (t_fCurrentX != t_fTargetX)
        {
            //if the current x is less than the target x
            if (t_fCurrentX < t_fTargetX)
            {
                //and speed is less than 0
                if (m_fSpeed < 0)
                {
                    //reverse the speed
                    m_fSpeed = -m_fSpeed;
                }
            }
            else
            {
                //if speed is greater than zero the reverse the speed
                if (m_fSpeed > 0)
                {
                    m_fSpeed = -m_fSpeed;
                }
            }

            //calculate a new x position
            Vector3 t_v3CurrentPos = this.transform.position;

            t_v3CurrentPos.x += m_fSpeed * Time.deltaTime;

            //if speed is greater than 0 and the current x is greater than the target x clamp x t the target
            if (m_fSpeed > 0)
            {
                if (t_v3CurrentPos.x > t_fTargetX)
                {
                    t_v3CurrentPos.x = t_fTargetX;
                }
            }
            //same but for when speed is less than 0 and the x is less than the target
            else if (m_fSpeed < 0)
            {
                if (t_v3CurrentPos.x < t_fTargetX)
                {
                    t_v3CurrentPos.x = t_fTargetX;
                }
            }

            //assign the new position
            this.transform.position = t_v3CurrentPos;

        }
        else
        {
            //if the current x equals the target x then increase the target index
            if (m_iTargetIndex != m_aTargetPositions.Length - 1)
            {
                ++m_iTargetIndex;
            }
            else
            {
                m_iTargetIndex = 0;
            }
        }
    }

    //function for Vertical Movement same as the x but for y
    public void VerticalMovement()
    {
        float t_fCurrentY = this.transform.position.y;
        float t_fTargetY = m_aTargetPositions[m_iTargetIndex].y;

        if (t_fCurrentY != t_fTargetY)
        {
            if (t_fCurrentY < t_fTargetY)
            {
                if (m_fSpeed < 0)
                {
                    m_fSpeed = -m_fSpeed;
                }
            }
            else
            {
                if (m_fSpeed > 0)
                {
                    m_fSpeed = -m_fSpeed;
                }
            }

            Vector3 t_v3CurrentPos = this.transform.position;

            t_v3CurrentPos.y += m_fSpeed * Time.deltaTime;

            if (m_fSpeed > 0)
            {
                if (t_v3CurrentPos.y > t_fTargetY)
                {
                    t_v3CurrentPos.y = t_fTargetY;
                }
            }
            else if (m_fSpeed < 0)
            {
                if (t_v3CurrentPos.y < t_fTargetY)
                {
                    t_v3CurrentPos.y = t_fTargetY;
                }
            }

            this.transform.position = t_v3CurrentPos;

        }
        else
        {
            if (m_iTargetIndex != m_aTargetPositions.Length - 1)
            {
                ++m_iTargetIndex;
            }
            else
            {
                m_iTargetIndex = 0;
            }
        }
    }

    //Function for Diagonal Movement
    public void DiagonalMovement()
    {
        Vector3 t_v3CurrentPos = this.transform.position;

        Vector3 t_CalcVec = t_v3CurrentPos - m_v3StartPos;

        if (t_CalcVec.magnitude > m_fDistance)
        {

            m_fSpeed = -m_fSpeed;

            if (m_iTargetIndex != m_aTargetPositions.Length - 1)
            {
                ++m_iTargetIndex;
            }
            else
            {
                m_iTargetIndex = 0;

            }
        }


        Vector3 t_v3TargetDir = new Vector3(0.0f, m_fDistance, 0.0f);

        t_v3TargetDir = Quaternion.AngleAxis(m_fAngle, Vector3.back) * t_v3TargetDir;

        t_v3CurrentPos += Vector3.Normalize(t_v3TargetDir) * (m_fSpeed * Time.deltaTime);

        this.transform.position = t_v3CurrentPos;
    }

    //function for following a path of positions
    public void PathMovement()
    {

        //get the current position the target position and claculate the direction
        Vector3 t_v3CurrentPos = this.transform.position;

        Vector3 t_v3TargetPos = m_PathPoints[m_iTargetIndex].transform.position;

        Vector3 t_v3TargetDir =  t_v3TargetPos - t_v3CurrentPos;


        //if the distance from the target position is less than or equal to .1
        if (Vector3.Distance(t_v3CurrentPos, t_v3TargetPos) <= .1)
        {
            // if there is no wait delay then increment the index
            if (m_fWaitPathDelay == 0)
            {

                if (m_iTargetIndex != m_PathPoints.Length - 1)
                {
                    ++m_iTargetIndex;
                }
                else
                {
                    m_iTargetIndex = 0;

                }
            }
            else
            {
                //otherwise start a delayed increase coroutine
                if (!m_bWaitStart)
                {
                    StartCoroutine(WaitIncreaseIndex(m_fWaitPathDelay));
                }
            }
        }
        else
        {

            //if the target has not yet been reached then move the platform
            t_v3CurrentPos += Vector3.Normalize(t_v3TargetDir) * (m_fSpeed * Time.deltaTime);

            this.transform.position = t_v3CurrentPos;
        }
    }

    //fuction to calculate the target positions
    public void CalculateTargetPositions()
    {
        m_aTargetPositions = new Vector3[2];

        switch (m_eTravelDirection)
        {
            case DIRECTION.HORIZONTAL:
                {
                    Vector3 t_v3TargetVec = m_v3StartPos;
                    Vector3 t_v3TargetVec2 = m_v3StartPos;
                    t_v3TargetVec.x += m_fDistance;
                    t_v3TargetVec2.x -= m_fDistance;

                    m_aTargetPositions[0] = t_v3TargetVec;
                    m_aTargetPositions[1] = t_v3TargetVec2;

                    break;
                }
            case DIRECTION.VERTICAL:
                {
                    Vector3 t_v3TargetVec = m_v3StartPos;
                    Vector3 t_v3TargetVec2 = m_v3StartPos;
                    t_v3TargetVec.y += m_fDistance;
                    t_v3TargetVec2.y -= m_fDistance;

                    m_aTargetPositions[0] = t_v3TargetVec;
                    m_aTargetPositions[1] = t_v3TargetVec2;
                    break;
                }
            case DIRECTION.DIAGONAL:
                {
                    Vector3 t_v3TargetVec = m_v3StartPos;
                    Vector3 t_v3TargetVec2 = m_v3StartPos;

                    Vector3 t_v3TargetDir = new Vector3(0.0f, m_fDistance, 0.0f);

                    t_v3TargetDir = Quaternion.AngleAxis(m_fAngle, Vector3.back) * t_v3TargetDir;

                    t_v3TargetVec += t_v3TargetDir;

                    m_aTargetPositions[0] = t_v3TargetVec;

                    t_v3TargetVec2 -= t_v3TargetDir;

                    m_aTargetPositions[1] = t_v3TargetVec2;

                    break;
                }
            case DIRECTION.PATH:

                break;
            default:

                break;
        }
    }


    #region Get&Set Functions
    public void SetSpeed(float a_fSpeed)
    {
        m_fSpeed = a_fSpeed;
    }
    public float GetSpeed()
    {
        return m_fSpeed;
    }

    public void SetDistance(float a_fDistance)
    {
        m_fDistance = a_fDistance;
    }
    public float GetDistance()
    {
        return m_fDistance;
    }

    public void SetTravelDirection(DIRECTION a_eDirection)
    {
        m_eTravelDirection = a_eDirection;
    }
    public DIRECTION GetTravelDirection()
    {
        return m_eTravelDirection;
    }
    public void SetPathPointSize(int a_size)
    {
        m_PathPoints = new GameObject[a_size];
    }
    #endregion
      
    //coroutine to delay an index increase by a set time
    public IEnumerator WaitIncreaseIndex(float a_fSecondsToWait)
    {
        m_bWaitStart = true;

        yield return new WaitForSeconds(a_fSecondsToWait);

        if (m_iTargetIndex != m_PathPoints.Length - 1)
        {
            ++m_iTargetIndex;
        }
        else
        {
            m_iTargetIndex = 0;

        }


        m_bWaitStart = false;

        yield return null;
    }

    //function to draw a gizmo in the editor to show where the platform will be at either end of the travel
    private void OnDrawGizmosSelected()
    {

#if (UNITY_EDITOR)

        //if the application isnt playing calculate the target positions for the gizmos to be drawn
        if(!Application.isPlaying)
        {
            m_v3StartPos = this.transform.position;
            CalculateTargetPositions();

        }

        //set gizmo color to blue
        Gizmos.color = Color.blue;

        //draw a line of travel
        Gizmos.DrawLine(m_aTargetPositions[0], m_aTargetPositions[1]);

        //Gizmos.matrix = transform.localToWorldMatrix;
        Gizmos.matrix = Matrix4x4.TRS(m_aTargetPositions[0], this.transform.rotation, this.transform.lossyScale);

        //draw the space the platform will occupy at each target position
        Gizmos.DrawWireCube(Vector3.zero, Vector3.one);

        Gizmos.matrix = Matrix4x4.TRS(m_aTargetPositions[1], this.transform.rotation, this.transform.lossyScale);

        Gizmos.DrawWireCube(Vector3.zero, Vector3.one);

#endif
    }


}
