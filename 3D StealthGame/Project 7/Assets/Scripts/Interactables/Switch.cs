using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Switch : Interactable
{

    //bool for if light switch is switched on
    private bool m_bIsActive;

    //the AI light switch script
    private LightSwitch m_AILightScript;
    
    public GameObject LightObject;


    public void Start()
    {
        m_AILightScript = GetComponent<LightSwitch>();

        LightObject = m_AILightScript.LightObject;

        m_AILightScript.LightReset();

    }

    public override void Interact()
    {

        m_AILightScript.ToggleActive();

        LightObject.SetActive(!LightObject.activeSelf);

    }

}
