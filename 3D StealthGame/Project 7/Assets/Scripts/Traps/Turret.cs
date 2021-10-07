using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class Turret : Interactable
{
    [SerializeField] private bool bActive;
    [SerializeField] private float fDetectRadius;
    [SerializeField] private float fDetectAngle;

    [SerializeField] bool bAlerted; //replace with enum?
    [SerializeField] float fFireDelay = 1f; //time in seconds the turret has to wait before firing.
    [SerializeField] float fEffectDespawnTime = 0.1f;
    [SerializeField] float fReloadTime = 5f;
    [SerializeField] GameObject goFirePoint; //position where the gun fires from
    [SerializeField] bool bShooting;

    [SerializeField] CylinderLine cylinderLine;
    [SerializeField] Material cylinderLineMaterial;

    // Start is called before the first frame update
    void Start()
    {
        cylinderLine = GetComponent<CylinderLine>();
        cylinderLine.material = cylinderLineMaterial;
    }

    // Update is called once per frame
    void Update()
    {
        if (bActive)
        {
            Collider[] colliders = Physics.OverlapSphere(transform.position, fDetectRadius);
            for (int i = 0; i < colliders.Length; i++)
            {
                PlayerController player = colliders[i].GetComponentInParent<PlayerController>();
                if (player != null && !bShooting)
                {
                    StartCoroutine(ShootAtPlayer(player));
                }
                else
                {
                    //player is not visible
                    //if alerted already, stop being alerted
                    if (bAlerted)
                    {
                        bAlerted = false;
                    }
                }
            }
        }
    }

    public override void Interact()
    {
        //if the player interacts with this, they disable it.
        bActive = false;
    }

    public override void AIInteract(AI_Agent a_Agent)
    {
        SetActive(true);
    }

    IEnumerator ShootAtPlayer(PlayerController player)
    {
        bShooting = true;
        //frustrum cull
        if (RaycastToPlayer(player))
        {
            //player is visible
            //be alerted
            bAlerted = true;
            //wait
            yield return new WaitForSeconds(fFireDelay);
            //fire at them
            RaycastHit hit;
            Vector3 firePos;
            if (Physics.Raycast(goFirePoint.transform.position, gameObject.transform.forward, out hit, fDetectRadius))
            {
                firePos = hit.point;
            }
            else
            {
                firePos = goFirePoint.transform.position + fDetectRadius * gameObject.transform.forward;
            }
            Vector3[] positions = { goFirePoint.transform.position, firePos };
            cylinderLine.enabled = true;
            cylinderLine.SetPositions(positions);
            //raycast in front of the gun to make sure the player is hit (if they are moving fast enough maybe they avoid getting hit)
            LayerMask mask = LayerMask.GetMask("Player");
            if (Physics.Raycast(goFirePoint.transform.position, gameObject.transform.forward, out hit, fDetectRadius, mask))
            {
                //shoot
                if (hit.collider.gameObject == player.gameObject)
                {
                    player.TakeDamage();
                    Debug.Log("Shot Player");
                    yield return new WaitForSeconds(fEffectDespawnTime);
                    cylinderLine.enabled = false;
                    yield return new WaitForSeconds(fReloadTime - fEffectDespawnTime);
                    bShooting = false;
                }
                else
                {
                    Debug.Log("Missed Player");
                    yield return new WaitForSeconds(fEffectDespawnTime);
                    cylinderLine.enabled = false;
                    yield return new WaitForSeconds(fReloadTime - fEffectDespawnTime);
                    bShooting = false;
                }
            }
            else
            {
                Debug.Log("Missed Player");
                yield return new WaitForSeconds(fEffectDespawnTime);
                cylinderLine.enabled = false;
                yield return new WaitForSeconds(fReloadTime - fEffectDespawnTime);
                bShooting = false;
            }
        }
        else
        {
            //player is not visible
            //if alerted already, stop being alerted
            if (bAlerted)
            {
                bAlerted = false;
            }
            bShooting = false;
        }
    }

    bool RaycastToPlayer(PlayerController player)
    {
        Vector3 direction = player.transform.position - transform.position;

        direction.Normalize();
        float angle = Vector3.Angle(transform.forward, direction);

        if (angle < fDetectAngle)
        {
            Vector3 origin = transform.position;

            Debug.DrawRay(origin, direction * 20, Color.red);

            Ray InteractRay = new Ray(origin, direction);

            LayerMask mask = LayerMask.GetMask("Player");

            RaycastHit hit;

            if (Physics.Raycast(InteractRay, out hit, 20, mask, QueryTriggerInteraction.Collide))
            {
                return true;
            }
            else
            {
                return false;
            }
        }
        else
        {
            return false;
        }
    }

    public bool GetActive()
    {
        return bActive;
    }

    public void SetActive(bool active)
    {
        bActive = active;
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(gameObject.transform.position, fDetectRadius);
        Gizmos.DrawLine(gameObject.transform.position, gameObject.transform.position + fDetectRadius * Mathf.Sin(fDetectAngle * Mathf.Deg2Rad) * gameObject.transform.forward + (fDetectRadius * Mathf.Cos(fDetectAngle * Mathf.Deg2Rad) * gameObject.transform.right));
        Gizmos.DrawLine(gameObject.transform.position, gameObject.transform.position + fDetectRadius * Mathf.Sin(fDetectAngle * Mathf.Deg2Rad) * gameObject.transform.forward + (fDetectRadius * Mathf.Cos(fDetectAngle * Mathf.Deg2Rad) * gameObject.transform.right * -1f));
        Gizmos.color = Color.blue;
        Gizmos.DrawLine(transform.position, transform.position + transform.forward * 2);
    }

}
