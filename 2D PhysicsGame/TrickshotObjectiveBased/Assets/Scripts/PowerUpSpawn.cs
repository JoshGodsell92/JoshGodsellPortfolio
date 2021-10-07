using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PowerUpSpawn : MonoBehaviour
{
    public float minX = -8f;            //Minimum X value - SM
    public float maxX = 8f;            //Maximum X value - SM
    public float minY = -4f;           //Minimum Y value - SM
    public float maxY = 4f;            //Maximum Y value - SM
    public GameObject PowerUp;          //powerup object - SM
    private GameObject powerUpInstance; // instance of the game object - JG
    private PowerUp script;             // reference to the power up script - SM
    public bool powerUpDestroyed = false;
    Vector2 spawnPosition;
    private Vector2[] positions;
    public GameObject[] walls;
    private Collider2D wallCollider;

    void Start()
    {
        //set the wall array to each of the game objects with the wall tag - SM
        walls = GameObject.FindGameObjectsWithTag("Wall");

        positions = new Vector2[walls.Length];

        //for the number of walls in the level, set the walls positions into vector2 positions - SM
        for (int i = 0; i < walls.Length; i++)
        {
            Vector2 position = walls[i].transform.position;
            positions[i] = position;
            Debug.Log(positions[i]);
        }

        //Getting the script componant from the power up object = SM
        script = PowerUp.GetComponent<PowerUp>();
        // Generate a random number between the minimum and maximum X and Y values - SM
        float randomX = Random.Range(minX, maxX);
        float randomY = Random.Range(minY, maxY);
        //setup our spawn position vector
        spawnPosition = Vector2.zero;

        //assign the spawn position values as the chosen random values - SM
        spawnPosition.x = randomX;
        spawnPosition.y = randomY;

        //set the gameobject to be a copy of the prefab gameobject - JG
        powerUpInstance = Instantiate(PowerUp, spawnPosition, Quaternion.identity);
        powerUpInstance.name = "newPowerUp";
        //goes through the walls one by one and checks them against the power ups position - SM & JG
        for (int i = 0; i < walls.Length; i++)
        {
            wallCollider = walls[i].GetComponent<Collider2D>();
            //uses the bounds to check if the power up game object is colliding with it - JG
            if (wallCollider.bounds.Contains(powerUpInstance.transform.position))
            {
                DestroyObject(powerUpInstance);
                powerUpDestroyed = true;
            }
        }


    }

    void Update()
    {
        if (powerUpDestroyed)
        {
            // Generate a random number between the minimum and maximum X and Y values - SM
            float randomX = Random.Range(minX, maxX);
            float randomY = Random.Range(minY, maxY);

            //assign the spawn position values as the chosen random values - SM
            spawnPosition.x = randomX;
            spawnPosition.y = randomY;
            //setup the powerup in the scene at the spawnPosition's new location - SM
            powerUpInstance = Instantiate(PowerUp, spawnPosition, Quaternion.identity);
            powerUpInstance.name = "newPowerUp";
            powerUpDestroyed = false;
        }
        for (int i = 0; i < walls.Length; i++)
        {
            wallCollider = walls[i].GetComponent<Collider2D>();

            if (wallCollider.bounds.Contains(powerUpInstance.transform.position))
            {
                DestroyObject(powerUpInstance);
                powerUpDestroyed = true;
            }
        }
    }
}