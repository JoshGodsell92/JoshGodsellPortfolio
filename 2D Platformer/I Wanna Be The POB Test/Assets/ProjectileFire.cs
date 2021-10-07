/////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
///Name: ProjectileFire.cs 
///Created by: Charlie Bullock
///Description: This script is attached to enemies projectiles and destroys these projectiles after a set amount of time,
///additionally this script will check the trigger collider to see if this projectile hit the player
/////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ProjectileFire : MonoBehaviour {
    [SerializeField]
    private float fProjectileSpeed;

	void Start () {
        GameObject player = GameObject.FindGameObjectWithTag("Player");
        Rigidbody2D rb = this.gameObject.GetComponent<Rigidbody2D>();
        rb.velocity = (player.transform.position - this.gameObject.transform.position).normalized * fProjectileSpeed * Time.deltaTime;
        Invoke("SelfDestructor", 2);
    }

    //This method destroys this projectile instance when called
    void SelfDestructor()
    {
        StateBasedAi.bProjectileLaunched = false;
        Destroy(this.gameObject);
    }
    //Function to check if player has been hit by projectile and when this happens simply set this projectile mesh renderer false as it will be destroyed regardless in 2 seconds
    private void OnTriggerEnter2D(Collider2D collision)
    {
         if (collision.gameObject.tag == "Player")
         {
            //Deduct player health
            MeshRenderer mr = this.gameObject.GetComponent<MeshRenderer>();
            mr.enabled = false;
         }
    }
}
