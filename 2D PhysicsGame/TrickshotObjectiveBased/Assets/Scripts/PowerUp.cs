using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PowerUp : MonoBehaviour
{
    public bool isHealth = false;  //a bool for if the powerup is a healthpack - SM
    public bool isDamage = false;
    public bool isBigger = false;

    public bool isDoubleCombo = false;
    public bool isExtraLife = false;

    public int damage;

    private GameObject Spawner;             //the PowerUpSpawner game object - SM
    private PowerUpSpawn spawnScript;       //the script form the PowerUp Spawner object - SM

    private GameObject GameControl;             //the PowerUpSpawner game object - SM
    private GameControl GameControlScript;       //the script form the PowerUp Spawner object - SM

    private GameObject thisObject;          
    private CircleCollider2D thiscollider;

    private int iRandomSeed = 0;

    void Start()
    {
        damage = 50;
        thisObject = this.gameObject; //sets the gameobject - SM 
        Spawner = GameObject.Find("PowerUp Spawner"); //assign the spawner gameobject to a variable - SM       
        spawnScript = Spawner.GetComponent<PowerUpSpawn>(); //get the script from the spawner object - SM        
        thiscollider = thisObject.GetComponent<CircleCollider2D>(); //get the powerups collider - SM

        GameControl = GameObject.Find("GameControl");   
        GameControlScript = GameControl.GetComponent<GameControl>(); 
        

        iRandomSeed = Random.Range(0, 3);//the randomiser, the number needs to be increased as powerups are added - SM & JG
        if (iRandomSeed == 0)
        {
            isDoubleCombo = true;
            this.gameObject.GetComponent<SpriteRenderer>().material.color = Color.cyan;
        }
        else if (iRandomSeed == 1)
        {
            isExtraLife = true;
            this.gameObject.GetComponent<SpriteRenderer>().material.color = Color.green;
        }
        else if (iRandomSeed == 2)
        {
            isBigger = true;
            this.gameObject.GetComponent<SpriteRenderer>().material.color = Color.yellow;
        }


    }

    private void Update()
    {
        if (iRandomSeed == 0)
        {
            isHealth = true;
            this.gameObject.GetComponent<SpriteRenderer>().material.color = Color.cyan;
        }
        else if (iRandomSeed == 1)
        {
            isExtraLife = true;
            this.gameObject.GetComponent<SpriteRenderer>().material.color = Color.green;
        }
        else if (iRandomSeed == 2)
        {
            isBigger = true;
            this.gameObject.GetComponent<SpriteRenderer>().material.color = Color.yellow;
        }
    }

    void OnTriggerStay2D(Collider2D other)
    {
        //if the object colliding with is the player 1 projectile - SM
        if (other.gameObject.CompareTag("Projectile"))
        {
            //Checks if it is a healthpack - SM
            if (isDoubleCombo == true)
            {
                GameObject GameController = GameObject.FindGameObjectWithTag("GameControl");
                GameControl GC = GameControl.GetComponent<GameControl>();
                GC.bX2combo = true;
            }
            else if (isExtraLife == true)
            {
                GameObject GameController = GameObject.FindGameObjectWithTag("GameControl");
                GameControl GC = GameControl.GetComponent<GameControl>();
                GC.AddShotCounter(1);
            }
            else if (isBigger == true)
            {
                GameObject Player = GameObject.FindGameObjectWithTag("Player");
                Movement PM = Player.GetComponent<Movement>();
                PM.SetScale(2);

            }
            //Destroy the gameobject at the end of the collision - SM
            spawnScript.powerUpDestroyed = true;
            DestroyObject(this.gameObject);
        }
        //if the object colliding with is the player 2 projectile - SM
        else if (other.gameObject.CompareTag("Player2Projectile"))
        {
            //Checks if it is a healthpack
            if (isHealth == true)
            {
                GameObject GameController = GameObject.FindGameObjectWithTag("GameControl");
                GameControl GC = GameControl.GetComponent<GameControl>();
                GC.bX2combo = true;
            }
            else if (isExtraLife == true)
            {
                GameObject GameController = GameObject.FindGameObjectWithTag("GameControl");
                GameControl GC = GameControl.GetComponent<GameControl>();
                GC.AddShotCounter(2);
            }
            else if (isBigger == true)
            {
                GameObject Player = GameObject.FindGameObjectWithTag("Player2");
                Movement PM = Player.GetComponent<Movement>();
                PM.SetScale(2);

            }
            //Set the gameobject to not active - SM
            spawnScript.powerUpDestroyed = true;
            DestroyObject(this.gameObject);
        }
    }

    //getter for the collider for the spawn script - SM
    public CircleCollider2D GetCollider()
    {
        return thiscollider;
    }
}
