/////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
///Name: RotatingObjects.cs 
///Created by: Charlie Bullock
///Description: This script is for rotating objects around a parent object or simply roating that object, this is at a
///speed specified by a variable editable in the inspector.
/////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RotatingObjects : MonoBehaviour {
    //Variables
    #region Variables
    //Private variables
    [SerializeField]
    private GameObject[] rotationalObjects;
    [SerializeField]
    private GameObject mainRotationalObject;
    [SerializeField]
    private bool bOneObjectRotatingOnly;
    [SerializeField]
    private float fSpeed;

    public bool m_bKeepOrientation = false;
    #endregion Variables

    //Fixed update function which ensures frame rate stable rotation
    private void FixedUpdate()
    {
        //If bOneObjectRotatingOnly is true then just the object itself will rotate
        if (bOneObjectRotatingOnly)
        {
            mainRotationalObject.transform.RotateAround(mainRotationalObject.transform.position, Vector3.forward, fSpeed * Time.deltaTime);
        }
        //Otherwise all objects in the rotationalObjects array is rotated around the main rotational object
        else
        {
            for (int i = 0; i < rotationalObjects.Length;i++)
            {
                rotationalObjects[i].transform.RotateAround(mainRotationalObject.transform.position, Vector3.forward, fSpeed * Time.deltaTime);

                if(m_bKeepOrientation)
                {
                    rotationalObjects[i].transform.up = new Vector3(0, 1, 0);
                }
            }
        }
    }
}
