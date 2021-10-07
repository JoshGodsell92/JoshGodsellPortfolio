using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Rotation : MonoBehaviour {


    //Some ranges for speed and angle of the rotation - KT
    [Range(-8.0f, 8.0f)]
    public float Speed = 1.0f;

    [Range(0.0f, 360.0f)]
    public float Angle = 360.0f;

    //The starting rotation - KT
    [SerializeField]
    private float StartingRotation;

    //Bools to check if it is going left or right - KT
    private bool Left = true;
    private bool Right = false;

	// Use this for initialization
	void Start () {
        //Get the starting rotation - KT
        StartingRotation = this.gameObject.transform.rotation.eulerAngles.z;
	}
	
	// Update is called once per frame
	void Update () {
        //Check if it needs to go left - KT
        if (this.gameObject.transform.rotation.eulerAngles.z < (StartingRotation - (Angle / 2)) && Right)
        {
            Left = true;
            Right = false;
        }
        //Check if it needs to go right - KT
        else if (this.gameObject.transform.eulerAngles.z > (StartingRotation + (Angle / 2)) && Left)
        {
            Left = false;
            Right = true;
        }

        //Rotate it left or right - KT
        if (Left)
        {
            this.gameObject.transform.Rotate(Vector3.forward, Speed);
        }
        else if (Right)
        {
            this.gameObject.transform.Rotate(Vector3.back, Speed);
        }

        //Left
        
        
        
	}
}
