using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StairLever : Interactable
{
    //Has lever already been pulled?
    private bool m_bIsActive = false;

    //Animations for what the lever does
    [SerializeField] private GameObject m_goLeverFunctionAni;
    [SerializeField] private GameObject m_goLeverAni;
    [SerializeField] private GameObject m_goBlockCube;

    public override void Interact()
    {
        if (!m_bIsActive)
        {
            m_bIsActive = true;

            m_goBlockCube.SetActive(false);
            m_goLeverAni.GetComponent<Animator>().SetTrigger("Pulled");
            m_goLeverFunctionAni.GetComponent<Animator>().SetTrigger("Pulled");
        }
    }
}
