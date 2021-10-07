using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MoveObject : MonoBehaviour {

    //A few floats for the movement. They must be public to be changed in the editor as they are a range. - KT
    [Range(0.0f, 20.0f)]
    public float fMovementRangeX;

    [Range(0.0f, 20.0f)]
    public float fMovementRangeY;

    [Range(0.01f, 0.3f)]
    public float fMovementSpeed;

    //A few floats that are needed to store the position that the object starts at. - KT
    [SerializeField]
    private float fStartingPositionX;

    [SerializeField]
    private float fStartingPositionY;

    //Some bools to check which direction it is moving in. - KT
    private bool bRight = true;
    private bool bLeft = false;
    private bool bUp = true;
    private bool bDown = false;

	// Use this for initialization
	void Start () {
        fStartingPositionX = this.gameObject.transform.position.x;
        fStartingPositionY = this.gameObject.transform.position.y;
	}
	
	// Update is called once per frame
	void Update () {
        //If the object's position is less than the starting position plus half of the range then it can move right. - KT
        if (this.gameObject.transform.position.x > fStartingPositionX + (fMovementRangeX / 2) && bRight)
        {
            bRight = false;
            bLeft = true;
        }
        //Opposite of the one above. - KT
        else if (this.gameObject.transform.position.x < fStartingPositionX - (fMovementRangeX / 2) && bLeft)
        {
            bRight = true;
            bLeft = false;
        }
        //If the object has no range in the x axis then it can't move in that axis. - KT
        if (fMovementRangeX == 0)
        {
            bRight = false;
            bLeft = false;
        }

        //If the object's position is less than the starting position plus half of the range then it can move up. - KT
        if (this.gameObject.transform.position.y > fStartingPositionY + (fMovementRangeY / 2) && bUp)
        {
            bUp = false;
            bDown = true;
        }
        //Opposite of the one above. - KT
        else if (this.gameObject.transform.position.y < fStartingPositionY - (fMovementRangeY / 2) && bDown)
        {
            bUp = true;
            bDown = false;
        }
        //If the object has no range in the y axis then it can't move in that axis. - KT
        if (fMovementRangeY == 0)
        {
            bUp = false;
            bDown = false;
        }

        //Move the object in a direction depending on which way it can move. - KT
        if (bRight)
        {
            this.gameObject.transform.Translate(new Vector2(fMovementSpeed, 0));
        }
        else if (bLeft)
        {
            this.gameObject.transform.Translate(new Vector2(-fMovementSpeed, 0));
        }

        if (bUp)
        {
            this.gameObject.transform.Translate(new Vector2(0, -fMovementSpeed));
        }
        else if (bDown)
        {
            this.gameObject.transform.Translate(new Vector2(0, fMovementSpeed));
        }
	}
}
