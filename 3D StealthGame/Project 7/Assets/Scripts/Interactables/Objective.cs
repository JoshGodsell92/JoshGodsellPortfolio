using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Objective : Interactable
{
    [SerializeField] Inventory inventory; //player inventory

    public override void Interact()
    {
        if(!inventory.GetHasObjective())
        {
            inventory.SetHasObjective(true);
            gameObject.SetActive(false);
        }
    }

    // Start is called before the first frame update
    void Start()
    {
        if (inventory == null)
        {
            inventory = FindObjectOfType<Inventory>();
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
