using System.Collections;
using System.Collections.Generic;
using UnityEngine;

////////////////////////////////////////////////////////////
// File: Interactable.cs
// Author: Cameron Lillie
// Brief: Parent class for interactable objects
////////////////////////////////////////////////////////////

public abstract class Interactable : MonoBehaviour
{
    [SerializeField] protected float fInteractTime;
    
    [SerializeField]
    protected bool bInteractable;

    [SerializeField]
    protected GameObject InteractTarget;

    private AI_Blackboard mBlackboard;

    // Start is called before the first frame update
    public virtual void Start()
    {
        try
        {
            mBlackboard = FindObjectOfType<AI_Blackboard>();
        }
        catch (System.Exception)
        {

            throw new System.Exception("No Blackboard found");
        }

        bInteractable = true;
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public abstract void Interact();

    public virtual void AIInteract(AI_Agent a_Agent)
    {

    }

    public void SetInteractTime(float time)
    {
        fInteractTime = time;
    }

    public float GetInteractTime()
    {
        return fInteractTime;
    }

    public void SetInteractable(bool interactable)
    {
        bInteractable = interactable;
    }

    public bool GetInteractable()
    {
        return bInteractable;
    }

    public void SetInteractTarget(GameObject a_targetObject)
    {
        InteractTarget = a_targetObject;
    }

    public GameObject GetTargetObject()
    {
        return InteractTarget;
    }

    public AI_Blackboard GetBlackBoard()
    {
        return mBlackboard;
    }
}
