using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SmokeEffect : MonoBehaviour
{
    [SerializeField] float fSmokeTimer = 5f;
    private const float SMOKERADIUS = 7.5f;

    private List<GameObject> a_goAIObjects;

    // Start is called before the first frame update
    void Start()
    {
        a_goAIObjects = new List<GameObject>();
        foreach(GameObject go in GameObject.FindGameObjectsWithTag("AIAgent"))
        {
            a_goAIObjects.Add(go);
        }
    }

    // Update is called once per frame
    void Update()
    {
        foreach(GameObject go in a_goAIObjects)
        {
            if(Vector3.Distance(go.transform.position, transform.position) <  SMOKERADIUS)
            {
                go.transform.Find("VisionStart").GetComponent<VisionCone>().SetInSmoke(true);
            }
            else
            {
                go.transform.Find("VisionStart").GetComponent<VisionCone>().SetInSmoke(false);
            }
        }

        fSmokeTimer -= Time.deltaTime;
        if (fSmokeTimer <= 0.0f)
        {
            foreach(GameObject go in a_goAIObjects)
            {
                go.transform.Find("VisionStart").GetComponent<VisionCone>().SetInSmoke(false);
            }

            Destroy(gameObject);
        }
    }
}
