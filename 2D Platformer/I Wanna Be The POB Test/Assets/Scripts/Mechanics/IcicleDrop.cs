//////////////////////////////////////////////////////////////////
// File Name: IcicleDrop.cs                                     //
// Author: Josh Godsell                                         //
// Date Created: 23/5/19                                        //
// Date Last Edited: 24/5/19                                    //
// Brief: Class to control the icicle dropper object            //
//////////////////////////////////////////////////////////////////
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IcicleDrop : MonoBehaviour {

    //the dropping speed
    public float m_fDropSpeed;

    //the distance the dropper check to the ground
    public float m_fCheckDistance;

    //the starting pos for the icicle thats dropped
    private Vector3 m_v3IcicleStartPos;

    //bools for checking
    private bool m_bDropIcicle = false;
    private bool m_bCanDropIcicle = true;

    //the icicle object
    private GameObject m_Icicles;

	// Use this for initialization
	void Start ()
    {
        //assign the icicle object from the child list and set the icicle parent to this 
        try
        {
            m_Icicles = this.transform.GetChild(0).gameObject;
            m_Icicles.GetComponent<IcicleImpact>().SetDropp(this);

            m_v3IcicleStartPos = m_Icicles.transform.position;
        }
        catch (System.Exception)
        {

            throw;
        }
	}
	
	// Update is called once per frame
	void Update ()
    {
        if (m_bCanDropIcicle)
        {
            CheckForPlayer();
        }
        if (m_bDropIcicle)
        {
            DropIce();
        }
        
	}

    //function to check if the player is benieth the dropper
    public bool CheckForPlayer()
    {
        //box size to cast and a layer mask
        Vector2 t_v2BoxSize = new Vector2(2.0f, 0.5f);
        int layerMask = 1 << 8;
        layerMask = ~layerMask;

        //get the first hit object
        RaycastHit2D t_RayHit2Ds = Physics2D.BoxCast(this.transform.position, t_v2BoxSize, 0, -this.transform.up,m_fCheckDistance, layerMask);

        //if the object his is a player
        if (t_RayHit2Ds.transform != null)
        {
            if (t_RayHit2Ds.transform.gameObject.tag == "Player")
            {
                //set drop icicle to true and can drop to false and return true
                m_bDropIcicle = true;
                m_bCanDropIcicle = false;
                return true;
            }
        }




        return false;
    }

    //function to drop an icicle
    public void DropIce()
    {
        //Get the dropping vector
        Vector3 t_v3DropVec = new Vector3(0, -1, 0);

        //move the icicle down
        m_Icicles.transform.position += t_v3DropVec * (m_fDropSpeed * Time.deltaTime);

        //if the icicle has passed the checking distance 
        if (m_Icicles.transform.position.y <= m_v3IcicleStartPos.y - m_fCheckDistance)
        {
            //reset the icicle
            m_Icicles.transform.position = m_v3IcicleStartPos;
            m_bDropIcicle = false;
            m_bCanDropIcicle = false;

            //and start a wait
            StartCoroutine(WaitToDrop(1.0f));
        }
    }

    //ienumerator to wait until the icicle can drop again
    public IEnumerator WaitToDrop(float a_SecondsToWait)
    {
        yield return new WaitForSeconds(a_SecondsToWait);

        m_bCanDropIcicle = true;

        yield return null;
    }

    //gizmo for checking distance
    private void OnDrawGizmosSelected()
    {

#if (UNITY_EDITOR)

        //set gizmo color to blue
        Gizmos.color = Color.blue;

        //draw a line of travel
        Gizmos.DrawLine(this.transform.position, this.transform.position - (new Vector3(0,m_fCheckDistance,0)));

#endif
    }
}
