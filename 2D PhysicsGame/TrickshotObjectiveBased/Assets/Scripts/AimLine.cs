using System.Collections;
using System.Collections.Generic;
using UnityEngine;


//ensures there is always a LineRenderer added to the entity this script is attached to - JG
[RequireComponent(typeof(LineRenderer))]
public class AimLine : MonoBehaviour {


    //variables for Game control and game control script - JG
    private GameObject goGameController;
    private GameControl gcGameControlScript;

    //variables for player start point and projectile and projectile dragging script - JG
    private GameObject goPlayer;
	private GameObject goProjectile;
	private ProjectileDrag pdProjectileDragScript;

    //variable for a ray - JG
    private Ray rRayToPointer;

    //variable for this objects line renderer - JG
    LineRenderer lrAimLine;

    //variables for arc calculations - JG
    public float fVelocity;
    public float fAngle;
    public int iResolution = 30;
    float fGravity;
    public float fRadianAngle;
    public Vector3 v3PlayerToPointer;
    public float fDistanceToBall;

    private void Awake()
    {
        //Gets line render component of the object and assigns it - JG
        lrAimLine = GetComponent<LineRenderer>();
        fGravity = Mathf.Abs(Physics2D.gravity.y);

        //Gets the player and projectile as well as the game controller - JG 
        goPlayer = this.gameObject;
        goProjectile = GameObject.FindGameObjectWithTag("Projectile");
        pdProjectileDragScript = goProjectile.GetComponent<ProjectileDrag>();

        goGameController = GameObject.FindWithTag("GameControl");
        gcGameControlScript = goGameController.GetComponent<GameControl>();

    }

    private void OnValidate()
    {
        //check linerenderer is not null and that the game is playing - JG
        if (lrAimLine != null && Application.isPlaying)
        {
            //RenderArc();
        }
    }
   
    // Update is called once per frame
    void Update()
	{

        //draws a ray starting from the players position going no where- JG
        rRayToPointer = new Ray(goPlayer.transform.position, Vector3.zero);

        // checks if projectile is pressed on - JG
        if (pdProjectileDragScript.pressedOn)
		{
            // if the projectile is pressed on then it enables the line renderer - JG
            lrAimLine.enabled = true;

            //takes the predicted velocity in - JG
            fVelocity = pdProjectileDragScript.predictedVelocity.magnitude / 2;

            //calls render arc - JG
            RenderArc();

        }
        else
		{
            //if not pressed on disable line renderer - JG
            lrAimLine.enabled = false;
		}
	}
    

    void RenderArc()
    {

        //vector between player and projectile positions for arc calculations - JG
        v3PlayerToPointer = goProjectile.transform.position - goPlayer.transform.position;
        fDistanceToBall = v3PlayerToPointer.magnitude;

        //angle between the player and horizontal - JG
        Vector3 aimVector = -v3PlayerToPointer;
        fAngle = Vector2.Angle(aimVector, goPlayer.transform.right);
        fRadianAngle = Mathf.Deg2Rad * fAngle;

        // sets line size and sorting layer - JG
        lrAimLine.sortingOrder = -1;
        lrAimLine.startWidth = 0.5f;
        lrAimLine.endWidth = 0.1f;

        //sets the colour of the line from the game control script  - JG
        lrAimLine.startColor = gcGameControlScript.cAimLineColor;
        lrAimLine.endColor = gcGameControlScript.cAimLineColor2;

        // takes the resolution and adds that many points to the line renderer 
        // and calculates each position in the arc array - JG
        lrAimLine.positionCount = iResolution + 1;
        lrAimLine.SetPositions(CalculateArcArray());
    }

    Vector3[] CalculateArcArray()
    {
        // an array of positions equal to the size of the line renderers points  - JG
        Vector3[] arcArray = new Vector3[iResolution + 1];

        // calculates the furthest point the projectile will travel  - JG
        float maxDistance = ((fVelocity * fVelocity) * Mathf.Sin(2 * fRadianAngle)) / fGravity;

        // loop for calculating each points position  - JG
        for (int i = 0; i <= iResolution;i++)
        {
            float t = (float)i / (float)iResolution;
            arcArray[i] = CalculateArcPoint(i,maxDistance);
        }

        return arcArray;
    }

    Vector3 CalculateArcPoint(float a_t,float a_maxDistance)
    {
        float x = a_t * a_maxDistance;
 
        float y = x * Mathf.Tan(fRadianAngle) - ((fGravity * x * x) / (2 * (fVelocity * fVelocity) * Mathf.Cos(fRadianAngle) * Mathf.Cos(fRadianAngle)));

        // calculates each point at time step a_t using velocity and gravity  - JG
        float t = 1f / 30.0f;
        Vector3 stepVel = t * pdProjectileDragScript.predictedVelocity;
        Vector3 stepGrav = t * t * Physics2D.gravity;

        Vector2 newPos = goPlayer.transform.position + a_t * stepVel + (0.5f * (a_t * a_t + a_t)) * stepGrav;

        if (goProjectile.transform.position.y > goPlayer.transform.position.y)
        {
            y = x * Mathf.Tan(fRadianAngle) + ((fGravity * x * x) / (2 * (fVelocity * fVelocity) * Mathf.Cos(fRadianAngle) * Mathf.Cos(fRadianAngle)));
            y = -y;
        }

        return new Vector3(newPos.x,newPos.y);

    }

    //functions for setting line colour  - JG 
    public void SetColour(Color a_Colour)
    {
        lrAimLine.startColor = a_Colour;
    }

    public void SetColour2(Color a_Colour)
    {
        lrAimLine.endColor = a_Colour;

    }
}
