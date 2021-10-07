using System.Collections;
using System.Collections.Generic;
using UnityEngine;

////////////////////////////////////////////////////////////
// File: Generator.cs
// Author: Cameron Lillie
// Brief: Script for the interactable generator
////////////////////////////////////////////////////////////

public class Generator : Interactable
{
    [SerializeField] private bool bPowered; //if false, power is off

    // Update is called once per frame
    void Update()
    {
        
    }

    public override void Interact()
    {
        Debug.Log("Generator Interacted");
        bPowered = !bPowered; //toggle power
    }

    public override void AIInteract(AI_Agent a_Agent)
    {
        if (!bPowered)
        {
            //Ai will reactivate the gnerator

            bPowered = true;
        }
    }

    public void SetPowered(bool powered)
    {
        bPowered = powered;
    }

    public bool GetPowered()
    {
        return bPowered;
    }
}