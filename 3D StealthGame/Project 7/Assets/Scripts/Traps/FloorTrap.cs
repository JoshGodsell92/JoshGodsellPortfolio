using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FloorTrap : Interactable
{
    [SerializeField] bool bActive;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public override void Interact()
    {

    }

    public override void AIInteract(AI_Agent a_Agent)
    {
        base.AIInteract(a_Agent);
    }

    private void OnTriggerStay(Collider other)
    {
        if (bActive)
        {
            PlayerController player = other.gameObject.GetComponent<PlayerController>();
            if (player != null)
            {
                Debug.Log("Spike Trap");
                if (!player.GetOnSpikes())
                {
                    player.SetOnSpikes(true);
                }
            }
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (bActive)
        {
            PlayerController player = other.gameObject.GetComponent<PlayerController>();
            if (player != null)
            {
                Debug.Log("Left Spikes");
                if (player.GetOnSpikes())
                {
                    player.SetOnSpikes(false);
                }
            }
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
}
