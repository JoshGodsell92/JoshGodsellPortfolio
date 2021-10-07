using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(LineRenderer))]

public class PastTrajectory : MonoBehaviour {

	//how many seconds we track the trajectory - JG
	public int secondsTracked;

	//reference to the frame rate and calculated frames tracked - JG
	private int FrameRate;
	private int framesTracked;
	int frameIndex;

	//reference to the projectile and its attached drag script - JG
	private GameObject projectile;
	private ProjectileDrag projectileScript;

	//Vector3 arrays to store the tracking points and the drawn points - JG
	private Vector3[] trackingPoints;
	private Vector3[] drawPoints;

	// reference to the line renderer - JG
	private LineRenderer trackingLine;

	// bool to check if a new line is needed - JG
	private bool newtrack = false;

    private GameObject GC;
    private GameControl GCScript;

    // Use this for initialization
    void Start ()
    {
     // takes the apllications frame rate - JG
        FrameRate = Application.targetFrameRate;

		// calculates the amount of frames tracked - JG
		framesTracked = secondsTracked * FrameRate;

		// resizes the array to be able tyo fit the frames tracked - JG
		trackingPoints = new Vector3[framesTracked];

		// sets the reference to the projectile and the drag script component - JG
		projectile = this.transform.Find("Projectile").gameObject;
		projectileScript = projectile.GetComponent<ProjectileDrag>();

        GC = GameObject.FindWithTag("GameControl");
        GCScript = GC.GetComponent<GameControl>();

        // sets the tracking line refence and sets its start color and width and start width and end width - JG 
        trackingLine = this.GetComponent<LineRenderer>();
		
        trackingLine.startWidth = .3f;
		trackingLine.endWidth = .01f;



		//frame Index - JG
		frameIndex = 0;

	}

	// Update is called once per frame
	void Update ()
    {
        trackingLine.startColor = GCScript.cLineOneColor;
        trackingLine.endColor = GCScript.cLineTwoColor;
        //checks if the spring is disabled and frame index is less than the tracked frames and not a new track - JG
        if (!projectileScript.isSpringEnabled() && frameIndex <= framesTracked && !newtrack)
		{
			// gets the current position and places it into the array at the index point then increments the frame index - JG
			trackingPoints[frameIndex] = projectile.transform.position;
			frameIndex++;

		}

		// if frame index is equal to frames tracked - JG
		if(frameIndex == framesTracked)
		{
			//sets the positioncount of the line renderer to the same as the final frame index
			// then resets the frame index back to zero, sets the positions to be the tracked points array
			// prints a debug message to the console and then sets newtrack to true - JG
			trackingLine.positionCount = frameIndex;
			frameIndex = 0;
			trackingLine.SetPositions(trackingPoints);
			Debug.Log("Tracked");
			newtrack = true;
            trackingLine.SetPosition(0, GameObject.FindGameObjectWithTag("Player").transform.position);
		}
		
		// if the spring is enabled and the frame index does not equal the frames tracked and not a new track - JG
		if(projectileScript.isSpringEnabled() && frameIndex != framesTracked && !newtrack)
		{
			// if frame index isnt zero then resize the draw points array, then take the points from the corrisponding tracked points
			// and place them into the tracking points array, set the position count of the line renderer to equal the frame index
			// reset the frame index to zero and put the draw points into the line renderer - JG
			if (frameIndex != 0)
			{
				drawPoints = new Vector3[frameIndex];

				for (int i = 0; i < drawPoints.Length; i++)
				{
					drawPoints[i] = trackingPoints[i];
				}

				trackingLine.positionCount = frameIndex;
				frameIndex = 0;
				trackingLine.SetPositions(drawPoints);
                trackingLine.SetPosition(0, GameObject.FindGameObjectWithTag("Player").transform.position);
            }
		}

		// if the spring is enabled and newtrack needed reset newtrack to false - JG
		if(projectileScript.isSpringEnabled() && newtrack)
		{
			newtrack = false;
		}

		
	}

}
