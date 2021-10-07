using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Camera))]

public class CameraControl : MonoBehaviour {

	//references to this objects camera component and its original transform - JG
	private Camera cmThisCamera;
	private Transform tCamOrigin;

	private float fZoomSpeed = .5f;

	//references to the player game object and its projectile - JG
	private GameObject goPlayer;
	private GameObject goProjectile;

	private Movement mPlayerMovementScript;


	//the projectile Drag script attached to the projectile - JG
	private ProjectileDrag pdProjectileDragScript;

	private GameObject goResetButton;

	private float fDifference = 5;

	private float fHeight;

	// Use this for initialization
	void Start ()
	{
		cmThisCamera = GetComponent<Camera>();
		tCamOrigin = cmThisCamera.transform;

		goResetButton = GameObject.FindGameObjectWithTag("ResetButton");

		goPlayer = GameObject.FindGameObjectWithTag("Player");

		mPlayerMovementScript = goPlayer.GetComponent<Movement>();

		goProjectile = goPlayer.transform.Find("Projectile").gameObject;
		pdProjectileDragScript = goProjectile.GetComponent<ProjectileDrag>();

	}

	// Update is called once per frame
	void Update()
	{

		if(!mPlayerMovementScript.getAllowMove())
		{
			pdProjectileDragScript = goProjectile.GetComponent<ProjectileDrag>();
		}
		else
		{
			goProjectile = goPlayer.transform.Find("Projectile").gameObject;
			pdProjectileDragScript = goProjectile.GetComponent<ProjectileDrag>();
		}

		if (!pdProjectileDragScript.isSpringEnabled())
		{
			fDifference = Vector3.Distance(goPlayer.transform.position, goProjectile.transform.position);

			fHeight = Mathf.Lerp(fHeight, fDifference, Time.deltaTime);

			cmThisCamera.transform.position.x.Equals(goProjectile.transform.position.x / 2);

			if (fDifference < 5)
			{ fDifference = 5; }

			cmThisCamera.orthographicSize = fDifference;

			cmThisCamera.transform.position.y.Equals(fHeight);

			goResetButton.SetActive(true);


		}
		else if (Input.touchCount == 2)
		{

			CameraZoom();

		}
		else
		{
			//ResetCamera();
		}

		

	}

	private void CameraZoom()
	{
		Touch firstTouch = Input.GetTouch(0);
		Touch secondTouch = Input.GetTouch(1);

		Vector2 firstTouchPrevPos = firstTouch.position - firstTouch.deltaPosition;
		Vector2 secondTouchPrevPos = secondTouch.position - secondTouch.deltaPosition;

		float prevTouchDeltaMag = (firstTouchPrevPos - secondTouchPrevPos).magnitude;
		float touchDeltaMag = (firstTouch.position - secondTouch.position).magnitude;

		float deltaMagnitudeDiff = prevTouchDeltaMag - touchDeltaMag;

		cmThisCamera.orthographicSize += deltaMagnitudeDiff * fZoomSpeed;

		cmThisCamera.orthographicSize = Mathf.Max(cmThisCamera.orthographicSize, 1.0f);

		goResetButton.SetActive(true);
	}

	public void ResetCamera()
	{
		cmThisCamera.orthographicSize = 5;
		goResetButton.SetActive(false);


	}
}
