using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Supply : Interactable
{
    [SerializeField] Material mMedkitBox;
    [SerializeField] Material mSmokeBox;
    [SerializeField] Material mStunBox;
    [SerializeField] Material mDecoyBox;
    [Space]
    [SerializeField] Inventory inventory; //player inventory
    [SerializeField] Inventory.CURRENT_ITEM eItem;
    [SerializeField] int iRefillAmount;
    [Tooltip("Chance of a Medkit spawning (make sure all chances add up to less than 100)")]
    [Range(0, 100)]
    [SerializeField] int iMedkitChance;
    [Tooltip("Chance of a Smoke Grenade spawning (make sure all chances add up to less than 100)")]
    [Range(0, 100)]
    [SerializeField] int iSmokeChance;
    //unused stun grenade chance, since it'll just be the left over value of the other chances. can be used if a fourth item is added.
    //As of the addition of the decoy the leftover amount will be the decoy
    [Tooltip("Chance of a medkit spawning (make sure all chances add up to less than 100)")]
    [Range(0, 100)]
    [SerializeField] int iStunChance;

    [SerializeField] bool bTrapped;

    // Start is called before the first frame update
    void Start()
    {
        inventory = GameObject.FindGameObjectWithTag("Player").GetComponent<Inventory>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public override void Interact()
    {
        if (bTrapped)
        {
            //do things
            //make sound
            //stun player
        }
        else
        {
            switch (eItem)
            {
                case Inventory.CURRENT_ITEM.MEDKIT:
                    if (inventory.GetMedkitStock() + iRefillAmount < inventory.GetMedkitMax())
                    {
                        inventory.SetMedkitStock(inventory.GetMedkitStock() + iRefillAmount);
                    }
                    else
                    {
                        inventory.SetMedkitStock(inventory.GetMedkitMax());
                    }
                    break;
                case Inventory.CURRENT_ITEM.SMOKEGRENADE:
                    if (inventory.GetSmokeStock() + iRefillAmount < inventory.GetSmokeMax())
                    {
                        inventory.SetSmokeStock(inventory.GetSmokeStock() + iRefillAmount);
                    }
                    else
                    {
                        inventory.SetSmokeStock(inventory.GetSmokeMax());
                    }
                    break;
                case Inventory.CURRENT_ITEM.STUNGRENADE:
                    if (inventory.GetStunStock() + iRefillAmount < inventory.GetStunMax())
                    {
                        inventory.SetStunStock(inventory.GetStunStock() + iRefillAmount);
                    }
                    else
                    {
                        inventory.SetStunStock(inventory.GetStunMax());
                    }
                    break;

                case Inventory.CURRENT_ITEM.DECOY:
                    if (inventory.GetDecoyStock() + iRefillAmount < inventory.GetDecoyMax())
                    {
                        inventory.SetDecoyStock(inventory.GetDecoyStock() + iRefillAmount);
                    }
                    else
                    {
                        inventory.SetDecoyStock(inventory.GetDecoyMax());
                    }
                    break;

                default:
                    break;
            }
        }
    }

    public override void AIInteract(AI_Agent a_Agent)
    {
        base.AIInteract(a_Agent);
    }

    public void SetupSupply()
    {
        //could probably do with being tidied up a bit or something

        //check to make sure values are ok
        if (iMedkitChance + iSmokeChance + iStunChance > 100)
        {
            Debug.LogError("iMedkitChance and iSmokeChance add up to more than 100");
        }
        else
        {
            int random = Random.Range(0, 100);
            if (random <= iMedkitChance)
            {
                eItem = Inventory.CURRENT_ITEM.MEDKIT;

                MeshRenderer[] compositeRenderers = gameObject.GetComponentsInChildren<MeshRenderer>();

                foreach (MeshRenderer MR in compositeRenderers)
                {
                    MR.material = mMedkitBox;
                }

                //gameObject.GetComponent<MeshRenderer>().material = mMedkitBox;
            }
            else if (random <= (iSmokeChance + iMedkitChance) && random > iMedkitChance)
            {
                eItem = Inventory.CURRENT_ITEM.SMOKEGRENADE;

                MeshRenderer[] compositeRenderers = gameObject.GetComponentsInChildren<MeshRenderer>();

                foreach (MeshRenderer MR in compositeRenderers)
                {
                    MR.material = mSmokeBox;
                }

                //gameObject.GetComponent<MeshRenderer>().material = mSmokeBox;
            }
            else if (random <= (iStunChance + iSmokeChance + iMedkitChance) && random > iMedkitChance + iSmokeChance)
            {
                eItem = Inventory.CURRENT_ITEM.STUNGRENADE;

                MeshRenderer[] compositeRenderers = gameObject.GetComponentsInChildren<MeshRenderer>();

                foreach (MeshRenderer MR in compositeRenderers)
                {
                    MR.material = mStunBox;
                }

                //gameObject.GetComponent<MeshRenderer>().material = mStunBox;
            }
            else if (random > (iSmokeChance + iMedkitChance + iStunChance))
            {
                eItem = Inventory.CURRENT_ITEM.DECOY;

                MeshRenderer[] compositeRenderers = gameObject.GetComponentsInChildren<MeshRenderer>();

                foreach (MeshRenderer MR in compositeRenderers)
                {
                    MR.material = mDecoyBox;
                }
            }

        }
    }
}
