///////////////////////////////////////////////////////////////////////////////////////////
// File Name: CollisionBroadcast.cs                                
// Author: Josh Godsell                                    
// Date Created: 25/4/19                                   
// Date Last Edited: 26/4/19                               
// Brief:class for passsing collision data to no monobehaviour script                    
///////////////////////////////////////////////////////////////////////////////////////////
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CollisionBroadcast : MonoBehaviour {

    //boss controller and projectile
    private BossController m_BossController;

    private BossProjectile m_ThisProjectile;

    //game manager
    private GameManager m_GM;


    virtual public void Awake()
    {
        //assign the boss controller and game manager
        try
        {
            m_GM = FindObjectOfType<GameManager>();
            m_BossController = FindObjectOfType<BossController>();
        }
        catch (System.Exception)
        {

            throw new System.Exception("Boss controller not found");
        }


    }

    //on a collision get the associated projectile script and call the collision detected function on the script passing the collider through
    virtual public void OnCollisionEnter2D(Collision2D collision)
    {
        GetProjectileAssociated();

        m_ThisProjectile.CollisionDetected(collision);
    }

    //function to get the associated projectile
    virtual public void GetProjectileAssociated()
    {
        //if level 1
        if (m_GM.GetCurrentLevel().GetLevelName() == "Level 1")
        {
            //get the boss controller as a boss_one script
            Boss_One t_Boss = (Boss_One)m_BossController;

            //get the projectile script list and find the asociated script from this object
            BossProjectile[] t_projectiles = t_Boss.GetProjectiles();

            foreach (BossProjectile projectileScript in t_projectiles)
            {
                if (projectileScript.GetInstance() == this.gameObject)
                {
                    m_ThisProjectile = projectileScript;
                }
            }
        }
    }

}
