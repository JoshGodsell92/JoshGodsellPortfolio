//////////////////////////////////////////////////////////////////
// File Name: SpikeSpawner.cs                                   //
// Author: Josh Godsell                                         //
// Date Created: 29/5/19                                        //
// Date Last Edited: 29/5/19                                    //
// Brief:Control for the spike spawner                          //
//////////////////////////////////////////////////////////////////

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpikeSpawner : MonoBehaviour {

    //the spike prefab
    public GameObject m_SpikePlatformPrefab;
    //the point to test y component against
    public GameObject m_DestroyPoint;

    //time between spawns
    public float m_fTimeGap;

	// Use this for initialization
	void Start ()
    {
         //start the spawn coroutine
        StartCoroutine(DropSpikes());
		
	}

    //function to spawn spike
    public void SpawnSpike()
    {
        //instatiate the spike
        GameObject t_Instance = Instantiate(m_SpikePlatformPrefab, this.transform);

        //pass the destroy point object along to the spike object
        t_Instance.GetComponent<FallingSpike>().m_DestroyPoint = m_DestroyPoint;

        //rename the object to platformHolder for use by the player controller
        t_Instance.name = "PlatformHolder";

    }

    //IEnumerator to call the spawn function at regular intervals
    public IEnumerator DropSpikes()
    {
        //while true spawn a spike then wait to spawn another.
        while(true)
        {
            SpawnSpike();

            yield return new WaitForSeconds(m_fTimeGap);
        }
        yield return null;
    }
}
