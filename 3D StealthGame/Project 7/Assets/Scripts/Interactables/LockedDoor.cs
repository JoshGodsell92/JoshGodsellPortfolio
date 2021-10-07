using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LockedDoor : Interactable
{
    [SerializeField] private GameObject goDoor;
    private bool m_bIsOpen = false;

    public override void Interact()
    {
        LevelDetails levelDetails = GameObject.FindGameObjectWithTag("GameManager").GetComponent<LevelDetails>();

        if (levelDetails.GetKey() > 0)
        {
            m_bIsOpen = true;
            goDoor.SetActive(false);
            levelDetails.TakeKey();
            levelDetails.OpenDoor();
        }

    }

    private void Update()
    {
        if(m_bIsOpen)
        {
            goDoor.SetActive(false);
        }
    }

}
