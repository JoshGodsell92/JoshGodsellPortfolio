using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DetectPlayer : MonoBehaviour {

    StateBasedAi m_AIScript;

	// Use this for initialization
	void Start ()
    {
        m_AIScript = this.gameObject.GetComponentInParent<StateBasedAi>();
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.gameObject.tag == "Player")
        {
            m_AIScript.bPatrolPoints = false;
        }
    }
}
