// Last edited by: KT 02/02/2018 16:37

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

[RequireComponent(typeof(LineRenderer))]

public class Movement : MonoBehaviour
{

    // players speed - JG
    public float fPlayerSpeed = 5;

    //booleans for if each button is down - JG
    //buttons are invisible on game view they are at the edges of the screen - JG
    private bool bIsLeftButtonDown = false;
    private bool bIsRightButtonDown = false;

    //Private variables for health, timer and score - KT
    private int iScale;

    public GameObject goProjectile;

    private Collider2D c2dCollider;
	private LineRenderer lrTrajLine;

    //A variable that will be used to check if it is this players turn - KT
    bool bAllowMove = true;

    private GameObject goGameController;
    private GameControl gcGameControlScript;
    

    void Start()
    {
        iScale = 1; //multiplier for making the ball bigger - SM

        goProjectile = transform.Find("Projectile").gameObject;

		c2dCollider = this.GetComponent<Collider2D>();
		lrTrajLine = this.GetComponent<LineRenderer>();

        goGameController = GameObject.FindWithTag("GameControl");
        gcGameControlScript = goGameController.GetComponent<GameControl>();
        gcGameControlScript.StartUI();

    }

    void Update()
    {
        if (iScale == 2)
        {
            goProjectile.transform.localScale = new Vector3(0.8f, 0.8f, 0.8f);
        }
        else
        {
            goProjectile.transform.localScale = new Vector3(0.4f, 0.4f, 0.4f);
        }
        //if left button is down move left - JG
        if (bIsLeftButtonDown == true)
        {
            playerLeft();
        }
        //if right button is down move right - JG
        if (bIsRightButtonDown == true)
        {
            playerRight();
        }

        //Decrease the timer and if the timer reaches 0 then end the turn - KT
        if (bAllowMove)
        {
			lrTrajLine.enabled = true;
        }
		else
		{
			lrTrajLine.enabled = false;
		}
        
    }

    //A function to move the player left - KT
    private void playerLeft()
    {
        //Check if the player can move - KT
        if (bAllowMove)
        {
            //multiply negative speed by time to get translation left - JG
            float translation = -fPlayerSpeed * Time.deltaTime;

            //move the player by the translation - JG
            gameObject.transform.Translate(translation, 0.0f, 0.0f);
        }

    }

    //A function to move the player right - KT
    private void playerRight()
    {
        //Check if the player can move - KT
        if (bAllowMove)
        {
            //multiply speed by time to get translation right - JG
            float translation = fPlayerSpeed * Time.deltaTime;

            //move the player by the translation - JG

            gameObject.transform.Translate(translation, 0.0f, 0.0f);
        }

    }

    //functions for the Event triggers to change the boolean values for when the buttons are held down - JG
    public void leftButtonDown()
    {
        bIsLeftButtonDown = true;
    }

    public void leftButtonUp()
    {
        bIsLeftButtonDown = false;
    }

    public void rightButtonDown()
    {
        bIsRightButtonDown = true;
    }

    public void rightButtonUp()
    {
        bIsRightButtonDown = false;
    }

    
    //A function for collisions - KT
    private void OnCollisionEnter2D(Collision2D other)
    {
        //Check if it is colliding with the player 2 projectile - KT
        if (other.gameObject.CompareTag("Projectile"))
        {
            Handheld.Vibrate();
        }
    }

    //A function to set the projectile Scale variable - SM
    public void SetScale(int a_iScale)
    {
        iScale = a_iScale;
    }

    //A function to get the projectile Scale variable - SM
    public int GetScale()
    {
        return iScale;
    }

    public bool getAllowMove()
    {
        return bAllowMove;
    }
}
