using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Collision : MonoBehaviour {

    //Private instances for GameControl Prefab, GameControl script, Player prefab and projectile script. - KT
    private GameObject goGameController;
    private GameControl gcGameControlScript;
    private GameObject goPlayer;
    private ProjectileDrag pdProjectileDragScript;

    //The amount of points you get for hitting the wall. - KT
    [SerializeField]
    private int iAmountOfPoints = 1;

	// Use this for initialization
	void Start () {

        //Initiate the instances - KT
        goGameController = GameObject.FindWithTag("GameControl");
        gcGameControlScript = goGameController.GetComponent<GameControl>();
        goPlayer = GameObject.FindWithTag("Player");
        pdProjectileDragScript = goPlayer.GetComponentInChildren<ProjectileDrag>();
	}

    //A function for collision - KT
    void OnCollisionEnter2D(Collision2D other)
    {
        //Check if it is a collision with the player projectile - KT
        if (other.gameObject.CompareTag("Projectile"))
        {
            //If there is a collision then the combo will increase. - KT
            goGameController = GameObject.FindWithTag("GameControl");
            gcGameControlScript = goGameController.GetComponent<GameControl>();
            gcGameControlScript.IncreaseCombo(iAmountOfPoints);            
        }
    }
}
