using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LockedBigDoor : Interactable
{
    [SerializeField] private GameObject goDoor;

    public override void Interact()
    {
        LevelDetails levelDetails = GameObject.FindGameObjectWithTag("GameManager").GetComponent<LevelDetails>();

        if (levelDetails.GetBigKey() > 0)
        {
            goDoor.SetActive(false);
            levelDetails.TakeBigKey();
            levelDetails.OpenDoor();
        }

    }
}
