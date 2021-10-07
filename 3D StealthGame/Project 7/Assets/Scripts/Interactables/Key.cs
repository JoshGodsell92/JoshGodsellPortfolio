using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Key : Interactable
{
    [SerializeField] private GameObject goKey;
    [SerializeField] private bool m_bBigKey;
    private bool m_bIsTaken = false;

    public override void Interact()
    {
        LevelDetails levelDetails;

        levelDetails = GameObject.FindGameObjectWithTag("GameManager").GetComponent<LevelDetails>();

        if (m_bBigKey)
        {
            levelDetails.TakeBigKey();
        }
        else
        {
            levelDetails.TakeKey();
        }

        m_bIsTaken = true;
        goKey.SetActive(false);
    }

    private void Update()
    {
        if (m_bIsTaken)
        {
            goKey.SetActive(false);
        }
    }

}
