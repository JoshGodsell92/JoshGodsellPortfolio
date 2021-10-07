using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SmokeGrenade : MonoBehaviour
{
    [SerializeField] GameObject goSmokePrefab;

    //Can the Player also trigger the mine
    [SerializeField]
    private bool PlayerTriggerMine;

    //The max distance an AI has to be to prompt a reaction to the mine
    private const float MAXDISTANCEFROMMINE = 5.0f;

    //How close the AI has to get for the mine to blow
    private const float EXPLODEDISTANCCE = 2.0f;

    //Has the mine hit a surface and become primed
    bool bPrimed = false;

    //Layermasks for AI and AI+PLayer Layers
    LayerMask AILayer;
    LayerMask PlayerAILayer;

    // Start is called before the first frame update
    void Start()
    {
        AILayer = LayerMask.GetMask("AI");
        PlayerAILayer = LayerMask.GetMask("AI") | LayerMask.GetMask("Player");
    }

    // Update is called once per frame
    void Update()
    {
        //if(gameObject.transform.position.y < 0.0f)
        //{
        //    gameObject.transform.position = new Vector3(gameObject.transform.position.x, 0.01f, gameObject.transform.position.z);
        //}
        if (bPrimed)
        {
            if (PlayerTriggerMine)
            {
                foreach (Collider col in Physics.OverlapSphere(transform.position, MAXDISTANCEFROMMINE, PlayerAILayer))
                {
                    if(Vector3.Distance(transform.position, col.transform.position) < EXPLODEDISTANCCE)
                    {
                        GameObject.Instantiate(goSmokePrefab, gameObject.transform.position, Quaternion.identity);
                        Destroy(gameObject);
                    }
                }
            }
            else
            {
                foreach (Collider col in Physics.OverlapSphere(transform.position, MAXDISTANCEFROMMINE, AILayer))
                {
                    if (Vector3.Distance(transform.position, col.transform.position) < EXPLODEDISTANCCE)
                    {
                        GameObject.Instantiate(goSmokePrefab, gameObject.transform.position, Quaternion.identity);
                        Destroy(gameObject);
                    }
                }
            }
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.tag == "GrenadeNet")
        {
            gameObject.transform.position = new Vector3(gameObject.transform.position.x, other.gameObject.transform.position.y + (other.gameObject.transform.localScale.y / 2) + 0.01f, gameObject.transform.position.z);
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Wood") || collision.gameObject.CompareTag("Metal") || collision.gameObject.CompareTag("Carpet"));
        {
            bPrimed = true;
            this.GetComponent<Rigidbody>().isKinematic = true;
            this.GetComponent<Rigidbody>().useGravity = false;
            gameObject.transform.rotation = new Quaternion(0, 0, 0, 0);

        }
    }
}
