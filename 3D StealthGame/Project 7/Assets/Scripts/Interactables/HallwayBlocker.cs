using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HallwayBlocker : Interactable
{
    [SerializeField] private GameObject m_goBlockage;

    [SerializeField] private bool m_bBlocked;

    private PlayerController m_goPlayerCon;
    private Inventory inventory;

    public override void Start()
    {
        base.Start();
        m_goPlayerCon = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerController>();
        inventory = GameObject.FindGameObjectWithTag("Player").GetComponent<Inventory>();
    }

    public override void Interact()
    {
        if (!m_bBlocked && inventory.GetRopeStock() > 0)
        {
            m_bBlocked = true;
            m_goBlockage.SetActive(true);
            inventory.SetRopeStock(inventory.GetRopeStock() - 1);
        }
        else if(m_bBlocked)
        {
            m_bBlocked = false;
            m_goBlockage.SetActive(false);
            inventory.SetRopeStock(inventory.GetRopeStock() + 1);
        }
    }
}
