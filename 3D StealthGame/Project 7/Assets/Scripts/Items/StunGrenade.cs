using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StunGrenade : MonoBehaviour
{
    [SerializeField] float fStunRadius;
    [SerializeField] int iEnemyLayer;

    //The max distance an AI has to be to prompt a reaction to the mine
    private const float MAXDISTANCEFROMMINE = 5.0f;

    //How close the AI has to get for the mine to blow
    private const float EXPLODEDISTANCCE = 1.0f;

    //Has the mine hit a surface and become primed
    bool bPrimed = false;

    //Layermasks for AI and AI+PLayer Layers
    LayerMask AILayer;

    // Start is called before the first frame update
    void Start()
    {
        AILayer = LayerMask.GetMask("AI");
    }

    // Update is called once per frame
    void Update()
    {
        //if (bPrimed)
        //{
        //    foreach (Collider col in Physics.OverlapSphere(transform.position, MAXDISTANCEFROMMINE, AILayer))
        //    {
        //        if (Vector3.Distance(transform.position, col.transform.position) < EXPLODEDISTANCCE)
        //        {
        //            Collider[] colliders = Physics.OverlapSphere(gameObject.transform.position, fStunRadius, AILayer);
        //            foreach (Collider cols in colliders)
        //            {
        //                if (cols.GetComponent<AI_Agent>())
        //                {
        //                    //stun
        //                    cols.GetComponent<AI_Agent>().SetState(AI_Agent.AGENT_STATE.STUNNED);

        //                    //do flashy things
        //                    Destroy(gameObject);
        //                }
        //            }
        //        }
        //    }

        //}
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
        if(collision.gameObject.tag == "AIAgent")
        {
            collision.gameObject.GetComponent<AI_Agent>().SetState(AI_Agent.AGENT_STATE.STUNNED);
            Destroy(this.gameObject);
        }
        else
        {
            Destroy(this.gameObject);
            //bPrimed = true;
            //this.GetComponent<Rigidbody>().isKinematic = true;
            //this.GetComponent<Rigidbody>().useGravity = false;
        }

    }
}
