/////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
///Name: BouncePad.cs 
///Created by: Charlie Bullock
///Description: Simple script for pushing force on player when they collider with this objects collider.  
/////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BouncePad : MonoBehaviour {
    //Variables
    #region Variables
    [SerializeField]
    private float fForce;
    [SerializeField]
    enum Direction
    {
        UP,
        DOWNWARD,
        LEFT,
        RIGHT
    }
    [SerializeField]
    Direction DirectionType;
    #endregion Variables

    //Function for checking if a player is touching this objects collider and if so applying 
    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.tag == "Player")
        {
            switch ((int)DirectionType)
            {
                //Force upward
                case (int)Direction.UP:
                collision.gameObject.GetComponent<Rigidbody2D>().AddForce(transform.up * fForce,ForceMode2D.Impulse);
                    break;
                //Force downward
                case (int)Direction.DOWNWARD:
                    collision.gameObject.GetComponent<Rigidbody2D>().AddForce(-transform.up * (fForce * 100), ForceMode2D.Impulse);
                    break;
                //Force left
                case (int)Direction.LEFT:
                    collision.gameObject.GetComponent<Rigidbody2D>().AddForce(-transform.right * fForce, ForceMode2D.Impulse);
                    break;
                //Force right
                case (int)Direction.RIGHT:
                    collision.gameObject.GetComponent<Rigidbody2D>().AddForce(transform.right * fForce, ForceMode2D.Impulse);
                    break;
                default:
                    break;
            }
        }
    }
}
