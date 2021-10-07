// Last Edited by: JG 29/01/2018 21:39

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class ProjectileDrag : MonoBehaviour {

    //maximum stretch of projectile from player - JG
    private float maxStretch = 2.0f;
    //maximum stretch squared for efficiency - JG
    private float maxStretchSqr;

    //this projectiles parent player object -JG
    private GameObject player;
	private Collider2D playercollider;
	private LineRenderer playerLine;

    // spring joint component and projectileBody2D component - JG
    public SpringJoint2D spring;
    private Rigidbody2D projectileBody;

    private Ray rayToPointer;

	private Camera mainCam;
	private CameraControl mainCamController;

    // bool to check if projectile is pressed on - JG
    public bool pressedOn = false;
    public bool inFan = false;

    //Velocity stuff for testing
    public float maxVelocity;
    public Vector3 predictedVelocity;
    public Vector3 playerToPointer;
    public float distanceAtRelease;

    //Audio sources for the ball bounce, fan and star sounds - SM
    public AudioSource Bounce;
    public AudioSource Fan;
    public AudioSource Star;

    private Vector3 fanUp;

    public bool firstHit = false;

    //projectiles previous velocity - JG
    private Vector2 prevVelocity;

    //point at which the projectile resets past the bottom edge - JG
    private GameObject vanishPoint;

    //Create a couple of game objects for the players - KT
    private GameObject _player;

    //Some variables that will be used to check turn and mode - KT
    bool bAllowFire = false;

    bool bFired = false;

    //A float that will be used to divide the velocity - KT
    float VelocityDivider = 0f;

    int iShotCounter = 0;

    private Text ShotCounter;

	private CircleCollider2D Collider;

    private GameObject GC;
    private GameControl GCScript;

    private GameObject gravzone;
    public PhysicsMaterial2D Ice;
    public PhysicsMaterial2D Standard;

    private GameObject TPEntrance;
    private GameObject TPExit;

	// Game object responcible for the particles - JG
	private ParticleSystem particleSystem;

    public bool bStart = true;
    public bool bFloatyBall = false;

	void Start ()
    {
		mainCam = GameObject.FindObjectOfType<Camera>();
		mainCamController = mainCam.GetComponent<CameraControl>();
        try
        {
            gravzone = GameObject.FindGameObjectWithTag("AntiGrav");
        }
        catch
        {

        }

        //calculates the sqr magnitude - JG
        maxStretchSqr = maxStretch * maxStretch;

        //finds the player object its collider and line renderer component - JG
        player = this.transform.parent.gameObject;
		playercollider = player.GetComponent<Collider2D>();
		playerLine = player.GetComponent<LineRenderer>();

		// if this is the player one object then set allow fire to true and start
		if(player.tag == "Player")
		{
			bAllowFire = true;
		}

        //finds the spring joint and projectileBody - JG
        spring = GetComponent<SpringJoint2D>();
        projectileBody = GetComponent<Rigidbody2D>();

		Collider = GetComponent<CircleCollider2D>();

        // sets the projectileBody to kinematic - JG
        projectileBody.isKinematic = true;

        //FindSceneObjectsOfType the vanishPoint object - JG
        vanishPoint = GameObject.FindGameObjectWithTag("vanishPoint");
        
        VelocityDivider = 2.0f;

        GameObject TextGameObject = GameObject.FindWithTag("ShotCounter");
        ShotCounter = TextGameObject.GetComponent<Text>();
        ShotCounter.text = iShotCounter.ToString();

        GC = GameObject.FindWithTag("GameControl");
        GCScript = GC.GetComponent<GameControl>();

		particleSystem = GetComponentInChildren<ParticleSystem>();
		particleSystem.Clear();
		particleSystem.Pause();

        GC = GameObject.FindWithTag("GameControl");
        GCScript = GC.GetComponent<GameControl>();
        GCScript.StartUI();

        this.gameObject.GetComponent<SpriteRenderer>().sprite = GCScript.GetCorrectBallSprite();
        this.gameObject.GetComponent<Rigidbody2D>().sharedMaterial = GCScript.GetCorrectBall();
        this.gameObject.GetComponent<CircleCollider2D>().sharedMaterial = GCScript.GetCorrectBall();
    }

	void Update ()
    {
        if (GCScript.GetBallTypeName() == "FloatyBall" && bStart)
        {
            Physics2D.gravity = new Vector2(0, -4.4f);
        }
        else if(bStart)
        {
            Physics2D.gravity = new Vector2(0, -9.8f);

        }

        if (Input.anyKey && bStart)
        {
            //bStart = false;
        }
    
        if(inFan && !(projectileBody.velocity.y > 10f))
        {
            projectileBody.AddForce(fanUp * 250);
        }
        if (bFired && Input.anyKey)
        {
            Reset();
        }

        //if the projectile has dropped below the vanishPoint reset it - JG
        if (this.transform.position.y < vanishPoint.transform.position.y)
        {
            Reset();
        }

        //draws a ray starting from the players position going no where- JG
        rayToPointer = new Ray(player.transform.position, Vector3.zero);

		if(!bAllowFire)
		{
			Collider.radius = 0.1f;
		}

		//if pressed on then projectile is being dragged - JG
		if (pressedOn)
        {
            Dragging();
            playerToPointer = projectileBody.transform.position - player.transform.position;
            distanceAtRelease = playerToPointer.magnitude;
            predictedVelocity = -playerToPointer.normalized * (maxVelocity * distanceAtRelease);
        }
        else if (!pressedOn && bAllowFire && !bFired)
        {
			playerLine.enabled = true;
			playercollider.enabled = false;
            Collider.radius = 0.6f;
        }
        else if (!pressedOn && bAllowFire && bFired)
        {
			playercollider.enabled = false;
            Collider.radius = 0.1f;
        }

        //if the spring is enabled - JG
        if (spring.enabled == true)
        {
            


            //and if the rigid body is not kinematic and the previous frame velocity is greater than the current velocity
            // and therefore just past the spring joint point connected to the player then disable the spring
            //and set the velocity of the projectile to match its peak as it was passing the spring joint - JG
            if (!projectileBody.isKinematic && prevVelocity.sqrMagnitude > projectileBody.velocity.sqrMagnitude)
            {
                spring.enabled = false;
				particleSystem.Play();


                Vector3 vel = -playerToPointer.normalized * (maxVelocity * distanceAtRelease);
                projectileBody.velocity = vel;
            }

            // if projectile is not being pressed on take its velocity every frame until the spring is disabled - JG
            if (!pressedOn)
            {
                prevVelocity = projectileBody.velocity;
            }
        }
    }

    private void OnMouseDown()
    {
        if (bAllowFire && !bStart)
        {
           
            //enables the spring joint when pressed and sets pressedOn to true - JG
            spring.enabled = true;
            pressedOn = true;
        }      
    }


    private void OnMouseUp()
    {
        if (bAllowFire && !bStart && pressedOn)
        {
            // the spring joint when not pressed and sets the projectileBody to dynamic so physics now aplies also sets pressedOn to false - JG
            spring.enabled = true;
            projectileBody.isKinematic = false;
            pressedOn = false;
            bFired = true;
            GCScript.AddShotCounter(1);
        }
    }

    private void Dragging()
    {
        if (bAllowFire)
        {
            //retrieves the pointers position in world space - JG
            Vector3 pointerWorldPoint = Camera.main.ScreenToWorldPoint(Input.mousePosition);

            //vector between player and projectile positions - JG
            Vector2 playerToPointer = pointerWorldPoint - player.transform.position;

            //if we are not within max stretch from the original position - JG
            if (playerToPointer.sqrMagnitude > maxStretchSqr)
            {
                //get a ray in that direction from the player - JG
                rayToPointer.direction = playerToPointer;

                //find the point at maxstretch along that ray and use that for our projectiles position - JG
                pointerWorldPoint = rayToPointer.GetPoint(maxStretch);
            }

            //to keep in 2d - JG
            pointerWorldPoint.z = 0.0f;

            //moves projectile with the pointer - JG
            transform.position = pointerWorldPoint;
        }
    }

    public void Reset()
    {

		// resets the main camera - JG
		//mainCamController.ResetCamera();

        //resets the projectile to the players position - JG
        this.transform.position = player.transform.position;

        ObjectiveScript[] objectiveScripts = FindObjectsOfType<ObjectiveScript>();

        for (int i = 0; i < objectiveScripts.Length; i++)
        {
            if (objectiveScripts[i].GetObjectiveType() == ObjectiveScript.objectiveType.WallTarget)
            {
                objectiveScripts[i].setWallHit(false);

            }
        }

        //resets velocity to zero - JG
        projectileBody.velocity = Vector2.zero;

        //sets it to be kinematic - JG
        projectileBody.isKinematic = true;

		//re-enables the spring joint and the particle game object - JG
		spring.enabled = true;
		particleSystem.Clear();
		particleSystem.Pause();


        bAllowFire = true; //Make sure the projectile can not be fired - KT

        bFired = false;

        GC = GameObject.FindWithTag("GameControl");
        GCScript = GC.GetComponent<GameControl>();
        GCScript.SetCombo(0);
    }

	public bool isSpringEnabled()
	{
		return spring.enabled;
	}

    public void OnTriggerEnter2D(Collider2D collision)
    {
        //If the trigger is an anti gravity object, set the gravity to 0 - SM
        if (collision.gameObject.tag == "AntiGrav")
        {
            Physics2D.gravity = new Vector2(0, 0);
        }
        //if the trigger collider is tagged Fan then play the sound and move the ball upwards - SM
        if (collision.gameObject.tag == "Fan")
        {
            Fan.Play();
            fanUp = collision.gameObject.transform.up;
            inFan = true;
        }
        if (collision.gameObject.tag == "Entrance")
        {
            TPEntrance = GameObject.FindGameObjectWithTag("Entrance");
            TPExit = GameObject.FindGameObjectWithTag("Exit");
            this.gameObject.transform.position = TPExit.transform.position;
            Vector3 EntranceVelocity = projectileBody.velocity;
            float rotationZ = (TPEntrance.transform.eulerAngles.z - TPExit.transform.eulerAngles.z);
            EntranceVelocity = Quaternion.Euler(0, 0, -rotationZ) * EntranceVelocity;
            projectileBody.velocity = EntranceVelocity;
        }
        //If the ball enters the waterfall object then half it's velocity - SM
        if (collision.gameObject.tag == "Waterfall")
        {
            projectileBody.velocity = (projectileBody.velocity / 2);
        }
        if (collision.gameObject.tag == "Star")
        {
            Star.Play();
            GC = GameObject.FindGameObjectWithTag("GameControl");
            GCScript = GC.GetComponent<GameControl>();
            Scene CurrentScene = SceneManager.GetActiveScene();
            int SceneNumber = int.Parse(CurrentScene.name);
            //GCScript.UpdateObjective(SceneNumber, 1, true);

            ObjectiveScript[] objectiveScripts = FindObjectsOfType<ObjectiveScript>();

            for(int i = 0; i < objectiveScripts.Length; i++)
            {
                if (objectiveScripts[i].GetObjectiveType() == ObjectiveScript.objectiveType.StarObject)
                {
                    objectiveScripts[i].setStarCollected(true);
                    objectiveScripts[i].testObjective();
                    GCScript.Save();
                }
            }

            DestroyObject(collision.gameObject);
        }

    }

    public void OnTriggerExit2D(Collider2D collision)
    {
        //reset the gravity once you leave a gravity zone - SM
        if (collision.gameObject.tag == "AntiGrav")
        {
            if (GCScript.GetBallTypeName() == "FloatyBall")
            {
                Physics2D.gravity = new Vector2(0, -4.4f);
            }
            else
            {
                Physics2D.gravity = new Vector2(0f, -9.81f);
            }
        }
        if (collision.gameObject.tag == "Fan")
        {
            inFan = false;
        }
        if (collision.gameObject.tag == "Ice")
        {
            projectileBody.sharedMaterial = Standard;
        }
    }

    public void OnTriggerStay2D(Collider2D collision)
    {
        if (collision.gameObject.tag == "Ice")
        {
            projectileBody.sharedMaterial = Ice;
        }
    }

    public void OnCollisionEnter2D(Collision2D collision)
    {
        //if the targetwall is hit check if there is an objective with target wall as type if there is set its completed to be true - JG
        if (collision.gameObject.tag == "TargetWall")
        {
            ObjectiveScript[] objectiveScripts = FindObjectsOfType<ObjectiveScript>();

            for(int i = 0; i < objectiveScripts.Length; i++)
            {
                if(objectiveScripts[i].GetObjectiveType() == ObjectiveScript.objectiveType.WallTarget)
                {
                    objectiveScripts[i].setWallHit(true);
                }
            }
        }

        
        Bounce.Play();
    }

}
