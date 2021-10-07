using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LightingOverPlayer : MonoBehaviour
{
    //Layermsak to determine the player layer for physics
    LayerMask ViewableLayer;

    private void Awake()
    {
        ViewableLayer = LayerMask.GetMask("Player") | LayerMask.GetMask("Level Geometry");
    }

    public bool RunLighting(GameObject a_Object)
    {
        //Get the type of light this light is
        LightType lightType = GetComponent<Light>().type;

        //Run a different function depending on what light type this is
        switch (lightType)
        {
            case LightType.Spot:
                return SpotLight(a_Object);

            case LightType.Point:
                return PointLight(a_Object);

            //Currently only point and spot lights will light up the player
            //If we use other types of light they are easy to add to this script
            default:
                return false;
        }
    }

    //Different functions for different types of light
    //Spotlight will use a "vision cone" of a similar style to the AI vision cones
    private bool SpotLight(GameObject a_Object)
    {
        //bool of object's status to be returned
        bool bIsLitUp = false;

        //The direction that the light is pointing
        Vector3 v3LightPointing = gameObject.transform.forward;

        //The direction that the light is pointing will use a fustrum and range check to see if the player is in its light
        Vector3 v3DirectionVector = a_Object.transform.position - transform.position;
        float fAngleBetween = Vector3.Angle(v3LightPointing, v3DirectionVector);


        if (fAngleBetween <= (GetComponent<Light>().innerSpotAngle * 0.5f) && Vector3.Distance(transform.position, a_Object.transform.position) <= (GetComponent<Light>().range))
        {
            //Return true if the object is in the light
            bIsLitUp = true;
        }

        //Return if the object is lit up or not
        return bIsLitUp;
    }

    //A point light uses a raycast to see if the player is in its raneg 
    private bool PointLight(GameObject a_Object)
    {
        //Since the light is not concentrated on one spot, an angle check is not needed
        //bool of object's status to be returned
        bool bIsLitUp = false;

        //Direction in which to fire a raycast
        Vector3 v3DirectionVector = a_Object.transform.position - transform.position;

        if (Vector3.Distance(transform.position, a_Object.transform.position) <= GetComponent<Light>().range)
        {
            //Raycast is used to see if the light hits the player
            RaycastHit hit;
            if (Physics.Raycast(transform.position, v3DirectionVector, out hit))
            {
                if (hit.collider == a_Object.GetComponent<Collider>())
                {
                    //If the object was hit, then it is lit up
                    bIsLitUp = true;
                }
            }
        }

        //Return if the object is lit up or not
        return bIsLitUp;
    }

}
