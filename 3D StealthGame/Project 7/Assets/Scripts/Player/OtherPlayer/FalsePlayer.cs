using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FalsePlayer : MonoBehaviour
{
    //The distance the player object will look for lights
    const float LIGHTSEARCHDISTANCE = 5.0f;

    //A bool to simulate the player being in darkness
    [SerializeField]
    private bool bInDark = false;

    //Layer mask for the Lights layer
    LayerMask LightMask;
    //The getter for darkness
    public bool GetInDarkness()
    {
        return bInDark;
    }

    public void SetInDarkness(bool a_bIsDark)
    {
        bInDark = a_bIsDark;
    }

    // Start is called before the first frame update
    void Awake()
    {
        LightMask = LayerMask.GetMask("Lights");
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void FixedUpdate()
    {
        //Bool for if the player is in light
        bool bIsInLight = false;

        foreach (Collider col in Physics.OverlapSphere(transform.position, LIGHTSEARCHDISTANCE, LightMask))
        {
            if(col.gameObject.GetComponent<LightingOverPlayer>() != null)
            {
                if(col.gameObject.GetComponent<LightingOverPlayer>().RunLighting(this.gameObject))
                {
                    bIsInLight = true;
                }
            }
        }

        if(bIsInLight)
        {
            bInDark = false;
            //Debug.Log("Lit");
        }
        else
        {
            bInDark = true;
        }


    }

}
